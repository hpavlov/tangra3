using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tangra.Model.Helpers;

namespace Tangra.VideoOperations.LightCurves.Helpers
{
    public partial class frmAcquisitionDelayHelp : Form
    {
        public frmAcquisitionDelayHelp()
        {
            InitializeComponent();
        }

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

            ShellHelper.OpenUrl("https://www.google.com/search?q=Pavlov%2C+H.%2C+Gault%2C+D.%2C+Using+the+Windows+Clock+with+Network+Time+Protocol+(NTP)+for+Occultation+Timing%2C+Journal+for+Occultation+Astronomy%2C+No.+2020-02%2C+Vol.+10+No.+2&oq=Pavlov%2C+H.%2C+Gault%2C+D.%2C+Using+the+Windows+Clock+with+Network+Time+Protocol+(NTP)+for+Occultation+Timing%2C+Journal+for+Occultation+Astronomy%2C+No.+2020-02%2C+Vol.+10+No.+2");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                var req = (HttpWebRequest)WebRequest.Create("http://www.iota-es.de/JOA/JOA2020_1.pdf");
                req.Method = "HEAD";
                var resp = (HttpWebResponse)req.GetResponse();
                if (resp.StatusCode == HttpStatusCode.OK)
                {
                    ShellHelper.OpenUrl("http://www.iota-es.de/JOA/JOA2020_1.pdf");
                    return;
                }
            }
            catch { }

            ShellHelper.OpenUrl("https://www.google.com/search?q=Gault%2C+D.%2C+Herald%2C+D.%2C+All-Of-System+Time+Testing+Using+Lunar+Occultations%2C+Journal+for+Occultation+Astronomy%2C+No.+2020-01%2C+Vol.+10+No.+1&oq=Gault%2C+D.%2C+Herald%2C+D.%2C+All-Of-System+Time+Testing+Using+Lunar+Occultations%2C+Journal+for+Occultation+Astronomy%2C+No.+2020-01%2C+Vol.+10+No.+1");

        }
    }
}
