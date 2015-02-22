/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Tangra.Model.Helpers;

namespace Tangra.StarCatalogues.NOMAD
{
    internal struct LoadPosition
    {
        internal int ZoneId;
        internal uint FromRecordId;
        internal uint ToRecordId;
        internal uint FirstStarNoInBin;

        internal LoadPosition(int zone, uint recordFrom, uint recordTo, uint firstStarNo)
        {
            ZoneId = zone;
            FromRecordId = recordFrom;
            ToRecordId = recordTo;
            FirstStarNoInBin = firstStarNo;
        }
    }

    internal class NOMADZoneIndex
    {
        internal uint[] RAIndex = new uint[1801];

        internal NOMADZoneIndex(string catalogLocation, int zone)
        {
            string indexFileName = string.Format(@"{0}\{1}\m{2}.inx", catalogLocation, (zone / 10).ToString().PadLeft(3, '0'), zone.ToString().PadLeft(4, '0'));
            using(FileStream fs = new FileStream(indexFileName, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader rdr = new BinaryReader(fs))
                {
                    for (int i = 0; i < 1801; i++)
                    {
                        RAIndex[i] = rdr.ReadUInt32();
                    }
                }
            }
        }
    }

    internal class NOMADIndex
    {
        private Dictionary<int, NOMADZoneIndex> m_LoadedZoneIndexes = new Dictionary<int, NOMADZoneIndex>();
        private Dictionary<int, string> m_FilePathsPerZone = new Dictionary<int, string>();

        private uint[] m_NOMADFirstStarNoPerBin = new uint[1801];
        private string m_CatalogLocation;

        internal static NOMADIndex GetIndex(string catalogLocation)
        {
            return new NOMADIndex(catalogLocation);
        }

        private NOMADIndex(string catalogLocation)
        {
            m_CatalogLocation = catalogLocation.TrimEnd('\\');

			using (Stream data = AssemblyHelper.GetEmbededResourceStreamThatClientMustDispose("Tangra.StarCatalogues.NOMAD", "nomad.idx"))
            {
                BinaryReader rdr = new BinaryReader(data);
                for (int i = 0; i < 1800; i++)
                {
                    m_NOMADFirstStarNoPerBin[i] = rdr.ReadUInt32();
                }

                m_NOMADFirstStarNoPerBin[1800] = 1117612732;
            }
        }

        internal List<LoadPosition> GetLoadPositions(SearchZone zone)
        {
            List<LoadPosition> loadPositions = new List<LoadPosition>();

            int dirIdxFrom = Math.Min(Math.Max((int) Math.Floor((zone.DEFrom + 90) * 10), 0), 1799);
            int dirIdxTo = Math.Max(Math.Min((int)Math.Ceiling((zone.DETo + 90) * 10) - 1, 1799), 0);

            int raIdxFrom = Math.Max((int)Math.Floor((zone.RAFrom * 5)), 0);
            int rsIdxTo = Math.Min((int)Math.Ceiling((zone.RATo * 5)) + 1, 1800);

            for (int i = dirIdxFrom; i <= dirIdxTo; i++)
            {
                NOMADZoneIndex zoneIdx = GetZoneIndex(i);

                uint idxFrom = zoneIdx.RAIndex[raIdxFrom];
                uint idxTo = zoneIdx.RAIndex[rsIdxTo];

                if (idxTo > idxFrom)
                    loadPositions.Add(new LoadPosition(i, idxFrom, idxTo, m_NOMADFirstStarNoPerBin[i]));
                else if (idxTo < idxFrom)
                {
#if ASTROMETRY_DEBUG
                    Trace.Assert(false);
#endif
                }
            }

            return loadPositions;
        }

        internal NOMADZoneIndex GetZoneIndex(int zone)
        {
            NOMADZoneIndex index;
            if (!m_LoadedZoneIndexes.TryGetValue(zone, out index))
            {
                index = new NOMADZoneIndex(m_CatalogLocation, zone);
                index.RAIndex[1800] = m_NOMADFirstStarNoPerBin[zone + 1] - m_NOMADFirstStarNoPerBin[zone];
                m_LoadedZoneIndexes.Add(zone, index);
            }

            return index;
        }

        internal string GetCatalogFileForZone(int zone)
        {
            string path;
            if (!m_FilePathsPerZone.TryGetValue(zone, out path))
            {
                path = string.Format(@"{0}\{1}\m{2}.cat", m_CatalogLocation, (zone / 10).ToString().PadLeft(3, '0'), zone.ToString().PadLeft(4, '0'));
                m_FilePathsPerZone.Add(zone, path);
            }
            return path;  
        }
    }


}
