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
        public float? SelectedDispersion;

        private SpectraViewerStateCalibrate m_State;

        public frmEnterWavelength()
        {
            InitializeComponent();
        }

        public frmEnterWavelength(SpectraViewerStateCalibrate state)
            : this()
        {
            m_State = state;
	        SelectedWaveLength = float.NaN;
            SelectedDispersion = null;
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
            if (cbxSinglePoint.Checked)
            {
                SelectedDispersion = (float) nudDispersion.Value;
                m_State.DispersionSelected(SelectedDispersion.Value);
            }

            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

		private void frmEnterWavelength_FormClosed(object sender, FormClosedEventArgs e)
		{
			if (float.IsNaN(SelectedWaveLength))
				m_State.CalibrationPointSelectionCancelled();
		}

        private void cbxSinglePoint_CheckedChanged(object sender, EventArgs e)
        {
            nudDispersion.Enabled = cbxSinglePoint.Checked;
        }

		private void frmEnterWavelength_Shown(object sender, EventArgs e)
		{
			pnlOnePointCalibration.Visible = !m_State.HasSelectedCalibrationPoints();
		}

		private void btnReset_Click(object sender, EventArgs e)
		{
			m_State.ResetCalibration();
			Close();
		}
    }
}
