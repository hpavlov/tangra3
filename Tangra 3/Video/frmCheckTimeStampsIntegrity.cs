using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Helpers;
using Tangra.Model.Helpers;
using Tangra.PInvoke;
using Tangra.Video.AstroDigitalVideo;
using Tangra.Video.SER;

namespace Tangra.Video
{
    public partial class frmCheckTimeStampsIntegrity : Form
    {
        private SERVideoStream m_SerStream;

        private AstroDigitalVideoStreamV2 m_AdvStream;

        public double MedianExposureMs { get; private set; }

        public double OneSigmaExposureMs { get; private set; }

        public int DroppedFrames { get; private set; }

        public int NonUtcTimestampedFrames { get; private set; }

        public double DroppedFramesPercentage { get; private set; }

        public bool HasTooManyDroppedFrames { get; private set; }

        public frmCheckTimeStampsIntegrity()
        {
            InitializeComponent();
        }

        public frmCheckTimeStampsIntegrity(SERVideoStream serStream)
            : this()
        {
            m_SerStream = serStream;
        }

        public frmCheckTimeStampsIntegrity(AstroDigitalVideoStreamV2 advStream)
            : this()
        {
            m_AdvStream = advStream;
        }

        private void RunSERIntegrityCheck()
        {
            SerFrameInfo prevFrameInfo = null;

            progressBar.Maximum = m_SerStream.LastFrame;
            progressBar.Minimum = m_SerStream.FirstFrame;
            var exposures = new List<double>();
            var timestamps = new List<DateTime>();

            for (int i = m_SerStream.FirstFrame; i <= m_SerStream.LastFrame; i++)
            {
                SerFrameInfo frameInfo = m_SerStream.SerFrameInfoOnly(i);

                TimeSpan ts = TimeSpan.Zero;
                TimeSpan tsUtc = TimeSpan.Zero;
                if (prevFrameInfo != null)
                {
                    ts = new TimeSpan(frameInfo.TimeStampUtc.Ticks - prevFrameInfo.TimeStampUtc.Ticks);

                    if (ts.TotalMilliseconds < 0)
                    {
                        MessageBox.Show(
                            this,
                            string.Format("Bad " + (m_SerStream.HasFireCaptureTimeStamps ? "FireCapture log " : "") + "timestamp at frame {0}. The timestamp jumps backwards.", i),
                            "SER Timestamp Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);

                        break;
                    }

                    if (m_SerStream.HasUTCTimeStamps)
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

                    if (m_SerStream.HasUTCTimeStamps && tsUtc != TimeSpan.Zero)
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

            var analyser = new TimestampIntegrityAnalyser();
            analyser.Calculate(exposures, timestamps);

            MedianExposureMs = analyser.MedianExposureMs;
            OneSigmaExposureMs = analyser.OneSigmaExposureMs;
            DroppedFrames = analyser.DroppedFrames;
            DroppedFramesPercentage = analyser.DroppedFramesPercentage;
            HasTooManyDroppedFrames = analyser.HasTooManyDroppedFrames;

            progressBar.Value = m_SerStream.LastFrame;
            progressBar.Update();
            Application.DoEvents();

            Close();
        }

        private void RunADVIntegrityCheck()
        {
            progressBar.Maximum = m_AdvStream.LastFrame;
            progressBar.Minimum = m_AdvStream.FirstFrame;
            NonUtcTimestampedFrames = 0;
            var exposures = new List<double>();
            var timestamps = new List<DateTime>();

            for (int i = m_AdvStream.FirstFrame; i <= m_AdvStream.LastFrame; i++)
            {
                var state = m_AdvStream.GetFrameStatusChannel(i);
                exposures.Add(state.ExposureInMilliseconds);
                if (state.CentralExposureTime > AdvFile.ADV_ZERO_DATE_REF)
                {
                    timestamps.Add(state.CentralExposureTime);
                }
                else
                {
                    NonUtcTimestampedFrames++;
                }

                progressBar.Value = i;
                progressBar.Update();

                if (i % 100 == 0)
                    Application.DoEvents();
            }

            timestamps.Sort();

            var analyser = new TimestampIntegrityAnalyser();
            analyser.Calculate(exposures, timestamps);

            MedianExposureMs = analyser.MedianExposureMs;
            OneSigmaExposureMs = analyser.OneSigmaExposureMs;
            DroppedFrames = analyser.DroppedFrames - NonUtcTimestampedFrames;
            DroppedFramesPercentage = analyser.DroppedFramesPercentage;
            HasTooManyDroppedFrames = analyser.HasTooManyDroppedFrames;

            progressBar.Value = m_AdvStream.LastFrame;
            progressBar.Update();
            Application.DoEvents();

            Close();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            if (m_SerStream != null)
            {
                RunSERIntegrityCheck();
            }
            else if (m_AdvStream != null)
            {
                RunADVIntegrityCheck();
            }
            else
            {
                Close();
            }
        }

        private void frmCheckTimeStampsIntegrity_Shown(object sender, EventArgs e)
        {
            timer1.Enabled = true;
        }

        private void frmCheckTimeStampsIntegrity_Load(object sender, EventArgs e)
        {
            if (m_SerStream != null)
            {
                Text = "Checking SER Timestamps Integrity";
            }
            else if (m_AdvStream != null)
            {
                Text = "Checking Timestamps Integrity";
            }
        }
    }
}
