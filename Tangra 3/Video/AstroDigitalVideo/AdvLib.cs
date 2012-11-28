using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Tangra.Video.AstroDigitalVideo
{
    public static class AdvLib
    {
        private const string ADVLIB_DLL_NAME = "AdvLib.dll";

        [DllImport(ADVLIB_DLL_NAME)]
        private static extern void AdvNewFile(byte[] fileName, byte[] compression, int keyFrames);

        [DllImport(ADVLIB_DLL_NAME)]
        private static extern IntPtr AdvGetCurrentFilePath();

        [DllImport(ADVLIB_DLL_NAME)]
        private static extern void AdvDefineImageSection(ushort width, ushort height, byte bpp);

        [DllImport(ADVLIB_DLL_NAME)]
        private static extern void AdvDefineStatusSectionTag(byte[] tagName, byte tagType);

        [DllImport(ADVLIB_DLL_NAME)]
        private static extern uint AdvAddFileTag(byte[] tagName, byte[] tagValue);

        [DllImport(ADVLIB_DLL_NAME)]
        private static extern void AdvAddOrUpdateImageSectionTag(byte[] tagName, byte[] tagValue);

        [DllImport(ADVLIB_DLL_NAME)]
        private static extern void AdvEndFile();

        [DllImport(ADVLIB_DLL_NAME)]
        private static extern void AdvBeginFrame(long timeStamp, uint elapsedTime, uint exposure);

        [DllImport(ADVLIB_DLL_NAME)]
        private static extern void AdvFrameAddImage(ushort[] pixels);

        [DllImport(ADVLIB_DLL_NAME)]
        private static extern void AdvFrameAddStatusTag(uint tagIndex, byte[] tagValue);

        [DllImport(ADVLIB_DLL_NAME)]
        private static extern void AdvEndFrame();

        public enum StatusTagType
        {
			UInt16 = 0,
			UInt32 = 1,
			ULong64 = 2,
			AnsiString255 = 3
        }

        public static void NewFile(string fileName, string compression, int keyFrames)
        {
            AdvNewFile(Encoding.ASCII.GetBytes(fileName), Encoding.ASCII.GetBytes(compression), keyFrames);
        }

        public static string GetCurrentFilePath()
        {
            IntPtr ptr = AdvGetCurrentFilePath();
            return Marshal.PtrToStringAnsi(ptr);
        }

        public static void DefineImageSection(ushort width, ushort height, byte bpp)
        {
            AdvDefineImageSection(width, height, bpp);
        }

        public static void DefineStatusSectionTag(string tagName, StatusTagType tagType)
        {
            AdvDefineStatusSectionTag(Encoding.ASCII.GetBytes(tagName), (byte)tagType);
        }

        public static uint AddFileTag(string tagName, string tagValue)
        {
            return AdvAddFileTag(Encoding.ASCII.GetBytes(tagName), Encoding.ASCII.GetBytes(tagValue));
        }

        public static void AddOrUpdateImageSectionTag(string tagName, string tagValue)
        {
            AdvAddOrUpdateImageSectionTag(Encoding.ASCII.GetBytes(tagName), Encoding.ASCII.GetBytes(tagValue));
        }

        public static void EndFile()
        {
            AdvEndFile();
        }

        public static void BeginFrame(long timeStamp, uint elapsedTime, uint exposure)
        {
            AdvBeginFrame(timeStamp, elapsedTime, exposure);
        }

        public static void FrameAddImage(ushort[,] pixels)
        {
            int width = pixels.GetLength(0);
            int height = pixels.GetLength(1);

            ushort[] array = new ushort[width * height];

            int idx = -1;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    idx++;
                    array[idx] = pixels[x, y];
                }
            }

            AdvFrameAddImage(array);
        }

        public static void FrameAddStatusTag(uint tagIndex, string tagValue)
        {
            AdvFrameAddStatusTag(tagIndex, Encoding.ASCII.GetBytes(tagValue));
        }

        public static void EndFrame()
        {
            AdvEndFrame();
        }
    }
}
