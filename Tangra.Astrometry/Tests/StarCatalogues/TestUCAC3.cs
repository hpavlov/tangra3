/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using NUnit.Framework;
using Tangra.Model.Helpers;
using Tangra.StarCatalogues;
using Tangra.StarCatalogues.UCAC3;

namespace Tangra.Tests.StarCatalogues
{
	#if !AUTOMATED_BUILD
    [TestFixture]
    public class TestUCAC3
    {
        private string CATALOG_LOCATION = @"F:\UCAC3\";

        [Test()]
        public void _1_TestFirstEntry()
        {
            IntPtr buffer = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(UCAC3Entry)));
            try
            {
                string fileName = CATALOG_LOCATION + @"z001";

                using (FileStream str = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (BinaryReader rdr = new BinaryReader(str))
                    {
                        UCAC3Entry firstEntry = ReadEntry(rdr, buffer);
                        firstEntry.InitUCAC3Entry(1, 1);

                        Assert.AreEqual("3U 001-0000001", firstEntry.GetStarDesignation(0));
                        Assert.IsTrue(Math.Abs(0.3415039 - firstEntry.RADeg) < 0.0000001, string.Format("Bad RA: {0} ", firstEntry.RADeg));
                        Assert.IsTrue(Math.Abs(-89.6576795 - firstEntry.DEDeg) < 0.0000001, string.Format("Bad DE: {0} ", firstEntry.DEDeg));

                        Assert.IsTrue(Math.Abs(-4.2 - firstEntry.ProperMotionDE) < 0.001, string.Format("Bad ProperMotionDE: {0} ", firstEntry.ProperMotionDE));
                        Assert.IsTrue(Math.Abs(1238.5796 - firstEntry.ProperMotionRA) < 0.0001, string.Format("Bad ProperMotionRA: {0} ", firstEntry.ProperMotionRA));

                        Assert.IsTrue(Math.Abs(16.24 /* Computed mag */ - firstEntry.MagV) < 0.001, string.Format("Bad MagV: {0} ", firstEntry.MagV));
                        Assert.IsTrue(Math.Abs(15.879 - firstEntry.MagR) < 0.0001, string.Format("Bad MagR: {0} ", firstEntry.MagR));
                        Assert.IsTrue(Math.Abs(16.48 /* fMag */  - firstEntry.Mag) < 0.0001, string.Format("Bad Mag: {0} ", firstEntry.Mag));
                        Assert.IsTrue(Math.Abs(17.017 - firstEntry.MagB) < 0.0001, string.Format("Bad MagB: {0} ", firstEntry.MagB));
                    }
                }
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }

        private UCAC3Entry ReadEntry(BinaryReader rdr, IntPtr buffer)
        {
            UCAC3Entry entry = new UCAC3Entry();
            byte[] rawData = rdr.ReadBytes(UCAC3Entry.Size);

            Marshal.Copy(rawData, 0, buffer, Marshal.SizeOf(entry));
            entry = (UCAC3Entry)Marshal.PtrToStructure(buffer, typeof(UCAC3Entry));
            entry.InitUCAC3Entry();

            return entry;
        }

        public void _2_GenerateIndex_VERY_LONG_DONT_DO_IT()
        {
            IntPtr buffer = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(UCAC3Entry)));
            try
            {
                string fileNameIndex = CATALOG_LOCATION + "zone_ra_index.bin";
                using (FileStream fs = new FileStream(fileNameIndex, FileMode.Create, FileAccess.Write))
                using (BinaryWriter bwrt = new BinaryWriter(fs))
                {
                    for (int i = 1; i <= 360; i++)
                    {
                        string fileName = Path.Combine(CATALOG_LOCATION, string.Format("z{0}", i.ToString("000")));

                        Console.WriteLine("Zone " + i.ToString());

                        using (FileStream str = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            int[] raIdx = new int[360];
                            int raCurr = 0;
                            int recNo = 0;
                            using (BinaryReader rdr = new BinaryReader(str))
                            {
                                do
                                {
                                    recNo++;
                                    UCAC3Entry entry = ReadEntry(rdr, buffer);
                                    int raTrunc = (int)entry.RADeg;

                                    while (raTrunc >= raCurr)
                                    {
                                        //Console.WriteLine("Index[" + raCurr.ToString()  +"] = " + recNo.ToString());
                                        raIdx[raCurr] = recNo;
                                        raCurr++;
                                    }
                                }
                                while (str.Length > recNo * UCAC3Entry.Size);
                            }

                            while (360 > raCurr)
                            {
                                raIdx[raCurr] = recNo - 1;
                                raCurr++;
                            }

                            for (int j = 0; j < 360; j++)
                            {
                                bwrt.Write(raIdx[j]);
                            }

                        }

                    }

                    bwrt.Flush();
                }
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }

		[Test]
		public void _3_Test_Zone_Ranges()
		{
			UCAC3Index index = UCAC3Index.GetIndex(CATALOG_LOCATION);
			Assert.IsNotNull(index);

			IntPtr buffer = Marshal.AllocHGlobal(Marshal.SizeOf(typeof (UCAC3Entry)));
			try
			{
				for (int i = 0; i < 360; i++)
				{
					UCAC3BinIndexEntry zoneIndex = index.ZoneIndex[i];
					Assert.AreEqual(zoneIndex.ZoneId, i + 1);

					string fileName = Path.Combine(CATALOG_LOCATION, string.Format("z{0}", zoneIndex.ZoneId.ToString("000")));

					//Console.WriteLine(string.Format("{0}, AstroConvert.ToDeclination(\"{1}\"), AstroConvert.ToDeclination(\"{2}\"),",
					//    zoneIndex.ZoneId, 

					//    AstroConvert.ToStringValue(zoneIndex.DEFrom, "DEC"),
					//    AstroConvert.ToStringValue(zoneIndex.DETo, "DEC")));

					using (FileStream str = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
					{
						using (BinaryReader rdr = new BinaryReader(str))
						{
							UCAC3Entry firstEntry = ReadEntry(rdr, buffer);
							Assert.IsTrue(firstEntry.DECat >= zoneIndex.DEFrom, string.Format("firstEntry.DECat ({0}) >= zoneIndex.DEFrom ({1})", firstEntry.DECat, zoneIndex.DEFrom));
							Assert.IsTrue(firstEntry.DECat <= zoneIndex.DETo, string.Format("firstEntry.DECat ({0}) <= zoneIndex.DETo ({1})", firstEntry.DECat, zoneIndex.DETo));
						}
					}
				}
			}
			finally
			{
				Marshal.FreeHGlobal(buffer);
			}


			#region RANGES

			double[] RANGES = new double[]
          	{
				1, AstroConvert.ToDeclination("-90:00:00"), AstroConvert.ToDeclination("-89:30:00"),
				2, AstroConvert.ToDeclination("-89:30:00"), AstroConvert.ToDeclination("-89:00:00"),
				3, AstroConvert.ToDeclination("-89:00:00"), AstroConvert.ToDeclination("-88:30:00"),
				4, AstroConvert.ToDeclination("-88:30:00"), AstroConvert.ToDeclination("-88:00:00"),
				5, AstroConvert.ToDeclination("-88:00:00"), AstroConvert.ToDeclination("-87:30:00"),
				6, AstroConvert.ToDeclination("-87:30:00"), AstroConvert.ToDeclination("-87:00:00"),
				7, AstroConvert.ToDeclination("-87:00:00"), AstroConvert.ToDeclination("-86:30:00"),
				8, AstroConvert.ToDeclination("-86:30:00"), AstroConvert.ToDeclination("-86:00:00"),
				9, AstroConvert.ToDeclination("-86:00:00"), AstroConvert.ToDeclination("-85:30:00"),
				10, AstroConvert.ToDeclination("-85:30:00"), AstroConvert.ToDeclination("-85:00:00"),
				11, AstroConvert.ToDeclination("-85:00:00"), AstroConvert.ToDeclination("-84:30:00"),
				12, AstroConvert.ToDeclination("-84:30:00"), AstroConvert.ToDeclination("-84:00:00"),
				13, AstroConvert.ToDeclination("-84:00:00"), AstroConvert.ToDeclination("-83:30:00"),
				14, AstroConvert.ToDeclination("-83:30:00"), AstroConvert.ToDeclination("-83:00:00"),
				15, AstroConvert.ToDeclination("-83:00:00"), AstroConvert.ToDeclination("-82:30:00"),
				16, AstroConvert.ToDeclination("-82:30:00"), AstroConvert.ToDeclination("-82:00:00"),
				17, AstroConvert.ToDeclination("-82:00:00"), AstroConvert.ToDeclination("-81:30:00"),
				18, AstroConvert.ToDeclination("-81:30:00"), AstroConvert.ToDeclination("-81:00:00"),
				19, AstroConvert.ToDeclination("-81:00:00"), AstroConvert.ToDeclination("-80:30:00"),
				20, AstroConvert.ToDeclination("-80:30:00"), AstroConvert.ToDeclination("-80:00:00"),
				21, AstroConvert.ToDeclination("-80:00:00"), AstroConvert.ToDeclination("-79:30:00"),
				22, AstroConvert.ToDeclination("-79:30:00"), AstroConvert.ToDeclination("-79:00:00"),
				23, AstroConvert.ToDeclination("-79:00:00"), AstroConvert.ToDeclination("-78:30:00"),
				24, AstroConvert.ToDeclination("-78:30:00"), AstroConvert.ToDeclination("-78:00:00"),
				25, AstroConvert.ToDeclination("-78:00:00"), AstroConvert.ToDeclination("-77:30:00"),
				26, AstroConvert.ToDeclination("-77:30:00"), AstroConvert.ToDeclination("-77:00:00"),
				27, AstroConvert.ToDeclination("-77:00:00"), AstroConvert.ToDeclination("-76:30:00"),
				28, AstroConvert.ToDeclination("-76:30:00"), AstroConvert.ToDeclination("-76:00:00"),
				29, AstroConvert.ToDeclination("-76:00:00"), AstroConvert.ToDeclination("-75:30:00"),
				30, AstroConvert.ToDeclination("-75:30:00"), AstroConvert.ToDeclination("-75:00:00"),
				31, AstroConvert.ToDeclination("-75:00:00"), AstroConvert.ToDeclination("-74:30:00"),
				32, AstroConvert.ToDeclination("-74:30:00"), AstroConvert.ToDeclination("-74:00:00"),
				33, AstroConvert.ToDeclination("-74:00:00"), AstroConvert.ToDeclination("-73:30:00"),
				34, AstroConvert.ToDeclination("-73:30:00"), AstroConvert.ToDeclination("-73:00:00"),
				35, AstroConvert.ToDeclination("-73:00:00"), AstroConvert.ToDeclination("-72:30:00"),
				36, AstroConvert.ToDeclination("-72:30:00"), AstroConvert.ToDeclination("-72:00:00"),
				37, AstroConvert.ToDeclination("-72:00:00"), AstroConvert.ToDeclination("-71:30:00"),
				38, AstroConvert.ToDeclination("-71:30:00"), AstroConvert.ToDeclination("-71:00:00"),
				39, AstroConvert.ToDeclination("-71:00:00"), AstroConvert.ToDeclination("-70:30:00"),
				40, AstroConvert.ToDeclination("-70:30:00"), AstroConvert.ToDeclination("-70:00:00"),
				41, AstroConvert.ToDeclination("-70:00:00"), AstroConvert.ToDeclination("-69:30:00"),
				42, AstroConvert.ToDeclination("-69:30:00"), AstroConvert.ToDeclination("-69:00:00"),
				43, AstroConvert.ToDeclination("-69:00:00"), AstroConvert.ToDeclination("-68:30:00"),
				44, AstroConvert.ToDeclination("-68:30:00"), AstroConvert.ToDeclination("-68:00:00"),
				45, AstroConvert.ToDeclination("-68:00:00"), AstroConvert.ToDeclination("-67:30:00"),
				46, AstroConvert.ToDeclination("-67:30:00"), AstroConvert.ToDeclination("-67:00:00"),
				47, AstroConvert.ToDeclination("-67:00:00"), AstroConvert.ToDeclination("-66:30:00"),
				48, AstroConvert.ToDeclination("-66:30:00"), AstroConvert.ToDeclination("-66:00:00"),
				49, AstroConvert.ToDeclination("-66:00:00"), AstroConvert.ToDeclination("-65:30:00"),
				50, AstroConvert.ToDeclination("-65:30:00"), AstroConvert.ToDeclination("-65:00:00"),
				51, AstroConvert.ToDeclination("-65:00:00"), AstroConvert.ToDeclination("-64:30:00"),
				52, AstroConvert.ToDeclination("-64:30:00"), AstroConvert.ToDeclination("-64:00:00"),
				53, AstroConvert.ToDeclination("-64:00:00"), AstroConvert.ToDeclination("-63:30:00"),
				54, AstroConvert.ToDeclination("-63:30:00"), AstroConvert.ToDeclination("-63:00:00"),
				55, AstroConvert.ToDeclination("-63:00:00"), AstroConvert.ToDeclination("-62:30:00"),
				56, AstroConvert.ToDeclination("-62:30:00"), AstroConvert.ToDeclination("-62:00:00"),
				57, AstroConvert.ToDeclination("-62:00:00"), AstroConvert.ToDeclination("-61:30:00"),
				58, AstroConvert.ToDeclination("-61:30:00"), AstroConvert.ToDeclination("-61:00:00"),
				59, AstroConvert.ToDeclination("-61:00:00"), AstroConvert.ToDeclination("-60:30:00"),
				60, AstroConvert.ToDeclination("-60:30:00"), AstroConvert.ToDeclination("-60:00:00"),
				61, AstroConvert.ToDeclination("-60:00:00"), AstroConvert.ToDeclination("-59:30:00"),
				62, AstroConvert.ToDeclination("-59:30:00"), AstroConvert.ToDeclination("-59:00:00"),
				63, AstroConvert.ToDeclination("-59:00:00"), AstroConvert.ToDeclination("-58:30:00"),
				64, AstroConvert.ToDeclination("-58:30:00"), AstroConvert.ToDeclination("-58:00:00"),
				65, AstroConvert.ToDeclination("-58:00:00"), AstroConvert.ToDeclination("-57:30:00"),
				66, AstroConvert.ToDeclination("-57:30:00"), AstroConvert.ToDeclination("-57:00:00"),
				67, AstroConvert.ToDeclination("-57:00:00"), AstroConvert.ToDeclination("-56:30:00"),
				68, AstroConvert.ToDeclination("-56:30:00"), AstroConvert.ToDeclination("-56:00:00"),
				69, AstroConvert.ToDeclination("-56:00:00"), AstroConvert.ToDeclination("-55:30:00"),
				70, AstroConvert.ToDeclination("-55:30:00"), AstroConvert.ToDeclination("-55:00:00"),
				71, AstroConvert.ToDeclination("-55:00:00"), AstroConvert.ToDeclination("-54:30:00"),
				72, AstroConvert.ToDeclination("-54:30:00"), AstroConvert.ToDeclination("-54:00:00"),
				73, AstroConvert.ToDeclination("-54:00:00"), AstroConvert.ToDeclination("-53:30:00"),
				74, AstroConvert.ToDeclination("-53:30:00"), AstroConvert.ToDeclination("-53:00:00"),
				75, AstroConvert.ToDeclination("-53:00:00"), AstroConvert.ToDeclination("-52:30:00"),
				76, AstroConvert.ToDeclination("-52:30:00"), AstroConvert.ToDeclination("-52:00:00"),
				77, AstroConvert.ToDeclination("-52:00:00"), AstroConvert.ToDeclination("-51:30:00"),
				78, AstroConvert.ToDeclination("-51:30:00"), AstroConvert.ToDeclination("-51:00:00"),
				79, AstroConvert.ToDeclination("-51:00:00"), AstroConvert.ToDeclination("-50:30:00"),
				80, AstroConvert.ToDeclination("-50:30:00"), AstroConvert.ToDeclination("-50:00:00"),
				81, AstroConvert.ToDeclination("-50:00:00"), AstroConvert.ToDeclination("-49:30:00"),
				82, AstroConvert.ToDeclination("-49:30:00"), AstroConvert.ToDeclination("-49:00:00"),
				83, AstroConvert.ToDeclination("-49:00:00"), AstroConvert.ToDeclination("-48:30:00"),
				84, AstroConvert.ToDeclination("-48:30:00"), AstroConvert.ToDeclination("-48:00:00"),
				85, AstroConvert.ToDeclination("-48:00:00"), AstroConvert.ToDeclination("-47:30:00"),
				86, AstroConvert.ToDeclination("-47:30:00"), AstroConvert.ToDeclination("-47:00:00"),
				87, AstroConvert.ToDeclination("-47:00:00"), AstroConvert.ToDeclination("-46:30:00"),
				88, AstroConvert.ToDeclination("-46:30:00"), AstroConvert.ToDeclination("-46:00:00"),
				89, AstroConvert.ToDeclination("-46:00:00"), AstroConvert.ToDeclination("-45:30:00"),
				90, AstroConvert.ToDeclination("-45:30:00"), AstroConvert.ToDeclination("-45:00:00"),
				91, AstroConvert.ToDeclination("-45:00:00"), AstroConvert.ToDeclination("-44:30:00"),
				92, AstroConvert.ToDeclination("-44:30:00"), AstroConvert.ToDeclination("-44:00:00"),
				93, AstroConvert.ToDeclination("-44:00:00"), AstroConvert.ToDeclination("-43:30:00"),
				94, AstroConvert.ToDeclination("-43:30:00"), AstroConvert.ToDeclination("-43:00:00"),
				95, AstroConvert.ToDeclination("-43:00:00"), AstroConvert.ToDeclination("-42:30:00"),
				96, AstroConvert.ToDeclination("-42:30:00"), AstroConvert.ToDeclination("-42:00:00"),
				97, AstroConvert.ToDeclination("-42:00:00"), AstroConvert.ToDeclination("-41:30:00"),
				98, AstroConvert.ToDeclination("-41:30:00"), AstroConvert.ToDeclination("-41:00:00"),
				99, AstroConvert.ToDeclination("-41:00:00"), AstroConvert.ToDeclination("-40:30:00"),
				100, AstroConvert.ToDeclination("-40:30:00"), AstroConvert.ToDeclination("-40:00:00"),
				101, AstroConvert.ToDeclination("-40:00:00"), AstroConvert.ToDeclination("-39:30:00"),
				102, AstroConvert.ToDeclination("-39:30:00"), AstroConvert.ToDeclination("-39:00:00"),
				103, AstroConvert.ToDeclination("-39:00:00"), AstroConvert.ToDeclination("-38:30:00"),
				104, AstroConvert.ToDeclination("-38:30:00"), AstroConvert.ToDeclination("-38:00:00"),
				105, AstroConvert.ToDeclination("-38:00:00"), AstroConvert.ToDeclination("-37:30:00"),
				106, AstroConvert.ToDeclination("-37:30:00"), AstroConvert.ToDeclination("-37:00:00"),
				107, AstroConvert.ToDeclination("-37:00:00"), AstroConvert.ToDeclination("-36:30:00"),
				108, AstroConvert.ToDeclination("-36:30:00"), AstroConvert.ToDeclination("-36:00:00"),
				109, AstroConvert.ToDeclination("-36:00:00"), AstroConvert.ToDeclination("-35:30:00"),
				110, AstroConvert.ToDeclination("-35:30:00"), AstroConvert.ToDeclination("-35:00:00"),
				111, AstroConvert.ToDeclination("-35:00:00"), AstroConvert.ToDeclination("-34:30:00"),
				112, AstroConvert.ToDeclination("-34:30:00"), AstroConvert.ToDeclination("-34:00:00"),
				113, AstroConvert.ToDeclination("-34:00:00"), AstroConvert.ToDeclination("-33:30:00"),
				114, AstroConvert.ToDeclination("-33:30:00"), AstroConvert.ToDeclination("-33:00:00"),
				115, AstroConvert.ToDeclination("-33:00:00"), AstroConvert.ToDeclination("-32:30:00"),
				116, AstroConvert.ToDeclination("-32:30:00"), AstroConvert.ToDeclination("-32:00:00"),
				117, AstroConvert.ToDeclination("-32:00:00"), AstroConvert.ToDeclination("-31:30:00"),
				118, AstroConvert.ToDeclination("-31:30:00"), AstroConvert.ToDeclination("-31:00:00"),
				119, AstroConvert.ToDeclination("-31:00:00"), AstroConvert.ToDeclination("-30:30:00"),
				120, AstroConvert.ToDeclination("-30:30:00"), AstroConvert.ToDeclination("-30:00:00"),
				121, AstroConvert.ToDeclination("-30:00:00"), AstroConvert.ToDeclination("-29:30:00"),
				122, AstroConvert.ToDeclination("-29:30:00"), AstroConvert.ToDeclination("-29:00:00"),
				123, AstroConvert.ToDeclination("-29:00:00"), AstroConvert.ToDeclination("-28:30:00"),
				124, AstroConvert.ToDeclination("-28:30:00"), AstroConvert.ToDeclination("-28:00:00"),
				125, AstroConvert.ToDeclination("-28:00:00"), AstroConvert.ToDeclination("-27:30:00"),
				126, AstroConvert.ToDeclination("-27:30:00"), AstroConvert.ToDeclination("-27:00:00"),
				127, AstroConvert.ToDeclination("-27:00:00"), AstroConvert.ToDeclination("-26:30:00"),
				128, AstroConvert.ToDeclination("-26:30:00"), AstroConvert.ToDeclination("-26:00:00"),
				129, AstroConvert.ToDeclination("-26:00:00"), AstroConvert.ToDeclination("-25:30:00"),
				130, AstroConvert.ToDeclination("-25:30:00"), AstroConvert.ToDeclination("-25:00:00"),
				131, AstroConvert.ToDeclination("-25:00:00"), AstroConvert.ToDeclination("-24:30:00"),
				132, AstroConvert.ToDeclination("-24:30:00"), AstroConvert.ToDeclination("-24:00:00"),
				133, AstroConvert.ToDeclination("-24:00:00"), AstroConvert.ToDeclination("-23:30:00"),
				134, AstroConvert.ToDeclination("-23:30:00"), AstroConvert.ToDeclination("-23:00:00"),
				135, AstroConvert.ToDeclination("-23:00:00"), AstroConvert.ToDeclination("-22:30:00"),
				136, AstroConvert.ToDeclination("-22:30:00"), AstroConvert.ToDeclination("-22:00:00"),
				137, AstroConvert.ToDeclination("-22:00:00"), AstroConvert.ToDeclination("-21:30:00"),
				138, AstroConvert.ToDeclination("-21:30:00"), AstroConvert.ToDeclination("-21:00:00"),
				139, AstroConvert.ToDeclination("-21:00:00"), AstroConvert.ToDeclination("-20:30:00"),
				140, AstroConvert.ToDeclination("-20:30:00"), AstroConvert.ToDeclination("-20:00:00"),
				141, AstroConvert.ToDeclination("-20:00:00"), AstroConvert.ToDeclination("-19:30:00"),
				142, AstroConvert.ToDeclination("-19:30:00"), AstroConvert.ToDeclination("-19:00:00"),
				143, AstroConvert.ToDeclination("-19:00:00"), AstroConvert.ToDeclination("-18:30:00"),
				144, AstroConvert.ToDeclination("-18:30:00"), AstroConvert.ToDeclination("-18:00:00"),
				145, AstroConvert.ToDeclination("-18:00:00"), AstroConvert.ToDeclination("-17:30:00"),
				146, AstroConvert.ToDeclination("-17:30:00"), AstroConvert.ToDeclination("-17:00:00"),
				147, AstroConvert.ToDeclination("-17:00:00"), AstroConvert.ToDeclination("-16:30:00"),
				148, AstroConvert.ToDeclination("-16:30:00"), AstroConvert.ToDeclination("-16:00:00"),
				149, AstroConvert.ToDeclination("-16:00:00"), AstroConvert.ToDeclination("-15:30:00"),
				150, AstroConvert.ToDeclination("-15:30:00"), AstroConvert.ToDeclination("-15:00:00"),
				151, AstroConvert.ToDeclination("-15:00:00"), AstroConvert.ToDeclination("-14:30:00"),
				152, AstroConvert.ToDeclination("-14:30:00"), AstroConvert.ToDeclination("-14:00:00"),
				153, AstroConvert.ToDeclination("-14:00:00"), AstroConvert.ToDeclination("-13:30:00"),
				154, AstroConvert.ToDeclination("-13:30:00"), AstroConvert.ToDeclination("-13:00:00"),
				155, AstroConvert.ToDeclination("-13:00:00"), AstroConvert.ToDeclination("-12:30:00"),
				156, AstroConvert.ToDeclination("-12:30:00"), AstroConvert.ToDeclination("-12:00:00"),
				157, AstroConvert.ToDeclination("-12:00:00"), AstroConvert.ToDeclination("-11:30:00"),
				158, AstroConvert.ToDeclination("-11:30:00"), AstroConvert.ToDeclination("-11:00:00"),
				159, AstroConvert.ToDeclination("-11:00:00"), AstroConvert.ToDeclination("-10:30:00"),
				160, AstroConvert.ToDeclination("-10:30:00"), AstroConvert.ToDeclination("-10:00:00"),
				161, AstroConvert.ToDeclination("-10:00:00"), AstroConvert.ToDeclination("-09:30:00"),
				162, AstroConvert.ToDeclination("-09:30:00"), AstroConvert.ToDeclination("-09:00:00"),
				163, AstroConvert.ToDeclination("-09:00:00"), AstroConvert.ToDeclination("-08:30:00"),
				164, AstroConvert.ToDeclination("-08:30:00"), AstroConvert.ToDeclination("-08:00:00"),
				165, AstroConvert.ToDeclination("-08:00:00"), AstroConvert.ToDeclination("-07:30:00"),
				166, AstroConvert.ToDeclination("-07:30:00"), AstroConvert.ToDeclination("-07:00:00"),
				167, AstroConvert.ToDeclination("-07:00:00"), AstroConvert.ToDeclination("-06:30:00"),
				168, AstroConvert.ToDeclination("-06:30:00"), AstroConvert.ToDeclination("-06:00:00"),
				169, AstroConvert.ToDeclination("-06:00:00"), AstroConvert.ToDeclination("-05:30:00"),
				170, AstroConvert.ToDeclination("-05:30:00"), AstroConvert.ToDeclination("-05:00:00"),
				171, AstroConvert.ToDeclination("-05:00:00"), AstroConvert.ToDeclination("-04:30:00"),
				172, AstroConvert.ToDeclination("-04:30:00"), AstroConvert.ToDeclination("-04:00:00"),
				173, AstroConvert.ToDeclination("-04:00:00"), AstroConvert.ToDeclination("-03:30:00"),
				174, AstroConvert.ToDeclination("-03:30:00"), AstroConvert.ToDeclination("-03:00:00"),
				175, AstroConvert.ToDeclination("-03:00:00"), AstroConvert.ToDeclination("-02:30:00"),
				176, AstroConvert.ToDeclination("-02:30:00"), AstroConvert.ToDeclination("-02:00:00"),
				177, AstroConvert.ToDeclination("-02:00:00"), AstroConvert.ToDeclination("-01:30:00"),
				178, AstroConvert.ToDeclination("-01:30:00"), AstroConvert.ToDeclination("-01:00:00"),
				179, AstroConvert.ToDeclination("-01:00:00"), AstroConvert.ToDeclination("-00:30:00"),
				180, AstroConvert.ToDeclination("-00:30:00"), AstroConvert.ToDeclination("-00:00:00"),
				181, AstroConvert.ToDeclination("-00:00:00"), AstroConvert.ToDeclination("+00:30:00"),
				182, AstroConvert.ToDeclination("+00:30:00"), AstroConvert.ToDeclination("+01:00:00"),
				183, AstroConvert.ToDeclination("+01:00:00"), AstroConvert.ToDeclination("+01:30:00"),
				184, AstroConvert.ToDeclination("+01:30:00"), AstroConvert.ToDeclination("+02:00:00"),
				185, AstroConvert.ToDeclination("+02:00:00"), AstroConvert.ToDeclination("+02:30:00"),
				186, AstroConvert.ToDeclination("+02:30:00"), AstroConvert.ToDeclination("+03:00:00"),
				187, AstroConvert.ToDeclination("+03:00:00"), AstroConvert.ToDeclination("+03:30:00"),
				188, AstroConvert.ToDeclination("+03:30:00"), AstroConvert.ToDeclination("+04:00:00"),
				189, AstroConvert.ToDeclination("+04:00:00"), AstroConvert.ToDeclination("+04:30:00"),
				190, AstroConvert.ToDeclination("+04:30:00"), AstroConvert.ToDeclination("+05:00:00"),
				191, AstroConvert.ToDeclination("+05:00:00"), AstroConvert.ToDeclination("+05:30:00"),
				192, AstroConvert.ToDeclination("+05:30:00"), AstroConvert.ToDeclination("+06:00:00"),
				193, AstroConvert.ToDeclination("+06:00:00"), AstroConvert.ToDeclination("+06:30:00"),
				194, AstroConvert.ToDeclination("+06:30:00"), AstroConvert.ToDeclination("+07:00:00"),
				195, AstroConvert.ToDeclination("+07:00:00"), AstroConvert.ToDeclination("+07:30:00"),
				196, AstroConvert.ToDeclination("+07:30:00"), AstroConvert.ToDeclination("+08:00:00"),
				197, AstroConvert.ToDeclination("+08:00:00"), AstroConvert.ToDeclination("+08:30:00"),
				198, AstroConvert.ToDeclination("+08:30:00"), AstroConvert.ToDeclination("+09:00:00"),
				199, AstroConvert.ToDeclination("+09:00:00"), AstroConvert.ToDeclination("+09:30:00"),
				200, AstroConvert.ToDeclination("+09:30:00"), AstroConvert.ToDeclination("+10:00:00"),
				201, AstroConvert.ToDeclination("+10:00:00"), AstroConvert.ToDeclination("+10:30:00"),
				202, AstroConvert.ToDeclination("+10:30:00"), AstroConvert.ToDeclination("+11:00:00"),
				203, AstroConvert.ToDeclination("+11:00:00"), AstroConvert.ToDeclination("+11:30:00"),
				204, AstroConvert.ToDeclination("+11:30:00"), AstroConvert.ToDeclination("+12:00:00"),
				205, AstroConvert.ToDeclination("+12:00:00"), AstroConvert.ToDeclination("+12:30:00"),
				206, AstroConvert.ToDeclination("+12:30:00"), AstroConvert.ToDeclination("+13:00:00"),
				207, AstroConvert.ToDeclination("+13:00:00"), AstroConvert.ToDeclination("+13:30:00"),
				208, AstroConvert.ToDeclination("+13:30:00"), AstroConvert.ToDeclination("+14:00:00"),
				209, AstroConvert.ToDeclination("+14:00:00"), AstroConvert.ToDeclination("+14:30:00"),
				210, AstroConvert.ToDeclination("+14:30:00"), AstroConvert.ToDeclination("+15:00:00"),
				211, AstroConvert.ToDeclination("+15:00:00"), AstroConvert.ToDeclination("+15:30:00"),
				212, AstroConvert.ToDeclination("+15:30:00"), AstroConvert.ToDeclination("+16:00:00"),
				213, AstroConvert.ToDeclination("+16:00:00"), AstroConvert.ToDeclination("+16:30:00"),
				214, AstroConvert.ToDeclination("+16:30:00"), AstroConvert.ToDeclination("+17:00:00"),
				215, AstroConvert.ToDeclination("+17:00:00"), AstroConvert.ToDeclination("+17:30:00"),
				216, AstroConvert.ToDeclination("+17:30:00"), AstroConvert.ToDeclination("+18:00:00"),
				217, AstroConvert.ToDeclination("+18:00:00"), AstroConvert.ToDeclination("+18:30:00"),
				218, AstroConvert.ToDeclination("+18:30:00"), AstroConvert.ToDeclination("+19:00:00"),
				219, AstroConvert.ToDeclination("+19:00:00"), AstroConvert.ToDeclination("+19:30:00"),
				220, AstroConvert.ToDeclination("+19:30:00"), AstroConvert.ToDeclination("+20:00:00"),
				221, AstroConvert.ToDeclination("+20:00:00"), AstroConvert.ToDeclination("+20:30:00"),
				222, AstroConvert.ToDeclination("+20:30:00"), AstroConvert.ToDeclination("+21:00:00"),
				223, AstroConvert.ToDeclination("+21:00:00"), AstroConvert.ToDeclination("+21:30:00"),
				224, AstroConvert.ToDeclination("+21:30:00"), AstroConvert.ToDeclination("+22:00:00"),
				225, AstroConvert.ToDeclination("+22:00:00"), AstroConvert.ToDeclination("+22:30:00"),
				226, AstroConvert.ToDeclination("+22:30:00"), AstroConvert.ToDeclination("+23:00:00"),
				227, AstroConvert.ToDeclination("+23:00:00"), AstroConvert.ToDeclination("+23:30:00"),
				228, AstroConvert.ToDeclination("+23:30:00"), AstroConvert.ToDeclination("+24:00:00"),
				229, AstroConvert.ToDeclination("+24:00:00"), AstroConvert.ToDeclination("+24:30:00"),
				230, AstroConvert.ToDeclination("+24:30:00"), AstroConvert.ToDeclination("+25:00:00"),
				231, AstroConvert.ToDeclination("+25:00:00"), AstroConvert.ToDeclination("+25:30:00"),
				232, AstroConvert.ToDeclination("+25:30:00"), AstroConvert.ToDeclination("+26:00:00"),
				233, AstroConvert.ToDeclination("+26:00:00"), AstroConvert.ToDeclination("+26:30:00"),
				234, AstroConvert.ToDeclination("+26:30:00"), AstroConvert.ToDeclination("+27:00:00"),
				235, AstroConvert.ToDeclination("+27:00:00"), AstroConvert.ToDeclination("+27:30:00"),
				236, AstroConvert.ToDeclination("+27:30:00"), AstroConvert.ToDeclination("+28:00:00"),
				237, AstroConvert.ToDeclination("+28:00:00"), AstroConvert.ToDeclination("+28:30:00"),
				238, AstroConvert.ToDeclination("+28:30:00"), AstroConvert.ToDeclination("+29:00:00"),
				239, AstroConvert.ToDeclination("+29:00:00"), AstroConvert.ToDeclination("+29:30:00"),
				240, AstroConvert.ToDeclination("+29:30:00"), AstroConvert.ToDeclination("+30:00:00"),
				241, AstroConvert.ToDeclination("+30:00:00"), AstroConvert.ToDeclination("+30:30:00"),
				242, AstroConvert.ToDeclination("+30:30:00"), AstroConvert.ToDeclination("+31:00:00"),
				243, AstroConvert.ToDeclination("+31:00:00"), AstroConvert.ToDeclination("+31:30:00"),
				244, AstroConvert.ToDeclination("+31:30:00"), AstroConvert.ToDeclination("+32:00:00"),
				245, AstroConvert.ToDeclination("+32:00:00"), AstroConvert.ToDeclination("+32:30:00"),
				246, AstroConvert.ToDeclination("+32:30:00"), AstroConvert.ToDeclination("+33:00:00"),
				247, AstroConvert.ToDeclination("+33:00:00"), AstroConvert.ToDeclination("+33:30:00"),
				248, AstroConvert.ToDeclination("+33:30:00"), AstroConvert.ToDeclination("+34:00:00"),
				249, AstroConvert.ToDeclination("+34:00:00"), AstroConvert.ToDeclination("+34:30:00"),
				250, AstroConvert.ToDeclination("+34:30:00"), AstroConvert.ToDeclination("+35:00:00"),
				251, AstroConvert.ToDeclination("+35:00:00"), AstroConvert.ToDeclination("+35:30:00"),
				252, AstroConvert.ToDeclination("+35:30:00"), AstroConvert.ToDeclination("+36:00:00"),
				253, AstroConvert.ToDeclination("+36:00:00"), AstroConvert.ToDeclination("+36:30:00"),
				254, AstroConvert.ToDeclination("+36:30:00"), AstroConvert.ToDeclination("+37:00:00"),
				255, AstroConvert.ToDeclination("+37:00:00"), AstroConvert.ToDeclination("+37:30:00"),
				256, AstroConvert.ToDeclination("+37:30:00"), AstroConvert.ToDeclination("+38:00:00"),
				257, AstroConvert.ToDeclination("+38:00:00"), AstroConvert.ToDeclination("+38:30:00"),
				258, AstroConvert.ToDeclination("+38:30:00"), AstroConvert.ToDeclination("+39:00:00"),
				259, AstroConvert.ToDeclination("+39:00:00"), AstroConvert.ToDeclination("+39:30:00"),
				260, AstroConvert.ToDeclination("+39:30:00"), AstroConvert.ToDeclination("+40:00:00"),
				261, AstroConvert.ToDeclination("+40:00:00"), AstroConvert.ToDeclination("+40:30:00"),
				262, AstroConvert.ToDeclination("+40:30:00"), AstroConvert.ToDeclination("+41:00:00"),
				263, AstroConvert.ToDeclination("+41:00:00"), AstroConvert.ToDeclination("+41:30:00"),
				264, AstroConvert.ToDeclination("+41:30:00"), AstroConvert.ToDeclination("+42:00:00"),
				265, AstroConvert.ToDeclination("+42:00:00"), AstroConvert.ToDeclination("+42:30:00"),
				266, AstroConvert.ToDeclination("+42:30:00"), AstroConvert.ToDeclination("+43:00:00"),
				267, AstroConvert.ToDeclination("+43:00:00"), AstroConvert.ToDeclination("+43:30:00"),
				268, AstroConvert.ToDeclination("+43:30:00"), AstroConvert.ToDeclination("+44:00:00"),
				269, AstroConvert.ToDeclination("+44:00:00"), AstroConvert.ToDeclination("+44:30:00"),
				270, AstroConvert.ToDeclination("+44:30:00"), AstroConvert.ToDeclination("+45:00:00"),
				271, AstroConvert.ToDeclination("+45:00:00"), AstroConvert.ToDeclination("+45:30:00"),
				272, AstroConvert.ToDeclination("+45:30:00"), AstroConvert.ToDeclination("+46:00:00"),
				273, AstroConvert.ToDeclination("+46:00:00"), AstroConvert.ToDeclination("+46:30:00"),
				274, AstroConvert.ToDeclination("+46:30:00"), AstroConvert.ToDeclination("+47:00:00"),
				275, AstroConvert.ToDeclination("+47:00:00"), AstroConvert.ToDeclination("+47:30:00"),
				276, AstroConvert.ToDeclination("+47:30:00"), AstroConvert.ToDeclination("+48:00:00"),
				277, AstroConvert.ToDeclination("+48:00:00"), AstroConvert.ToDeclination("+48:30:00"),
				278, AstroConvert.ToDeclination("+48:30:00"), AstroConvert.ToDeclination("+49:00:00"),
				279, AstroConvert.ToDeclination("+49:00:00"), AstroConvert.ToDeclination("+49:30:00"),
				280, AstroConvert.ToDeclination("+49:30:00"), AstroConvert.ToDeclination("+50:00:00"),
				281, AstroConvert.ToDeclination("+50:00:00"), AstroConvert.ToDeclination("+50:30:00"),
				282, AstroConvert.ToDeclination("+50:30:00"), AstroConvert.ToDeclination("+51:00:00"),
				283, AstroConvert.ToDeclination("+51:00:00"), AstroConvert.ToDeclination("+51:30:00"),
				284, AstroConvert.ToDeclination("+51:30:00"), AstroConvert.ToDeclination("+52:00:00"),
				285, AstroConvert.ToDeclination("+52:00:00"), AstroConvert.ToDeclination("+52:30:00"),
				286, AstroConvert.ToDeclination("+52:30:00"), AstroConvert.ToDeclination("+53:00:00"),
				287, AstroConvert.ToDeclination("+53:00:00"), AstroConvert.ToDeclination("+53:30:00"),
				288, AstroConvert.ToDeclination("+53:30:00"), AstroConvert.ToDeclination("+54:00:00"),
				289, AstroConvert.ToDeclination("+54:00:00"), AstroConvert.ToDeclination("+54:30:00"),
				290, AstroConvert.ToDeclination("+54:30:00"), AstroConvert.ToDeclination("+55:00:00"),
				291, AstroConvert.ToDeclination("+55:00:00"), AstroConvert.ToDeclination("+55:30:00"),
				292, AstroConvert.ToDeclination("+55:30:00"), AstroConvert.ToDeclination("+56:00:00"),
				293, AstroConvert.ToDeclination("+56:00:00"), AstroConvert.ToDeclination("+56:30:00"),
				294, AstroConvert.ToDeclination("+56:30:00"), AstroConvert.ToDeclination("+57:00:00"),
				295, AstroConvert.ToDeclination("+57:00:00"), AstroConvert.ToDeclination("+57:30:00"),
				296, AstroConvert.ToDeclination("+57:30:00"), AstroConvert.ToDeclination("+58:00:00"),
				297, AstroConvert.ToDeclination("+58:00:00"), AstroConvert.ToDeclination("+58:30:00"),
				298, AstroConvert.ToDeclination("+58:30:00"), AstroConvert.ToDeclination("+59:00:00"),
				299, AstroConvert.ToDeclination("+59:00:00"), AstroConvert.ToDeclination("+59:30:00"),
				300, AstroConvert.ToDeclination("+59:30:00"), AstroConvert.ToDeclination("+60:00:00"),
				301, AstroConvert.ToDeclination("+60:00:00"), AstroConvert.ToDeclination("+60:30:00"),
				302, AstroConvert.ToDeclination("+60:30:00"), AstroConvert.ToDeclination("+61:00:00"),
				303, AstroConvert.ToDeclination("+61:00:00"), AstroConvert.ToDeclination("+61:30:00"),
				304, AstroConvert.ToDeclination("+61:30:00"), AstroConvert.ToDeclination("+62:00:00"),
				305, AstroConvert.ToDeclination("+62:00:00"), AstroConvert.ToDeclination("+62:30:00"),
				306, AstroConvert.ToDeclination("+62:30:00"), AstroConvert.ToDeclination("+63:00:00"),
				307, AstroConvert.ToDeclination("+63:00:00"), AstroConvert.ToDeclination("+63:30:00"),
				308, AstroConvert.ToDeclination("+63:30:00"), AstroConvert.ToDeclination("+64:00:00"),
				309, AstroConvert.ToDeclination("+64:00:00"), AstroConvert.ToDeclination("+64:30:00"),
				310, AstroConvert.ToDeclination("+64:30:00"), AstroConvert.ToDeclination("+65:00:00"),
				311, AstroConvert.ToDeclination("+65:00:00"), AstroConvert.ToDeclination("+65:30:00"),
				312, AstroConvert.ToDeclination("+65:30:00"), AstroConvert.ToDeclination("+66:00:00"),
				313, AstroConvert.ToDeclination("+66:00:00"), AstroConvert.ToDeclination("+66:30:00"),
				314, AstroConvert.ToDeclination("+66:30:00"), AstroConvert.ToDeclination("+67:00:00"),
				315, AstroConvert.ToDeclination("+67:00:00"), AstroConvert.ToDeclination("+67:30:00"),
				316, AstroConvert.ToDeclination("+67:30:00"), AstroConvert.ToDeclination("+68:00:00"),
				317, AstroConvert.ToDeclination("+68:00:00"), AstroConvert.ToDeclination("+68:30:00"),
				318, AstroConvert.ToDeclination("+68:30:00"), AstroConvert.ToDeclination("+69:00:00"),
				319, AstroConvert.ToDeclination("+69:00:00"), AstroConvert.ToDeclination("+69:30:00"),
				320, AstroConvert.ToDeclination("+69:30:00"), AstroConvert.ToDeclination("+70:00:00"),
				321, AstroConvert.ToDeclination("+70:00:00"), AstroConvert.ToDeclination("+70:30:00"),
				322, AstroConvert.ToDeclination("+70:30:00"), AstroConvert.ToDeclination("+71:00:00"),
				323, AstroConvert.ToDeclination("+71:00:00"), AstroConvert.ToDeclination("+71:30:00"),
				324, AstroConvert.ToDeclination("+71:30:00"), AstroConvert.ToDeclination("+72:00:00"),
				325, AstroConvert.ToDeclination("+72:00:00"), AstroConvert.ToDeclination("+72:30:00"),
				326, AstroConvert.ToDeclination("+72:30:00"), AstroConvert.ToDeclination("+73:00:00"),
				327, AstroConvert.ToDeclination("+73:00:00"), AstroConvert.ToDeclination("+73:30:00"),
				328, AstroConvert.ToDeclination("+73:30:00"), AstroConvert.ToDeclination("+74:00:00"),
				329, AstroConvert.ToDeclination("+74:00:00"), AstroConvert.ToDeclination("+74:30:00"),
				330, AstroConvert.ToDeclination("+74:30:00"), AstroConvert.ToDeclination("+75:00:00"),
				331, AstroConvert.ToDeclination("+75:00:00"), AstroConvert.ToDeclination("+75:30:00"),
				332, AstroConvert.ToDeclination("+75:30:00"), AstroConvert.ToDeclination("+76:00:00"),
				333, AstroConvert.ToDeclination("+76:00:00"), AstroConvert.ToDeclination("+76:30:00"),
				334, AstroConvert.ToDeclination("+76:30:00"), AstroConvert.ToDeclination("+77:00:00"),
				335, AstroConvert.ToDeclination("+77:00:00"), AstroConvert.ToDeclination("+77:30:00"),
				336, AstroConvert.ToDeclination("+77:30:00"), AstroConvert.ToDeclination("+78:00:00"),
				337, AstroConvert.ToDeclination("+78:00:00"), AstroConvert.ToDeclination("+78:30:00"),
				338, AstroConvert.ToDeclination("+78:30:00"), AstroConvert.ToDeclination("+79:00:00"),
				339, AstroConvert.ToDeclination("+79:00:00"), AstroConvert.ToDeclination("+79:30:00"),
				340, AstroConvert.ToDeclination("+79:30:00"), AstroConvert.ToDeclination("+80:00:00"),
				341, AstroConvert.ToDeclination("+80:00:00"), AstroConvert.ToDeclination("+80:30:00"),
				342, AstroConvert.ToDeclination("+80:30:00"), AstroConvert.ToDeclination("+81:00:00"),
				343, AstroConvert.ToDeclination("+81:00:00"), AstroConvert.ToDeclination("+81:30:00"),
				344, AstroConvert.ToDeclination("+81:30:00"), AstroConvert.ToDeclination("+82:00:00"),
				345, AstroConvert.ToDeclination("+82:00:00"), AstroConvert.ToDeclination("+82:30:00"),
				346, AstroConvert.ToDeclination("+82:30:00"), AstroConvert.ToDeclination("+83:00:00"),
				347, AstroConvert.ToDeclination("+83:00:00"), AstroConvert.ToDeclination("+83:30:00"),
				348, AstroConvert.ToDeclination("+83:30:00"), AstroConvert.ToDeclination("+84:00:00"),
				349, AstroConvert.ToDeclination("+84:00:00"), AstroConvert.ToDeclination("+84:30:00"),
				350, AstroConvert.ToDeclination("+84:30:00"), AstroConvert.ToDeclination("+85:00:00"),
				351, AstroConvert.ToDeclination("+85:00:00"), AstroConvert.ToDeclination("+85:30:00"),
				352, AstroConvert.ToDeclination("+85:30:00"), AstroConvert.ToDeclination("+86:00:00"),
				353, AstroConvert.ToDeclination("+86:00:00"), AstroConvert.ToDeclination("+86:30:00"),
				354, AstroConvert.ToDeclination("+86:30:00"), AstroConvert.ToDeclination("+87:00:00"),
				355, AstroConvert.ToDeclination("+87:00:00"), AstroConvert.ToDeclination("+87:30:00"),
				356, AstroConvert.ToDeclination("+87:30:00"), AstroConvert.ToDeclination("+88:00:00"),
				357, AstroConvert.ToDeclination("+88:00:00"), AstroConvert.ToDeclination("+88:30:00"),
				358, AstroConvert.ToDeclination("+88:30:00"), AstroConvert.ToDeclination("+89:00:00"),
				359, AstroConvert.ToDeclination("+89:00:00"), AstroConvert.ToDeclination("+89:30:00"),
				360, AstroConvert.ToDeclination("+89:30:00"), AstroConvert.ToDeclination("+90:00:00")
          	};
			#endregion

			for (int i = 0; i < RANGES.Length / 3; i++)
			{
				int zoneId = (int) RANGES[3*i];
				double deFrom = RANGES[3 * i + 1];
				double deTo = RANGES[3 * i + 2];

				UCAC3Catalogue cat = new UCAC3Catalogue(CATALOG_LOCATION);

				double de = (deFrom + deTo) / 2;
				double ra = 22.0;

				List<IStar> stars = cat.GetStarsInRegion(ra, de, 0.2, 22, 2000);

				Assert.IsNotNull(stars, string.Format("No Stars in zone: {0}", zoneId));
				if (stars.Count > 0)
				{
					bool statsFromOtherZonesPresent = stars.Exists(s => s.StarNo < (ulong)(zoneId * 10000000) && s.StarNo >= (ulong)((zoneId + 1) * 10000000));
					Assert.IsFalse(statsFromOtherZonesPresent);

					de = stars[stars.Count / 2].DEDeg;
					Assert.IsTrue(de >= deFrom && de < deTo);
				}
				else

					Console.WriteLine(string.Format("Zone {0} not tested", zoneId));

			}
		}


        [Test()]
        public void _3_LoadIndex_LongCheckTest_350sec()
        {
            UCAC3Index index = UCAC3Index.GetIndex(CATALOG_LOCATION);
            Assert.IsNotNull(index);

            IntPtr buffer = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(UCAC3Entry)));
            try
            {
                for (int i = 0; i < 360; i++)
                {
                    UCAC3BinIndexEntry zoneIndex = index.ZoneIndex[i];
                    Assert.AreEqual(zoneIndex.ZoneId, i + 1);

                    string fileName = Path.Combine(CATALOG_LOCATION, string.Format("z{0}", zoneIndex.ZoneId.ToString("000")));

                    Console.WriteLine(i.ToString());
                    using (FileStream str = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        using (BinaryReader rdr = new BinaryReader(str))
                        {
                            UCAC3Entry firstEntry = ReadEntry(rdr, buffer);
                            Assert.IsTrue(firstEntry.DECat >= zoneIndex.DEFrom, string.Format("firstEntry.DECat ({0}) >= zoneIndex.DEFrom ({1})", firstEntry.DECat, zoneIndex.DEFrom));
                            Assert.IsTrue(firstEntry.DECat <= zoneIndex.DETo, string.Format("firstEntry.DECat ({0}) <= zoneIndex.DETo ({1})", firstEntry.DECat, zoneIndex.DETo));

                            for (int j = 0; j < 360; j++)
                            {
                                double raValFrom = j;
                                double raValTo = (j + 1);

                                uint raIdxPrev = 0;
                                uint raIdx = j != 359 ? (uint)(index.ZoneIndex[i].RAStartPositions[j + 1] - 1) : (uint)index.ZoneIndex[i].TotalStarsInZone;
                                if (j != 0)
                                    raIdxPrev = (uint)(index.ZoneIndex[i].RAStartPositions[j] - 1);

                                if (raIdxPrev == raIdx) continue; /* no stars in this bin */

                                rdr.BaseStream.Position = raIdxPrev * UCAC3Entry.Size;
                                UCAC3Entry firstIndexEntry = ReadEntry(rdr, buffer);

                                Assert.IsTrue(firstIndexEntry.RAJ2000 >= raValFrom, string.Format("firstIndexEntry: {0} >= {1}; ZoneId={2}; jj={3}", firstIndexEntry.RACat, raValFrom, zoneIndex.ZoneId, j));
                                Assert.IsTrue(firstIndexEntry.RAJ2000 <= raValTo, string.Format("firstIndexEntry: {0} <= {1}; ZoneId={2}; jj={3}", firstIndexEntry.RACat, raValTo, zoneIndex.ZoneId, j));

                                rdr.BaseStream.Position = (raIdx - 1) * UCAC3Entry.Size;
                                UCAC3Entry lastIndexEntry = ReadEntry(rdr, buffer);

                                Assert.IsTrue(lastIndexEntry.RAJ2000 >= raValFrom, string.Format("lastIndexEntry: {0} >= {1}; ZoneId={2}; jj={3}", lastIndexEntry.RACat, raValFrom, zoneIndex.ZoneId, j));
                                Assert.IsTrue(lastIndexEntry.RAJ2000 <= raValTo, string.Format("lastIndexEntry: {0} <= {1}; ZoneId={2}; jj={3}", lastIndexEntry.RACat, raValTo, zoneIndex.ZoneId, j));
                            }

                            //Assert.AreEqual((index.RAIndexPerZone[i, 239] - (zoneIndex.LastStarNo - zoneIndex.TotalStarsInBin)) * UCAC2Entry.Size, rdr.BaseStream.Length);
                        }
                    }

                    Assert.IsNotNull(index.ZoneIndex[i]);
                }
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }

        [Test()]
        public void _4_ReadStars_1()
        {
            UCAC3Catalogue cat = new UCAC3Catalogue(CATALOG_LOCATION);
            List<IStar> satrs = cat.GetStarsInRegion(12.34, -51.32, 0.2, 15, 2000);

            Assert.IsNotNull(satrs);
            Assert.IsTrue(satrs.Count > 0);

            foreach (UCAC3Entry star in satrs)
            {
                Assert.IsTrue(star.RAJ2000 >= 12.34 - 0.25);
                Assert.IsTrue(star.RAJ2000 <= 12.34 + 0.25);
                Assert.IsTrue(star.DEJ2000 >= -51.32 - 0.25);
                Assert.IsTrue(star.DEJ2000 <= -51.32 + 0.25);
                Assert.IsTrue(star.Mag <= 15);
            }
        }


        [Test()]
        public void _4_TestIndexSearch_1()
        {
            double fov = 0.7877334195533845;

            UCAC3Catalogue cat = new UCAC3Catalogue(CATALOG_LOCATION);
            List<IStar> stars = cat.GetStarsInRegion(
                AstroConvert.ToRightAcsension("18:32:56"),
                AstroConvert.ToDeclination("+03:01:19"), fov, 15, 2000);

            Assert.IsNotNull(stars);
            Assert.IsTrue(stars.Count > 0);
			bool has184Stars = stars.Exists(s => s.StarNo >= 1840000000 && s.StarNo < 1850000000);
        	bool has185Stars = stars.Exists(s => s.StarNo >= 1850000000 && s.StarNo < 1860000000);
			bool has186Stars = stars.Exists(s => s.StarNo >= 1860000000 && s.StarNo < 1870000000);
			bool has187Stars = stars.Exists(s => s.StarNo >= 1870000000 && s.StarNo < 1880000000);
			bool has188Stars = stars.Exists(s => s.StarNo >= 1880000000 && s.StarNo < 1890000000);

        	Assert.IsFalse(has184Stars);
			Assert.IsFalse(has185Stars);
			Assert.IsTrue(has186Stars);
			Assert.IsTrue(has187Stars);
			Assert.IsFalse(has188Stars);

            List<IStar> stars2 = cat.GetStarsInRegion(
                AstroConvert.ToRightAcsension("18:33:10"),
                AstroConvert.ToDeclination("+03:00:05"), fov, 15, 2000);

            Assert.IsNotNull(stars2);
            Assert.IsTrue(stars2.Count > 0);

			has184Stars = stars2.Exists(s => s.StarNo >= 1840000000 && s.StarNo < 1850000000);
			has185Stars = stars2.Exists(s => s.StarNo >= 1850000000 && s.StarNo < 1860000000);
			has186Stars = stars2.Exists(s => s.StarNo >= 1860000000 && s.StarNo < 1870000000);
			has187Stars = stars2.Exists(s => s.StarNo >= 1870000000 && s.StarNo < 1880000000);
			has188Stars = stars2.Exists(s => s.StarNo >= 1880000000 && s.StarNo < 1890000000);

			Assert.IsFalse(has184Stars);
			Assert.IsFalse(has185Stars);
			Assert.IsTrue(has186Stars);
			Assert.IsTrue(has187Stars);
			Assert.IsFalse(has188Stars);

            foreach (UCAC3Entry star in stars)
            {
                Console.WriteLine(star.StarNo);
            }
            Console.WriteLine("-------------------------------------------------------------------");

            foreach (UCAC3Entry star in stars2)
            {
                Console.WriteLine(star.StarNo);
            }
        }

		[Test()]
		public void _4_TestIndexSearch_2()
		{
			double[] DECLINATIONS = new double[]
        	{
        		AstroConvert.ToDeclination("+03:01:19"),
				AstroConvert.ToDeclination("+12:00:01"),
				AstroConvert.ToDeclination("+12:59:59"),
				AstroConvert.ToDeclination("-03:01:19"),
				AstroConvert.ToDeclination("-12:00:01"),
				AstroConvert.ToDeclination("-12:59:59"),
				AstroConvert.ToDeclination("+00:05:00"),
				AstroConvert.ToDeclination("-00:05:00")
        	};
			
			foreach(double de in DECLINATIONS)
			{
				double ra = AstroConvert.ToRightAcsension("18:32:56");
				TestIndex(ra, de);
			}
		}

		private void TestIndex(double ra, double de)
		{
			double deFrom = (int)de;
			double deTo = (int)(de + 1);

			int fromZone = Math.Min(360, Math.Max(1, (int)(de * 2) + 180 + (de > 0 ? 1 : 0)));
			Console.WriteLine(string.Format("{0}: Zone{1} ({2} -- {3})", AstroConvert.ToStringValue(de, "DEC"), fromZone, AstroConvert.ToStringValue(deFrom, "DEC"), AstroConvert.ToStringValue(deTo, "DEC")));

			UCAC3Catalogue cat = new UCAC3Catalogue(CATALOG_LOCATION);

			List<IStar> stars = cat.GetStarsInRegion(ra, de, 1, 15, 2000);

			Assert.IsNotNull(stars);
			Assert.IsTrue(stars.Count > 0);

			List<IStar> zoneStars = stars.Where(s => s.StarNo >= (ulong)(fromZone * 10000000) && s.StarNo < (ulong)((fromZone + 1) * 10000000)).ToList();

			foreach (IStar star in zoneStars)
			{
				Assert.IsTrue(star.DEDeg >= deFrom && star.DEDeg < deTo);
			}			
        }
    }
	#endif
}
