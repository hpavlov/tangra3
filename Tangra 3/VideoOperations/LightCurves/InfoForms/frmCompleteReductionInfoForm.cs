using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Model.Video;

namespace Tangra.VideoOperations.LightCurves.InfoForms
{
	public partial class frmCompleteReductionInfoForm : Form
	{
		private LCFile m_lcFile;

		internal frmCompleteReductionInfoForm(LCFile lcFile)
		{
			InitializeComponent();

			m_lcFile = lcFile;
		}

		private void frmCompleteReductionInfoForm_Load(object sender, EventArgs e)
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

		    lblSecondTime.Text = m_lcFile.Header.SecondTimedFrameTime.ToString("HH:mm:ss.fff");
		    long tmb;
		    m_lcFile.Footer.ThumbPrintDict.TryGetValue(m_lcFile.Header.LastTimedFrameNo, out tmb);
            lblSecondTimedFrame.Text = string.Format("{0} (0x{1})", m_lcFile.Header.LastTimedFrameNo, Convert.ToString(tmb, 16));

            lblFirstTime.Text = m_lcFile.Header.FirstTimedFrameTime.ToString("HH:mm:ss.fff");
            m_lcFile.Footer.ThumbPrintDict.TryGetValue(m_lcFile.Header.FirstTimedFrameNo, out tmb);
            lblFirstTimedFrame.Text = string.Format("{0} (0x{1})", m_lcFile.Header.FirstTimedFrameNo, Convert.ToString(tmb, 16));

			switch (m_lcFile.Header.ReductionType)
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

			switch (m_lcFile.Footer.ReductionContext.FrameIntegratingMode)
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

			lblIntegratedFrames.Text = m_lcFile.Footer.ReductionContext.NumberFramesToIntegrate.ToString();
			lblPixelIntegration.Text =
				m_lcFile.Footer.ReductionContext.PixelIntegrationType == PixelIntegrationType.Mean
					? "Mean"
					: "Median";

			pnlIntegration.Visible =
				m_lcFile.Footer.ReductionContext.FrameIntegratingMode != FrameIntegratingMode.NoIntegration;

			lblGamma.Text = m_lcFile.Footer.ProcessedWithTangraConfig.Photometry.EncodingGamma == 1
				? "No"
				: m_lcFile.Footer.ProcessedWithTangraConfig.Photometry.EncodingGamma.ToString("0.00");

			lblWind.Text = m_lcFile.Footer.ReductionContext.WindOrShaking ? "Yes" : "No";
			lblFullD.Text = m_lcFile.Footer.ReductionContext.FullDisappearance ? "Yes" : "No";
			lblFlickering.Text = m_lcFile.Footer.ReductionContext.HighFlickering ? "Yes" : "No";

			lblIsColourVideo.Text = m_lcFile.Footer.ReductionContext.IsColourVideo ? "Yes" : "No";

			//m_lcFile.Footer.TrackedObjects

			bool usesReprocessing = m_lcFile.Footer.ReductionContext.UseClipping ||
			                        m_lcFile.Footer.ReductionContext.UseStretching ||
			                        m_lcFile.Footer.ReductionContext.UseBrightnessContrast;
			pnlReProcess.Visible = usesReprocessing;
			if (usesReprocessing) SetReProcessInfo();
		}

		private void SetReProcessInfo()
		{
			lblReprocess.Text = "N/A";
			lblReprocessLbl.Text = "Reprocessing:";

			if (m_lcFile.Footer.ReductionContext.UseClipping)
			{
				lblReprocess.Text = "clipping";
				lblReprocessVal1Label.Text = "From byte:";
				lblReprocessVal2Label.Text = "To byte:";
				lblReprocessVal1.Text = m_lcFile.Footer.ReductionContext.FromByte.ToString();
				lblReprocessVal2.Text = m_lcFile.Footer.ReductionContext.ToByte.ToString();
			}
			else if (m_lcFile.Footer.ReductionContext.UseClipping)
			{
				lblReprocess.Text = "stretching";
				lblReprocessVal1Label.Text = "From byte:";
				lblReprocessVal2Label.Text = "To byte:";
				lblReprocessVal1.Text = m_lcFile.Footer.ReductionContext.FromByte.ToString();
				lblReprocessVal2.Text = m_lcFile.Footer.ReductionContext.ToByte.ToString();
			}
			else if (m_lcFile.Footer.ReductionContext.UseBrightnessContrast)
			{
				lblReprocess.Text = "brightness/contrast";
				lblReprocessVal1Label.Text = "Brightness:";
				lblReprocessVal2Label.Text = "Contrast:";
				lblReprocessVal1.Text = m_lcFile.Footer.ReductionContext.Brightness.ToString();
				lblReprocessVal2.Text = m_lcFile.Footer.ReductionContext.Contrast.ToString();
			}
		}
	}
}
