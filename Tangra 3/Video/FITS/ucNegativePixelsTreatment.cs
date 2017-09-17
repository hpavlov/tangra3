using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Tangra.Video.FITS
{
    public partial class ucNegativePixelsTreatment : UserControl
    {
        public bool ZeroOut { get; private set; }

        public ucNegativePixelsTreatment()
        {
            InitializeComponent();
        }

        public void Initialise(uint bzero, uint minPixelValue)
        {
            nudBZero.Minimum = minPixelValue;
            nudBZero.Value = minPixelValue;

            if (bzero == 0)
                rbBZero.Checked = true;
            else
                rbZeroOut.Checked = true;
        }

        private void rbBZero_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
