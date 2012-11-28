using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace Tangra.Video.AstroDigitalVideo
{
	public class AdvImageLayout
	{
		public readonly byte BitsPerPixel;
		public readonly byte LayoutId;
		public readonly uint Width;
		public readonly uint Height;

		private int m_KeyFrame;
		private bool m_IsRawDataLayout;
		private bool m_IsDiffCorrLayout;
		private DiffCorrFrameMode m_DiffCorrFrame;
		private string m_Compression;
		private bool m_UsesCompression;
		private bool m_UsesCRC;

		private AdvImageSection m_ImageSection;

		internal Dictionary<string, string> ImageSerializationProperties = new Dictionary<string, string>();

		static AdvImageLayout()
		{
			crc32_init();
		}

		public bool IsDiffCorrLayout
		{
			get { return m_IsDiffCorrLayout; }
		}

		public AdvImageLayout(AdvImageSection imageSection, byte layoutId, uint width, uint height, BinaryReader reader)
		{
			m_ImageSection = imageSection;
			LayoutId = layoutId;
			Width = width;
			Height = height;

			byte version = reader.ReadByte();

			if (version >= 1)
			{				
				BitsPerPixel = reader.ReadByte();

				byte propCnt = reader.ReadByte();
				for (int i = 0; i < propCnt; i++)
				{
					string propName = reader.ReadAsciiString256();
					string propValue = reader.ReadAsciiString256();

					ImageSerializationProperties.Add(propName, propValue);
				}
			}

			InitSerializationProperties();			
		}

		private void InitSerializationProperties()
		{
			string propVal;
			if (ImageSerializationProperties.TryGetValue(AdvKeywords.KEY_DIFFCORR_KEY_FRAME_FREQUENCY, out propVal))
				m_KeyFrame = int.Parse(propVal);

			string dataLayout = ImageSerializationProperties[AdvKeywords.KEY_DATA_LAYOUT];

			Trace.Assert(dataLayout == DataLayouts.FULL_IMAGE_DIFFERENTIAL_CODING || dataLayout == DataLayouts.FULL_IMAGE_RAW);
			if (dataLayout == DataLayouts.FULL_IMAGE_RAW)
			{
				m_IsRawDataLayout = true;
				m_IsDiffCorrLayout = false;
			}
			if (dataLayout == DataLayouts.FULL_IMAGE_DIFFERENTIAL_CODING)
			{
				m_IsRawDataLayout = false;
				m_IsDiffCorrLayout = true;
			}


			m_DiffCorrFrame = DiffCorrFrameMode.KeyFrame;
			string baseFrame;
			if (ImageSerializationProperties.TryGetValue("DIFFCODE-BASE-FRAME", out baseFrame))
			{
				if ("KEY-FRAME" == baseFrame)
					m_DiffCorrFrame = DiffCorrFrameMode.KeyFrame;
				else if ("PREV-FRAME" == baseFrame)
					m_DiffCorrFrame = DiffCorrFrameMode.PrevFrame;
			}

			m_UsesCompression = false;
			ImageSerializationProperties.TryGetValue("SECTION-DATA-COMPRESSION", out m_Compression);
			m_UsesCompression = !string.IsNullOrEmpty(m_Compression) && m_Compression != AdvCompressionMethods.COMPR_UNCOMPRESSED;

			m_UsesCRC = false;
			string crcType;
			ImageSerializationProperties.TryGetValue("SECTION-DATA-REDUNDANCY-CHECK", out crcType);
			m_UsesCRC = !string.IsNullOrEmpty(crcType) && crcType == AdvRedundancyCheck.CRC32;
		}

		public enum GetByteMode
		{
			Normal,
			KeyFrameBytes,
			DiffCorrBytes
		}

		public DiffCorrFrameMode DiffCorrFrame
		{
			get { return m_DiffCorrFrame; }
		}

		public object GetDataFromDataBytes(byte[] dataBytes, ushort[,] prevImageData, GetByteMode byteMode, int size, int startIndex)
		{
			byte[] bytes;
			int readIndex = 0;
			if (!m_UsesCompression)
            {
				bytes = dataBytes;
				readIndex = startIndex;
            }
			else if (m_Compression == AdvCompressionMethods.COMPR_DIFF_CORR_QUICKLZ)
			{
				byte[] compressedBytes = new byte[size];
				Array.Copy(dataBytes, startIndex, compressedBytes, 0, size);

				readIndex = 0;
				bytes = QuickLZ.decompress(compressedBytes);
            }			
            else
                throw new NotSupportedException(string.Format("Don't know how to apply compression '{0}'", m_Compression));

				
			ushort[,] imageData;
			bool crcOkay;
			if (BitsPerPixel == 12)
				imageData = GetPixelsFrom12BitByteArray(bytes, prevImageData, byteMode, ref readIndex, out crcOkay);
			else if (BitsPerPixel == 16)
			{
				if (m_IsRawDataLayout)
					imageData = GetPixelsFrom16BitByteArrayRawLayout(bytes, prevImageData, ref readIndex, out crcOkay);
				else
					imageData = GetPixelsFrom16BitByteArrayDiffCorrLayout(bytes, prevImageData, ref readIndex, out crcOkay);
			}
			else
				throw new NotSupportedException();

			return new AdvImageData()
			{
				ImageData = imageData,
				CRCOkay = m_UsesCRC ? crcOkay : true,
				Bpp = m_ImageSection.BitsPerPixel
			};
		}

		private ushort[,] GetPixelsFrom12BitByteArray(byte[] bytes, ushort[,] prevFramePixels, GetByteMode byteMode, ref int idx, out bool crcOkay)
		{
			var rv = new ushort[Width, Height];

			bool isLittleEndian = m_ImageSection.ByteOrder == AdvImageSection.ImageByteOrder.LittleEndian;
			bool convertTo12Bit = this.m_ImageSection.BitsPerPixel == 12;
			bool convertTo16Bit = this.m_ImageSection.BitsPerPixel == 16;

			bool isKeyFrame = byteMode == GetByteMode.KeyFrameBytes;
			bool keyFrameNotUsed = byteMode == GetByteMode.Normal;
			bool isDiffCorrFrame = byteMode == GetByteMode.DiffCorrBytes;

			int counter = 0;
			for (int y = 0; y < Height; ++y)
			{
				for (int x = 0; x < Width; ++x)
				{
					counter++;
					// Every 2 12-bit values can be encoded in 3 bytes
					// xxxxxxxx|xxxxyyyy|yyyyyyy

					switch (counter % 2)
					{
						case 1:
							byte bt1 = bytes[idx];
							idx++;
							byte bt2 = bytes[idx];

							ushort val = (ushort)(((ushort)bt1 << 4) + ((bt2 >> 4) & 0x0F));
							if (!isLittleEndian)
							{
								val = (ushort)(val << 4);
								val = (ushort)((ushort)((val & 0xFF) << 8) + (ushort)(val >> 8));

								if (convertTo12Bit)
									throw new NotSupportedException();
							}
							else
								if (convertTo16Bit) val = (ushort)(val << 4);

							if (isDiffCorrFrame)
							{
								val = (ushort)((ushort)prevFramePixels[x, y] + (ushort)val);
								if (convertTo12Bit && val > 4095) val -= 4095;
							}

							rv[x, y] = val;
							if (counter < 10 || counter > Height * Width - 10) Trace.WriteLine(string.Format("{0}: {1}", counter, val));
							break;

						case 0:
							bt1 = bytes[idx];
							idx++;
							bt2 = bytes[idx];
							idx++;

							val = (ushort)((((ushort)bt1 & 0x0F) << 8) + bt2);
							if (!isLittleEndian)
							{
								val = (ushort)(val << 4);
								val = (ushort)((ushort)((val & 0xFF) << 8) + (ushort)(val >> 8));

								if (convertTo12Bit) 
									throw new NotSupportedException();
							}
							else
								if (convertTo16Bit) val = (ushort)(val << 4);

							if (isDiffCorrFrame)
							{
								val = (ushort)((ushort)prevFramePixels[x, y] + (ushort)val);
								if (convertTo12Bit && val > 4095) val -= 4095;
							}

							rv[x, y] = val;
							if (counter < 10 || counter > Height * Width - 10) Trace.WriteLine(string.Format("{0}: {1}", counter, val));
							break;
					}
				}
			}

			if (m_UsesCRC)
			{
				uint savedFrameCrc = (uint)(bytes[idx] + (bytes[idx + 1] << 8) + (bytes[idx + 2] << 16) + (bytes[idx + 3] << 24));
				idx += 4;

				uint crc3 = ComputePixelsCRC(rv);

				crcOkay = crc3 == savedFrameCrc;				
			}
			else
				crcOkay = true;

			return rv;

		}

		private ushort[,] GetPixelsFrom16BitByteArrayRawLayout(byte[] bytes, ushort[,] prevFramePixels, ref int idx, out bool crcOkay)
		{
			bool isLittleEndian = m_ImageSection.ByteOrder == AdvImageSection.ImageByteOrder.LittleEndian;

			var rv = new ushort[Width, Height];
			bool convertT012Bit = this.m_ImageSection.BitsPerPixel == 12;
			
			for (int y = 0; y < Height; ++y)
			{
				for (int x = 0; x < Width; ++x)
				{
					byte bt1 = bytes[idx];
					idx++;
					byte bt2 = bytes[idx];
					idx++;

					ushort val;
					if (!isLittleEndian)
						val = (ushort)(((ushort)bt1 << 8) + bt2);
					else
						val = (ushort)(((ushort)bt2 << 8) + bt1);

					if (convertT012Bit) val = (ushort)(val >> 4);

					rv[x, y] = (ushort)val;
				}
			}

			if (m_UsesCRC)
			{
				uint savedFrameCrc = (uint)(bytes[idx] + (bytes[idx + 1] << 8) + (bytes[idx + 2] << 16) + (bytes[idx + 3] << 24));
				idx += 4;

				uint crc3 = ComputePixelsCRC(rv);
				crcOkay = crc3 == savedFrameCrc;
			}
			else
				crcOkay = true;

			return rv;
		}

		private ushort[,] GetPixelsFrom16BitByteArrayDiffCorrLayout(byte[] bytes, ushort[,] prevFramePixels, ref int idx, out bool crcOkay)
		{
			var rv = new ushort[Width, Height];

			byte frameFlag = bytes[idx]; Trace.Assert(frameFlag == 0 || frameFlag == 1 || frameFlag == 2);
			idx++;
			bool isKeyFrame = frameFlag == 1;
			bool keyFrameNotUsed = frameFlag == 0;
			bool isDiffCorrFrame = frameFlag == 2;

			int counter = 0;
			for (int y = 0; y < Height; ++y)
			{
				for (int x = 0; x < Width; ++x)
				{
					counter++;

					byte bt1 = bytes[idx];
					idx++;
					byte bt2 = bytes[idx];
					idx++;

					short val = (short)(((short)bt1 << 8) + bt2);
					if (isDiffCorrFrame)
						rv[x, y] = (ushort)((short)prevFramePixels[x, y] + (short)val);
					else
						rv[x, y] = (ushort)val;
				}
			}

			if (m_UsesCRC)
			{
				uint savedFrameCrc = (uint) (bytes[idx] + (bytes[idx + 1] << 8) + (bytes[idx + 2] << 16) + (bytes[idx + 3] << 24));
				idx += 4;

				uint crc3 = ComputePixelsCRC(rv);
				crcOkay = crc3 == savedFrameCrc;
			}
			else
				crcOkay = true;

			return rv;
		}


		public UInt32 ComputePixelsCRC(ushort[,] pixels)
		{
			int width = pixels.GetLength(0);
			int height = pixels.GetLength(1);
			byte[] pixelBytes = new byte[width * height * 2];

			int idx = 0;
			for (int y = 0; y < height; ++y)
			{
				for (int x = 0; x < width; ++x)
				{
					ushort val = pixels[x, y];
					pixelBytes[idx] = (byte)(val & 0xFF);
					pixelBytes[idx + 1] = (byte)((val >> 8) & 0xFF);
					idx += 2;
				}
			}

			uint result;
			int i;

			result = (uint)(pixelBytes[0] << 24);
			result |= (uint)(pixelBytes[1] << 16);
			result |= (uint)(pixelBytes[2] << 8);
			result |= (uint)(pixelBytes[3]);
			result = ~result;
			int len = pixelBytes.Length;

			for (i = 4; i < len; i++)
			{
				result = (result << 8 | pixelBytes[i]) ^ s_crctab[result >> 24];
			}

			return ~result;
		}

		private static uint[] s_crctab;

		static void crc32_init()
		{
			uint i, j;
			uint crc;

			s_crctab = new uint[256];

			for (i = 0; i < 256; i++)
			{
				crc = i << 24;
				for (j = 0; j < 8; j++)
				{
					if ((crc & 0x80000000) != 0)
						crc = (crc << 1) ^ 0x04c11db7;
					else
						crc = crc << 1;
				}
				s_crctab[i] = crc;
			}
		}
	}
}
