using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MagnitudeFitter
{
    public partial class frmConfigureFit : Form
    {
        internal double FixedColourCoeff;

        public frmConfigureFit()
        {
            InitializeComponent();
        }

        private void frmConfigureFit_Load(object sender, EventArgs e)
        {
            if (double.IsNaN(FixedColourCoeff))
            {
                cbxFixedBV.Checked = false;
            }
            else
            {
                cbxFixedBV.Checked = true;
                numericUpDown1.Value = (decimal)FixedColourCoeff;
            }
        }

        private void frmConfigureFit_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (cbxFixedBV.Checked)
                FixedColourCoeff = (double) numericUpDown1.Value;
            else
                FixedColourCoeff = double.NaN;
        }
    }
}
