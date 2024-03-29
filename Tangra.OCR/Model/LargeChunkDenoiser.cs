﻿/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;

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
			public int ObjectPixelsYFrom;
			public int ObjectPixelsYTo;
			public int ObjectPixelsXFrom;
			public int ObjectPixelsXTo;
			public Stack<int> ObjectPixelsPath;

		    public IDenoiser Denoiser;
		}

	    internal interface IDenoiser
	    {
	        bool IdentifyNoise(Context context);
	    }

        internal class LargeChunkDenoiseContext : IDenoiser
	    {
            public int MaxLowerBoundNoiseChunkPixels;
            public int MinUpperBoundNoiseChunkPixels;
            public int MinLowerBoundNoiseChunkHeight;
            public int MaxUpperBoundNoiseChunkWidth;

            public bool IdentifyNoise(Context context)
            {
                return
                    context.ObjectPixelsCount < MaxLowerBoundNoiseChunkPixels ||
                    context.ObjectPixelsCount > MinUpperBoundNoiseChunkPixels ||
                    (context.ObjectPixelsYTo - context.ObjectPixelsYFrom) < MinLowerBoundNoiseChunkHeight ||
                    (context.ObjectPixelsXTo - context.ObjectPixelsXFrom) > MaxUpperBoundNoiseChunkWidth;
            }
        }

	    internal class SmallHeightDenoiseContext : IDenoiser
	    {
	        private int m_MaxNoiseChunkHeight;
            private int m_MinUpperBoundNoiseChunkPixels;
	        public int MaxNoiseChunkHeight
	        {
	            get { return m_MaxNoiseChunkHeight; }
	            set
	            {
	                m_MaxNoiseChunkHeight = value;
                    m_MinUpperBoundNoiseChunkPixels = 6 * value * value;
	            }
	        }

	        public bool IdentifyNoise(Context context)
	        {
                return 
                    (context.ObjectPixelsYTo - context.ObjectPixelsYFrom) <= m_MaxNoiseChunkHeight ||
                    context.ObjectPixelsCount > m_MinUpperBoundNoiseChunkPixels;
	        }
	    }

	    public static void Process(bool useNative, uint[] pixels, int width, int height)
        {
            if (useNative)
                TangraCore.LargeChunkDenoise(pixels, width, height);
            else
                ProcessManaged(pixels, width, height, 0, 255);
        }


	    private static void ProcessManaged(uint[] pixels, int width, int height, uint onColour, uint offColour)
	    {
            var denoiser = new LargeChunkDenoiseContext();
            denoiser.MaxLowerBoundNoiseChunkPixels = (int)Math.Round(35.0 * height / 20);
            denoiser.MinUpperBoundNoiseChunkPixels = 6 * denoiser.MaxLowerBoundNoiseChunkPixels;
            denoiser.MinLowerBoundNoiseChunkHeight = (int)Math.Round(0.5 * (height - 4));
            denoiser.MaxUpperBoundNoiseChunkWidth = height + 4;
		    
            RunManagedDenoiser(pixels, width, height, onColour, offColour, denoiser);
	    }

        public static void RemoveSmallHeightNoise(uint[] pixels, int width, int height, int maxNoiseHeight, uint onColour = 0, uint offColour = 255)
        {
            var denoiser = new SmallHeightDenoiseContext();
            denoiser.MaxNoiseChunkHeight = maxNoiseHeight;

            RunManagedDenoiser(pixels, width, height, onColour, offColour, denoiser);
        }

        private static void RunManagedDenoiser(uint[] pixels, int width, int height, uint onColour, uint offColour, IDenoiser denoiser)
		{
			var context = new Context()
			{
				Pixels = pixels,
				CheckedPixels = new int[width * height],
				ObjectPixelsIndex = new int[width * height],
				ObjectPixelsCount = -1,
				ObjectPixelsYFrom = int.MaxValue,
				ObjectPixelsYTo = int.MinValue,
				ObjectPixelsXFrom = int.MaxValue,
				ObjectPixelsXTo = int.MinValue,
				Index = -1,
				Width = width,
				Height = height,
				MaxIndex = width * height,
				ColorOn = onColour,
				ObjectPixelsPath = new Stack<int>()
			};

		    context.Denoiser = denoiser;

			do
			{
				int nextObjectPixelId = FindNextObjectPixel(context);

				if (nextObjectPixelId != -1)
					CheckAndRemoveNoiseObjectAsNecessary(context, nextObjectPixelId, offColour);
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

		private static void SetObjectPixelStats(Context context, int pixelIndex)
		{
			int x = pixelIndex % context.Width;
			int y = pixelIndex / context.Width;
			if (context.ObjectPixelsYFrom > y) context.ObjectPixelsYFrom = y;
			if (context.ObjectPixelsYTo < y) context.ObjectPixelsYTo = y;
			if (context.ObjectPixelsXFrom > x) context.ObjectPixelsXFrom = x;
			if (context.ObjectPixelsXTo < x) context.ObjectPixelsXTo = x;
		}

		private static void CheckAndRemoveNoiseObjectAsNecessary(Context context, int firstPixel, uint offColour)
		{
			context.ObjectPixelsCount = 0;
			context.ObjectPixelsYFrom = int.MaxValue;
			context.ObjectPixelsYTo = int.MinValue;
			context.ObjectPixelsXFrom = int.MaxValue;
			context.ObjectPixelsXTo = int.MinValue;
			context.ObjectPixelsPath.Clear();
			context.ObjectPixelsPath.Push(firstPixel);

			int currPixel = firstPixel;

            context.ObjectPixelsIndex[context.ObjectPixelsCount] = firstPixel;
            context.ObjectPixelsCount++;
			SetObjectPixelStats(context, firstPixel);

			while (ProcessNoiseObjectPixel(context, ref currPixel))
			{ }
	
			if (context.Denoiser.IdentifyNoise(context))
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
							SetObjectPixelStats(context, pixel);
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
							SetObjectPixelStats(context, pixel);
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
							SetObjectPixelStats(context, pixel);
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
							SetObjectPixelStats(context, pixel);
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
