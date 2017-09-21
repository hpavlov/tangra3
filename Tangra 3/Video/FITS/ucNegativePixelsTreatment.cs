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
        private double m_MinPixelPercent;

        public ucNegativePixelsTreatment()
        {
            InitializeComponent();
        }

        public void Initialise(int bzero, short minPixelValue, uint maxPixelValue)
        {
            m_BZero = bzero;
            m_MinPixelValue = minPixelValue;
            m_MinPixelPercent = 100.0 * minPixelValue / (maxPixelValue - minPixelValue);

            rbMinPixVal.Text = string.Format("{0} ({1:0.0}%)", minPixelValue, m_MinPixelPercent);

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
