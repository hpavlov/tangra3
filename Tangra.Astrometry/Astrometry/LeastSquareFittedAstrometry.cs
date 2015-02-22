/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Tangra.Astrometry.Recognition;
using Tangra.Model.Astro;
using Tangra.Model.Helpers;

namespace Tangra.Astrometry
{
	[Serializable]
	public class LeastSquareFittedAstrometry : IAstrometricFit, ISerializable
	{
		public double m_RA0Deg;
		public double m_DE0Deg;

		public double m_Variance;
		public double m_StdDevRAArcSec;
		public double m_StdDevDEArcSec;

		private AstroPlate m_Image;
		private PlateConstantsFit m_SolvedConstants;

		public LeastSquareFittedAstrometry(
			AstroPlate image,
			double RA0Deg, double DE0Deg,
			PlateConstantsFit solution)
		{
			m_Image = image;

			this.m_RA0Deg = RA0Deg;
			this.m_DE0Deg = DE0Deg;

			m_SolvedConstants = solution;
			m_Variance = solution.Variance;
			m_StdDevRAArcSec = Math.Sqrt(solution.VarianceArcSecRA);
			m_StdDevDEArcSec = Math.Sqrt(solution.VarianceArcSecDE);

		}

		public FitOrder FitOrder
		{
			get { return m_SolvedConstants.FitOrder; }
		}

		internal PlateConstantsFit SolvedConstants
		{
			get { return m_SolvedConstants; }
		}

		public double RA0Deg
		{
			get { return m_RA0Deg; }
		}

		public double DE0Deg
		{
			get { return m_DE0Deg; }
		}

		public double Variance
		{
			get { return m_Variance; }
		}

		public double StdDevRAArcSec
		{
			get { return m_StdDevRAArcSec; }
		}

		public double StdDevDEArcSec
		{
			get { return m_StdDevDEArcSec; }
		}

		public AstroPlate Image
		{
			get { return m_Image; }
		}

		public FitInfo FitInfo
		{
			get { return m_SolvedConstants.FitInfo; }
		}

		public void GetRADEFromImageCoords(double x, double y, out double RADeg, out double DE)
		{
			double xTang, yTang;
			m_SolvedConstants.GetTangentCoordsFromImageCoords(x, y, out xTang, out yTang);

			TangentPlane.TangentToCelestial(xTang, yTang, m_RA0Deg, m_DE0Deg, out RADeg, out DE);
		}

		public void GetImageCoordsFromRADE(double RADeg, double DE, out double x, out double y)
		{
			double xTang, yTang;
			TangentPlane.CelestialToTangent(RADeg, DE, m_RA0Deg, m_DE0Deg, out xTang, out yTang);


			m_SolvedConstants.GetImageCoordsFromTangentCoords(xTang, yTang, out x, out y);
		}

		public double GetDistanceInArcSec(double x1, double y1, double x2, double y2)
		{
			double ra1, de1, ra2, de2;
			GetRADEFromImageCoords(x1, y1, out ra1, out de1);
			GetRADEFromImageCoords(x2, y2, out ra2, out de2);

			return AngleUtility.Elongation(ra1, de1, ra2, de2) * 3600.0;
		}

		public double GetDistanceInPixels(double distArcSec)
		{
			double x1, y1, x2, y2;
			GetImageCoordsFromRADE(m_RA0Deg, m_DE0Deg, out x1, out y1);
			GetRADEFromImageCoords(m_RA0Deg, m_DE0Deg + distArcSec, out x2, out y2);

			return Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
		}

		public double GetDistanceInPixels(double x1, double y1, double x2, double y2)
		{
			return Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
		}

		public string InfoString
		{
			get { return m_SolvedConstants.FitOrder.ToString() + " Fit"; }
		}

		#region ISerializable Members

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("m_RA0Deg", m_RA0Deg);
			info.AddValue("m_DE0Deg", m_DE0Deg);

			BinaryFormatter fmt = new BinaryFormatter();
			using (MemoryStream mem = new MemoryStream())
			{
				fmt.Serialize(mem, m_Image);
				mem.Seek(0, SeekOrigin.Begin);

				info.AddValue("m_Image", mem.ToArray());
			}

			using (MemoryStream mem = new MemoryStream())
			{
				fmt.Serialize(mem, m_SolvedConstants);
				mem.Seek(0, SeekOrigin.Begin);

				info.AddValue("m_SolvedConstants", mem.ToArray());
			}
		}

		public LeastSquareFittedAstrometry(SerializationInfo info, StreamingContext context)
		{
			m_RA0Deg = info.GetDouble("m_RA0Deg");
			m_DE0Deg = info.GetDouble("m_DE0Deg");

			byte[] data = (byte[])info.GetValue("m_Image", typeof(byte[]));

			BinaryFormatter fmt = new BinaryFormatter();
			using (MemoryStream mem = new MemoryStream(data))
			{
				m_Image = (AstroPlate)fmt.Deserialize(mem);
			}

			data = (byte[])info.GetValue("m_SolvedConstants", typeof(byte[]));
			using (MemoryStream mem = new MemoryStream(data))
			{
				m_SolvedConstants = (PlateConstantsFit)fmt.Deserialize(mem);
			}
		}

		private LeastSquareFittedAstrometry()
		{ }

		public static LeastSquareFittedAstrometry FromReflectedObject(object reflObj)
		{
			var rv = new LeastSquareFittedAstrometry();

			rv.m_RA0Deg = StarMap.GetPropValue<double>(reflObj, "m_RA0Deg");
			rv.m_DE0Deg = StarMap.GetPropValue<double>(reflObj, "m_DE0Deg");
			rv.m_Variance = StarMap.GetPropValue<double>(reflObj, "m_Variance");
			rv.m_StdDevRAArcSec = StarMap.GetPropValue<double>(reflObj, "m_StdDevRAArcSec");
			rv.m_StdDevDEArcSec = StarMap.GetPropValue<double>(reflObj, "m_StdDevDEArcSec");

			object image = StarMap.GetPropValue<object>(reflObj, "m_Image");
			rv.m_Image = AstroPlate.FromReflectedObject(image);

			object constants = StarMap.GetPropValue<object>(reflObj, "m_SolvedConstants");

			if (constants != null)
				rv.m_SolvedConstants = PlateConstantsFit.FromReflectedObject(constants);

			return rv;
		}

		#endregion
	}

}
