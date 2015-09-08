/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tangra.Model.Astro;
using Tangra.Model.Helpers;

namespace Tangra.Astrometry
{
	public class TangentalTransRotAstrometry : IAstrometricFit
	{
		public readonly double EtaRadians;
		public readonly double m_RA0Deg;
		public readonly double m_DE0Deg;

		private double cellWidth;
		private double cellHeight;
		private double focalLength;

		private AstroPlate m_Image;
		private string m_InfoString;

		public TangentalTransRotAstrometry(double cellWidth, double cellHeight, double focalLength, double RA0Deg, double DE0Deg, double EtaDeg)
		{
			EtaRadians = EtaDeg * Math.PI / 180;
			this.m_RA0Deg = RA0Deg;
			this.m_DE0Deg = DE0Deg;

			this.cellWidth = cellWidth;
			this.cellHeight = cellHeight;
			this.focalLength = focalLength;
			m_InfoString = string.Format("Tangental [{0}; {1}; {2}]", cellWidth.ToString("0.00"), cellHeight.ToString("0.00"), focalLength.ToString("0.0"));
		}

		public TangentalTransRotAstrometry(AstroPlate image, double RA0Deg, double DE0Deg, double EtaDeg)
		{
			m_Image = image;

			EtaRadians = EtaDeg * Math.PI / 180;
			this.m_RA0Deg = RA0Deg;
			this.m_DE0Deg = DE0Deg;

			cellWidth = m_Image.EffectivePixelWidth;
			cellHeight = m_Image.EffectivePixelHeight;
			focalLength = m_Image.EffectiveFocalLength;
			m_InfoString = string.Format("Tangental [{0}; {1}; {2}]", cellWidth.ToString("0.00"), cellHeight.ToString("0.00"), focalLength.ToString("0.0"));
		}

		public TangentalTransRotAstrometry(TangentalTransRotAstrometry prev, AstroPlate image, double RA0Deg, double DE0Deg, double EtaDeg)
		{
			m_Image = prev.m_Image;

			EtaRadians = EtaDeg * Math.PI / 180;
			this.m_RA0Deg = RA0Deg;
			this.m_DE0Deg = DE0Deg;

			cellWidth = prev.cellWidth;
			cellHeight = prev.cellHeight;
			focalLength = image.EffectiveFocalLength; // prev.focalLength;
			m_InfoString = string.Format("Tangental [{0}; {1}; {2}]", cellWidth.ToString("0.00"), cellHeight.ToString("0.00"), focalLength.ToString("0.0"));
		}

		public double RA0Deg
		{
			get { return m_RA0Deg; }
		}

		public double DE0Deg
		{
			get { return m_DE0Deg; }
		}

		public AstroPlate Image
		{
			get { return m_Image; }
		}

		public string InfoString
		{
			get { return m_InfoString; }
		}

		public void GetRADEFromImageCoords(double x, double y, out double RADeg, out double DE)
		{
			double mtxX, mtxY;
			m_Image.ImageCoordsToMatrixCoords(x - m_Image.CenterXImage, y - m_Image.CenterYImage, out mtxX, out mtxY, CoordinateReference.Center);

			double xt = Math.Cos(EtaRadians) * mtxX - Math.Sin(EtaRadians) * mtxY;
			double yt = Math.Sin(EtaRadians) * mtxX + Math.Cos(EtaRadians) * mtxY;

			double tangentalX = xt * cellWidth / (focalLength * 1000.0);
			double tangentalY = yt * cellHeight / (focalLength * 1000.0);

			tangentalY = -tangentalY;

			TangentPlane.TangentToCelestial(tangentalX, tangentalY, m_RA0Deg, this.m_DE0Deg, out RADeg, out DE);
		}

		public void GetImageCoordsFromRADE(double RADeg, double DE, out double x, out double y)
		{
			double tangentalX, tangentalY;
			TangentPlane.CelestialToTangent(RADeg, DE, this.m_RA0Deg, this.m_DE0Deg, out tangentalX, out tangentalY);

			tangentalY = -tangentalY;

			double plateX = tangentalX * (focalLength * 1000.0 / cellWidth);
			double plateY = tangentalY * (focalLength * 1000.0 / cellHeight);

			double mtxX = Math.Cos(EtaRadians) * plateX + Math.Sin(EtaRadians) * plateY;
			double mtxY = Math.Cos(EtaRadians) * plateY - Math.Sin(EtaRadians) * plateX;


			m_Image.MatrixCoordsToImageCoords(mtxX, mtxY, out x, out y, CoordinateReference.Center);

			x += m_Image.CenterXImage;
			y += m_Image.CenterYImage;
		}


		public double GetDistanceInArcSec(double x1, double y1, double x2, double y2)
		{
			double ra1, de1, ra2, de2;
			GetRADEFromImageCoords(x1, y1, out ra1, out de1);
			GetRADEFromImageCoords(x2, y2, out ra2, out de2);

			return AngleUtility.Elongation(ra1, de1, ra2, de2) * 3600.0;
		}
	}
}
