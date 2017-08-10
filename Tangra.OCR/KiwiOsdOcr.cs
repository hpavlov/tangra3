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
    public class KiwiOsdOcr : ITimestampOcr
    {
        private IVideoController m_VideoController;
        private TimestampOCRData m_InitializationData;

        private Dictionary<string, uint[]> m_CalibrationImages = new Dictionary<string, uint[]>();
        private List<string> m_CalibrationErrors = new List<string>();
        private uint[] m_LatestFrameImage;

        public string NameAndVersion()
        {
            return "KIWI-OSD v1.0";
        }

        public string OSDType()
        {
            return "KIWI-OSD";
        }

        public void Initialize(TimestampOCRData initializationData, IVideoController videoController, int performanceMode)
        {
            m_InitializationData = initializationData;
            m_VideoController = videoController;
        }

        public bool ExtractTime(int frameNo, int frameStep, uint[] data, bool debug, out DateTime time)
        {
            LastOddFieldOSD = null;
            LastEvenFieldOSD = null;

            if (m_VideoController != null)
                // Leave this to capture statistics
                m_VideoController.RegisterExtractingOcrTimestamps();

	        m_LatestFrameImage = data;

            // TODO: Add main OCR Implementation HERE


            // NOTE: if there is an error extracting the timestamp then call m_VideoController.RegisterOcrError(); for Tangra to show a counter in the status bar with number of errors so far
            // NOTE: This dummy implementation simply marks all frames as OCR errors
            if (m_VideoController != null)
                m_VideoController.RegisterOcrError();

            // NOTE: See IotaVtiOrcManaged.ExtractDateTime() for ideas about checking incorrectly extracted times and attempting to correct them.

            time = DateTime.MinValue;
            return false;
        }

        public IVtiTimeStamp LastOddFieldOSD { get; private set; }
        public IVtiTimeStamp LastEvenFieldOSD { get; private set; }
        public string LastFailedReason { get; private set; }

        public Bitmap GetOCRDebugImage()
        {
            // TODO:
            return null;
        }

        public bool RequiresCalibration
        {
            get
            {
                return false;
            }
        }

        public bool ProcessCalibrationFrame(int frameNo, uint[] data)
        {
            // If RequiresCalibration returns true, Tangra will be calling ProcessCalibrationFrame() either until
            // this method returns 'true' or until InitiazliationError returns a non null value, which will be considered 
            // as failure to recognize the timestamp/calibrate and in this case OCR will not be run with the measurements

            // NOTE: In case of a calibration error Tangra will offer the end user to submit an error report
            // All images returned by GetCalibrationReportImages() and all errors returned by GetCalibrationErrors()
            // will be included in this error report. See the IotaVtiOcrManaged implementation for how to add calibration images to the dictionary

            return false;
        }

        public Dictionary<string, uint[]> GetCalibrationReportImages()
        {
            return m_CalibrationImages;
        }

        public List<string> GetCalibrationErrors()
        {
            return m_CalibrationErrors;
        }

        public uint[] GetLastUnmodifiedImage()
        {
            return m_LatestFrameImage;
        }

        public TimestampOCRData InitializationData
        {
            get { return m_InitializationData; }
        }

        public void DrawLegend(Graphics graphics)
        {
            // Optional

            // This can be used to draw anything on the current video frame. 
            // For example the IOTA-VTI OCR engine draws rectangles around the positions of the digits
            // This is a quick indication to the user that the OCR is doing its job
        }

        public string InitiazliationError { get; private set; }
    }
}
