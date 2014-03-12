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
	internal const string LIBRARY_TANGRA_CORE = "AdvLib.Core.dll";
	internal const string LIBRARY_TANGRA_CORE32 = "AdvLib.Core32.dll";
	internal const string LIBRARY_TANGRA_CORE64 = "AdvLib.Core64.dll";

	[DllImport(LIBRARY_TANGRA_CORE, CallingConvention = CallingConvention.Cdecl)]
	//void AdvNewFile(const char* fileName);
	public static extern void AdvNewFile(string fileName);

	[DllImport(LIBRARY_TANGRA_CORE, CallingConvention = CallingConvention.Cdecl)]
	//void AdvEndFile();
	public static extern void AdvEndFile();

	[DllImport(LIBRARY_TANGRA_CORE, CallingConvention = CallingConvention.Cdecl)]
	//char* AdvGetCurrentFilePath(void);
	public static extern string AdvGetCurrentFilePath();

	[DllImport(LIBRARY_TANGRA_CORE, CallingConvention = CallingConvention.Cdecl)]
	//void AdvDefineImageSection(unsigned short width, unsigned short height, unsigned char dataBpp);
	public static extern void AdvDefineImageSection(ushort width, ushort height, byte dataBpp);

	[DllImport(LIBRARY_TANGRA_CORE, CallingConvention = CallingConvention.Cdecl)]
	//void AdvDefineImageLayout(unsigned char layoutId, const char* layoutType, const char* compression, unsigned char layoutBpp, int keyFrame, const char* diffCorrFromBaseFrame);
	public static extern void AdvDefineImageLayout(byte layoutId, string layoutType, string compression, byte layoutBpp, int keyFrame, string diffCorrFromBaseFrame);

	[DllImport(LIBRARY_TANGRA_CORE, CallingConvention = CallingConvention.Cdecl)]
	//unsigned int AdvDefineStatusSectionTag(const char* tagName, int tagType);
	public static extern uint AdvDefineStatusSectionTag(string tagName, AdvTagType tagType);

	[DllImport(LIBRARY_TANGRA_CORE, CallingConvention = CallingConvention.Cdecl)]
	//unsigned int AdvAddFileTag(const char* tagName, const char* tagValue);
	public static extern uint AdvAddFileTag(string tagName, string tagValue);

	[DllImport(LIBRARY_TANGRA_CORE, CallingConvention = CallingConvention.Cdecl)]
	//void AdvAddOrUpdateImageSectionTag(const char* tagName, const char* tagValue);
	public static extern uint AdvAddOrUpdateImageSectionTag(string tagName, string tagValue);

	[DllImport(LIBRARY_TANGRA_CORE, CallingConvention = CallingConvention.Cdecl)]
	//bool AdvBeginFrame(long long timeStamp, unsigned int elapsedTime, unsigned int exposure);
	public static extern bool AdvBeginFrame(long timeStamp, uint elapsedTime, uint exposure);

	[DllImport(LIBRARY_TANGRA_CORE, CallingConvention = CallingConvention.Cdecl)]
	//void AdvFrameAddImage(unsigned char layoutId, unsigned short* pixels, unsigned char pixelsBpp);
	public static extern void AdvFrameAddImage(byte layoutId, [In, MarshalAs(UnmanagedType.LPArray)] ushort[] pixels, byte pixelsBpp);

	[DllImport(LIBRARY_TANGRA_CORE, CallingConvention = CallingConvention.Cdecl)]
	//void AdvFrameAddStatusTag(unsigned int tagIndex, const char* tagValue);
	public static extern void AdvFrameAddStatusTag(uint tagIndex, string tagValue);

	[DllImport(LIBRARY_TANGRA_CORE, CallingConvention = CallingConvention.Cdecl)]
	//void AdvFrameAddStatusTagMessage(unsigned int tagIndex, const char* tagValue);
	public static extern void AdvFrameAddStatusTagMessage(uint tagIndex, string tagValue);

	[DllImport(LIBRARY_TANGRA_CORE, CallingConvention = CallingConvention.Cdecl)]
	//void AdvFrameAddStatusTagUInt8(unsigned int tagIndex, unsigned char tagValue);
	public static extern void AdvFrameAddStatusTagUInt8(uint tagIndex, byte tagValue);

	[DllImport(LIBRARY_TANGRA_CORE, CallingConvention = CallingConvention.Cdecl)]
	//void AdvFrameAddStatusTag16(unsigned int tagIndex, unsigned short tagValue);
	public static extern void AdvFrameAddStatusTag16(uint tagIndex, ushort tagValue);

	[DllImport(LIBRARY_TANGRA_CORE, CallingConvention = CallingConvention.Cdecl)]
	//void AdvFrameAddStatusTagReal(unsigned int tagIndex, float tagValue);
	public static extern void AdvFrameAddStatusTagReal(uint tagIndex, float tagValue);

	[DllImport(LIBRARY_TANGRA_CORE, CallingConvention = CallingConvention.Cdecl)]
	//void AdvFrameAddStatusTag64(unsigned int tagIndex, long long tagValue);
	public static extern void AdvFrameAddStatusTag64(uint tagIndex, long tagValue);		

	[DllImport(LIBRARY_TANGRA_CORE, CallingConvention = CallingConvention.Cdecl)]
	//void AdvEndFrame();
	public static extern void AdvEndFrame();
}
