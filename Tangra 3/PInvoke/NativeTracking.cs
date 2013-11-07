using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Tangra.Model.Astro;
using Tangra.Model.Config;
using Tangra.VideoOperations.LightCurves.Tracking;

namespace Tangra.PInvoke
{
	[StructLayout(LayoutKind.Explicit)]
	internal class NativePsfFitInfo
	{
		[FieldOffset(0)]
		public float XCenter;
		[FieldOffset(4)]
		public float YCenter;
		[FieldOffset(8)]
		public float FWHM;
		[FieldOffset(12)]
		public float IMax;
		[FieldOffset(16)]
		public float I0;
		[FieldOffset(20)]
		public float X0;
		[FieldOffset(24)]
		public float Y0;
		[FieldOffset(28)]
		public byte MatrixSize;
		[FieldOffset(29)]
		public byte IsSolved;

		[FieldOffset(30)] 
		public byte IsAsymmetric;
		[FieldOffset(31)]
		public byte Reserved;
		[FieldOffset(32)]
		public float R0;
		[FieldOffset(36)]
		public float R02;
	}

	internal class NativeTrackedObjectPsfFit : ITrackedObjectPsfFit
	{
		private float m_R0;
		private float m_R02;
		private float m_IBackground;
		private float m_IStarMax;
		private bool m_IsAsymmetric;
		private uint m_Saturation;

		private double[] m_Residuals;

		public NativeTrackedObjectPsfFit(NativePsfFitInfo psfInfo, double[] residuals, int bpp)
		{
			XCenter = psfInfo.XCenter;
			YCenter = psfInfo.YCenter;
			FWHM = psfInfo.FWHM;
			IMax = psfInfo.IMax;
			I0 = psfInfo.I0;
			X0 = psfInfo.X0;
			Y0 = psfInfo.Y0;
			MatrixSize = psfInfo.MatrixSize;
			IsSolved = psfInfo.IsSolved == 1;

			m_R0 = psfInfo.R0;
			m_R02 = psfInfo.R02;
			m_IsAsymmetric = psfInfo.IsAsymmetric == 1;

			m_IBackground = psfInfo.I0;
			m_IStarMax = psfInfo.IMax - psfInfo.I0;

			m_Residuals = residuals;

			switch (bpp)
			{
				case 8:
					m_Saturation = TangraConfig.Settings.Photometry.Saturation.Saturation8Bit;
					break;

				case 12:
					m_Saturation = TangraConfig.Settings.Photometry.Saturation.Saturation12Bit;
					break;

				case 14:
					m_Saturation = TangraConfig.Settings.Photometry.Saturation.Saturation14Bit;
					break;

				default:
					m_Saturation = TangraConfig.Settings.Photometry.Saturation.Saturation8Bit;
					break;
			}
		}

		public double XCenter { get; private set; }

		public double YCenter { get; private set; }

		public double FWHM { get; private set; }

		public double IMax { get; private set; }

		public double I0 { get; private set; }

		public double X0 { get; private set; }

		public double Y0 { get; private set; }

		public int MatrixSize { get; private set; }

		public bool IsSolved { get; private set; }

		private double GetPSFValueInternal(double x, double y)
		{
			return m_IBackground + m_IStarMax * Math.Exp(-((x - X0) * (x - X0) + (y - Y0) * (y - Y0)) / (m_R0 * m_R0));
		}

		private double GetPSFValueInternalAsymetric(double x, double y)
		{
			return m_IBackground + m_IStarMax * Math.Exp(-(x - X0) * (x - X0) / (m_R0 * m_R0) + (y - Y0) * (y - Y0) / (m_R02 * m_R02));
		}

		public double GetPSFValueAt(double x, double y)
		{
			return m_IsAsymmetric 
				? GetPSFValueInternalAsymetric(x, y) 
				: GetPSFValueInternal(x, y);
		}

		public double GetResidualAt(int x, int y)
		{
			return m_Residuals[y * MatrixSize + x];
		}

		public void DrawDataPixels(System.Drawing.Graphics g, System.Drawing.Rectangle rect, float aperture, System.Drawing.Pen aperturePen, int bpp)
		{
			PSFFit.DrawDataPixels(g, rect, aperture, aperturePen, bpp, this);
		}

		public void DrawGraph(System.Drawing.Graphics g, System.Drawing.Rectangle rect, int bpp)
		{
			PSFFit.DrawGraph(g, rect, bpp, this, m_Saturation);
		}
	}

	internal static class NativeTracking
	{
		private const string LIBRARY_TANGRA_CORE = "TangraCore";

		[DllImport(LIBRARY_TANGRA_CORE, CallingConvention = CallingConvention.Cdecl)]
		// DLL_PUBLIC long TrackerSettings(double maxElongation, double minFWHM, double maxFWHM, double minCertainty);
		internal static extern int TrackerSettings(double maxElongation, double minFWHM, double maxFWHM, double minCertainty);

		[DllImport(LIBRARY_TANGRA_CORE, CallingConvention = CallingConvention.Cdecl)]
		// DLL_PUBLIC long TrackerNewConfiguration(long width, long height, long numTrackedObjects, bool isFullDisappearance);
		internal static extern int TrackerNewConfiguration(int width, int height, int numTrackedObjects, bool isFullDisappearance);

		[DllImport(LIBRARY_TANGRA_CORE, CallingConvention = CallingConvention.Cdecl)]
		// DLL_PUBLIC long TrackerNextFrame(long frameId, unsigned long* pixels);
		private static extern int TrackerNextFrame(int frameId, [In, Out] uint[] pixels);

		[DllImport(LIBRARY_TANGRA_CORE, CallingConvention = CallingConvention.Cdecl)]
		// DLL_PUBLIC long TrackerConfigureObject(long objectId, bool isFixedAperture, bool isOccultedStar, double startingX, double startingY, double apertureInPixels);
		private static extern int TrackerConfigureObject(int objectId, bool isFixedAperture, bool isOccultedStar, double startingX, double startingY, double apertureInPixels);

		[DllImport(LIBRARY_TANGRA_CORE, CallingConvention = CallingConvention.Cdecl)]
		private static extern int TrackerGetTargetPsf(int objectId, [In, Out] NativePsfFitInfo psfInfo, [In, Out] double[] residuals);

		internal static void ConfigureTrackedObject(int objectId, TrackedObjectConfig obj)
		{
			TrackerConfigureObject(
				objectId,
				obj.IsFixedAperture, 
				obj.TrackingType == TrackingType.OccultedStar, 
				obj.ApertureStartingX, 
				obj.ApertureStartingX,
				obj.ApertureInPixels);
		}

		internal static bool TrackNextFrame(int frameId, [In, Out] uint[] pixels)
		{
			int rv = TrackerNextFrame(frameId, pixels);
			return rv == 0;
		}
	}
}
