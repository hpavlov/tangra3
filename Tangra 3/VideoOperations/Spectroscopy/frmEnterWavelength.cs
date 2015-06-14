using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.VideoOperations.Spectroscopy.ViewSpectraStates;

namespace Tangra.VideoOperations.Spectroscopy
{
    public partial class frmEnterWavelength : Form
    {
        public float SelectedWaveLength;
        private SpectraViewerStateCalibrate m_State;

        public frmEnterWavelength()
        {
            InitializeComponent();
        }

        public frmEnterWavelength(SpectraViewerStateCalibrate state)
            : this()
        {
            m_State = state;
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

            m_State.CalibrationPointSelected(SelectedWaveLength);

            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
