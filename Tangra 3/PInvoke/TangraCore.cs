using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Tangra.PInvoke
{
	[StructLayout(LayoutKind.Sequential)]
	public struct AdvFileInfo
	{
		public int Width;
		public int Height;
		public float FrameRate;
		public int CountFrames;
		public int Bpp;
	};


	public class AdvFrameInfo : AdvFrameInfoNative
	{
		public AdvFrameInfo(AdvFrameInfoNative copyFrom)
		{
			MidFrameTimeStampMillisecondsLo = copyFrom.MidFrameTimeStampMillisecondsLo;
			MidFrameTimeStampMillisecondsHi = copyFrom.MidFrameTimeStampMillisecondsHi;
			Exposure10thMs = copyFrom.Exposure10thMs;
			Gamma = copyFrom.Gamma;
			Gain = copyFrom.Gain;
			Shutter = copyFrom.Shutter;
			Offset = copyFrom.Offset;
			SystemTimeLo = copyFrom.SystemTimeLo;
			SystemTimeHi = copyFrom.SystemTimeHi;
			GPSTrackedSattelites = copyFrom.GPSTrackedSattelites;
			GPSAlmanacStatus = copyFrom.GPSAlmanacStatus;
			GPSFixStatus = copyFrom.GPSFixStatus;
			VideoCameraFrameIdLo = copyFrom.VideoCameraFrameIdLo;
			VideoCameraFrameIdHi = copyFrom.VideoCameraFrameIdHi;
			HardwareTimerFrameIdLo = copyFrom.HardwareTimerFrameIdLo;
			HardwareTimerFrameIdHi = copyFrom.HardwareTimerFrameIdHi;
		}

		public string GPSFixString;

		public string SystemErrorString;

		public string UserCommandString;

		internal static string GetStringFromBytes(byte[] chars)
		{
			string str = Encoding.ASCII.GetString(chars);
			return str.Substring(0, str.IndexOf('\0'));
		}
	}

	[StructLayout(LayoutKind.Explicit)]
	public class AdvFrameInfoNative
	{
		private static DateTime REFERENCE_DATETIME = new DateTime(2010, 1, 1, 0, 0, 0, 0);

		public AdvFrameInfoNative()
		{
			MidFrameTimeStampMillisecondsLo = 675;
			MidFrameTimeStampMillisecondsHi = 987;
			Exposure10thMs = 1234;
			Gamma = 12.34f;
			Gain = 23.45f;
			Shutter = 34.56f;
			Offset = 45.67f;
			SystemTimeLo = 100;
			SystemTimeHi = 234;
			GPSTrackedSattelites = 17;
			GPSAlmanacStatus = 18;
			GPSFixStatus = 19;
		}

		[FieldOffset(0)]
		public uint MidFrameTimeStampMillisecondsLo;
		[FieldOffset(4)]
		public uint MidFrameTimeStampMillisecondsHi;
		[FieldOffset(8)]
		public int Exposure10thMs;
		[FieldOffset(12)]
		public float Gamma;
		[FieldOffset(16)]
		public float Gain;
		[FieldOffset(20)]
		public float Shutter;
		[FieldOffset(24)]
		public float Offset;
		[FieldOffset(28)]
		public uint SystemTimeLo;
		[FieldOffset(32)]
		public uint SystemTimeHi;
		[FieldOffset(36)]
		public byte GPSTrackedSattelites;
		[FieldOffset(37)]
		public byte GPSAlmanacStatus;
		[FieldOffset(38)]
		public byte GPSFixStatus;
		[FieldOffset(40)]
		public uint VideoCameraFrameIdLo;
		[FieldOffset(44)]
		public uint VideoCameraFrameIdHi;
		[FieldOffset(48)]
		public uint HardwareTimerFrameIdLo;
		[FieldOffset(52)]
		public uint HardwareTimerFrameIdHi;

		public DateTime MiddleExposureTimeStamp
		{
			get
			{
				long millisecondsElapsed = (((long)MidFrameTimeStampMillisecondsHi) << 32) + (long)MidFrameTimeStampMillisecondsLo;
				return REFERENCE_DATETIME.AddMilliseconds(millisecondsElapsed);
			}
		}

		public DateTime SystemTime
		{
			get
			{
				long millisecondsElapsed = (((long)SystemTimeHi) << 32) + (long)SystemTimeLo;
				return REFERENCE_DATETIME.AddMilliseconds(millisecondsElapsed);
			}
		}

		public long VideoCameraFrameId
		{
			get
			{
				return (((long)VideoCameraFrameIdHi) << 32) + (long)VideoCameraFrameIdLo;
			}
		}

		public long HardwareTimerFrameId
		{
			get
			{
				return (((long)HardwareTimerFrameIdHi) << 32) + (long)HardwareTimerFrameIdLo;
			}
		}
	}

	public static class TangraCore
	{
		private const string LIBRARY_TANGRA_CORE = "TangraCore";

		[DllImport(LIBRARY_TANGRA_CORE, CallingConvention = CallingConvention.Cdecl)]
		//HRESULT ADVOpenFile(char* fileName, AdvLib::AdvFileInfo* fileInfo);
		public static extern int ADVOpenFile(string fileName, [In, Out] ref AdvFileInfo fileInfo);

		[DllImport(LIBRARY_TANGRA_CORE, CallingConvention = CallingConvention.Cdecl)]
		//HRESULT ADVCloseFile();
		public static extern int ADVCloseFile();
				
		[DllImport(LIBRARY_TANGRA_CORE, CallingConvention = CallingConvention.Cdecl)]
		//HRESULT ADVGetFrame(int frameNo, unsigned long* pixels, BYTE* bitmapPixels, BYTE* bitmapBytes, AdvLib::AdvFrameInfo* frameInfo, char* gpsFix, char* userCommand, char* systemError);
		public static extern int ADVGetFrame(int frameNo, [In, Out] uint[] pixels, [In, Out] byte[] bitmapBytes, [In, Out] byte[] bitmapDisplayBytes, [In, Out] AdvFrameInfoNative frameInfo, [In, Out] byte[] gpsFix, [In, Out] byte[] userCommand, [In, Out] byte[] systemError);

		[DllImport(LIBRARY_TANGRA_CORE, CallingConvention = CallingConvention.Cdecl)]
		//HRESULT ADVGetIntegratedFrame(int startFrameNo, int framesToIntegrate, bool isSlidingIntegration, bool isMedianAveraging, unsigned long* pixels, BYTE* bitmapBytes, BYTE* bitmapDisplayBytes, AdvLib::AdvFrameInfo* frameInfo);
		public static extern int ADVGetIntegratedFrame(int startFrameNo, int framesToIntegrate, bool isSlidingIntegration, bool isMedianAveraging, [Out] uint[] pixels, [Out] byte[] bitmapBytes, [Out] byte[] bitmapDisplayBytes, [In, Out] AdvFrameInfoNative frameInfo);

		[DllImport(LIBRARY_TANGRA_CORE, CallingConvention = CallingConvention.Cdecl)]
		//HRESULT GetVersion();
		private static extern int GetProductVersion();

		public static string GetTangraCoreVersion()
		{
			int ver = GetProductVersion();

			int major = ver >> 28;
			int minor = ver & 0x0FFFFFFF >> 28;
			int revision = ver & 0x000FFFFF;

			return string.Format("{0}.{1}.{2}", major, minor, revision);
		}		
	}
}
