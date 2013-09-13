using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Tangra.Model.Config;

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
			GPSAlmanacOffset = copyFrom.GPSAlmanacOffset;
			VideoCameraFrameIdLo = copyFrom.VideoCameraFrameIdLo;
			VideoCameraFrameIdHi = copyFrom.VideoCameraFrameIdHi;
			HardwareTimerFrameIdLo = copyFrom.HardwareTimerFrameIdLo;
			HardwareTimerFrameIdHi = copyFrom.HardwareTimerFrameIdHi;
			IntegratedFrames = copyFrom.IntegratedFrames;
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
#if DEBUG
			MidFrameTimeStampMillisecondsLo = 0;
			MidFrameTimeStampMillisecondsHi = 0;
			Exposure10thMs = 0;
			Gamma = 0f;
			Gain = 0f;
			Shutter = 0f;
			Offset = 0f;
			SystemTimeLo = 0;
			SystemTimeHi = 0;
			GPSTrackedSattelites = 0;
			GPSAlmanacStatus = 0;
			GPSFixStatus = 0;
			IntegratedFrames = 0;
#else
			MidFrameTimeStampMillisecondsLo = 0;
			MidFrameTimeStampMillisecondsHi = 0;
			Exposure10thMs = 0;
			Gamma = 0f;
			Gain = 0f;
			Shutter = 0f;
			Offset = 0f;
			SystemTimeLo = 0;
			SystemTimeHi = 0;
			GPSTrackedSattelites = 0;
			GPSAlmanacStatus = 0;
			GPSFixStatus = 0;
#endif
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
		[FieldOffset(39)]
		public byte GPSAlmanacOffset;
		[FieldOffset(40)]
		public uint VideoCameraFrameIdLo;
		[FieldOffset(44)]
		public uint VideoCameraFrameIdHi;
		[FieldOffset(48)]
		public uint HardwareTimerFrameIdLo;
		[FieldOffset(52)]
		public uint HardwareTimerFrameIdHi;
		[FieldOffset(56)]
		public uint IntegratedFrames; 

		public DateTime MiddleExposureTimeStamp
		{
			get
			{
				long millisecondsElapsed = (((long)MidFrameTimeStampMillisecondsHi) << 32) + (long)MidFrameTimeStampMillisecondsLo;
                try
                {
                    return REFERENCE_DATETIME.AddMilliseconds(millisecondsElapsed);    
                }
                catch(ArgumentOutOfRangeException)
                {
                    return REFERENCE_DATETIME;
                }
			}
		}

		public bool HasTimeStamp
		{
			get { return MidFrameTimeStampMillisecondsHi != 0 && MidFrameTimeStampMillisecondsLo != 0; }
		}

		public DateTime SystemTime
		{
			get
			{
				long millisecondsElapsed = (((long)SystemTimeHi) << 32) + (long)SystemTimeLo;
                try
                {
                    return REFERENCE_DATETIME.AddMilliseconds(millisecondsElapsed);
                }
                catch (ArgumentOutOfRangeException)
                {
                    return REFERENCE_DATETIME;
                }
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

		public short GetSignedAlamancOffset()
		{
			short signedOffset = GPSAlmanacOffset;
			if ((GPSAlmanacOffset & 0x80) == 0x80)
				signedOffset = (short) (GPSAlmanacOffset + (0xFF << 8));

			return signedOffset;
		}

		public bool AlmanacStatusIsGood
		{
			get { return GPSAlmanacStatus != 0x00; }			
		}
	}

	public static class TangraCore
	{
		internal const string LIBRARY_TANGRA_CORE = "TangraCore";

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
		//HRESULT ADVGetFrameStatusChannel(int frameNo, AdvLib::AdvFrameInfo* frameInfo, char* gpsFix, char* userCommand, char* systemError);
		public static extern int ADVGetFrameStatusChannel(int frameNo, [In, Out] AdvFrameInfoNative frameInfo, [In, Out] byte[] gpsFix, [In, Out] byte[] userCommand, [In, Out] byte[] systemError);

		[DllImport(LIBRARY_TANGRA_CORE, CallingConvention = CallingConvention.Cdecl)]
		//HRESULT ADVGetFileTag(char* tagName, char* tagValue);
		public static extern int ADVGetFileTag(string tagName, [In, Out] byte[] tagValue);


		[DllImport(LIBRARY_TANGRA_CORE, CallingConvention = CallingConvention.Cdecl)]
		//HRESULT GetBitmapPixels(long width, long height, unsigned long* pixels, BYTE* bitmapPixels, BYTE* bitmapBytes, bool isLittleEndian, int bpp);
		public static extern int GetBitmapPixels(int width, int height, [In] uint[] pixels, [In, Out] byte[] bitmapBytes, [In, Out] byte[] bitmapDisplayBytes, bool isLittleEndian, int bpp);
		
		[DllImport(LIBRARY_TANGRA_CORE, CallingConvention = CallingConvention.Cdecl)]
		//DLL_PUBLIC HRESULT GetBitmapPixels(BYTE* bitmapPixels, long width, long height, long firstOsdLine, long lastOsdLine);
		public static extern int BitmapSplitFieldsOSD([In, Out] byte[] bitmapBytes, int width, int height, int firstOsdLine, int lastOsdLine);

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

		public static class PreProcessors
		{
			[DllImport(LIBRARY_TANGRA_CORE, CallingConvention = CallingConvention.Cdecl)]
			private static extern int PreProcessingClearAll();

			[DllImport(LIBRARY_TANGRA_CORE, CallingConvention = CallingConvention.Cdecl)]
			private static extern int PreProcessingAddStretching(ushort fromValue, ushort toValue);

			[DllImport(LIBRARY_TANGRA_CORE, CallingConvention = CallingConvention.Cdecl)]
			private static extern int PreProcessingAddClipping(ushort fromValue, ushort toValue);

			[DllImport(LIBRARY_TANGRA_CORE, CallingConvention = CallingConvention.Cdecl)]
			private static extern int PreProcessingAddBrightnessContrast(int brigtness, int contrast);

			[DllImport(LIBRARY_TANGRA_CORE, CallingConvention = CallingConvention.Cdecl)]
			private static extern int PreProcessingAddDigitalFilter(int filter);

			[DllImport(LIBRARY_TANGRA_CORE, CallingConvention = CallingConvention.Cdecl)]
			private static extern int PreProcessingAddGammaCorrection(float encodingGamma);

			[DllImport(LIBRARY_TANGRA_CORE, CallingConvention = CallingConvention.Cdecl)]
			private static extern int PreProcessingAddDarkFrame(uint[] darkFramePixels, uint pixelsCount, uint darkFrameMedian);

			[DllImport(LIBRARY_TANGRA_CORE, CallingConvention = CallingConvention.Cdecl)]
			private static extern int PreProcessingDarkFrameAdjustLevelToMedian(bool adjustLevelToMedian);

			[DllImport(LIBRARY_TANGRA_CORE, CallingConvention = CallingConvention.Cdecl)]
			private static extern int PreProcessingAddFlatFrame(uint[] flatFramePixels, uint pixelsCount, uint flatFrameMedian);

			[DllImport(LIBRARY_TANGRA_CORE, CallingConvention = CallingConvention.Cdecl)]
			private static extern int PreProcessingUsesPreProcessing([In, Out] ref bool usesPreProcessing);

			[DllImport(LIBRARY_TANGRA_CORE, CallingConvention = CallingConvention.Cdecl)]
			private static extern int PreProcessingGetConfig(
					[In, Out] ref PreProcessingType preProcessingType,
					[In, Out] ref ushort fromValue,
					[In, Out] ref ushort toValue,
					[In, Out] ref int brigtness,
					[In, Out] ref int contrast,
					[In, Out] ref TangraConfig.PreProcessingFilter filter,
					[In, Out] ref float gamma,
					[In, Out] ref ushort darkPixelsCount,
					[In, Out] ref ushort flatPixelsCount);

			
			[DllImport(LIBRARY_TANGRA_CORE, CallingConvention = CallingConvention.Cdecl)]
			public static extern int ApplyPreProcessingPixelsOnly(uint[] pixesl, int width, int height, int bpp);

			public static void ClearAll()
			{
				PreProcessingClearAll();
			}

			public static void AddStretching(ushort fromValue, ushort toValue)
			{
				PreProcessingAddStretching(fromValue, toValue);
			}

			public static void AddClipping(ushort fromValue, ushort toValue)
			{
				PreProcessingAddClipping(fromValue, toValue);
			}

			public static void AddBrightnessContrast(int brigtness, int contrast)
			{
				PreProcessingAddBrightnessContrast(brigtness, contrast);
			}

			public static void AddDigitalFilter(TangraConfig.PreProcessingFilter filter)
			{
				PreProcessingAddDigitalFilter((int)filter);
			}

			public static void AddGammaCorrection(float encodingGamma)
			{
                if (Math.Abs(encodingGamma - 1.0f) > 0.01f)
				    PreProcessingAddGammaCorrection(encodingGamma);
			}

			public static void AddDarkFrame(uint[,] darkFramePixels, uint darkFrameMedian)
			{
				int width = darkFramePixels.GetLength(0);
				int height = darkFramePixels.GetLength(1);

				uint pixelsCount = (uint)(width * height);
				uint[] darkFrame = new uint[pixelsCount];

				int idx = 0;
				
				for (int y = 0; y < height; y++)
					for (int x = 0; x < width; x++)
					{
						darkFrame[idx] = darkFramePixels[x, y];
						idx++;
					}

				PreProcessingDarkFrameAdjustLevelToMedian(TangraConfig.Settings.Generic.DarkFrameAdjustLevelToMedian);
				PreProcessingAddDarkFrame(darkFrame, pixelsCount, darkFrameMedian);
			}

			public static void SetDarkFrameAdjustLevelToMedian()
			{
				PreProcessingDarkFrameAdjustLevelToMedian(TangraConfig.Settings.Generic.DarkFrameAdjustLevelToMedian);
			}

			public static void AddFlatFrame(uint[,] flatFramePixels, uint flatFrameMedian)
			{
				int width = flatFramePixels.GetLength(0);
				int height = flatFramePixels.GetLength(1);

				uint pixelsCount = (uint)(width * height);
				uint[] flatFrame = new uint[pixelsCount];

				int idx = 0;

				for (int y = 0; y < height; y++)
					for (int x = 0; x < width; x++)
					{
						flatFrame[idx] = flatFramePixels[x, y];
						idx++;
					}

				PreProcessingAddFlatFrame(flatFrame, pixelsCount, flatFrameMedian);
			}

			public static void PreProcessingGetConfig(out PreProcessingInfo preProcessingInfo)
			{
				preProcessingInfo = new PreProcessingInfo();

				bool usesPreProcessing = false;
				PreProcessingUsesPreProcessing(ref usesPreProcessing);

				if (usesPreProcessing)
				{
					preProcessingInfo.PreProcessing = true;

					PreProcessingType preProcessingType = PreProcessingType.None;
					ushort fromValue = 0;
					ushort toValue = 0;
					int brigtness = 0;
					int contrast = 0;
					TangraConfig.PreProcessingFilter filter = 0;
					float gamma = 0;
					ushort darkPixelsCount = 0;
					ushort flatPixelsCount = 0;

					PreProcessingGetConfig(ref preProcessingType, ref fromValue, ref toValue, ref brigtness, ref contrast, ref filter, ref gamma, ref darkPixelsCount, ref flatPixelsCount);

					preProcessingInfo.PreProcessingType = preProcessingType;

					if (preProcessingType == PreProcessingType.BrightnessContrast)
					{
						preProcessingInfo.Brigtness = brigtness;
						preProcessingInfo.Contrast = contrast;
					}
					else if (preProcessingType == PreProcessingType.Stretching)
					{
						preProcessingInfo.StretchingFrom = fromValue;
						preProcessingInfo.StretchingTo = toValue;
					}
					else if (preProcessingType == PreProcessingType.Clipping)
					{
						preProcessingInfo.ClippingFrom = fromValue;
						preProcessingInfo.ClippingTo = toValue;
					}

					preProcessingInfo.GammaCorrection = gamma;
					preProcessingInfo.Filter = filter;
					preProcessingInfo.DarkFrameBytes = darkPixelsCount;
					preProcessingInfo.FlatFrameBytes = flatPixelsCount;
				}
			}

			public static bool PreProcessingHasDarkFrameSet()
			{
				PreProcessingInfo preProcessingInfo;
				PreProcessingGetConfig(out preProcessingInfo);

				return preProcessingInfo.DarkFrameBytes > 0;
			}
		}
	}

	public enum PreProcessingType
	{
		None = 0,
		Stretching = 1,
		Clipping = 2,
		BrightnessContrast = 3
	}

	public class PreProcessingInfo
	{
		public bool PreProcessing;
		public PreProcessingType PreProcessingType;
		public ushort StretchingFrom;
		public ushort StretchingTo;
		public ushort ClippingFrom;
		public ushort ClippingTo;
		public int Brigtness;
		public int Contrast;
		public float GammaCorrection;
		public ushort DarkFrameBytes;
		public ushort FlatFrameBytes;
		public TangraConfig.PreProcessingFilter Filter;
	}

	
}
