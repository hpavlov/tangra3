using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
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

			// Only 8 bit video (analogue) has fields
			btnShowFields.Visible = m_lcFile.Footer.ReductionContext.BitPix == 8;
			m_ShowingFields = false;
        }

        private void DisplayLCFileInfo()
        {
			bool isAdvFile = 
				m_lcFile.Footer.ReductionContext.BitPix > 8 || 
				m_lcFile.Header.SourceInfo.Contains("(AAV.8)");

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

		private void btnCompleteInfo_Click(object sender, EventArgs e)
		{
			var frm = new frmCompleteReductionInfoForm(m_lcFile);
			frm.ShowDialog(this);
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
