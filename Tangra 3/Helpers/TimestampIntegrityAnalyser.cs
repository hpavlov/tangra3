using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tangra.Model.Helpers;

namespace Tangra.Helpers
{
    public class TimestampIntegrityAnalyser
    {
        public double MedianExposureMs { get; private set; }

        public double OneSigmaExposureMs { get; private set; }

        public int DroppedFrames { get; private set; }

        public double DroppedFramesPercentage { get; private set; }

        public void Calculate(List<double> exposuresMs, List<DateTime> orderedTimestamps)
        {
            // Fist estimate assuming no dropped frames
            MedianExposureMs = exposuresMs.Median();
            OneSigmaExposureMs = 0;
            DroppedFrames = 0;
            DroppedFramesPercentage = 0;

            if (orderedTimestamps.Count > 2)
            {
                exposuresMs.Clear();

                var currTs = orderedTimestamps[0];
                for (int i = 1; i < orderedTimestamps.Count; i++)
                {
                    int frameGap = 1;
                    var nextTs = currTs.AddMilliseconds(MedianExposureMs);
                    while (Math.Abs((nextTs - orderedTimestamps[i]).TotalMilliseconds) > MedianExposureMs / 10 && nextTs < orderedTimestamps[i])
                    {
                        DroppedFrames++;
                        nextTs = nextTs.AddMilliseconds(MedianExposureMs);
                        frameGap++;
                    }

                    exposuresMs.Add((orderedTimestamps[i] - currTs).TotalMilliseconds / frameGap);
                    currTs = orderedTimestamps[i];
                }

                DroppedFramesPercentage = DroppedFrames * 100.0 / (orderedTimestamps.Count + DroppedFrames);

                // Second estimate, adjustment for dropped frames.
                MedianExposureMs = exposuresMs.Median();

                OneSigmaExposureMs = Math.Sqrt(exposuresMs.Select(x => (x - MedianExposureMs) * (x - MedianExposureMs)).Sum() / (exposuresMs.Count - 1));
            }
        }
    }
}
