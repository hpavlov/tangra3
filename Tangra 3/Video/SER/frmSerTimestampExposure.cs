using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tangra.Video.SER
{
    public partial class frmSerTimestampExposure : Form
    {
        public frmSerTimestampExposure(double medianExposure, double? oneSigma, int droppedFrames, double droppedFramesPercentage)
            : this()
        {
            nudExposureMs.Value = (decimal)medianExposure;
            cbxTimeReference.SelectedIndex = 0;
            gbExposureJitter.Visible = oneSigma.HasValue;
            if (oneSigma.HasValue)
            {
                lblJitter.Text = string.Format("{0}", Math.Round(oneSigma.Value, 2));
                lblJitterMs.Left = lblJitter.Right + 2;

                if (droppedFrames > 0)
                {
                    lblDroppedFrames.Text = string.Format("Dropped Frames: {0} ({1:0.0}%)", droppedFrames, droppedFramesPercentage);
                    lblDroppedFrames.Left = lblJitterMs.Right + 5;
                    lblDroppedFrames.Visible = true;
                }
                else
                {
                    lblDroppedFrames.Visible = false;
                }
            }
        }

        public frmSerTimestampExposure()
        {
            InitializeComponent();
        }

        public double ConfirmedExposure { get; private set; }

        public SerTimeStampReference ConfirmedTimeReference { get; private set; }

        private void btnOK_Click(object sender, EventArgs e)
        {
            ConfirmedExposure = (double)nudExposureMs.Value;
            ConfirmedTimeReference = (SerTimeStampReference)cbxTimeReference.SelectedIndex;

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
