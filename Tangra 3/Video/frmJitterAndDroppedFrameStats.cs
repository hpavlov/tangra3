using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tangra.Video
{
    public partial class frmJitterAndDroppedFrameStats : Form
    {
        public frmJitterAndDroppedFrameStats(double medianExposure, double? oneSigma, int droppedFrames, double droppedFramesPercentage, bool hasTooManyDroppedFrames, int? nonUtcTimestamps = 0)
            : this()
        {
            lblExposure.Text = medianExposure.ToString("0.00");
            lblExposureMs.Left = lblExposure.Right + 5;

            gbExposureJitter.Visible = oneSigma.HasValue;
            if (oneSigma.HasValue)
            {
                if (hasTooManyDroppedFrames)
                {
                    lblJitter.Text = "N/A";
                    lblJitterMs.Left = lblJitter.Right + 2;
                    lblJitterMs.Visible = false;
                }
                else
                {
                    lblJitter.Text = string.Format("{0}", Math.Round(oneSigma.Value, 2));
                    lblJitterMs.Left = lblJitter.Right + 2;
                    lblJitterMs.Visible = true;
                }
            }

            if (droppedFrames > 0)
            {
                if (hasTooManyDroppedFrames)
                {
                    lblDroppedFrames.Text = string.Format("Dropped Frames: {0}+ (67+%)", droppedFrames);
                }
                else
                {
                    lblDroppedFrames.Text = string.Format("Dropped Frames: {0} ({1:0.0}%)", droppedFrames, droppedFramesPercentage);
                }

                lblDroppedFrames.Left = lblJitterMs.Right + 5;
                lblDroppedFrames.Visible = true;
            }
            else if (nonUtcTimestamps > 0)
            {
                lblDroppedFrames.Text = string.Format("Non UTC Timestamps: {0} ({1:0.0}%)", nonUtcTimestamps, nonUtcTimestamps);
                lblDroppedFrames.Left = lblJitterMs.Right + 5;
                lblDroppedFrames.Visible = true;
            }
            else
            {
                lblDroppedFrames.Visible = false;
            }
        }

        public frmJitterAndDroppedFrameStats()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
