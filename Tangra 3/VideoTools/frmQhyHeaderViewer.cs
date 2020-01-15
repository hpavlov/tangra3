using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tangra.Controller;
using Tangra.Helpers;
using Tangra.Model.Astro;
using Tangra.Model.Helpers;

namespace Tangra.VideoTools
{
    public partial class frmQhyHeaderViewer : Form
    {
        private VideoController m_VideoController;

        public frmQhyHeaderViewer()
        {
            InitializeComponent();
        }

        public frmQhyHeaderViewer(VideoController videoController)
        {
            InitializeComponent();

            m_VideoController = videoController;
        }

        public void ShowImageHeader(AstroImage currentImage)
        {
            try
            {
                var hdr = new QHYImageHeader(currentImage);

                lblSeqNo.Text = hdr.SeqNumber.ToString();
                lblTempNo.Text = hdr.TempNumber.ToString();

                lblGpsState.Text = hdr.GPSStatus.ToString();
                lblMaxClock.Text = hdr.MaxClock.ToString();
                try
                {
                    lblStartTime.Text = hdr.StartTime.ToString("yyyy-MM-dd  HH:mm:ss.fffffff");
                    lblEndTime.Text = hdr.EndTime.ToString("yyyy-MM-dd  HH:mm:ss.fffffff");
                }
                catch { }

                lblStartFlag.Text = "0x" + Convert.ToString(hdr.StartFlag, 16).PadLeft(2, '0');
                lblEndFlag.Text = "0x" + Convert.ToString(hdr.EndFlag, 16).PadLeft(2, '0');
                lblNowFlag.Text = "0x" + Convert.ToString(hdr.NowFlag, 16).PadLeft(2, '0');

                lblLongitude.Text = AstroConvert.ToStringValue(hdr.ParseLongitude, "+DD°MM'SS.TT\"");
                lblLatitude.Text = AstroConvert.ToStringValue(hdr.ParseLatitude, "+DD°MM'SS.TT\"");
                lblImageSize.Text = string.Format("{0} x {1}", hdr.Width, hdr.Height);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }
        }
    }
}
