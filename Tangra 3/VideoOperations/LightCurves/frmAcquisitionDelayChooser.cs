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
using Tangra.Model.Config;
using Tangra.Model.Helpers;
using Tangra.VideoOperations.LightCurves.Helpers;

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

            TangraConfig.CameraSystem cameraSystem = TangraConfig.CameraSystem.Other;
            if (cbxCameraSystem.SelectedIndex != cbxCameraSystem.Items.Count - 1)
            {
                cameraSystem = (TangraConfig.CameraSystem) (cbxCameraSystem.SelectedIndex + 1);
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

            TangraConfig.TimestampingSystem timestampingSystem = TangraConfig.TimestampingSystem.Other;
            if (cbxTimestamping.SelectedIndex != cbxTimestamping.Items.Count - 1)
            {
                timestampingSystem = (TangraConfig.TimestampingSystem)(cbxTimestamping.SelectedIndex + 1);
            }

            TangraConfig.Settings.LastUsed.TimingCorrectionsCameraSystem = cameraSystem;
            TangraConfig.Settings.LastUsed.TimingCorrectionsTimestampingSystem = timestampingSystem;
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
            if (cbxCameraSystem.SelectedIndex >= 0 && cbxCameraSystem.SelectedIndex <= 2)
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
            tbxCameraModel.Visible = cbxCameraSystem.SelectedIndex == 3;
            pnlTimestamping.Visible = cbxCameraSystem.SelectedIndex > -1;

            pnlTimingCorrections.Visible = cbxTimestamping.SelectedIndex > 0;
            pnlReferenceTimeOffset.Enabled = cbxEnterRefertenceTimeOffset.Checked;
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
    }
}
