/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Tangra.Model.Helpers;
using Tangra.Model.Image;

namespace Tangra.PInvoke
{
	public class TangraVideoException : Exception
	{
		public TangraVideoException(string message)
			: base(message)
		{ }
	}

	// http://www.mono-project.com/Interop_with_Native_Libraries
	// http://docs.go-mono.com/index.aspx?link=T%3ASystem.Runtime.InteropServices.MarshalAsAttribute
	[StructLayout(LayoutKind.Sequential)]
	public class VideoFileInfo
	{
		public int Width;
		public int Height;
		public float FrameRate;
		public int CountFrames;
		public int FirstFrame;
		public int VideoEncodedBitmapImageSize;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
		public byte[] EngineBuffer = new byte[16];

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
		public byte[] VideoFileTypeBuffer = new byte[32];

		public string Engine
		{
			get
			{
				if (EngineBuffer[0] == 0)
					return "UnknownVideoEngine";
				else
					return Encoding.ASCII.GetString(EngineBuffer).Trim('\0');
			}
		}

		public string VideoFileType
		{
			get
			{
                if (VideoFileTypeBuffer[0] == 0)
                {
                    if (VideoFileTypeBuffer[1] == 0 && VideoFileTypeBuffer[2] == 0 && VideoFileTypeBuffer[3] == 0)
						return "AVI.BI_RGB"; // FourCC of 0x00000000 is BI_RGB according to the specs
                    else
						return "AVI.Unknown";
                }					
				else
					return "AVI." + Encoding.ASCII.GetString(VideoFileTypeBuffer).Trim('\0');
			}
		}
	}

	public static class TangraVideo
	{
		private const string LIBRARY_TANGRA_VIDEO = "TangraVideo";

		[DllImport(LIBRARY_TANGRA_VIDEO, CallingConvention = CallingConvention.Cdecl)]
		//HRESULT TangraVideoEnumVideoEngines(char* videoEngines);
		private static extern int TangraVideoEnumVideoEngines([In, Out] byte[] videoEngines, int buffLen);

		[DllImport(LIBRARY_TANGRA_VIDEO, CallingConvention = CallingConvention.Cdecl)]
		//HRESULT TangraVideoSetVideoEngine(int videoEngine);
		private static extern int TangraVideoSetVideoEngine(int videoEngine);

		[DllImport(LIBRARY_TANGRA_VIDEO, CallingConvention = CallingConvention.Cdecl)]
		//HRESULT TangraVideoOpenFile(char* fileName, VideoFileInfo* fileInfo);
		private static extern int TangraVideoOpenFile(string fileName, [In, Out] VideoFileInfo fileInfo);

		[DllImport(LIBRARY_TANGRA_VIDEO, CallingConvention = CallingConvention.Cdecl)]
		//HRESULT TangraVideoCloseFile();
		private static extern int TangraVideoCloseFile();

		[DllImport(LIBRARY_TANGRA_VIDEO, CallingConvention = CallingConvention.Cdecl)]
        //HRESULT TangraVideoGetFrame(long frameNo, unsigned long* pixels, unsigned char* bitmapPixels, unsigned char* bitmapBytes);
        private static extern int TangraVideoGetFrame(int frameNo, [Out] uint[] pixels, [Out] byte[] bitmapPixels, [Out] byte[] bitmapBytes);

		[DllImport(LIBRARY_TANGRA_VIDEO, CallingConvention = CallingConvention.Cdecl)]
        //HRESULT TangraVideoGetFramePixels(long frameNo, unsigned long* pixels);
        private static extern int TangraVideoGetFramePixels(int frameNo, uint[] pixels);

		[DllImport(LIBRARY_TANGRA_VIDEO, CallingConvention = CallingConvention.Cdecl)]
        //HRESULT TangraVideoGetIntegratedFrame(long startFrameNo, long framesToIntegrate, bool isSlidingIntegration, bool isMedianAveraging, unsigned long* pixels, unsigned char* bitmapPixels, unsigned char* bitmapBytes);
        private static extern int TangraVideoGetIntegratedFrame(int startFrameNo, int framesToIntegrate, bool isSlidingIntegration, bool isMedianAveraging, uint[] pixels, byte[] bitmapPixels, byte[] bitmapBytes);

        [DllImport(LIBRARY_TANGRA_VIDEO, CallingConvention = CallingConvention.Cdecl)]
        private static extern int TangraCreateNewAviFile([MarshalAs(UnmanagedType.LPStr)]string fileName, int width, int height, int bpp, double fps, bool showCompressionDialog);

        [DllImport(LIBRARY_TANGRA_VIDEO, CallingConvention = CallingConvention.Cdecl)]
        private static extern int TangraAviFileAddFrame([In, MarshalAs(UnmanagedType.LPArray)] int[,] pixels);

        [DllImport(LIBRARY_TANGRA_VIDEO, CallingConvention = CallingConvention.Cdecl)]
        private static extern int TangraAviFileClose();

        [DllImport(LIBRARY_TANGRA_VIDEO, CallingConvention = CallingConvention.Cdecl)]
        private static extern int TangraGetLastAviFileError(IntPtr errorMessage, int buffLen);


		[DllImport(LIBRARY_TANGRA_VIDEO, CallingConvention = CallingConvention.Cdecl)]
		//HRESULT GetVersion();
		private static extern int GetTangraVideoVersion();

		private static VideoFileInfo s_fileInfo; 

		public static string[] EnumVideoEngines()
		{
			byte[] buffer = new byte[255];
			TangraVideoEnumVideoEngines(buffer, 255);

			return Encoding.ASCII.GetString(buffer).Trim('\0').Split(';');
		}

		public static void SetVideoEngine(int videoEngineIndex)
		{
			TangraVideoSetVideoEngine(videoEngineIndex);
		}

		public static VideoFileInfo OpenFile(string fileName)
		{
			string fileExtension = Path.GetExtension(fileName).ToLower().Trim('.');

			if (fileExtension != "avi" && fileExtension != "avs")
			{
				throw new TangraVideoException(string.Format("'.{0}' video files are not supported.", fileExtension));
			}

			VideoFileInfo fileInfo = new VideoFileInfo();

			TangraVideoOpenFile(fileName, fileInfo);

			if (fileInfo.CountFrames == 0)
			{
				// TODO: Get an error message from TangraVideo
				throw new TangraVideoException("Tangra was unable to load this file. Check file format and integrity.");
			}
				

			s_fileInfo = new VideoFileInfo()
			{
				Width = fileInfo.Width,
				Height = fileInfo.Height,
				FrameRate = fileInfo.FrameRate,
				CountFrames = fileInfo.CountFrames,
				FirstFrame = fileInfo.FirstFrame,
				VideoEncodedBitmapImageSize = fileInfo.VideoEncodedBitmapImageSize,
				VideoFileTypeBuffer = fileInfo.VideoFileTypeBuffer,
				EngineBuffer = fileInfo.EngineBuffer
			};

			return fileInfo;
		}

		public static void CloseFile()
		{
			TangraVideoCloseFile();
			s_fileInfo = null;
		}

		private static Font s_ErrorFont = new Font(FontFamily.GenericMonospace, 10, FontStyle.Regular);

        private static object s_SyncRoot = new object();
        
        public static void GetFrame(int frameNo, out uint[] pixels, out uint[] originalPixels, out Bitmap videoFrame, out byte[] bitmapBytes)
        {
            originalPixels = null;

			if (s_fileInfo != null)
			{
				int width = s_fileInfo.Width;
				int height = s_fileInfo.Height;
				pixels = new uint[width * height];
                originalPixels = new uint[width * height];
				byte[] bitmapPixels = new byte[width * height * 3 + 40 + 14 + 1];
				bitmapBytes = new byte[width * height];
				int rv = -1;
                try
                {
                    lock (s_SyncRoot)
                    {
                        rv = TangraVideoGetFrame(frameNo, pixels, bitmapPixels, bitmapBytes);

                        if (rv == 0)
                        {
                            byte[] rawBitmapBytes = new byte[(width * height * 3) + 40 + 14 + 1];

                            Array.Copy(pixels, originalPixels, pixels.Length);

                            TangraCore.PreProcessors.ApplyPreProcessingPixelsOnly(originalPixels, pixels, width, height, 8, 0 /* No normal value for FITS files */, 0 /* No exposure support for 8 bit darks. They must be same exposure */);

                            TangraCore.GetBitmapPixels(width, height, pixels, rawBitmapBytes, bitmapBytes, true, 8, 0);

                            videoFrame = Pixelmap.ConstructBitmapFromBitmapPixels(bitmapBytes, width, height);
                        }
                        else
                            throw new InvalidOperationException("The core returned an error when trying to get a frame. Error code: " + rv.ToString());
                    }
                }
                catch (Exception ex)
                {
					if (rv == 0)
					{
						try
						{
							videoFrame = Pixelmap.ConstructBitmapFromBitmapPixels(bitmapBytes, width, height);
							return;
						}
						catch (Exception ex2)
						{
							Trace.WriteLine(ex2.GetFullStackTrace());
						}
					}

                    videoFrame = new Bitmap(width, height);
                    using (Graphics g = Graphics.FromImage(videoFrame))
                    {
                        g.Clear(Color.White);
                        g.DrawString(ex.Message, s_ErrorFont, Brushes.Red, 10, 10);
                        g.Save();
                    }
                }
			}
			else
			{
                videoFrame = new Bitmap(100, 100);
                using (Graphics g = Graphics.FromImage(videoFrame))
                {
                    g.Clear(Color.White);
                    g.DrawString("Invalid file.", s_ErrorFont, Brushes.Red, 10, 10);
                    g.Save();
                }
                bitmapBytes = new byte[100 * 100];
                pixels = new uint[100 * 100];
			}
		}

        public static void GetFramePixels(int frameNo, out uint[] pixels, out uint[] originalPixels)
		{
			if (s_fileInfo != null)
			{
				pixels = new uint[s_fileInfo.Width * s_fileInfo.Height];
                originalPixels = new uint[s_fileInfo.Width * s_fileInfo.Height];
                TangraVideoGetFramePixels(frameNo, pixels);

                Array.Copy(pixels, originalPixels, pixels.Length);

                TangraCore.PreProcessors.ApplyPreProcessingPixelsOnly(originalPixels, pixels, s_fileInfo.Width, s_fileInfo.Height, 8, 0 /* No normal value for FITS files */, 0 /* No exposure support for 8 bit darks. They must be same exposure */);

			}
			else
			{
                pixels = new uint[100 * 100];
                originalPixels = new uint[100 * 100];
			}	
		}

        public static void GetIntegratedFrame(int startFrameNo, int framesToIntegrate, bool isSlidingIntegration, bool isMedianAveraging, out uint[] pixels, out uint[] originalPixels, out Bitmap bitmap, out byte[] bitmapBytes)
        {
             if (s_fileInfo != null)
			{
                originalPixels = new uint[s_fileInfo.Width * s_fileInfo.Height];
				pixels = new uint[s_fileInfo.Width * s_fileInfo.Height];
				byte[] bitmapPixels = new byte[s_fileInfo.Width * s_fileInfo.Height * 3 + 40 + 14 + 1];
				bitmapBytes = new byte[s_fileInfo.Width * s_fileInfo.Height];

                TangraVideoGetIntegratedFrame(startFrameNo, framesToIntegrate, isSlidingIntegration, isMedianAveraging, pixels, bitmapPixels, bitmapBytes);

                byte[] rawBitmapBytes = new byte[(s_fileInfo.Width * s_fileInfo.Height * 3) + 40 + 14 + 1];

                Array.Copy(pixels, originalPixels, pixels.Length);

                TangraCore.PreProcessors.ApplyPreProcessingPixelsOnly(originalPixels, pixels, s_fileInfo.Width, s_fileInfo.Height, 8, 0 /* No normal value for FITS files */, 0 /* No exposure support for 8 bit darks. They must be same exposure */);

				TangraCore.GetBitmapPixels(s_fileInfo.Width, s_fileInfo.Height, pixels, rawBitmapBytes, bitmapBytes, true, 8, 0);

				bitmap = Pixelmap.ConstructBitmapFromBitmapPixels(bitmapBytes, s_fileInfo.Width, s_fileInfo.Height);	
			}
			else
			{
                bitmap = new Bitmap(100, 100);
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.Clear(Color.White);
                    g.DrawString("Invalid file.", s_ErrorFont, Brushes.Red, 10, 10);
                    g.Save();
                }

                bitmapBytes = new byte[100 * 100];
                pixels = new uint[100 * 100];
                originalPixels = new uint[100 * 100];
			}
		}

		public static string GetVideoEngineVersion()
		{
            int ver = TangraVideo.GetTangraVideoVersion();

			int major = ver >> 28;
			int minor = ver & 0x0FFFFFFF >> 28;
			int revision = ver & 0x000FFFFF;

			return string.Format("{0}.{1}.{2}", major, minor, revision);
		}

        public static string GetLastAviErrorMessage()
        {
            string error = null;
            IntPtr buffer = IntPtr.Zero;

            try
            {
                byte[] errorMessage = new byte[200];
                buffer = Marshal.AllocHGlobal(errorMessage.Length + 1);
                Marshal.Copy(errorMessage, 0, buffer, errorMessage.Length);
                Marshal.WriteByte(buffer + errorMessage.Length, 0); // terminating null

                TangraGetLastAviFileError(buffer, 200);

                error = Marshal.PtrToStringAnsi(buffer);

                if (error != null)
                    error = error.Trim();
            }
            finally
            {
                if (buffer != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(buffer);
                }
            }

            return error;
        }

        private static void TraceLastNativeError()
        {
            string error = GetLastAviErrorMessage();

            Trace.WriteLine(error, "VideoNativeException");
        }

        public static bool StartNewAviFile(string fileName, int width, int height, int bpp, double fps, bool showCompressionDialog)
        {
            if (TangraCreateNewAviFile(fileName, width, height, bpp, fps, showCompressionDialog) != 0)
            {
                TraceLastNativeError();
                return false;
            }

            return true;
        }

		private static int s_BitPixTableBitPixVal = 0;
	    private static double s_BitPixTableMaxVal = 0.0;
        private static int[] s_BitPixTable = new int[ushort.MaxValue + 1];
		private static double s_GammaTableGammaVal = double.MaxValue + 1;
		private static int[] s_GammaTable = new int[256];

        public static bool AddAviVideoFrame(Pixelmap pixmap, double addedGamma, int? adv16NormalisationValue)
        {
            int[,] pixels = new int[pixmap.Height, pixmap.Width];
			bool usesGamma = Math.Abs(addedGamma - 1) > 0.01;
            bool needsBitPixConversion = pixmap.BitPixCamera > 8 || (adv16NormalisationValue != null && adv16NormalisationValue.Value > 0);
            
            double maxVal = pixmap.BitPixCamera.GetMaxValueForBitPix();
            if (adv16NormalisationValue != null && adv16NormalisationValue.Value > 0)
                maxVal = adv16NormalisationValue.Value;

            if (Math.Abs(s_BitPixTableBitPixVal - pixmap.BitPixCamera) > 0.01 || Math.Abs(s_BitPixTableMaxVal - maxVal) > 0.01)
			{
				for (int i = 0; i <= maxVal; i++)
				{
					double convVal = 255.0 * i / maxVal;

					s_BitPixTable[i] = Math.Max(0, Math.Min(255, (int)Math.Round(convVal)));
				}

				s_BitPixTableBitPixVal = pixmap.BitPixCamera;
			    s_BitPixTableMaxVal = maxVal;
			}

	        if (Math.Abs(s_GammaTableGammaVal - addedGamma) > 0.01)
            {
				double gammaFactor = usesGamma ? (255.0 / Math.Pow(255.0, addedGamma)) : 0;

                for (int i = 0; i < 256; i++)
                {
                    double convVal = gammaFactor * Math.Pow(i, addedGamma);

                    s_GammaTable[i] = Math.Max(0, Math.Min(255, (int) Math.Round(convVal)));
                }

                s_GammaTableGammaVal = addedGamma;
            }

            for (int x = 0; x < pixmap.Width; x++) 
            {
                for (int y = 0; y < pixmap.Height; y++)
                {
	                uint conv8BitVal = pixmap.Pixels[x + y*pixmap.Width];
	                if (needsBitPixConversion)
		                conv8BitVal = (uint)s_BitPixTable[conv8BitVal];

					if (usesGamma)
						pixels[y, x] = s_GammaTable[conv8BitVal];
					else
						pixels[y, x] =  Math.Max(0, Math.Min(255, (int)conv8BitVal));
                }
            }

            if (TangraAviFileAddFrame(pixels) != 0)
            {
                TraceLastNativeError();
                return false;
            }

            return true;
        }

        public static void CloseAviFile()
        {
            if (TangraAviFileClose() != 0)
            {
                TraceLastNativeError();
            }
        }
	}
}
