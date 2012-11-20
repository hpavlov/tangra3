using System;
using System.Collections.Generic;
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
		private static extern int TangraVideoOpenFile(string fileName, VideoFileInfo fileInfo);

		[DllImport(LIBRARY_TANGRA_VIDEO, CallingConvention = CallingConvention.Cdecl)]
		//HRESULT TangraVideoCloseFile();
		private static extern int TangraVideoCloseFile();

		[DllImport(LIBRARY_TANGRA_VIDEO, CallingConvention = CallingConvention.Cdecl)]
		//HRESULT TangraVideoGetFrame(long frameNo, unsigned long* pixels, unsigned char* bitmapPixels, unsigned char* bitmapBytes);
		private static extern int TangraVideoGetFrame(long frameNo, uint[] pixels, byte[] bitmapPixels, byte[] bitmapBytes);

		[DllImport(LIBRARY_TANGRA_VIDEO, CallingConvention = CallingConvention.Cdecl)]
		//HRESULT TangraVideoGetFramePixels(long frameNo, unsigned long* pixels);
		private static extern int TangraVideoGetFramePixels(long frameNo, uint[] pixels);

		[DllImport(LIBRARY_TANGRA_VIDEO, CallingConvention = CallingConvention.Cdecl)]
		//HRESULT TangraVideoGetIntegratedFrame(long startFrameNo, long framesToIntegrate, bool isSlidingIntegration, bool isMedianAveraging, unsigned long* pixels, unsigned char* bitmapPixels, unsigned char* bitmapBytes);
		private static extern int TangraVideoGetIntegratedFrame(long startFrameNo, long framesToIntegrate, bool isSlidingIntegration, bool isMedianAveraging, uint[] pixels, byte[] bitmapPixels, byte[] bitmapBytes);


		public static string[] EnumVideoEngines()
		{
			byte[] buffer = new byte[255];
			TangraVideoEnumVideoEngines(buffer);

			return Encoding.ASCII.GetString(buffer).Trim('\0').Split(';');
		}

		public static void SetVideoEngine()
		{
			throw new NotImplementedException();
		}

		public static VideoFileInfo OpenFile(string fileName)
		{
			throw new NotImplementedException();
		}

		public static void CloseFile()
		{
			TangraVideoCloseFile();
		}

		public static void GetFrame(long frameNo, out uint[] pixels, out byte[] bitmapPixels, out byte[] bitmapBytes)
		{
			throw new NotImplementedException();
		}

		public static void GetFramePixels(long frameNo, out uint[] pixels)
		{
			throw new NotImplementedException();
		}

		public static void GetIntegratedFrame(long startFrameNo, long framesToIntegrate, bool isSlidingIntegration, bool isMedianAveraging, out uint[] pixels, out byte[] bitmapPixels, out byte[] bitmapBytes)
		{
			throw new NotImplementedException();
		}
	}
}
