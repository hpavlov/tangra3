/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml.Serialization;
using Tangra.Astrometry.Recognition;
using Tangra.AstroServices;
using Tangra.Model.Astro;
using Tangra.Model.Config;
using Tangra.Photometry;
using Tangra.StarCatalogues;
using Tangra.VideoOperations.LightCurves;

namespace Tangra.VideoOperations.Astrometry.Engine
{
	public enum RecognitionMethod
	{
		LostInSpace,
		KnownCenter
	}

	[Serializable()]
	public class FieldSolveContext : ISerializable
	{
		public double RADeg;
		public double DEDeg;
		public double ErrFoVs;
		public RecognitionMethod Method;
		public string DataBaseServer;
		public string DataBaseName;

		public double CellX;
		public double CellY;
		public double FocalLength;
		public double LimitMagn;

		public double PyramidMinMag;
        public double PyramidMaxMag;

		public double Epoch;

		public DateTime UtcTime;
		public int FrameNoOfUtcTime;

		public string ObsCode;

		public bool DetermineAutoLimitMagnitude;

        public double AutoLimitMagnitude;

		[XmlIgnore]
		internal List<IStar> CatalogueStars = new List<IStar>();

		[XmlIgnore]
		public MPEph2.MPEphEntry FoundObject;

		public TangraConfig.PreProcessingFilter UseFilter = TangraConfig.PreProcessingFilter.NoFilter;

		[XmlIgnore]
        public StarCatalogueFacade StarCatalogueFacade;

		public FieldSolveContext()
		{ }

		public override string ToString()
		{
			return string.Format(
                "RA={0}; DE={1}; ErrFoVs={2}; PyramidMinMag={3}; PyramidMaxMag={4}; LimitMagn={5}; CellX={6}; CellY={7}; FocalLength={8};",
                RADeg, DEDeg, ErrFoVs, PyramidMinMag, PyramidMaxMag, LimitMagn, CellX, CellY, FocalLength);
		}

		#region ISerializable Members

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("RADeg", RADeg);
            info.AddValue("DEDeg", DEDeg);
            info.AddValue("ErrFoVs", ErrFoVs);
            info.AddValue("Method", (int)Method);
            info.AddValue("DataBaseServer", DataBaseServer);
            info.AddValue("DataBaseName", DataBaseName);
            info.AddValue("CellX", CellX);
			info.AddValue("CellY", CellY);
			info.AddValue("FocalLength", FocalLength);
			info.AddValue("LimitMagn", LimitMagn);
            info.AddValue("MinMag", PyramidMinMag);
            info.AddValue("MaxMag", PyramidMaxMag);
			info.AddValue("UtcTime", UtcTime);
			info.AddValue("FrameNoOfUtcTime", FrameNoOfUtcTime);

			info.AddValue("ObsCode", ObsCode);
			info.AddValue("UseFilter", UseFilter);

			BinaryFormatter fmt = new BinaryFormatter();
			using(MemoryStream mem = new MemoryStream())
			{
				fmt.Serialize(mem, CatalogueStars.Count);

				foreach(IStar star in CatalogueStars)
				{
					Star copiedStar = new Star(star);
					fmt.Serialize(mem, copiedStar);	
				}
			    
			    info.AddValue("CatalogueStars", mem.ToArray());
			}

        }


		public FieldSolveContext(SerializationInfo info, StreamingContext context)
        {
			RADeg = info.GetDouble("RADeg");
			DEDeg = info.GetDouble("DEDeg");
			ErrFoVs = info.GetDouble("ErrFoVs");
			Method = (RecognitionMethod)info.GetInt32("Method");
			DataBaseServer = info.GetString("DataBaseServer");
			DataBaseName = info.GetString("DataBaseName");
			CellX = info.GetDouble("CellX");
			CellY = info.GetDouble("CellY");
			FocalLength = info.GetDouble("FocalLength");
			LimitMagn = info.GetDouble("LimitMagn");
            PyramidMinMag = info.GetDouble("MinMag");
            PyramidMaxMag = info.GetDouble("MaxMag");

			UtcTime = info.GetDateTime("UtcTime");
			FrameNoOfUtcTime = info.GetInt32("FrameNoOfUtcTime");

			FrameNoOfUtcTime = info.GetInt32("FrameNoOfUtcTime");
			ObsCode = info.GetString("ObsCode");
			
            byte[] data = (byte[])info.GetValue("CatalogueStars", typeof(byte[]));

            BinaryFormatter fmt = new BinaryFormatter();
            using (MemoryStream mem = new MemoryStream(data))
            {
            	CatalogueStars = new List<IStar>();

				int count = (int)fmt.Deserialize(mem);
            	for (int i = 0; i < count; i++)
            	{
            		IStar star = (Star)fmt.Deserialize(mem);
            		CatalogueStars.Add(star);
            	}
            }

			try
			{
				
			}
			catch(InvalidCastException)
			{ }
        }

		public static FieldSolveContext FromReflectedObject(object reflObj)
		{
			var rv = new FieldSolveContext();

			rv.RADeg = StarMap.GetPropValue<double>(reflObj, "RADeg");
			rv.DEDeg = StarMap.GetPropValue<double>(reflObj, "DEDeg");
			rv.ErrFoVs = StarMap.GetPropValue<double>(reflObj, "ErrFoVs");

			rv.Method = (RecognitionMethod)StarMap.GetPropValue<int>(reflObj, "Method");

			rv.DataBaseServer = StarMap.GetPropValue<string>(reflObj, "DataBaseServer");
			rv.DataBaseName = StarMap.GetPropValue<string>(reflObj, "DataBaseName");

			rv.CellX = StarMap.GetPropValue<double>(reflObj, "CellX");
			rv.CellY = StarMap.GetPropValue<double>(reflObj, "CellY");
			rv.FocalLength = StarMap.GetPropValue<double>(reflObj, "FocalLength");
			rv.LimitMagn = StarMap.GetPropValue<double>(reflObj, "LimitMagn");
 			rv.PyramidMinMag = StarMap.GetPropValue<double>(reflObj, "PyramidMinMag");
			rv.PyramidMaxMag = StarMap.GetPropValue<double>(reflObj, "PyramidMaxMag");

			rv.UtcTime = StarMap.GetPropValue<DateTime>(reflObj, "UtcTime");

			rv.FrameNoOfUtcTime = StarMap.GetPropValue<int>(reflObj, "FrameNoOfUtcTime");
			rv.ObsCode = StarMap.GetPropValue<string>(reflObj, "ObsCode");
			rv.UseFilter = (TangraConfig.PreProcessingFilter)StarMap.GetPropValue<int>(reflObj, "UseFilter");

			object stars = StarMap.GetPropValue<object>(reflObj, "CatalogueStars");

			rv.CatalogueStars = CreateStars(stars);
			
			return rv;
		}

		private static List<IStar> CreateStars(object reflObj)
		{
			var rv = new List<IStar>();

			PropertyInfo piItem = reflObj.GetType().GetProperty("Item");
			PropertyInfo piCount = reflObj.GetType().GetProperty("Count");

			int count = (int)piCount.GetValue(reflObj, null);
			if (count > 0)
			{
				object istar = piItem.GetValue(reflObj, new object[] { 0 });

				PropertyInfo piStarNo = istar.GetType().GetProperty("StarNo");
				PropertyInfo piRADeg = istar.GetType().GetProperty("RADeg");
				PropertyInfo piDEDeg = istar.GetType().GetProperty("DEDeg");
				PropertyInfo piMag = istar.GetType().GetProperty("Mag");

				for (int i = 0; i < count; i++)
				{
					istar = piItem.GetValue(reflObj, new object[] { i });

                    ulong id = (ulong)piStarNo.GetValue(istar, null);
					double ra = (double)piRADeg.GetValue(istar, null);
					double de = (double)piDEDeg.GetValue(istar, null);
					double m = (double)piMag.GetValue(istar, null);

					rv.Add(new Star(id, ra, de, m));
				}
			}

			return rv;
		}
		#endregion
	}
}
