using System;
using System.Windows.Forms;
using Tangra.Controller;
using Tangra.Video;

namespace Tangra.VideoTools
{
    public partial class frmCorrectInterlacedDefects : Form
    {
        private ReInterlacingVideoStream m_ReInterlacedStream;
        private VideoController m_VideoController;

        public frmCorrectInterlacedDefects()
        {
            InitializeComponent();
        }

        public frmCorrectInterlacedDefects(VideoController videoController)
            : this()
        {
            m_VideoController = videoController;
            m_ReInterlacedStream = videoController.FramePlayer.Video as ReInterlacingVideoStream;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            ReInterlaceMode mode = ReInterlaceMode.None;

            if (rbReInterlaceNon.Checked)
                mode = ReInterlaceMode.None;
            else if (rbReInterlaceSwapFields.Checked)
                mode = ReInterlaceMode.SwapFields;
            else if (rbReInterlaceShiftForward.Checked)
                mode = ReInterlaceMode.ShiftOneField;
            else if (rbReInterlaceShiftAndSwap.Checked)
                mode = ReInterlaceMode.SwapAndShiftOneField;

            if (m_ReInterlacedStream.ChangeReInterlaceMode(mode))
            {
                m_VideoController.RefreshCurrentFrame();
            }

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
