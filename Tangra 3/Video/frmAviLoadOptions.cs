using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Helpers;
using Tangra.Model.Config;
using Tangra.PInvoke;

namespace Tangra.Video
{
    public partial class frmAviLoadOptions : Form
    {
        public ReInterlaceMode ReInterlaceMode = ReInterlaceMode.None;

        public frmAviLoadOptions()
        {
            InitializeComponent();

            cbxRenderingEngineAttemptOrder.Items.Clear();

            string[] availableRenderingEngines = TangraVideo.EnumVideoEngines();
            cbxRenderingEngineAttemptOrder.Items.AddRange(availableRenderingEngines);
        }

        private void frmAviLoadOptions_Load(object sender, EventArgs e)
        {
            cbxRenderingEngineAttemptOrder.SetCBXIndex(TangraConfig.Settings.Generic.AviRenderingEngineIndex);
            if (cbxRenderingEngineAttemptOrder.SelectedIndex == -1 && cbxRenderingEngineAttemptOrder.Items.Count > 0)
                cbxRenderingEngineAttemptOrder.SelectedIndex = 0;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            TangraConfig.Settings.Generic.AviRenderingEngineIndex = cbxRenderingEngineAttemptOrder.SelectedIndex;

            if (rbReInterlaceSwapFields.Checked)
                ReInterlaceMode = ReInterlaceMode.SwapFields;
            else if (rbReInterlaceShiftForward.Checked)
                ReInterlaceMode = ReInterlaceMode.ShiftOneField;
            else if (rbReInterlaceShiftAndSwap.Checked)
                ReInterlaceMode = ReInterlaceMode.SwapAndShiftOneField;

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
