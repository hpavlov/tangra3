using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Tangra.VideoOperations.Spectroscopy
{
    public partial class frmEnterWavelength : Form
    {
        public float SelectedWaveLength;

        public frmEnterWavelength()
        {
            InitializeComponent();
        }

        private void UpdateCheckboxDerivedState(object sender, EventArgs e)
        {
            UpdateState();
        }

        private void UpdateState()
        {
            if (rbEnterManually.Checked)
            {
                nudManualWavelength.Enabled = true;
                lvFraunhoferLines.Enabled = false;
            }
            else if (rbPickFromList.Checked)
            {
                nudManualWavelength.Enabled = false;
                lvFraunhoferLines.Enabled = true;
            }
        }

        private void frmEnterWavelength_Load(object sender, EventArgs e)
        {
            UpdateState();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (rbEnterManually.Checked)
            {
                SelectedWaveLength = (float)nudManualWavelength.Value;
            }
            else if (rbPickFromList.Checked)
            {
                if (lvFraunhoferLines.SelectedItems.Count != 1)
                {
                    MessageBox.Show("Please select a line", "Tangra", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                SelectedWaveLength = float.Parse(lvFraunhoferLines.SelectedItems[0].SubItems[2].Text, CultureInfo.InvariantCulture);
            }

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
