using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using Tangra.Model.Image;

namespace Tangra.Video.AstroDigitalVideo
{
    public class AdvImageSection : IAdvDataSection
    {
		public enum ImageByteOrder
		{
			BigEndian,
			LittleEndian
		}

        public readonly uint Width;
        public readonly uint Height;
        public readonly byte BitsPerPixel;

    	public ImageByteOrder ByteOrder;

    	internal Dictionary<string, string> ImageSerializationProperties = new Dictionary<string, string>();
    	internal Dictionary<byte, AdvImageLayout> ImageLayouts = new Dictionary<byte, AdvImageLayout>();
		
        public AdvImageSection(ushort width, ushort height, byte bpp)
        {
            Width = width;
            Height = height;
            BitsPerPixel = bpp;
        }

        public string SectionType
        {
            get { return AdvSectionTypes.SECTION_IMAGE; }
        }        

        public AdvImageSection(BinaryReader reader)
        {
            byte version = reader.ReadByte();

            if (version >= 1)
            {
                Width = reader.ReadUInt32();
                Height = reader.ReadUInt32();
                BitsPerPixel = reader.ReadByte();

				byte lyCnt = reader.ReadByte();
				for (int i = 0; i < lyCnt; i++)
				{
					byte layoutId = reader.ReadByte();
					var layout = new AdvImageLayout(this, layoutId, Width, Height, reader);
					ImageLayouts.Add(layoutId, layout);
				}

            	byte propCnt = reader.ReadByte();
				for (int i = 0; i < propCnt; i++)
            	{
					string propName = reader.ReadAsciiString256();
					string propValue = reader.ReadAsciiString256();
					
            		ImageSerializationProperties.Add(propName, propValue);
            	}
				
				InitSerializationProperties();
            }
        }

		private void InitSerializationProperties()
		{
			ByteOrder = ImageByteOrder.LittleEndian;
			string propVal;
			if (ImageSerializationProperties.TryGetValue(AdvKeywords.KEY_IMAGE_BYTE_ORDER, out propVal))
			{
				if (propVal == "BIG-ENDIAN") ByteOrder = ImageByteOrder.BigEndian;
			}
		}

		public object GetDataFromDataBytes(byte[] bytes, ushort[,] prevImageData, int size, int startIndex)
		{
			byte layoutId = bytes[startIndex];
			byte byteMode = bytes[startIndex + 1];
			startIndex+=2;
			size-=2;

			AdvImageData rv = (AdvImageData)ImageLayouts[layoutId].GetDataFromDataBytes(bytes, prevImageData, (AdvImageLayout.GetByteMode)byteMode, size, startIndex);
			rv.LayoutId = layoutId;
			rv.ByteMode = (AdvImageLayout.GetByteMode)byteMode;
			rv.DataBlocksBytesCount = size;

			return rv;
		}

		public AdvImageLayout GetImageLayoutFromLayoutId(byte layoutId)
		{
			return ImageLayouts[layoutId];
		}

    	public enum GetPixelMode
		{
			Raw8Bit,
			StretchTo12Bit, 
			StretchTo16Bit,
		}

		public static ushort[,] GetPixelArray(Bitmap b)
		{
			return GetPixelArray(b, GetPixelMode.Raw8Bit);
		}

    	public static ushort[,] GetPixelArray(Bitmap b, GetPixelMode mode)
    	{
    		bool stretch = mode != GetPixelMode.Raw8Bit;
    		uint maxval = 256;
			if (stretch)
			{
				if (mode == GetPixelMode.StretchTo12Bit)
					maxval = 4096;
				else if (mode == GetPixelMode.StretchTo16Bit)
					maxval = ushort.MaxValue;
			}

    		Random rnd = new Random((int)DateTime.Now.Ticks);

            ushort[,] rv = new ushort[b.Width, b.Height];

            // GDI+ still lies to us - the return format is BGR, NOT RGB.
            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            try
            {
                int stride = bmData.Stride;
                System.IntPtr Scan0 = bmData.Scan0;

                unsafe
                {
                    byte* p = (byte*)(void*)Scan0;

                    int nOffset = stride - b.Width * 3;

                    for (int y = 0; y < b.Height; ++y)
                    {
                        for (int x = 0; x < b.Width; ++x)
                        {
                            ushort red = p[2];
							if (stretch)
							{
								uint stretchedVal = Math.Max(0, Math.Min(maxval, (uint)(red * maxval / 256.0 + (2 * (rnd.NextDouble() - 0.5) * maxval / 256.0))));
								red = (ushort) stretchedVal;
							}

                            rv[x, y] = red;

                            p += 3;
                        }

                        p += nOffset;
                    }
                }
            }
            finally
            {
                b.UnlockBits(bmData);
            }

            return rv;
		}

		public Bitmap CreateBitmapFromPixels(ushort[,] pixels)
		{
			Bitmap b = new Bitmap((int)Width, (int)Height);

			// GDI+ still lies to us - the return format is BGR, NOT RGB.
			BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

			int stride = bmData.Stride;
			System.IntPtr Scan0 = bmData.Scan0;

			unsafe
			{
				byte* p = (byte*)(void*)Scan0;

				int nOffset = stride - b.Width * 3;

				for (int y = 0; y < b.Height; ++y)
				{
					for (int x = 0; x < b.Width; ++x)
					{
						//byte red = (byte)(pixels[x, y] & 0xFF);
						byte red = (byte)(pixels[x, y] / 0xFF);

						p[0] = red;
						p[1] = red;
						p[2] = red;

						p += 3;
					}

					p += nOffset;
				}
			}

			b.UnlockBits(bmData);

			return b;			
		}

		public Pixelmap CreatePixelmap(AdvImageData imageData)
		{
			uint[] pixels = new uint[Width * Height];
			byte[] displayBitmapBytes = new byte[Width * Height];
			for (int y = 0; y < Height; y++)
			{
				for (int x = 0; x < Width; x++)
				{
					pixels[x + y * Width] = imageData.ImageData[x, y];
					if (BitsPerPixel == 12)
						displayBitmapBytes[x + y * Width] = (byte)((imageData.ImageData[x, y] >> 4));
					if (BitsPerPixel == 14)
						displayBitmapBytes[x + y * Width] = (byte)((imageData.ImageData[x, y] >> 6));
					else if (BitsPerPixel == 16)
						displayBitmapBytes[x + y * Width] = (byte)((imageData.ImageData[x, y] >> 8));
					else
						displayBitmapBytes[x + y * Width] = (byte)((imageData.ImageData[x, y] >> (BitsPerPixel - 8)));
				}	
			}

			Bitmap displayBitmap = Pixelmap.ConstructBitmapFromBitmapPixels(displayBitmapBytes, (int) Width, (int) Height);

            Pixelmap rv = new Pixelmap((int)Width, (int)Height, BitsPerPixel, pixels, displayBitmap, displayBitmapBytes);

			return rv;
		}
    }

    public class AdvImageData
    {
        public DateTime MidExposureUtc;
        public float ExposureMilliseconds;

        public ushort[,] ImageData;
    	public ushort Median;
    	public bool CRCOkay;

    	public byte Bpp;

    	public int LayoutId;
    	public AdvImageLayout.GetByteMode ByteMode;
    	public int DataBlocksBytesCount;
    }
}
