using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Tangra.VideoOperations.Spectroscopy.AbsFluxCalibration
{
    public partial class frmCompleteSpectra : Form
    {
        private AbsFluxSpectra m_Spectra;

        public frmCompleteSpectra()
        {
            InitializeComponent();
        }

        internal frmCompleteSpectra(AbsFluxSpectra spectra)
            : this()
        {
            m_Spectra = spectra;

            if (spectra.IsStandard) 
                rbCalibrationStar.Checked = true;
            else 
                rbProgramStar.Checked = true;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (rbProgramStar.Checked)
            {
                if (!VerifyPogramSpectraCompleteness())
                    return;
            }
            else if (!VerifyStandardSpectraCompleteness())
                    return;

            DialogResult = DialogResult.OK;
            Close();
        }

        private bool VerifyStandardSpectraCompleteness()
        {
            return false;
        }

        private bool VerifyPogramSpectraCompleteness()
        {
            m_Spectra.SetAsProgramStar();

            return m_Spectra.IsComplete;
        }
    }
}
