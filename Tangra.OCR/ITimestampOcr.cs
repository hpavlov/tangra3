﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Tangra.Model.Config;
using Tangra.Model.VideoOperations;

namespace Tangra.OCR
{
    public class TimestampOCRData
    {
        public int FrameWidth;
        public int FrameHeight;
        public Rectangle OSDFrame;
        public float VideoFrameRate;
        public string SourceInfo;
	    public bool ForceErrorReport;
    }

    public interface ITimestampOcr
    {
        string NameAndVersion();
        string OSDType();
        void Initialize(TimestampOCRData initializationData, IVideoController videoController, int performanceMode);
        void RefiningFrame(uint[] data, float refiningPercentageLeft);
        void PrepareForMeasurements(uint[] data);
        bool ExtractTime(int frameNo, uint[] data, out DateTime time);

        bool RequiresConfiguring { get; }
        bool RequiresCalibration { get; }
        void TryToAutoConfigure(uint[] data);

        bool ProcessCalibrationFrame(int frameNo, uint[] data);
        void AddConfiguration(uint[] data, OCRConfigEntry config);
		Dictionary<string, uint[]> GetCalibrationReportImages();
        uint[] GetLastUnmodifiedImage();
		TimestampOCRData InitializationData { get; }
	    void DrawLegend(Graphics graphics);

        string InitiazliationError { get; }
    }
}