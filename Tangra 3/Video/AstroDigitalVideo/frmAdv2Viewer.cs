using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Adv;
using Tangra.Helpers;
using Tangra.Model.Image;
using Tangra.Video.AstroDigitalVideo.UI;

namespace Tangra.Video.AstroDigitalVideo
{
    public partial class frmAdv2Viewer : Form
    {
        private string m_FileName;
        private AdvFile2 m_AdvFile;

        public frmAdv2Viewer(string fileName)
        {
            InitializeComponent();

            m_FileName = fileName;

            m_AdvFile = new AdvFile2(fileName);
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            if (m_AdvFile != null)
            {
                m_AdvFile.Dispose();
                m_AdvFile = null;
            }
            base.Dispose(disposing);
        }

        private void frmAdv2Viewer_Load(object sender, EventArgs e)
        {
            PresentFile();
        }

        private void PresentFile()
        {
            lblFileName.Text = m_FileName;
            lblColourOrBW.Text = m_AdvFile.IsColourImage ? "Colour" : "Monochrome";
            lblUTCAccuracy.Text = GetTimingAccuracyNanoSec(m_AdvFile.UtcTimestampAccuracyInNanoseconds);

            var bindingList = new List<AdvTagValuePair>();
            foreach (string key in m_AdvFile.SystemMetadataTags.Keys)
            {
                bindingList.Add(new AdvTagValuePair() { Tag = key, Value = m_AdvFile.SystemMetadataTags[key] });
            }
            foreach (string key in m_AdvFile.UserMetadataTags.Keys)
            {
                bindingList.Add(new AdvTagValuePair() { Tag = key, Value = m_AdvFile.UserMetadataTags[key] });
            }
            dgvFileTags.DataSource = bindingList;

            var bindingListMainTags = new List<AdvTagValuePair>();
            foreach (string key in m_AdvFile.MainSteamInfo.MetadataTags.Keys)
            {
                bindingListMainTags.Add(new AdvTagValuePair() { Tag = key, Value = m_AdvFile.MainSteamInfo.MetadataTags[key] });
            }
            dgvMainTags.DataSource = bindingListMainTags;
            lblMainFrames.Text = m_AdvFile.MainSteamInfo.FrameCount.ToString();
            lblMainFrequency.Text = GetHertzValue(m_AdvFile.MainSteamInfo.ClockFrequency);
            lblMainAccuracy.Text = GetTicksAccuracy(m_AdvFile.MainSteamInfo.TimingAccuracy, m_AdvFile.MainSteamInfo.ClockFrequency);

            var bindingListCalibTags = new List<AdvTagValuePair>();
            foreach (string key in m_AdvFile.CalibrationSteamInfo.MetadataTags.Keys)
            {
                bindingListCalibTags.Add(new AdvTagValuePair() { Tag = key, Value = m_AdvFile.CalibrationSteamInfo.MetadataTags[key] });
            }
            dgvCalibrationTags.DataSource = bindingListCalibTags;
            lblCalibrationFrames.Text = m_AdvFile.CalibrationSteamInfo.FrameCount.ToString();
            lblCalibrationFrequency.Text = GetHertzValue(m_AdvFile.CalibrationSteamInfo.ClockFrequency);
            lblCalibrationAccuracy.Text = GetTicksAccuracy(m_AdvFile.CalibrationSteamInfo.TimingAccuracy, m_AdvFile.CalibrationSteamInfo.ClockFrequency);

            lblWidth.Text = m_AdvFile.Width.ToString();
            lblHeight.Text = m_AdvFile.Height.ToString();
            lblPixelRange.Text = GetPixelRange(m_AdvFile.DataBpp, m_AdvFile.MaxPixelValue);
            var bindingListImageTags = new List<AdvTagValuePair>();
            foreach (string key in m_AdvFile.ImageSectionTags.Keys)
            {
                bindingListImageTags.Add(new AdvTagValuePair() { Tag = key, Value = m_AdvFile.ImageSectionTags[key] });
            }
            dgvImageTags.DataSource = bindingListImageTags;
            foreach (var layout in m_AdvFile.ImageLayouts)
            {
                var listItem = lvImageLayouts.Items.Add(layout.LayoutId.ToString());
                listItem.SubItems.Add(layout.Bpp.ToString());
                listItem.Tag = layout;
            }      
            if (lvImageLayouts.Items.Count > 0) lvImageLayouts.Items[0].Selected = true;

            foreach (var entry in m_AdvFile.StatusTagDefinitions)
            {
                var listItem = lvStatusTags.Items.Add(entry.Item2.ToString());
                listItem.SubItems.Add(entry.Item1.ToString());
                listItem.SubItems.Add(entry.Item3.ToString());
            }

            if (m_AdvFile.MainSteamInfo.FrameCount > 0)
            {
                nudCropMainFirstFrame.Maximum = (int)m_AdvFile.MainSteamInfo.FrameCount - 1;
                nudCropMainFirstFrame.Value = 0;
                nudCropMainLastFrame.Maximum = (int)m_AdvFile.MainSteamInfo.FrameCount - 1;
                nudCropMainLastFrame.Value = nudCropMainLastFrame.Maximum;

                rbMainStream.Checked = true;
                rbMainStrToAVI.Checked = true;
                rbMainStrToCSV.Checked = true;
            }
            else
            {
                rbMainStrToAVI.Enabled = false;
                rbMainStrToCSV.Enabled = false;
                rbMainStream.Enabled = false;
                rbMainStream.Checked = false;
                rbMainStrToAVI.Checked = false;
                rbMainStrToCSV.Checked = false;

                nudCropMainFirstFrame.Enabled = false;
                nudCropMainLastFrame.Enabled = false;
            }

            if (m_AdvFile.CalibrationSteamInfo.FrameCount > 0)
            {
                nudCropCalibrFirstFrame.Maximum = (int) m_AdvFile.CalibrationSteamInfo.FrameCount - 1;
                nudCropCalibrFirstFrame.Value = 0;
                nudCropCalibrLastFrame.Maximum = (int) m_AdvFile.CalibrationSteamInfo.FrameCount - 1;
                nudCropCalibrLastFrame.Value = nudCropCalibrFirstFrame.Maximum;
                pnlCalibrCropControls.Visible = true;
                rbCalibrStrToAVI.Enabled = true;
                rbCalibStrToCSV.Enabled = true;
                rbCalibrationStream.Enabled = true;

                if (m_AdvFile.MainSteamInfo.FrameCount == 0)
                {
                    rbCalibrStrToAVI.Checked = true;
                    rbCalibStrToCSV.Checked = true;
                    rbCalibrationStream.Checked = true;                   
                }
            }
            else
            {
                pnlCalibrCropControls.Visible = false;
                rbCalibrStrToAVI.Enabled = false;
                rbCalibStrToCSV.Enabled = false;
                rbCalibrationStream.Enabled = false;
            }

            cbxFrameRate.SelectedIndex = 0;
            cbxAddedGamma.SelectedIndex = 0;
            UpdateSelectedStreamControls();

#if !WIN32
		    gbxConvertToAVI.Visible = false;
#endif
        }
        
        private void lvImageLayouts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvImageLayouts.SelectedItems.Count == 1)
            {
                var layout = lvImageLayouts.SelectedItems[0].Tag as ImageLayoutDefinition;
                if (layout != null)
                {
                    dgvLayoutTags.DataSource = null;
                    var bindingListLayoutTags = new List<AdvTagValuePair>();
                    foreach (string key in layout.ImageLayoutTags.Keys)
                    {
                        bindingListLayoutTags.Add(new AdvTagValuePair() { Tag = key, Value = layout.ImageLayoutTags[key] });
                    }
                    dgvLayoutTags.DataSource = bindingListLayoutTags;
                }
            }
        }

        private string GetTimingAccuracyNanoSec(long nanosec)
        {
            if (nanosec == 0)
                return "N/A";

            if (nanosec > 1000000)
                return string.Format("{0:0.000} ms", nanosec/100000.0);
            else if (nanosec > 1000)
                return string.Format("{0:0.000} us", nanosec/100.0);
            else
                return string.Format("{0} ns", nanosec);
        }

        private string GetPixelRange(int bpp, long maxPixelValue)
        {
            if (maxPixelValue > 0)
                return string.Format("0 - {0}", maxPixelValue);
            else
                return string.Format("0 - {0}", Math.Pow(2, bpp));
        }

        private string GetTicksAccuracy(long accuracy, long frequency)
        {
            if (accuracy <= 0)
                return "N/A";

            double ms = 1000.0 * accuracy / frequency;
            if (ms > 0.001)
                return string.Format("{0:0.000} ms\r\n{1} ticks", ms, accuracy);
            else if (ms > 0.000001)
                return string.Format("{0:0.000} us\r\n{1} ticks", ms / 1000.0, accuracy);
            else 
                return string.Format("{0:0.000} ns\r\n{1} ticks", ms / 1000000.0, accuracy);
        }

        private string GetHertzValue(long frequency)
        {
            if (frequency <= 0)
                return "N/A";
            if (frequency > 1000000)
                return string.Format("{0:0.000000} MHz", frequency / 1000000.0);
            else if (frequency > 1000)
                return string.Format("{0:0.000} KHz", frequency / 1000.0);
            else 
                return string.Format("{0} Hz", frequency);
        }

        private void SelectedStreamChanged(object sender, EventArgs e)
        {
            if (rbMainStream.Checked)
            {
                sbFrames.Minimum = -1;
                sbFrames.Value = -1;

                if (m_AdvFile.MainSteamInfo.FrameCount > 0)
                {
                    sbFrames.Maximum = m_AdvFile.MainSteamInfo.FrameCount;
                    sbFrames.Minimum = 1;
                    sbFrames.Value = 1;
                    sbFrames.Enabled = true;
                }
                else
                {
                    sbFrames.Enabled = false;
                }
            }
            else if (rbCalibrationStream.Checked)
            {
                sbFrames.Minimum = -1;
                sbFrames.Value = -1;

                if (m_AdvFile.CalibrationSteamInfo.FrameCount > 0)
                {
                    sbFrames.Maximum = m_AdvFile.CalibrationSteamInfo.FrameCount;
                    sbFrames.Minimum = 1;
                    sbFrames.Value = 1;
                    sbFrames.Enabled = true;
                }
                else
                {
                    sbFrames.Enabled = false;
                }
            }
        }

        private void sbFrames_ValueChanged(object sender, EventArgs e)
        {
            timerScrolling.Enabled = false;
            timerScrolling.Enabled = true;
        }

        private void timerScrolling_Tick(object sender, EventArgs e)
        {
            timerScrolling.Enabled = false;

            AdvFrameInfo frameInfo = null;
            uint[] pixels = null;
            int frameId = sbFrames.Value - 1;
            if (rbMainStream.Checked)
                pixels = m_AdvFile.GetMainFramePixels((uint)frameId, out frameInfo);                
            else
                pixels = m_AdvFile.GetCalibrationFramePixels((uint)frameId, out frameInfo);       

            DisplayFrame(frameId, pixels, frameInfo);
        }

        public Pixelmap CreatePixelmap(uint[] pixels)
        {
            byte[] displayBitmapBytes = new byte[m_AdvFile.Width * m_AdvFile.Height];
            for (int y = 0; y < m_AdvFile.Height; y++)
            {
                for (int x = 0; x < m_AdvFile.Width; x++)
                {
                    int index = x + y * m_AdvFile.Width;

                    if (m_AdvFile.MaxPixelValue == 8)
                        displayBitmapBytes[index] = (byte)((pixels[index] & 0xFF));
                    else if (m_AdvFile.DataBpp == 12)
                        displayBitmapBytes[index] = (byte)((pixels[index] >> 4));
                    else if (m_AdvFile.DataBpp == 14)
                        displayBitmapBytes[index] = (byte)((pixels[index] >> 6));
                    else if (m_AdvFile.DataBpp == 16)
                    {
                        if (m_AdvFile.MaxPixelValue > 0)
                            displayBitmapBytes[index] = (byte)((255.0 * pixels[index] / m_AdvFile.MaxPixelValue));
                        else
                            displayBitmapBytes[index] = (byte)((pixels[index] >> 8));
                    }
                    else
                        displayBitmapBytes[index] = (byte)((pixels[index] >> (m_AdvFile.DataBpp - 8)));
                }
            }

            Bitmap displayBitmap = Pixelmap.ConstructBitmapFromBitmapPixels(displayBitmapBytes, (int)m_AdvFile.Width, (int)m_AdvFile.Height);

            Pixelmap rv = new Pixelmap((int)m_AdvFile.Width, (int)m_AdvFile.Height, m_AdvFile.DataBpp, pixels, displayBitmap, displayBitmapBytes);

            return rv;
        }

        private void DisplayFrame(int frameId, uint[] pixels, AdvFrameInfo frameInfo)
        {
            Bitmap displayBitmap;
            if (pixels != null)
            {
                var pixelMap = CreatePixelmap(pixels);
                displayBitmap = pixelMap.DisplayBitmap;
            }
            else
            {
                displayBitmap = new Bitmap(picSmallImage.Width, picSmallImage.Height);
                using (Graphics g = Graphics.FromImage(displayBitmap))
                {
                    g.Clear(SystemColors.ControlDarkDark);
                    g.Save();
                }
            }

            picSmallImage.Image = displayBitmap;

            lvFrameStatusData.Items.Clear();
            lblFrameStart.Text = "";
            lblFrameExposure.Text = "";
            lblFrameEnd.Text = "";
            lblFrameSystemTime.Text = "";
            lblFrameId.Text = "";
            lblFrameLayout.Text = "";
            lblDataBlockSize.Text = "";
            lblFrameGain.Text = "";
            lblFrameGamma.Text = "";
            lblFrameOffset.Text = "";
            lblFrameNumSatellites.Text = "";
            lblFrameAlmanacStatus.Text = "";
            lblFrameAlmanacOffset.Text = "";

            if (frameInfo != null)
            {
                if (frameInfo.HasUtcTimeStamp)
                {
                    lblFrameStart.Text = frameInfo.UtcStartExposureTimeStamp.ToString("dd-MMM-yyyy HH:mm:ss.fff");
                    lblFrameExposure.Text = frameInfo.UtcExposureMilliseconds.ToString("0.0") + " ms";
                    lblFrameEnd.Text = frameInfo.UtcStartExposureTimeStamp.AddMilliseconds(frameInfo.UtcExposureMilliseconds).ToString("dd-MMM-yyyy HH:mm:ss.fff");                    
                }
                else
                {
                    lblFrameStart.Text = "N/A";
                    lblFrameExposure.Text = "N/A";
                    lblFrameEnd.Text = "N/A";
                }

                lblFrameSystemTime.Text = frameInfo.SystemTimestamp.ToString("dd-MMM-yyyy HH:mm:ss.fff");
                lblFrameId.Text = frameId.ToString();
                lblFrameLayout.Text = frameInfo.ImageLayoutId.ToString();
                lblDataBlockSize.Text = frameInfo.RawDataBlockSize.ToString() + " bytes";

                lblFrameGain.Text = frameInfo.Gain.ToString("0.00");
                lblFrameGamma.Text = frameInfo.Gamma.ToString("0.000");
                lblFrameOffset.Text = frameInfo.Offset.ToString("0.00");

                lblFrameNumSatellites.Text = frameInfo.GPSTrackedSattelites.ToString();
                lblFrameAlmanacStatus.Text = frameInfo.GPSAlmanacStatus.ToString();
                lblFrameAlmanacOffset.Text = frameInfo.GPSAlmanacOffset.ToString();

                foreach (string key in frameInfo.Status.Keys)
                {
                    var item = lvFrameStatusData.Items.Add(key);
                    item.SubItems.Add(Convert.ToString(frameInfo.Status[key]));
                }
            }
        }

        private void UpdateSelectedStreamControls()
        {
            if (rbMainStrToAVI.Checked)
            {
                nudAviFirstFrame.Maximum = Math.Max(0, (int) m_AdvFile.MainSteamInfo.FrameCount - 1);
                nudAviFirstFrame.Value = 0;
                nudAviLastFrame.Maximum = Math.Max(0, (int) m_AdvFile.MainSteamInfo.FrameCount - 1);
                nudAviLastFrame.Value = nudAviFirstFrame.Maximum;
            }
            else if (rbCalibrStrToAVI.Checked)
            {
                nudAviFirstFrame.Maximum = Math.Max(0, (int)m_AdvFile.CalibrationSteamInfo.FrameCount - 1);
                nudAviFirstFrame.Value = 0;
                nudAviLastFrame.Maximum = Math.Max(0, (int)m_AdvFile.CalibrationSteamInfo.FrameCount - 1);
                nudAviLastFrame.Value = nudAviFirstFrame.Maximum;             
            }

            if (rbMainStrToCSV.Checked)
            {
                nudCsvFirstFrame.Maximum = Math.Max(0, (int) m_AdvFile.MainSteamInfo.FrameCount - 1);
                nudCsvFirstFrame.Value = 0;
                nudCsvLastFrame.Maximum = Math.Max(0, (int) m_AdvFile.MainSteamInfo.FrameCount - 1);
                nudCsvLastFrame.Value = nudCsvFirstFrame.Maximum;
            }
            else if (rbCalibStrToCSV.Checked)
            {
                nudCsvFirstFrame.Maximum = Math.Max(0, (int)m_AdvFile.CalibrationSteamInfo.FrameCount - 1);
                nudCsvFirstFrame.Value = 0;
                nudCsvLastFrame.Maximum = Math.Max(0, (int)m_AdvFile.CalibrationSteamInfo.FrameCount - 1);
                nudCsvLastFrame.Value = nudCsvFirstFrame.Maximum;
            }
        }

        private void SelectedStreamRBChanged(object sender, EventArgs e)
        {
            UpdateSelectedStreamControls();
        }

        private void btnCropADV_Click(object sender, EventArgs e)
        {
            if (nudCropMainLastFrame.Value < nudCropMainFirstFrame.Value)
            {
                MessageBox.Show(
                    this,
                    "The MAIN stream last frame cannot be 'before' the first frame.",
                    "Tangra",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                nudCropMainLastFrame.Focus();
                return;
            }

            if (nudCropCalibrLastFrame.Value < nudCropCalibrFirstFrame.Value)
            {
                MessageBox.Show(
                    this,
                    "The CALIBRATION stream last frame cannot be 'before' the first frame.",
                    "Tangra",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                nudCropCalibrLastFrame.Focus();
                return;
            }

            if (Path.GetExtension(m_FileName).Equals(".adv", StringComparison.InvariantCultureIgnoreCase))
                saveFileDialog.Filter = "ADV Files (*.adv)|*.adv";
            else
                saveFileDialog.Filter = "All Files (*.*)|*.*";

            if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                UsageStats.Instance.FSTFCropInvoked++;
                UsageStats.Instance.Save();

                ThreadPool.QueueUserWorkItem(
                    new WaitCallback(CropFileWorker),
                    new Tuple<string, int, int, int, int>(
                        saveFileDialog.FileName, 
                        (int)nudCropMainFirstFrame.Value, 
                        (int)nudCropMainLastFrame.Value,
                        (int)nudCropCalibrFirstFrame.Value,
                        (int)nudCropCalibrLastFrame.Value));
            }
        }

        private void btnSaveAsAVI_Click(object sender, EventArgs e)
        {
#if WIN32
            if (nudAviLastFrame.Value < nudAviFirstFrame.Value)
            {
                MessageBox.Show(
                    this,
                    "The last frame cannot be 'before' the first frame.",
                    "Tangra",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                nudAviLastFrame.Focus();
                return;
            }

            if (cbxSpecifyCodec.Checked)
            {
                MessageBox.Show(
                    this,
                    "Please note than some of the codecs on the system may not be working correctly with this conversion to AVI by Tangra.",
                    "Tangra",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }

            if (saveAviFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                double msPerFrame = cbxFrameRate.SelectedIndex == 0 ? 40 : 33.37;
                double addedGamma = 1;
                if (cbxAddedGamma.SelectedIndex == 1)
                    addedGamma = 0.45;
                else if (cbxAddedGamma.SelectedIndex == 2)
                    addedGamma = 0.35;
                else if (cbxAddedGamma.SelectedIndex == 3)
                    addedGamma = 1 / 0.45;
                else if (cbxAddedGamma.SelectedIndex == 4)
                    addedGamma = 1 / 0.35;

                UsageStats.Instance.FSTSToAVIInvoked++;
                UsageStats.Instance.Save();

                ThreadPool.QueueUserWorkItem(
                    new WaitCallback(SaveAsAviFileWorker),
                    new Tuple<string, int, int, bool, bool, double, double, Tuple<AdvToAviConverter>>(
                        saveAviFileDialog.FileName,
                        (int)nudAviFirstFrame.Value,
                        (int)nudAviLastFrame.Value,
                        cbxSpecifyCodec.Checked,
                        !rbMainStrToAVI.Checked,
                        msPerFrame,
                        addedGamma,
                        new Tuple<AdvToAviConverter>(AdvToAviConverter.VideoForWindowsAviSaver)));
            }
#endif

        }

        private void btnSaveAsCSV_Click(object sender, EventArgs e)
        {
            if (nudCsvLastFrame.Value < nudCsvFirstFrame.Value)
            {
                MessageBox.Show(
                    this,
                    "The last frame cannot be 'before' the first frame.",
                    "Tangra",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                nudCsvLastFrame.Focus();
                return;
            }

            saveFileDialog.Filter = "CSV Files (*.csv)|*.csv|All Files (*.*)|*.*";
            saveFileDialog.FileName = Path.ChangeExtension(m_FileName, "csv");

            if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                UsageStats.Instance.FSTSToCSVInvoked++;
                UsageStats.Instance.Save();

                ThreadPool.QueueUserWorkItem(new WaitCallback(ExportToCSVWorker), 
                    new Tuple<string, int, int, bool>(
                        saveFileDialog.FileName, 
                        (int)nudCsvFirstFrame.Value, 
                        (int)nudCsvLastFrame.Value,
                        rbMainStrToCSV.Checked));
            }
        }

        private delegate void UpdateUIDelegate(int pbarId, int percent, bool show);

        private void UpdateUI(int pbarId, int percent, bool show)
        {
            ProgressBar pbar;
            if (pbarId == 1) pbar = pbar1;
            else if (pbarId == 2) pbar = pbar2;
            else if (pbarId == 3) pbar = pbar3;
            else pbar = pbar1;

            pbar.Value = percent;

            if (show && !pbar.Visible)
            {
                pbar.Visible = true;
                pnlCropChooseFrames.Enabled = false;
                pnlToAviConfig.Enabled = false;
                pnlToCSVConfig.Enabled = false;
            }
            else if (!show && pbar.Visible)
            {
                pbar.Visible = false;
                pnlCropChooseFrames.Enabled = true;
                pnlToAviConfig.Enabled = true;
                pnlToCSVConfig.Enabled = true;
            }

            pbar.Update();

            Update();
            Application.DoEvents();
        }

        private void InvokeUpdateUI(int pbarId, int percentDone, bool show)
        {
            try
            {
                Invoke(new UpdateUIDelegate(UpdateUI), new object[] { pbarId, percentDone, show });
            }
            catch (InvalidOperationException)
            { }
        }

        private void CropFileWorker(object state)
        {
            Tuple<string, int, int, int, int> cropFileCfg = (Tuple<string, int, int, int, int>)state;

            InvokeUpdateUI(1, 0, true);

            try
            {
                CropAdvFile(
                    cropFileCfg.Item1,
                    cropFileCfg.Item2,
                    cropFileCfg.Item3,
                    cropFileCfg.Item4,
                    cropFileCfg.Item5,
                    delegate(int percentDone, int framesFound, bool show)
                    {
                        InvokeUpdateUI(1, percentDone, show);
                    });
            }
            finally
            {
                InvokeUpdateUI(1, 100, false);
            }
        }

        private void CopyRawBytes(BinaryReader reader, long offset, long length, BinaryWriter writer)
        {
            reader.BaseStream.Seek(offset, SeekOrigin.Begin);

            for (int i = 0; i < length; i++)
            {
                byte bt = reader.ReadByte();
                writer.Write(bt);
            }
        }

        private const int FRAME_MAGIC_LENGTH = 4;

        private void CropAdvFile(string fileName, int firstMainFrame, int lastMainFrame, int firstCalibrationFrame, int lastCalibrationFrame, UpdateUIDelegate progressCallback)
        {
            using (var fsr = new FileStream(m_FileName, FileMode.Open, FileAccess.Read))
            using (var reader = new BinaryReader(fsr))
            using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            using (var writer = new BinaryWriter(fs))
            {
                AdvIndexEntry firstEntry;
                if (m_AdvFile.MainIndex.Count > 0)
                {
                    firstEntry = m_AdvFile.MainIndex[0];
                    if (m_AdvFile.CalibrationIndex.Count > 0 && m_AdvFile.CalibrationIndex[0].FrameOffset < firstEntry.FrameOffset)
                        firstEntry = m_AdvFile.CalibrationIndex[0];
                }
                else
                    firstEntry = m_AdvFile.CalibrationIndex[0];

                CopyRawBytes(reader, 0, firstEntry.FrameOffset, writer);

                progressCallback(5, 0, true);

                int MainStreamProgressScale = 5 + 85 * (lastMainFrame - firstMainFrame) / (lastMainFrame - firstMainFrame + lastCalibrationFrame - firstCalibrationFrame);
                int CalibrationStreamProgressScale = 5 + 85 * (lastCalibrationFrame - firstCalibrationFrame) / (lastMainFrame - firstMainFrame + lastCalibrationFrame - firstCalibrationFrame);

                var newMainIndex = new List<AdvIndexEntry>();
                var newCalibrationIndex = new List<AdvIndexEntry>();

                #region Copy Main Frames

                if (lastMainFrame > firstMainFrame)
                {
                    AdvIndexEntry firstEntryToCopy = m_AdvFile.MainIndex[firstMainFrame];
                    long zeroTicks = firstEntryToCopy.ElapsedTicks;

                    for (int i = firstMainFrame; i <= lastMainFrame; i++)
                    {
                        AdvIndexEntry frameToCopy = m_AdvFile.MainIndex[i];

                        var copiedFrame = new AdvIndexEntry()
                        {
                            FrameOffset = writer.BaseStream.Position,
                            BytesCount = frameToCopy.BytesCount
                        };

                        CopyRawBytes(reader, frameToCopy.FrameOffset, frameToCopy.BytesCount + FRAME_MAGIC_LENGTH, writer);

                        copiedFrame.ElapsedTicks = frameToCopy.ElapsedTicks - zeroTicks;
                        newMainIndex.Add(copiedFrame);

                        int percDone = (int)Math.Min(90, MainStreamProgressScale * (i - firstMainFrame) * 1.0 / (lastMainFrame - firstMainFrame + 1));
                        progressCallback(5 + percDone, 0, true);
                    }
                }
                #endregion

                #region Copy Calibration Frames

                if (lastCalibrationFrame > firstCalibrationFrame)
                {
                    AdvIndexEntry firstEntryToCopy = m_AdvFile.CalibrationIndex[firstCalibrationFrame];
                    long zeroTicks = firstEntryToCopy.ElapsedTicks;

                    for (int i = firstCalibrationFrame; i <= lastCalibrationFrame; i++)
                    {
                        AdvIndexEntry frameToCopy = m_AdvFile.CalibrationIndex[i];

                        var copiedFrame = new AdvIndexEntry()
                        {
                            FrameOffset = writer.BaseStream.Position,
                            BytesCount = frameToCopy.BytesCount
                        };

                        CopyRawBytes(reader, frameToCopy.FrameOffset, frameToCopy.BytesCount + FRAME_MAGIC_LENGTH, writer);

                        copiedFrame.ElapsedTicks = frameToCopy.ElapsedTicks - zeroTicks;
                        newCalibrationIndex.Add(copiedFrame);

                        int percDone = (int)Math.Min(90, CalibrationStreamProgressScale * (i - firstCalibrationFrame) * 1.0 / (lastCalibrationFrame - firstCalibrationFrame + 1));
                        progressCallback(5 + percDone, 0, true);
                    }
                }
                #endregion

                long indexTableOffset = writer.BaseStream.Position;

                progressCallback(95, 0, true);

                // Save the new INDEX
                writer.Write((byte)2 /*Index Table Format Version*/);
                writer.Write((int)9 /* Start Offset*/);
                writer.Write((int)((newMainIndex.Count + newCalibrationIndex.Count) * 20 + 10) /*Table Size */);
                writer.Write((uint)newMainIndex.Count);
                foreach (AdvIndexEntry newIndexEntry in newMainIndex)
                {
                    writer.Write((long)newIndexEntry.ElapsedTicks);
                    writer.Write((long)newIndexEntry.FrameOffset);
                    writer.Write((int)newIndexEntry.BytesCount);
                }   
                writer.Write((uint)newCalibrationIndex.Count);
                foreach (AdvIndexEntry newIndexEntry in newCalibrationIndex)
                {
                    writer.Write((long)newIndexEntry.ElapsedTicks);
                    writer.Write((long)newIndexEntry.FrameOffset);
                    writer.Write((int)newIndexEntry.BytesCount);
                }

                long userMetadataTablePosition = writer.BaseStream.Position;

                // Read userMetadataTableOffset from source
                reader.BaseStream.Seek(25, SeekOrigin.Begin);
                long userMetadataTableOffset = reader.ReadInt64();

                if (fsr.Length > userMetadataTableOffset)
                {
                    CopyRawBytes(reader, userMetadataTableOffset, fsr.Length - userMetadataTableOffset, writer);
                }

                // Update new Main Frames Count
                writer.BaseStream.Seek(40, SeekOrigin.Begin);
                writer.Write((uint)newMainIndex.Count);

                // Update new Calibration Frames Count
                writer.BaseStream.Seek(77, SeekOrigin.Begin);
                writer.Write((uint)newCalibrationIndex.Count);

                // Update new index table position
                writer.BaseStream.Seek(9, SeekOrigin.Begin);
                writer.Write(indexTableOffset);

                // Update new userMetadataTable position
                writer.BaseStream.Seek(25, SeekOrigin.Begin);
                writer.Write(userMetadataTablePosition);

                writer.BaseStream.Flush();

                progressCallback(100, 0, true);
            }
        }

#if WIN32
        private void SaveAsAviFileWorker(object state)
        {
            Tuple<string, int, int, bool, bool, double, double, Tuple<AdvToAviConverter>> cropFileCfg = (Tuple<string, int, int, bool, bool, double, double, Tuple<AdvToAviConverter>>)state;

            InvokeUpdateUI(2, 0, true);

            try
            {
                SaveAsAviFile(
                    cropFileCfg.Item1,
                    cropFileCfg.Item2,
                    cropFileCfg.Item3,
                    cropFileCfg.Rest.Item1,
                    cropFileCfg.Item4,
                    cropFileCfg.Item5,
                    cropFileCfg.Item6,
                    cropFileCfg.Item7,
                    delegate(int percentDone, int framesFound)
                    {
                        InvokeUpdateUI(2, percentDone, true);
                    });
            }
            finally
            {
                InvokeUpdateUI(2, 100, false);
            }
        }
#endif

        internal delegate void OnProgressDelegate(int percentDone, int argument);

        internal delegate DialogResult MessageBoxDelegate(string message, MessageBoxButtons buttons, MessageBoxIcon icon);

        private DialogResult InvokeMessageBox(string message, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            try
            {
                return (DialogResult)Invoke(new MessageBoxDelegate((m, b, i) =>
                {
                    return MessageBox.Show(this, m, "Tangra 3", b, i);
                }), new object[] { message, buttons, icon });
            }
            catch (InvalidOperationException)
            { }

            return DialogResult.Abort;
        }

        internal bool SaveAsAviFile(string fileName, int firstFrame, int lastFrame, AdvToAviConverter converter, bool tryCodec, bool isCalibrationStream, double msPerFrame, double addedGamma, OnProgressDelegate progressCallback)
        {
            IAviSaver saver = AdvToAviConverterFactory.CreateConverter(converter);

            saver.CloseAviFile();
            if (!saver.StartNewAviFile(fileName, (int)m_AdvFile.Width, (int)m_AdvFile.Height, 8, 25, tryCodec))
            {
                progressCallback(100, 0);
                return false;
            }
            try
            {
                int aviFrameNo = 0;
                AdvIndexEntry firstFrameIdx = isCalibrationStream ? m_AdvFile.CalibrationIndex[firstFrame] : m_AdvFile.MainIndex[firstFrame];
                double ticksToMSFactor = 1000.0 / (isCalibrationStream ? m_AdvFile.CalibrationSteamInfo.ClockFrequency : m_AdvFile.MainSteamInfo.ClockFrequency);
                double startingTimeMilliseconds = firstFrameIdx.ElapsedTicks * ticksToMSFactor;

                if (m_AdvFile.MainIndex[lastFrame].ElapsedTicks != 0)
                {
                    // Sampling can be done as we have sufficient timing information
                }
                else
                {
                    InvokeMessageBox(
                        "There is insufficient timing information in this file to convert it to AVI. This could be caused by an old file format or trying to make an AVI from a single frame.",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                    return false;
                }

                if (InvokeMessageBox(
                    "Please note that the AVI export is doing resampling of the original video which will typically cause frames to duplicated and/or dropped.\r\n\r\nThis export function is meant to be used for video streaming (i.e. sharing the video for viewing on the Internet) and should not be used to convert the video to another format for measuring in another software. If you want to measure the video in another software either measure it directly as ADV/AAV file (if supported) or export it to a FITS file sequence from the main file menu and measure the FITS images.\r\n\r\nDo you wish to continue?",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning) != DialogResult.Yes)
                    return false;

                progressCallback(5, 0);

                for (int i = firstFrame; i <= lastFrame; i++)
                {
                    AdvIndexEntry frame = isCalibrationStream ? m_AdvFile.CalibrationIndex[i] : m_AdvFile.MainIndex[i];

                    AdvFrameInfo frameInfo = null;
                    uint[] pixels = null;

                    if (isCalibrationStream)
                        pixels = m_AdvFile.GetCalibrationFramePixels((uint)i, out frameInfo);                
                    else
                        pixels = m_AdvFile.GetMainFramePixels((uint)i, out frameInfo); 

                    using (Pixelmap pixmap = CreatePixelmap(pixels))
                    {
                        int lastRepeatedAviFrameNo = 0;

                        if (frame.ElapsedTicks != 0)
                            lastRepeatedAviFrameNo = (int)Math.Round((frame.ElapsedTicks * ticksToMSFactor - startingTimeMilliseconds) / msPerFrame);

                        while (aviFrameNo < lastRepeatedAviFrameNo)
                        {
                            if (!saver.AddAviVideoFrame(pixmap, addedGamma, m_AdvFile.MaxPixelValue))
                            {
                                progressCallback(100, 0);
                                return false;
                            }
                            aviFrameNo++;
                        }
                    }

                    int percDone = (int)Math.Min(90, 90 * (i - firstFrame) * 1.0 / (lastFrame - firstFrame + 1));
                    progressCallback(5 + percDone, 0);
                }
            }
            finally
            {
                saver.CloseAviFile();
                progressCallback(100, 0);
            }

            return false;
        }

        private void ExportToCSVWorker(object state)
        {
            Tuple<string, int, int, bool> cropFileCfg = (Tuple<string, int, int, bool>)state;

            InvokeUpdateUI(3, 0, true);

            try
            {
                ExportStatusSectionToCSV(
                    cropFileCfg.Item1,
                    cropFileCfg.Item2,
                    cropFileCfg.Item3,
                    !cropFileCfg.Item4,
                    delegate(int percentDone, int framesFound)
                    {
                        InvokeUpdateUI(3, percentDone, true);
                    });
            }
            finally
            {
                InvokeUpdateUI(3, 100, false);
            }	           
        }
        
        internal void ExportStatusSectionToCSV(string fileName, int firstFrame, int lastFrame, bool isCalibrationFrame, OnProgressDelegate progressCallback)
        {
            progressCallback(5, 0);

            bool headerAppended = false;

            using (FileStream fsOutput = new FileStream(fileName, FileMode.CreateNew, FileAccess.Write))
            using (TextWriter writer = new StreamWriter(fsOutput))
            {
                for (int i = firstFrame; i <= lastFrame; i++)
                {
                    AdvFrameInfo frameInfo;
                    if (isCalibrationFrame)
                        m_AdvFile.GetCalibrationFramePixels((uint)i, out frameInfo);
                    else
                        m_AdvFile.GetMainFramePixels((uint)i, out frameInfo);

                    string headerRow;
                    string nextRow = StatusDataToCsvRow(frameInfo, i, out headerRow);
                    if (!headerAppended)
                    {
                        writer.WriteLine(headerRow);
                        headerAppended = true;
                    }
                    writer.WriteLine(nextRow);

                    int percDone = (int)Math.Min(90, 90 * (i - firstFrame) * 1.0 / (lastFrame - firstFrame + 1));
                    progressCallback(5 + percDone, 0);
                }

                progressCallback(95, 0);

                writer.Flush();
            }
        }
		private string StatusDataToCsvRow(AdvFrameInfo frameInfo, int frameNo, out string headerRow)
		{
			var output = new StringBuilder();
			output.AppendFormat("\"{0}\"", frameNo);
            output.AppendFormat(",\"{0}\"", frameInfo.UtcStartExposureTimeStamp.ToString("dd-MMM-yyyy HH:mm:ss.fff"));
            output.AppendFormat(",\"{0}\"", frameInfo.UtcStartExposureTimeStamp.AddMilliseconds(frameInfo.UtcExposureMilliseconds).ToString("dd-MMM-yyyy HH:mm:ss.fff"));

			var header = new StringBuilder();
            header.Append("FrameNo,StartTimestamp,EndTimestamp");

		    foreach (var definition in m_AdvFile.StatusTagDefinitions)
		    {
		        object tagValueObj;
		        string tagValue = string.Empty;
                if (frameInfo.Status.TryGetValue(definition.Item1, out tagValueObj))
                {
                    tagValue = Convert.ToString(tagValueObj);
                }

                output.AppendFormat(",\"{0}\"", tagValue.Replace("\"", "\"\""));
				header.AppendFormat(",{0}", definition.Item1);
		    }            

			headerRow = header.ToString();
			return output.ToString();
		}
    }
}
