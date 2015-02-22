/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Tangra.Model.Config;
using Tangra.StarCatalogues;

namespace Tangra.StarCatalogues.UCAC3
{
    public class UCAC3Catalogue
    {
        public static bool IsValidCatalogLocation(ref string folderPath)
        {
            if (CheckSingleFolder(folderPath))
                return true;

            return false;
        }

        private static bool CheckSingleFolder(string folderPath)
        {
            IEnumerator<string> ucac3Files = UCAC3FileIterator.UCAC3Files(folderPath);
            while (ucac3Files.MoveNext())
            {
                if (File.Exists(ucac3Files.Current))
                    // Some people may only have a partial version of the catalogue (e.g. Northern Hemisphere)
                    return true;
            }

            return false;
        }

        private static CatalogMagnitudeBand ModelFitMagnitudeBand = new CatalogMagnitudeBand(UCAC3Entry.BAND_ID_UNFILTERED, "Model Fit Magnitude (fMag)");
        private static CatalogMagnitudeBand JohnsonVFromModelFitMagnitudeBand = new CatalogMagnitudeBand(UCAC3Entry.BAND_ID_V, "Johnson V - Computed from fMag");
        private static CatalogMagnitudeBand CousinsRFromMagnitudeBand = new CatalogMagnitudeBand(UCAC3Entry.BAND_ID_R, "Cousins R - Computed from fMag");

        public static CatalogMagnitudeBand[] CatalogMagnitudeBands = new CatalogMagnitudeBand[] { ModelFitMagnitudeBand, JohnsonVFromModelFitMagnitudeBand, CousinsRFromMagnitudeBand };

        public static double ConvertMagnitude(double measuredMag, double vrColorIndex, Guid catalogMagBand, TangraConfig.MagOutputBand magOutputBand)
        {
            if (catalogMagBand == UCAC3Entry.BAND_ID_R && magOutputBand == TangraConfig.MagOutputBand.CousinsR) return measuredMag;
            if (catalogMagBand == UCAC3Entry.BAND_ID_R && magOutputBand == TangraConfig.MagOutputBand.JohnsonV) return ColourIndexTables.GetVFromRAndVR(measuredMag, vrColorIndex);

            if (catalogMagBand == UCAC3Entry.BAND_ID_V && magOutputBand == TangraConfig.MagOutputBand.CousinsR) return ColourIndexTables.GetRFromVAndVR(measuredMag, vrColorIndex);
            if (catalogMagBand == UCAC3Entry.BAND_ID_V && magOutputBand == TangraConfig.MagOutputBand.JohnsonV) return measuredMag;

            if (catalogMagBand == UCAC3Entry.BAND_ID_UNFILTERED)
            {
                double jk = ColourIndexTables.GetJKFromVR(vrColorIndex);
                if (magOutputBand == TangraConfig.MagOutputBand.CousinsR) return -0.295 * jk + 1.323 * measuredMag - 0.0377 * measuredMag * measuredMag + 0.001142 * measuredMag * measuredMag * measuredMag - 0.68;
                if (magOutputBand == TangraConfig.MagOutputBand.JohnsonV) return 0.552 * jk + 1.578 * measuredMag - 0.0560 * measuredMag * measuredMag + 0.001562 * measuredMag * measuredMag * measuredMag - 1.76;
            }

            return double.NaN;
        }

        private UCAC3Index m_Index;
        private string m_CatalogLocation;

        public UCAC3Catalogue(string catalogLocation)
        {
            m_CatalogLocation = catalogLocation;
            m_Index = UCAC3Index.GetIndex(catalogLocation.TrimEnd('\\'));
        }

        public List<IStar> GetStarsInRegion(double raDeg, double deDeg, double radiusDeg, double limitMag, float epoch)
        {
            List<SearchZone> searchZones = CatalogHelper.GetZones(raDeg, deDeg, radiusDeg);

            List<IStar> starsFromThisZone = new List<IStar>();

            foreach (SearchZone zone in searchZones)
            {
                LoadStars(zone, limitMag, starsFromThisZone);
            }

            UCAC3Entry.TargetEpoch = epoch;
            return starsFromThisZone;
        }

        private void LoadStars(SearchZone zone, double limitMag, List<IStar> starsFromThisZone)
        {
            List<LoadPosition> searchIndexes = m_Index.GetLoadPositions(zone);

            IntPtr buffer = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(UCAC3Entry)));
            try
            {
                foreach (LoadPosition pos in searchIndexes)
                {
                    string fileName;
                    fileName = Path.Combine(m_CatalogLocation, string.Format("z{0}", pos.ZoneId.ToString("000")));

                    long positionFrom = (pos.FromRecordId - 1) * UCAC3Entry.Size;
                    uint firstStarNoToRead = pos.FirstStarNoInBin + pos.FromRecordId;
                    uint numRecords = pos.ToRecordId - pos.FromRecordId;

                    using (FileStream str = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        using (BinaryReader rdr = new BinaryReader(str))
                        {
                            rdr.BaseStream.Position = positionFrom;

                            for (int i = 0; i < numRecords; i++, firstStarNoToRead++)
                            {
                                UCAC3Entry entry = new UCAC3Entry();
                                byte[] rawData = rdr.ReadBytes(UCAC3Entry.Size);

                                Marshal.Copy(rawData, 0, buffer, Marshal.SizeOf(entry));
                                entry = (UCAC3Entry)Marshal.PtrToStructure(buffer, typeof(UCAC3Entry));

                                entry.InitUCAC3Entry((ushort)pos.ZoneId, firstStarNoToRead);

                                if (entry.Mag > limitMag) continue;

                                if (entry.RAJ2000 < zone.RAFrom) continue;
                                if (entry.RAJ2000 > zone.RATo) continue;
                                if (entry.DEJ2000 < zone.DEFrom) continue;
                                if (entry.DEJ2000 > zone.DETo) continue;

                                starsFromThisZone.Add(entry);
                            }
                        }
                    }
                }
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }

        public UCAC3Entry GetStarBy3U(int zone, int record)
        {
            IntPtr buffer = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(UCAC3Entry)));
            try
            {
                string fileName;
                fileName = Path.Combine(m_CatalogLocation, string.Format("z{0}", zone.ToString("000")));

                long positionFrom = (record - 1) * UCAC3Entry.Size;

                using (FileStream str = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (BinaryReader rdr = new BinaryReader(str))
                    {
                        rdr.BaseStream.Position = positionFrom;

                        UCAC3Entry entry = new UCAC3Entry();
                        byte[] rawData = rdr.ReadBytes(UCAC3Entry.Size);

                        Marshal.Copy(rawData, 0, buffer, Marshal.SizeOf(entry));
                        entry = (UCAC3Entry)Marshal.PtrToStructure(buffer, typeof(UCAC3Entry));

                        entry.InitUCAC3Entry((ushort)zone, 0);

                        return entry;
                    }
                }
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }
    }
}
