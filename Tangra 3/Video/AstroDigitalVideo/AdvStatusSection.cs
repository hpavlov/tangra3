using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Tangra.Video.AstroDigitalVideo
{
    public enum AdvTagType
    {
		UInt8 = 0,
        UInt16 = 1,
        UInt32 = 2,
		ULong64 = 3,
        Real= 4,
        AnsiString254 = 5,
		List16AnsiString254 = 6,
    }

	[StructLayout(LayoutKind.Explicit)]
	public struct IntToFloatStruct
	{
		[FieldOffset(0)]
		public uint UInt32Value;

		[FieldOffset(0)]
		public float RealValue;
	}

    public class AdvTagDefinition
    {
        public string Name;
        public AdvTagType Type;
    }

    public class AdvStatusSection : IAdvDataSection
    {      
        public AdvStatusSection()
        { }

        public string SectionType
        {
            get { return AdvSectionTypes.SECTION_SYSTEM_STATUS; }
        }

        private List<AdvTagDefinition> m_TagDefinitions = new List<AdvTagDefinition>();

        public void AddTagDefinition(AdvTagDefinition tagDefinition)
        {
            m_TagDefinitions.Add(tagDefinition);
        }

    	public List<AdvTagDefinition> TagDefinitions
    	{
    		get { return m_TagDefinitions; }
    	}

        public AdvStatusSection(BinaryReader reader)
        {
		    byte version = reader.ReadByte();

            if (version >= 1)
            {
                byte count = reader.ReadByte();

                for (int i = 0; i < count; i++)
                {
                    string tagName = reader.ReadAsciiString256();
                    AdvTagType tagType = (AdvTagType)reader.ReadByte();

                    m_TagDefinitions.Add(new AdvTagDefinition() { Name = tagName, Type = tagType} );
                }
            }
        }

		public object GetDataFromDataBytes(byte[] bytes, ushort[,] prevFrame, int size, int startIndex)
		{
			var rv = new AdvStatusData();

			if (size > 0)
			{
				byte tagValuesCount = bytes[startIndex];
				startIndex++;

				for (int i = 0; i < tagValuesCount; i++)
				{
					int tagId = bytes[startIndex];
					AdvTagDefinition tagDef = m_TagDefinitions[tagId];

					if (tagDef.Type == AdvTagType.UInt8)
					{
						byte value = bytes[startIndex + 1];
						rv.TagValues[tagDef] = value.ToString();

						startIndex += 2;						
					}
					else if (tagDef.Type == AdvTagType.UInt16)
					{
						byte b1 = bytes[startIndex + 1];
						byte b2 = bytes[startIndex + 2];
						ushort value = (ushort)((b2 << 8) + b1);
						rv.TagValues[tagDef] = value.ToString();

						startIndex += 3;
					}
					else if (tagDef.Type == AdvTagType.UInt32)
					{
						byte b1 = bytes[startIndex + 1];
						byte b2 = bytes[startIndex + 2];
						byte b3 = bytes[startIndex + 3];
						byte b4 = bytes[startIndex + 4];

						uint value = (uint)((b4 << 24) + (b3 << 16) + (b2 << 8) + b1);
						rv.TagValues[tagDef] = value.ToString();

						startIndex += 5;
					}
					else if (tagDef.Type == AdvTagType.Real)
					{
						byte b1 = bytes[startIndex + 1];
						byte b2 = bytes[startIndex + 2];
						byte b3 = bytes[startIndex + 3];
						byte b4 = bytes[startIndex + 4];

						uint value = (uint)((b4 << 24) + (b3 << 16) + (b2 << 8) + b1);

						IntToFloatStruct converter = new IntToFloatStruct();
						converter.UInt32Value = value;
						float realValue = converter.RealValue;

						rv.TagValues[tagDef] = realValue.ToString();

						startIndex += 5;
					}
					else if (tagDef.Type == AdvTagType.ULong64)
					{
						byte b1 = bytes[startIndex + 1];
						byte b2 = bytes[startIndex + 2];
						byte b3 = bytes[startIndex + 3];
						byte b4 = bytes[startIndex + 4];
						byte b5 = bytes[startIndex + 5];
						byte b6 = bytes[startIndex + 6];
						byte b7 = bytes[startIndex + 7];
						byte b8 = bytes[startIndex + 8];

						uint valLo = (uint)((b4 << 24) + (b3 << 16) + (b2 << 8) + b1);
						uint valHi = (uint)((b8 << 24) + (b7 << 16) + (b6 << 8) + b5);
						long value = (((long)valHi) << 32) + (long)valLo;
						rv.TagValues[tagDef] = value.ToString();

						startIndex += 9;
					}
					else if (tagDef.Type == AdvTagType.AnsiString254)
					{
						byte len = bytes[startIndex + 1];
						byte[] tagValuebytes = new byte[len];
						Array.Copy(bytes, startIndex + 2, tagValuebytes, 0, len);

						rv.TagValues[tagDef] = Encoding.ASCII.GetString(tagValuebytes);

						startIndex += 2 + len;
					}
					else if (tagDef.Type == AdvTagType.List16AnsiString254)
					{
						StringBuilder bld = new StringBuilder();
						byte count = bytes[startIndex + 1];
						startIndex += 2;
						for (int j = 0; j < count; j++)
						{
							byte len = bytes[startIndex];
							byte[] tagValuebytes = new byte[len];
							Array.Copy(bytes, startIndex + 1, tagValuebytes, 0, len);

							bld.AppendLine(Encoding.ASCII.GetString(tagValuebytes));

							startIndex += 1 + len;
						}

						rv.TagValues[tagDef] = bld.ToString();
					}
				}
			}
		
            return rv;
        }
    }

	internal class AdvStatusData
	{
		public Dictionary<AdvTagDefinition, string> TagValues = new Dictionary<AdvTagDefinition, string>();
	}

	internal static class AdvStatusValuesHelper
	{
		public static string TranslateGpsFixStatus(int status)
		{
			if (status == 0)
				return "No Fix";
			else if (status == 1)
				return "G Fix";
			else if (status == 2)
				return "P Fix";
			else
				return "Unknown";
		}

		public static string TranslateGpsAlmanacStatus(int status)
		{
			if (status == 0)
				return "Uncertain";
			else if (status == 1)
				return "Good";
			else
				return "Unknown";
		}

		public static string GetWellKnownGammaForValue(float gamma)
		{
			if (Math.Abs(gamma - 0.500) < 0.005)
				return "(MIN)";
			else if (Math.Abs(gamma - 1.000) < 0.005)
				return "(OFF)";
			else if (Math.Abs(gamma - (1 / 0.45)) < 0.005)
				return "(LO)";
			else if (Math.Abs(gamma - (1 / 0.35)) < 0.005)
				return "(HI)";
			else if (Math.Abs(gamma - 4.000) < 0.005)
				return "(MAX)";
			else
				return string.Empty;
		}
	}
}
