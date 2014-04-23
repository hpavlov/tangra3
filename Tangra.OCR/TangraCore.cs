using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Tangra.Model.Config;

namespace Tangra.PInvoke
{
	public static class TangraCore
	{
		internal const string LIBRARY_TANGRA_CORE = "TangraCore";

		[DllImport(LIBRARY_TANGRA_CORE, CallingConvention = CallingConvention.Cdecl)]
		//HRESULT PrepareImageForOCR(unsigned long* pixels, long bpp, long width, long height);
		private static extern int PrepareImageForOCR([In, Out] uint[] pixels, int bpp, int width, int height);

		[DllImport(LIBRARY_TANGRA_CORE, CallingConvention = CallingConvention.Cdecl)]
		//HRESULT PrepareImageForOCRSingleStep(unsigned long* pixels, long bpp, long width, long height, long stepNo, unsigned long* average);
		private static extern int PrepareImageForOCRSingleStep([In, Out] uint[] pixels, int bpp, int width, int height, int stepNo, [In, Out] ref uint average);

        [DllImport(LIBRARY_TANGRA_CORE, CallingConvention = CallingConvention.Cdecl)]
        //HRESULT LargeChunkDenoise(unsigned long* pixels, long width, long height, unsigned long onColour, unsigned long offColour);
        private static extern int LargeChunkDenoise([In, Out] uint[] pixels, int width, int height, uint onColour, uint offColour);

		public static void PrepareImageForOCR(uint[] pixelsIn, uint[] pixelsOut, int width, int height)
		{
			Array.Copy(pixelsIn, pixelsOut, width * height);

			PrepareImageForOCR(pixelsOut, 8, width, height);
		}

		public static void PrepareImageForOCRSingleStep(uint[] pixelsIn, uint[] pixelsOut, int width, int height, int stepNo, ref uint average)
		{
			Array.Copy(pixelsIn, pixelsOut, width * height);

			PrepareImageForOCRSingleStep(pixelsOut, 8, width, height, stepNo, ref average);
		}

        public static void LargeChunkDenoise(uint[] pixels, int width, int height)
        {
            LargeChunkDenoise(pixels, width, height, 0, 255);
        }
	}

}
