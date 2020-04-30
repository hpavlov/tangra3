using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tangra.Video
{
    public interface IDynamicRangeStream
    {
        uint MinPixelValue { get; }
        uint MaxPixelValue { get; }
        bool CanDetermineAutoDynamicRange { get; }
    }

    public static class DynamicRangeHelper
    {
        public static void SuggestUpperLowerAutoDynamicRangeLimit(uint[] pixels, uint pixMin, uint pixMax, out uint? lowerLimit, out uint? upperLimit)
        {
            uint maxBuckets = 255;
            var distribution = new Dictionary<uint, float>();
            for (uint i = 0; i <= maxBuckets; i++)
            {
                distribution.Add(i, 0);
            }

            var bucketFactor = 1.0f * maxBuckets / pixMax;

            uint mostCommonVal = 0;
            double mostCommonCount = 0;

            double nonZeroPixelCount = 0;
            for (int i = 0; i < pixels.Length; i++)
            {
                uint val = pixels[i];
                if (val == 0)
                {
                    // Don't include zero as this is is full black
                    continue;
                }
                nonZeroPixelCount++;

                uint bucket = Math.Max(0, Math.Min(maxBuckets - 1, (uint)Math.Round(val * bucketFactor)));

                var cnt = distribution[bucket]++;

                if (cnt > mostCommonCount)
                {
                    mostCommonCount = cnt;
                    mostCommonVal = bucket;
                }
            }

            uint? minCont = null;
            uint? maxCont = null;
            double halfPercent = nonZeroPixelCount * 0.005;
            for (uint i = mostCommonVal - 1; i > 0; i--)
            {
                if (!distribution.ContainsKey(i) || distribution[i] < halfPercent)
                {
                    minCont = i + 1;
                    break;
                }
            }

            for (uint i = mostCommonVal + 1; i < uint.MaxValue; i++)
            {
                if (!distribution.ContainsKey(i) || distribution[i] < halfPercent)
                {
                    maxCont = i + 1;
                    break;
                }
            }

            if (maxCont != null && minCont != null && (maxCont.Value - minCont.Value) > 255 * 0.01)
            {
                lowerLimit = (uint)Math.Max(pixMin, ((minCont.Value - 1) / bucketFactor));
                upperLimit = (uint)Math.Min(pixMax, ((maxCont.Value + 1) / bucketFactor));
            }
            else
            {
                lowerLimit = pixMin;
                upperLimit = pixMax;
            }
        }
    }
}
