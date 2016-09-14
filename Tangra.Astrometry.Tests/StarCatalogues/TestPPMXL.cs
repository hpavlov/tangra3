/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using NUnit.Framework;
using Tangra.Model.Helpers;
using Tangra.StarCatalogues;
using Tangra.StarCatalogues.PPMXL;

namespace Tangra.Tests.StarCatalogues
{
	#if !AUTOMATED_BUILD 
    [TestFixture]
    public class TestPPMXL
    {
        private string CATALOG_LOCATION = @"Z:\STAR CATALOGS\PPMXL\DATA\";

        [Test()]
        public void _1_TestFirstEntry()
        {
            string fileName = Path.GetFullPath(CATALOG_LOCATION + "n89d.dat");
            using(FileStream fileStr = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            using (BinaryReader rdr = new BinaryReader(fileStr))
            {
                byte[] header = rdr.ReadBytes(174);
                byte[] firstLine = rdr.ReadBytes(174);

                PPMXLEntry entry = new PPMXLEntry(Encoding.ASCII.GetString(firstLine));

                //#PPMXL_ID          |   RA   (ICRS)Dec        pmRA     pmDE|  epRA    epDE  eRA eDE epma epmd|  Jmag:+/-     Hmag:+/-     Kmag:+/-  |b1mag b2mag r1mag r2mag  imag|bbrri|No|fl
                // 480381039136370330|000.031470+89.788187    +10.4     -6.7|1985.06 1985.06  89  89  4.2  4.2|16.229:0.126 15.708:0.180 15.497:0.223|19.73 20.28 17.96 18.32 17.35|02137| 6| 0

                Assert.AreEqual(0.031470, entry.RADeg, 0.00000001);
                Assert.AreEqual(89.788187, entry.DEDeg, 0.00000001);
                Assert.AreEqual(10.4, entry.pmRA * Math.Cos(entry.DEDeg * Math.PI / 180), 0.0001);
                Assert.AreEqual(-6.7, entry.pmDE, 0.0001);

                Assert.AreEqual(16.229, entry.MagJ, 0.0001);
                Assert.AreEqual(15.497, entry.MagK, 0.0001);

                Assert.AreEqual((19.73 + 20.28) / 2, entry.MagB, 0.0001);
                Assert.AreEqual((17.96 + 18.32) / 2, entry.MagR, 0.0001);
            }
        }

        [Test()]
        public void BuildRAIndex_TakesHourAndAHalfToBuild()
        {
            PPMXLIndex.BuildFullIndex(CATALOG_LOCATION); 
        }

        [Test()]
        public void LoadRAIndex()
        {
            PPMXLIndex index = PPMXLIndex.GetIndex(CATALOG_LOCATION);
            Assert.IsNotNull(index);
        }


        [Test]
        public void _3_Test_PPMXL_Index()
        {
            PPMXLIndex index = PPMXLIndex.GetIndex(CATALOG_LOCATION);
            Assert.IsNotNull(index);

            for (int i = 0; i < 2*360; i++)
            {
                PPMXLIndexEntry zoneIndex = index.ZoneIndex[i];
                Assert.AreEqual(zoneIndex.UniqueZone, i);

                string fileName = Path.Combine(CATALOG_LOCATION, zoneIndex.FileName);

                using (FileStream str = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (BinaryReader rdr = new BinaryReader(str))
                    {
                        byte[] header = rdr.ReadBytes(174);
                        byte[] firstLine = rdr.ReadBytes(174);

                        PPMXLEntry firstEntry = new PPMXLEntry(Encoding.ASCII.GetString(firstLine));

                        Console.WriteLine(string.Format("#{0} Zone:{1} 1stStarDE:{2} ZoneDERange[{3} | {4}] {5}",
                                                        i + 1,
                                                        zoneIndex.FileName,
                                                        AstroConvert.ToStringValue(firstEntry.DEDeg, "DEC"),
                                                        AstroConvert.ToStringValue(zoneIndex.DEFrom, "DEC"),
                                                        AstroConvert.ToStringValue(zoneIndex.DETo, "DEC"),
                                                        zoneIndex.ZoneId));

                        //Console.WriteLine(string.Format("{0}, AstroConvert.ToDeclination(\"{1}\"), AstroConvert.ToDeclination(\"{2}\"),", zoneIndex.UniqueZone, AstroConvert.ToStringValue(zoneIndex.DEFrom, "+DD:MM:SS"), AstroConvert.ToStringValue(zoneIndex.DETo, "+DD:MM:SS")));

                        Assert.IsTrue(firstEntry.DEDeg >= zoneIndex.DEFrom,
                                      string.Format("firstEntry.DECat ({0}) >= zoneIndex.DEFrom ({1})", firstEntry.DEDeg,
                                                    zoneIndex.DEFrom));
                        Assert.IsTrue(firstEntry.DEDeg <= zoneIndex.DETo,
                                      string.Format("firstEntry.DECat ({0}) <= zoneIndex.DETo ({1})", firstEntry.DEDeg,
                                                    zoneIndex.DETo));

                        for (int j = 0; j < 360; j++)
                        {
                            double entryMinOutsideRA = j - (1.0/36000);
                            double entryMaxOutsideRA = j + 1;
                            double entryMinInsideRA = j;

                            int minEntryIndex = zoneIndex.RAIndex[j];
                            int nextRAZoneIndex = j < 359 ? zoneIndex.RAIndex[j + 1] : minEntryIndex;

                            int prevEntryIndex = minEntryIndex - 1;

                            rdr.BaseStream.Position = PPMXLEntry.Size*(minEntryIndex + 1);
                            byte[] data = rdr.ReadBytes(174);

                            PPMXLEntry minEntry = new PPMXLEntry(Encoding.ASCII.GetString(data));

                            if (!(minEntry.RADeg > entryMinOutsideRA))
                            {
                                Assert.Fail();
                            }

                            if (!(nextRAZoneIndex == minEntryIndex || minEntry.RADeg < entryMaxOutsideRA))
                            {
                                Assert.Fail();
                            }

                            if (prevEntryIndex >= 0)
                            {
                                rdr.BaseStream.Position = PPMXLEntry.Size*(prevEntryIndex + 1);
                                data = rdr.ReadBytes(174);

                                PPMXLEntry prevEntry = new PPMXLEntry(Encoding.ASCII.GetString(data));

                                if (!(prevEntry.RADeg < entryMinInsideRA))
                                {
                                    Assert.Fail();
                                }
                            }
                        }
                    }
                }
            }
        }

        [Test()]
        public void _4_ReadStars_1()
        {
            PPMXLCatalogue cat = new PPMXLCatalogue(CATALOG_LOCATION);
            List<IStar> satrs = cat.GetStarsInRegion(12.34, -51.32, 0.2, 16, 2000);

            Assert.IsNotNull(satrs);
            Assert.IsTrue(satrs.Count > 0);

            foreach (PPMXLEntry star in satrs)
            {
                Assert.IsTrue(star.RAJ2000 >= 12.34 - 0.25);
                Assert.IsTrue(star.RAJ2000 <= 12.34 + 0.25);
                Assert.IsTrue(star.DEJ2000 >= -51.32 - 0.25);
                Assert.IsTrue(star.DEJ2000 <= -51.32 + 0.25);
                Assert.IsTrue(star.Mag <= 16);
                Console.WriteLine(star.GetStarDesignation(0) + "\t\t" + star.GetStarDesignation(1));
            }
        }

        [Test()]
        public void _4_ReadStars_2_Equator()
        {
            PPMXLCatalogue cat = new PPMXLCatalogue(CATALOG_LOCATION);
            List<IStar> satrs = cat.GetStarsInRegion(12.34, -0.02, 0.2, 16, 2000);

            Assert.IsNotNull(satrs);
            Assert.IsTrue(satrs.Count > 0);

            foreach (PPMXLEntry star in satrs)
            {
                Assert.IsTrue(star.RAJ2000 >= 12.34 - 0.25);
                Assert.IsTrue(star.RAJ2000 <= 12.34 + 0.25);
                Assert.IsTrue(star.DEJ2000 >= -0.02 - 0.25);
                Assert.IsTrue(star.DEJ2000 <= -0.02 + 0.25);
                Assert.IsTrue(star.Mag <= 16);
                Console.WriteLine(star.GetStarDesignation(0) + "\t\t" + star.GetStarDesignation(1));
            }
        }

        [Test()]
        public void _5_TestLoadPositions()
        {
            PPMXLIndex index = PPMXLIndex.GetIndex(CATALOG_LOCATION);

            for (int i = -90 * 4; i < 90 * 4; i++)
            {
                double zoneDeclFrom = i / 4.0;
                double zoneDeclTo = (i + 1) / 4.0;

                double declFrom = i / 4.0 + 0.1;
                double declTo = i / 4.0 + 0.2;
                double raFrom = 1;
                double raTo = 3;



                int fromZone = (int)Math.Floor(((declFrom + 90) * 4));
                int toZone = (int)Math.Ceiling(((declTo + 90) * 4)) - 1;

                Assert.AreEqual(fromZone, toZone);

                var zone = new SearchZone()
                {
                    RAFrom = raFrom,
                    RATo = raTo,
                    DEFrom = declFrom,
                    DETo = declTo
                };

                List<LoadPosition> loadPositions = index.GetLoadPositions(zone);

                foreach (LoadPosition pos in loadPositions)
                {
                    string fileName;
                    fileName = Path.Combine(CATALOG_LOCATION, string.Format("{0}{1}{2}.dat", pos.Hemisphere, pos.ZoneId.ToString("00"), pos.SubZoneId));

                    long firstPosition = (pos.FromRecordId + 1 /* for the header row */) * PPMXLEntry.Size;
                    long lastPosition = (pos.ToRecordId - 1 + 1 /* for the header row */) * PPMXLEntry.Size;

                    Console.Write(string.Format("{0}{1}{2} ...", pos.Hemisphere, pos.ZoneId.ToString("00"),pos.SubZoneId));

                    using (FileStream fileStr = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                    using (BinaryReader rdr = new BinaryReader(fileStr))
                    {
                        rdr.BaseStream.Position = firstPosition;
                        byte[] data = rdr.ReadBytes(PPMXLEntry.Size);
                        PPMXLEntry entry = new PPMXLEntry(Encoding.ASCII.GetString(data));

                        Assert.IsTrue(entry.DEDeg >= zoneDeclFrom);
                        Assert.IsTrue(entry.DEDeg < zoneDeclTo);
                        Assert.IsTrue(entry.RADeg >= raFrom);
                        Assert.IsTrue(entry.RADeg < raTo);

                        rdr.BaseStream.Position = lastPosition;
                        data = rdr.ReadBytes(PPMXLEntry.Size);
                        entry = new PPMXLEntry(Encoding.ASCII.GetString(data));

                        Assert.IsTrue(entry.DEDeg >= zoneDeclFrom);
                        Assert.IsTrue(entry.DEDeg < zoneDeclTo);
                        Assert.IsTrue(entry.RADeg >= raFrom);
                        Assert.IsTrue(entry.RADeg < raTo);
                    }

                    Console.WriteLine("Ok.");
                }
            }
            
        }

        //[Test]
        //public void _5_Test_Zone_Ranges()
        //{
        //    PPMXLIndex index = PPMXLIndex.GetIndex(CATALOG_LOCATION);
        //    Assert.IsNotNull(index);


        //    #region RANGES

        //    double[] RANGES = new double[]
        //    {
        //        0, AstroConvert.ToDeclination("-90:00:00"), AstroConvert.ToDeclination("-89:45:00"),
        //        1, AstroConvert.ToDeclination("-89:45:00"), AstroConvert.ToDeclination("-89:30:00"),
        //        2, AstroConvert.ToDeclination("-89:30:00"), AstroConvert.ToDeclination("-89:15:00"),
        //        3, AstroConvert.ToDeclination("-89:15:00"), AstroConvert.ToDeclination("-89:00:00"),
        //        4, AstroConvert.ToDeclination("-89:00:00"), AstroConvert.ToDeclination("-88:45:00"),
        //        5, AstroConvert.ToDeclination("-88:45:00"), AstroConvert.ToDeclination("-88:30:00"),
        //        6, AstroConvert.ToDeclination("-88:30:00"), AstroConvert.ToDeclination("-88:15:00"),
        //        7, AstroConvert.ToDeclination("-88:15:00"), AstroConvert.ToDeclination("-88:00:00"),
        //        8, AstroConvert.ToDeclination("-88:00:00"), AstroConvert.ToDeclination("-87:45:00"),
        //        9, AstroConvert.ToDeclination("-87:45:00"), AstroConvert.ToDeclination("-87:30:00"),
        //        10, AstroConvert.ToDeclination("-87:30:00"), AstroConvert.ToDeclination("-87:15:00"),
        //        11, AstroConvert.ToDeclination("-87:15:00"), AstroConvert.ToDeclination("-87:00:00"),
        //        12, AstroConvert.ToDeclination("-87:00:00"), AstroConvert.ToDeclination("-86:45:00"),
        //        13, AstroConvert.ToDeclination("-86:45:00"), AstroConvert.ToDeclination("-86:30:00"),
        //        14, AstroConvert.ToDeclination("-86:30:00"), AstroConvert.ToDeclination("-86:15:00"),
        //        15, AstroConvert.ToDeclination("-86:15:00"), AstroConvert.ToDeclination("-86:00:00"),
        //        16, AstroConvert.ToDeclination("-86:00:00"), AstroConvert.ToDeclination("-85:45:00"),
        //        17, AstroConvert.ToDeclination("-85:45:00"), AstroConvert.ToDeclination("-85:30:00"),
        //        18, AstroConvert.ToDeclination("-85:30:00"), AstroConvert.ToDeclination("-85:15:00"),
        //        19, AstroConvert.ToDeclination("-85:15:00"), AstroConvert.ToDeclination("-85:00:00"),
        //        20, AstroConvert.ToDeclination("-85:00:00"), AstroConvert.ToDeclination("-84:45:00"),
        //        21, AstroConvert.ToDeclination("-84:45:00"), AstroConvert.ToDeclination("-84:30:00"),
        //        22, AstroConvert.ToDeclination("-84:30:00"), AstroConvert.ToDeclination("-84:15:00"),
        //        23, AstroConvert.ToDeclination("-84:15:00"), AstroConvert.ToDeclination("-84:00:00"),
        //        24, AstroConvert.ToDeclination("-84:00:00"), AstroConvert.ToDeclination("-83:45:00"),
        //        25, AstroConvert.ToDeclination("-83:45:00"), AstroConvert.ToDeclination("-83:30:00"),
        //        26, AstroConvert.ToDeclination("-83:30:00"), AstroConvert.ToDeclination("-83:15:00"),
        //        27, AstroConvert.ToDeclination("-83:15:00"), AstroConvert.ToDeclination("-83:00:00"),
        //        28, AstroConvert.ToDeclination("-83:00:00"), AstroConvert.ToDeclination("-82:45:00"),
        //        29, AstroConvert.ToDeclination("-82:45:00"), AstroConvert.ToDeclination("-82:30:00"),
        //        30, AstroConvert.ToDeclination("-82:30:00"), AstroConvert.ToDeclination("-82:15:00"),
        //        31, AstroConvert.ToDeclination("-82:15:00"), AstroConvert.ToDeclination("-82:00:00"),
        //        32, AstroConvert.ToDeclination("-82:00:00"), AstroConvert.ToDeclination("-81:45:00"),
        //        33, AstroConvert.ToDeclination("-81:45:00"), AstroConvert.ToDeclination("-81:30:00"),
        //        34, AstroConvert.ToDeclination("-81:30:00"), AstroConvert.ToDeclination("-81:15:00"),
        //        35, AstroConvert.ToDeclination("-81:15:00"), AstroConvert.ToDeclination("-81:00:00"),
        //        36, AstroConvert.ToDeclination("-81:00:00"), AstroConvert.ToDeclination("-80:45:00"),
        //        37, AstroConvert.ToDeclination("-80:45:00"), AstroConvert.ToDeclination("-80:30:00"),
        //        38, AstroConvert.ToDeclination("-80:30:00"), AstroConvert.ToDeclination("-80:15:00"),
        //        39, AstroConvert.ToDeclination("-80:15:00"), AstroConvert.ToDeclination("-80:00:00"),
        //        40, AstroConvert.ToDeclination("-80:00:00"), AstroConvert.ToDeclination("-79:45:00"),
        //        41, AstroConvert.ToDeclination("-79:45:00"), AstroConvert.ToDeclination("-79:30:00"),
        //        42, AstroConvert.ToDeclination("-79:30:00"), AstroConvert.ToDeclination("-79:15:00"),
        //        43, AstroConvert.ToDeclination("-79:15:00"), AstroConvert.ToDeclination("-79:00:00"),
        //        44, AstroConvert.ToDeclination("-79:00:00"), AstroConvert.ToDeclination("-78:45:00"),
        //        45, AstroConvert.ToDeclination("-78:45:00"), AstroConvert.ToDeclination("-78:30:00"),
        //        46, AstroConvert.ToDeclination("-78:30:00"), AstroConvert.ToDeclination("-78:15:00"),
        //        47, AstroConvert.ToDeclination("-78:15:00"), AstroConvert.ToDeclination("-78:00:00"),
        //        48, AstroConvert.ToDeclination("-78:00:00"), AstroConvert.ToDeclination("-77:45:00"),
        //        49, AstroConvert.ToDeclination("-77:45:00"), AstroConvert.ToDeclination("-77:30:00"),
        //        50, AstroConvert.ToDeclination("-77:30:00"), AstroConvert.ToDeclination("-77:15:00"),
        //        51, AstroConvert.ToDeclination("-77:15:00"), AstroConvert.ToDeclination("-77:00:00"),
        //        52, AstroConvert.ToDeclination("-77:00:00"), AstroConvert.ToDeclination("-76:45:00"),
        //        53, AstroConvert.ToDeclination("-76:45:00"), AstroConvert.ToDeclination("-76:30:00"),
        //        54, AstroConvert.ToDeclination("-76:30:00"), AstroConvert.ToDeclination("-76:15:00"),
        //        55, AstroConvert.ToDeclination("-76:15:00"), AstroConvert.ToDeclination("-76:00:00"),
        //        56, AstroConvert.ToDeclination("-76:00:00"), AstroConvert.ToDeclination("-75:45:00"),
        //        57, AstroConvert.ToDeclination("-75:45:00"), AstroConvert.ToDeclination("-75:30:00"),
        //        58, AstroConvert.ToDeclination("-75:30:00"), AstroConvert.ToDeclination("-75:15:00"),
        //        59, AstroConvert.ToDeclination("-75:15:00"), AstroConvert.ToDeclination("-75:00:00"),
        //        60, AstroConvert.ToDeclination("-75:00:00"), AstroConvert.ToDeclination("-74:45:00"),
        //        61, AstroConvert.ToDeclination("-74:45:00"), AstroConvert.ToDeclination("-74:30:00"),
        //        62, AstroConvert.ToDeclination("-74:30:00"), AstroConvert.ToDeclination("-74:15:00"),
        //        63, AstroConvert.ToDeclination("-74:15:00"), AstroConvert.ToDeclination("-74:00:00"),
        //        64, AstroConvert.ToDeclination("-74:00:00"), AstroConvert.ToDeclination("-73:45:00"),
        //        65, AstroConvert.ToDeclination("-73:45:00"), AstroConvert.ToDeclination("-73:30:00"),
        //        66, AstroConvert.ToDeclination("-73:30:00"), AstroConvert.ToDeclination("-73:15:00"),
        //        67, AstroConvert.ToDeclination("-73:15:00"), AstroConvert.ToDeclination("-73:00:00"),
        //        68, AstroConvert.ToDeclination("-73:00:00"), AstroConvert.ToDeclination("-72:45:00"),
        //        69, AstroConvert.ToDeclination("-72:45:00"), AstroConvert.ToDeclination("-72:30:00"),
        //        70, AstroConvert.ToDeclination("-72:30:00"), AstroConvert.ToDeclination("-72:15:00"),
        //        71, AstroConvert.ToDeclination("-72:15:00"), AstroConvert.ToDeclination("-72:00:00"),
        //        72, AstroConvert.ToDeclination("-72:00:00"), AstroConvert.ToDeclination("-71:45:00"),
        //        73, AstroConvert.ToDeclination("-71:45:00"), AstroConvert.ToDeclination("-71:30:00"),
        //        74, AstroConvert.ToDeclination("-71:30:00"), AstroConvert.ToDeclination("-71:15:00"),
        //        75, AstroConvert.ToDeclination("-71:15:00"), AstroConvert.ToDeclination("-71:00:00"),
        //        76, AstroConvert.ToDeclination("-71:00:00"), AstroConvert.ToDeclination("-70:45:00"),
        //        77, AstroConvert.ToDeclination("-70:45:00"), AstroConvert.ToDeclination("-70:30:00"),
        //        78, AstroConvert.ToDeclination("-70:30:00"), AstroConvert.ToDeclination("-70:15:00"),
        //        79, AstroConvert.ToDeclination("-70:15:00"), AstroConvert.ToDeclination("-70:00:00"),
        //        80, AstroConvert.ToDeclination("-70:00:00"), AstroConvert.ToDeclination("-69:45:00"),
        //        81, AstroConvert.ToDeclination("-69:45:00"), AstroConvert.ToDeclination("-69:30:00"),
        //        82, AstroConvert.ToDeclination("-69:30:00"), AstroConvert.ToDeclination("-69:15:00"),
        //        83, AstroConvert.ToDeclination("-69:15:00"), AstroConvert.ToDeclination("-69:00:00"),
        //        84, AstroConvert.ToDeclination("-69:00:00"), AstroConvert.ToDeclination("-68:45:00"),
        //        85, AstroConvert.ToDeclination("-68:45:00"), AstroConvert.ToDeclination("-68:30:00"),
        //        86, AstroConvert.ToDeclination("-68:30:00"), AstroConvert.ToDeclination("-68:15:00"),
        //        87, AstroConvert.ToDeclination("-68:15:00"), AstroConvert.ToDeclination("-68:00:00"),
        //        88, AstroConvert.ToDeclination("-68:00:00"), AstroConvert.ToDeclination("-67:45:00"),
        //        89, AstroConvert.ToDeclination("-67:45:00"), AstroConvert.ToDeclination("-67:30:00"),
        //        90, AstroConvert.ToDeclination("-67:30:00"), AstroConvert.ToDeclination("-67:15:00"),
        //        91, AstroConvert.ToDeclination("-67:15:00"), AstroConvert.ToDeclination("-67:00:00"),
        //        92, AstroConvert.ToDeclination("-67:00:00"), AstroConvert.ToDeclination("-66:45:00"),
        //        93, AstroConvert.ToDeclination("-66:45:00"), AstroConvert.ToDeclination("-66:30:00"),
        //        94, AstroConvert.ToDeclination("-66:30:00"), AstroConvert.ToDeclination("-66:15:00"),
        //        95, AstroConvert.ToDeclination("-66:15:00"), AstroConvert.ToDeclination("-66:00:00"),
        //        96, AstroConvert.ToDeclination("-66:00:00"), AstroConvert.ToDeclination("-65:45:00"),
        //        97, AstroConvert.ToDeclination("-65:45:00"), AstroConvert.ToDeclination("-65:30:00"),
        //        98, AstroConvert.ToDeclination("-65:30:00"), AstroConvert.ToDeclination("-65:15:00"),
        //        99, AstroConvert.ToDeclination("-65:15:00"), AstroConvert.ToDeclination("-65:00:00"),
        //        100, AstroConvert.ToDeclination("-65:00:00"), AstroConvert.ToDeclination("-64:45:00"),
        //        101, AstroConvert.ToDeclination("-64:45:00"), AstroConvert.ToDeclination("-64:30:00"),
        //        102, AstroConvert.ToDeclination("-64:30:00"), AstroConvert.ToDeclination("-64:15:00"),
        //        103, AstroConvert.ToDeclination("-64:15:00"), AstroConvert.ToDeclination("-64:00:00"),
        //        104, AstroConvert.ToDeclination("-64:00:00"), AstroConvert.ToDeclination("-63:45:00"),
        //        105, AstroConvert.ToDeclination("-63:45:00"), AstroConvert.ToDeclination("-63:30:00"),
        //        106, AstroConvert.ToDeclination("-63:30:00"), AstroConvert.ToDeclination("-63:15:00"),
        //        107, AstroConvert.ToDeclination("-63:15:00"), AstroConvert.ToDeclination("-63:00:00"),
        //        108, AstroConvert.ToDeclination("-63:00:00"), AstroConvert.ToDeclination("-62:45:00"),
        //        109, AstroConvert.ToDeclination("-62:45:00"), AstroConvert.ToDeclination("-62:30:00"),
        //        110, AstroConvert.ToDeclination("-62:30:00"), AstroConvert.ToDeclination("-62:15:00"),
        //        111, AstroConvert.ToDeclination("-62:15:00"), AstroConvert.ToDeclination("-62:00:00"),
        //        112, AstroConvert.ToDeclination("-62:00:00"), AstroConvert.ToDeclination("-61:45:00"),
        //        113, AstroConvert.ToDeclination("-61:45:00"), AstroConvert.ToDeclination("-61:30:00"),
        //        114, AstroConvert.ToDeclination("-61:30:00"), AstroConvert.ToDeclination("-61:15:00"),
        //        115, AstroConvert.ToDeclination("-61:15:00"), AstroConvert.ToDeclination("-61:00:00"),
        //        116, AstroConvert.ToDeclination("-61:00:00"), AstroConvert.ToDeclination("-60:45:00"),
        //        117, AstroConvert.ToDeclination("-60:45:00"), AstroConvert.ToDeclination("-60:30:00"),
        //        118, AstroConvert.ToDeclination("-60:30:00"), AstroConvert.ToDeclination("-60:15:00"),
        //        119, AstroConvert.ToDeclination("-60:15:00"), AstroConvert.ToDeclination("-60:00:00"),
        //        120, AstroConvert.ToDeclination("-60:00:00"), AstroConvert.ToDeclination("-59:45:00"),
        //        121, AstroConvert.ToDeclination("-59:45:00"), AstroConvert.ToDeclination("-59:30:00"),
        //        122, AstroConvert.ToDeclination("-59:30:00"), AstroConvert.ToDeclination("-59:15:00"),
        //        123, AstroConvert.ToDeclination("-59:15:00"), AstroConvert.ToDeclination("-59:00:00"),
        //        124, AstroConvert.ToDeclination("-59:00:00"), AstroConvert.ToDeclination("-58:45:00"),
        //        125, AstroConvert.ToDeclination("-58:45:00"), AstroConvert.ToDeclination("-58:30:00"),
        //        126, AstroConvert.ToDeclination("-58:30:00"), AstroConvert.ToDeclination("-58:15:00"),
        //        127, AstroConvert.ToDeclination("-58:15:00"), AstroConvert.ToDeclination("-58:00:00"),
        //        128, AstroConvert.ToDeclination("-58:00:00"), AstroConvert.ToDeclination("-57:45:00"),
        //        129, AstroConvert.ToDeclination("-57:45:00"), AstroConvert.ToDeclination("-57:30:00"),
        //        130, AstroConvert.ToDeclination("-57:30:00"), AstroConvert.ToDeclination("-57:15:00"),
        //        131, AstroConvert.ToDeclination("-57:15:00"), AstroConvert.ToDeclination("-57:00:00"),
        //        132, AstroConvert.ToDeclination("-57:00:00"), AstroConvert.ToDeclination("-56:45:00"),
        //        133, AstroConvert.ToDeclination("-56:45:00"), AstroConvert.ToDeclination("-56:30:00"),
        //        134, AstroConvert.ToDeclination("-56:30:00"), AstroConvert.ToDeclination("-56:15:00"),
        //        135, AstroConvert.ToDeclination("-56:15:00"), AstroConvert.ToDeclination("-56:00:00"),
        //        136, AstroConvert.ToDeclination("-56:00:00"), AstroConvert.ToDeclination("-55:45:00"),
        //        137, AstroConvert.ToDeclination("-55:45:00"), AstroConvert.ToDeclination("-55:30:00"),
        //        138, AstroConvert.ToDeclination("-55:30:00"), AstroConvert.ToDeclination("-55:15:00"),
        //        139, AstroConvert.ToDeclination("-55:15:00"), AstroConvert.ToDeclination("-55:00:00"),
        //        140, AstroConvert.ToDeclination("-55:00:00"), AstroConvert.ToDeclination("-54:45:00"),
        //        141, AstroConvert.ToDeclination("-54:45:00"), AstroConvert.ToDeclination("-54:30:00"),
        //        142, AstroConvert.ToDeclination("-54:30:00"), AstroConvert.ToDeclination("-54:15:00"),
        //        143, AstroConvert.ToDeclination("-54:15:00"), AstroConvert.ToDeclination("-54:00:00"),
        //        144, AstroConvert.ToDeclination("-54:00:00"), AstroConvert.ToDeclination("-53:45:00"),
        //        145, AstroConvert.ToDeclination("-53:45:00"), AstroConvert.ToDeclination("-53:30:00"),
        //        146, AstroConvert.ToDeclination("-53:30:00"), AstroConvert.ToDeclination("-53:15:00"),
        //        147, AstroConvert.ToDeclination("-53:15:00"), AstroConvert.ToDeclination("-53:00:00"),
        //        148, AstroConvert.ToDeclination("-53:00:00"), AstroConvert.ToDeclination("-52:45:00"),
        //        149, AstroConvert.ToDeclination("-52:45:00"), AstroConvert.ToDeclination("-52:30:00"),
        //        150, AstroConvert.ToDeclination("-52:30:00"), AstroConvert.ToDeclination("-52:15:00"),
        //        151, AstroConvert.ToDeclination("-52:15:00"), AstroConvert.ToDeclination("-52:00:00"),
        //        152, AstroConvert.ToDeclination("-52:00:00"), AstroConvert.ToDeclination("-51:45:00"),
        //        153, AstroConvert.ToDeclination("-51:45:00"), AstroConvert.ToDeclination("-51:30:00"),
        //        154, AstroConvert.ToDeclination("-51:30:00"), AstroConvert.ToDeclination("-51:15:00"),
        //        155, AstroConvert.ToDeclination("-51:15:00"), AstroConvert.ToDeclination("-51:00:00"),
        //        156, AstroConvert.ToDeclination("-51:00:00"), AstroConvert.ToDeclination("-50:45:00"),
        //        157, AstroConvert.ToDeclination("-50:45:00"), AstroConvert.ToDeclination("-50:30:00"),
        //        158, AstroConvert.ToDeclination("-50:30:00"), AstroConvert.ToDeclination("-50:15:00"),
        //        159, AstroConvert.ToDeclination("-50:15:00"), AstroConvert.ToDeclination("-50:00:00"),
        //        160, AstroConvert.ToDeclination("-50:00:00"), AstroConvert.ToDeclination("-49:45:00"),
        //        161, AstroConvert.ToDeclination("-49:45:00"), AstroConvert.ToDeclination("-49:30:00"),
        //        162, AstroConvert.ToDeclination("-49:30:00"), AstroConvert.ToDeclination("-49:15:00"),
        //        163, AstroConvert.ToDeclination("-49:15:00"), AstroConvert.ToDeclination("-49:00:00"),
        //        164, AstroConvert.ToDeclination("-49:00:00"), AstroConvert.ToDeclination("-48:45:00"),
        //        165, AstroConvert.ToDeclination("-48:45:00"), AstroConvert.ToDeclination("-48:30:00"),
        //        166, AstroConvert.ToDeclination("-48:30:00"), AstroConvert.ToDeclination("-48:15:00"),
        //        167, AstroConvert.ToDeclination("-48:15:00"), AstroConvert.ToDeclination("-48:00:00"),
        //        168, AstroConvert.ToDeclination("-48:00:00"), AstroConvert.ToDeclination("-47:45:00"),
        //        169, AstroConvert.ToDeclination("-47:45:00"), AstroConvert.ToDeclination("-47:30:00"),
        //        170, AstroConvert.ToDeclination("-47:30:00"), AstroConvert.ToDeclination("-47:15:00"),
        //        171, AstroConvert.ToDeclination("-47:15:00"), AstroConvert.ToDeclination("-47:00:00"),
        //        172, AstroConvert.ToDeclination("-47:00:00"), AstroConvert.ToDeclination("-46:45:00"),
        //        173, AstroConvert.ToDeclination("-46:45:00"), AstroConvert.ToDeclination("-46:30:00"),
        //        174, AstroConvert.ToDeclination("-46:30:00"), AstroConvert.ToDeclination("-46:15:00"),
        //        175, AstroConvert.ToDeclination("-46:15:00"), AstroConvert.ToDeclination("-46:00:00"),
        //        176, AstroConvert.ToDeclination("-46:00:00"), AstroConvert.ToDeclination("-45:45:00"),
        //        177, AstroConvert.ToDeclination("-45:45:00"), AstroConvert.ToDeclination("-45:30:00"),
        //        178, AstroConvert.ToDeclination("-45:30:00"), AstroConvert.ToDeclination("-45:15:00"),
        //        179, AstroConvert.ToDeclination("-45:15:00"), AstroConvert.ToDeclination("-45:00:00"),
        //        180, AstroConvert.ToDeclination("-45:00:00"), AstroConvert.ToDeclination("-44:45:00"),
        //        181, AstroConvert.ToDeclination("-44:45:00"), AstroConvert.ToDeclination("-44:30:00"),
        //        182, AstroConvert.ToDeclination("-44:30:00"), AstroConvert.ToDeclination("-44:15:00"),
        //        183, AstroConvert.ToDeclination("-44:15:00"), AstroConvert.ToDeclination("-44:00:00"),
        //        184, AstroConvert.ToDeclination("-44:00:00"), AstroConvert.ToDeclination("-43:45:00"),
        //        185, AstroConvert.ToDeclination("-43:45:00"), AstroConvert.ToDeclination("-43:30:00"),
        //        186, AstroConvert.ToDeclination("-43:30:00"), AstroConvert.ToDeclination("-43:15:00"),
        //        187, AstroConvert.ToDeclination("-43:15:00"), AstroConvert.ToDeclination("-43:00:00"),
        //        188, AstroConvert.ToDeclination("-43:00:00"), AstroConvert.ToDeclination("-42:45:00"),
        //        189, AstroConvert.ToDeclination("-42:45:00"), AstroConvert.ToDeclination("-42:30:00"),
        //        190, AstroConvert.ToDeclination("-42:30:00"), AstroConvert.ToDeclination("-42:15:00"),
        //        191, AstroConvert.ToDeclination("-42:15:00"), AstroConvert.ToDeclination("-42:00:00"),
        //        192, AstroConvert.ToDeclination("-42:00:00"), AstroConvert.ToDeclination("-41:45:00"),
        //        193, AstroConvert.ToDeclination("-41:45:00"), AstroConvert.ToDeclination("-41:30:00"),
        //        194, AstroConvert.ToDeclination("-41:30:00"), AstroConvert.ToDeclination("-41:15:00"),
        //        195, AstroConvert.ToDeclination("-41:15:00"), AstroConvert.ToDeclination("-41:00:00"),
        //        196, AstroConvert.ToDeclination("-41:00:00"), AstroConvert.ToDeclination("-40:45:00"),
        //        197, AstroConvert.ToDeclination("-40:45:00"), AstroConvert.ToDeclination("-40:30:00"),
        //        198, AstroConvert.ToDeclination("-40:30:00"), AstroConvert.ToDeclination("-40:15:00"),
        //        199, AstroConvert.ToDeclination("-40:15:00"), AstroConvert.ToDeclination("-40:00:00"),
        //        200, AstroConvert.ToDeclination("-40:00:00"), AstroConvert.ToDeclination("-39:45:00"),
        //        201, AstroConvert.ToDeclination("-39:45:00"), AstroConvert.ToDeclination("-39:30:00"),
        //        202, AstroConvert.ToDeclination("-39:30:00"), AstroConvert.ToDeclination("-39:15:00"),
        //        203, AstroConvert.ToDeclination("-39:15:00"), AstroConvert.ToDeclination("-39:00:00"),
        //        204, AstroConvert.ToDeclination("-39:00:00"), AstroConvert.ToDeclination("-38:45:00"),
        //        205, AstroConvert.ToDeclination("-38:45:00"), AstroConvert.ToDeclination("-38:30:00"),
        //        206, AstroConvert.ToDeclination("-38:30:00"), AstroConvert.ToDeclination("-38:15:00"),
        //        207, AstroConvert.ToDeclination("-38:15:00"), AstroConvert.ToDeclination("-38:00:00"),
        //        208, AstroConvert.ToDeclination("-38:00:00"), AstroConvert.ToDeclination("-37:45:00"),
        //        209, AstroConvert.ToDeclination("-37:45:00"), AstroConvert.ToDeclination("-37:30:00"),
        //        210, AstroConvert.ToDeclination("-37:30:00"), AstroConvert.ToDeclination("-37:15:00"),
        //        211, AstroConvert.ToDeclination("-37:15:00"), AstroConvert.ToDeclination("-37:00:00"),
        //        212, AstroConvert.ToDeclination("-37:00:00"), AstroConvert.ToDeclination("-36:45:00"),
        //        213, AstroConvert.ToDeclination("-36:45:00"), AstroConvert.ToDeclination("-36:30:00"),
        //        214, AstroConvert.ToDeclination("-36:30:00"), AstroConvert.ToDeclination("-36:15:00"),
        //        215, AstroConvert.ToDeclination("-36:15:00"), AstroConvert.ToDeclination("-36:00:00"),
        //        216, AstroConvert.ToDeclination("-36:00:00"), AstroConvert.ToDeclination("-35:45:00"),
        //        217, AstroConvert.ToDeclination("-35:45:00"), AstroConvert.ToDeclination("-35:30:00"),
        //        218, AstroConvert.ToDeclination("-35:30:00"), AstroConvert.ToDeclination("-35:15:00"),
        //        219, AstroConvert.ToDeclination("-35:15:00"), AstroConvert.ToDeclination("-35:00:00"),
        //        220, AstroConvert.ToDeclination("-35:00:00"), AstroConvert.ToDeclination("-34:45:00"),
        //        221, AstroConvert.ToDeclination("-34:45:00"), AstroConvert.ToDeclination("-34:30:00"),
        //        222, AstroConvert.ToDeclination("-34:30:00"), AstroConvert.ToDeclination("-34:15:00"),
        //        223, AstroConvert.ToDeclination("-34:15:00"), AstroConvert.ToDeclination("-34:00:00"),
        //        224, AstroConvert.ToDeclination("-34:00:00"), AstroConvert.ToDeclination("-33:45:00"),
        //        225, AstroConvert.ToDeclination("-33:45:00"), AstroConvert.ToDeclination("-33:30:00"),
        //        226, AstroConvert.ToDeclination("-33:30:00"), AstroConvert.ToDeclination("-33:15:00"),
        //        227, AstroConvert.ToDeclination("-33:15:00"), AstroConvert.ToDeclination("-33:00:00"),
        //        228, AstroConvert.ToDeclination("-33:00:00"), AstroConvert.ToDeclination("-32:45:00"),
        //        229, AstroConvert.ToDeclination("-32:45:00"), AstroConvert.ToDeclination("-32:30:00"),
        //        230, AstroConvert.ToDeclination("-32:30:00"), AstroConvert.ToDeclination("-32:15:00"),
        //        231, AstroConvert.ToDeclination("-32:15:00"), AstroConvert.ToDeclination("-32:00:00"),
        //        232, AstroConvert.ToDeclination("-32:00:00"), AstroConvert.ToDeclination("-31:45:00"),
        //        233, AstroConvert.ToDeclination("-31:45:00"), AstroConvert.ToDeclination("-31:30:00"),
        //        234, AstroConvert.ToDeclination("-31:30:00"), AstroConvert.ToDeclination("-31:15:00"),
        //        235, AstroConvert.ToDeclination("-31:15:00"), AstroConvert.ToDeclination("-31:00:00"),
        //        236, AstroConvert.ToDeclination("-31:00:00"), AstroConvert.ToDeclination("-30:45:00"),
        //        237, AstroConvert.ToDeclination("-30:45:00"), AstroConvert.ToDeclination("-30:30:00"),
        //        238, AstroConvert.ToDeclination("-30:30:00"), AstroConvert.ToDeclination("-30:15:00"),
        //        239, AstroConvert.ToDeclination("-30:15:00"), AstroConvert.ToDeclination("-30:00:00"),
        //        240, AstroConvert.ToDeclination("-30:00:00"), AstroConvert.ToDeclination("-29:45:00"),
        //        241, AstroConvert.ToDeclination("-29:45:00"), AstroConvert.ToDeclination("-29:30:00"),
        //        242, AstroConvert.ToDeclination("-29:30:00"), AstroConvert.ToDeclination("-29:15:00"),
        //        243, AstroConvert.ToDeclination("-29:15:00"), AstroConvert.ToDeclination("-29:00:00"),
        //        244, AstroConvert.ToDeclination("-29:00:00"), AstroConvert.ToDeclination("-28:45:00"),
        //        245, AstroConvert.ToDeclination("-28:45:00"), AstroConvert.ToDeclination("-28:30:00"),
        //        246, AstroConvert.ToDeclination("-28:30:00"), AstroConvert.ToDeclination("-28:15:00"),
        //        247, AstroConvert.ToDeclination("-28:15:00"), AstroConvert.ToDeclination("-28:00:00"),
        //        248, AstroConvert.ToDeclination("-28:00:00"), AstroConvert.ToDeclination("-27:45:00"),
        //        249, AstroConvert.ToDeclination("-27:45:00"), AstroConvert.ToDeclination("-27:30:00"),
        //        250, AstroConvert.ToDeclination("-27:30:00"), AstroConvert.ToDeclination("-27:15:00"),
        //        251, AstroConvert.ToDeclination("-27:15:00"), AstroConvert.ToDeclination("-27:00:00"),
        //        252, AstroConvert.ToDeclination("-27:00:00"), AstroConvert.ToDeclination("-26:45:00"),
        //        253, AstroConvert.ToDeclination("-26:45:00"), AstroConvert.ToDeclination("-26:30:00"),
        //        254, AstroConvert.ToDeclination("-26:30:00"), AstroConvert.ToDeclination("-26:15:00"),
        //        255, AstroConvert.ToDeclination("-26:15:00"), AstroConvert.ToDeclination("-26:00:00"),
        //        256, AstroConvert.ToDeclination("-26:00:00"), AstroConvert.ToDeclination("-25:45:00"),
        //        257, AstroConvert.ToDeclination("-25:45:00"), AstroConvert.ToDeclination("-25:30:00"),
        //        258, AstroConvert.ToDeclination("-25:30:00"), AstroConvert.ToDeclination("-25:15:00"),
        //        259, AstroConvert.ToDeclination("-25:15:00"), AstroConvert.ToDeclination("-25:00:00"),
        //        260, AstroConvert.ToDeclination("-25:00:00"), AstroConvert.ToDeclination("-24:45:00"),
        //        261, AstroConvert.ToDeclination("-24:45:00"), AstroConvert.ToDeclination("-24:30:00"),
        //        262, AstroConvert.ToDeclination("-24:30:00"), AstroConvert.ToDeclination("-24:15:00"),
        //        263, AstroConvert.ToDeclination("-24:15:00"), AstroConvert.ToDeclination("-24:00:00"),
        //        264, AstroConvert.ToDeclination("-24:00:00"), AstroConvert.ToDeclination("-23:45:00"),
        //        265, AstroConvert.ToDeclination("-23:45:00"), AstroConvert.ToDeclination("-23:30:00"),
        //        266, AstroConvert.ToDeclination("-23:30:00"), AstroConvert.ToDeclination("-23:15:00"),
        //        267, AstroConvert.ToDeclination("-23:15:00"), AstroConvert.ToDeclination("-23:00:00"),
        //        268, AstroConvert.ToDeclination("-23:00:00"), AstroConvert.ToDeclination("-22:45:00"),
        //        269, AstroConvert.ToDeclination("-22:45:00"), AstroConvert.ToDeclination("-22:30:00"),
        //        270, AstroConvert.ToDeclination("-22:30:00"), AstroConvert.ToDeclination("-22:15:00"),
        //        271, AstroConvert.ToDeclination("-22:15:00"), AstroConvert.ToDeclination("-22:00:00"),
        //        272, AstroConvert.ToDeclination("-22:00:00"), AstroConvert.ToDeclination("-21:45:00"),
        //        273, AstroConvert.ToDeclination("-21:45:00"), AstroConvert.ToDeclination("-21:30:00"),
        //        274, AstroConvert.ToDeclination("-21:30:00"), AstroConvert.ToDeclination("-21:15:00"),
        //        275, AstroConvert.ToDeclination("-21:15:00"), AstroConvert.ToDeclination("-21:00:00"),
        //        276, AstroConvert.ToDeclination("-21:00:00"), AstroConvert.ToDeclination("-20:45:00"),
        //        277, AstroConvert.ToDeclination("-20:45:00"), AstroConvert.ToDeclination("-20:30:00"),
        //        278, AstroConvert.ToDeclination("-20:30:00"), AstroConvert.ToDeclination("-20:15:00"),
        //        279, AstroConvert.ToDeclination("-20:15:00"), AstroConvert.ToDeclination("-20:00:00"),
        //        280, AstroConvert.ToDeclination("-20:00:00"), AstroConvert.ToDeclination("-19:45:00"),
        //        281, AstroConvert.ToDeclination("-19:45:00"), AstroConvert.ToDeclination("-19:30:00"),
        //        282, AstroConvert.ToDeclination("-19:30:00"), AstroConvert.ToDeclination("-19:15:00"),
        //        283, AstroConvert.ToDeclination("-19:15:00"), AstroConvert.ToDeclination("-19:00:00"),
        //        284, AstroConvert.ToDeclination("-19:00:00"), AstroConvert.ToDeclination("-18:45:00"),
        //        285, AstroConvert.ToDeclination("-18:45:00"), AstroConvert.ToDeclination("-18:30:00"),
        //        286, AstroConvert.ToDeclination("-18:30:00"), AstroConvert.ToDeclination("-18:15:00"),
        //        287, AstroConvert.ToDeclination("-18:15:00"), AstroConvert.ToDeclination("-18:00:00"),
        //        288, AstroConvert.ToDeclination("-18:00:00"), AstroConvert.ToDeclination("-17:45:00"),
        //        289, AstroConvert.ToDeclination("-17:45:00"), AstroConvert.ToDeclination("-17:30:00"),
        //        290, AstroConvert.ToDeclination("-17:30:00"), AstroConvert.ToDeclination("-17:15:00"),
        //        291, AstroConvert.ToDeclination("-17:15:00"), AstroConvert.ToDeclination("-17:00:00"),
        //        292, AstroConvert.ToDeclination("-17:00:00"), AstroConvert.ToDeclination("-16:45:00"),
        //        293, AstroConvert.ToDeclination("-16:45:00"), AstroConvert.ToDeclination("-16:30:00"),
        //        294, AstroConvert.ToDeclination("-16:30:00"), AstroConvert.ToDeclination("-16:15:00"),
        //        295, AstroConvert.ToDeclination("-16:15:00"), AstroConvert.ToDeclination("-16:00:00"),
        //        296, AstroConvert.ToDeclination("-16:00:00"), AstroConvert.ToDeclination("-15:45:00"),
        //        297, AstroConvert.ToDeclination("-15:45:00"), AstroConvert.ToDeclination("-15:30:00"),
        //        298, AstroConvert.ToDeclination("-15:30:00"), AstroConvert.ToDeclination("-15:15:00"),
        //        299, AstroConvert.ToDeclination("-15:15:00"), AstroConvert.ToDeclination("-15:00:00"),
        //        300, AstroConvert.ToDeclination("-15:00:00"), AstroConvert.ToDeclination("-14:45:00"),
        //        301, AstroConvert.ToDeclination("-14:45:00"), AstroConvert.ToDeclination("-14:30:00"),
        //        302, AstroConvert.ToDeclination("-14:30:00"), AstroConvert.ToDeclination("-14:15:00"),
        //        303, AstroConvert.ToDeclination("-14:15:00"), AstroConvert.ToDeclination("-14:00:00"),
        //        304, AstroConvert.ToDeclination("-14:00:00"), AstroConvert.ToDeclination("-13:45:00"),
        //        305, AstroConvert.ToDeclination("-13:45:00"), AstroConvert.ToDeclination("-13:30:00"),
        //        306, AstroConvert.ToDeclination("-13:30:00"), AstroConvert.ToDeclination("-13:15:00"),
        //        307, AstroConvert.ToDeclination("-13:15:00"), AstroConvert.ToDeclination("-13:00:00"),
        //        308, AstroConvert.ToDeclination("-13:00:00"), AstroConvert.ToDeclination("-12:45:00"),
        //        309, AstroConvert.ToDeclination("-12:45:00"), AstroConvert.ToDeclination("-12:30:00"),
        //        310, AstroConvert.ToDeclination("-12:30:00"), AstroConvert.ToDeclination("-12:15:00"),
        //        311, AstroConvert.ToDeclination("-12:15:00"), AstroConvert.ToDeclination("-12:00:00"),
        //        312, AstroConvert.ToDeclination("-12:00:00"), AstroConvert.ToDeclination("-11:45:00"),
        //        313, AstroConvert.ToDeclination("-11:45:00"), AstroConvert.ToDeclination("-11:30:00"),
        //        314, AstroConvert.ToDeclination("-11:30:00"), AstroConvert.ToDeclination("-11:15:00"),
        //        315, AstroConvert.ToDeclination("-11:15:00"), AstroConvert.ToDeclination("-11:00:00"),
        //        316, AstroConvert.ToDeclination("-11:00:00"), AstroConvert.ToDeclination("-10:45:00"),
        //        317, AstroConvert.ToDeclination("-10:45:00"), AstroConvert.ToDeclination("-10:30:00"),
        //        318, AstroConvert.ToDeclination("-10:30:00"), AstroConvert.ToDeclination("-10:15:00"),
        //        319, AstroConvert.ToDeclination("-10:15:00"), AstroConvert.ToDeclination("-10:00:00"),
        //        320, AstroConvert.ToDeclination("-10:00:00"), AstroConvert.ToDeclination("-09:45:00"),
        //        321, AstroConvert.ToDeclination("-09:45:00"), AstroConvert.ToDeclination("-09:30:00"),
        //        322, AstroConvert.ToDeclination("-09:30:00"), AstroConvert.ToDeclination("-09:15:00"),
        //        323, AstroConvert.ToDeclination("-09:15:00"), AstroConvert.ToDeclination("-09:00:00"),
        //        324, AstroConvert.ToDeclination("-09:00:00"), AstroConvert.ToDeclination("-08:45:00"),
        //        325, AstroConvert.ToDeclination("-08:45:00"), AstroConvert.ToDeclination("-08:30:00"),
        //        326, AstroConvert.ToDeclination("-08:30:00"), AstroConvert.ToDeclination("-08:15:00"),
        //        327, AstroConvert.ToDeclination("-08:15:00"), AstroConvert.ToDeclination("-08:00:00"),
        //        328, AstroConvert.ToDeclination("-08:00:00"), AstroConvert.ToDeclination("-07:45:00"),
        //        329, AstroConvert.ToDeclination("-07:45:00"), AstroConvert.ToDeclination("-07:30:00"),
        //        330, AstroConvert.ToDeclination("-07:30:00"), AstroConvert.ToDeclination("-07:15:00"),
        //        331, AstroConvert.ToDeclination("-07:15:00"), AstroConvert.ToDeclination("-07:00:00"),
        //        332, AstroConvert.ToDeclination("-07:00:00"), AstroConvert.ToDeclination("-06:45:00"),
        //        333, AstroConvert.ToDeclination("-06:45:00"), AstroConvert.ToDeclination("-06:30:00"),
        //        334, AstroConvert.ToDeclination("-06:30:00"), AstroConvert.ToDeclination("-06:15:00"),
        //        335, AstroConvert.ToDeclination("-06:15:00"), AstroConvert.ToDeclination("-06:00:00"),
        //        336, AstroConvert.ToDeclination("-06:00:00"), AstroConvert.ToDeclination("-05:45:00"),
        //        337, AstroConvert.ToDeclination("-05:45:00"), AstroConvert.ToDeclination("-05:30:00"),
        //        338, AstroConvert.ToDeclination("-05:30:00"), AstroConvert.ToDeclination("-05:15:00"),
        //        339, AstroConvert.ToDeclination("-05:15:00"), AstroConvert.ToDeclination("-05:00:00"),
        //        340, AstroConvert.ToDeclination("-05:00:00"), AstroConvert.ToDeclination("-04:45:00"),
        //        341, AstroConvert.ToDeclination("-04:45:00"), AstroConvert.ToDeclination("-04:30:00"),
        //        342, AstroConvert.ToDeclination("-04:30:00"), AstroConvert.ToDeclination("-04:15:00"),
        //        343, AstroConvert.ToDeclination("-04:15:00"), AstroConvert.ToDeclination("-04:00:00"),
        //        344, AstroConvert.ToDeclination("-04:00:00"), AstroConvert.ToDeclination("-03:45:00"),
        //        345, AstroConvert.ToDeclination("-03:45:00"), AstroConvert.ToDeclination("-03:30:00"),
        //        346, AstroConvert.ToDeclination("-03:30:00"), AstroConvert.ToDeclination("-03:15:00"),
        //        347, AstroConvert.ToDeclination("-03:15:00"), AstroConvert.ToDeclination("-03:00:00"),
        //        348, AstroConvert.ToDeclination("-03:00:00"), AstroConvert.ToDeclination("-02:45:00"),
        //        349, AstroConvert.ToDeclination("-02:45:00"), AstroConvert.ToDeclination("-02:30:00"),
        //        350, AstroConvert.ToDeclination("-02:30:00"), AstroConvert.ToDeclination("-02:15:00"),
        //        351, AstroConvert.ToDeclination("-02:15:00"), AstroConvert.ToDeclination("-02:00:00"),
        //        352, AstroConvert.ToDeclination("-02:00:00"), AstroConvert.ToDeclination("-01:45:00"),
        //        353, AstroConvert.ToDeclination("-01:45:00"), AstroConvert.ToDeclination("-01:30:00"),
        //        354, AstroConvert.ToDeclination("-01:30:00"), AstroConvert.ToDeclination("-01:15:00"),
        //        355, AstroConvert.ToDeclination("-01:15:00"), AstroConvert.ToDeclination("-01:00:00"),
        //        356, AstroConvert.ToDeclination("-01:00:00"), AstroConvert.ToDeclination("-00:45:00"),
        //        357, AstroConvert.ToDeclination("-00:45:00"), AstroConvert.ToDeclination("-00:30:00"),
        //        358, AstroConvert.ToDeclination("-00:30:00"), AstroConvert.ToDeclination("-00:15:00"),
        //        359, AstroConvert.ToDeclination("-00:15:00"), AstroConvert.ToDeclination("-00:00:00"),
        //        360, AstroConvert.ToDeclination("-00:00:00"), AstroConvert.ToDeclination("+00:15:00"),
        //        361, AstroConvert.ToDeclination("+00:15:00"), AstroConvert.ToDeclination("+00:30:00"),
        //        362, AstroConvert.ToDeclination("+00:30:00"), AstroConvert.ToDeclination("+00:45:00"),
        //        363, AstroConvert.ToDeclination("+00:45:00"), AstroConvert.ToDeclination("+01:00:00"),
        //        364, AstroConvert.ToDeclination("+01:00:00"), AstroConvert.ToDeclination("+01:15:00"),
        //        365, AstroConvert.ToDeclination("+01:15:00"), AstroConvert.ToDeclination("+01:30:00"),
        //        366, AstroConvert.ToDeclination("+01:30:00"), AstroConvert.ToDeclination("+01:45:00"),
        //        367, AstroConvert.ToDeclination("+01:45:00"), AstroConvert.ToDeclination("+02:00:00"),
        //        368, AstroConvert.ToDeclination("+02:00:00"), AstroConvert.ToDeclination("+02:15:00"),
        //        369, AstroConvert.ToDeclination("+02:15:00"), AstroConvert.ToDeclination("+02:30:00"),
        //        370, AstroConvert.ToDeclination("+02:30:00"), AstroConvert.ToDeclination("+02:45:00"),
        //        371, AstroConvert.ToDeclination("+02:45:00"), AstroConvert.ToDeclination("+03:00:00"),
        //        372, AstroConvert.ToDeclination("+03:00:00"), AstroConvert.ToDeclination("+03:15:00"),
        //        373, AstroConvert.ToDeclination("+03:15:00"), AstroConvert.ToDeclination("+03:30:00"),
        //        374, AstroConvert.ToDeclination("+03:30:00"), AstroConvert.ToDeclination("+03:45:00"),
        //        375, AstroConvert.ToDeclination("+03:45:00"), AstroConvert.ToDeclination("+04:00:00"),
        //        376, AstroConvert.ToDeclination("+04:00:00"), AstroConvert.ToDeclination("+04:15:00"),
        //        377, AstroConvert.ToDeclination("+04:15:00"), AstroConvert.ToDeclination("+04:30:00"),
        //        378, AstroConvert.ToDeclination("+04:30:00"), AstroConvert.ToDeclination("+04:45:00"),
        //        379, AstroConvert.ToDeclination("+04:45:00"), AstroConvert.ToDeclination("+05:00:00"),
        //        380, AstroConvert.ToDeclination("+05:00:00"), AstroConvert.ToDeclination("+05:15:00"),
        //        381, AstroConvert.ToDeclination("+05:15:00"), AstroConvert.ToDeclination("+05:30:00"),
        //        382, AstroConvert.ToDeclination("+05:30:00"), AstroConvert.ToDeclination("+05:45:00"),
        //        383, AstroConvert.ToDeclination("+05:45:00"), AstroConvert.ToDeclination("+06:00:00"),
        //        384, AstroConvert.ToDeclination("+06:00:00"), AstroConvert.ToDeclination("+06:15:00"),
        //        385, AstroConvert.ToDeclination("+06:15:00"), AstroConvert.ToDeclination("+06:30:00"),
        //        386, AstroConvert.ToDeclination("+06:30:00"), AstroConvert.ToDeclination("+06:45:00"),
        //        387, AstroConvert.ToDeclination("+06:45:00"), AstroConvert.ToDeclination("+07:00:00"),
        //        388, AstroConvert.ToDeclination("+07:00:00"), AstroConvert.ToDeclination("+07:15:00"),
        //        389, AstroConvert.ToDeclination("+07:15:00"), AstroConvert.ToDeclination("+07:30:00"),
        //        390, AstroConvert.ToDeclination("+07:30:00"), AstroConvert.ToDeclination("+07:45:00"),
        //        391, AstroConvert.ToDeclination("+07:45:00"), AstroConvert.ToDeclination("+08:00:00"),
        //        392, AstroConvert.ToDeclination("+08:00:00"), AstroConvert.ToDeclination("+08:15:00"),
        //        393, AstroConvert.ToDeclination("+08:15:00"), AstroConvert.ToDeclination("+08:30:00"),
        //        394, AstroConvert.ToDeclination("+08:30:00"), AstroConvert.ToDeclination("+08:45:00"),
        //        395, AstroConvert.ToDeclination("+08:45:00"), AstroConvert.ToDeclination("+09:00:00"),
        //        396, AstroConvert.ToDeclination("+09:00:00"), AstroConvert.ToDeclination("+09:15:00"),
        //        397, AstroConvert.ToDeclination("+09:15:00"), AstroConvert.ToDeclination("+09:30:00"),
        //        398, AstroConvert.ToDeclination("+09:30:00"), AstroConvert.ToDeclination("+09:45:00"),
        //        399, AstroConvert.ToDeclination("+09:45:00"), AstroConvert.ToDeclination("+10:00:00"),
        //        400, AstroConvert.ToDeclination("+10:00:00"), AstroConvert.ToDeclination("+10:15:00"),
        //        401, AstroConvert.ToDeclination("+10:15:00"), AstroConvert.ToDeclination("+10:30:00"),
        //        402, AstroConvert.ToDeclination("+10:30:00"), AstroConvert.ToDeclination("+10:45:00"),
        //        403, AstroConvert.ToDeclination("+10:45:00"), AstroConvert.ToDeclination("+11:00:00"),
        //        404, AstroConvert.ToDeclination("+11:00:00"), AstroConvert.ToDeclination("+11:15:00"),
        //        405, AstroConvert.ToDeclination("+11:15:00"), AstroConvert.ToDeclination("+11:30:00"),
        //        406, AstroConvert.ToDeclination("+11:30:00"), AstroConvert.ToDeclination("+11:45:00"),
        //        407, AstroConvert.ToDeclination("+11:45:00"), AstroConvert.ToDeclination("+12:00:00"),
        //        408, AstroConvert.ToDeclination("+12:00:00"), AstroConvert.ToDeclination("+12:15:00"),
        //        409, AstroConvert.ToDeclination("+12:15:00"), AstroConvert.ToDeclination("+12:30:00"),
        //        410, AstroConvert.ToDeclination("+12:30:00"), AstroConvert.ToDeclination("+12:45:00"),
        //        411, AstroConvert.ToDeclination("+12:45:00"), AstroConvert.ToDeclination("+13:00:00"),
        //        412, AstroConvert.ToDeclination("+13:00:00"), AstroConvert.ToDeclination("+13:15:00"),
        //        413, AstroConvert.ToDeclination("+13:15:00"), AstroConvert.ToDeclination("+13:30:00"),
        //        414, AstroConvert.ToDeclination("+13:30:00"), AstroConvert.ToDeclination("+13:45:00"),
        //        415, AstroConvert.ToDeclination("+13:45:00"), AstroConvert.ToDeclination("+14:00:00"),
        //        416, AstroConvert.ToDeclination("+14:00:00"), AstroConvert.ToDeclination("+14:15:00"),
        //        417, AstroConvert.ToDeclination("+14:15:00"), AstroConvert.ToDeclination("+14:30:00"),
        //        418, AstroConvert.ToDeclination("+14:30:00"), AstroConvert.ToDeclination("+14:45:00"),
        //        419, AstroConvert.ToDeclination("+14:45:00"), AstroConvert.ToDeclination("+15:00:00"),
        //        420, AstroConvert.ToDeclination("+15:00:00"), AstroConvert.ToDeclination("+15:15:00"),
        //        421, AstroConvert.ToDeclination("+15:15:00"), AstroConvert.ToDeclination("+15:30:00"),
        //        422, AstroConvert.ToDeclination("+15:30:00"), AstroConvert.ToDeclination("+15:45:00"),
        //        423, AstroConvert.ToDeclination("+15:45:00"), AstroConvert.ToDeclination("+16:00:00"),
        //        424, AstroConvert.ToDeclination("+16:00:00"), AstroConvert.ToDeclination("+16:15:00"),
        //        425, AstroConvert.ToDeclination("+16:15:00"), AstroConvert.ToDeclination("+16:30:00"),
        //        426, AstroConvert.ToDeclination("+16:30:00"), AstroConvert.ToDeclination("+16:45:00"),
        //        427, AstroConvert.ToDeclination("+16:45:00"), AstroConvert.ToDeclination("+17:00:00"),
        //        428, AstroConvert.ToDeclination("+17:00:00"), AstroConvert.ToDeclination("+17:15:00"),
        //        429, AstroConvert.ToDeclination("+17:15:00"), AstroConvert.ToDeclination("+17:30:00"),
        //        430, AstroConvert.ToDeclination("+17:30:00"), AstroConvert.ToDeclination("+17:45:00"),
        //        431, AstroConvert.ToDeclination("+17:45:00"), AstroConvert.ToDeclination("+18:00:00"),
        //        432, AstroConvert.ToDeclination("+18:00:00"), AstroConvert.ToDeclination("+18:15:00"),
        //        433, AstroConvert.ToDeclination("+18:15:00"), AstroConvert.ToDeclination("+18:30:00"),
        //        434, AstroConvert.ToDeclination("+18:30:00"), AstroConvert.ToDeclination("+18:45:00"),
        //        435, AstroConvert.ToDeclination("+18:45:00"), AstroConvert.ToDeclination("+19:00:00"),
        //        436, AstroConvert.ToDeclination("+19:00:00"), AstroConvert.ToDeclination("+19:15:00"),
        //        437, AstroConvert.ToDeclination("+19:15:00"), AstroConvert.ToDeclination("+19:30:00"),
        //        438, AstroConvert.ToDeclination("+19:30:00"), AstroConvert.ToDeclination("+19:45:00"),
        //        439, AstroConvert.ToDeclination("+19:45:00"), AstroConvert.ToDeclination("+20:00:00"),
        //        440, AstroConvert.ToDeclination("+20:00:00"), AstroConvert.ToDeclination("+20:15:00"),
        //        441, AstroConvert.ToDeclination("+20:15:00"), AstroConvert.ToDeclination("+20:30:00"),
        //        442, AstroConvert.ToDeclination("+20:30:00"), AstroConvert.ToDeclination("+20:45:00"),
        //        443, AstroConvert.ToDeclination("+20:45:00"), AstroConvert.ToDeclination("+21:00:00"),
        //        444, AstroConvert.ToDeclination("+21:00:00"), AstroConvert.ToDeclination("+21:15:00"),
        //        445, AstroConvert.ToDeclination("+21:15:00"), AstroConvert.ToDeclination("+21:30:00"),
        //        446, AstroConvert.ToDeclination("+21:30:00"), AstroConvert.ToDeclination("+21:45:00"),
        //        447, AstroConvert.ToDeclination("+21:45:00"), AstroConvert.ToDeclination("+22:00:00"),
        //        448, AstroConvert.ToDeclination("+22:00:00"), AstroConvert.ToDeclination("+22:15:00"),
        //        449, AstroConvert.ToDeclination("+22:15:00"), AstroConvert.ToDeclination("+22:30:00"),
        //        450, AstroConvert.ToDeclination("+22:30:00"), AstroConvert.ToDeclination("+22:45:00"),
        //        451, AstroConvert.ToDeclination("+22:45:00"), AstroConvert.ToDeclination("+23:00:00"),
        //        452, AstroConvert.ToDeclination("+23:00:00"), AstroConvert.ToDeclination("+23:15:00"),
        //        453, AstroConvert.ToDeclination("+23:15:00"), AstroConvert.ToDeclination("+23:30:00"),
        //        454, AstroConvert.ToDeclination("+23:30:00"), AstroConvert.ToDeclination("+23:45:00"),
        //        455, AstroConvert.ToDeclination("+23:45:00"), AstroConvert.ToDeclination("+24:00:00"),
        //        456, AstroConvert.ToDeclination("+24:00:00"), AstroConvert.ToDeclination("+24:15:00"),
        //        457, AstroConvert.ToDeclination("+24:15:00"), AstroConvert.ToDeclination("+24:30:00"),
        //        458, AstroConvert.ToDeclination("+24:30:00"), AstroConvert.ToDeclination("+24:45:00"),
        //        459, AstroConvert.ToDeclination("+24:45:00"), AstroConvert.ToDeclination("+25:00:00"),
        //        460, AstroConvert.ToDeclination("+25:00:00"), AstroConvert.ToDeclination("+25:15:00"),
        //        461, AstroConvert.ToDeclination("+25:15:00"), AstroConvert.ToDeclination("+25:30:00"),
        //        462, AstroConvert.ToDeclination("+25:30:00"), AstroConvert.ToDeclination("+25:45:00"),
        //        463, AstroConvert.ToDeclination("+25:45:00"), AstroConvert.ToDeclination("+26:00:00"),
        //        464, AstroConvert.ToDeclination("+26:00:00"), AstroConvert.ToDeclination("+26:15:00"),
        //        465, AstroConvert.ToDeclination("+26:15:00"), AstroConvert.ToDeclination("+26:30:00"),
        //        466, AstroConvert.ToDeclination("+26:30:00"), AstroConvert.ToDeclination("+26:45:00"),
        //        467, AstroConvert.ToDeclination("+26:45:00"), AstroConvert.ToDeclination("+27:00:00"),
        //        468, AstroConvert.ToDeclination("+27:00:00"), AstroConvert.ToDeclination("+27:15:00"),
        //        469, AstroConvert.ToDeclination("+27:15:00"), AstroConvert.ToDeclination("+27:30:00"),
        //        470, AstroConvert.ToDeclination("+27:30:00"), AstroConvert.ToDeclination("+27:45:00"),
        //        471, AstroConvert.ToDeclination("+27:45:00"), AstroConvert.ToDeclination("+28:00:00"),
        //        472, AstroConvert.ToDeclination("+28:00:00"), AstroConvert.ToDeclination("+28:15:00"),
        //        473, AstroConvert.ToDeclination("+28:15:00"), AstroConvert.ToDeclination("+28:30:00"),
        //        474, AstroConvert.ToDeclination("+28:30:00"), AstroConvert.ToDeclination("+28:45:00"),
        //        475, AstroConvert.ToDeclination("+28:45:00"), AstroConvert.ToDeclination("+29:00:00"),
        //        476, AstroConvert.ToDeclination("+29:00:00"), AstroConvert.ToDeclination("+29:15:00"),
        //        477, AstroConvert.ToDeclination("+29:15:00"), AstroConvert.ToDeclination("+29:30:00"),
        //        478, AstroConvert.ToDeclination("+29:30:00"), AstroConvert.ToDeclination("+29:45:00"),
        //        479, AstroConvert.ToDeclination("+29:45:00"), AstroConvert.ToDeclination("+30:00:00"),
        //        480, AstroConvert.ToDeclination("+30:00:00"), AstroConvert.ToDeclination("+30:15:00"),
        //        481, AstroConvert.ToDeclination("+30:15:00"), AstroConvert.ToDeclination("+30:30:00"),
        //        482, AstroConvert.ToDeclination("+30:30:00"), AstroConvert.ToDeclination("+30:45:00"),
        //        483, AstroConvert.ToDeclination("+30:45:00"), AstroConvert.ToDeclination("+31:00:00"),
        //        484, AstroConvert.ToDeclination("+31:00:00"), AstroConvert.ToDeclination("+31:15:00"),
        //        485, AstroConvert.ToDeclination("+31:15:00"), AstroConvert.ToDeclination("+31:30:00"),
        //        486, AstroConvert.ToDeclination("+31:30:00"), AstroConvert.ToDeclination("+31:45:00"),
        //        487, AstroConvert.ToDeclination("+31:45:00"), AstroConvert.ToDeclination("+32:00:00"),
        //        488, AstroConvert.ToDeclination("+32:00:00"), AstroConvert.ToDeclination("+32:15:00"),
        //        489, AstroConvert.ToDeclination("+32:15:00"), AstroConvert.ToDeclination("+32:30:00"),
        //        490, AstroConvert.ToDeclination("+32:30:00"), AstroConvert.ToDeclination("+32:45:00"),
        //        491, AstroConvert.ToDeclination("+32:45:00"), AstroConvert.ToDeclination("+33:00:00"),
        //        492, AstroConvert.ToDeclination("+33:00:00"), AstroConvert.ToDeclination("+33:15:00"),
        //        493, AstroConvert.ToDeclination("+33:15:00"), AstroConvert.ToDeclination("+33:30:00"),
        //        494, AstroConvert.ToDeclination("+33:30:00"), AstroConvert.ToDeclination("+33:45:00"),
        //        495, AstroConvert.ToDeclination("+33:45:00"), AstroConvert.ToDeclination("+34:00:00"),
        //        496, AstroConvert.ToDeclination("+34:00:00"), AstroConvert.ToDeclination("+34:15:00"),
        //        497, AstroConvert.ToDeclination("+34:15:00"), AstroConvert.ToDeclination("+34:30:00"),
        //        498, AstroConvert.ToDeclination("+34:30:00"), AstroConvert.ToDeclination("+34:45:00"),
        //        499, AstroConvert.ToDeclination("+34:45:00"), AstroConvert.ToDeclination("+35:00:00"),
        //        500, AstroConvert.ToDeclination("+35:00:00"), AstroConvert.ToDeclination("+35:15:00"),
        //        501, AstroConvert.ToDeclination("+35:15:00"), AstroConvert.ToDeclination("+35:30:00"),
        //        502, AstroConvert.ToDeclination("+35:30:00"), AstroConvert.ToDeclination("+35:45:00"),
        //        503, AstroConvert.ToDeclination("+35:45:00"), AstroConvert.ToDeclination("+36:00:00"),
        //        504, AstroConvert.ToDeclination("+36:00:00"), AstroConvert.ToDeclination("+36:15:00"),
        //        505, AstroConvert.ToDeclination("+36:15:00"), AstroConvert.ToDeclination("+36:30:00"),
        //        506, AstroConvert.ToDeclination("+36:30:00"), AstroConvert.ToDeclination("+36:45:00"),
        //        507, AstroConvert.ToDeclination("+36:45:00"), AstroConvert.ToDeclination("+37:00:00"),
        //        508, AstroConvert.ToDeclination("+37:00:00"), AstroConvert.ToDeclination("+37:15:00"),
        //        509, AstroConvert.ToDeclination("+37:15:00"), AstroConvert.ToDeclination("+37:30:00"),
        //        510, AstroConvert.ToDeclination("+37:30:00"), AstroConvert.ToDeclination("+37:45:00"),
        //        511, AstroConvert.ToDeclination("+37:45:00"), AstroConvert.ToDeclination("+38:00:00"),
        //        512, AstroConvert.ToDeclination("+38:00:00"), AstroConvert.ToDeclination("+38:15:00"),
        //        513, AstroConvert.ToDeclination("+38:15:00"), AstroConvert.ToDeclination("+38:30:00"),
        //        514, AstroConvert.ToDeclination("+38:30:00"), AstroConvert.ToDeclination("+38:45:00"),
        //        515, AstroConvert.ToDeclination("+38:45:00"), AstroConvert.ToDeclination("+39:00:00"),
        //        516, AstroConvert.ToDeclination("+39:00:00"), AstroConvert.ToDeclination("+39:15:00"),
        //        517, AstroConvert.ToDeclination("+39:15:00"), AstroConvert.ToDeclination("+39:30:00"),
        //        518, AstroConvert.ToDeclination("+39:30:00"), AstroConvert.ToDeclination("+39:45:00"),
        //        519, AstroConvert.ToDeclination("+39:45:00"), AstroConvert.ToDeclination("+40:00:00"),
        //        520, AstroConvert.ToDeclination("+40:00:00"), AstroConvert.ToDeclination("+40:15:00"),
        //        521, AstroConvert.ToDeclination("+40:15:00"), AstroConvert.ToDeclination("+40:30:00"),
        //        522, AstroConvert.ToDeclination("+40:30:00"), AstroConvert.ToDeclination("+40:45:00"),
        //        523, AstroConvert.ToDeclination("+40:45:00"), AstroConvert.ToDeclination("+41:00:00"),
        //        524, AstroConvert.ToDeclination("+41:00:00"), AstroConvert.ToDeclination("+41:15:00"),
        //        525, AstroConvert.ToDeclination("+41:15:00"), AstroConvert.ToDeclination("+41:30:00"),
        //        526, AstroConvert.ToDeclination("+41:30:00"), AstroConvert.ToDeclination("+41:45:00"),
        //        527, AstroConvert.ToDeclination("+41:45:00"), AstroConvert.ToDeclination("+42:00:00"),
        //        528, AstroConvert.ToDeclination("+42:00:00"), AstroConvert.ToDeclination("+42:15:00"),
        //        529, AstroConvert.ToDeclination("+42:15:00"), AstroConvert.ToDeclination("+42:30:00"),
        //        530, AstroConvert.ToDeclination("+42:30:00"), AstroConvert.ToDeclination("+42:45:00"),
        //        531, AstroConvert.ToDeclination("+42:45:00"), AstroConvert.ToDeclination("+43:00:00"),
        //        532, AstroConvert.ToDeclination("+43:00:00"), AstroConvert.ToDeclination("+43:15:00"),
        //        533, AstroConvert.ToDeclination("+43:15:00"), AstroConvert.ToDeclination("+43:30:00"),
        //        534, AstroConvert.ToDeclination("+43:30:00"), AstroConvert.ToDeclination("+43:45:00"),
        //        535, AstroConvert.ToDeclination("+43:45:00"), AstroConvert.ToDeclination("+44:00:00"),
        //        536, AstroConvert.ToDeclination("+44:00:00"), AstroConvert.ToDeclination("+44:15:00"),
        //        537, AstroConvert.ToDeclination("+44:15:00"), AstroConvert.ToDeclination("+44:30:00"),
        //        538, AstroConvert.ToDeclination("+44:30:00"), AstroConvert.ToDeclination("+44:45:00"),
        //        539, AstroConvert.ToDeclination("+44:45:00"), AstroConvert.ToDeclination("+45:00:00"),
        //        540, AstroConvert.ToDeclination("+45:00:00"), AstroConvert.ToDeclination("+45:15:00"),
        //        541, AstroConvert.ToDeclination("+45:15:00"), AstroConvert.ToDeclination("+45:30:00"),
        //        542, AstroConvert.ToDeclination("+45:30:00"), AstroConvert.ToDeclination("+45:45:00"),
        //        543, AstroConvert.ToDeclination("+45:45:00"), AstroConvert.ToDeclination("+46:00:00"),
        //        544, AstroConvert.ToDeclination("+46:00:00"), AstroConvert.ToDeclination("+46:15:00"),
        //        545, AstroConvert.ToDeclination("+46:15:00"), AstroConvert.ToDeclination("+46:30:00"),
        //        546, AstroConvert.ToDeclination("+46:30:00"), AstroConvert.ToDeclination("+46:45:00"),
        //        547, AstroConvert.ToDeclination("+46:45:00"), AstroConvert.ToDeclination("+47:00:00"),
        //        548, AstroConvert.ToDeclination("+47:00:00"), AstroConvert.ToDeclination("+47:15:00"),
        //        549, AstroConvert.ToDeclination("+47:15:00"), AstroConvert.ToDeclination("+47:30:00"),
        //        550, AstroConvert.ToDeclination("+47:30:00"), AstroConvert.ToDeclination("+47:45:00"),
        //        551, AstroConvert.ToDeclination("+47:45:00"), AstroConvert.ToDeclination("+48:00:00"),
        //        552, AstroConvert.ToDeclination("+48:00:00"), AstroConvert.ToDeclination("+48:15:00"),
        //        553, AstroConvert.ToDeclination("+48:15:00"), AstroConvert.ToDeclination("+48:30:00"),
        //        554, AstroConvert.ToDeclination("+48:30:00"), AstroConvert.ToDeclination("+48:45:00"),
        //        555, AstroConvert.ToDeclination("+48:45:00"), AstroConvert.ToDeclination("+49:00:00"),
        //        556, AstroConvert.ToDeclination("+49:00:00"), AstroConvert.ToDeclination("+49:15:00"),
        //        557, AstroConvert.ToDeclination("+49:15:00"), AstroConvert.ToDeclination("+49:30:00"),
        //        558, AstroConvert.ToDeclination("+49:30:00"), AstroConvert.ToDeclination("+49:45:00"),
        //        559, AstroConvert.ToDeclination("+49:45:00"), AstroConvert.ToDeclination("+50:00:00"),
        //        560, AstroConvert.ToDeclination("+50:00:00"), AstroConvert.ToDeclination("+50:15:00"),
        //        561, AstroConvert.ToDeclination("+50:15:00"), AstroConvert.ToDeclination("+50:30:00"),
        //        562, AstroConvert.ToDeclination("+50:30:00"), AstroConvert.ToDeclination("+50:45:00"),
        //        563, AstroConvert.ToDeclination("+50:45:00"), AstroConvert.ToDeclination("+51:00:00"),
        //        564, AstroConvert.ToDeclination("+51:00:00"), AstroConvert.ToDeclination("+51:15:00"),
        //        565, AstroConvert.ToDeclination("+51:15:00"), AstroConvert.ToDeclination("+51:30:00"),
        //        566, AstroConvert.ToDeclination("+51:30:00"), AstroConvert.ToDeclination("+51:45:00"),
        //        567, AstroConvert.ToDeclination("+51:45:00"), AstroConvert.ToDeclination("+52:00:00"),
        //        568, AstroConvert.ToDeclination("+52:00:00"), AstroConvert.ToDeclination("+52:15:00"),
        //        569, AstroConvert.ToDeclination("+52:15:00"), AstroConvert.ToDeclination("+52:30:00"),
        //        570, AstroConvert.ToDeclination("+52:30:00"), AstroConvert.ToDeclination("+52:45:00"),
        //        571, AstroConvert.ToDeclination("+52:45:00"), AstroConvert.ToDeclination("+53:00:00"),
        //        572, AstroConvert.ToDeclination("+53:00:00"), AstroConvert.ToDeclination("+53:15:00"),
        //        573, AstroConvert.ToDeclination("+53:15:00"), AstroConvert.ToDeclination("+53:30:00"),
        //        574, AstroConvert.ToDeclination("+53:30:00"), AstroConvert.ToDeclination("+53:45:00"),
        //        575, AstroConvert.ToDeclination("+53:45:00"), AstroConvert.ToDeclination("+54:00:00"),
        //        576, AstroConvert.ToDeclination("+54:00:00"), AstroConvert.ToDeclination("+54:15:00"),
        //        577, AstroConvert.ToDeclination("+54:15:00"), AstroConvert.ToDeclination("+54:30:00"),
        //        578, AstroConvert.ToDeclination("+54:30:00"), AstroConvert.ToDeclination("+54:45:00"),
        //        579, AstroConvert.ToDeclination("+54:45:00"), AstroConvert.ToDeclination("+55:00:00"),
        //        580, AstroConvert.ToDeclination("+55:00:00"), AstroConvert.ToDeclination("+55:15:00"),
        //        581, AstroConvert.ToDeclination("+55:15:00"), AstroConvert.ToDeclination("+55:30:00"),
        //        582, AstroConvert.ToDeclination("+55:30:00"), AstroConvert.ToDeclination("+55:45:00"),
        //        583, AstroConvert.ToDeclination("+55:45:00"), AstroConvert.ToDeclination("+56:00:00"),
        //        584, AstroConvert.ToDeclination("+56:00:00"), AstroConvert.ToDeclination("+56:15:00"),
        //        585, AstroConvert.ToDeclination("+56:15:00"), AstroConvert.ToDeclination("+56:30:00"),
        //        586, AstroConvert.ToDeclination("+56:30:00"), AstroConvert.ToDeclination("+56:45:00"),
        //        587, AstroConvert.ToDeclination("+56:45:00"), AstroConvert.ToDeclination("+57:00:00"),
        //        588, AstroConvert.ToDeclination("+57:00:00"), AstroConvert.ToDeclination("+57:15:00"),
        //        589, AstroConvert.ToDeclination("+57:15:00"), AstroConvert.ToDeclination("+57:30:00"),
        //        590, AstroConvert.ToDeclination("+57:30:00"), AstroConvert.ToDeclination("+57:45:00"),
        //        591, AstroConvert.ToDeclination("+57:45:00"), AstroConvert.ToDeclination("+58:00:00"),
        //        592, AstroConvert.ToDeclination("+58:00:00"), AstroConvert.ToDeclination("+58:15:00"),
        //        593, AstroConvert.ToDeclination("+58:15:00"), AstroConvert.ToDeclination("+58:30:00"),
        //        594, AstroConvert.ToDeclination("+58:30:00"), AstroConvert.ToDeclination("+58:45:00"),
        //        595, AstroConvert.ToDeclination("+58:45:00"), AstroConvert.ToDeclination("+59:00:00"),
        //        596, AstroConvert.ToDeclination("+59:00:00"), AstroConvert.ToDeclination("+59:15:00"),
        //        597, AstroConvert.ToDeclination("+59:15:00"), AstroConvert.ToDeclination("+59:30:00"),
        //        598, AstroConvert.ToDeclination("+59:30:00"), AstroConvert.ToDeclination("+59:45:00"),
        //        599, AstroConvert.ToDeclination("+59:45:00"), AstroConvert.ToDeclination("+60:00:00"),
        //        600, AstroConvert.ToDeclination("+60:00:00"), AstroConvert.ToDeclination("+60:15:00"),
        //        601, AstroConvert.ToDeclination("+60:15:00"), AstroConvert.ToDeclination("+60:30:00"),
        //        602, AstroConvert.ToDeclination("+60:30:00"), AstroConvert.ToDeclination("+60:45:00"),
        //        603, AstroConvert.ToDeclination("+60:45:00"), AstroConvert.ToDeclination("+61:00:00"),
        //        604, AstroConvert.ToDeclination("+61:00:00"), AstroConvert.ToDeclination("+61:15:00"),
        //        605, AstroConvert.ToDeclination("+61:15:00"), AstroConvert.ToDeclination("+61:30:00"),
        //        606, AstroConvert.ToDeclination("+61:30:00"), AstroConvert.ToDeclination("+61:45:00"),
        //        607, AstroConvert.ToDeclination("+61:45:00"), AstroConvert.ToDeclination("+62:00:00"),
        //        608, AstroConvert.ToDeclination("+62:00:00"), AstroConvert.ToDeclination("+62:15:00"),
        //        609, AstroConvert.ToDeclination("+62:15:00"), AstroConvert.ToDeclination("+62:30:00"),
        //        610, AstroConvert.ToDeclination("+62:30:00"), AstroConvert.ToDeclination("+62:45:00"),
        //        611, AstroConvert.ToDeclination("+62:45:00"), AstroConvert.ToDeclination("+63:00:00"),
        //        612, AstroConvert.ToDeclination("+63:00:00"), AstroConvert.ToDeclination("+63:15:00"),
        //        613, AstroConvert.ToDeclination("+63:15:00"), AstroConvert.ToDeclination("+63:30:00"),
        //        614, AstroConvert.ToDeclination("+63:30:00"), AstroConvert.ToDeclination("+63:45:00"),
        //        615, AstroConvert.ToDeclination("+63:45:00"), AstroConvert.ToDeclination("+64:00:00"),
        //        616, AstroConvert.ToDeclination("+64:00:00"), AstroConvert.ToDeclination("+64:15:00"),
        //        617, AstroConvert.ToDeclination("+64:15:00"), AstroConvert.ToDeclination("+64:30:00"),
        //        618, AstroConvert.ToDeclination("+64:30:00"), AstroConvert.ToDeclination("+64:45:00"),
        //        619, AstroConvert.ToDeclination("+64:45:00"), AstroConvert.ToDeclination("+65:00:00"),
        //        620, AstroConvert.ToDeclination("+65:00:00"), AstroConvert.ToDeclination("+65:15:00"),
        //        621, AstroConvert.ToDeclination("+65:15:00"), AstroConvert.ToDeclination("+65:30:00"),
        //        622, AstroConvert.ToDeclination("+65:30:00"), AstroConvert.ToDeclination("+65:45:00"),
        //        623, AstroConvert.ToDeclination("+65:45:00"), AstroConvert.ToDeclination("+66:00:00"),
        //        624, AstroConvert.ToDeclination("+66:00:00"), AstroConvert.ToDeclination("+66:15:00"),
        //        625, AstroConvert.ToDeclination("+66:15:00"), AstroConvert.ToDeclination("+66:30:00"),
        //        626, AstroConvert.ToDeclination("+66:30:00"), AstroConvert.ToDeclination("+66:45:00"),
        //        627, AstroConvert.ToDeclination("+66:45:00"), AstroConvert.ToDeclination("+67:00:00"),
        //        628, AstroConvert.ToDeclination("+67:00:00"), AstroConvert.ToDeclination("+67:15:00"),
        //        629, AstroConvert.ToDeclination("+67:15:00"), AstroConvert.ToDeclination("+67:30:00"),
        //        630, AstroConvert.ToDeclination("+67:30:00"), AstroConvert.ToDeclination("+67:45:00"),
        //        631, AstroConvert.ToDeclination("+67:45:00"), AstroConvert.ToDeclination("+68:00:00"),
        //        632, AstroConvert.ToDeclination("+68:00:00"), AstroConvert.ToDeclination("+68:15:00"),
        //        633, AstroConvert.ToDeclination("+68:15:00"), AstroConvert.ToDeclination("+68:30:00"),
        //        634, AstroConvert.ToDeclination("+68:30:00"), AstroConvert.ToDeclination("+68:45:00"),
        //        635, AstroConvert.ToDeclination("+68:45:00"), AstroConvert.ToDeclination("+69:00:00"),
        //        636, AstroConvert.ToDeclination("+69:00:00"), AstroConvert.ToDeclination("+69:15:00"),
        //        637, AstroConvert.ToDeclination("+69:15:00"), AstroConvert.ToDeclination("+69:30:00"),
        //        638, AstroConvert.ToDeclination("+69:30:00"), AstroConvert.ToDeclination("+69:45:00"),
        //        639, AstroConvert.ToDeclination("+69:45:00"), AstroConvert.ToDeclination("+70:00:00"),
        //        640, AstroConvert.ToDeclination("+70:00:00"), AstroConvert.ToDeclination("+70:15:00"),
        //        641, AstroConvert.ToDeclination("+70:15:00"), AstroConvert.ToDeclination("+70:30:00"),
        //        642, AstroConvert.ToDeclination("+70:30:00"), AstroConvert.ToDeclination("+70:45:00"),
        //        643, AstroConvert.ToDeclination("+70:45:00"), AstroConvert.ToDeclination("+71:00:00"),
        //        644, AstroConvert.ToDeclination("+71:00:00"), AstroConvert.ToDeclination("+71:15:00"),
        //        645, AstroConvert.ToDeclination("+71:15:00"), AstroConvert.ToDeclination("+71:30:00"),
        //        646, AstroConvert.ToDeclination("+71:30:00"), AstroConvert.ToDeclination("+71:45:00"),
        //        647, AstroConvert.ToDeclination("+71:45:00"), AstroConvert.ToDeclination("+72:00:00"),
        //        648, AstroConvert.ToDeclination("+72:00:00"), AstroConvert.ToDeclination("+72:15:00"),
        //        649, AstroConvert.ToDeclination("+72:15:00"), AstroConvert.ToDeclination("+72:30:00"),
        //        650, AstroConvert.ToDeclination("+72:30:00"), AstroConvert.ToDeclination("+72:45:00"),
        //        651, AstroConvert.ToDeclination("+72:45:00"), AstroConvert.ToDeclination("+73:00:00"),
        //        652, AstroConvert.ToDeclination("+73:00:00"), AstroConvert.ToDeclination("+73:15:00"),
        //        653, AstroConvert.ToDeclination("+73:15:00"), AstroConvert.ToDeclination("+73:30:00"),
        //        654, AstroConvert.ToDeclination("+73:30:00"), AstroConvert.ToDeclination("+73:45:00"),
        //        655, AstroConvert.ToDeclination("+73:45:00"), AstroConvert.ToDeclination("+74:00:00"),
        //        656, AstroConvert.ToDeclination("+74:00:00"), AstroConvert.ToDeclination("+74:15:00"),
        //        657, AstroConvert.ToDeclination("+74:15:00"), AstroConvert.ToDeclination("+74:30:00"),
        //        658, AstroConvert.ToDeclination("+74:30:00"), AstroConvert.ToDeclination("+74:45:00"),
        //        659, AstroConvert.ToDeclination("+74:45:00"), AstroConvert.ToDeclination("+75:00:00"),
        //        660, AstroConvert.ToDeclination("+75:00:00"), AstroConvert.ToDeclination("+75:15:00"),
        //        661, AstroConvert.ToDeclination("+75:15:00"), AstroConvert.ToDeclination("+75:30:00"),
        //        662, AstroConvert.ToDeclination("+75:30:00"), AstroConvert.ToDeclination("+75:45:00"),
        //        663, AstroConvert.ToDeclination("+75:45:00"), AstroConvert.ToDeclination("+76:00:00"),
        //        664, AstroConvert.ToDeclination("+76:00:00"), AstroConvert.ToDeclination("+76:15:00"),
        //        665, AstroConvert.ToDeclination("+76:15:00"), AstroConvert.ToDeclination("+76:30:00"),
        //        666, AstroConvert.ToDeclination("+76:30:00"), AstroConvert.ToDeclination("+76:45:00"),
        //        667, AstroConvert.ToDeclination("+76:45:00"), AstroConvert.ToDeclination("+77:00:00"),
        //        668, AstroConvert.ToDeclination("+77:00:00"), AstroConvert.ToDeclination("+77:15:00"),
        //        669, AstroConvert.ToDeclination("+77:15:00"), AstroConvert.ToDeclination("+77:30:00"),
        //        670, AstroConvert.ToDeclination("+77:30:00"), AstroConvert.ToDeclination("+77:45:00"),
        //        671, AstroConvert.ToDeclination("+77:45:00"), AstroConvert.ToDeclination("+78:00:00"),
        //        672, AstroConvert.ToDeclination("+78:00:00"), AstroConvert.ToDeclination("+78:15:00"),
        //        673, AstroConvert.ToDeclination("+78:15:00"), AstroConvert.ToDeclination("+78:30:00"),
        //        674, AstroConvert.ToDeclination("+78:30:00"), AstroConvert.ToDeclination("+78:45:00"),
        //        675, AstroConvert.ToDeclination("+78:45:00"), AstroConvert.ToDeclination("+79:00:00"),
        //        676, AstroConvert.ToDeclination("+79:00:00"), AstroConvert.ToDeclination("+79:15:00"),
        //        677, AstroConvert.ToDeclination("+79:15:00"), AstroConvert.ToDeclination("+79:30:00"),
        //        678, AstroConvert.ToDeclination("+79:30:00"), AstroConvert.ToDeclination("+79:45:00"),
        //        679, AstroConvert.ToDeclination("+79:45:00"), AstroConvert.ToDeclination("+80:00:00"),
        //        680, AstroConvert.ToDeclination("+80:00:00"), AstroConvert.ToDeclination("+80:15:00"),
        //        681, AstroConvert.ToDeclination("+80:15:00"), AstroConvert.ToDeclination("+80:30:00"),
        //        682, AstroConvert.ToDeclination("+80:30:00"), AstroConvert.ToDeclination("+80:45:00"),
        //        683, AstroConvert.ToDeclination("+80:45:00"), AstroConvert.ToDeclination("+81:00:00"),
        //        684, AstroConvert.ToDeclination("+81:00:00"), AstroConvert.ToDeclination("+81:15:00"),
        //        685, AstroConvert.ToDeclination("+81:15:00"), AstroConvert.ToDeclination("+81:30:00"),
        //        686, AstroConvert.ToDeclination("+81:30:00"), AstroConvert.ToDeclination("+81:45:00"),
        //        687, AstroConvert.ToDeclination("+81:45:00"), AstroConvert.ToDeclination("+82:00:00"),
        //        688, AstroConvert.ToDeclination("+82:00:00"), AstroConvert.ToDeclination("+82:15:00"),
        //        689, AstroConvert.ToDeclination("+82:15:00"), AstroConvert.ToDeclination("+82:30:00"),
        //        690, AstroConvert.ToDeclination("+82:30:00"), AstroConvert.ToDeclination("+82:45:00"),
        //        691, AstroConvert.ToDeclination("+82:45:00"), AstroConvert.ToDeclination("+83:00:00"),
        //        692, AstroConvert.ToDeclination("+83:00:00"), AstroConvert.ToDeclination("+83:15:00"),
        //        693, AstroConvert.ToDeclination("+83:15:00"), AstroConvert.ToDeclination("+83:30:00"),
        //        694, AstroConvert.ToDeclination("+83:30:00"), AstroConvert.ToDeclination("+83:45:00"),
        //        695, AstroConvert.ToDeclination("+83:45:00"), AstroConvert.ToDeclination("+84:00:00"),
        //        696, AstroConvert.ToDeclination("+84:00:00"), AstroConvert.ToDeclination("+84:15:00"),
        //        697, AstroConvert.ToDeclination("+84:15:00"), AstroConvert.ToDeclination("+84:30:00"),
        //        698, AstroConvert.ToDeclination("+84:30:00"), AstroConvert.ToDeclination("+84:45:00"),
        //        699, AstroConvert.ToDeclination("+84:45:00"), AstroConvert.ToDeclination("+85:00:00"),
        //        700, AstroConvert.ToDeclination("+85:00:00"), AstroConvert.ToDeclination("+85:15:00"),
        //        701, AstroConvert.ToDeclination("+85:15:00"), AstroConvert.ToDeclination("+85:30:00"),
        //        702, AstroConvert.ToDeclination("+85:30:00"), AstroConvert.ToDeclination("+85:45:00"),
        //        703, AstroConvert.ToDeclination("+85:45:00"), AstroConvert.ToDeclination("+86:00:00"),
        //        704, AstroConvert.ToDeclination("+86:00:00"), AstroConvert.ToDeclination("+86:15:00"),
        //        705, AstroConvert.ToDeclination("+86:15:00"), AstroConvert.ToDeclination("+86:30:00"),
        //        706, AstroConvert.ToDeclination("+86:30:00"), AstroConvert.ToDeclination("+86:45:00"),
        //        707, AstroConvert.ToDeclination("+86:45:00"), AstroConvert.ToDeclination("+87:00:00"),
        //        708, AstroConvert.ToDeclination("+87:00:00"), AstroConvert.ToDeclination("+87:15:00"),
        //        709, AstroConvert.ToDeclination("+87:15:00"), AstroConvert.ToDeclination("+87:30:00"),
        //        710, AstroConvert.ToDeclination("+87:30:00"), AstroConvert.ToDeclination("+87:45:00"),
        //        711, AstroConvert.ToDeclination("+87:45:00"), AstroConvert.ToDeclination("+88:00:00"),
        //        712, AstroConvert.ToDeclination("+88:00:00"), AstroConvert.ToDeclination("+88:15:00"),
        //        713, AstroConvert.ToDeclination("+88:15:00"), AstroConvert.ToDeclination("+88:30:00"),
        //        714, AstroConvert.ToDeclination("+88:30:00"), AstroConvert.ToDeclination("+88:45:00"),
        //        715, AstroConvert.ToDeclination("+88:45:00"), AstroConvert.ToDeclination("+89:00:00"),
        //        716, AstroConvert.ToDeclination("+89:00:00"), AstroConvert.ToDeclination("+89:15:00"),
        //        717, AstroConvert.ToDeclination("+89:15:00"), AstroConvert.ToDeclination("+89:30:00"),
        //        718, AstroConvert.ToDeclination("+89:30:00"), AstroConvert.ToDeclination("+89:45:00"),
        //        719, AstroConvert.ToDeclination("+89:45:00"), AstroConvert.ToDeclination("+90:00:00")
        //    };
        //    #endregion

        //    for (int i = 0; i < RANGES.Length / 3; i++)
        //    {
        //        int zoneId = (int)RANGES[3 * i];
        //        double deFrom = RANGES[3 * i + 1];
        //        double deTo = RANGES[3 * i + 2];

        //        PPMXLCatalogue cat = new PPMXLCatalogue(CATALOG_LOCATION);

        //        double de = (deFrom + deTo) / 2;
        //        double ra = 22.0;

        //        List<IStar> stars = cat.GetStarsInRegion(ra, de, 0.2, 22, 2000);

        //        Assert.IsNotNull(stars, string.Format("No Stars in zone: {0}", zoneId));
        //        if (stars.Count > 0)
        //        {
        //            bool statsFromOtherZonesPresent = stars.Exists(s => s.StarNo < zoneId * 10000000 && s.StarNo >= (zoneId + 1) * 10000000);
        //            Assert.IsFalse(statsFromOtherZonesPresent);

        //            de = stars[stars.Count / 2].DEDeg;
        //            Assert.IsTrue(de >= deFrom && de < deTo);
        //        }
        //        else

        //            Console.WriteLine(string.Format("Zone {0} not tested", zoneId));

        //    }
        //}
    }
	#endif
}
