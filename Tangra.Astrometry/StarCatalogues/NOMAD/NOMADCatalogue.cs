/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Tangra.Model.Config;
using Tangra.StarCatalogues;

namespace Tangra.StarCatalogues.NOMAD
{
    public class NOMADCatalogue
    {
        private string m_CatalogLocation;
        private NOMADIndex m_Index;

        public static bool IsValidCatalogLocation(ref string folderPath)
        {
            NOMADIndex index = NOMADIndex.GetIndex(folderPath);

            return File.Exists(index.GetCatalogFileForZone(933)); /* Random File */
        }

        private static CatalogMagnitudeBand CousinsRMagnitudeBand = new CatalogMagnitudeBand(NOMADEntry.BAND_ID_R, "Cousins R from USNOB or UCAC2");
        private static CatalogMagnitudeBand JohnsonVMagnitudeBand = new CatalogMagnitudeBand(NOMADEntry.BAND_ID_V, "Johnson V from YB6 or Tycho");
        private static CatalogMagnitudeBand JohnsonBMagnitudeBand = new CatalogMagnitudeBand(NOMADEntry.BAND_ID_B, "Johnson B from USNOB, YB6 or Tycho");

        public static CatalogMagnitudeBand[] CatalogMagnitudeBands = new CatalogMagnitudeBand[] { CousinsRMagnitudeBand, JohnsonVMagnitudeBand, JohnsonBMagnitudeBand };

        public static double ConvertMagnitude(double measuredMag, double vrColorIndex, Guid catalogMagBand, TangraConfig.MagOutputBand magOutputBand)
        {
            if (catalogMagBand == NOMADEntry.BAND_ID_R && magOutputBand == TangraConfig.MagOutputBand.CousinsR) return measuredMag;
            if (catalogMagBand == NOMADEntry.BAND_ID_R && magOutputBand == TangraConfig.MagOutputBand.JohnsonV) return ColourIndexTables.GetVFromRAndVR(measuredMag, vrColorIndex);

            if (catalogMagBand == NOMADEntry.BAND_ID_V && magOutputBand == TangraConfig.MagOutputBand.CousinsR) return ColourIndexTables.GetRFromVAndVR(measuredMag, vrColorIndex);
            if (catalogMagBand == NOMADEntry.BAND_ID_V && magOutputBand == TangraConfig.MagOutputBand.JohnsonV) return measuredMag;

            if (catalogMagBand == NOMADEntry.BAND_ID_B && magOutputBand == TangraConfig.MagOutputBand.CousinsR) return ColourIndexTables.GetRFromBAndVR(measuredMag, vrColorIndex);
            if (catalogMagBand == NOMADEntry.BAND_ID_B && magOutputBand == TangraConfig.MagOutputBand.JohnsonV) return ColourIndexTables.GetVFromBAndVR(measuredMag, vrColorIndex);

            return double.NaN;
        }

        public NOMADCatalogue(string catalogLocation)
        {
            m_CatalogLocation = catalogLocation;
            m_Index = NOMADIndex.GetIndex(catalogLocation);
        }

        public List<IStar> GetStarsInRegion(double raDeg, double deDeg, double radiusDeg, double limitMag)
        {
            return GetStarsInRegion(raDeg, deDeg, radiusDeg, limitMag, 2000.0f);
        }

        public List<IStar> GetStarsInRegion(double raDeg, double deDeg, double radiusDeg, double limitMag, float epoch)
        {
            List<SearchZone> searchZones = CatalogHelper.GetZones(raDeg, deDeg, radiusDeg);

            List<IStar> starsFromThisZone = new List<IStar>();

            foreach (SearchZone zone in searchZones)
            {
                LoadStars(zone, limitMag, starsFromThisZone);
            }

            NOMADEntry.TargetEpoch = epoch;
            return starsFromThisZone;
        }

        private void LoadStars(SearchZone zone, double limitMag, List<IStar> starsFromThisZone)
        {
            List<LoadPosition> loadPositions = m_Index.GetLoadPositions(zone);
            foreach (LoadPosition loadPos in loadPositions)
            {
                LoadStarsBrighterThan(zone, loadPos, limitMag, starsFromThisZone);
            }
        }

        private void LoadStarsBrighterThan(SearchZone zone, LoadPosition loadPos, double limitMag, List<IStar> starsFromThisZone)
        {
            IntPtr buffer = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NOMADEntry)));
            try
            {
                using (FileStream fs = new FileStream(m_Index.GetCatalogFileForZone(loadPos.ZoneId), FileMode.Open, FileAccess.Read))
                using (BinaryReader rdr = new BinaryReader(fs))
                {
#if ASTROMETRY_DEBUG
                    Trace.Assert(fs.Length >= (loadPos.FromRecordId + 1) * 88);
#endif
                    uint currStarInBin = loadPos.FromRecordId - 1;
                    fs.Position = 88 * loadPos.FromRecordId;

                    for (int i = 0; i < loadPos.ToRecordId - loadPos.FromRecordId; i++)
                    {
                        currStarInBin++;

#if ASTROMETRY_DEBUG
                        Trace.Assert(fs.Position  % 88 == 0);
#endif

                        fs.Seek(44, SeekOrigin.Current);
                        double mag = rdr.ReadInt32() / 1000.0; /* V Mag */
                        int byteDiff = 0;
                        if (mag == 30000.0) 
                        {
                            mag = rdr.ReadInt32() / 1000.0; /* R Mag */
                            byteDiff = 4;
                        }

                        if (mag > limitMag)
                        {
                            fs.Seek(40 - byteDiff, SeekOrigin.Current);
                            continue;
                        }

                        fs.Seek(-(48 + byteDiff), SeekOrigin.Current);
                        NOMADEntry entry = new NOMADEntry();
                        byte[] rawData = rdr.ReadBytes(NOMADEntry.Size);

                        Marshal.Copy(rawData, 0, buffer, Marshal.SizeOf(entry));
                        entry = (NOMADEntry)Marshal.PtrToStructure(buffer, typeof(NOMADEntry));

                        if (entry.RAJ2000 >= zone.RAFrom &&
                            entry.RAJ2000 <= zone.RATo)
                        {
                            entry.InitNOMADEntry(loadPos.FirstStarNoInBin + currStarInBin, (uint)loadPos.ZoneId, currStarInBin);
                            starsFromThisZone.Add(entry);
                        }
                    }
                }
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }

        public static string GetStarDesignation(uint starId)
        {
            return string.Format("{0}-{1}", (starId >> 32).ToString().PadLeft(4, '0'), (starId & 0xFFFFFFFF).ToString().PadLeft(7, '0'));
        }
    }
}
