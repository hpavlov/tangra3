using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tangra.Model.Image;

namespace OcrTester
{
	public class OsdProcessor
	{
		private const int MAX_POSITIONS = 29;

		private int m_Width;
		private int m_Height;

		private int m_MinBlockWidth;
		private int m_MaxBlockWidth;
		private int m_MinBlockHeight;
		private int m_MaxBlockHeight;

		private int m_BlockWidth;
		private int m_BlockHeight;
		private int m_BlockOffsetX;
		private int m_BlockOffsetY;

		public void Initialise(int width, int height)
		{
			m_Width = width;
			m_Height = height;

			m_MaxBlockWidth = width / 27;
			m_MinBlockWidth = width / 29;
			m_MinBlockHeight = (int)Math.Round(0.66 * height);
			m_MaxBlockHeight = height - 2;
		}

		private int RateBlock(Pixelmap image, Rectangle block)
		{
			int rv = 0;
			bool leftOk = block.Left < image.Width;
			bool left2Ok = (block.Left + 1) < image.Width;
			bool bottomOk = block.Bottom < image.Height;
			bool bottom2Ok = (block.Bottom + 1) < image.Height;
			bool topOk = block.Top - 1 >= 0;

			for (int y = block.Top; y < block.Bottom; y++)
			{
				if (y >= image.Height) continue;
				if (leftOk && image[block.Left, y] > 127) rv++;
				if (left2Ok && image[block.Left + 1, y] < 127) rv++;
			}

			for (int x = block.Left; x < block.Right; x++)
			{
				if (x >= image.Height) continue;
				if (bottomOk && image[x, block.Bottom] < 127) rv++;
				if (bottom2Ok && image[x, block.Bottom + 1] > 127) rv++;
				if (topOk && image[x, block.Top - 1] > 127) rv++;
				if (image[x, block.Top] < 127) rv++;
			}



			return rv;
		}

		public void Process(Pixelmap image, Graphics g)
		{
			int count = 0;
			int maxRating = -1;
			
			int bestXOffs = -1;
			int bestYOffs = -1;
			int bestWidth = -1;
			int bestHeight = -1;

			Stopwatch sw = new Stopwatch();
			sw.Start();
			for (int xOffs = 0; xOffs < 8; xOffs++)
			{
				for (int yOffs = 0; yOffs < 4; yOffs++)
				{
					for (int x = m_MinBlockWidth; x <= m_MaxBlockWidth; x++)
					{
						for (int y = m_MinBlockHeight; y <= m_MaxBlockHeight; y++)
						{
							int totalRating = 0;
							Rectangle rect;

							for (int i = 0; i < MAX_POSITIONS; i++)
							{
								rect = new Rectangle(xOffs + i * x, yOffs, x, y);
								totalRating += RateBlock(image, rect);
							}

							if (totalRating > maxRating)
							{
								maxRating = totalRating;
								bestXOffs = xOffs;
								bestYOffs = yOffs;
								bestWidth = x;
								bestHeight = y;
							}

							count++;
						}
					}
				}
			}
			sw.Stop();
			double totalMillisec = sw.ElapsedMilliseconds;

			Trace.WriteLine(string.Format("{0}", totalMillisec));

			//g.DrawRectangle(Pens.BlueViolet, 1, 1, m_MinBlockWidth, m_MinBlockHeight);
			for (int i = 0; i < MAX_POSITIONS; i++)
			{
				g.DrawRectangle(Pens.Chartreuse, i * bestWidth + bestXOffs, bestYOffs, bestWidth, bestHeight);
			}

			m_BlockWidth = bestWidth;
			m_BlockHeight = bestHeight;
			m_BlockOffsetX = bestXOffs;
			m_BlockOffsetY = bestYOffs;
		}

		public string GetBlockAt(int x, int y)
		{
			if (y >= m_BlockOffsetY && y <= m_BlockOffsetY + m_BlockHeight)
			{
				for (int i = 0; i < MAX_POSITIONS; i++)
				{
					if (x >= m_BlockOffsetX + i * m_BlockWidth && x < m_BlockOffsetX + (i + 1) * m_BlockWidth)
						return i.ToString()
				}
			}

			return null;
		}
	}
}
