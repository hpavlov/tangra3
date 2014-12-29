using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Tangra.VideoOperations.LightCurves
{
    public partial class frmConfigureCsvExport : Form
    {
        public frmConfigureCsvExport()
        {
            InitializeComponent();
        }

        internal void DisableAbsoluteTimeExport()
        {
            rbJulianDays.Enabled = false;
        }

        internal CSVExportOptions GetSelectedOptions()
        {
            var rv = new CSVExportOptions();

            if (rbTimeString.Checked) rv.TimeFormat = TimeFormat.String;
            else if (rbDecimalDays.Checked) rv.TimeFormat = TimeFormat.DecimalDays;
            else if (rbJulianDays.Checked) rv.TimeFormat = TimeFormat.DecimalJulianDays;

            return rv;
        }
    }
}
