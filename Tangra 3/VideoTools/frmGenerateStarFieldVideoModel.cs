using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Model.Config;
using Tangra.Model.Context;
using Tangra.VideoOperations.Astrometry;

namespace Tangra.VideoTools
{
    public partial class frmGenerateStarFieldVideoModel : Form
    {
        public frmGenerateStarFieldVideoModel()
        {
            InitializeComponent();

            cbxVideoFormat.SelectedIndex = 0;
            cbxAAVIntegration.SelectedIndex = 5;
            cbxPhotometricFilter.SelectedIndex = 2;
        }

        private void cbxVideoFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxVideoFormat.SelectedIndex == 1)
            {
                cbxAAVIntegration.SelectedIndex = 0;
                cbxAAVIntegration.Enabled = false;
            }
            else
                cbxAAVIntegration.Enabled = true;
        }
    }
}
