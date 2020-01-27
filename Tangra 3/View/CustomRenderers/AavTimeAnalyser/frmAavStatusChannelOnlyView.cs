using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic.Devices;
using Tangra.Controller;
using Tangra.Model.Helpers;
using Tangra.Video;

namespace Tangra.View.CustomRenderers.AavTimeAnalyser
{
    public partial class frmAavStatusChannelOnlyView : Form
    {
        internal enum GraphType
        {
            gtNone,
            gtTimeDeltas,
            gtSystemUtilisation,
            gtNtpUpdates,
            gtNtpUpdatesInclUnapplied,
            gtZoomedTotalDelta,
            gtDeltaBucketIntervals
        }

        internal enum GridlineStyle
        {
            Line,
            Tick
        }

        internal class GraphConfig
        {
            public GridlineStyle GridlineStyle = GridlineStyle.Line;
            public bool ConnectionLines = false;
        }

        private GraphType m_GrapthType = GraphType.gtNone;

        private VideoController m_VideoController;
        private AstroDigitalVideoStream m_Aav;
        private AavTimeAnalyser m_TimeAnalyser;

        private int m_LastGraphWidth;
        private int m_LastGraphHeight;
        private int? m_PlotFromIndex = null;
        private int? m_PlotToIndex = null;

        const float MIN_PIX_DIFF = 1f;

        private bool m_IsDataReady;

        private float m_MedianSystemFileTime;

        private List<float> m_OneMinIntervalData = new List<float>();

        private GraphConfig m_GraphConfig = new GraphConfig();

        private static Font m_TitleFont = new Font(DefaultFont, FontStyle.Bold);

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

        private void UpdateProgressBarImpl(int val, int max, ProgressBar pbar, Action onFinished)
        {
            if (val == 0 && max == 0)
            {
                pbar.Visible = false;
                onFinished();
            }
            else if (val == 0 && max > 0)
            {
                pbar.Maximum = max;
                pbar.Value = 0;
                pbar.Visible = true;
                tabControl.SelectedTab = tabOverview;
            }
            else
            {
                pbar.Value = Math.Min(val, max);
                pbar.Update();
            }
        }

        private void UpdateProgressBar(int val, int max)
        {
            UpdateProgressBarImpl(val, max, pbLoadData, OnTimeAnalysisDataReady);
        }

        private void UpdateExportProgressBar(int val, int max)
        {
            UpdateProgressBarImpl(val, max, pbLoadData, OnExportFinished);
        }

        private void frmAavStatusChannelOnlyView_Load(object sender, EventArgs e)
        {
            Text = string.Format("AAV Time Analysis: {0}", m_Aav.FileName);

            m_IsDataReady = false;
            pbLoadData.Visible = true;
            Height = Height + 10 + (Math.Max(480, m_Aav.Height) - pbOcrErrorFrame.Height);
            Width = 10 + Math.Max(800, m_Aav.Width);

            m_TimeAnalyser.Initialize((val, max) =>
            {
                this.Invoke(new Action<int, int>(UpdateProgressBar), val, max);
            });
        }

        private void OnTimeAnalysisDataReady()
        {
            nudOcrErrorFrame.Enabled = m_TimeAnalyser.DebugFrames.Count > 0;
            if (nudOcrErrorFrame.Enabled)
            {
                nudOcrErrorFrame.Maximum = m_TimeAnalyser.DebugFrames.Count - 1;
                nudOcrErrorFrame.Minimum = 0;
                nudOcrErrorFrame.Value = 0;
            }

            cbxGraphType.SelectedIndex = 0;
            m_IsDataReady = true;
            
            ShowOcrErrorFrame();

            tabControl.SelectedTab = tabGraphs;
            DrawGraph();

            AnalyseData();

            tbxAnalysisDetails.Visible = true;
        }

        private void AnalyseData()
        {
            tbxAnalysisDetails.Clear();
            m_OneMinIntervalData.Clear();

            var nonOutliers = m_TimeAnalyser.Entries.Where(x => !x.IsOutlier).ToList();
            m_MedianSystemFileTime = nonOutliers.Select(x => x.DeltaSystemFileTimeMs).Median();
            double medianSystemFileTimeVariance = nonOutliers.Sum(x => (m_MedianSystemFileTime - x.DeltaSystemFileTimeMs) * (m_MedianSystemFileTime - x.DeltaSystemFileTimeMs));
            medianSystemFileTimeVariance = Math.Sqrt(medianSystemFileTimeVariance / nonOutliers.Count);

            var medianSystemTime = nonOutliers.Select(x => x.DeltaSystemTimeMs).Median();
            double medianSystemTimeVariance = nonOutliers.Sum(x => (medianSystemTime - x.DeltaSystemTimeMs) * (medianSystemTime - x.DeltaSystemTimeMs));
            medianSystemTimeVariance = Math.Sqrt(medianSystemTimeVariance / nonOutliers.Count);

            var medianNtpDiff = nonOutliers.Select(x => Math.Abs(x.DeltaSystemTimeMs - x.DeltaNTPTimeMs)).Median();

            tbxAnalysisDetails.AppendText(string.Format("SystemTimeAsFileTime: {0:0.0} ms +/- {1:0.00} ms    SystemTime: {2:0.0} ms +/- {3:0.00} ms    MedianNTPDiff: {4:0.0} ms\r\n\r\n", m_MedianSystemFileTime, medianSystemFileTimeVariance, medianSystemTime, medianSystemTimeVariance, medianNtpDiff));
            tbxAnalysisDetails.AppendText(string.Format("Acqusition Delay: {0:0.0} ms    RecordingDelay: {1:0.0} ms\r\n\r\n", m_MedianSystemFileTime, medianSystemTime - m_MedianSystemFileTime));

            ComputeMedianAndErrorForAllOneMinuteIntervals(nonOutliers);

            for (int i = 0; i < m_TimeAnalyser.Entries.Count; i++)
            {
                var ol = m_TimeAnalyser.Entries[i];

                var delta = ol.DeltaSystemFileTimeMs - m_MedianSystemFileTime;
                if (Math.Abs(delta) > 6 * medianSystemFileTimeVariance)
                {
                    var ol2 = i == 0 ? m_TimeAnalyser.Entries[1] : m_TimeAnalyser.Entries[i - 1];
                    var utilEntry = ol.UtilisationEntry;
                    if (utilEntry == null) utilEntry = m_TimeAnalyser.Entries.Last(x => x.UtilisationEntry != null).UtilisationEntry;
                    tbxAnalysisDetails.AppendText(string.Format("Outlier at {0}, FrameNo: {1} Delta: {2:0.0} ms; Non-Acquisition Delay: {3:0.0} ms; CPU: {4:0.0}% Disks: {5:0.0}%  {6}\r\n", ol.SystemTimeFileTime.ToString("HH:mm:ss.fff"), ol.FrameNo, delta, (ol2.DeltaNTPTimeMs - ol2.DeltaSystemFileTimeMs) - (ol.DeltaNTPTimeMs - ol.DeltaSystemFileTimeMs), utilEntry.CpuUtilisation, utilEntry.DiskUtilisation, ol.DebugImage != null ? "OCR-ERR:" + ol.OrcField1 + "  " + ol.OrcField2 : null));
                }
            }
        }

        private void ComputeMedianAndErrorForAllOneMinuteIntervals(List<TimeAnalyserEntry> nonOutliers)
        {
            var varianceList = new List<double>();
            DateTime? startIntervalTime = null;
            for (int i = 0; i < nonOutliers.Count; i++)
            {
                if (startIntervalTime == null) startIntervalTime = nonOutliers[i].SystemTimeFileTime;
                m_OneMinIntervalData.Add(nonOutliers[i].DeltaSystemFileTimeMs);
                if (nonOutliers[i].SystemTimeFileTime > startIntervalTime.Value.AddMinutes(1) || i == nonOutliers.Count - 1)
                {
                    var oneMinMedianSystemTime = m_OneMinIntervalData.Median();
                    double diff = oneMinMedianSystemTime - m_MedianSystemFileTime;
                    varianceList.Add(diff);
                    startIntervalTime = null;
                    m_OneMinIntervalData.Clear();
                }
            }

            tbxAnalysisDetails.AppendText(string.Format("SystemTimeAsFileTime 1-Min Median Delta: {0:0.0} ms / {1:0.00} ms\r\n\r\n", varianceList.Max(), varianceList.Min()));
           
        }

        private void DrawGraph()
        {
            if (!m_IsDataReady) return;

            switch (m_GrapthType)
            {
                case GraphType.gtTimeDeltas:
                    DrawTimeDeltasGraph();
                    break;
                case GraphType.gtSystemUtilisation:
                    DrawSystemUtilisationGraph();
                    break;
                case GraphType.gtNtpUpdates:
                case GraphType.gtNtpUpdatesInclUnapplied:
                    DrawNtpUpdatesGraph();
                    break;
                case GraphType.gtZoomedTotalDelta:
                    DrawZoomedTimeDeltasGraph();
                    break;
                case GraphType.gtDeltaBucketIntervals:
                    DrawDeltaBucketsGraph();
                    break;
            }
        }

        private void DrawTimeDeltasGraph()
        {
            Cursor = Cursors.WaitCursor;

            bool plotSystemTime = rbSystemTime.Checked;
            bool plotOccuRecTime = cbxNtpTime.Checked;
            bool plotNtpError = cbxNtpError.Checked;
            int graphWidth = pbGraph.Width;
            int graphHeight = pbGraph.Height;
            var image = new Bitmap(graphWidth, graphHeight, PixelFormat.Format24bppRgb);

            bool ellipses = !m_GraphConfig.ConnectionLines;
            bool tickGridlines = m_GraphConfig.GridlineStyle == GridlineStyle.Tick;

            Task.Run(() =>
            {
                Pen SysTimePen = Pens.Red;
                Brush SysTimeBrush = Brushes.Red;
                Pen NtpTimePen = Pens.LimeGreen;
                Brush NtpTimeBrush = Brushes.LimeGreen;
                Pen NtpErrorPen = Pens.DarkKhaki;
                Brush NtpErrorBrush = Brushes.DarkKhaki;

                float maxDelta = 0;
                float minDelta = 0;

                bool isSubset = m_PlotFromIndex.HasValue && m_PlotToIndex.HasValue;
                IEnumerable<TimeAnalyserEntry> subsetEnum = null;
                if (isSubset)
                {
                    subsetEnum = m_TimeAnalyser.Entries.Skip(m_PlotFromIndex.Value).Take(m_PlotToIndex.Value);
                }
                
                if (plotSystemTime)
                {
                    if (isSubset)
                    {
                        minDelta = subsetEnum.Min(x => x.DeltaSystemTimeMs);
                        maxDelta = subsetEnum.Max(x => x.DeltaSystemTimeMs);
                    }
                    else
                    {
                        minDelta = m_TimeAnalyser.MinDeltaSystemTimeMs;
                        maxDelta = m_TimeAnalyser.MaxDeltaSystemTimeMs;
                    }
                }
                else
                {
                    if (isSubset)
                    {
                        minDelta = subsetEnum.Min(x => x.DeltaSystemFileTimeMs);
                        maxDelta = subsetEnum.Max(x => x.DeltaSystemFileTimeMs);
                    }
                    else
                    {
                        minDelta = m_TimeAnalyser.MinDeltaSystemFileTimeMs;
                        maxDelta = m_TimeAnalyser.MaxDeltaSystemFileTimeMs;
                    }
                }

                if (plotOccuRecTime)
                {
                    if (isSubset)
                    {
                        var minNtpDelta = subsetEnum.Min(x => x.DeltaNTPTimeMs);
                        var maxNtpDelta = subsetEnum.Max(x => x.DeltaNTPTimeMs);
                        minDelta = Math.Min(minDelta, minNtpDelta);
                        maxDelta = Math.Max(maxDelta, maxNtpDelta);
                    }
                    else
                    {
                        minDelta = Math.Min(minDelta, m_TimeAnalyser.MinDeltaNTPMs);
                        maxDelta = Math.Max(maxDelta, m_TimeAnalyser.MaxDeltaNTPMs);
                    }
                }

                if (plotNtpError)
                {
                    if (isSubset)
                    {
                        var maxNtpErr = subsetEnum.Max(x => x.NTPErrorMs);
                        minDelta = Math.Min(minDelta, -maxNtpErr);
                        maxDelta = Math.Max(maxDelta, maxNtpErr);
                    }
                    else
                    {
                        minDelta = Math.Min(minDelta, m_TimeAnalyser.MinDeltaNTPErrorMs);
                        maxDelta = Math.Max(maxDelta, m_TimeAnalyser.MaxDeltaNTPErrorMs);
                    }
                }

                // Extend the Y range by 50ms for better display
                minDelta -= 50;
                maxDelta += 50;

                using (var g = Graphics.FromImage(image))
                {
                    float padding = 10;
                    float paddingX = 40;
                    float paddingY = 25;
                    float width = graphWidth - padding - paddingX;
                    float height = graphHeight - 2 * paddingY;
                    float yFactor = height / (maxDelta - minDelta);
                    float minX = m_PlotFromIndex.HasValue ? m_PlotFromIndex.Value : 0;
                    float maxX = m_PlotToIndex.HasValue ? m_PlotToIndex.Value : m_TimeAnalyser.Entries.Count - 1;
                    float xFactor = width / (maxX - minX + 1);

                    g.FillRectangle(Brushes.WhiteSmoke, 0, 0, graphWidth, graphHeight);

                    for (int ya = -2000; ya < 2000; ya += 100)
                    {
                        float y = graphHeight - paddingY - yFactor * (ya - minDelta);
                        if (y < paddingY || y > graphHeight - paddingY) continue;

                        if (tickGridlines && ya != 0 /* The zero grid line is fully drawn even in 'tick' mode */)
                        {
                            g.DrawLine(Pens.Gray, paddingX, y, paddingX + 5, y);
                            g.DrawLine(Pens.Gray, graphWidth - padding - 5, y, graphWidth - padding, y);
                        }
                        else
                        {
                            g.DrawLine(Pens.Gray, paddingX, y, graphWidth - padding, y);
                        }
                        
                        if (ya == 0)
                        {
                            if (tickGridlines)
                            {
                                g.DrawLine(Pens.Gray, paddingX, y - 1, paddingX + 5, y - 1);
                                g.DrawLine(Pens.Gray, graphWidth - padding - 5, y - 1, graphWidth - padding, y - 1);
                                g.DrawLine(Pens.Gray, paddingX, y + 1, paddingX + 5, y + 1);
                                g.DrawLine(Pens.Gray, graphWidth - padding - 5, y + 1, graphWidth - padding, y + 1);
                            }
                            else
                            {
                                g.DrawLine(Pens.Gray, paddingX, y - 1, graphWidth - padding, y - 1);
                                g.DrawLine(Pens.Gray, paddingX, y + 1, graphWidth - padding, y + 1);
                            }
                            var sizF = g.MeasureString("0", DefaultFont);
                            g.DrawString("0", DefaultFont, Brushes.Black, paddingX - sizF.Width - 5, y - sizF.Height / 2);
                        }
                        else if (ya % 100 == 0)
                        {
                            var label = string.Format("{0:0.0} s", ya / 1000.0);
                            var sizF = g.MeasureString(label, DefaultFont);
                            g.DrawString(label, DefaultFont, Brushes.Black, paddingX - sizF.Width - 5, y - sizF.Height / 2);
                        }
                    }

                    int idx = 0;
                    float y1p = 0;
                    float y2p = 0;
                    float y3pt = 0;
                    float y3pb = 0;
                    float xp1 = 0;
                    float xp2 = 0;
                    float xp3 = 0;

                    float y1 = 0;
                    float y2 = 0;
                    float y3t = 0;
                    float y3b = 0;

                    foreach (var entry in m_TimeAnalyser.Entries)
                    {
                        if (entry.IsOutlier) continue;
                        if (m_PlotFromIndex.HasValue && m_PlotFromIndex.Value > idx)
                        {
                            idx++;
                            continue;
                        }
                        if (m_PlotToIndex.HasValue && m_PlotToIndex.Value < idx)
                        {
                            idx++;
                            continue;
                        }

                        float x = paddingX + (idx - minX) * xFactor;
                        bool calcOnly = idx == 0;

                        if (plotNtpError)
                        {
                            y3t = graphHeight - paddingY - yFactor * (entry.NTPErrorMs - minDelta);
                            y3b = graphHeight - paddingY - yFactor * (-entry.NTPErrorMs - minDelta);
                            if (!calcOnly && (x - xp3 > MIN_PIX_DIFF || Math.Abs(y3t - y3pt) > MIN_PIX_DIFF || Math.Abs(y3b - y3pb) > MIN_PIX_DIFF))
                            {
                                if (ellipses)
                                {
                                    g.FillEllipse(NtpErrorBrush, x - 1, y3t - 1, 2, 2);
                                    g.FillEllipse(NtpErrorBrush, x - 1, y3b - 1, 2, 2);
                                }
                                else
                                {
                                    g.DrawLine(NtpErrorPen, xp3, y3pt, x, y3t);
                                    g.DrawLine(NtpErrorPen, xp3, y3pb, x, y3b); 
                                }
                                xp3 = x;
                                y3pt = y3t;
                                y3pb = y3b;
                            }
                        }

                        if (plotOccuRecTime)
                        {
                            y1 = graphHeight - paddingY - yFactor * (entry.DeltaNTPTimeMs - minDelta);
                            if (!calcOnly && (x - xp1 > MIN_PIX_DIFF || Math.Abs(y1 - y1p) > MIN_PIX_DIFF))
                            {
                                if (ellipses)
                                {
                                    g.FillEllipse(NtpTimeBrush, x - 1, y1 - 1, 2, 2);
                                }
                                else
                                {
                                    g.DrawLine(NtpTimePen, xp1, y1p, x, y1);
                                }
                                xp1 = x;
                                y1p = y1;
                            }
                        }

                        if (plotSystemTime)
                            y2 = graphHeight - paddingY - yFactor * (entry.DeltaSystemTimeMs - minDelta);
                        else
                            y2 = graphHeight - paddingY - yFactor * (entry.DeltaSystemFileTimeMs - minDelta);

                        if (!calcOnly && (x - xp2 > MIN_PIX_DIFF || Math.Abs(y2 - y2p) > MIN_PIX_DIFF))
                        {
                            if (ellipses)
                            {
                                g.FillEllipse(SysTimeBrush, x - 1, y2 - 1, 2, 2);
                            }
                            else
                            {
                                g.DrawLine(SysTimePen, xp2, y2p, x, y2);
                            }
                            xp2 = x;
                            y2p = y2;
                        }

                        if (idx == 0)
                        {
                            y1p = y1;
                            y2p = y2;
                            y3pt = y3t;
                            y3pb = y3b;
                            xp1 = x;
                            xp2 = x;
                            xp3 = x;
                        }

                        idx++;
                    }

                    g.DrawRectangle(Pens.Black, paddingX, paddingY, graphWidth - padding - paddingX, graphHeight - 2 * paddingY);

                    var title = GetTitle(m_TimeAnalyser.Entries, "Time Delta analysis");
                    var sizeF = g.MeasureString(title, m_TitleFont);
                    g.DrawString(title, m_TitleFont, Brushes.Black, (width - sizeF.Width) / 2 + paddingX, (paddingY - sizeF.Height) / 2);

                    var thirdW = width / 3;
                    int legPos = -1;
                    for (int i = 0; i < 3; i++)
                    {
                        string legend = "";
                        Pen legendPen = Pens.Black;
                        if (i == 0)
                        {
                            legend = plotSystemTime ? "GetSystemTime()" : "GetSystemTimePreciseAsFileTime()";
                            legendPen = SysTimePen;
                        }
                        else if (i == 1)
                        {
                            if (plotOccuRecTime)
                            {
                                legend = "OccuRec's Time Keeping (NTP reference)";
                                legendPen = NtpTimePen;
                            }
                            else continue;
                        }
                        else if (i == 2)
                        {
                            if (plotNtpError)
                            {
                                legend = "Max 3-Sigma NTP Error";
                                legendPen = NtpErrorPen;
                            }
                            else continue;
                        }

                        legPos++;

                        sizeF = g.MeasureString(legend, m_TitleFont);
                        var y = paddingY + height + sizeF.Height / 2;
                        var yl = paddingY + height + sizeF.Height;
                        g.DrawString(legend, m_TitleFont, Brushes.Black, (thirdW - sizeF.Width) / 2 + paddingX + legPos * thirdW + 15, y);
                        g.DrawLine(legendPen, (thirdW - sizeF.Width) / 2 + paddingX + legPos * thirdW, yl - 1, 6 + (thirdW - sizeF.Width) / 2 + paddingX + legPos * thirdW, yl - 1);
                        g.DrawLine(legendPen, (thirdW - sizeF.Width) / 2 + paddingX + legPos * thirdW, yl, 6 + (thirdW - sizeF.Width) / 2 + paddingX + legPos * thirdW, yl);
                        g.DrawLine(legendPen, (thirdW - sizeF.Width) / 2 + paddingX + legPos * thirdW, yl + 1, 6 + (thirdW - sizeF.Width) / 2 + paddingX + legPos * thirdW, yl + 1);
                    }

                    g.Save();
                }
            }).ContinueWith((r) =>
            {
                if (r.IsCompleted)
                {
                    this.Invoke(new Action(() =>
                    {
                        if (pbGraph.Image != null)
                        {
                            pbGraph.Image.Dispose();
                        }

                        pbGraph.Image = image;
                        pbGraph.Update();

                        m_LastGraphWidth = pbGraph.Width;
                        m_LastGraphHeight = pbGraph.Height;

                        Cursor = Cursors.Default;
                    }));
                }
                else
                {
                    Cursor = Cursors.Default;
                }
            });
        }

        private void DrawZoomedTimeDeltasGraph()
        {
            Cursor = Cursors.WaitCursor;

            Pen MedianPen = new Pen(Brushes.Blue, 2);
            MedianPen.DashCap = DashCap.Round;
            Brush MedianBrush = Brushes.Blue;
            Brush SysTimeBrush = Brushes.Chocolate;
            Pen SysTimePen = Pens.Chocolate;
            int densityThreshold = (int)nudDensityThreshold.Value;

            int graphWidth = pbGraph.Width;
            int graphHeight = pbGraph.Height;
            var image = new Bitmap(graphWidth, graphHeight, PixelFormat.Format24bppRgb);
            var densityMonitor = new int[graphWidth, graphHeight];

            bool ellipses = !m_GraphConfig.ConnectionLines;
            bool tickGridlines = m_GraphConfig.GridlineStyle == GridlineStyle.Tick;
            float yScalePaddingCoeff = 1.2f;

            Task.Run(() =>
            {
                bool isSubset = m_PlotFromIndex.HasValue && m_PlotToIndex.HasValue;
                List<TimeAnalyserEntry> subsetEnum = null;
                if (isSubset)
                {
                    subsetEnum = m_TimeAnalyser.Entries.Skip(m_PlotFromIndex.Value).Take(m_PlotToIndex.Value - m_PlotFromIndex.Value).ToList();
                }
                else
                {
                    subsetEnum = m_TimeAnalyser.Entries;
                }

                var deltas = subsetEnum.Where(x => !x.IsOutlier).Select(x => x.DeltaSystemFileTimeMs).ToList();

                int idx = 0;

                float minDeltaTryRun = float.MaxValue;
                float maxDeltaTryRun = float.MinValue;

                if (densityThreshold > 0)
                {
                    // Dry run to determine actual min/max Y values

                    float xFactorDry = graphWidth * 1.0f / (subsetEnum.Count() - 1 - 0 + 1);
                    float minDeltaDry = deltas.Min();
                    float maxDeltaDry = deltas.Max();
                    float yFactorDry = graphHeight / (maxDeltaDry - minDeltaDry);
                    var densityMonitorDry = new int[graphWidth, graphHeight];

                    foreach (var entry in subsetEnum)
                    {
                        if (entry.IsOutlier) continue;

                        float x = idx * xFactorDry;
                        float y = graphHeight - yFactorDry * (entry.DeltaSystemFileTimeMs - minDeltaDry);

                        int xi = (int)x;
                        int yi = (int)y;

                        if (xi >= 0 && xi < graphWidth && yi >= 0 && yi < graphHeight && densityMonitorDry[xi, yi] <= densityThreshold)
                        {
                            densityMonitorDry[xi, yi]++;

                            if (densityMonitorDry[xi, yi] > densityThreshold)
                            {
                                if (minDeltaTryRun > entry.DeltaSystemFileTimeMs) minDeltaTryRun = entry.DeltaSystemFileTimeMs;
                                if (maxDeltaTryRun < entry.DeltaSystemFileTimeMs) maxDeltaTryRun = entry.DeltaSystemFileTimeMs;
                            }
                        }

                        idx++;
                    }
                }

                float minDelta = densityThreshold > 0 ? minDeltaTryRun : deltas.Min();
                float maxDelta = densityThreshold > 0 ? maxDeltaTryRun : deltas.Max();

                float b = (maxDelta - minDelta) * yScalePaddingCoeff;
                float m = (maxDelta + minDelta)/2;
                minDelta = m - b/2.0f;
                maxDelta = m + b/2.0f;
                TimeSpan medianInterval = TimeSpan.FromMinutes((int)nudMedianInterval.Value);
                DateTime? medianStartInterval = null;
                int medianStartIntervalIndex = 0;
                List<float> medianCalsList = new List<float>();

                using (var g = Graphics.FromImage(image))
                {
                    float padding = 10;
                    float paddingY = 25;
                    float height = graphHeight - 2 * paddingY;
                    float yFactor = height / (maxDelta - minDelta);

                    int interval = GetReadableLabelYAxisTickInterval(g, yFactor);

                    int yGridFrom = (int)Math.Floor((m - b) / interval) * interval;
                    int yGridTo = (int)Math.Ceiling((m + b) / interval) * interval;

                    float paddingX = GetXAsisPaddingToFitYLabels(g, yGridFrom, yGridTo);
                    float width = graphWidth - padding - paddingX;

                    float minX = 0;
                    float maxX = subsetEnum.Count - 1;
                    float xFactor = width / (maxX - minX + 1);

                    g.FillRectangle(Brushes.WhiteSmoke, 0, 0, graphWidth, graphHeight);

                    idx = 0;

                    float y2 = 0;
                    float xp2 = 0;
                    float y2p = 0;

                    var totalHours = (float)(subsetEnum[subsetEnum.Count - 1].SystemTimeFileTime - subsetEnum[0].SystemTimeFileTime).TotalHours;
                    int? intervalX = GetReadableLabelXAxisTickInterval(g, xFactor * subsetEnum.Count / totalHours, totalHours);
                    var nextTick = DateTime.MaxValue;
                    if (intervalX != null)
                    {
                        nextTick = subsetEnum[0].SystemTimeFileTime.Date;
                        while (nextTick < subsetEnum[0].SystemTimeFileTime)
                        {
                            nextTick = nextTick.AddSeconds(intervalX.Value);
                        }
                    }

                    var medianData = new List<Tuple<float, float>>();

                    if (m_TimeAnalyser.MeinbergAdvLog.Count > 1)
                    {
                        peerPens.Clear();
                        var timeInterval = subsetEnum[subsetEnum.Count - 1].ReferenceTime - subsetEnum[0].ReferenceTime;
                        
                        for (int i = 1; i < m_TimeAnalyser.MeinbergAdvLog.Count; i++)
                        {
                            var meinbergLogPrev = m_TimeAnalyser.MeinbergAdvLog[i - 1];
                            var meinbergLog = m_TimeAnalyser.MeinbergAdvLog[i];
                            if (meinbergLog.PeerIP != meinbergLogPrev.PeerIP || meinbergLog.PeerIP == null)
                            {
                                var x = paddingX + (float)(width * (meinbergLogPrev.Time - subsetEnum[0].ReferenceTime).TotalSeconds / timeInterval.TotalSeconds);

                                // Peer IP Changed
                                g.DrawLine(GetPenForPeerIP(meinbergLog.PeerIP), x, paddingY + 1, x, graphHeight - paddingY);
                            }
                        }
                    }

                    foreach (var entry in subsetEnum)
                    {
                        if (entry.IsOutlier) continue;

                        if (!medianStartInterval.HasValue)
                        {
                            medianStartInterval = entry.SystemTimeFileTime;
                            medianStartIntervalIndex = idx;
                        }

                        float x = paddingX + (idx - minX) * xFactor;
                        bool calcOnly = idx == 0;

                        y2 = graphHeight - paddingY - yFactor * (entry.DeltaSystemFileTimeMs - minDelta);

                        if (!calcOnly && (x - xp2 > MIN_PIX_DIFF || Math.Abs(y2 - y2p) > MIN_PIX_DIFF))
                        {
                            int xi = (int) x;
                            int yi = (int) y2;
                            if (xi < 0) xi = 0; if (xi > graphWidth - 1) xi = graphWidth - 1;
                            if (yi < 0) yi = 0; if (yi > graphHeight - 1) yi = graphHeight - 1;

                            if (densityMonitor[xi, yi] <= densityThreshold)
                            {
                                densityMonitor[xi, yi]++;

                                if (ellipses)
                                {
                                    if (densityMonitor[xi, yi] > densityThreshold)
                                    {
                                        g.FillEllipse(SysTimeBrush, x - 1, y2 - 1, 2, 2);
                                    }
                                }
                                else
                                {
                                    g.DrawLine(SysTimePen, xp2, y2p, x, y2);
                                }
                            }

                            xp2 = x;
                            y2p = y2;
                        }

                        if (idx == 0)
                        {
                            y2p = y2;
                            xp2 = x;
                        }

                        if (medianInterval != TimeSpan.Zero)
                        {
                            if (entry.SystemTimeFileTime - medianStartInterval > medianInterval)
                            {
                                var median = medianCalsList.Median();
                                var ym = graphHeight - paddingY - yFactor * (median - minDelta);
                                var xm = paddingX + ((idx + medianStartIntervalIndex) / 2.0f - minX) * xFactor;

                                medianData.Add(Tuple.Create(xm, ym));

                                medianCalsList.Clear();
                                medianStartInterval = entry.SystemTimeFileTime;
                                medianStartIntervalIndex = idx;
                            }
                            else
                            {
                                medianCalsList.Add(entry.DeltaSystemFileTimeMs);
                            }
                        }

                        if (intervalX != null && nextTick < entry.SystemTimeFileTime)
                        {
                            g.DrawLine(Pens.Gray, x, graphHeight - paddingY + 1, x, graphHeight - paddingY - 5);

                            var label = intervalX.Value >= 3600 ? string.Format("{0} UT", nextTick.Hour) : (intervalX.Value >= 60 ? string.Format("{0}:{1:00} UT", nextTick.Hour, nextTick.Minute) : string.Format("{0}:{1:00}:{2:00} UT", nextTick.Hour, nextTick.Minute, nextTick.Second));
                            var lblSiz = g.MeasureString(label, DefaultFont);
                            g.DrawString(label, DefaultFont, Brushes.Black, x - lblSiz.Width / 2, graphHeight - paddingY + 5);

                            nextTick = nextTick.AddSeconds(intervalX.Value);
                        }

                        idx++;
                    }

                    if (medianInterval != TimeSpan.Zero)
                    {
                        for (int i = 0; i < medianData.Count - 1; i++)
                        {
                            g.FillEllipse(MedianBrush, medianData[i].Item1 - 2, medianData[i].Item2 - 2, 5, 5);
                            g.DrawLine(MedianPen, medianData[i].Item1, medianData[i].Item2, medianData[i + 1].Item1, medianData[i + 1].Item2);
                        }
                    }

                    SizeF sizF;
                    for (int ya = yGridFrom; ya < yGridTo; ya += interval)
                    {
                        float y = graphHeight - paddingY - yFactor * (ya - minDelta);
                        if (y < paddingY || y > graphHeight - paddingY) continue;

                        if (tickGridlines && ya != 0 /* The zero grid line is fully drawn even in 'tick' mode */)
                        {
                            g.DrawLine(Pens.Gray, paddingX, y, paddingX + 5, y);
                            g.DrawLine(Pens.Gray, graphWidth - padding - 5, y, graphWidth - padding, y);
                        }
                        else
                        {
                            g.DrawLine(Pens.Gray, paddingX, y, graphWidth - padding, y);
                        }

                        if (ya == 0)
                        {
                            if (tickGridlines)
                            {
                                g.DrawLine(Pens.Gray, paddingX, y - 1, paddingX + 5, y - 1);
                                g.DrawLine(Pens.Gray, graphWidth - padding - 5, y - 1, graphWidth - padding, y - 1);
                                g.DrawLine(Pens.Gray, paddingX, y + 1, paddingX + 5, y + 1);
                                g.DrawLine(Pens.Gray, graphWidth - padding - 5, y + 1, graphWidth - padding, y + 1);
                            }
                            else
                            {
                                g.DrawLine(Pens.Gray, paddingX, y - 1, graphWidth - padding, y - 1);
                                g.DrawLine(Pens.Gray, paddingX, y + 1, graphWidth - padding, y + 1);
                            }
                            sizF = g.MeasureString("0", DefaultFont);
                            g.DrawString("0", DefaultFont, Brushes.Black, paddingX - sizF.Width - 5, y - sizF.Height / 2);
                        }
                        else
                        {
                            var label = string.Format("{0} ms", ya);
                            sizF = g.MeasureString(label, DefaultFont);
                            g.DrawString(label, DefaultFont, Brushes.Black, paddingX - sizF.Width - 5, y - sizF.Height / 2);
                        }
                    }

                    g.DrawRectangle(Pens.Black, paddingX, paddingY, graphWidth - padding - paddingX, graphHeight - 2 * paddingY);

                    var title = GetTitle(subsetEnum, "Time Delta analysis");
                    var sizeF = g.MeasureString(title, m_TitleFont);
                    g.DrawString(title, m_TitleFont, Brushes.Black, (width - sizeF.Width) / 2 + paddingX, (paddingY - sizeF.Height) / 2);
                }
            })
            .ContinueWith((r) =>
            {
                if (r.IsCompleted)
                {
                    this.Invoke(new Action(() =>
                    {
                        if (pbGraph.Image != null)
                        {
                            pbGraph.Image.Dispose();
                        }

                        pbGraph.Image = image;
                        pbGraph.Update();

                        m_LastGraphWidth = pbGraph.Width;
                        m_LastGraphHeight = pbGraph.Height;

                        Cursor = Cursors.Default;
                    }));
                }
                else
                {
                    Cursor = Cursors.Default;
                }
            });
        }

        Dictionary<string, Pen> peerPens = new Dictionary<string, Pen>(); 

        private Pen GetPenForPeerIP(string ipAddress)
        {
            if (ipAddress ==  null) return Pens.DarkGray;

            var pens = new Pen[] { Pens.Blue, Pens.Green, Pens.Purple, Pens.Aqua, Pens.Crimson, Pens.DodgerBlue, Pens.HotPink };
            Pen pen;
            if (peerPens.Count >= pens.Length) return Pens.Black;
            if (peerPens.TryGetValue(ipAddress, out pen)) return pen;
            pen = pens[peerPens.Count];
            peerPens[ipAddress] = pen;
            return pen;
        }

        private int GetReadableLabelYAxisTickInterval(Graphics g, float yFactor)
        {
            var sizF = g.MeasureString("123", DefaultFont);
            var probeInts = new [] { 1, 2, 5, 10, 20, 25, 50, 100, 200, 250, 500, 1000, 2000 };
            for (int i = 0; i < probeInts.Length; i++)
            {
                if (probeInts[i] * yFactor > sizF.Height*2)
                {
                    return probeInts[i];
                }
            }

            return (int)Math.Pow(10, (int) Math.Ceiling(Math.Log10(sizF.Height*2)));
        }

        private int? GetReadableLabelXAxisTickInterval(Graphics g, float xFactorPerHour, float totalHours)
        {
            if (totalHours > 4)
            {
                var sizF = g.MeasureString("12 UT", DefaultFont);

                var probeInts = new[] { 1, 2, 3, 5 };
                for (int i = 0; i < probeInts.Length; i++)
                {
                    if (probeInts[i] * xFactorPerHour > sizF.Width * 2)
                    {
                        return probeInts[i] * 3600;
                    }
                }
            }
            else if (totalHours > 0.03333333 /*2 min*/)
            {
                var sizF = g.MeasureString("12:30 UT", DefaultFont);
                var probeInts = new[] { 1, 5, 10, 20, 30 };
                for (int i = 0; i < probeInts.Length; i++)
                {
                    if (probeInts[i] * xFactorPerHour / 60 > sizF.Width * 2)
                    {
                        return probeInts[i] * 60;
                    }
                }
            }
            else
            {
                var sizF = g.MeasureString("12:30:30 UT", DefaultFont);
                var probeInts = new[] { 1, 5, 10, 20, 30 };
                for (int i = 0; i < probeInts.Length; i++)
                {
                    if (probeInts[i] * xFactorPerHour / 3600 > sizF.Width * 2)
                    {
                        return probeInts[i];
                    }
                }
            }

            return null;
        }

        private int GetXAsisPaddingToFitYLabels(Graphics g, int yFrom, int yTo)
        {
            var sizF1 = g.MeasureString(string.Format("{0} ms", yFrom), DefaultFont);
            var sizF2 = g.MeasureString(string.Format("{0} ms", yTo), DefaultFont);

            return (int) Math.Ceiling(Math.Max(sizF1.Width, sizF2.Width)) + 10;
        }

        private void DrawDeltaBucketsGraph()
        {
            Cursor = Cursors.WaitCursor;

            Brush SysTimeBrush = Brushes.Chocolate;
            Pen BucketBorderPen = Pens.Blue;

            int graphWidth = pbGraph.Width;
            int graphHeight = pbGraph.Height;
            var image = new Bitmap(graphWidth, graphHeight, PixelFormat.Format24bppRgb);

            float bucketSize = (float)nudDeltaBucketInterval.Value;

            Task.Run(() =>
            {
                var distribution = new Dictionary<int, float>();
                var deltas = m_TimeAnalyser.Entries.Where(x => !x.IsOutlier).Select(x => x.DeltaSystemFileTimeMs).ToList();
                var median = deltas.Median();
                for (int i = 0; i < deltas.Count; i++)
                {
                    var offset = deltas[i] - median;
                    int bucketSign = Math.Sign(offset);
                    offset = Math.Abs(offset);

                    int bucketNo = 0;
                    offset = (offset - bucketSize/2.0f);
                    if (offset > 0) bucketNo = bucketSign * (1 + (int)Math.Floor(offset / bucketSize));
                    if (distribution.ContainsKey(bucketNo))
                    {
                        distribution[bucketNo]++;
                    }
                    else
                    {
                        distribution[bucketNo] = 1;
                    }
                }

                var allValues = distribution.Values.ToArray();

                // Normalisation
                var totalDataPoints = allValues.Sum();
                foreach (var key in distribution.Keys.ToArray())
                {
                    distribution[key] = distribution[key] * 100.0f / totalDataPoints;
                    // Remove entried smaller than 0.1%
                    if (distribution[key] < 0.1) distribution.Remove(key);
                }

                var allKeys = distribution.Keys.ToArray();
                float minX = allKeys.Min() - 2;
                float maxX = allKeys.Max() + 1;

                

                float padding = 10;
                float paddingY = 25;
                float height = graphHeight - 2 * paddingY;
                float yFactor = height / (distribution.Values.Max() * 1.25f);

                float paddingX = 50;
                float width = graphWidth - padding - paddingX;

                float xFactor = width / (maxX - minX);

                using (var g = Graphics.FromImage(image))
                {
                    g.FillRectangle(Brushes.WhiteSmoke, 0, 0, graphWidth, graphHeight);

                    for (int i = 0; i < 100; i++)
                    {
                        var y = graphHeight - paddingY - yFactor * i;
                        if (y < paddingY) break;

                        g.DrawLine(Pens.Gray, paddingX, y, graphWidth - padding, y);
                        if (i % 5 == 0)
                        {
                            g.DrawLine(Pens.Gray, paddingX, y + 1, graphWidth - padding, y + 1);
                            var label = string.Format("{0} %", i);
                            var lblSiz = g.MeasureString(label, DefaultFont);
                            g.DrawString(label, DefaultFont, Brushes.Black, paddingX - lblSiz.Width - 5, y - lblSiz.Height / 2);
                        }
                    }

                    foreach (var kvp in distribution)
                    {
                        var key = kvp.Key;
                        var x1 = paddingX + ((key - 1) - minX) * xFactor;
                        var y = graphHeight - paddingY - yFactor * kvp.Value;
                        g.FillRectangle(SysTimeBrush, x1, y, xFactor, graphHeight - paddingY - y);
                        g.DrawRectangle(BucketBorderPen, x1, y, xFactor, graphHeight - paddingY - y);

                        var xTick = x1 + xFactor / 2.0f;
                        g.DrawLine(Pens.Gray, xTick, graphHeight - paddingY + 3, xTick, graphHeight - paddingY - 2);

                        // TODO: Check if labels should be every, every second, every third etc tick based on label width
                        var label = key == 0 ? "0" : string.Format("{0} ms", key * bucketSize);
                        if (key > 0) label = "+" + label;
                        var lblSiz = g.MeasureString(label, DefaultFont);
                        g.DrawString(label, DefaultFont, Brushes.Black, xTick - lblSiz.Width / 2, graphHeight - paddingY + 5);
                    }

                    g.DrawRectangle(Pens.Black, paddingX, paddingY, graphWidth - padding - paddingX, graphHeight - 2 * paddingY);

                    var title = GetTitle(m_TimeAnalyser.Entries, "Destribution around median");
                    var sizeF = g.MeasureString(title, m_TitleFont);
                    g.DrawString(title, m_TitleFont, Brushes.Black, (width - sizeF.Width) / 2 + paddingX, (paddingY - sizeF.Height) / 2);
                }
            })
            .ContinueWith((r) =>
            {
                if (r.IsCompleted)
                {
                    this.Invoke(new Action(() =>
                    {
                        if (pbGraph.Image != null)
                        {
                            pbGraph.Image.Dispose();
                        }

                        pbGraph.Image = image;
                        pbGraph.Update();

                        m_LastGraphWidth = pbGraph.Width;
                        m_LastGraphHeight = pbGraph.Height;

                        Cursor = Cursors.Default;
                    }));
                }
                else
                {
                    Cursor = Cursors.Default;
                }
            });
        }

        private void DrawSystemUtilisationGraph()
        {
            Cursor = Cursors.WaitCursor;

            int graphWidth = pbGraph.Width;
            int graphHeight = pbGraph.Height;

            bool tickGridlines = m_GraphConfig.GridlineStyle == GridlineStyle.Tick;

            var image = new Bitmap(graphWidth, graphHeight, PixelFormat.Format24bppRgb);

            Task.Run(() =>
            {
                Pen CpuPen = Pens.Blue;
                Pen DisksPen = Pens.Red;

                using (var g = Graphics.FromImage(image))
                {
                    float padding = 10;
                    float paddingX = 45;
                    float paddingY = 25;
                    float width = graphWidth - padding - paddingX;
                    float height = graphHeight - 2*paddingY;
                    float yFactor = height/100.0f; // 100% Max
                    float minX = 0;
                    float maxX = m_TimeAnalyser.SystemUtilisation.Count - 1;
                    float xFactor = width/(maxX - minX + 1);

                    g.FillRectangle(Brushes.WhiteSmoke, 0, 0, graphWidth, graphHeight);

                    for (int ya = 0; ya <= 100; ya += 10)
                    {
                        float y = graphHeight - paddingY - yFactor * ya;
                        if (y < paddingY || y > graphHeight - paddingY) continue;

                        if (tickGridlines && ya != 0 /* The zero grid line is fully drawn even in 'tick' mode */)
                        {
                            g.DrawLine(Pens.Gray, paddingX, y, paddingX + 5, y);
                            g.DrawLine(Pens.Gray, graphWidth - padding - 5, y, graphWidth - padding, y);
                        }
                        else
                        {
                            g.DrawLine(Pens.Gray, paddingX, y, graphWidth - padding, y);
                        }

                        if (ya % 20 == 0)
                        {
                            var label = string.Format("{0}%", ya);
                            var sizF = g.MeasureString(label, DefaultFont);
                            g.DrawString(label, DefaultFont, Brushes.Black, paddingX - sizF.Width - 5, y - sizF.Height / 2);
                        }
                    }

                    int idx = 0;
                    float xp = 0;
                    float y1p = 0;
                    float y2p = 0;

                    foreach (var utilEntry in m_TimeAnalyser.SystemUtilisation)
                    {
                        float x = paddingX + idx * xFactor;
                        float y1 = graphHeight - paddingY - yFactor * utilEntry.CpuUtilisation;
                        float y2 = graphHeight - paddingY - yFactor * Math.Min(100, utilEntry.DiskUtilisation); // NOTE: Disk utilisation of more than 100% is possible when there are more than 1 disks

                        if (idx > 0)
                        {
                            g.DrawLine(CpuPen, xp, y1p, x, y1);
                            g.DrawLine(DisksPen, xp, y2p, x, y2);
                        }

                        xp = x;
                        y1p = y1;
                        y2p = y2;

                        idx++;
                    }

                    g.DrawRectangle(Pens.Black, paddingX, paddingY, graphWidth - padding - paddingX, graphHeight - 2 * paddingY);

                    var title = string.Format("System utilisation between {0} UT and {1} UT", m_TimeAnalyser.FromDateTime.ToString("dd-MMM HH:mm"), m_TimeAnalyser.ToDateTime.ToString("dd-MMM HH:mm"));
                    var sizeF = g.MeasureString(title, m_TitleFont);
                    g.DrawString(title, m_TitleFont, Brushes.Black, (width - sizeF.Width) / 2 + paddingX, (paddingY - sizeF.Height) / 2);

                    var thirdW = width / 3;
                    int legPos = -1;
                    for (int i = 0; i < 2; i++)
                    {
                        string legend = "";
                        Pen legendPen = Pens.Black;
                        if (i == 0)
                        {
                            legend = "CPU Utilisation";
                            legendPen = CpuPen;
                        }
                        else if (i == 1)
                        {
                            legend = "All Disks Utilisation";
                            legendPen = DisksPen;
                        }

                        legPos++;

                        sizeF = g.MeasureString(legend, m_TitleFont);
                        var y = paddingY + height + sizeF.Height / 2;
                        var yl = paddingY + height + sizeF.Height;
                        g.DrawString(legend, m_TitleFont, Brushes.Black, (thirdW - sizeF.Width) / 2 + paddingX + legPos * thirdW + 15, y);
                        g.DrawLine(legendPen, (thirdW - sizeF.Width) / 2 + paddingX + legPos * thirdW, yl - 1, 6 + (thirdW - sizeF.Width) / 2 + paddingX + legPos * thirdW, yl - 1);
                        g.DrawLine(legendPen, (thirdW - sizeF.Width) / 2 + paddingX + legPos * thirdW, yl, 6 + (thirdW - sizeF.Width) / 2 + paddingX + legPos * thirdW, yl);
                        g.DrawLine(legendPen, (thirdW - sizeF.Width) / 2 + paddingX + legPos * thirdW, yl + 1, 6 + (thirdW - sizeF.Width) / 2 + paddingX + legPos * thirdW, yl + 1);
                    }

                    g.Save();
                }

            }).ContinueWith((r) =>
            {
                if (r.IsCompleted)
                {
                    this.Invoke(new Action(() =>
                    {
                        if (pbGraph.Image != null)
                        {
                            pbGraph.Image.Dispose();
                        }

                        pbGraph.Image = image;
                        pbGraph.Update();

                        m_LastGraphWidth = pbGraph.Width;
                        m_LastGraphHeight = pbGraph.Height;

                        Cursor = Cursors.Default;
                    }));
                }
                else
                {
                    Cursor = Cursors.Default;
                }
            });
        }

        private void DrawNtpUpdatesGraph()
        {
            Cursor = Cursors.WaitCursor;

            int graphWidth = pbGraph.Width;
            int graphHeight = pbGraph.Height;

            bool tickGridlines = m_GraphConfig.GridlineStyle == GridlineStyle.Tick;
            bool includeUnapplied = m_GrapthType == GraphType.gtNtpUpdatesInclUnapplied;

            var image = new Bitmap(graphWidth, graphHeight, PixelFormat.Format24bppRgb);

            Task.Run(() =>
            {
                Pen DeltaPen = Pens.DarkOrchid;
                Pen LatencyPen = Pens.Red;

                var minDelta = m_TimeAnalyser.NtpUpdates.Where(x => x.Updated || includeUnapplied).Min(x => x.Delta);
                var maxDelta = m_TimeAnalyser.NtpUpdates.Where(x => x.Updated || includeUnapplied).Max(x => x.Delta);
                var minLatency = m_TimeAnalyser.NtpUpdates.Min(x => x.Latency);
                var maxLatency = m_TimeAnalyser.NtpUpdates.Max(x => x.Latency);

                var minY = Math.Min(minLatency, minDelta);
                var maxY = Math.Max(maxLatency, maxDelta);

                using (var g = Graphics.FromImage(image))
                {
                    float padding = 10;
                    float paddingX = 45;
                    float paddingY = 25;
                    float width = graphWidth - padding - paddingX;
                    float height = graphHeight - 2 * paddingY;
                    float yFactor = height / (1.2f * (maxY - minY));
                    float minX = 0;
                    float maxX = m_TimeAnalyser.NtpUpdates.Count - 1;
                    float xFactor = width / (maxX - minX + 1);

                    g.FillRectangle(Brushes.WhiteSmoke, 0, 0, graphWidth, graphHeight);

                    for (int ya = -2000; ya < 2000; ya += 100)
                    {
                        float y = graphHeight - paddingY - yFactor * (ya - minY);
                        if (y < paddingY || y > graphHeight - paddingY) continue;

                        if (tickGridlines && ya != 0 /* The zero grid line is fully drawn even in 'tick' mode */)
                        {
                            g.DrawLine(Pens.Gray, paddingX, y, paddingX + 5, y);
                            g.DrawLine(Pens.Gray, graphWidth - padding - 5, y, graphWidth - padding, y);
                        }
                        else
                        {
                            g.DrawLine(Pens.Gray, paddingX, y, graphWidth - padding, y);
                        }

                        if (ya == 0)
                        {
                            if (tickGridlines)
                            {
                                g.DrawLine(Pens.Gray, paddingX, y - 1, paddingX + 5, y - 1);
                                g.DrawLine(Pens.Gray, graphWidth - padding - 5, y - 1, graphWidth - padding, y - 1);
                                g.DrawLine(Pens.Gray, paddingX, y + 1, paddingX + 5, y + 1);
                                g.DrawLine(Pens.Gray, graphWidth - padding - 5, y + 1, graphWidth - padding, y + 1);
                            }
                            else
                            {
                                g.DrawLine(Pens.Gray, paddingX, y - 1, graphWidth - padding, y - 1);
                                g.DrawLine(Pens.Gray, paddingX, y + 1, graphWidth - padding, y + 1);
                            }
                            var sizF = g.MeasureString("0", DefaultFont);
                            g.DrawString("0", DefaultFont, Brushes.Black, paddingX - sizF.Width - 5, y - sizF.Height / 2);
                        }
                        else if (ya % 100 == 0)
                        {
                            var label = string.Format("{0:0.0} s", ya / 1000.0);
                            var sizF = g.MeasureString(label, DefaultFont);
                            g.DrawString(label, DefaultFont, Brushes.Black, paddingX - sizF.Width - 5, y - sizF.Height / 2);
                        }
                    }

                    int idx = 0;
                    float x1p = 0;
                    float x2p = 0;
                    float y1p = 0;
                    float y2p = 0;

                    foreach (var utilEntry in m_TimeAnalyser.NtpUpdates)
                    {
                        float x = paddingX + idx * xFactor;
                        float y1 = graphHeight - paddingY - yFactor * (utilEntry.Delta - minY);
                        float y2 = graphHeight - paddingY - yFactor * (utilEntry.Latency - minY);

                        if (idx > 0)
                        {
                            if (utilEntry.Updated || includeUnapplied)
                            {
                                g.DrawLine(DeltaPen, x1p, y1p, x, y1);
                            }

                            g.DrawLine(LatencyPen, x2p, y2p, x, y2);
                        }

                        if (utilEntry.Updated || includeUnapplied)
                        {
                            y1p = y1;
                            x1p = x;
                        }

                        x2p = x;
                        y2p = y2;

                        idx++;
                    }

                    g.DrawRectangle(Pens.Black, paddingX, paddingY, graphWidth - padding - paddingX, graphHeight - 2 * paddingY);

                    var title = string.Format("OccuRec NTP updates between {0} UT and {1} UT", m_TimeAnalyser.FromDateTime.ToString("dd-MMM HH:mm"), m_TimeAnalyser.ToDateTime.ToString("dd-MMM HH:mm"));
                    var sizeF = g.MeasureString(title, m_TitleFont);
                    g.DrawString(title, m_TitleFont, Brushes.Black, (width - sizeF.Width) / 2 + paddingX, (paddingY - sizeF.Height) / 2);

                    var thirdW = width / 3;
                    int legPos = -1;
                    for (int i = 0; i < 2; i++)
                    {
                        string legend = "";
                        Pen legendPen = Pens.Black;
                        if (i == 0)
                        {
                            legend = includeUnapplied ? "NTP Deltas, Including Unapplied (ms)" : "NTP Deltas (ms)";
                            legendPen = DeltaPen;
                        }
                        else if (i == 1)
                        {
                            legend = "Latency (ms)";
                            legendPen = LatencyPen;
                        }

                        legPos++;

                        sizeF = g.MeasureString(legend, m_TitleFont);
                        var y = paddingY + height + sizeF.Height / 2;
                        var yl = paddingY + height + sizeF.Height;
                        g.DrawString(legend, m_TitleFont, Brushes.Black, (thirdW - sizeF.Width) / 2 + paddingX + legPos * thirdW + 15, y);
                        g.DrawLine(legendPen, (thirdW - sizeF.Width) / 2 + paddingX + legPos * thirdW, yl - 1, 6 + (thirdW - sizeF.Width) / 2 + paddingX + legPos * thirdW, yl - 1);
                        g.DrawLine(legendPen, (thirdW - sizeF.Width) / 2 + paddingX + legPos * thirdW, yl, 6 + (thirdW - sizeF.Width) / 2 + paddingX + legPos * thirdW, yl);
                        g.DrawLine(legendPen, (thirdW - sizeF.Width) / 2 + paddingX + legPos * thirdW, yl + 1, 6 + (thirdW - sizeF.Width) / 2 + paddingX + legPos * thirdW, yl + 1);
                    }

                    g.Save();
                }

            }).ContinueWith((r) =>
            {
                if (r.IsCompleted)
                {
                    this.Invoke(new Action(() =>
                    {
                        if (pbGraph.Image != null)
                        {
                            pbGraph.Image.Dispose();
                        }

                        pbGraph.Image = image;
                        pbGraph.Update();

                        m_LastGraphWidth = pbGraph.Width;
                        m_LastGraphHeight = pbGraph.Height;

                        Cursor = Cursors.Default;
                    }));
                }
                else
                {
                    Cursor = Cursors.Default;
                }
            });
        }

        private void resizeUpdateTimer_Tick(object sender, EventArgs e)
        {
            resizeUpdateTimer.Enabled = false;

            if (m_LastGraphWidth != pbGraph.Width || m_LastGraphHeight != pbGraph.Height)
            {
                DrawGraph();
            }
        }

        private void frmAavStatusChannelOnlyView_ResizeEnd(object sender, EventArgs e)
        {
            resizeUpdateTimer.Enabled = true;
        }

        private void cbxGraphType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxGraphType.SelectedIndex == 0)
            {
                m_GrapthType = GraphType.gtTimeDeltas;
                pnlTimeDeltaConfig.Visible = true;
                pnlTimeMedianConfig.Visible = false;
                pnlTimeBucketsConfig.Visible = false;
            }
            else if (cbxGraphType.SelectedIndex == 1)
            {
                m_GrapthType = GraphType.gtSystemUtilisation;
                pnlTimeDeltaConfig.Visible = false;
                pnlTimeMedianConfig.Visible = false;
                pnlTimeBucketsConfig.Visible = false;
            }
            else if (cbxGraphType.SelectedIndex == 2)
            {
                m_GrapthType = GraphType.gtNtpUpdates;
                pnlTimeDeltaConfig.Visible = false;
                pnlTimeMedianConfig.Visible = false;
                pnlTimeBucketsConfig.Visible = false;
            }
            else if (cbxGraphType.SelectedIndex == 3)
            {
                m_GrapthType = GraphType.gtNtpUpdatesInclUnapplied;
                pnlTimeDeltaConfig.Visible = false;
                pnlTimeMedianConfig.Visible = false;
                pnlTimeBucketsConfig.Visible = false;
            }
            else if (cbxGraphType.SelectedIndex == 4)
            {
                m_GrapthType = GraphType.gtZoomedTotalDelta;
                pnlTimeDeltaConfig.Visible = false;
                pnlTimeMedianConfig.Visible = true;
                pnlTimeBucketsConfig.Visible = false;
            }
            else if (cbxGraphType.SelectedIndex == 5)
            {
                m_GrapthType = GraphType.gtDeltaBucketIntervals;
                pnlTimeDeltaConfig.Visible = false;
                pnlTimeMedianConfig.Visible = false;
                pnlTimeBucketsConfig.Visible = true;
            }

            DrawGraph();
        }

        private void TimeDeltasTimeSourceChanged(object sender, EventArgs e)
        {
            if (m_GrapthType != GraphType.gtNone)
            {
                DrawGraph();
            }
        }

        private void GridlinesStyleChanged(object sender, EventArgs e)
        {
            if (sender == miCompleteGridlines)
            {
                m_GraphConfig.GridlineStyle = GridlineStyle.Line;
            }
            else if (sender == miTickGridlines)
            {
                m_GraphConfig.GridlineStyle = GridlineStyle.Tick;
            }

            foreach (ToolStripMenuItem item in ((sender as ToolStripMenuItem).OwnerItem as ToolStripMenuItem).DropDownItems)
            {
                item.Checked = (item == (sender as ToolStripMenuItem));
            }

            DrawGraph();
        }

        private void ShowOcrErrorFrame()
        {
            if (!m_IsDataReady || m_TimeAnalyser.DebugFrames.Count == 0) return;

            var frame = m_TimeAnalyser.DebugFrames[(int)nudOcrErrorFrame.Value];
            lblOcrText.Text = string.Format("[{0}]           [{1}]", frame.OrcField1, frame.OrcField2);
            pbOcrErrorFrame.Image = frame.DebugImage;
        }

        private void nudOcrErrorFrame_ValueChanged(object sender, EventArgs e)
        {
            ShowOcrErrorFrame();
        }

        private string m_ExportFileName;

        private void miExport_Click(object sender, EventArgs e)
        {
            saveFileDialog.FileName = Path.ChangeExtension(m_Aav.FileName, "csv");

            if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                m_ExportFileName = saveFileDialog.FileName;
                m_TimeAnalyser.ExportData(
                    m_ExportFileName,
                    (val, max) =>
                    {
                        this.Invoke(new Action<int, int>(UpdateExportProgressBar), val, max);
                    }
                );
            }
        }

        private void OnExportFinished()
        {
            Process.Start(m_ExportFileName);
        }

        private void miSubset_Click(object sender, EventArgs e)
        {
            var frm = new frmChooseSubset(m_TimeAnalyser);
            if (frm.ShowDialog(this) == DialogResult.OK)
            {
                if (frm.From == 0 && frm.To == m_TimeAnalyser.Entries.Count - 1)
                {
                    m_PlotFromIndex = null;
                    m_PlotToIndex = null;
                }
                else
                {
                    m_PlotFromIndex = frm.From;
                    m_PlotToIndex = frm.To;
                }
                DrawGraph();
            }
        }

        private void DataPointConnectionStyleChanged(object sender, EventArgs e)
        {
            foreach (ToolStripMenuItem item in ((sender as ToolStripMenuItem).OwnerItem as ToolStripMenuItem).DropDownItems)
            {
                item.Checked = (item == (sender as ToolStripMenuItem));
            }

            m_GraphConfig.ConnectionLines = miLineConnections.Checked;

            DrawGraph();
        }

        private string GetTitle(List<TimeAnalyserEntry> subsetEnum, string analysisType)
        {
            string numUnits = null;
            decimal totalNum = 0;
            if (subsetEnum.Count > 1000000.0)
            {
                numUnits = "million";
                totalNum = subsetEnum.Count / 1000000.0M;
            }
            else if (subsetEnum.Count > 1000.0)
            {
                numUnits = "thousand";
                totalNum = subsetEnum.Count / 1000.0M;
            }
            else
            {
                numUnits = "";
                totalNum = subsetEnum.Count;
            }

            var timeFrom = subsetEnum.Min(x => x.SystemTimeFileTime);
            var timeTo = subsetEnum.Max(x => x.SystemTimeFileTime);

            var timeFormat = "dd-MMM HH:mm";
            if ((timeTo - timeFrom).TotalMinutes < 5)
            {
                timeFormat = "dd-MMM HH:mm:ss";
            }

            return string.Format("{0} of {1:0.0} {2} data points recorded between {3} UT and {4} UT", analysisType, totalNum, numUnits, timeFrom.ToString(timeFormat), timeTo.ToString(timeFormat));
        }
    }
}
