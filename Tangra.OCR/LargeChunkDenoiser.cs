using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tangra.OCR
{
	public class LargeChunkDenoiser
	{
		private const int UNCHECKED = 0;
		private const int WENT_UP = 1;
		private const int WENT_LEFT = 2;
		private const int WENT_RIGHT = 3;
		private const int WENT_DOWN = 4;
		private const int CHECKED = 5;

		internal class Context
		{
			public uint[] Pixels;
			public int[] CheckedPixels;
			public int Index;
			public int Width;
			public int Height;
			public int MaxIndex;
			public uint ColorOn;
			public int[] ObjectPixelsIndex;
			public int ObjectPixelsCount;
			public Stack<int> ObjectPixelsPath;
		}

		public static void Process(uint[] pixels, int width, int height, uint onColour, uint offColour)
		{
            // The max noise chink part to be removed is 50% of the pixels in "1".
            // This value is determined experimentally and varied based on the area hight
            // Block(22x16), AreaHeight(20) -> '1' is 70 pixels (50% = 35 pixels)
            int maxNoiseChunkPixels = (int)Math.Round(35.0 * height / 20);

			var context = new Context()
			{
				Pixels = pixels,
				CheckedPixels = new int[width * height],
				ObjectPixelsIndex = new int[width * height],
				ObjectPixelsCount = -1,
				Index = -1,
				Width = width,
				Height = height,
				MaxIndex = width * height,
				ColorOn = onColour,
				ObjectPixelsPath = new Stack<int>()
			};

			do
			{
				int nextObjectPixelId = FindNextObjectPixel(context);

				if (nextObjectPixelId != -1)
					CheckAndRemoveNoiseObjectAsNecessary(context, nextObjectPixelId, maxNoiseChunkPixels, offColour);
				else
					break;

			} 
			while (true);
		}

		private static int FindNextObjectPixel(Context context)
		{
			while (context.Index < context.MaxIndex - 1)
			{
				context.Index++;
				if (context.CheckedPixels[context.Index] == UNCHECKED)
				{
					if (context.Pixels[context.Index] != context.ColorOn)
						context.CheckedPixels[context.Index] = CHECKED;
					else
						return context.Index;
				}
			}

			return -1;
		}

		private static void CheckAndRemoveNoiseObjectAsNecessary(Context context, int firstPixel, int maxNoiseChunkPixels, uint offColour)
		{
			context.ObjectPixelsCount = 0;
			context.ObjectPixelsPath.Clear();
			context.ObjectPixelsPath.Push(firstPixel);

			int currPixel = firstPixel;

            context.ObjectPixelsIndex[context.ObjectPixelsCount] = firstPixel;
            context.ObjectPixelsCount++;

			while (ProcessNoiseObjectPixel(context, ref currPixel))
			{ }
	
			if (context.ObjectPixelsCount < maxNoiseChunkPixels)
			{
				for (int i = 0; i < context.ObjectPixelsCount; i++)
				{
					context.Pixels[context.ObjectPixelsIndex[i]] = offColour;
				}
			}
		}

		private static bool ProcessNoiseObjectPixel(Context context, ref int pixel)
		{
			int x = pixel % context.Width;
			int y = pixel / context.Width;
			int width = context.Width;
			int nextPixel;

			if (context.CheckedPixels[pixel] == UNCHECKED)
			{
				nextPixel = (y - 1) * width + x;

				context.CheckedPixels[pixel] = WENT_UP;

				if (y > 0)
				{
					if (context.CheckedPixels[nextPixel] == UNCHECKED)
					{
					    if (context.Pixels[nextPixel] == context.ColorOn)
					    {
					        context.ObjectPixelsPath.Push(nextPixel);
					        pixel = nextPixel;
					        context.ObjectPixelsIndex[context.ObjectPixelsCount] = nextPixel;
					        context.ObjectPixelsCount++;
					    }
					    else
					        context.CheckedPixels[nextPixel] = CHECKED;
					}
				}
				
				return true;
			}
			else if (context.CheckedPixels[pixel] == WENT_UP)
			{
				nextPixel = y * width + (x - 1);

				context.CheckedPixels[pixel] = WENT_LEFT;

				if (x > 0)
				{
					if (context.CheckedPixels[nextPixel] == UNCHECKED)
					{
					    if (context.Pixels[nextPixel] == context.ColorOn)
					    {
					        context.ObjectPixelsPath.Push(nextPixel);
					        pixel = nextPixel;
					        context.ObjectPixelsIndex[context.ObjectPixelsCount] = nextPixel;
					        context.ObjectPixelsCount++;
					    }
					    else
					        context.CheckedPixels[nextPixel] = CHECKED;
					}
				}
				
				return true;
			}
			else if (context.CheckedPixels[pixel] == WENT_LEFT)
			{
				nextPixel = y * width + (x + 1);

				context.CheckedPixels[pixel] = WENT_RIGHT;

				if (x < context.Width - 1)
				{
					if (context.CheckedPixels[nextPixel] == UNCHECKED)
					{
					    if (context.Pixels[nextPixel] == context.ColorOn)
					    {
						    context.ObjectPixelsPath.Push(nextPixel);
						    pixel = nextPixel;
						    context.ObjectPixelsIndex[context.ObjectPixelsCount] = nextPixel;
                            context.ObjectPixelsCount++;
					    }
					    else
						    context.CheckedPixels[nextPixel] = CHECKED;
					}
				}
			
				return true;
			}
			else if (context.CheckedPixels[pixel] == WENT_RIGHT)
			{
				nextPixel = (y + 1) * width + x;

				context.CheckedPixels[pixel] = WENT_DOWN;

				if (y < context.Height - 1)
				{
					if (context.CheckedPixels[nextPixel] == UNCHECKED)
                    {
                        if (context.Pixels[nextPixel] == context.ColorOn)
                        {
                            context.ObjectPixelsPath.Push(nextPixel);
                            pixel = nextPixel;
                            context.ObjectPixelsIndex[context.ObjectPixelsCount] = nextPixel;
                            context.ObjectPixelsCount++;
                        }
                        else
                            context.CheckedPixels[nextPixel] = CHECKED;
                    }
				}

				return true;
			}
			else if (context.CheckedPixels[pixel] >= WENT_DOWN)
			{
				if (context.ObjectPixelsPath.Count == 0)
					return false;

				context.CheckedPixels[pixel] = CHECKED;

                nextPixel = context.ObjectPixelsPath.Pop();

                if (pixel == nextPixel && context.ObjectPixelsPath.Count > 0)
				    nextPixel = context.ObjectPixelsPath.Pop();

				pixel = nextPixel;
				return true;
			}
			else
				return false;
		}
	}
}
