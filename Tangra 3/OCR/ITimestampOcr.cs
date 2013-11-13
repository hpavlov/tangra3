using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Tangra.Model.Config;

namespace Tangra.OCR
{
    public class TimestampOCRData
    {
        public int FrameWidth;
        public int FrameHeight;
        public Rectangle OSDFrame;
        public float VideoFrameRate;
        public string SourceInfo;
    }

    public interface ITimestampOcr
    {
        string NameAndVersion();
        string OSDType();
        void Initialize(TimestampOCRData initializationData);
        void RefiningFrame(uint[] data, float refiningPercentageLeft);
        void PrepareForMeasurements(uint[] data);
        bool ExtractTime(int frameNo, uint[] data, out DateTime time);

        bool RequiresConfiguring { get; }
        bool RequiresCalibration { get; }
        void TryToAutoConfigure(uint[] data);

        OCRConfigEntry TryCalibrate(uint[] data);
        void AddConfiguration(uint[] data, OCRConfigEntry config);
    }
}
