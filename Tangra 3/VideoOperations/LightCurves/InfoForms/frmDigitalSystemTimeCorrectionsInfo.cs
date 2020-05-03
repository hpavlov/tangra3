using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tangra.Model.Video;

namespace Tangra.VideoOperations.LightCurves.InfoForms
{
    public partial class frmDigitalSystemTimeCorrectionsInfo : Form
    {
        public frmDigitalSystemTimeCorrectionsInfo()
        {
            InitializeComponent();
        }

        public frmDigitalSystemTimeCorrectionsInfo(TimeCorrectonsInfo info, string finalTimeStamp)
            : this()
        {

            lblSystem.Text = info.VideoFormatType == VideoFormatType.Analogue ? "Analogue" : "Digital";
            switch (info.VideoFileType)
            {
                case VideoFileFormat.ADV:
                    lblSystem.Text += " (ADVS)";
                    break;
            }
            lblVideoFileType.Text = info.VideoFileType.ToString();
            lblCameraType.Text = info.CameraName ?? "Unknown";
            lblDelaysApplied.Text = info.NotAffectedByAcquisitionDelays ? "Not Required" : (info.AcquisitionDelaySec != null ? "Yes" : "No");
            lblAquisitionDelay.Text = info.NotAffectedByAcquisitionDelays ? "-" : (info.AcquisitionDelaySec != null ? string.Format("{0} sec", info.AcquisitionDelaySec) : (info.ReferenceTimeOffsetSec != null ? "-" : "N/A"));
            lblReferenceTimeOffset.Text = info.NotAffectedByAcquisitionDelays ? "-" : (info.ReferenceTimeOffsetSec != null ? string.Format("{0} sec", info.ReferenceTimeOffsetSec) : (info.AcquisitionDelaySec != null ? "-" : "N/A"));

            lblMidFrameTimestamp.Text = info.MidFrameTimestamp.ToString("HH:mm:ss.fff");
            lblFinalFrameTimestamp.Text = finalTimeStamp;
        }
    }
}
