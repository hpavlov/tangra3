﻿/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Model.Helpers;
using Tangra.Model.Video;
using Tangra.Model.VideoOperations;
using Tangra.VideoOperations.LightCurves.InfoForms;

namespace Tangra.VideoOperations.LightCurves
{
    public partial class ucLCFileInfo : UserControl
    {
        private LCFile m_lcFile;
		protected IVideoController m_VideoController;
	    private bool m_ShowingFields;

		internal ucLCFileInfo(LCFile lcFile, IVideoController videoController)
        {
            InitializeComponent();

            m_lcFile = lcFile;
			m_VideoController = videoController;
            DisplayLCFileInfo();

			// Only analogue video (or derived from it) has fields
			VideoFileFormat videoFileFormat = m_lcFile.Header.GetVideoFileFormat();
            btnShowFields.Visible = videoFileFormat == VideoFileFormat.AVI || videoFileFormat.IsAAV();
			m_ShowingFields = false;
        }

        private void DisplayLCFileInfo()
        {
            var videoFileFormat = m_lcFile.Header.GetVideoFileFormat();
            bool isAdvFile = videoFileFormat == VideoFileFormat.ADV || videoFileFormat == VideoFileFormat.ADV2;

            lblFileName.Text = Path.GetFileName(m_lcFile.Header.PathToVideoFile);
            lblSource.Text = m_lcFile.Header.SourceInfo;
            lblTotalFrames.Text = m_lcFile.Header.CountFrames.ToString();
            lblMeasuredFrames.Text = m_lcFile.Header.MeasuredFrames.ToString();
			lblFPS.Text = isAdvFile ? "N/A" : m_lcFile.Header.FramesPerSecond.ToString("0.000");
			lblFPSComp.Text = isAdvFile ? "N/A" : m_lcFile.Header.ComputedFramesPerSecond.ToString("0.000");
			
            switch(m_lcFile.Header.ReductionType)
            {
                case LightCurveReductionType.Asteroidal:
                    lblEventType.Text = "Asteroidal Occultation";
                    break;
                case LightCurveReductionType.MutualEvent:
                    lblEventType.Text = "Mutual Event";
                    break;
                case LightCurveReductionType.TotalLunarDisappearance:
                    lblEventType.Text = "Total Lunar";
                    break;
                case LightCurveReductionType.UntrackedMeasurement:
                    lblEventType.Text = "Untracked";
                    break;
            }

            switch(m_lcFile.Footer.ReductionContext.FrameIntegratingMode)
            {
                case FrameIntegratingMode.NoIntegration:
                    lblIntegration.Text = "No";
                    break;
                case FrameIntegratingMode.SteppedAverage:
                    lblIntegration.Text = "Stepped";
                    break;
                case FrameIntegratingMode.SlidingAverage:
                    lblIntegration.Text = "Sliding";
                    break;
            }

			lblGamma.Text = m_lcFile.Footer.ProcessedWithTangraConfig.Photometry.EncodingGamma == 1 
				? "No" 
				: m_lcFile.Footer.ProcessedWithTangraConfig.Photometry.EncodingGamma.ToString("0.00");

			lblReversedCameraResponse.Text = m_lcFile.Footer.ProcessedWithTangraConfig.Photometry.KnownCameraResponse == 0
				? "No"
				: m_lcFile.Footer.ProcessedWithTangraConfig.Photometry.KnownCameraResponse.ToString();

        	bool preProcessing = m_lcFile.Footer.ReductionContext.UseStretching ||
        	                     m_lcFile.Footer.ReductionContext.UseClipping ||
        	                     m_lcFile.Footer.ReductionContext.UseBrightnessContrast;
        	lblPreProcessing.Text = preProcessing ? "Yes" : "No";

			if (!string.IsNullOrEmpty(m_lcFile.Footer.ReductionContext.UsedTracker))
			{
				lblTrackerLabel.Visible = true;
				lblTracker.Visible = true;
				lblTracker.Text = m_lcFile.Footer.ReductionContext.UsedTracker;
			}
			else
			{
				lblTrackerLabel.Visible = false;
				lblTracker.Visible = false;				
			}
        }

		private void btnShowFields_Click(object sender, EventArgs e)
		{
			m_ShowingFields = !m_ShowingFields;
            m_VideoController.ToggleShowFieldsMode(m_ShowingFields);
			m_VideoController.RedrawCurrentFrame(m_ShowingFields);

			if (m_ShowingFields)
				btnShowFields.Text = "Show Frames";
			else
				btnShowFields.Text = "Show Fields";
		}
    }
}
