/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Tangra.StarCatalogues.PPMXL
{
    public class PPMXLIndex
    {
        internal List<PPMXLIndexEntry> ZoneIndex;

        private PPMXLIndex(List<int[]> zoneIndex)
        {
            ZoneIndex = new List<PPMXLIndexEntry>();

            for (int i = 0; i < zoneIndex.Count; i++)
            {
                var entry = new PPMXLIndexEntry()
                                {
                                    RAIndex = zoneIndex[i],
                                    UniqueZone = i
                                };

                if (i < 360)
                {
                    entry.SubZoneId = "dcba"[i % 4];
                    entry.Hemisphere = 's';
                    entry.ZoneId = 89 - (i / 4);
                }
                else
                {
                    entry.SubZoneId = "abcd"[i % 4];
                    entry.Hemisphere = 'n';
                    entry.ZoneId = (i / 4) - 90;
                }

                ZoneIndex.Add(entry);
            }
        }

        public static PPMXLIndex GetIndex(string catalogLocation)
        {
            string indexFileName = Path.GetFullPath(catalogLocation + @"\zones.idx");

            List<int[]> zoneIndex = new List<int[]>();

            using (FileStream fs = new FileStream(indexFileName, FileMode.Open, FileAccess.Read))
            using (BinaryReader rdr = new BinaryReader(fs))
            {
                for (int i = 0; i < 4 * 180; i++)
                {
                    int[] idx = new int[360];

                    for (int j = 0; j < 360; j++)
                    {
                        idx[j] = rdr.ReadInt32();
                    }

                    zoneIndex.Add(idx);
                }
            }

            return new PPMXLIndex(zoneIndex);
        }

        public static void BuildFullIndex(string catalogLocation)
        {
            int idx = -1;
            IEnumerator<string> filesEnu = PPMXLFileIterator.PPMXLFiles(catalogLocation);
            string fileNameIndex = catalogLocation + "zones.idx";

            if (File.Exists(fileNameIndex))
            {
                MessageBox.Show(string.Format("Index file {0} already exists.", fileNameIndex));
                return;
            }

            using (FileStream fs = new FileStream(fileNameIndex, FileMode.Create, FileAccess.Write))
            using (BinaryWriter bwrt = new BinaryWriter(fs))
            {
                while (filesEnu.MoveNext())
                {
                    string fileName = filesEnu.Current;
                    Console.WriteLine(fileName);
                    idx++;

                    int[] raIdx = new int[360];
                    int raCurr = 0;
                    int recNo = -1;

                    using (FileStream fileStr = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                    using (BinaryReader rdr = new BinaryReader(fileStr))
                    {
                        byte[] header = rdr.ReadBytes(174);
                        while (true)
                        {
                            recNo++;

                            byte[] data = rdr.ReadBytes(174);
                            if (data == null || data.Length == 0) break;

                            PPMXLEntry entry = new PPMXLEntry(Encoding.ASCII.GetString(data));
                            int raTrunc = (int)entry.RADeg;

                            while (raTrunc >= raCurr)
                            {
                                //Console.WriteLine("Index[" + raCurr.ToString() + "] = " + recNo.ToString());
                                if (raCurr < 360) raIdx[raCurr] = recNo;
                                raCurr++;
                            }
                        }
                    }

                    while (360 > raCurr)
                    {
                        //Console.WriteLine("Index[" + raCurr.ToString() + "] = " + (recNo - 1).ToString());
                        raIdx[raCurr] = recNo - 1;
                        raCurr++;
                    }

                    for (int j = 0; j < 360; j++)
                    {
                        bwrt.Write(raIdx[j]);
                    }
                }

                bwrt.Flush();
            }
        }

        internal List<LoadPosition> GetLoadPositions(SearchZone zone)
        {
            List<LoadPosition> loadPositions = new List<LoadPosition>();

            int signDEFrom = Math.Sign(zone.DEFrom);

            int fromZone, toZone;
            bool isSouth = signDEFrom < 0;

            fromZone = (int)Math.Floor(((zone.DEFrom + 90) * 4));
            toZone = (int)Math.Ceiling(((zone.DETo + 90) * 4));

            for (int zoneId = fromZone; zoneId < toZone; zoneId++)
            {
                LoadPosition loadPos = new LoadPosition();

                if (zoneId == 359)
                {
                    loadPos.ZoneId = 0;
                    loadPos.SubZoneId = 'a';
                    loadPos.Hemisphere = 's';
                }
                else if (zoneId == 360)
                {
                    loadPos.ZoneId = 0;
                    loadPos.SubZoneId = 'a';
                    loadPos.Hemisphere = 'n';                        
                }
                else
                {
                    if (isSouth)
                    {
                        loadPos.ZoneId = 89 - (zoneId / 4);
                        loadPos.SubZoneId = "dcba"[zoneId % 4];
                        loadPos.Hemisphere = 's';
                    }
                    else
                    {
                        loadPos.ZoneId = (zoneId / 4) - 90;
                        loadPos.SubZoneId = "abcd"[zoneId % 4];
                        loadPos.Hemisphere = 'n';
                    }                        
                }

                int raFrom = Math.Max(0, (int)Math.Floor(zone.RAFrom));
                int raTo = Math.Min(359, (int)Math.Ceiling(zone.RATo));

                loadPos.FromRecordId = (uint)ZoneIndex[zoneId].RAIndex[raFrom];
                loadPos.ToRecordId = (uint)ZoneIndex[zoneId].RAIndex[raTo];

                loadPositions.Add(loadPos);
            }

            return loadPositions;
        }
    }

    public class PPMXLIndexEntry
    {
        internal int ZoneId;
        internal char SubZoneId;
        internal char Hemisphere;
        internal int UniqueZone;

        public int[] RAIndex;

        public string FileName
        {
            get { return string.Concat(Hemisphere, ZoneId.ToString("00"), SubZoneId, ".dat"); }
        }

        public double DEFrom
        {
            get
            {
                if (Hemisphere == 's')
                {
                    return (-1 * (ZoneId + 1) * 4 + "dcba".IndexOf(SubZoneId)) / 4.0;
                }
                else
                {
                    return (ZoneId * 4 + "abcd".IndexOf(SubZoneId)) / 4.0;
                }
            }
        }

        public double DETo
        {
            get { return DEFrom + 0.25; }
        }
    }

    internal class LoadPosition
    {
        internal int ZoneId;
        internal char SubZoneId;
        internal char Hemisphere;
        internal uint FromRecordId;
        internal uint ToRecordId;
    }
}
