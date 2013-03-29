using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Tangra.Video.AstroDigitalVideo
{
    public interface IAdvDataSection
    {
        string SectionType { get; }
		object GetDataFromDataBytes(byte[] bytes, ushort[,] prevImageData, int size, int startIndex);
    }

	public interface IAdvDataSectionWriter
	{
		void WriteSectionHeader(BinaryWriter writer);
	}

    public static class Extensions
    {
        public static void WriteAsciiString256(this BinaryWriter writer, string strData)
        {
            if (strData == null)
            {
                writer.Write((byte)0);
            }
            else
            {
                writer.Write((byte)strData.Length);
                writer.Write(Encoding.ASCII.GetBytes(strData));                
            }
        }

        public static string ReadAsciiString256(this BinaryReader reader)
        {
            byte len = reader.ReadByte();
            if (len == 0)
                return string.Empty;
            else
            {
                byte[] bytes = reader.ReadBytes(len);
                return Encoding.ASCII.GetString(bytes);                
            }
        }
    }

    public enum DiffCorrFrameMode
    {
        KeyFrame = 0,
        PrevFrame = 1
    }
}
