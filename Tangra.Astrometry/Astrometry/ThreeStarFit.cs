/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tangra.Model.Astro;
using Tangra.StarCatalogues;

namespace Tangra.Astrometry
{
	public class ThreeStarFit : IAstrometricFit
	{
		private AstroPlate m_Image;
		private double m_RA0Deg;
		private double m_DE0Deg;

		private double m_A;
		private double m_B;
		private double m_C;
		private double m_D;
		private double m_E;
		private double m_F;

		private double m_A1;
		private double m_B1;
		private double m_C1;
		private double m_D1;
		private double m_E1;
		private double m_F1;

		private bool m_IsSolved = false;
		private bool m_IsSingularity = false;

		public ThreeStarFit(
			AstroPlate image,
			StarPair pair1,
			StarPair pair2,
			StarPair pair3)
		{
			m_Image = image;

			// X1 = a*x1 + b*y1 + c
			// Y1 = d*x1 + e*y1 + f
			// X2 = a*x2 + b*y2 + c
			// Y2 = d*x2 + e*y2 + f
			// X3 = a*x3 + b*y3 + c
			// Y3 = d*x3 + e*y3 + f

			// NOTE: First do a SimpleRaDec fit to get the RA0/DE0

			double DX12 = pair1.RADeg - pair2.RADeg;
			double DX23 = pair2.RADeg - pair3.RADeg;
			double DY12 = pair1.DEDeg - pair2.DEDeg;
			double DY23 = pair2.DEDeg - pair3.DEDeg;
			double dx12 = pair1.XImage - pair2.XImage;
			double dx23 = pair2.XImage - pair3.XImage;
			double dy12 = pair1.YImage - pair2.YImage;
			double dy23 = pair2.YImage - pair3.YImage;

			// Singularity
			if (DX12 * DX23 * DY12 * DY23 * dx12 * dx23 * dy12 * dy23 == 0)
			{
				m_IsSingularity = true;
				return;
			}

			m_A = (DX12 * dy23 - DX23 * dy12) / (dx12 * dy23 - dx23 * dy12);
			m_B = (DX12 - m_A * dx12) / dy12;
			m_C = pair1.RADeg - m_A * pair1.XImage - m_B * pair1.YImage;
			m_D = (DY12 * dy23 - DY23 * dy12) / (dx12 * dy23 - dx23 * dy12);
			m_E = (DY12 - m_D * dx12) / dy12;
			m_F = pair1.DEDeg - m_D * pair1.XImage - m_E * pair1.YImage;

			m_RA0Deg = m_A * m_Image.CenterXImage + m_B * m_Image.CenterYImage + m_C;
			m_DE0Deg = m_D * m_Image.CenterXImage + m_E * m_Image.CenterYImage + m_F;

			// NOTE: Then do the Tangental solution

			TangentPlane.CelestialToTangent(pair1.RADeg, pair1.DEDeg, m_RA0Deg, m_DE0Deg, out pair1.XTangent, out pair1.YTangent);
			TangentPlane.CelestialToTangent(pair2.RADeg, pair2.DEDeg, m_RA0Deg, m_DE0Deg, out pair2.XTangent, out pair2.YTangent);
			TangentPlane.CelestialToTangent(pair3.RADeg, pair3.DEDeg, m_RA0Deg, m_DE0Deg, out pair3.XTangent, out pair3.YTangent);

			DX12 = pair1.XTangent - pair2.XTangent;
			DX23 = pair2.XTangent - pair3.XTangent;
			DY12 = pair1.YTangent - pair2.YTangent;
			DY23 = pair2.YTangent - pair3.YTangent;

			m_A = (DX12 * dy23 - DX23 * dy12) / (dx12 * dy23 - dx23 * dy12);
			m_B = (DX12 - m_A * dx12) / dy12;
			m_C = pair1.XTangent - m_A * pair1.XImage - m_B * pair1.YImage;
			m_D = (DY12 * dy23 - DY23 * dy12) / (dx12 * dy23 - dx23 * dy12);
			m_E = (DY12 - m_D * dx12) / dy12;
			m_F = pair1.YTangent - m_D * pair1.XImage - m_E * pair1.YImage;

			m_A1 = m_E / (m_E * m_A - m_B * m_D);
			m_B1 = -m_B / (m_E * m_A - m_B * m_D);
			m_C1 = (m_B * m_F - m_C * m_E) / (m_E * m_A - m_B * m_D);
			m_D1 = m_D / (m_B * m_D - m_A * m_E);
			m_E1 = -m_A / (m_B * m_D - m_A * m_E);
			m_F1 = (m_A * m_F - m_C * m_D) / (m_B * m_D - m_A * m_E);

			m_IsSolved = true;
		}

		public bool IsSolved
		{
			get { return m_IsSolved; }
		}

		public bool IsSingularity
		{
			get { return m_IsSingularity; }
		}

		#region IAstrometricFit Members

		public void GetRADEFromImageCoords(double x, double y, out double RADeg, out double DEDeg)
		{
			double xTang = m_A * x + m_B * y + m_C;
			double yTang = m_D * x + m_E * y + m_F;

			TangentPlane.TangentToCelestial(xTang, yTang, m_RA0Deg, m_DE0Deg, out RADeg, out DEDeg);
		}

		public void GetImageCoordsFromRADE(double RADeg, double DEDeg, out double x, out double y)
		{
			double xTang, yTang;
			TangentPlane.CelestialToTangent(RADeg, DEDeg, m_RA0Deg, m_DE0Deg, out xTang, out yTang);

			x = m_A1 * xTang + m_B1 * yTang + m_C1;
			y = m_D1 * xTang + m_E1 * yTang + m_F1;
		}

		public double GetDistanceInArcSec(double x1, double y1, double x2, double y2)
		{
			throw new NotImplementedException();
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

		#endregion

		public class StarPair
		{
			public StarPair(double x, double y)
			{
				XImage = x;
				YImage = y;
			}

			public double RADeg;
			public double DEDeg;
			public IStar Star;
			public readonly double XImage;
			public readonly double YImage;
			internal double XTangent;
			internal double YTangent;
		}
	}
}
