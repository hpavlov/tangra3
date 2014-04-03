/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

public enum AdvTagType
{
	UInt8 = 0,
	UInt16 = 1,
	UInt32 = 2,
	ULong64 = 3,
	Real = 4, // IEEE/REAL*4
	AnsiString255 = 5,
	List16OfAnsiString255 = 6,
};

public static class AdvLib
{
	[DllImport("kernel32.dll", SetLastError = false)]
	private static extern bool SetDllDirectory(string lpPathName);

	static AdvLib()
	{
		SetDllDirectory(AppDomain.CurrentDomain.BaseDirectory);
	}

	internal const string LIBRARY_ADVLIB_CORE32 = "AdvLib.Core32.dll";
	internal const string LIBRARY_ADVLIB_CORE64 = "AdvLib.Core64.dll";

	[DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvNewFile")]
	//void AdvNewFile(const char* fileName);
	private static extern void AdvNewFile32(string fileName);

	[DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvEndFile")]
	//void AdvEndFile();
	private static extern void AdvEndFile32();

	[DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvGetCurrentFilePath")]
	//char* AdvGetCurrentFilePath(void);
	private static extern string AdvGetCurrentFilePath32();

	[DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvDefineImageSection")]
	//void AdvDefineImageSection(unsigned short width, unsigned short height, unsigned char dataBpp);
	private static extern void AdvDefineImageSection32(ushort width, ushort height, byte dataBpp);

	[DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "AdvDefineImageLayout")]
	//void AdvDefineImageLayout(unsigned char layoutId, const char* layoutType, const char* compression, unsigned char layoutBpp, int keyFrame, const char* diffCorrFromBaseFrame);
	private static extern void AdvDefineImageLayout32(byte layoutId, [MarshalAs(UnmanagedType.LPStr)]string layoutType, [MarshalAs(UnmanagedType.LPStr)]string compression, byte layoutBpp, int keyFrame, [MarshalAs(UnmanagedType.LPStr)]string diffCorrFromBaseFrame);

	[DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "AdvDefineStatusSectionTag")]
	//unsigned int AdvDefineStatusSectionTag(const char* tagName, int tagType);
	private static extern uint AdvDefineStatusSectionTag32([MarshalAs(UnmanagedType.LPStr)]string tagName, AdvTagType tagType);

	[DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "AdvAddFileTag")]
	//unsigned int AdvAddFileTag(const char* tagName, const char* tagValue);
	private static extern uint AdvAddFileTag32([MarshalAs(UnmanagedType.LPStr)]string tagName, [MarshalAs(UnmanagedType.LPStr)]string tagValue);

	[DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "AdvAddOrUpdateImageSectionTag")]
	//void AdvAddOrUpdateImageSectionTag(const char* tagName, const char* tagValue);
	private static extern uint AdvAddOrUpdateImageSectionTag32([MarshalAs(UnmanagedType.LPStr)]string tagName, [MarshalAs(UnmanagedType.LPStr)]string tagValue);

	[DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvBeginFrame")]
	//bool AdvBeginFrame(long long timeStamp, unsigned int elapsedTime, unsigned int exposure);
	private static extern bool AdvBeginFrame32(long timeStamp, uint elapsedTime, uint exposure);

	[DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvFrameAddImage")]
	//void AdvFrameAddImage(unsigned char layoutId, unsigned short* pixels, unsigned char pixelsBpp);
	private static extern void AdvFrameAddImage32(byte layoutId, [In, MarshalAs(UnmanagedType.LPArray)] ushort[] pixels, byte pixelsBpp);

	[DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvFrameAddImageBytes")]
	//void AdvFrameAddImageBytes(unsigned char layoutId, unsigned char* pixels, unsigned char pixelsBpp);
	private static extern void AdvFrameAddImageBytes32(byte layoutId, [In, MarshalAs(UnmanagedType.LPArray)] byte[] pixels, byte pixelsBpp);

	[DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "AdvFrameAddStatusTag")]
	//void AdvFrameAddStatusTag(unsigned int tagIndex, const char* tagValue);
	private static extern void AdvFrameAddStatusTag32(uint tagIndex, [MarshalAs(UnmanagedType.LPStr)]string tagValue);

	[DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "AdvFrameAddStatusTagMessage")]
	//void AdvFrameAddStatusTagMessage(unsigned int tagIndex, const char* tagValue);
	private static extern void AdvFrameAddStatusTagMessage32(uint tagIndex, [MarshalAs(UnmanagedType.LPStr)]string tagValue);

	[DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvFrameAddStatusTagUInt8")]
	//void AdvFrameAddStatusTagUInt8(unsigned int tagIndex, unsigned char tagValue);
	private static extern void AdvFrameAddStatusTagUInt832(uint tagIndex, byte tagValue);

	[DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvFrameAddStatusTag16")]
	//void AdvFrameAddStatusTag16(unsigned int tagIndex, unsigned short tagValue);
	private static extern void AdvFrameAddStatusTag1632(uint tagIndex, ushort tagValue);

	[DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvFrameAddStatusTagReal")]
	//void AdvFrameAddStatusTagReal(unsigned int tagIndex, float tagValue);
	private static extern void AdvFrameAddStatusTagReal32(uint tagIndex, float tagValue);

	[DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvFrameAddStatusTag32")]
	//void AdvFrameAddStatusTag32(unsigned int tagIndex, unsigned long tagValue);
	private static extern void AdvFrameAddStatusTag3232(uint tagIndex, uint tagValue);

	[DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvFrameAddStatusTag64")]
	//void AdvFrameAddStatusTag64(unsigned int tagIndex, long long tagValue);
	private static extern void AdvFrameAddStatusTag6432(uint tagIndex, ulong tagValue);

	[DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvEndFrame")]
	//void AdvEndFrame();
	private static extern void AdvEndFrame32();



	[DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvNewFile")]
	//void AdvNewFile(const char* fileName);
	private static extern void AdvNewFile64(string fileName);

	[DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvEndFile")]
	//void AdvEndFile();
	private static extern void AdvEndFile64();

	[DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvGetCurrentFilePath")]
	//char* AdvGetCurrentFilePath(void);
	private static extern string AdvGetCurrentFilePath64();

	[DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvDefineImageSection")]
	//void AdvDefineImageSection(unsigned short width, unsigned short height, unsigned char dataBpp);
	private static extern void AdvDefineImageSection64(ushort width, ushort height, byte dataBpp);

	[DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "AdvDefineImageLayout")]
	//void AdvDefineImageLayout(unsigned char layoutId, const char* layoutType, const char* compression, unsigned char layoutBpp, int keyFrame, const char* diffCorrFromBaseFrame);
	private static extern void AdvDefineImageLayout64(byte layoutId, [MarshalAs(UnmanagedType.LPStr)]string layoutType, [MarshalAs(UnmanagedType.LPStr)]string compression, byte layoutBpp, int keyFrame, [MarshalAs(UnmanagedType.LPStr)]string diffCorrFromBaseFrame);

	[DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "AdvDefineStatusSectionTag")]
	//unsigned int AdvDefineStatusSectionTag(const char* tagName, int tagType);
	private static extern uint AdvDefineStatusSectionTag64([MarshalAs(UnmanagedType.LPStr)]string tagName, AdvTagType tagType);

	[DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "AdvAddFileTag")]
	//unsigned int AdvAddFileTag(const char* tagName, const char* tagValue);
	private static extern uint AdvAddFileTag64([MarshalAs(UnmanagedType.LPStr)]string tagName, [MarshalAs(UnmanagedType.LPStr)]string tagValue);

	[DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "AdvAddOrUpdateImageSectionTag")]
	//void AdvAddOrUpdateImageSectionTag(const char* tagName, const char* tagValue);
	private static extern uint AdvAddOrUpdateImageSectionTag64([MarshalAs(UnmanagedType.LPStr)]string tagName, [MarshalAs(UnmanagedType.LPStr)]string tagValue);

	[DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvBeginFrame")]
	//bool AdvBeginFrame(long long timeStamp, unsigned int elapsedTime, unsigned int exposure);
	private static extern bool AdvBeginFrame64(long timeStamp, uint elapsedTime, uint exposure);

	[DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvFrameAddImage")]
	//void AdvFrameAddImage(unsigned char layoutId, unsigned short* pixels, unsigned char pixelsBpp);
	private static extern void AdvFrameAddImage64(byte layoutId, [In, MarshalAs(UnmanagedType.LPArray)] ushort[] pixels, byte pixelsBpp);

	[DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvFrameAddImageBytes")]
	//void AdvFrameAddImageBytes(unsigned char layoutId, unsigned char* pixels, unsigned char pixelsBpp);
	private static extern void AdvFrameAddImageBytes64(byte layoutId, [In, MarshalAs(UnmanagedType.LPArray)] byte[] pixels, byte pixelsBpp);

	[DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "AdvFrameAddStatusTag")]
	//void AdvFrameAddStatusTag(unsigned int tagIndex, const char* tagValue);
	private static extern void AdvFrameAddStatusTag64(uint tagIndex, [MarshalAs(UnmanagedType.LPStr)]string tagValue);

	[DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "AdvFrameAddStatusTagMessage")]
	//void AdvFrameAddStatusTagMessage(unsigned int tagIndex, const char* tagValue);
	private static extern void AdvFrameAddStatusTagMessage64(uint tagIndex, [MarshalAs(UnmanagedType.LPStr)]string tagValue);

	[DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvFrameAddStatusTagUInt8")]
	//void AdvFrameAddStatusTagUInt8(unsigned int tagIndex, unsigned char tagValue);
	private static extern void AdvFrameAddStatusTagUInt864(uint tagIndex, byte tagValue);

	[DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvFrameAddStatusTag16")]
	//void AdvFrameAddStatusTag16(unsigned int tagIndex, unsigned short tagValue);
	private static extern void AdvFrameAddStatusTag1664(uint tagIndex, ushort tagValue);

	[DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvFrameAddStatusTagReal")]
	//void AdvFrameAddStatusTagReal(unsigned int tagIndex, float tagValue);
	private static extern void AdvFrameAddStatusTagReal64(uint tagIndex, float tagValue);

	[DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvFrameAddStatusTag32")]
	//void AdvFrameAddStatusTag32(unsigned int tagIndex, unsigned long tagValue);
	private static extern void AdvFrameAddStatusTag3264(uint tagIndex, uint tagValue);

	[DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvFrameAddStatusTag64")]
	//void AdvFrameAddStatusTag64(unsigned int tagIndex, long long tagValue);
	private static extern void AdvFrameAddStatusTag6464(uint tagIndex, ulong tagValue);

	[DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvEndFrame")]
	//void AdvEndFrame();
	private static extern void AdvEndFrame64();

	public static void AdvNewFile(string fileName)
	{
		if (Is64Bit())
			AdvNewFile64(fileName);
		else
			AdvNewFile32(fileName);
	}

	public static void AdvEndFile()
	{
		if (Is64Bit())
			AdvEndFile64();
		else
			AdvEndFile32();		
	}

	public static string AdvGetCurrentFilePath()
	{
		if (Is64Bit())
			return AdvGetCurrentFilePath64();
		else
			return AdvGetCurrentFilePath32();
	}

	public static void AdvDefineImageSection(ushort width, ushort height, byte dataBpp)
	{
		if (Is64Bit())
			AdvDefineImageSection64(width, height, dataBpp);
		else
			AdvDefineImageSection32(width, height, dataBpp);
	}

	public static void AdvDefineImageLayout(byte layoutId, string layoutType, string compression, byte layoutBpp, int keyFrame, string diffCorrFromBaseFrame)
	{
		if (Is64Bit())
			AdvDefineImageLayout64(layoutId, layoutType, compression, layoutBpp, keyFrame, diffCorrFromBaseFrame);
		else
			AdvDefineImageLayout32(layoutId, layoutType, compression, layoutBpp, keyFrame, diffCorrFromBaseFrame);
	}

	public static uint AdvDefineStatusSectionTag(string tagName, AdvTagType tagType)
	{
		if (Is64Bit())
			return AdvDefineStatusSectionTag64(tagName, tagType);
		else
			return AdvDefineStatusSectionTag32(tagName, tagType);
	}

	public static uint AdvAddFileTag(string tagName, string tagValue)
	{
		if (Is64Bit())
			return AdvAddFileTag64(tagName, tagValue);
		else
			return AdvAddFileTag32(tagName, tagValue);
	}

	public static uint AdvAddOrUpdateImageSectionTag(string tagName, string tagValue)
	{
		if (Is64Bit())
			return AdvAddOrUpdateImageSectionTag64(tagName, tagValue);
		else
			return AdvAddOrUpdateImageSectionTag32(tagName, tagValue);
	}

	public static bool AdvBeginFrame(long timeStamp, uint elapsedTime, uint exposure)
	{
		if (Is64Bit())
			return AdvBeginFrame64(timeStamp, elapsedTime, exposure);
		else
			return AdvBeginFrame32(timeStamp, elapsedTime, exposure);
	}

	public static void AdvFrameAddImage(byte layoutId, [In, MarshalAs(UnmanagedType.LPArray)] ushort[] pixels, byte pixelsBpp)
	{
		if (Is64Bit())
			AdvFrameAddImage64(layoutId, pixels, pixelsBpp);
		else
			AdvFrameAddImage32(layoutId, pixels, pixelsBpp);
	}

	public static void AdvFrameAddImageBytes(byte layoutId, [In, MarshalAs(UnmanagedType.LPArray)] byte[] pixels, byte pixelsBpp)
	{
		if (Is64Bit())
			AdvFrameAddImageBytes64(layoutId, pixels, pixelsBpp);
		else
			AdvFrameAddImageBytes32(layoutId, pixels, pixelsBpp);
	}

	public static void AdvFrameAddStatusTag(uint tagIndex, string tagValue)
	{
		if (Is64Bit())
			AdvFrameAddStatusTag64(tagIndex, tagValue);
		else
			AdvFrameAddStatusTag32(tagIndex, tagValue);
	}

	public static void AdvFrameAddStatusTagMessage(uint tagIndex, string tagValue)
	{
		if (Is64Bit())
			AdvFrameAddStatusTagMessage64(tagIndex, tagValue);
		else
			AdvFrameAddStatusTagMessage32(tagIndex, tagValue);
	}

	public static void AdvFrameAddStatusTagUInt8(uint tagIndex, byte tagValue)
	{
		if (Is64Bit())
			AdvFrameAddStatusTagUInt864(tagIndex, tagValue);
		else
			AdvFrameAddStatusTagUInt832(tagIndex, tagValue);
	}

	public static void AdvFrameAddStatusTag16(uint tagIndex, ushort tagValue)
	{
		if (Is64Bit())
			AdvFrameAddStatusTag1664(tagIndex, tagValue);
		else
			AdvFrameAddStatusTag1632(tagIndex, tagValue);
	}

	public static void AdvFrameAddStatusTagReal(uint tagIndex, float tagValue)
	{
		if (Is64Bit())
			AdvFrameAddStatusTagReal64(tagIndex, tagValue);
		else
			AdvFrameAddStatusTagReal32(tagIndex, tagValue);
	}

	public static void AdvFrameAddStatusTag32(uint tagIndex, uint tagValue)
	{
		if (Is64Bit())
			AdvFrameAddStatusTag3264(tagIndex, tagValue);
		else
			AdvFrameAddStatusTag3232(tagIndex, tagValue);
	}

	public static void AdvFrameAddStatusTag64(uint tagIndex, ulong tagValue)
	{
		if (Is64Bit())
			AdvFrameAddStatusTag6464(tagIndex, tagValue);
		else
			AdvFrameAddStatusTag6432(tagIndex, tagValue);
	}

	public static void AdvEndFrame()
	{
		if (Is64Bit())
			AdvEndFrame64();
		else
			AdvEndFrame32();
	}

	private static bool Is64Bit()
	{
		//Check whether we are running on a 32 or 64bit system.
		if (IntPtr.Size == 8)
		{
			return true;
		}
		else
		{
			return false;
		}
	}
}
