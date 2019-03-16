using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
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
        internal enum GraphType
        {
            gtNone,
            gtTimeDeltasLines,
            gtTimeDeltasDots
        }

        internal enum GridlineStyle
        {
            Line,
            Tick
        }

        internal class GraphConfig
        {
            public GridlineStyle GridlineStyle = GridlineStyle.Line;
        }

        private GraphType m_GrapthType = GraphType.gtNone;

        private VideoController m_VideoController;
        private AstroDigitalVideoStream m_Aav;
        private AavTimeAnalyser m_TimeAnalyser;

        private int m_LastGraphWidth;
        private int m_LastGraphHeight;

        const float MIN_PIX_DIFF = 1f;

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

        private void UpdateProgressBar(int val, int max)
        {
            if (val == 0 && max == 0)
            {
                pbLoadData.Visible = false;
                DrawGraph();
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

        private void DrawGraph()
        {
            if (!m_TimeAnalyser.IsDataReady) return;

            switch (m_GrapthType)
            {
                case GraphType.gtTimeDeltasLines:
                case GraphType.gtTimeDeltasDots:
                    DrawTimeDeltasGraph();
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

            bool ellipses = m_GrapthType == GraphType.gtTimeDeltasDots;
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
                if (plotSystemTime)
                {
                    minDelta = m_TimeAnalyser.MinDeltaSystemTimeMs;
                    maxDelta = m_TimeAnalyser.MaxDeltaSystemTimeMs;
                }
                else
                {
                    minDelta = m_TimeAnalyser.MinDeltaSystemFileTimeMs;
                    maxDelta = m_TimeAnalyser.MaxDeltaSystemFileTimeMs;
                }

                if (plotOccuRecTime)
                {
                    minDelta = Math.Min(minDelta, m_TimeAnalyser.MinDeltaNTPMs);
                    maxDelta = Math.Max(maxDelta, m_TimeAnalyser.MaxDeltaNTPMs);
                }

                if (plotNtpError)
                {
                    minDelta = Math.Min(minDelta, m_TimeAnalyser.MinDeltaNTPErrorMs);
                    maxDelta = Math.Max(maxDelta, m_TimeAnalyser.MaxDeltaNTPErrorMs);
                }

                using (var g = Graphics.FromImage(image))
                {
                    float padding = 10;
                    float paddingX = 40;
                    float paddingY = 25;
                    float width = graphWidth - padding - paddingX;
                    float height = graphHeight - 2 * paddingY;
                    float yFactor = height / (maxDelta - minDelta);
                    float xFactor = width / (m_TimeAnalyser.Entries.Count);

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
                    float xp = 0;

                    float y1 = 0;
                    float y2 = 0;
                    float y3t = 0;
                    float y3b = 0;

                    foreach (var entry in m_TimeAnalyser.Entries)
                    {
                        if (entry.IsOutlier) continue;

                        float x = paddingX + idx * xFactor;
                        bool plotted = false;
                        bool calcOnly = idx == 0;

                        if (plotNtpError)
                        {
                            y3t = graphHeight - paddingY - yFactor * (entry.NTPErrorMs - minDelta);
                            y3b = graphHeight - paddingY - yFactor * (-entry.NTPErrorMs - minDelta);
                            if (!calcOnly && (x - xp > MIN_PIX_DIFF || Math.Abs(y3t - y3pt) > MIN_PIX_DIFF || Math.Abs(y3b - y3pb) > MIN_PIX_DIFF))
                            {
                                if (ellipses)
                                {
                                    g.FillEllipse(NtpErrorBrush, x - 1, y3t - 1, 2, 2);
                                    g.FillEllipse(NtpErrorBrush, x - 1, y3b - 1, 2, 2);
                                }
                                else
                                {
                                    g.DrawLine(NtpErrorPen, xp, y3pt, x, y3t);
                                    g.DrawLine(NtpErrorPen, xp, y3pb, x, y3b); 
                                }
                                plotted = true;
                            }
                        }

                        if (plotOccuRecTime)
                        {
                            y1 = graphHeight - paddingY - yFactor * (entry.DeltaNTPTimeMs - minDelta);
                            if (!calcOnly && (x - xp > MIN_PIX_DIFF || Math.Abs(y1 - y1p) > MIN_PIX_DIFF))
                            {
                                if (ellipses)
                                {
                                    g.FillEllipse(NtpTimeBrush, x - 1, y1 - 1, 2, 2);
                                }
                                else
                                {
                                    g.DrawLine(NtpTimePen, xp, y1p, x, y1);
                                }
                                plotted = true;
                            }
                        }

                        if (plotSystemTime)
                            y2 = graphHeight - paddingY - yFactor * (entry.DeltaSystemTimeMs - minDelta);
                        else
                            y2 = graphHeight - paddingY - yFactor * (entry.DeltaSystemFileTimeMs - minDelta);

                        if (!calcOnly && (x - xp > MIN_PIX_DIFF || Math.Abs(y2 - y2p) > MIN_PIX_DIFF))
                        {
                            if (ellipses)
                            {
                                g.FillEllipse(SysTimeBrush, x - 1, y2 - 1, 2, 2);
                            }
                            else
                            {
                                g.DrawLine(SysTimePen, xp, y2p, x, y2);
                            }
                            plotted = true;
                        }

                        if (plotted || idx == 0)
                        {
                            y1p = y1;
                            y2p = y2;
                            y3pt = y3t;
                            y3pb = y3b;
                            xp = x;
                        }

                        idx++;
                    }

                    g.DrawRectangle(Pens.Black, paddingX, paddingY, graphWidth - padding - paddingX, graphHeight - 2 * paddingY);

                    var title = string.Format("Time Delta analysis of {0:0.0} million data points recorded between {1} UT and {2} UT", m_TimeAnalyser.Entries.Count / 1000000.0, m_TimeAnalyser.FromDateTime.ToString("dd-MMM HH:mm"), m_TimeAnalyser.ToDateTime.ToString("dd-MMM HH:mm"));
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
                                legend = "Max 1-Sigma NTP Error";
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
                m_GrapthType = GraphType.gtTimeDeltasLines;
            }
            else if (cbxGraphType.SelectedIndex == 1)
            {
                m_GrapthType = GraphType.gtTimeDeltasDots;
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
    }
}
