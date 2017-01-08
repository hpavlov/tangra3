using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.VideoOperations.Astrometry;

namespace Tangra.ImageTools
{
    public partial class frmIdentifiedStarAction : Form
    {
        private AstrometricState m_State;

        public frmIdentifiedStarAction()
        {
            InitializeComponent();
        }

        public frmIdentifiedStarAction(AstrometricState state)
            : this()
        {
            m_State = state;
            rbAttemptPlateSolve.Checked = m_State.ManuallyIdentifiedStars.Count >= 3;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (rbAttemptPlateSolve.Checked)
                DialogResult = DialogResult.OK;
            else
                DialogResult = DialogResult.Retry;

            Close();
        }
    }
}
