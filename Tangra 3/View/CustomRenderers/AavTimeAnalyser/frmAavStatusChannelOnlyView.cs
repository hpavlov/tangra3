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
        private VideoController m_VideoController;
        private AstroDigitalVideoStream m_Aav;
        private AavTimeAnalyser m_TimeAnalyser;

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
            Cursor = Cursors.WaitCursor;
            try
            {
                var image = new Bitmap(pbGraph.Width, pbGraph.Height, PixelFormat.Format24bppRgb);

                using (var g = Graphics.FromImage(image))
                {
                    float padding = 10;
                    float paddingX = 40;
                    float paddingY = 25;
                    float width = pbGraph.Width - padding - paddingX;
                    float height = pbGraph.Height - 2 * paddingY;
                    float yFactor = height / (m_TimeAnalyser.MaxDeltaMs - m_TimeAnalyser.MinDeltaMs);
                    float xFactor = width / (m_TimeAnalyser.Entries.Count);

                    g.FillRectangle(Brushes.WhiteSmoke, 0, 0, pbGraph.Width, pbGraph.Height);

                    for (int ya = -2000; ya < 2000; ya += 100)
                    {
                        float y = pbGraph.Height - paddingY - yFactor * (ya - m_TimeAnalyser.MinDeltaMs);
                        if (y < paddingY || y > pbGraph.Height - paddingY) continue;

                        g.DrawLine(Pens.Gray, paddingX, y, pbGraph.Width - padding, y);
                        if (ya == 0)
                        {
                            g.DrawLine(Pens.Gray, paddingX, y - 1, pbGraph.Width - padding, y - 1);
                            g.DrawLine(Pens.Gray, paddingX, y + 1, pbGraph.Width - padding, y + 1);
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

                    foreach (var entry in m_TimeAnalyser.Entries)
                    {
                        if (entry.IsOutlier) continue;

                        float x = paddingX + idx * xFactor;
                        float y1 = pbGraph.Height - paddingY - yFactor * (entry.DeltaNTPTimeMs - m_TimeAnalyser.MinDeltaMs);
                        float y2 = pbGraph.Height - paddingY - yFactor * (entry.DeltaSystemFileTimeMs - m_TimeAnalyser.MinDeltaMs);
                        float y3t = pbGraph.Height - paddingY - yFactor * (entry.NTPErrorMs - m_TimeAnalyser.MinDeltaMs);
                        float y3b = pbGraph.Height - paddingY - yFactor * (-entry.NTPErrorMs - m_TimeAnalyser.MinDeltaMs);

                        if (idx > 0)
                        {
                            g.DrawLine(Pens.LightGray, xp, y3pt, x, y3t);
                            g.DrawLine(Pens.LightGray, xp, y3pb, x, y3b);
                            g.DrawLine(Pens.LimeGreen, xp, y1p, x, y1);
                            g.DrawLine(Pens.Red, xp, y2p, x, y2);
                        }

                        y1p = y1;
                        y2p = y2;
                        y3pt = y3t;
                        y3pb = y3b;
                        xp = x;

                        idx++;
                    }

                    g.DrawRectangle(Pens.Black, paddingX, paddingY, pbGraph.Width - padding - paddingX, pbGraph.Height - 2 * paddingY);

                    var title = string.Format("Time anaysis of {0:0.0} million data points recorded between {1} UT and {2} UT", m_TimeAnalyser.Entries.Count / 1000000.0, m_TimeAnalyser.FromDateTime.ToString("dd-MMM HH:mm"), m_TimeAnalyser.ToDateTime.ToString("dd-MMM HH:mm"));
                    var sizeF = g.MeasureString(title, m_TitleFont);
                    g.DrawString(title, m_TitleFont, Brushes.Black, (width - sizeF.Width) / 2 + paddingX, (paddingY - sizeF.Height) / 2);

                    var thirdW = width / 3;
                    for (int i = 0; i < 3; i++)
                    {
                        string legend = "";
                        Pen legendPen = Pens.Black;
                        if (i == 0)
                        {
                            legend = "GetSystemTimePreciseAsFileTime()";
                            legendPen = Pens.Red;
                        }
                        else if (i == 1)
                        {
                            legend = "OccuRec's NTP-based Time Keeping";
                            legendPen = Pens.LimeGreen;
                        }
                        else if (i == 2)
                        {
                            legend = "Max NTP Error";
                            legendPen = Pens.LightGray;
                        }

                        sizeF = g.MeasureString(legend, m_TitleFont);
                        var y = paddingY + height + sizeF.Height/2;
                        var yl = paddingY + height + sizeF.Height;
                        g.DrawString(legend, m_TitleFont, Brushes.Black, (thirdW - sizeF.Width) / 2 + paddingX + i * thirdW + 15, y);
                        g.DrawLine(legendPen, (thirdW - sizeF.Width) / 2 + paddingX + i * thirdW, yl - 1, 6 + (thirdW - sizeF.Width) / 2 + paddingX + i * thirdW, yl - 1);
                        g.DrawLine(legendPen, (thirdW - sizeF.Width) / 2 + paddingX + i * thirdW, yl, 6 + (thirdW - sizeF.Width) / 2 + paddingX + i * thirdW, yl);
                        g.DrawLine(legendPen, (thirdW - sizeF.Width) / 2 + paddingX + i * thirdW, yl + 1, 6 + (thirdW - sizeF.Width) / 2 + paddingX + i * thirdW, yl + 1);
                    }

                    g.Save();
                }

                if (pbGraph.Image != null)
                {
                    pbGraph.Image.Dispose();
                }

                pbGraph.Image = image;
                pbGraph.Update();
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void resizeUpdateTimer_Tick(object sender, EventArgs e)
        {
            resizeUpdateTimer.Enabled = false;

            DrawGraph();
        }

        private void frmAavStatusChannelOnlyView_ResizeEnd(object sender, EventArgs e)
        {
            resizeUpdateTimer.Enabled = true;
        }
    }
}
