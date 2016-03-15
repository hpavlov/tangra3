/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
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
		    bool isAdvFile = m_lcFile.Header.GetVideoFileFormat() == VideoFileFormat.ADV;

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

			lblReversedCameraResponse.Text = m_lcFile.Footer.ProcessedWithTangraConfig.Photometry.KnownCameraResponse == 0
				? "No"
				: m_lcFile.Footer.ProcessedWithTangraConfig.Photometry.KnownCameraResponse.ToString();

			lblWind.Text = m_lcFile.Footer.ReductionContext.WindOrShaking ? "Yes" : "No";
			lblFullD.Text = m_lcFile.Footer.ReductionContext.FullDisappearance ? "Yes" : "No";
			lblFlickering.Text = m_lcFile.Footer.ReductionContext.HighFlickeringOrLargeStars ? "Yes" : "No";
            lblFieldRotation.Text = m_lcFile.Footer.ReductionContext.FieldRotation ? "Yes" : "No";

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

		private void btnShowAll_Click(object sender, EventArgs e)
		{
			BuildAllInfo();
			dgvLCFileInfo.Visible = true;
			dgvLCFileInfo.BringToFront();
		}

		private void BuildAllInfo()
		{
			var data = new List<LCFileTagValuePair>();

			AddPropertiesViaReflection(m_lcFile, data);
			AddPropertiesViaReflection(m_lcFile.Header, data);
			AddPropertiesViaReflection(m_lcFile.Footer, data);
			AddPropertiesViaReflection(m_lcFile.Footer.ReductionContext, data, "Reduction.");
			for (int i = 0; i < m_lcFile.Footer.TrackedObjects.Count; i++)
				AddPropertiesViaReflection(m_lcFile.Footer.TrackedObjects[i], data, string.Format("ObjectConfig[{0}].", i));
				

			data.Sort((x, y) => x.Property.CompareTo(y.Property));
			dgvLCFileInfo.DataSource = data;
		}

		private void AddPropertiesViaReflection(object root, List<LCFileTagValuePair> data, string prefix = null)
		{
			PropertyInfo[] pis = root.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			foreach (PropertyInfo pi in pis)
			{
				if (pi.PropertyType.IsPrimitive || pi.PropertyType == typeof(string) || pi.PropertyType.IsEnum)
				{
					data.Add(new LCFileTagValuePair()
						{
							Property = prefix + pi.Name,
							Value = Convert.ToString(pi.GetValue(root, null))
						});
				}
			}

			FieldInfo[] fis = root.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			foreach (FieldInfo fi in fis)
			{
				if (fi.IsPrivate) continue;
				if (fi.FieldType.IsPrimitive || fi.FieldType == typeof(string) || fi.FieldType.IsEnum)
				{
					data.Add(new LCFileTagValuePair()
					{
						Property = prefix + fi.Name,
						Value = Convert.ToString(fi.GetValue(root))
					});
				}
			}
		}
	}
}
