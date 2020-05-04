using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tangra.Controller;
using Tangra.Helpers;
using Tangra.Model.Config;
using Tangra.Model.Helpers;
using Tangra.VideoOperations.LightCurves.Helpers;
using Tangra.VideoOperations.LightCurves.InfoForms;

namespace Tangra.VideoOperations.LightCurves
{
    public partial class frmAcquisitionDelayChooser : Form
    {
        private VideoController m_VideoController;

        public frmAcquisitionDelayChooser()
        {
            InitializeComponent();
        }

        public frmAcquisitionDelayChooser(VideoController videoController)
            : this()
        {
            m_VideoController = videoController;

            var cameraSystems = Enum.GetValues(typeof(TangraConfig.CameraSystem)).Cast<TangraConfig.CameraSystem>().ToList();
            for (int i = 1; i < cameraSystems.Count; i++)
            {
                cbxCameraSystem.Items.Add(cameraSystems[i].GetEnumValDisplayName());
            }
            cbxCameraSystem.Items.Add(cameraSystems[0].GetEnumValDisplayName());

            var timestampingSystems = Enum.GetValues(typeof(TangraConfig.TimestampingSystem)).Cast<TangraConfig.TimestampingSystem>().ToList();
            for (int i = 1; i < timestampingSystems.Count; i++)
            {
                cbxTimestamping.Items.Add(timestampingSystems[i].GetEnumValDisplayName());
            }
            cbxTimestamping.Items.Add(timestampingSystems[0].GetEnumValDisplayName());

            if (TangraConfig.Settings.LastUsed.TimingCorrectionsCameraSystem != null)
            {
                if (TangraConfig.Settings.LastUsed.TimingCorrectionsCameraSystem.Value == TangraConfig.CameraSystem.Other)
                {
                    cbxCameraSystem.SelectedIndex = cbxCameraSystem.Items.Count - 1;
                }
                else
                {
                    cbxCameraSystem.SelectedIndex = ((int)TangraConfig.Settings.LastUsed.TimingCorrectionsCameraSystem.Value) - 1;
                }
            }
            else
            {
                cbxCameraSystem.SelectedIndex = -1;
            }

            if (TangraConfig.Settings.LastUsed.TimingCorrectionsTimestampingSystem != null)
            {
                if (TangraConfig.Settings.LastUsed.TimingCorrectionsTimestampingSystem.Value == TangraConfig.TimestampingSystem.Other)
                {
                    cbxTimestamping.SelectedIndex = cbxTimestamping.Items.Count - 1;
                }
                else
                {
                    cbxTimestamping.SelectedIndex = ((int)TangraConfig.Settings.LastUsed.TimingCorrectionsTimestampingSystem.Value) - 1;
                }
            }
            else
            {
                cbxTimestamping.SelectedIndex = -1;
            }

            tbxCameraModel.Text = TangraConfig.Settings.LastUsed.TimingCorrectionsCameraName;
            if (TangraConfig.Settings.LastUsed.TimingCorrectionsAquisitionDelay.HasValue)
            {
                tbxAcquisitionDelay.Text = TangraConfig.Settings.LastUsed.TimingCorrectionsAquisitionDelay.Value.ToString(CultureInfo.CurrentCulture);
            }

            cbxEnterRefertenceTimeOffset.Checked = TangraConfig.Settings.LastUsed.TimingCorrectionsEnteringRefertenceTimeOffset;

            if (videoController.AstroVideoCameraModel != null && videoController.AstroVideoCameraModel.StartsWith("DVTI"))
            {
                cbxCameraSystem.SelectedIndex = ((int)TangraConfig.CameraSystem.Dvti) - 1;
            }
        }

        internal bool TimingCorrectionsRequired { get; private set; }

        internal double? AcquisitionDelayMs { get; private set; }

        internal double? ReferenceTimeToUtcOffsetMs { get; private set; }

        internal string CameraModel { get; private set; }

        Regex m_Validator = new Regex(@"^[\+\-]?\d+((\.|,)\d+)?$");

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (cbxCameraSystem.SelectedIndex == -1)
            {
                MessageBox.Show(this, "Please select the camera system used in this video recording.", "Tangra", MessageBoxButtons.OK, MessageBoxIcon.Error);
                cbxCameraSystem.Focus();
                return;
            }

            TangraConfig.CameraSystem cameraSystem = SelectedCameraSystem();
            if (cameraSystem != TangraConfig.CameraSystem.Other)
            {
                CameraModel = cameraSystem.GetEnumDescription();
            }
            else 
            {
                if (string.IsNullOrWhiteSpace(tbxCameraModel.Text))
                {
                    MessageBox.Show(this, "Please specify the camera model.", "Tangra", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    tbxCameraModel.Focus();
                    return;
                }

                CameraModel = tbxCameraModel.Text.Trim();
            }

            if (cbxTimestamping.SelectedIndex == -1)
            {
                MessageBox.Show(this, "Please select the timestamping mode used in this video recording.", "Tangra", MessageBoxButtons.OK, MessageBoxIcon.Error);
                cbxTimestamping.Focus();
                return;
            }

            TangraConfig.Settings.LastUsed.TimingCorrectionsCameraSystem = cameraSystem;
            TangraConfig.Settings.LastUsed.TimingCorrectionsTimestampingSystem = SelectedTimestampingSystem();
            TangraConfig.Settings.LastUsed.TimingCorrectionsCameraName = tbxCameraModel.Text;


            if (pnlTimingCorrections.Visible)
            {
                if (!m_Validator.IsMatch(tbxAcquisitionDelay.Text.Trim()))
                {
                    MessageBox.Show(this, "Please enter a valid acquisition delay value.", "Tangra",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    tbxAcquisitionDelay.Focus();
                    return;
                }

                var userInput = double.Parse(tbxAcquisitionDelay.Text.Trim().Replace(',', '.'), NumberStyles.Float,
                    CultureInfo.InvariantCulture);
                if (userInput < 0)
                {
                    MessageBox.Show(this,
                        "By definition the acquisition delay cannot be negative. Please enter a correct value.",
                        "Tangra", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    tbxAcquisitionDelay.Focus();
                    return;
                }

                AcquisitionDelayMs = userInput;
                TangraConfig.Settings.LastUsed.TimingCorrectionsAquisitionDelay = AcquisitionDelayMs;

                if (cbxEnterRefertenceTimeOffset.Checked)
                {
                    if (!m_Validator.IsMatch(tbxReferenceTimeOffset.Text.Trim()))
                    {
                        MessageBox.Show(this, "Please enter a valid refernce time offset value.", "Tangra",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        tbxReferenceTimeOffset.Focus();
                        return;
                    }

                    ReferenceTimeToUtcOffsetMs = double.Parse(tbxReferenceTimeOffset.Text.Trim().Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture);
                }
                else
                {
                    ReferenceTimeToUtcOffsetMs = null;
                }

                TangraConfig.Settings.LastUsed.TimingCorrectionsEnteringRefertenceTimeOffset = cbxEnterRefertenceTimeOffset.Checked;

                TimingCorrectionsRequired = true;
            }
            else
            {
                TimingCorrectionsRequired = false;
            }

            TangraConfig.Settings.Save();

            DialogResult = DialogResult.OK;
            Close();
        }

        private void cbxCameraSystem_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxCameraSystem.SelectedIndex > -1 && SelectedCameraSystem().ProvidesUtcTime())
            {
                cbxTimestamping.SelectedIndex = 0;
                cbxTimestamping.Enabled = false;
            }
            else
            {
                cbxTimestamping.Enabled = true;
                cbxTimestamping.SelectedIndex = -1;
            }

            UpdateControlState();
        }

        private void cbxTimestamping_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateControlState();
        }

        private void UpdateControlState()
        { 
            TangraConfig.CameraSystem? selectedCameraSystem = cbxCameraSystem.SelectedIndex > -1 ? SelectedCameraSystem() : (TangraConfig.CameraSystem?)null;
            tbxCameraModel.Visible = selectedCameraSystem != null && selectedCameraSystem.Value.IsGenericCameraSystem();
            pnlTimestamping.Visible = cbxCameraSystem.SelectedIndex > -1;

            TangraConfig.TimestampingSystem? selectedTimestampingSystem = cbxTimestamping.SelectedIndex > -1 ? SelectedTimestampingSystem() : (TangraConfig.TimestampingSystem?)null;

            pnlTimingCorrections.Visible = selectedTimestampingSystem != null && selectedTimestampingSystem.Value.RequiresTimingCorrections();
            pnlReferenceTimeOffset.Enabled = cbxEnterRefertenceTimeOffset.Checked;

            if (selectedCameraSystem == null)
            {
                lblCameraSystem.Text = null;
                llCameraSystemLink.Text = null;
            }
            else
            {
                var att = selectedCameraSystem.Value.GetEnumValDetails();
                lblCameraSystem.Text = att != null ? att.Description : null;
                llCameraSystemLink.Text = att != null ? att.Url : null;
            }

            if (selectedTimestampingSystem == null)
            {
                lblTimestamping.Text = null;
            }
            else
            {
                var att = selectedTimestampingSystem.Value.GetEnumValDetails();
                lblTimestamping.Text = att != null ? att.Description : null;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var frm = new frmReferenceTimeOffsetHelp();
            frm.ShowDialog(this);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var frm = new frmAcquisitionDelayHelp();
            frm.ShowDialog(this);
        }

        private void cbxEnterRefertenceTimeOffset_CheckedChanged(object sender, EventArgs e)
        {
            UpdateControlState();
        }

        private TangraConfig.TimestampingSystem SelectedTimestampingSystem()
        {
            if (cbxTimestamping.SelectedIndex != cbxTimestamping.Items.Count - 1)
            {
                return (TangraConfig.TimestampingSystem)(cbxTimestamping.SelectedIndex + 1);
            }
            return TangraConfig.TimestampingSystem.Other;
        }

        private TangraConfig.CameraSystem SelectedCameraSystem()
        {
            if (cbxCameraSystem.SelectedIndex != cbxCameraSystem.Items.Count - 1)
            {
                return (TangraConfig.CameraSystem)(cbxCameraSystem.SelectedIndex + 1);
            }
            return TangraConfig.CameraSystem.Other;
        }

        private void llCameraSystemLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ShellHelper.OpenUrl(llCameraSystemLink.Text);
        }

        private void frmAcquisitionDelayChooser_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing ||
                e.CloseReason == CloseReason.FormOwnerClosing)
            {
                if (DialogResult != DialogResult.OK)
                {
                    if (MessageBox.Show(
                        "Are you sure you want to continue without specifying Aqcuisition Delays? Your timestamps and derived event times in AOTA may not be correct.",
                        "Tangra",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.No)
                    {
                        e.Cancel = true;
                    }
                }
            }
        }

        private void btnSupportedFileFormats_Click(object sender, EventArgs e)
        {
            var frm = new frmSupportedFileFormats();
            frm.ShowDialog(this);
        }
    }
}
