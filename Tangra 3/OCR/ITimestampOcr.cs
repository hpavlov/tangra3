﻿using System;
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
        void RefiningFrame(uint[] data, int refiningFramesRemaining);
        void PrepareForMeasurements(uint[] data);
        bool ExtractTime(uint[] data, out DateTime time, out byte[,] timestampPixels);

        bool RequiresConfiguring { get; }
        bool RequiresCalibration { get; }
        void TryToAutoConfigure(uint[] data);

        OCRConfigEntry TryCalibrate(uint[] data);
        void AddConfiguration(uint[] data, OCRConfigEntry config);
    }
}