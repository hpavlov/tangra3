/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Tangra.Model.Helpers;

namespace Tangra.StarCatalogues.UCAC3
{
    internal class UCAC3BinIndexEntry
    {
        public int[] RAStartPositions = new int[360];
        public readonly int ZoneId;
        public readonly int TotalStarsInZone;

        public double DEFrom
        {
            get { return (ZoneId - 1) * 0.5 - 90.0; }
        }

        public double DETo
        {
            get { return ZoneId * 0.5 - 90.0; }
        }

        internal UCAC3BinIndexEntry(int zoneId, int totalStarsInZone, BinaryReader rdr)
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

    internal class UCAC3Index
    {
        private string m_CatalogLocation;
        private static UCAC3Index s_UCAC3Index;
        private static object s_SyncRoot = new object();

        public UCAC3BinIndexEntry[] ZoneIndex = new UCAC3BinIndexEntry[360];

        private UCAC3Index(string catalogLocation)
        {
            m_CatalogLocation = catalogLocation;
            int idx = -1;
            using (Stream data = AssemblyHelper.GetEmbededResourceStreamThatClientMustDispose("Tangra.StarCatalogues.UCAC3", "ucac3.idx"))
            {
                BinaryReader rdr = new BinaryReader(data);

                while (rdr.BaseStream.Position < rdr.BaseStream.Length)
                {
                    idx++;
                    string zoneFile = Path.GetFullPath(string.Format("{0}\\z{1}", catalogLocation, (idx + 1).ToString("000")));
                    FileInfo fi = new FileInfo(zoneFile);
                    int numStarsInZone = (int)fi.Length / UCAC3Entry.Size;

                    ZoneIndex[idx] = new UCAC3BinIndexEntry(idx, numStarsInZone, rdr);
                }
            }            
        }

        public static UCAC3Index GetIndex(string catalogLocation)
        {
            if (s_UCAC3Index == null)
            {
                lock(s_SyncRoot)
                {
                    if (s_UCAC3Index == null)
                    {
                        s_UCAC3Index = new UCAC3Index(catalogLocation);
                    }
                }
            }

            return s_UCAC3Index;
        }

        public List<LoadPosition> GetLoadPositions(SearchZone zone)
        {
            List<LoadPosition> loadPositions = new List<LoadPosition>();

        	int deltaFrom = zone.DEFrom > 0 ? 1 : 0;
			int deltaTo = zone.DETo > 0 ? 1 : 0;

			int fromZone = Math.Max(1, (int)(zone.DEFrom * 2) + 180 + deltaFrom);
			int toZone = Math.Min(360, (int)(zone.DETo * 2) + 180 + deltaTo);

            for (int zoneId = fromZone; zoneId <= toZone; zoneId++)
            {
                LoadPosition loadPos = new LoadPosition();
                loadPos.ZoneId = zoneId;
                loadPos.FirstStarNoInBin = 0;
                int raFrom = Math.Max(0, (int)Math.Floor(zone.RAFrom));
                int raTo = Math.Min(359, (int)Math.Ceiling(zone.RATo));
                loadPos.FromRecordId = (uint)ZoneIndex[zoneId - 1].RAStartPositions[raFrom];
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
