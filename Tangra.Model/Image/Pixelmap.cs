using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Tangra.Model.Image
{
	public class Pixelmap : IDisposable
	{
		private int m_Width;
		private int m_Height;
		private int m_BitPix;
		private uint[] m_Pixels;
		private Bitmap m_Bitmap;

		public FrameStateData FrameState;

		public Pixelmap(int width, int height, int bitPix, uint[] pixels, Bitmap bmp)
		{
			m_Width = width;
			m_Height = height;
			m_BitPix = bitPix;
			m_Pixels = pixels;
			m_Bitmap = bmp;
		}

		public Bitmap DisplayBitmap
		{
			get { return m_Bitmap; }
		}

		public void Dispose()
		{
			if (m_Bitmap != null)
			{
				m_Bitmap.Dispose();
				m_Bitmap = null;
			}
		}
	}
}
