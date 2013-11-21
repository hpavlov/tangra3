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
			lblOsdReaderType.Text = m_TimestampOCR.OSDType();
		}

        private void EnableDisableFormControls(bool enable)
        {
            btnIgnore.Enabled = enable;
            btnSendReport.Enabled = enable;
        }

		private void btnSendReport_Click(object sender, EventArgs e)
		{
			Cursor = Cursors.WaitCursor;
			string tempDir = Path.GetFullPath(Path.GetTempPath() + @"\" + Guid.NewGuid().ToString());
			string tempFile = Path.GetTempFileName();
            EnableDisableFormControls(false);
			try
			{
				Directory.CreateDirectory(tempDir);

				int fieldAreaWidth = m_TimestampOCR.InitializationData.OSDFrame.Width;
				int fieldAreaHeight = m_TimestampOCR.InitializationData.OSDFrame.Height;

				foreach (string key in Images.Keys)
				{
					Bitmap img = Pixelmap.ConstructBitmapFromBitmapPixels(Images[key], fieldAreaWidth, fieldAreaHeight);
					img.Save(Path.GetFullPath(string.Format(@"{0}\{1}", tempDir, key)), ImageFormat.Bmp);
				}

                Bitmap fullFrame = Pixelmap.ConstructBitmapFromBitmapPixels(LastUnmodifiedImage, m_TimestampOCR.InitializationData.FrameWidth, m_TimestampOCR.InitializationData.FrameHeight);
                fullFrame.Save(Path.GetFullPath(string.Format(@"{0}\full-frame.bmp", tempDir)), ImageFormat.Bmp);

				ZipUnzip.Zip(tempDir, tempFile, false);
				byte[] attachment = File.ReadAllBytes(tempFile);

				var binding = new BasicHttpBinding();
				var address = new EndpointAddress("http://208.106.227.157/CGI-BIN/TangraErrors/ErrorReports.asmx");
				var client = new TangraErrorSinkSoapClient(binding, address);
				client.ReportErrorWithAttachment(
					"OSD OCR Calibration Error\r\n\r\nOCR OSD Engine: " + m_TimestampOCR.NameAndVersion() + "\r\nOSD Type: " + m_TimestampOCR.OSDType(), string.Format("CalibrationFrames-{0}.zip", Guid.NewGuid().ToString()),
					attachment);
			}
			catch (Exception ex)
			{
				MessageBox.Show(this, "Error sending the report.\r\n\r\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

				Cursor = Cursors.Default;

                EnableDisableFormControls(true);

			    MessageBox.Show("The error report was submitted successfully.", "Tangra", MessageBoxButtons.OK, MessageBoxIcon.Information);

				DialogResult = DialogResult.OK;
				Close();
			}
		}
	}
}
