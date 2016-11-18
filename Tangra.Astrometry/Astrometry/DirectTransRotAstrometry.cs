/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Tangra.Astrometry.Recognition;
using Tangra.Model.Astro;
using Tangra.Model.Helpers;
using Tangra.Model.Image;
using Tangra.StarCatalogues;

namespace Tangra.Astrometry
{
	[Serializable()]
	public class DirectTransRotAstrometry : IAstrometricFit, ISerializable
	{
		private double EtaRadians;
		private double m_RA0Deg;
		private double m_DE0Deg;
		private double m_EtaDeg;
		private double m_Aspect;

		private AstroPlate m_Image;

		public DirectTransRotAstrometry(AstroPlate image, double RA0Deg, double DE0Deg, double EtaDeg)
			: this(image, RA0Deg, DE0Deg, EtaDeg, 1.0)
		{ }

		public DirectTransRotAstrometry(AstroPlate image, double RA0Deg, double DE0Deg, double EtaDeg, double aspect)
		{
			m_Image = image;

			EtaRadians = EtaDeg * Math.PI / 180;
			this.m_RA0Deg = RA0Deg;
			this.m_DE0Deg = DE0Deg;
			this.m_EtaDeg = EtaDeg;
			this.m_Aspect = aspect;
		}

		public double RA0Deg
		{
			get { return m_RA0Deg; }
		}

		public double DE0Deg
		{
			get { return m_DE0Deg; }
		}

		public double EtaDeg
		{
			get { return m_EtaDeg; }
		}

		public double Aspect
		{
			get { return m_Aspect; }
		}

	    public double Residual { get; set; }

	    public AstroPlate Image
		{
			get { return m_Image; }
		}

		public string InfoString
		{
			get { return "Simple RA/DEC"; }
		}

		public void GetRADEFromImageCoords(double x, double y, out double RADeg, out double DE)
		{
			double dx = x - m_Image.CenterXImage;
			double dy = y - m_Image.CenterYImage;

			double dxmcr = dx * (m_Image.EffectivePixelWidth * 206265) * m_Aspect / (1000.0 * m_Image.EffectiveFocalLength * 3600);
			double dymcr = dy * (m_Image.EffectivePixelHeight * 206265) / (1000.0 * m_Image.EffectiveFocalLength * 3600);

			RADeg = (Math.Cos(EtaRadians) * dxmcr - Math.Sin(EtaRadians) * dymcr) + this.m_RA0Deg;
			DE = (Math.Sin(EtaRadians) * dxmcr + Math.Cos(EtaRadians) * dymcr) + this.m_DE0Deg;
		}

		public void GetImageCoordsFromRADE(double RADeg, double DE, out double x, out double y)
		{
			double dRA = RADeg - this.m_RA0Deg;
			double dDE = DE - this.m_DE0Deg;

			double dxmcr = Math.Cos(EtaRadians) * dRA + Math.Sin(EtaRadians) * dDE;
			double dymcr = Math.Cos(EtaRadians) * dDE - Math.Sin(EtaRadians) * dRA;

			x = m_Image.CenterXImage + (1000.0 * m_Image.EffectiveFocalLength) * dxmcr * 3600 / (206265 * m_Image.EffectivePixelWidth * m_Aspect);
			y = m_Image.CenterYImage + (1000.0 * m_Image.EffectiveFocalLength) * dymcr * 3600 / (206265 * m_Image.EffectivePixelHeight);
		}

		public double GetDistanceInArcSec(double x1, double y1, double x2, double y2)
		{
			double ra1, de1, ra2, de2;
			GetRADEFromImageCoords(x1, y1, out ra1, out de1);
			GetRADEFromImageCoords(x2, y2, out ra2, out de2);

			return AngleUtility.Elongation(ra1, de1, ra2, de2) * 3600.0;
		}

		public static DirectTransRotAstrometry SolveByThreeStars(
			AstroPlate image,
			Dictionary<ImagePixel, IStar> userStarIdentification,
            int tolerance)
		{
			double SingularityMinDiffPix = 2.0;

			List<KeyValuePair<ImagePixel, IStar>> master = userStarIdentification.ToList();

			List<KeyValuePair<ImagePixel, IStar>> list = new List<KeyValuePair<ImagePixel, IStar>>();
		    for (int i = 0; i < 3; i++)
		    {
		        list.Clear();
                if (i == 0)
                {
                    list.Add(master[0]);
                    list.Add(master[1]);
                    list.Add(master[2]);
                }
                else if (i == 1)
                {
                    list.Add(master[2]);
                    list.Add(master[0]);
                    list.Add(master[1]);
                }
                else if (i == 2)
                {
                    list.Add(master[1]);
                    list.Add(master[2]);
                    list.Add(master[0]);
                }

                double x1 = list[0].Key.XDouble;
                double y1 = list[0].Key.YDouble;
                double ra1 = list[0].Value.RADeg;
                double de1 = list[0].Value.DEDeg;

                double x2 = list[1].Key.XDouble;
                double y2 = list[1].Key.YDouble;
                double ra2 = list[1].Value.RADeg;
                double de2 = list[1].Value.DEDeg;

                double x3 = list[2].Key.XDouble;
                double y3 = list[2].Key.YDouble;
                double ra3 = list[2].Value.RADeg;
                double de3 = list[2].Value.DEDeg;

				#region Dealing with singularity issues
				if (Math.Abs(x1 - x2) < SingularityMinDiffPix)
				{
					if (x1 < x2)
						x2 = x1 + SingularityMinDiffPix;
					else
						x1 = x2 + SingularityMinDiffPix;
				}

				if (Math.Abs(x1 - x3) < SingularityMinDiffPix)
				{
					if (x1 < x3)
						x3 = x1 + SingularityMinDiffPix;
					else
						x1 = x3 + SingularityMinDiffPix;
				}

				if (Math.Abs(x2 - x3) < SingularityMinDiffPix)
				{
					if (x2 < x3)
						x3 = x2 + SingularityMinDiffPix;
					else
						x2 = x3 + SingularityMinDiffPix;
				}

				if (Math.Abs(y1 - y2) < SingularityMinDiffPix)
				{
					if (y1 < y2)
						y2 = y1 + SingularityMinDiffPix;
					else
						y1 = y2 + SingularityMinDiffPix;
				}
				if (Math.Abs(y1 - y3) < SingularityMinDiffPix)
				{
					if (y1 < y3)
						y3 = y1 + SingularityMinDiffPix;
					else
						y1 = y3 + SingularityMinDiffPix;
				}
				if (Math.Abs(y2 - y3) < SingularityMinDiffPix)
				{
					if (y2 < y3)
						y3 = y2 + SingularityMinDiffPix;
					else
						y2 = y3 + SingularityMinDiffPix;
				}
				#endregion

		    	double YY = 1000.0 * 3600 / (206265 * image.EffectivePixelHeight);
                double XX = 1000.0 * 3600 / (206265 * image.EffectivePixelWidth);

                double f_cose = ((y1 - y2) * (ra1 - ra3) - (y1 - y3) * (ra1 - ra2)) / (YY * ((de1 - de2) * (ra1 - ra3) - (de1 - de3) * (ra1 - ra2)));
                double f_sine = (de1 - de2) * f_cose / (ra1 - ra2) - (y1 - y2) / (YY * (ra1 - ra2));

                double eta1rad = Math.Atan(f_sine / f_cose);
                double eta2rad = Math.PI + eta1rad;

                double foc_len1 = Math.Abs(f_sine / Math.Sin(eta1rad));
                double foc_len2 = Math.Abs(f_sine / Math.Sin(eta2rad));

                double aspect1 = (foc_len1 * XX * Math.Cos(eta1rad) * (ra1 - ra2 + (de1 - de2) * Math.Tan(eta1rad))) / (x1 - x2);
                double DE01 = de1 - Math.Sin(eta1rad) * Math.Cos(eta1rad) * (aspect1 * (x1 - image.CenterXImage) / (foc_len1 * XX * Math.Cos(eta1rad)) + (y1 - image.CenterYImage) / (foc_len1 * YY * Math.Sin(eta1rad)));
                double RA01 = ra1 - ((de1 - DE01) * Math.Cos(eta1rad) - (y1 - image.CenterYImage) / (foc_len1 * YY)) / Math.Sin(eta1rad);

                double aspect2 = (foc_len2 * XX * Math.Cos(eta2rad) * (ra1 - ra2 + (de1 - de2) * Math.Tan(eta2rad))) / (x1 - x2);
                double DE02 = de1 - Math.Sin(eta2rad) * Math.Cos(eta2rad) * (aspect2 * (x1 - image.CenterXImage) / (foc_len2 * XX * Math.Cos(eta2rad)) + (y1 - image.CenterYImage) / (foc_len2 * YY * Math.Sin(eta2rad)));
                double RA02 = ra1 - ((de1 - DE02) * Math.Cos(eta2rad) - (y1 - image.CenterYImage) / (foc_len2 * YY)) / Math.Sin(eta2rad);

                AstroPlate plate1 = image.Clone();
                plate1.EffectiveFocalLength = foc_len1;
                DirectTransRotAstrometry solution1 = new DirectTransRotAstrometry(plate1, RA01, DE01, eta1rad * 180.0 / Math.PI, aspect1);

                AstroPlate plate2 = image.Clone();
                plate2.EffectiveFocalLength = foc_len2;
                DirectTransRotAstrometry solution2 = new DirectTransRotAstrometry(plate2, RA02, DE02, eta2rad * 180.0 / Math.PI, aspect2);

                double xx1, yy1, xx2, yy2, xx3, yy3;
                solution1.GetImageCoordsFromRADE(ra1, de1, out xx1, out yy1);
                solution1.GetImageCoordsFromRADE(ra2, de2, out xx2, out yy2);
                solution1.GetImageCoordsFromRADE(ra3, de3, out xx3, out yy3);
                solution1.Residual = Math.Sqrt(
                    (x1 - xx1) * (x1 - xx1) + (y1 - yy1) * (y1 - yy1) +
                    (x2 - xx2) * (x2 - xx2) + (y2 - yy2) * (y2 - yy2) +
                    (x3 - xx3) * (x3 - xx3) + (y3 - yy3) * (y3 - yy3));

                solution2.GetImageCoordsFromRADE(ra1, de1, out xx1, out yy1);
                solution2.GetImageCoordsFromRADE(ra2, de2, out xx2, out yy2);
                solution2.GetImageCoordsFromRADE(ra3, de3, out xx3, out yy3);
                solution2.Residual = Math.Sqrt(
                    (x1 - xx1) * (x1 - xx1) + (y1 - yy1) * (y1 - yy1) +
                    (x2 - xx2) * (x2 - xx2) + (y2 - yy2) * (y2 - yy2) +
                    (x3 - xx3) * (x3 - xx3) + (y3 - yy3) * (y3 - yy3));

		        double maxResidual = CorePyramidConfig.Default.MaxThreeIdentifiedStarsFitResidual;
		        if (tolerance == 1) maxResidual *= 0.75;
                else if (tolerance == 3) maxResidual *= 2;
                else if (tolerance == 4) maxResidual *= 3;

                if (solution1.Residual < solution2.Residual)
                {
                    if (solution1.Residual < maxResidual && aspect1 > 0)
                        return solution1;
                }
                else
                {
                    if (solution2.Residual < maxResidual && aspect2 > 0)
                        return solution2;
                }
		    }

			return null;
		}

		public static DirectTransRotAstrometry SolveByThreeStars(
			AstroPlate image,
			Dictionary<PSFFit, IStar> userStarIdentification,
            int tolerance)
		{
			Dictionary<ImagePixel, IStar> transformedDict =
				userStarIdentification.ToDictionary(
					kvp => new ImagePixel(255, kvp.Key.XCenter, kvp.Key.YCenter),
					kvp => kvp.Value);

            return SolveByThreeStars(image, transformedDict, tolerance);
		}

		#region ISerializable Members

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("EtaRadians", EtaRadians);
			info.AddValue("m_RA0Deg", m_RA0Deg);
			info.AddValue("m_DE0Deg", m_DE0Deg);
			info.AddValue("m_EtaDeg", m_EtaDeg);
			info.AddValue("m_Aspect", m_Aspect);

			BinaryFormatter fmt = new BinaryFormatter();
			using (MemoryStream mem = new MemoryStream())
			{
				fmt.Serialize(mem, m_Image);
				mem.Seek(0, SeekOrigin.Begin);

				info.AddValue("m_Image", mem.ToArray());
			}
		}

		private DirectTransRotAstrometry()
		{ }

		public DirectTransRotAstrometry(SerializationInfo info, StreamingContext context)
		{
			EtaRadians = info.GetDouble("EtaRadians");
			m_RA0Deg = info.GetDouble("m_RA0Deg");
			m_DE0Deg = info.GetDouble("m_DE0Deg");
			m_EtaDeg = info.GetDouble("m_EtaDeg");
			m_Aspect = info.GetDouble("m_Aspect");

			byte[] data = (byte[])info.GetValue("m_Image", typeof(byte[]));

			BinaryFormatter fmt = new BinaryFormatter();
			using (MemoryStream mem = new MemoryStream(data))
			{
				m_Image = (AstroPlate)fmt.Deserialize(mem);
			}
		}

		public static DirectTransRotAstrometry FromReflectedObject(object reflObj)
		{
			var rv = new DirectTransRotAstrometry();

			rv.m_RA0Deg = StarMap.GetPropValue<double>(reflObj, "m_RA0Deg");
			rv.m_DE0Deg = StarMap.GetPropValue<double>(reflObj, "m_DE0Deg");
			rv.m_EtaDeg = StarMap.GetPropValue<double>(reflObj, "m_EtaDeg");
			rv.m_Aspect = StarMap.GetPropValue<double>(reflObj, "m_Aspect");

			object image = StarMap.GetPropValue<object>(reflObj, "m_Image");
			rv.m_Image = AstroPlate.FromReflectedObject(image);

			return rv;
		}

		#endregion
	}
}
