using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Video;
using Tangra.VideoOperations.LightCurves.Helpers;

namespace Tangra.VideoOperations.LightCurves
{
    public partial class frmDefineBinning : Form
    {
        public frmDefineBinning()
        {
            InitializeComponent();
        }

        internal LCFile LCFile;

        private void btnIntegrationDetection_Click(object sender, EventArgs e)
        {
            nudNumFramesToBin.BackColor = SystemColors.Window;
            nudReferenceFrame.BackColor = SystemColors.Window;

            var provider = new LCFileImagePixelProvider(LCFile);

            var frm = new frmIntegrationDetection(provider, (int)LCFile.Header.MinFrame);
            frm.StartPosition = FormStartPosition.CenterParent;
            if (frm.ShowDialog(this) == DialogResult.OK &&
                frm.IntegratedFrames != null)
            {
                nudNumFramesToBin.Value = frm.IntegratedFrames.Interval;
                nudReferenceFrame.Value = frm.IntegratedFrames.StartingAtFrame;

                nudNumFramesToBin.BackColor = Color.Honeydew;
                nudReferenceFrame.BackColor = Color.Honeydew;
            }
        }

        private void frmDefineBinning_Load(object sender, EventArgs e)
        {
            btnIntegrationDetection.Enabled = LCFile.Header.GetVideoFileFormat() == VideoFileFormat.AVI;

            nudReferenceFrame.Maximum = LCFile.Header.MaxFrame;
            nudReferenceFrame.Minimum = LCFile.Header.MinFrame;
            nudReferenceFrame.Value = LCFile.Header.MinFrame;
        }
    }
}
