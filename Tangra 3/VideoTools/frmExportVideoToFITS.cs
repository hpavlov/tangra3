using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Controller;

namespace Tangra.VideoTools
{
    public partial class frmExportVideoToFITS : Form
    {
        private VideoController m_VideoController;

        public frmExportVideoToFITS()
        {
            InitializeComponent();
        }

        public frmExportVideoToFITS(VideoController videoController)
            : this()
        {
            m_VideoController = videoController;

            nudFirstFrame.Minimum = m_VideoController.VideoFirstFrame;
            nudFirstFrame.Maximum = m_VideoController.VideoLastFrame - 1;
            nudFirstFrame.Value = nudFirstFrame.Minimum;

            nudLastFrame.Minimum = m_VideoController.VideoFirstFrame;
            nudLastFrame.Maximum = m_VideoController.VideoLastFrame - 1;
            nudLastFrame.Value = nudLastFrame.Maximum;
        }
    }
}
