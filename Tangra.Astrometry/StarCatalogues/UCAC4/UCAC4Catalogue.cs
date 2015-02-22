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

namespace Tangra.StarCatalogues.UCAC4
{
    public class UCAC4Catalogue
    {
        public static bool IsValidCatalogLocation(ref string folderPath)
        {
			if (CheckSingleFolder(folderPath))
				return true;

			string u2Path = Path.GetFullPath(Path.Combine(folderPath, "u4b"));
			if (CheckSingleFolder(u2Path))
			{
				folderPath = u2Path;
				return true;
			}

			return false;
		}

        private static bool CheckSingleFolder(string folderPath)
        {
			IEnumerator<string> ucac4Files = UCAC4FileIterator.UCAC4Files(folderPath);
			while (ucac4Files.MoveNext())
			{
				if (File.Exists(ucac4Files.Current))
					// Some people may only have a partial version of the catalogue (e.g. Northern Hemisphere)
					return true;
			}

			return false;
        }

        private static CatalogMagnitudeBand ModelFitMagnitudeBand = new CatalogMagnitudeBand(UCAC4Entry.BAND_ID_UNFILTERED, "Model Fit Magnitude (fMag)");
        private static CatalogMagnitudeBand JohnsonVMagnitudeBand = new CatalogMagnitudeBand(UCAC4Entry.BAND_ID_V, "Johnson V (APASS)");
        private static CatalogMagnitudeBand CousinsRFromMagnitudeBand = new CatalogMagnitudeBand(UCAC4Entry.BAND_ID_R, "Cousins R - Computed from APASS V and B");
        private static CatalogMagnitudeBand SloanRMagnitudeBand = new CatalogMagnitudeBand(UCAC4Entry.BAND_ID_SLOAN_r, "SLOAN r (APASS)");
        private static CatalogMagnitudeBand SloanGMagnitudeBand = new CatalogMagnitudeBand(UCAC4Entry.BAND_ID_SLOAN_r, "SLOAN g (APASS)");

        public static CatalogMagnitudeBand[] CatalogMagnitudeBands = new CatalogMagnitudeBand[] { ModelFitMagnitudeBand, SloanRMagnitudeBand, SloanGMagnitudeBand, JohnsonVMagnitudeBand, CousinsRFromMagnitudeBand };

		public static double ConvertMagnitude(double measuredMag, double vrColorIndex, Guid catalogMagBand, TangraConfig.MagOutputBand magOutputBand)
		{
			if (catalogMagBand == UCAC4Entry.BAND_ID_R && magOutputBand == TangraConfig.MagOutputBand.CousinsR) return measuredMag;
			if (catalogMagBand == UCAC4Entry.BAND_ID_R && magOutputBand == TangraConfig.MagOutputBand.JohnsonV) return ColourIndexTables.GetVFromRAndVR(measuredMag, vrColorIndex);

			if (catalogMagBand == UCAC4Entry.BAND_ID_V && magOutputBand == TangraConfig.MagOutputBand.CousinsR) return ColourIndexTables.GetRFromVAndVR(measuredMag, vrColorIndex);
			if (catalogMagBand == UCAC4Entry.BAND_ID_V && magOutputBand == TangraConfig.MagOutputBand.JohnsonV) return measuredMag;

			// V = r + 0.44 * (g - r) - 0.02
			// B = V + 1.04 * (g - r) + 0.19
			// R = V - 0.508 * (B - V) - 0.040   (0.3 < B - V < 0.9) 


			// V - R = 0.508 * (B - V) + 0.040
			// B = V + ((V - R) - 0.040) / 0.508 = V + 1.04 * (g - r) + 0.19 => 1.04 * (g - r) = ((V - R) - 0.040) / 0.508 - 0.19 => (g - r) = (((V - R) - 0.040) / 0.508 - 0.19) / 1.04
			double grColorIndex = (((vrColorIndex) - 0.040) / 0.508 - 0.19) / 1.04;


			// R = r + 0.44 * (g - r) - 0.02 - 0.508 * (1.04 * (g - r) + 0.19) - 0.040 = r + (0.44 - 0.508 * 1.04)*(g-r) + (-0.060 - 0.19 * 0.508)
			if (catalogMagBand == UCAC4Entry.BAND_ID_SLOAN_r && magOutputBand == TangraConfig.MagOutputBand.CousinsR) return measuredMag + (0.44 - 0.508 * 1.04) * grColorIndex + (-0.060 - 0.19 * 0.508);
			// V = r + 0.44 * (g - r) - 0.02
			if (catalogMagBand == UCAC4Entry.BAND_ID_SLOAN_r && magOutputBand == TangraConfig.MagOutputBand.JohnsonV) return measuredMag + 0.44 * grColorIndex - 0.02;


			if (catalogMagBand == UCAC4Entry.BAND_ID_SLOAN_g && magOutputBand == TangraConfig.MagOutputBand.CousinsR)
				throw new NotImplementedException();

			// V = r + 0.44 * (g - r) - 0.02 = 0.44 * g + 0.56 * r - 0.02
			// B = V + 1.04 * g - 1.04 * r + 0.19 | *0.56 / 1.04 => 0.56 * (B - V) / 1.04 = 0.56 * g - 0.56 * r + 0.56 * 0.19 / 1.04
			// (1.04 * V + 0.56 * B - 0.56 * V) / 1.04 = g + 0.56 * 0.16/1.04 - 0.02 => 0.48 * V + 0.56 * B = 1.04 * g + 0.0688 => V = 2.17 * g - 1.17 * B + 0.14
			if (catalogMagBand == UCAC4Entry.BAND_ID_SLOAN_g && magOutputBand == TangraConfig.MagOutputBand.JohnsonV)
				throw new NotImplementedException();

			if (catalogMagBand == UCAC4Entry.BAND_ID_UNFILTERED)
			{
				throw new NotImplementedException("Need to determine the formulas for converting from fitMag to V and R from LONEOS?");
				//double jk = ColourIndexTables.GetJKFromVR(vrColorIndex);
				//if (magOutputBand == MagOutputBand.CousinsR) return -0.295 * jk + 1.323 * measuredMag - 0.0377 * measuredMag * measuredMag + 0.001142 * measuredMag * measuredMag * measuredMag - 0.68;
				//if (magOutputBand == MagOutputBand.JohnsonV) return 0.552 * jk + 1.578 * measuredMag - 0.0560 * measuredMag * measuredMag + 0.001562 * measuredMag * measuredMag * measuredMag - 1.76;
			}

			return double.NaN;
		}

        private UCAC4Index m_Index;
        private string m_CatalogLocation;

        public UCAC4Catalogue(string catalogLocation)
        {
            m_CatalogLocation = catalogLocation;
            m_Index = UCAC4Index.GetIndex(catalogLocation.TrimEnd('\\'));
        }

        public List<IStar> GetStarsInRegion(double raDeg, double deDeg, double radiusDeg, double limitMag, float epoch)
        {
			List<SearchZone> searchZones = CatalogHelper.GetZones(raDeg, deDeg, radiusDeg);

			List<IStar> starsFromThisZone = new List<IStar>();

			foreach (SearchZone zone in searchZones)
			{
				LoadStars(zone, limitMag, starsFromThisZone);
			}

			UCAC4Entry.TargetEpoch = epoch;
			return starsFromThisZone;
        }

		private void LoadStars(SearchZone zone, double limitMag, List<IStar> starsFromThisZone)
		{
			List<LoadPosition> searchIndexes = m_Index.GetLoadPositions(zone);

			IntPtr buffer = Marshal.AllocHGlobal(UCAC4Entry.Size);
			try
			{
				foreach (LoadPosition pos in searchIndexes)
				{
					string fileName;
					fileName = Path.Combine(m_CatalogLocation, string.Format("z{0}", pos.ZoneId.ToString("000")));

					long positionFrom = (pos.FromRecordId - 1) * UCAC4Entry.Size;
					uint firstStarNoToRead = pos.FirstStarNoInBin + pos.FromRecordId;
					uint numRecords = pos.ToRecordId - pos.FromRecordId;

					using (FileStream str = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
					{
						using (BinaryReader rdr = new BinaryReader(str))
						{
							rdr.BaseStream.Position = positionFrom;

							for (int i = 0; i <= numRecords; i++, firstStarNoToRead++)
							{
								UCAC4Entry entry = new UCAC4Entry();
								byte[] rawData = rdr.ReadBytes(UCAC4Entry.Size);

								Marshal.Copy(rawData, 0, buffer, UCAC4Entry.Size);
								entry = (UCAC4Entry)Marshal.PtrToStructure(buffer, typeof(UCAC4Entry));

								entry.InitUCAC4Entry((ushort)pos.ZoneId, firstStarNoToRead);

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
    }
}
