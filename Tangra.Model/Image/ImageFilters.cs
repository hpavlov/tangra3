using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tangra.Model.Config;

namespace Tangra.Model.Image
{
    public static class ImageFilters
    {
        public static uint[,] CutArrayEdges(uint[,] data)
        {
            return CutArrayEdges(data, 1);
        }

        public static uint[,] CutArrayEdges(uint[,] data, int linesToCut)
        {
            int nWidth = data.GetLength(0);
            int nHeight = data.GetLength(1);

            uint[,] cutData = new uint[nWidth - 2 * linesToCut, nHeight - 2 * linesToCut];

            for (int i = linesToCut; i < nWidth - linesToCut; i++)
                for (int j = linesToCut; j < nHeight - linesToCut; j++)
                {
                    cutData[i - linesToCut, j - linesToCut] = data[i, j];
                }

            return cutData;
        }

        private static uint[,] SetArrayEdgesToZero(uint[,] data)
        {
            int nWidth = data.GetLength(0);
            int nHeight = data.GetLength(1);

            for (int i = 0; i < nWidth; i++)
            {
                data[i, 0] = 0;
                data[i, nHeight - 1] = 0;
            }

            for (int j = 0; j < nHeight; j++)
            {
                data[0, j] = 0;
                data[nWidth - 1, j] = 0;
            }

            return data;
        }

        public static int GetColourByteOffset24bbp(TangraConfig.ColourChannel channel)
        {
            if (channel == TangraConfig.ColourChannel.GrayScale)
                // Special meaning
                return -1;

            // BGR
            switch (channel)
            {
                case TangraConfig.ColourChannel.Red:
                    return 2;
                case TangraConfig.ColourChannel.Green:
                    return 1;
                case TangraConfig.ColourChannel.Blue:
                    return 0;
            }

            return 1;
        }

        public static int GetColourByteOffset32bbp(TangraConfig.ColourChannel channel)
        {
            if (channel == TangraConfig.ColourChannel.GrayScale)
                // Special meaning
                return -1;

            // BGRT
            switch (channel)
            {
                case TangraConfig.ColourChannel.Red:
                    return 2;
                case TangraConfig.ColourChannel.Green:
                    return 1;
                case TangraConfig.ColourChannel.Blue:
                    return 0;
            }

            return 1;
        }

        // http://www.echoview.com/WebHelp/Reference/Algorithms/Operators/Convolution_algorithms.htm
        private static ConvMatrix LOW_PASS_FILTER_MATRIX =
            new ConvMatrix(new float[,]
            {
                { 1 / 16.0f, 1 / 8.0f, 1 / 16.0f }, 
                { 1 / 8.0f, 1 / 4.0f, 1 / 8.0f }, 
                { 1 / 16.0f, 1 / 8.0f, 1 / 16.0f }
            });

        public static bool LowPassFilter(Pixelmap b)
        {
            return Convolution.Conv3x3(b, LOW_PASS_FILTER_MATRIX);
        }

		public static uint[,] LowPassFilter(uint[,] b, bool cutEdges)
        {
			uint[,] data = Convolution.Conv3x3(b, LOW_PASS_FILTER_MATRIX);
            if (cutEdges)
                return CutArrayEdges(data);
            else
            {
                data = SetArrayEdgesToZero(data);
                return data;
            }
        }

        public static uint[,] LowPassDifferenceFilter(uint[,] b, bool cutEdges)
        {
            uint[,] data = LowPassDifferenceFilter(b);
            if (cutEdges)
                return CutArrayEdges(data);
            else
                return data;
        }

        public static uint[,] LowPassDifferenceFilter(uint[,] b)
        {
            uint[,] lowPassData = Convolution.Conv3x3(b, LOW_PASS_FILTER_MATRIX);
            uint[,] lowPassDiffData = new uint[lowPassData.GetLength(0), lowPassData.GetLength(1)];

            int nWidth = lowPassData.GetLength(0);
            int nHeight = lowPassData.GetLength(1);

            List<uint> allValues = new List<uint>();

            for (int y = 0; y < nHeight; ++y)
            {
                for (int x = 0; x < nWidth; ++x)
                {
                    // The median 5x5 value is the median of all values in the 5x5 region around the point
                    allValues.Clear();

                    if (x - 2 >= 0)
                    {
                        if (y - 2 >= 0) allValues.Add(lowPassData[x - 2, y - 2]);
                        if (y - 1 >= 0) allValues.Add(lowPassData[x - 2, y - 1]);
                        allValues.Add(lowPassData[x - 2, y]);
                        if (y + 1 < nHeight) allValues.Add(lowPassData[x - 2, y + 1]);
                        if (y + 2 < nHeight) allValues.Add(lowPassData[x - 2, y + 2]);
                    }

                    if (x - 1 >= 0)
                    {
                        if (y - 2 >= 0) allValues.Add(lowPassData[x - 1, y - 2]);
                        if (y - 1 >= 0) allValues.Add(lowPassData[x - 1, y - 1]);
                        allValues.Add(lowPassData[x - 1, y]);
                        if (y + 1 < nHeight) allValues.Add(lowPassData[x - 1, y + 1]);
                        if (y + 2 < nHeight) allValues.Add(lowPassData[x - 1, y + 2]);
                    }

                    allValues.Add(lowPassData[x, y]);

                    if (x + 1 < nWidth)
                    {
                        if (y - 2 >= 0) allValues.Add(lowPassData[x + 1, y - 2]);
                        if (y - 1 >= 0) allValues.Add(lowPassData[x + 1, y - 1]);
                        allValues.Add(lowPassData[x + 1, y]);
                        if (y + 1 < nHeight) allValues.Add(lowPassData[x + 1, y + 1]);
                        if (y + 2 < nHeight) allValues.Add(lowPassData[x + 1, y + 2]);
                    }

                    if (x + 2 < nWidth)
                    {
                        if (y - 2 >= 0) allValues.Add(lowPassData[x + 2, y - 2]);
                        if (y - 1 >= 0) allValues.Add(lowPassData[x + 2, y - 1]);
                        allValues.Add(lowPassData[x + 2, y]);
                        if (y + 1 < nHeight) allValues.Add(lowPassData[x + 2, y + 1]);
                        if (y + 2 < nHeight) allValues.Add(lowPassData[x + 2, y + 2]);
                    }

                    allValues.Sort();
                    int middleCal = allValues.Count / 2;
                    if (allValues.Count % 2 == 1)
                        lowPassDiffData[x, y] = allValues[middleCal];
                    else if (allValues.Count > 1)
                        lowPassDiffData[x, y] = (uint)((allValues[middleCal] + allValues[middleCal - 1]) / 2);
                    else if (allValues.Count == 1)
                        lowPassDiffData[x, y] = allValues[0];

                    if (lowPassDiffData[x, y] > lowPassData[x, y])
                        lowPassDiffData[x, y] = 0;
                    else
                        lowPassDiffData[x, y] = (uint)(lowPassData[x, y] - lowPassDiffData[x, y]);
                }
            }

            return lowPassDiffData;
        }
    }
}
