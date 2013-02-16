using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Model.Helpers;

namespace Tangra
{
    public partial class frmFITSHeader : Form
    {
        private nom.tam.fits.Header Header;

        public frmFITSHeader(nom.tam.fits.Header hdr)
        {
            InitializeComponent();

            Header = hdr;

            //TODO: Load the Scope & Camera from the current configuration 
            //      Load the observer from the global configuration 
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (ValidateHeader())
            {
                SaveHeader();

                //TODO: Save the Scope & Camera to the current configuration 
                //      Save the observer to the global configuration 

                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private double m_RA = double.NaN;
        private double m_DEC = double.NaN;
        private string m_ExposureTime = string.Empty;

        private bool ValidateHeader()
        {
            m_RA = double.NaN;
            m_DEC = double.NaN;

            if (!string.IsNullOrEmpty(tbxRA.Text))
            {
                try
                {
                    m_RA = AstroConvert.ToRightAcsension(tbxRA.Text);
                }
                catch (FormatException)
                {
                    MessageBox.Show(this, "Please enter a valid Right Ascension value.\r\n\r\nAccepted formats: HH:MM:SS.S; HH:MM:SS; HH:MM.M; HH.H; HH MM SS.S; HH MM SS; HH MM.M; HH.H", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    tbxRA.SelectAll();
                    tbxRA.Focus();
                    return false;
                }
            }

            if (!string.IsNullOrEmpty(tbxDE.Text))
            {
                try
                {
                    m_DEC = AstroConvert.ToDeclination(tbxDE.Text);
                }
                catch (FormatException)
                {
                    MessageBox.Show(this, "Please enter a valid Declination value.\r\n\r\nAccepted formats: +DD:MM:SS.S; +DD:MM:SS; +DD:MM.M; +DD.D; +DD MM SS.S; DD MM SS; DD MM.M; DD.D", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    tbxDE.SelectAll();
                    tbxDE.Focus();
                    return false;
                }
            }

            bool hasRA = !double.IsNaN(m_RA);
            bool hasDE = !double.IsNaN(m_DEC);

            if (hasRA ^ hasDE)
            {
                MessageBox.Show(this, "Please enter both the Right Ascension and Declination", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (double.IsNaN(m_RA))
                {
                    tbxRA.SelectAll();
                    tbxRA.Focus();
                }
                else if (double.IsNaN(m_DEC))
                {
                    tbxDE.SelectAll();
                    tbxDE.Focus();
                }
                return false;
            }

            if (cbxDateTime.Checked)
            {
                DateTime dt = dtpDate.Value.Date.Add(new TimeSpan(dtpTime.Value.Hour, dtpTime.Value.Minute, dtpTime.Value.Second));
                m_ExposureTime = dt.ToString("yyyy-MM-ddTHH:mm:ss");
            }

            return true;
        }

        private void SaveHeader()
        {
            Header.AddValue("HISTORY", "Fits header populated by Tangra v0.1", string.Empty);

            if (!string.IsNullOrEmpty(tbxObject.Text))
                Header.AddValue("OBJECT", tbxObject.Text, null);

            if (!string.IsNullOrEmpty(tbxScope.Text))
                Header.AddValue("TELESCO", tbxScope.Text, null);

            if (!string.IsNullOrEmpty(tbxCamera.Text))
                Header.AddValue("INSTRUME", tbxCamera.Text, null);

            if (!string.IsNullOrEmpty(tbxObserver.Text))
                Header.AddValue("OBSERVER", tbxObserver.Text, null);

            if (!string.IsNullOrEmpty(tbxNotes.Text))
                Header.AddValue("NOTES", tbxNotes.Text, null);

            if (!string.IsNullOrEmpty(tbxRA.Text))
            {
                Header.AddValue("OBJCTRA", AstroConvert.ToStringValue(m_RA, "HH MM SS.TT"), string.Empty);
                Header.AddValue("RA", AstroConvert.ToStringValue(m_RA, "HH MM SS.TT"), string.Empty);
            }

            if (!string.IsNullOrEmpty(tbxDE.Text))
            {
                Header.AddValue("OBJCTDEC", AstroConvert.ToStringValue(m_DEC, "DD MM SS.T"), string.Empty);
                Header.AddValue("DEC", AstroConvert.ToStringValue(m_DEC, "DD MM SS.T"), string.Empty);
            }

            if (cbxDateTime.Checked)
            {
                Header.AddValue("DATE-OBS", m_ExposureTime, string.Empty);
                Header.AddValue("TIMESYS", "UTC", "Default time system");
            }
            
            Header.AddValue("CLRBAND", "R", string.Empty);
        }

        private void cbxDateTime_CheckedChanged(object sender, EventArgs e)
        {
            dtpDate.Enabled = cbxDateTime.Checked;
            dtpTime.Enabled = cbxDateTime.Checked;
        }
    }
}
