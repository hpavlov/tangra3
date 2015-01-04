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
        internal double TimeOfMinimumHJD;
        internal double TimeCorrectionHJD;

        public frmHJDCalculation()
        {
            InitializeComponent();

            TimeOfMinimumHJD = double.NaN;
            TimeCorrectionHJD = double.NaN;
        }

        public override object InitializeLifetimeService()
        {
            // The lifetime of the object is managed by the add-in
            return null;
        }

        private void frmHJDCalculation_Load(object sender, EventArgs e)
        {
            dateTimePicker.Value = AstroUtilities.JDToDateTimeUtc(TimeOfMinimumJD).Date;
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

            double hjd = TimeOfMinimumJD - (correction / (3600 * 24));

            tbxCorrection.Text = hjd.ToString();

            TimeOfMinimumHJD = hjd;
            TimeCorrectionHJD = -(correction/(3600*24));
        }

        private void btnFindGSVSStar_Click(object sender, EventArgs e)
        {
            string userDesig = tbxVarStar.Text.Trim();
            EclipsingVariable foundVar = EclipsingVariableCatalogue.Instance.Stars.FirstOrDefault(x => x.Designation.Equals(userDesig, StringComparison.InvariantCultureIgnoreCase) || x.StandardDesignation.Equals(userDesig, StringComparison.InvariantCultureIgnoreCase));
            if (foundVar != null)
            {
                double ra = foundVar.RA*15*Math.PI/180.0;
                double de = foundVar.Dec*Math.PI / 180.0;

                AstroUtilities.ApparentStarPosition(ref ra, ref de, 0, 0, 2000, TimeOfMinimumJD);

                tbxRA.Text = AstroConvert.ToStringValue(ra * 180 / (Math.PI * 15), "HH MM SS.T");
                tbxDE.Text = AstroConvert.ToStringValue(de * 180 / Math.PI, "DD MM SS");
            }

        }
    }
}
