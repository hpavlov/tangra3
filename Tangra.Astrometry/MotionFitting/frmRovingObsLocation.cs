using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tangra.Model.Config;
using Tangra.Model.Helpers;

namespace Tangra.MotionFitting
{
    public partial class frmRovingObsLocation : Form
    {
        public RovingObsLocation ObservatoryLocation = new RovingObsLocation() { IsValid = false };

        public frmRovingObsLocation()
        {
            InitializeComponent();
            
            if (TangraConfig.Settings.LastUsed.RovingLongitude == null)
            {
                cbxLongitude.SelectedIndex = -1;
                tbxLongitude.Text = string.Empty;
            }
            else
            {
                double longNoSign = Math.Abs(TangraConfig.Settings.LastUsed.RovingLongitude.Value);
                tbxLongitude.Text = AstroConvert.ToStringValue(longNoSign, "DD MM SS");
                if (TangraConfig.Settings.LastUsed.RovingLongitude.Value < 0)
                    cbxLongitude.SelectedIndex = 1;
                else
                    cbxLongitude.SelectedIndex = 0;
            }

            if (TangraConfig.Settings.LastUsed.RovingLatitude == null)
            {
                cbxLatitude.SelectedIndex = -1;
                tbxLatitude.Text = string.Empty;
            }
            else
            {
                double latNoSign = Math.Abs(TangraConfig.Settings.LastUsed.RovingLatitude.Value);
                tbxLatitude.Text = AstroConvert.ToStringValue(latNoSign, "DD MM SS");
                if (TangraConfig.Settings.LastUsed.RovingLatitude.Value < 0)
                    cbxLatitude.SelectedIndex = 1;
                else
                    cbxLatitude.SelectedIndex = 0;
            }

            if (TangraConfig.Settings.LastUsed.RovingAltitude != null)
            {
                tbxAltitude.Text = string.Format("{0:0}", TangraConfig.Settings.LastUsed.RovingAltitude.Value);
            }
            else
            {
                tbxAltitude.Text = string.Empty;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(tbxLongitude.Text))
            {
                if (cbxLongitude.SelectedIndex == -1)
                {
                    MessageBox.Show("Please select West or East latitude.", "Tangra", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    cbxLongitude.Focus();
                    return;
                }

                try
                {
                    int sign = cbxLongitude.SelectedIndex == 1 ? -1 : 1;
                    ObservatoryLocation.LongitudeInDeg = sign * AstroConvert.ToDeclination(tbxLongitude.Text);
                }
                catch (FormatException)
                {
                    MessageBox.Show(string.Format("'{0}' is not a valid longitude.", tbxLongitude.Text), "Tangra", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    tbxLongitude.Focus();
                    return;
                }
            }
            else
            {
                MessageBox.Show("Please enter longitude.", "Tangra", MessageBoxButtons.OK, MessageBoxIcon.Error);

                tbxLongitude.Focus();
                return;
            }

            if (!string.IsNullOrEmpty(tbxLatitude.Text))
            {
                if (cbxLatitude.SelectedIndex == -1)
                {
                    MessageBox.Show("Please select North or South latitude.", "Tangra", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    cbxLatitude.Focus();
                    return;
                }

                try
                {
                    int sign = cbxLatitude.SelectedIndex == 1 ? -1 : 1;
                    ObservatoryLocation.LatitudeInDeg = sign * AstroConvert.ToDeclination(tbxLatitude.Text);
                }
                catch (FormatException)
                {
                    MessageBox.Show(string.Format("'{0}' is not a valid latitude.", tbxLatitude.Text), "Tangra", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    tbxLatitude.Focus();
                    return;
                }
            }
            else
            {
                MessageBox.Show("Please enter latitude.", "Tangra", MessageBoxButtons.OK, MessageBoxIcon.Error);

                tbxLongitude.Focus();
                return;
            }

            if (!string.IsNullOrEmpty(tbxAltitude.Text))
            {
                try
                {
                    ObservatoryLocation.AltitudeInMeters = double.Parse(tbxAltitude.Text.Trim());
                }
                catch (FormatException)
                {
                    MessageBox.Show(string.Format("'{0}' is not a valid altitude.", tbxAltitude.Text), "Tangra", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    tbxAltitude.Focus();
                    return;
                }
                
            }
            else
            {
                MessageBox.Show("Please enter altitude.", "Tangra", MessageBoxButtons.OK, MessageBoxIcon.Error);
                tbxAltitude.Focus();
                return;
            }

            ObservatoryLocation.IsValid = true;

            TangraConfig.Settings.LastUsed.RovingLongitude = ObservatoryLocation.LongitudeInDeg;
            TangraConfig.Settings.LastUsed.RovingLatitude = ObservatoryLocation.LatitudeInDeg;
            TangraConfig.Settings.LastUsed.RovingAltitude = ObservatoryLocation.AltitudeInMeters;
            TangraConfig.Settings.Save();

            DialogResult = DialogResult.OK;
            Close();
        }

        private void llMPC_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ShellHelper.OpenUrl("https://www.minorplanetcenter.net/iau/mpc.html");
        }
    }
}
