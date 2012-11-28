using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Tangra.PInvoke
{
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
		public int BitmapImageSize;		
		public byte[] EngineBuffer = new byte[16];
		public byte[] VideoFileTypeBuffer = new byte[32];

		public string Engine
		{
			get { return Encoding.ASCII.GetString(EngineBuffer).Trim('\0'); }
		}

		public string VideoFileType
		{
			get { return Encoding.ASCII.GetString(VideoFileTypeBuffer).Trim('\0'); }
		}
	}

	public static class TangraVideo
	{
		private const string LIBRARY_TANGRA_VIDEO = "TangraVideo";

		[DllImport(LIBRARY_TANGRA_VIDEO, CallingConvention = CallingConvention.Cdecl)]
		//HRESULT TangraVideoEnumVideoEngines(char* videoEngines);
		private static extern int TangraVideoEnumVideoEngines([In, Out] byte[] videoEngines);

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
		//HRESULT GetVersion();
		private static extern int GetProductVersion();

		private static VideoFileInfo s_fileInfo; 

		public static string[] EnumVideoEngines()
		{
			byte[] buffer = new byte[255];
			TangraVideoEnumVideoEngines(buffer);

			return Encoding.ASCII.GetString(buffer).Trim('\0').Split(';');
		}

		public static void SetVideoEngine(int videoEngineIndex)
		{
			TangraVideoSetVideoEngine(videoEngineIndex);
		}

		public static VideoFileInfo OpenFile(string fileName)
		{
			VideoFileInfo fileInfo = new VideoFileInfo();

			TangraVideoOpenFile(fileName, fileInfo);

			s_fileInfo = new VideoFileInfo()
			{
				Width = fileInfo.Width,
				Height = fileInfo.Height,
				FrameRate = fileInfo.FrameRate,
				CountFrames = fileInfo.CountFrames,
				FirstFrame = fileInfo.FirstFrame,
				BitmapImageSize = fileInfo.BitmapImageSize
			};

			return fileInfo;
		}

		public static void CloseFile()
		{
			TangraVideoCloseFile();
			s_fileInfo = null;
		}

		public static void GetFrame(int frameNo, out uint[] pixels, out Bitmap videoFrame)
		{
			if (s_fileInfo != null)
			{
				pixels = new uint[s_fileInfo.Width * s_fileInfo.Height];
				byte[] bitmapPixels = new byte[s_fileInfo.BitmapImageSize + 40 + 14 + 1];
				byte[] bitmapBytes = new byte[s_fileInfo.Width * s_fileInfo.Height];

				TangraVideoGetFrame(frameNo, pixels, bitmapPixels, bitmapBytes);

				using (MemoryStream memStr = new MemoryStream(bitmapPixels))
				{
					videoFrame = (Bitmap)Bitmap.FromStream(memStr);
				}
			}
			else
			{
				throw new InvalidOperationException();
			}			
		}

		public static void GetFramePixels(int frameNo, out uint[] pixels)
		{
			throw new NotImplementedException();
		}

		public static void GetIntegratedFrame(int startFrameNo, int framesToIntegrate, bool isSlidingIntegration, bool isMedianAveraging, out uint[] pixels, out byte[] bitmapPixels, out byte[] bitmapBytes)
		{
			throw new NotImplementedException();
		}

		public static string GetVideoEngineVersion()
		{
			int ver = GetProductVersion();

			int major = ver >> 28;
			int minor = ver & 0x0FFFFFFF >> 28;
			int revision = ver & 0x000FFFFF;

			return string.Format("{0}.{1}.{2}", major, minor, revision);
		}
	}
}
