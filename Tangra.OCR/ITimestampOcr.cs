/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
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
		Dictionary<string, uint[]> GetCalibrationReportImages();
	    List<string> GetCalibrationErrors();
        uint[] GetLastUnmodifiedImage();
		TimestampOCRData InitializationData { get; }
	    void DrawLegend(Graphics graphics);

        string InitiazliationError { get; }
    }
}
