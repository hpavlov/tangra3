using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tangra.Controller;
using Tangra.Model.Helpers;

namespace Tangra.VideoOperations.LightCurves
{
    public partial class frmAcquisitionDelayChooser : Form
    {
        private VideoController m_VideoController;

        public frmAcquisitionDelayChooser()
        {
            InitializeComponent();
        }

        public frmAcquisitionDelayChooser(VideoController videoController)
            : this()
        {
            m_VideoController = videoController;

            // TODO: For ADV v2 videos, try to get a default value for the delay from the header
        }

        internal double AcquisitionDelayMs { get; private set; }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                var req = (HttpWebRequest)WebRequest.Create("http://www.iota-es.de/JOA/JOA2020_2.pdf");
                req.Method = "HEAD";
                var resp = (HttpWebResponse)req.GetResponse();
                if (resp.StatusCode == HttpStatusCode.OK)
                {
                    ShellHelper.OpenUrl("http://www.iota-es.de/JOA/JOA2020_2.pdf");
                    return;
                }
            }
            catch { }

            ShellHelper.OpenUrl("https://www.google.com/search?q=Pavlov%2C+H.%2C+Gault%2C+D.%2C+Using+the+Windows+Clock+with+Network+Time+Protocol+(NTP)+for+Occultation+Timing%2C+Journal+for+Occultation+Astronomy%2C+No.+2020-02%2C+Vol.+10+No.+1&oq=Pavlov%2C+H.%2C+Gault%2C+D.%2C+Using+the+Windows+Clock+with+Network+Time+Protocol+(NTP)+for+Occultation+Timing%2C+Journal+for+Occultation+Astronomy%2C+No.+2020-02%2C+Vol.+10+No.+1");
        }

        Regex m_Validator = new Regex(@"^[\+\-]?\d+((\.|,)\d+)?$");

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!m_Validator.IsMatch(tbxAcquisitionDelay.Text.Trim()))
            {
                MessageBox.Show(this, "Please enter a valid acquisition delay value.", "Tangra", MessageBoxButtons.OK, MessageBoxIcon.Error);
                tbxAcquisitionDelay.Focus();
                return;
            }

            var userInput = double.Parse(tbxAcquisitionDelay.Text.Trim().Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture);
            if (userInput < 0)
            {
                MessageBox.Show(this, "By definition the acquisition delay cannot be negative. Please enter a correct value.", "Tangra", MessageBoxButtons.OK, MessageBoxIcon.Error);
                tbxAcquisitionDelay.Focus();
                return;
            }

            AcquisitionDelayMs = userInput;

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
