using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Model.Helpers;
using Tangra.PInvoke;

namespace Tangra.Video.SER
{
    public partial class frmCheckTimeStampsIntegrity : Form
    {
        private SERVideoStream m_Stream;

        public double MedianExposure { get; private set; }

        public double OneSigmaExposure { get; private set; }

        public frmCheckTimeStampsIntegrity()
        {
            InitializeComponent();
        }

        public frmCheckTimeStampsIntegrity(SERVideoStream stream)
            : this()
        {
            m_Stream = stream;
        }

        private void RunIntegrityCheck()
        {
            SerFrameInfo prevFrameInfo = null;

            progressBar.Maximum = m_Stream.LastFrame;
            progressBar.Minimum = m_Stream.FirstFrame;
            var exposures = new List<double>();

            for (int i = m_Stream.FirstFrame; i <= m_Stream.LastFrame; i++)
            {
                SerFrameInfo frameInfo = m_Stream.SerFrameInfoOnly(i);

                TimeSpan ts = TimeSpan.Zero;
                TimeSpan tsUtc = TimeSpan.Zero;
                if (prevFrameInfo != null)
                {
                    ts = new TimeSpan(frameInfo.TimeStampUtc.Ticks - prevFrameInfo.TimeStampUtc.Ticks);

                    if (ts.TotalMilliseconds < 0)
                    {
                        MessageBox.Show(
                            this,
                            string.Format("Bad " + (m_Stream.HasFireCaptureTimeStamps ? "FireCapture log " : "") + "timestamp at frame {0}. The timestamp jumps backwards.", i),
                            "SER Timestamp Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);

                        break;
                    }

                    if (m_Stream.HasUTCTimeStamps)
                    {
                        tsUtc = new TimeSpan(frameInfo.TimeStampUtc.Ticks - prevFrameInfo.TimeStampUtc.Ticks);
                        if (tsUtc.TotalMilliseconds < 0)
                        {
                            MessageBox.Show(
                                this,
                                string.Format("Bad Utc timestamp at frame {0}. The timestamp jumps backwards.", i),
                                "SER Timestamp Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);

                            break;
                        }
                    }

                    if (m_Stream.HasUTCTimeStamps && tsUtc != TimeSpan.Zero)
                        exposures.Add(tsUtc.TotalMilliseconds);
                    else if (ts != TimeSpan.Zero)
                        exposures.Add(ts.TotalMilliseconds);
                }

                prevFrameInfo = frameInfo;
                progressBar.Value = i;
                progressBar.Update();

                if (i % 100 == 0)
                    Application.DoEvents();
            }

            progressBar.Value = m_Stream.LastFrame;
            progressBar.Update();
            Application.DoEvents();

            MedianExposure = exposures.Median();

            if (exposures.Count > 2)
            {
                OneSigmaExposure = Math.Sqrt(exposures.Select(x => (x - MedianExposure) * (x - MedianExposure)).Sum() / (exposures.Count - 1));
            }
            else
            {
                OneSigmaExposure = 0;
            }

            Close();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            RunIntegrityCheck();
        }

        private void frmCheckTimeStampsIntegrity_Shown(object sender, EventArgs e)
        {
            timer1.Enabled = true;
        }
    }
}
