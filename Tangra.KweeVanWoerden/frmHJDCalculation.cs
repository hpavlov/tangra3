using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Tangra.KweeVanWoerden
{
    public partial class frmHJDCalculation : Form
    {
        internal double TimeOfMinimumJD;

        public frmHJDCalculation()
        {
            InitializeComponent();
        }

        private void btnCalculate_Click(object sender, EventArgs e)
        {
            double sunRA;
            double sunDec;
            double sunDistance;

            AstroUtilities.QuickPlanet(TimeOfMinimumJD, 3, true, out sunRA, out sunDec, out sunDistance);

            double dDeg = AstroConvert.ToDeclination(tbxDE.Text);
            double raHours = AstroConvert.ToRightAcsension(tbxRA.Text);

            double dRad = dDeg * Math.PI / 180.0;
            double raRad = raHours * 15 * Math.PI / 180.0;

            double C = 299792458.0; // m/s
            // HJD = JD - (r/c) * [sin(d) * sin (dSun) + cos(d) * cos (dSun) * cos (a - aSun)]
            double correction = (sunDistance * 1.4960E11 / C) * (Math.Sin(dRad) * Math.Sin(sunDec) + Math.Cos(dRad) * Math.Cos(sunDec) * Math.Cos(raRad - sunRA));

            double hjd = TimeOfMinimumJD - (correction / 3600 * 24);

            tbxCorrection.Text = hjd.ToString();
        }
    }
}
