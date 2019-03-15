using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tangra.Controller;
using Tangra.Video;

namespace Tangra.View.CustomRenderers.AavTimeAnalyser
{
    public partial class frmAavStatusChannelOnlyView : Form
    {
        private VideoController m_VideoController;
        private AstroDigitalVideoStream m_Aav;
        private AavTimeAnalyser m_TimeAnalyser;

        public frmAavStatusChannelOnlyView()
        {
            InitializeComponent();
        }

        public frmAavStatusChannelOnlyView(VideoController videoController, AstroDigitalVideoStream aav)
            : this()
        {
            m_Aav = aav;
            m_VideoController = videoController;
            m_TimeAnalyser = new AavTimeAnalyser(aav);
        }

        private void UpdateProgressBar(int val, int max)
        {
            if (val == 0 && max == 0)
            {
                pbLoadData.Visible = false;
            }
            else if (val == 0 && max > 0)
            {
                pbLoadData.Maximum = max;
                pbLoadData.Value = 0;
                pbLoadData.Visible = true;
            }
            else
            {
                pbLoadData.Value = Math.Min(val, max);
                pbLoadData.Update();
            }
        }

        private void frmAavStatusChannelOnlyView_Load(object sender, EventArgs e)
        {
            Text = m_Aav.FileName;

            pbLoadData.Visible = true;

            m_TimeAnalyser.Initialize((val, max) =>
            {
                this.Invoke(new Action<int, int>(UpdateProgressBar), val, max);
            });

            // TODO: Extract all times in memory, to plot them easier
            // TODO: Have a tab with plotted times
            // TODO: Have a tab with debug images + all status channel data under it
            // TODO: Tab with export in CSV
        }

    }
}
