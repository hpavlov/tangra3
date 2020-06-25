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

        public int DroppedFrames { get; private set; }

        public double DroppedFramesPercentage { get; private set; }

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
            var timestamps = new List<DateTime>();

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
                    {
                        timestamps.Add(frameInfo.TimeStampUtc);
                        exposures.Add(tsUtc.TotalMilliseconds);
                    }
                    else if (ts != TimeSpan.Zero)
                    {
                        timestamps.Add(frameInfo.TimeStampUtc);
                        exposures.Add(ts.TotalMilliseconds);
                    }
                }

                prevFrameInfo = frameInfo;
                progressBar.Value = i;
                progressBar.Update();

                if (i % 100 == 0)
                    Application.DoEvents();
            }

            // Fist estimate assuming no dropped frames
            MedianExposure = exposures.Median();

            DroppedFrames = 0;
            DroppedFramesPercentage = 0;

            if (timestamps.Count > 2)
            {
                exposures.Clear();

                var currTs = timestamps[0];
                for (int i = 1; i < timestamps.Count; i++)
                {
                    int frameGap = 1;
                    var nextTs = currTs.AddMilliseconds(MedianExposure);
                    while (Math.Abs((nextTs - timestamps[i]).TotalMilliseconds) > MedianExposure / 10 && nextTs < timestamps[i])
                    {
                        DroppedFrames++;
                        nextTs = nextTs.AddMilliseconds(MedianExposure);
                        frameGap++;
                    }

                    exposures.Add((timestamps[i] - currTs).TotalMilliseconds / frameGap);
                    currTs = timestamps[i];
                }

                DroppedFramesPercentage = DroppedFrames * 100.0 / (timestamps.Count + DroppedFrames);

                // Second estimate, adjustment for dropped frames.
                MedianExposure = exposures.Median();
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
