/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Tangra.Helpers;
using Tangra.Model.Image;
using Tangra.OCR;
using Tangra.TangraService;

namespace Tangra.VideoOperations
{
	public partial class frmOsdOcrCalibrationFailure : Form
	{
		private Dictionary<string, uint[]> Images;

        private uint[] LastUnmodifiedImage;

		private ITimestampOcr m_TimestampOCR;

		public ITimestampOcr TimestampOCR
		{
			get { return m_TimestampOCR; }
			set
			{
				m_TimestampOCR = value;
				Images = m_TimestampOCR.GetCalibrationReportImages();
			    LastUnmodifiedImage = m_TimestampOCR.GetLastUnmodifiedImage();
			}
		}

		internal bool ForcedErrorReport;

		public bool CanSendReport()
		{
			return Images != null && Images.Count > 0;
		}

		public frmOsdOcrCalibrationFailure()
		{
			InitializeComponent();
		}

		private void frmOsdOcrCalibrationFailure_Load(object sender, EventArgs e)
		{
			if (ForcedErrorReport)
			{
				pnlForcedReport.Top = 3;
				pnlForcedReport.Left = 3;
				pnlForcedReport.Visible = true;
				pnlForcedReport.BringToFront();
				pnlGenuineReport.Visible = false;
				pnlGenuineReport.SendToBack();

				lblOsdReaderType2.Text = m_TimestampOCR.OSDType();
			}
			else
			{
				pnlGenuineReport.Top = 3;
				pnlGenuineReport.Left = 3;
				pnlGenuineReport.Visible = true;
				pnlGenuineReport.BringToFront();
				pnlForcedReport.Visible = false;
				pnlForcedReport.SendToBack();

				lblOsdReaderType.Text = m_TimestampOCR.OSDType();
			}
		}

        private void EnableDisableFormControls(bool enable)
        {
            btnIgnore.Enabled = enable;
            btnSendReport.Enabled = enable;
        }

		private void btnSendReport_Click(object sender, EventArgs e)
		{
			Cursor = Cursors.WaitCursor;
            EnableDisableFormControls(false);

			bool reportSendingErrored = false;

			try
			{
				SendOcrErrorReport(m_TimestampOCR, Images, LastUnmodifiedImage, tbxEmail.Text);
			}
			catch (Exception ex)
			{
				MessageBox.Show(this, "Error sending the report.\r\n\r\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				reportSendingErrored = true;
			}
			finally
			{

				Cursor = Cursors.Default;

				EnableDisableFormControls(true);

				if (!reportSendingErrored)
					MessageBox.Show("The error report was submitted successfully.", "Tangra", MessageBoxButtons.OK, MessageBoxIcon.Information);

				DialogResult = DialogResult.OK;
				Close();
			}
		}

		public static void SendOcrErrorReport(ITimestampOcr timestampOCR, Dictionary<string, uint[]> images, uint[] lastUnmodifiedImage, string email)
		{
			string tempDir = Path.GetFullPath(Path.GetTempPath() + @"\" + Guid.NewGuid().ToString());
			string tempFile = Path.GetTempFileName();
			
			try
			{
				Directory.CreateDirectory(tempDir);

				int fieldAreaWidth = timestampOCR.InitializationData.OSDFrame.Width;
				int fieldAreaHeight = timestampOCR.InitializationData.OSDFrame.Height;
				int frameWidth = timestampOCR.InitializationData.FrameWidth;
				int frameHeight = timestampOCR.InitializationData.FrameHeight;

				foreach (string key in images.Keys)
				{
					uint[] pixels = images[key];
					Bitmap img = null;
					if (pixels.Length == fieldAreaWidth * fieldAreaHeight)
						img = Pixelmap.ConstructBitmapFromBitmapPixels(pixels, fieldAreaWidth, fieldAreaHeight);
					else if (pixels.Length == frameWidth * frameHeight)
						img = Pixelmap.ConstructBitmapFromBitmapPixels(pixels, frameWidth, frameHeight);

					if (img != null)
						img.Save(Path.GetFullPath(string.Format(@"{0}\{1}", tempDir, key)), ImageFormat.Bmp);
				}

				if (lastUnmodifiedImage != null)
				{
					Bitmap fullFrame = Pixelmap.ConstructBitmapFromBitmapPixels(lastUnmodifiedImage, frameWidth, frameHeight);
					fullFrame.Save(Path.GetFullPath(string.Format(@"{0}\full-frame.bmp", tempDir)), ImageFormat.Bmp);
				}

				ZipUnzip.Zip(tempDir, tempFile, false);
				byte[] attachment = File.ReadAllBytes(tempFile);

				var binding = new BasicHttpBinding();
				var address = new EndpointAddress("http://www.tangra-observatory.org/TangraErrors/ErrorReports.asmx");
				var client = new TangraService.ServiceSoapClient(binding, address);
				
				string errorReportBody = "OSD OCR Calibration Error\r\n\r\n" +
				                         "OCR OSD Engine: " + timestampOCR.NameAndVersion() + "\r\n" +
				                         "OSD Type: " + timestampOCR.OSDType() + "\r\n\r\n" +
										 "Contact Email: " + email + "\r\n\r\n" +
				                         frmSystemInfo.GetFullVersionInfo();

				List<string> errorMesages = timestampOCR.GetCalibrationErrors();
				if (errorMesages != null && errorMesages.Count > 0)
					errorReportBody += "\r\n\r\n" + string.Join("\r\n", errorMesages);

				client.ReportErrorWithAttachment(
					errorReportBody,
					string.Format("CalibrationFrames-{0}.zip", Guid.NewGuid().ToString()),
					attachment);
			}
			finally
			{
				if (Directory.Exists(tempDir))
				{
					try
					{
						Directory.Delete(tempDir, true);
					}
					catch { }
				}

				if (File.Exists(tempFile))
				{
					try
					{
						File.Delete(tempFile);
					}
					catch { }
				}
			}
		}

		private Regex m_SimpleEmailRegex = new Regex("^[^@]+@[^\\.]+\\.[^\\.]+$");

		private void tbxEmail_TextChanged(object sender, EventArgs e)
		{
			btnSendReport.Enabled = m_SimpleEmailRegex.IsMatch(tbxEmail.Text);
		}
	}
}
