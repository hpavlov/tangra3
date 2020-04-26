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
        public frmSerTimestampExposure(double medianExposure, double? oneSigma)
            : this()
        {
            nudExposureMs.Value = (decimal)medianExposure;
            cbxTimeReference.SelectedIndex = 0;
            pnlExposureJitter.Visible = oneSigma.HasValue;
            if (oneSigma.HasValue)
            {
                lblJitter.Text = string.Format("{0}", Math.Round(oneSigma.Value, 2));
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

        private void cbxTimeReference_SelectedIndexChanged(object sender, EventArgs e)
        {
            pnlExposure.Visible = cbxTimeReference.SelectedIndex != 1;
        }
    }
}
