/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
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
		public uint Aav16NormVal;
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
			EndFrameNtpTimeStampMillisecondsLo = copyFrom.EndFrameNtpTimeStampMillisecondsLo;
			EndFrameNtpTimeStampMillisecondsHi = copyFrom.EndFrameNtpTimeStampMillisecondsHi;
			EndFrameSecondaryTimeStampMillisecondsLo = copyFrom.EndFrameSecondaryTimeStampMillisecondsLo;
			EndFrameSecondaryTimeStampMillisecondsHi = copyFrom.EndFrameSecondaryTimeStampMillisecondsHi;
			NtpTimeStampError = copyFrom.NtpTimeStampError;
			
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
			EndFrameNtpTimeStampMillisecondsLo = 0;
			EndFrameNtpTimeStampMillisecondsHi = 0;
			EndFrameSecondaryTimeStampMillisecondsLo = 0;
			EndFrameSecondaryTimeStampMillisecondsHi = 0;
			NtpTimeStampError = -1;
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
			EndFrameNtpTimeStampMillisecondsLo = 0;
			EndFrameNtpTimeStampMillisecondsHi = 0;
			EndFrameSecondaryTimeStampMillisecondsLo = 0;
			EndFrameSecondaryTimeStampMillisecondsHi = 0;
			NtpTimeStampError = -1;
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
		[FieldOffset(60)]
		public uint EndFrameNtpTimeStampMillisecondsLo;
		[FieldOffset(64)]
		public uint EndFrameNtpTimeStampMillisecondsHi;
		[FieldOffset(68)]
		public uint EndFrameSecondaryTimeStampMillisecondsLo;
		[FieldOffset(72)]
		public uint EndFrameSecondaryTimeStampMillisecondsHi;
		[FieldOffset(76)]
		public int NtpTimeStampError;

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

        public bool HasNtpTimeStamp
        {
            get { return EndFrameNtpTimeStampMillisecondsHi != 0 && EndFrameNtpTimeStampMillisecondsLo != 0; }
        }

		public DateTime EndExposureNtpTimeStamp
		{
			get
			{
				long millisecondsElapsed = (((long)EndFrameNtpTimeStampMillisecondsHi) << 32) + (long)EndFrameNtpTimeStampMillisecondsLo;
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

		public DateTime EndExposureSecondaryTimeStamp
		{
			get
			{
				long millisecondsElapsed = (((long)EndFrameSecondaryTimeStampMillisecondsHi) << 32) + (long)EndFrameSecondaryTimeStampMillisecondsLo;
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

	[StructLayout(LayoutKind.Sequential)]
	public struct SerFileInfo
	{
		public int CameraId;
		public int ColourId;
		public int LittleEndian;
		public int Width;
		public int Height;
		public int PixelDepthPerPlane;
		public int CountFrames;
		public int SequenceStartTimeLo;
		public int SequenceStartTimeHi;
		public int SequenceStartTimeUTCLo;
		public int SequenceStartTimeUTCHi;
		public uint NormalisationValue;
	};

	[StructLayout(LayoutKind.Sequential)]
	public struct SerNativeFrameInfo
	{
		public int TimeStampLo;
		public int TimeStampHi;
	};

	public class SerFrameInfo
	{
		public DateTime TimeStamp { get; private set; }

		internal SerFrameInfo(SerNativeFrameInfo nativeInfo)
		{
		    try
		    {
                TimeStamp = new DateTime((long)nativeInfo.TimeStampLo + ((long)nativeInfo.TimeStampHi << 32));
		    }
            catch (ArgumentOutOfRangeException aex)
            {
                Trace.WriteLine(aex);
                TimeStamp = DateTime.MinValue;
            }
		}
	}

	public static class TangraCore
	{
		internal const string LIBRARY_TANGRA_CORE = "TangraCore";

		[DllImport(LIBRARY_TANGRA_CORE, CallingConvention = CallingConvention.Cdecl)]
        // DLL_PUBLIC HRESULT ADVOpenFile(char* fileName, AdvLib::AdvFileInfo* fileInfo);
		public static extern int ADVOpenFile(string fileName, [In, Out] ref AdvFileInfo fileInfo);

		[DllImport(LIBRARY_TANGRA_CORE, CallingConvention = CallingConvention.Cdecl)]
        // DLL_PUBLIC HRESULT ADVCloseFile();
		public static extern int ADVCloseFile();
				
		[DllImport(LIBRARY_TANGRA_CORE, CallingConvention = CallingConvention.Cdecl)]
        // DLL_PUBLIC HRESULT ADVGetFrame(long frameNo, unsigned long* pixels, unsigned long* originalPixels, BYTE* bitmapPixels, BYTE* bitmapBytes, AdvLib::AdvFrameInfo* frameInfo, char* gpsFix, char* userCommand, char* systemError);
        public static extern int ADVGetFrame(int frameNo, [In, Out] uint[] pixels, [In, Out] uint[] originalPixels, [In, Out] byte[] bitmapBytes, [In, Out] byte[] bitmapDisplayBytes, [In, Out] AdvFrameInfoNative frameInfo, [In, Out] byte[] gpsFix, [In, Out] byte[] userCommand, [In, Out] byte[] systemError);

		[DllImport(LIBRARY_TANGRA_CORE, CallingConvention = CallingConvention.Cdecl)]
        // DLL_PUBLIC HRESULT ADVGetIntegratedFrame(long startFrameNo, long framesToIntegrate, bool isSlidingIntegration, bool isMedianAveraging, unsigned long* pixels, unsigned long* originalPixels, BYTE* bitmapBytes, BYTE* bitmapDisplayBytes, AdvLib::AdvFrameInfo* frameInfo);
        public static extern int ADVGetIntegratedFrame(int startFrameNo, int framesToIntegrate, bool isSlidingIntegration, bool isMedianAveraging, [Out] uint[] pixels, [In, Out] uint[] originalPixels, [Out] byte[] bitmapBytes, [Out] byte[] bitmapDisplayBytes, [In, Out] AdvFrameInfoNative frameInfo);

		[DllImport(LIBRARY_TANGRA_CORE, CallingConvention = CallingConvention.Cdecl)]
        // DLL_PUBLIC HRESULT ADVGetFrameStatusChannel(long frameNo, AdvLib::AdvFrameInfo* frameInfo, char* gpsFix, char* userCommand, char* systemError);
		public static extern int ADVGetFrameStatusChannel(int frameNo, [In, Out] AdvFrameInfoNative frameInfo, [In, Out] byte[] gpsFix, [In, Out] byte[] userCommand, [In, Out] byte[] systemError);

		[DllImport(LIBRARY_TANGRA_CORE, CallingConvention = CallingConvention.Cdecl)]
		//HRESULT ADVGetFileTag(char* tagName, char* tagValue);
		public static extern int ADVGetFileTag(string tagName, [In, Out] byte[] tagValue);


		[DllImport(LIBRARY_TANGRA_CORE, CallingConvention = CallingConvention.Cdecl)]
        // DLL_PUBLIC HRESULT GetBitmapPixels(long width, long height, unsigned long* pixels, BYTE* bitmapPixels, BYTE* bitmapBytes, bool isLittleEndian, int bpp, unsigned long normVal);
		public static extern int GetBitmapPixels(int width, int height, [In] uint[] pixels, [In, Out] byte[] bitmapBytes, [In, Out] byte[] bitmapDisplayBytes, bool isLittleEndian, ushort dataBpp, uint normVal);
		
		[DllImport(LIBRARY_TANGRA_CORE, CallingConvention = CallingConvention.Cdecl)]
        // DLL_PUBLIC HRESULT BitmapSplitFieldsOSD(BYTE* bitmapPixels, long firstOsdLine, long lastOsdLine);
		public static extern int BitmapSplitFieldsOSD([In, Out] byte[] bitmapBytes, int firstOsdLine, int lastOsdLine);

		[DllImport(LIBRARY_TANGRA_CORE, CallingConvention = CallingConvention.Cdecl)]
		//HRESULT GetVersion();
		private static extern int GetProductVersion();

		[DllImport(LIBRARY_TANGRA_CORE, CallingConvention = CallingConvention.Cdecl)]
        // DLL_PUBLIC HRESULT Lagarith16Decompress(long width, long height, unsigned char* compressedBytes, unsigned char* decompressedBytes);
		private static extern int Lagarith16Decompress(int width, int height, [In] byte[] compressedBytes, [In, Out] byte[] decompressedBytes);


		[DllImport(LIBRARY_TANGRA_CORE, CallingConvention = CallingConvention.Cdecl)]
        // DLL_PUBLIC HRESULT SEROpenFile(char* fileName, SerLib::SerFileInfo* fileInfo, char* observer, char* instrument, char* telescope, bool checkMagic);
		public static extern int SEROpenFile(string fileName, [In, Out] ref SerFileInfo fileInfo, [In, Out] byte[] observer, [In, Out] byte[] instrument, [In, Out] byte[] telescope, bool checkMagic);

		[DllImport(LIBRARY_TANGRA_CORE, CallingConvention = CallingConvention.Cdecl)]
        // DLL_PUBLIC HRESULT SERCloseFile();
		public static extern int SERCloseFile();

		[DllImport(LIBRARY_TANGRA_CORE, CallingConvention = CallingConvention.Cdecl)]
        // DLL_PUBLIC HRESULT SERGetFrame(long frameNo, unsigned long* pixels, unsigned long* originalPixels, BYTE* bitmapPixels, BYTE* bitmapBytes, unsigned int cameraBitPix, SerLib::SerFrameInfo* frameInfo);		
        public static extern int SERGetFrame(int frameNo, [In, Out] uint[] pixels, [In, Out] uint[] originalPixels, [In, Out] byte[] bitmapBytes, [In, Out] byte[] bitmapDisplayBytes, ushort cameraBitPix, [In, Out] ref SerNativeFrameInfo frameInfo);

		[DllImport(LIBRARY_TANGRA_CORE, CallingConvention = CallingConvention.Cdecl)]
        // DLL_PUBLIC HRESULT SERGetIntegratedFrame(long startFrameNo, long framesToIntegrate, bool isSlidingIntegration, bool isMedianAveraging, unsigned long* pixels, unsigned long* originalPixels, BYTE* bitmapBytes, BYTE* bitmapDisplayBytes, unsigned int cameraBitPix, SerLib::SerFrameInfo* frameInfo);
        public static extern int SERGetIntegratedFrame(int startFrameNo, int framesToIntegrate, bool isSlidingIntegration, bool isMedianAveraging, [Out] uint[] pixels, [In, Out] uint[] originalPixels, [Out] byte[] bitmapBytes, [Out] byte[] bitmapDisplayBytes, ushort cameraBitPix, [In, Out] ref SerNativeFrameInfo frameInfo);


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
            private static extern int PreProcessingAddDarkFrame(float[] darkFramePixels, uint pixelsCount, float exposureSeconds, bool isBiasCorrected, bool isSameExposure);

			[DllImport(LIBRARY_TANGRA_CORE, CallingConvention = CallingConvention.Cdecl)]
            private static extern int PreProcessingAddFlatFrame(float[] flatFramePixels, uint pixelsCount, float flatFrameMedian);

			[DllImport(LIBRARY_TANGRA_CORE, CallingConvention = CallingConvention.Cdecl)]
            private static extern int PreProcessingAddBiasFrame(float[] biasFramePixels, uint pixelsCount);

			[DllImport(LIBRARY_TANGRA_CORE, CallingConvention = CallingConvention.Cdecl)]
			private static extern int PreProcessingUsesPreProcessing([In, Out] ref bool usesPreProcessing);

			[DllImport(LIBRARY_TANGRA_CORE, CallingConvention = CallingConvention.Cdecl)]
			private static extern int PreProcessingAddFlipAndRotation(int rotateFlipType);

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
					[In, Out] ref ushort flatPixelsCount,
                    [In, Out] ref ushort biasPixelsCount,
					[In, Out] ref RotateFlipType rotateFlipType);
		
			[DllImport(LIBRARY_TANGRA_CORE, CallingConvention = CallingConvention.Cdecl)]
            public static extern int ApplyPreProcessingPixelsOnly(uint[] pixesl, int width, int height, int bpp, uint normVal, float exposureSeconds);

			public static void ClearAll()
			{
				PreProcessingClearAll();
			}

            public static bool UsesPreProcessing()
            {
                bool usesPreProcessing = false;
                PreProcessingUsesPreProcessing(ref usesPreProcessing);
                return usesPreProcessing;
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
                PreProcessingAddGammaCorrection(encodingGamma);
			}

			public static void AddFlipAndRotation(RotateFlipType rotateFlipType)
			{
				PreProcessingAddFlipAndRotation((int)rotateFlipType);
			}

            public static void AddDarkFrame(float[,] darkFramePixels, float exposureSeconds, bool isBiasCorrected, bool isSameExposure)
			{
				int width = darkFramePixels.GetLength(0);
				int height = darkFramePixels.GetLength(1);

				uint pixelsCount = (uint)(width * height);
				float[] darkFrame = new float[pixelsCount];

				int idx = 0;
				
				for (int y = 0; y < height; y++)
					for (int x = 0; x < width; x++)
					{
                        darkFrame[idx] = darkFramePixels[x, y];
						idx++;
					}

                PreProcessingAddDarkFrame(darkFrame, pixelsCount, exposureSeconds, isBiasCorrected, isSameExposure);
			}

            public static void AddFlatFrame(float[,] flatFramePixels, float flatFrameMedian)
			{
				int width = flatFramePixels.GetLength(0);
				int height = flatFramePixels.GetLength(1);

				uint pixelsCount = (uint)(width * height);
				float[] flatFrame = new float[pixelsCount];

				int idx = 0;

				for (int y = 0; y < height; y++)
					for (int x = 0; x < width; x++)
					{
						flatFrame[idx] = flatFramePixels[x, y];
						idx++;
					}

                PreProcessingAddFlatFrame(flatFrame, pixelsCount, flatFrameMedian);
			}

            public static void AddBiasFrame(float[,] biasFramePixels)
            {
                int width = biasFramePixels.GetLength(0);
                int height = biasFramePixels.GetLength(1);

                uint pixelsCount = (uint)(width * height);
                float[] biasFrame = new float[pixelsCount];

                int idx = 0;

                for (int y = 0; y < height; y++)
                    for (int x = 0; x < width; x++)
                    {
                        biasFrame[idx] = biasFramePixels[x, y];
                        idx++;
                    }

                PreProcessingAddBiasFrame(biasFrame, pixelsCount);
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
				    ushort biasPixelsCount = 0;
					RotateFlipType rotateFlipType = 0;

					PreProcessingGetConfig(ref preProcessingType, ref fromValue, ref toValue, ref brigtness, ref contrast, ref filter, ref gamma, ref darkPixelsCount, ref flatPixelsCount, ref biasPixelsCount, ref rotateFlipType);

					preProcessingInfo.PreProcessingType = preProcessingType;
					preProcessingInfo.RotateFlipType = rotateFlipType;

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
				    preProcessingInfo.BiasFrameBytes = biasPixelsCount;
				}
			}

			public static bool PreProcessingHasDarkFrameSet()
			{
				PreProcessingInfo preProcessingInfo;
				PreProcessingGetConfig(out preProcessingInfo);

				return preProcessingInfo.DarkFrameBytes > 0;
			}

            public static bool PreProcessingHasBiasFrameSet()
            {
                PreProcessingInfo preProcessingInfo;
                PreProcessingGetConfig(out preProcessingInfo);

                return preProcessingInfo.BiasFrameBytes > 0;
            }
		}

		public static byte[] Lagarith16Decompress(uint width, uint height, byte[] compressedBytes)
		{
			var rv = new byte[width * height * 2 + 16];
			Lagarith16Decompress((int) width, (int) height, compressedBytes, rv);

			return rv;
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
	    public ushort BiasFrameBytes;
		public TangraConfig.PreProcessingFilter Filter;
		public RotateFlipType RotateFlipType;
	}

	
}
