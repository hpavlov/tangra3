/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Tangra.Model.Astro;
using Tangra.Model.Config;
using Tangra.VideoOperations.LightCurves.Tracking;

namespace Tangra.PInvoke
{
	[StructLayout(LayoutKind.Explicit)]
	internal class NativeTrackedObjectInfo
	{
		[FieldOffset(0)]
		public float CenterX;
		[FieldOffset(4)]
		public float CenterY;
		[FieldOffset(8)]
		public float LastGoodPositionX;
		[FieldOffset(12)]
		public float LastGoodPositionY;
		[FieldOffset(16)]
        public uint IsLocated;
		[FieldOffset(20)]
        public uint IsOffScreen;
		[FieldOffset(24)]
		public uint TrackingFlags;
        [FieldOffset(28)]
        public float LastGoodPsfCertainty;
	}

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

		public NativeTrackedObjectPsfFit(int bpp)
		{
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

				case 16:
					m_Saturation = TangraConfig.Settings.Photometry.Saturation.Saturation16Bit;
					break;

				default:
					m_Saturation = TangraConfig.Settings.Photometry.Saturation.Saturation8Bit;
					break;
			}
		}

		public void LoadFromNativePsfFitInfo(NativePsfFitInfo psfInfo, double[] residuals)
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

		public void DrawDataPixels(System.Drawing.Graphics g, System.Drawing.Rectangle rect, float aperture, System.Drawing.Pen aperturePen, int bpp, uint normVal)
		{
			PSFFit.DrawDataPixels(g, rect, aperture, aperturePen, bpp, normVal, this);
		}

		public void DrawGraph(System.Drawing.Graphics g, System.Drawing.Rectangle rect, int bpp, uint normVal, float aperture = 0)
		{
			PSFFit.DrawGraph(g, rect, bpp, normVal, this, m_Saturation, aperture);
		}
	}

	internal static class NativeTracking
	{
		private const string LIBRARY_TANGRA_CORE = "TangraCore";

		[DllImport(LIBRARY_TANGRA_CORE, CallingConvention = CallingConvention.Cdecl)]
		// DLL_PUBLIC long TrackerSettings(double maxElongation, double minFWHM, double maxFWHM, double minCertainty);
		private static extern int TrackerSettings(double maxElongation, double minFWHM, double maxFWHM, double minCertainty);

		[DllImport(LIBRARY_TANGRA_CORE, CallingConvention = CallingConvention.Cdecl)]
		// DLL_PUBLIC long TrackerNewConfiguration(long width, long height, long numTrackedObjects, bool isFullDisappearance, PSFFittingDataRange dataRange);
		private static extern int TrackerNewConfiguration(int width, int height, int numTrackedObjects, bool isFullDisappearance, PSFFittingDataRange dataRange, uint maxPixelValue);

		[DllImport(LIBRARY_TANGRA_CORE, CallingConvention = CallingConvention.Cdecl)]
		// DLL_PUBLIC long TrackerNextFrame(long frameId, unsigned long* pixels);
		private static extern int TrackerNextFrame(int frameId, [In, Out] uint[] pixels);

		[DllImport(LIBRARY_TANGRA_CORE, CallingConvention = CallingConvention.Cdecl)]
		// DLL_PUBLIC long TrackerConfigureObject(long objectId, bool isFixedAperture, bool isOccultedStar, double startingX, double startingY, double apertureInPixels);
		private static extern int TrackerConfigureObject(int objectId, bool isFixedAperture, bool isOccultedStar, double startingX, double startingY, double apertureInPixels);

		[DllImport(LIBRARY_TANGRA_CORE, CallingConvention = CallingConvention.Cdecl)]
		// DLL_PUBLIC long TrackerGetTargetState(long objectId, NativeTrackedObjectInfo* trackingInfo, NativePsfFitInfo* psfInfo, double* residuals);
		private static extern int TrackerGetTargetState(int objectId, [In, Out] NativeTrackedObjectInfo trackingInfo, [In, Out] NativePsfFitInfo psfInfo, [In, Out] double[] residuals);

		[DllImport(LIBRARY_TANGRA_CORE, CallingConvention = CallingConvention.Cdecl)]
		//DLL_PUBLIC void ConfigureSaturationLevels(unsigned long saturation8Bit, unsigned long saturation12Bit, unsigned long saturation14Bit, unsigned long saturation16Bit);
		private static extern int ConfigureSaturationLevels(uint saturation8Bit, uint saturation12Bit, uint saturation14Bit, uint saturation16Bit);

		[DllImport(LIBRARY_TANGRA_CORE, CallingConvention = CallingConvention.Cdecl)]
		//DLL_PUBLIC long TrackerInitialiseNewTracking();
		private static extern int TrackerInitialiseNewTracking();

		[DllImport(LIBRARY_TANGRA_CORE, CallingConvention = CallingConvention.Cdecl)]
		//DLL_PUBLIC long TrackerDoManualFrameCorrection(long objectId, long deltaX, long deltaY);
		private static extern int TrackerDoManualFrameCorrection(int objectId, int deltaX, int deltaY);

		internal static void ConfigureNativeTracker()
		{
			TrackerSettings(
				TangraConfig.Settings.Tracking.CheckElongation ? TangraConfig.Settings.Tracking.AdHokMaxElongation : 0,
				TangraConfig.Settings.Tracking.AdHokMinFWHM,
				TangraConfig.Settings.Tracking.AdHokMaxFWHM,
				TangraConfig.Settings.Tracking.AdHokMinCertainty);

			ConfigureSaturationLevels(
				TangraConfig.Settings.Photometry.Saturation.Saturation8Bit,
				TangraConfig.Settings.Photometry.Saturation.Saturation12Bit,
				TangraConfig.Settings.Photometry.Saturation.Saturation14Bit,
				TangraConfig.Settings.Photometry.Saturation.Saturation16Bit);
		}

		private static int s_NumTrackedObjects;

		internal static void InitNewTracker(int width, int height, int numTrackedObjects, bool isFullDisappearance, PSFFittingDataRange dataRange, uint maxPixelValue)
		{
            int rv = TrackerNewConfiguration(width, height, numTrackedObjects, isFullDisappearance, dataRange, maxPixelValue);

			if (rv == 0)
			{
				s_NumTrackedObjects = numTrackedObjects;
			}
		}

		internal static void InitialiseNewTracking()
		{
			TrackerInitialiseNewTracking();
		}

		internal static void ConfigureTrackedObject(int objectId, TrackedObjectConfig obj)
		{
			TrackerConfigureObject(
				objectId,
				obj.IsFixedAperture,
				obj.TrackingType == TrackingType.OccultedStar,
				obj.ApertureStartingX,
				obj.ApertureStartingY,
				obj.ApertureInPixels);
		}

		internal static bool TrackNextFrame(int frameId, uint[] pixels, List<NativeTrackedObject> managedTrackedObjects)
		{
			int rv = TrackerNextFrame(frameId, pixels);

			for (int i = 0; i < s_NumTrackedObjects; i++)
			{
				var trackingInfo = new NativeTrackedObjectInfo();
				var psfInfo = new NativePsfFitInfo();
				var residuals = new double[35 * 35];

				TrackerGetTargetState(i, trackingInfo, psfInfo, residuals);

				managedTrackedObjects[i].LoadFromNativeData(trackingInfo, psfInfo, residuals);
			}

			return rv == 0;
		}

		internal static bool DoManualFrameCorrection(int objectId, int deltaX, int deltaY)
		{
			return TrackerDoManualFrameCorrection(objectId, deltaX, deltaY) == 0;
		}
	}
}
