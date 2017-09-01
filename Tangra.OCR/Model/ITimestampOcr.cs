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
using Tangra.OCR.TimeExtraction;

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
        public int IntegratedAAVFrames;
    }

    public interface ITimestampOcr
    {
        string NameAndVersion();
        string OSDType();
        void Initialize(TimestampOCRData initializationData, IVideoController videoController, int performanceMode);
        bool ExtractTime(int frameNo, int frameStep, uint[] data, bool debug, out DateTime time);

        bool RequiresCalibration { get; }

        bool ProcessCalibrationFrame(int frameNo, uint[] data);
		Dictionary<string, Tuple<uint[], int, int>> GetCalibrationReportImages();
	    List<string> GetCalibrationErrors();
        uint[] GetLastUnmodifiedImage();
		TimestampOCRData InitializationData { get; }
	    void DrawLegend(Graphics graphics);

        string InitiazliationError { get; }

        IVtiTimeStamp LastOddFieldOSD { get; }
        IVtiTimeStamp LastEvenFieldOSD { get; }
        string LastFailedReason { get; }

        Bitmap GetOCRDebugImage();
        Bitmap GetOCRCalibrationDebugImage();
        void PrepareFailedCalibrationReport();

        bool TimeStampHasDatePart { get; }
    }
}
