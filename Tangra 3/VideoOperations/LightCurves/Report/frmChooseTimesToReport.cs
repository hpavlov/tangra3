using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Tangra.VideoOperations.LightCurves.Report
{
    public partial class frmChooseTimesToReport : Form
    {
        private OccultationEventInfo m_Event;

        public frmChooseTimesToReport()
        {
            InitializeComponent();

            UseTangrasTimes = true;
        }

        internal void SetTimes(OccultationEventInfo evt)
        {
            m_Event = evt;
        }

        internal string CameraNameTangra;

        internal string CameraNameAOTA;

        internal bool? TangraKnowsCameraDelays;

        internal bool AOTAKnowsCameraDelays;

        internal bool UseTangrasTimes;

        private void frmChooseTimesToReport_Load(object sender, EventArgs e)
        {
            lblDTimeAOTA.Text = string.Format("{0} ± {1}", m_Event.DTimeAOTA.ToString("HH mm ss.ff"), (m_Event.DTimeErrorMSAOTA / 1000.0).ToString("0.00"));
            lblRTimeAOTA.Text = string.Format("{0} ± {1}", m_Event.RTimeAOTA.ToString("HH mm ss.ff"), (m_Event.RTimeErrorMSAOTA / 1000.0).ToString("0.00"));
            lblDTimeTangra.Text = string.Format("{0} ± {1}", m_Event.DTime.ToString("HH mm ss.ff"), (m_Event.DTimeErrorMS / 1000.0).ToString("0.00"));
            lblRTimeTangra.Text = string.Format("{0} ± {1}", m_Event.RTime.ToString("HH mm ss.ff"), (m_Event.RTimeErrorMS / 1000.0).ToString("0.00"));

            lblCameraNameTangra.Text = CameraNameTangra;
            lblCameraNameAOTA.Text = CameraNameAOTA;
            lblDelaysAppliedTangra.Text = TangraKnowsCameraDelays.HasValue ? (TangraKnowsCameraDelays.Value ? "Yes" : "No") : "Not Required";
            lblDelaysAppliedAOTA.Text = AOTAKnowsCameraDelays ? "Yes" : "No";
        }

        private void btnReportTangra_Click(object sender, EventArgs e)
        {
            UseTangrasTimes = true;
            Close();
        }

        private void btnReportAOTA_Click(object sender, EventArgs e)
        {
            UseTangrasTimes = false;
            Close();
        }
    }
}
