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

namespace Tangra.StarCatalogues.UCAC2
{
    public class UCAC2BinIndexEntry
    {
        public readonly short ZoneId;
        public readonly uint TotalStarsInBin;
        public readonly uint LastStarNo;
        public readonly double RAFrom;
        public readonly double RATo;
        public readonly double DEFrom;
        public readonly double DETo;

        public UCAC2BinIndexEntry(BinaryReader rdr)
        {
            ZoneId = rdr.ReadInt16();
            TotalStarsInBin = rdr.ReadUInt32();
            LastStarNo = rdr.ReadUInt32();
            RAFrom = rdr.ReadInt32() / 3600000.0;
            RATo = rdr.ReadInt32() / 3600000.0;
            DEFrom = rdr.ReadInt32() / 3600000.0;
            DETo = rdr.ReadInt32() / 3600000.0;
        }
    }

    internal class LoadPosition
    {
        internal int ZoneId;
        internal uint FromRecordId;
        internal uint ToRecordId;
        internal uint FirstStarNoInBin;
        internal readonly bool BSS = false;

        public LoadPosition(UCAC2Index index, UCAC2BinIndexEntry entry, SearchZone zone)
            : this(index, entry, zone, false)
        { }

        public LoadPosition(UCAC2Index index, UCAC2BinIndexEntry entry, SearchZone zone, bool bss)
        {
            this.ZoneId = entry.ZoneId;
            FirstStarNoInBin = entry.LastStarNo - entry.TotalStarsInBin;

            if (bss)
            {
                if (zone.RADeciHoursFrom == 0)
                    this.FromRecordId = 1;
                else
                    this.FromRecordId = index.BSSRAIndexPerZone[entry.ZoneId - 1, zone.RADeciHoursFrom - 1] -
                                        FirstStarNoInBin;

                if (zone.RADeciHoursTo == 0)
                    this.ToRecordId = 1;
                else
                    this.ToRecordId = index.BSSRAIndexPerZone[entry.ZoneId - 1, zone.RADeciHoursTo - 1] -
                                      (entry.LastStarNo - entry.TotalStarsInBin);
            }
            else
            {
                if (zone.RADeciHoursFrom == 0)
                    this.FromRecordId = 1;
                else
                    this.FromRecordId = index.RAIndexPerZone[entry.ZoneId - 1, zone.RADeciHoursFrom - 1] -
                                        FirstStarNoInBin;

                if (zone.RADeciHoursTo == 0)
                    this.ToRecordId = 1;
                else
                    this.ToRecordId = index.RAIndexPerZone[entry.ZoneId - 1, zone.RADeciHoursTo - 1] -
                                      (entry.LastStarNo - entry.TotalStarsInBin);                
            }
            BSS = bss;
        }
    }

    public class UCAC2Index
    {
        private static UCAC2Index s_Index = null;

        public static UCAC2Index GetIndex(string ucac2Folder)
        {
            if (s_Index == null)
                s_Index = new UCAC2Index(ucac2Folder);

            return s_Index;
        }

        public uint[,] RAIndexPerZone = new uint[288, 240];
        public UCAC2BinIndexEntry[] ZoneIndex = new UCAC2BinIndexEntry[288];

        public uint[,] BSSRAIndexPerZone = new uint[36, 240];
        public UCAC2BinIndexEntry[] BSSZoneIndex = new UCAC2BinIndexEntry[36];

        private UCAC2Index(string ucac2Folder)
        {
            string indexFile = Path.Combine(ucac2Folder, "u2index.da");

            using(FileStream fs = new FileStream(indexFile, FileMode.Open, FileAccess.Read))
            {
                BinaryReader reader = new BinaryReader(fs);
                for (int i = 0; i < 288; i++)
                for (int j = 0; j < 240; j++)
                {
                    RAIndexPerZone[i, j] = reader.ReadUInt32();
                }

#if ASTROMETRY_DEBUG
                Trace.Assert(reader.BaseStream.Position == reader.BaseStream.Length);
#endif
            }

            int idx = -1;
            using (Stream data = AssemblyHelper.GetEmbededResourceStreamThatClientMustDispose("Tangra.StarCatalogues.UCAC2", "ucac2.idx"))
            {
                BinaryReader rdr = new BinaryReader(data);

                while (rdr.BaseStream.Position < rdr.BaseStream.Length)
                {
                    idx++;
                    ZoneIndex[idx] = new UCAC2BinIndexEntry(rdr);
                }
            }

            idx = -1;
            using (Stream data = AssemblyHelper.GetEmbededResourceStreamThatClientMustDispose("Tangra.StarCatalogues.UCAC2", "bss2.idx"))
            {
                BinaryReader rdr = new BinaryReader(data);

                while (rdr.BaseStream.Position < rdr.BaseStream.Length)
                {
                    idx++;
                    BSSZoneIndex[idx] = new UCAC2BinIndexEntry(rdr);
                }
            }

            using (Stream data = AssemblyHelper.GetEmbededResourceStreamThatClientMustDispose("Tangra.StarCatalogues.UCAC2", "bsindex.da"))
            {
                BinaryReader reader = new BinaryReader(data);
                for (int i = 0; i < 36; i++)
                for (int j = 0; j < 240; j++)
                {
                    BSSRAIndexPerZone[i, j] = reader.ReadUInt32();
                }

#if ASTROMETRY_DEBUG
                Trace.Assert(reader.BaseStream.Position == reader.BaseStream.Length);
#endif
            }
        }

        internal List<LoadPosition> GetLoadPositions(SearchZone zone)
        {
            List<LoadPosition> searchPositions = new List<LoadPosition>();

            UCAC2BinIndexEntry lastEntry = null;

            for (int i = 0; i < ZoneIndex.Length; i++)
            {
                UCAC2BinIndexEntry entry = ZoneIndex[i];

                if (zone.DEFrom > entry.DEFrom)
                {
                    lastEntry = entry;
                    continue;
                }

                if (lastEntry != null) entry = lastEntry;

                // Add one position
                LoadPosition pos = new LoadPosition(this, entry, zone);
                searchPositions.Add(pos);

                while (zone.DETo > entry.DETo)
                {
                    if (entry.ZoneId < ZoneIndex.Length)
                    {
                        entry = ZoneIndex[entry.ZoneId];
                        // Add another position
                        LoadPosition pos2 = new LoadPosition(this, entry, zone);
                        searchPositions.Add(pos2);
                    }
                }

                break;
            }

            lastEntry = null;
            for (int i = 0; i < BSSZoneIndex.Length; i++)
            {
                UCAC2BinIndexEntry entry = BSSZoneIndex[i];

                if (zone.DEFrom > entry.DEFrom)
                {
                    lastEntry = entry;
                    continue;
                }

                if (lastEntry != null) entry = lastEntry;

                // Add one position
                LoadPosition pos = new LoadPosition(this, entry, zone, true);
                searchPositions.Add(pos);

                while (zone.DETo > entry.DETo)
                {
                    if (entry.ZoneId < BSSZoneIndex.Length)
                    {
                        entry = BSSZoneIndex[entry.ZoneId];
                        // Add another position
                        LoadPosition pos2 = new LoadPosition(this, entry, zone, true);
                        searchPositions.Add(pos2);
                    }
                }

                break;
            }

            return searchPositions;
        }

    }
}
