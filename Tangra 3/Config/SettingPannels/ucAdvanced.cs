using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Model.Config;

namespace Tangra.Config.SettingPannels
{
    public partial class ucAdvanced : SettingsPannel
    {
        public ucAdvanced()
        {
            InitializeComponent();
        }

        public override void LoadSettings()
        {
            cbxOcrDebugMode.Checked = TangraConfig.Settings.Generic.OcrDebugModeEnabled;
            cbxDebugOutput.Checked = TangraConfig.Settings.Astrometry.SaveDebugOutput;
        }

        public override void SaveSettings()
        {
            TangraConfig.Settings.Generic.OcrDebugModeEnabled = cbxOcrDebugMode.Checked;
            TangraConfig.Settings.Astrometry.SaveDebugOutput = cbxDebugOutput.Checked;
        }
    }
}
