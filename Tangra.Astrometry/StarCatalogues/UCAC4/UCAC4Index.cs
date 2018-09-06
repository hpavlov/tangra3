/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Model.Helpers;

namespace Tangra.StarCatalogues.UCAC4
{
	internal class UCAC4BinIndexEntry
	{
		public int[] RAStartPositions = new int[360];
		public readonly int ZoneId;
		public readonly int TotalStarsInZone;

		public double DEFrom
		{
			get { return (ZoneId - 1) * 0.2 - 90.0; }
		}

		public double DETo
		{
			get { return ZoneId * 0.2 - 90.0; }
		}

		internal UCAC4BinIndexEntry(int zoneId, int totalStarsInZone, BinaryReader rdr)
		{
			ZoneId = zoneId + 1;
			TotalStarsInZone = totalStarsInZone;

			for (int i = 0; i < 360; i++)
			{
				int startIdx = rdr.ReadInt32();
				RAStartPositions[i] = startIdx;
			}
		}
	}

    internal class UCAC4Index
    {
        private string m_CatalogLocation;
        private static UCAC4Index s_UCAC4Index;
        private static object s_SyncRoot = new object();

		public UCAC4BinIndexEntry[] ZoneIndex = new UCAC4BinIndexEntry[900];

        private UCAC4Index(string catalogLocation)
        {
            m_CatalogLocation = catalogLocation;

			int idx = -1;
			using (Stream data = AssemblyHelper.GetEmbededResourceStreamThatClientMustDispose("Tangra.StarCatalogues.UCAC4", "ucac4.idx"))
			{
				BinaryReader rdr = new BinaryReader(data);

				while (rdr.BaseStream.Position < rdr.BaseStream.Length)
				{
					idx++;
					string zoneFile = Path.GetFullPath(string.Format("{0}\\z{1}", catalogLocation, (idx + 1).ToString("000")));
					FileInfo fi = new FileInfo(zoneFile);
				    if (!fi.Exists)
				    {
                        MessageBox.Show(null, string.Format("Cannnot find UCAC4 file '{0}'\r\nPlease ensure that you have all necessary catalogue files!", zoneFile), "Tangra", MessageBoxButtons.OK, MessageBoxIcon.Error);
				    }
					int numStarsInZone = (int)fi.Length / UCAC4Entry.Size;

					ZoneIndex[idx] = new UCAC4BinIndexEntry(idx, numStarsInZone, rdr);
				}
			}            
        }

        public static UCAC4Index GetIndex(string catalogLocation)
        {
            if (s_UCAC4Index == null)
            {
                lock (s_SyncRoot)
                {
                    if (s_UCAC4Index == null)
                    {
                        s_UCAC4Index = new UCAC4Index(catalogLocation);
                    }
                }
            }

            return s_UCAC4Index;
        }

		public List<LoadPosition> GetLoadPositions(SearchZone zone)
		{
			List<LoadPosition> loadPositions = new List<LoadPosition>();

			int deltaFrom = zone.DEFrom > 0 ? 1 : 0;
			int deltaTo = zone.DETo > 0 ? 1 : 0;

			int fromZone = Math.Max(1, (int)(zone.DEFrom * 5) + 90 * 5 + deltaFrom);
			int toZone = Math.Min(900, (int)(zone.DETo * 5) + 90 * 5 + deltaTo);

			for (int zoneId = fromZone; zoneId <= toZone; zoneId++)
			{
				LoadPosition loadPos = new LoadPosition();
				loadPos.ZoneId = zoneId;
				loadPos.FirstStarNoInBin = 0;
				int raFrom = Math.Max(0, (int)Math.Floor(zone.RAFrom));
				int raTo = Math.Min(360, (int)Math.Ceiling(zone.RATo) + 1);
				loadPos.FromRecordId = (uint)ZoneIndex[zoneId - 1].RAStartPositions[raFrom];
				if (raTo == 360)
					loadPos.ToRecordId = (uint)ZoneIndex[zoneId - 1].TotalStarsInZone;
				else
					loadPos.ToRecordId = (uint)ZoneIndex[zoneId - 1].RAStartPositions[raTo];

				loadPositions.Add(loadPos);
			}

			return loadPositions;
		}
    }

	internal class LoadPosition
	{
		internal int ZoneId;
		internal uint FromRecordId;
		internal uint ToRecordId;
		internal uint FirstStarNoInBin;
	}
}
