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
        public int NegPixCorrection { get; private set; }

        private int m_BZero;
        private int m_MinPixelValue;

        public ucNegativePixelsTreatment()
        {
            InitializeComponent();
        }

        public void Initialise(int bzero, short minPixelValue)
        {
            rbMinPixVal.Text = minPixelValue.ToString();
            m_BZero = bzero;
            m_MinPixelValue = minPixelValue;

            UpdateNegPixCorrection();
        }

        private void rbZero_CheckedChanged(object sender, EventArgs e)
        {
            UpdateNegPixCorrection();
        }

        private void UpdateNegPixCorrection()
        {
            if (rbZero.Checked)
                NegPixCorrection = 0;
            else
                NegPixCorrection = m_MinPixelValue;
        }
    }
}
