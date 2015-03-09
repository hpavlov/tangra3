/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using Tangra.Model.Astro;
using Tangra.Model.Image;

namespace Tangra.Astrometry.Analysis
{
	public class PeakPixelResolver
	{
		private AstroImage m_Image;

		private uint[,] m_PixelData;
		public uint[,] PixelData
		{
			get { return m_PixelData; }
		}

		private Dictionary<KeyValuePair<int, int>, uint> m_PeakPixels = new Dictionary<KeyValuePair<int, int>, uint>();
		public Dictionary<KeyValuePair<int, int>, uint> PeakPixels
		{
			get { return m_PeakPixels; }
		}

		internal PeakPixelResolver(AstroImage image)
		{
			m_Image = image;
		}

		public void ResolvePeakPixels(
			Rectangle osdRectangleToExclude, Rectangle rectToInclude, bool limitByInclusion,
			int edgeDistance, int minDistanceBetweenPixels)
		{
				int width = m_Image.Width;
				int height = m_Image.Height;
				
				uint[,] data = BitmapFilter.LowPassFilter(m_Image.Pixelmap.GetPixelsCopy(), m_Image.Pixelmap.BitPixCamera, false);

				List<uint> allPixels = new List<uint>();
				for (int y = edgeDistance; y < height - edgeDistance; ++y)
				{
					for (int x = edgeDistance; x < width - edgeDistance; ++x)
					{
						if (limitByInclusion && !rectToInclude.Contains(x, y)) continue;
						if (!limitByInclusion && osdRectangleToExclude.Contains(x, y)) continue;

						allPixels.Add(data[x, y]);
					}
				}
				double mode = BitmapFilter.GetMode(allPixels, 6);

				if (minDistanceBetweenPixels % 2 == 0) minDistanceBetweenPixels++;
				int halfDistance = (minDistanceBetweenPixels / 2) + 1;

				// TODO: Use 2x2 or 3x3 binning to reduce the number of peak pixels found

				for (int y = edgeDistance; y < height - edgeDistance; y += minDistanceBetweenPixels)
				{
					for (int x = edgeDistance; x < width - edgeDistance; x+= minDistanceBetweenPixels)
					{
						int maxX = int.MinValue;
						int maxY = int.MinValue;
						uint maxVal = 0;

						for (int i = 0; i < minDistanceBetweenPixels; i++)
						{
							for (int j = 0; j < minDistanceBetweenPixels; j++)
							{
								int xx = x + i;
								int yy = y + j;

								if (limitByInclusion && !rectToInclude.Contains(xx, yy)) continue;
								if (!limitByInclusion && osdRectangleToExclude.Contains(xx, yy)) continue;

								if (data[xx, yy] < mode) continue;

								if (data[xx, yy] > maxVal)
								{
									maxVal = data[xx, yy];
									maxX = xx;
									maxY = yy;
								}
							}
						}

						if (maxX != int.MinValue)
						{
							uint pix = data[maxX, maxY];
							if (pix >= data[maxX - 1, maxY - 1] &&
								pix >= data[maxX - 1, maxY] &&
								pix >= data[maxX - 1, maxY + 1] &&
								pix >= data[maxX, maxY - 1] &&
								pix >= data[maxX, maxY + 1] &&
								pix >= data[maxX + 1, maxY - 1] &&
								pix >= data[maxX + 1, maxY] &&
								pix >= data[maxX + 1, maxY + 1])
							{
								// peak Pixel found
								m_PeakPixels.Add(new KeyValuePair<int, int>(maxX, maxY), pix);
							}							
						}
					}
				}

				m_PixelData = data;
		}
	}
}
