using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tangra.Model.Helpers;
using Tangra.Model.Video;

namespace Tangra.VideoOperations.LightCurves.InfoForms
{
    public partial class frmAnalogueSystemTimeCorrectionsInfo : Form
    {
        private TimeCorrectonsInfo info;

        public frmAnalogueSystemTimeCorrectionsInfo()
        {
            InitializeComponent();
        }

        public frmAnalogueSystemTimeCorrectionsInfo(TimeCorrectonsInfo info, string finalTimeStamp)
            : this()
        {
            this.info = info;

            lblSystem.Text = info.VideoFormatType == VideoFormatType.Analogue ? string.Format("Analogue ({0})", info.AnalogueVideoFormat) : "Digital";
            lblVideoFileType.Text = info.VideoFileType.ToString();
            lblCameraType.Text = !string.IsNullOrWhiteSpace(info.CameraName) ? info.CameraName : "Unknown";
            lblTimestampSource.Text = info.RawTimestampSource.GetEnumDescription();
            lblIntegrationRate.Text = info.VideoFileType != VideoFileFormat.AVI && info.IntegratedFrames != null ? string.Format("x{0} frames", info.IntegratedFrames) : "Unknown";
            lblDelaysApplied.Text = info.InstrumentalDelaySec != null ? "Yes" : "No";
            lblInstrumentalDelay.Text = info.InstrumentalDelaySec != null ? string.Format("{0:F3}", -1 * info.InstrumentalDelaySec).TrimEnd('0') + " sec" : "N/A";

            lblMidFrameTimestamp.Text = info.MidFrameTimestamp.ToString("HH:mm:ss.fff");
            lblRawFrameTimestamp.Text = info.RawFrameTimeStamp != null ? info.RawFrameTimeStamp.Value.ToString("HH:mm:ss.fff") : "N/A";
            lblFinalFrameTimestamp.Text = finalTimeStamp;
        }
    }
}
