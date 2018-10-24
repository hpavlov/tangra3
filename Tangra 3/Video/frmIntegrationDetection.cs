using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Tangra.Controller;
using Tangra.Model.Helpers;
using Tangra.Model.Image;
using Tangra.Model.Numerical;
using Tangra.VideoTools;

namespace Tangra.Video
{
	public partial class frmIntegrationDetection : Form
	{
	    private VideoController m_VideoController;

	    private IntegrationDetectionController m_IntegrationDetectionController;

        private PotentialIntegrationFit m_FoundIntegration;

        public frmIntegrationDetection(IntegrationDetectionController integrationDetectionController, VideoController videoController)
	    {
            InitializeComponent();

	        m_IntegrationDetectionController = integrationDetectionController;
            m_VideoController = videoController;

	        m_IntegrationDetectionController.OnPotentialIntegration += DisplayFoundIntegration;
            m_IntegrationDetectionController.OnFrameData += OnFrameData;
            m_IntegrationDetectionController.OnBeginProgress += m_IntegrationDetectionController_OnBeginProgress;
            m_IntegrationDetectionController.OnProgress += m_IntegrationDetectionController_OnProgress;


            picFrameSpectrum.Image = new Bitmap(picFrameSpectrum.Width, picFrameSpectrum.Height);
            picSigmas.Image = new Bitmap(picSigmas.Width, picSigmas.Height);

            pnlResult.Visible = false;
            pnlResult.SendToBack();

            btnCorrectInterlaced.Visible = videoController != null && videoController.FramePlayer.Video is ReInterlacingVideoStream;
	    }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                if (m_IntegrationDetectionController != null)
                {
                    m_IntegrationDetectionController.OnPotentialIntegration -= DisplayFoundIntegration;
                    m_IntegrationDetectionController.OnFrameData -= OnFrameData;
                    m_IntegrationDetectionController.OnBeginProgress -= m_IntegrationDetectionController_OnBeginProgress;
                    m_IntegrationDetectionController.OnProgress -= m_IntegrationDetectionController_OnProgress;

                    m_IntegrationDetectionController.Dispose();
                }

                components.Dispose();
            }
            base.Dispose(disposing);
        }

	    private void StartMeasurements()
	    {
            pnlResult.Visible = false;
            pnlResult.SendToBack();

            timer.Interval = 50;
            timer.Enabled = true;	        
	    }
		
        private void frmIntegrationDetection_Shown(object sender, EventArgs e)
        {
            StartMeasurements();
        }

		private void timer_Tick(object sender, EventArgs e)
		{
			timer.Enabled = false;
            m_IntegrationDetectionController.RunMeasurements();
		}


		private void DisplayFoundIntegration(PotentialIntegrationFit foundIntegration)
		{
			m_FoundIntegration = foundIntegration;

		    this.Invoke(new Action(() =>
		    {
                progressBar1.Value = progressBar1.Maximum;
                progressBar1.Style = ProgressBarStyle.Continuous;
                progressBar1.Refresh();

                pnlResult.Visible = true;
                pnlResult.BringToFront();

                if (foundIntegration != null)
                {
                    lblIntegrationFrames.Text = string.Format("{0} frames", foundIntegration.Interval);
                    lblStartingAt.Text = foundIntegration.StartingAtFrame.ToString();
                    lblCertainty.Text = foundIntegration.CertaintyText;

                    btnAccept.Visible = true;
                    btnReject.Text = "Reject";
                }
                else
                {
                    lblIntegrationFrames.Text = "Detection Failed";
                    lblStartingAt.Text = "N/A";
                    lblCertainty.Text = "N/A";

                    btnAccept.Visible = false;
                    btnReject.Text = "Close";
                }
		    }));
		}

        void m_IntegrationDetectionController_OnProgress(int val)
        {
            this.Invoke(new Action(() =>
            {
                progressBar1.Value = Math.Max(progressBar1.Minimum, Math.Min(progressBar1.Maximum, val));
                progressBar1.Refresh();
            }));
        }

        void m_IntegrationDetectionController_OnBeginProgress(int min, int max)
        {
            this.Invoke(new Action(() =>
            {

                progressBar1.Minimum = min;
                progressBar1.Maximum = max - 1;
                progressBar1.Style = ProgressBarStyle.Marquee;
                progressBar1.Refresh();
            }));
        }

		internal PotentialIntegrationFit IntegratedFrames
		{
			get { return m_FoundIntegration; }
		}

		private double xScale;
		private double minY;
		private int minX;

        void OnFrameData(List<AverageCalculator> calcs, Dictionary<int, double> sigmas, double median, double variance)
        {
            this.Invoke(new Action(() =>
            {
                Plot(calcs);
                PlotSigmas(sigmas, median, variance);
                Refresh();
            }));
        }


		private void Plot(List<AverageCalculator> calcs)
		{
			double minY = calcs.Min(a => a.Average - a.Sigma);
			double maxY = calcs.Max(a => a.Average + a.Sigma);

			double scaleY = (picFrameSpectrum.Height - 10) / (maxY - minY);
			double xScale = 1.0 * (picFrameSpectrum.Width - 10) / (0.5 * calcs.Count); 
			
			using (Graphics g = Graphics.FromImage(picFrameSpectrum.Image))
			{
				g.Clear(Color.WhiteSmoke);

				for (int i = 0; i < calcs.Count - 2; i+=2)
				{
					float x = (float)(i * xScale) + 5;
					float y = 5 + (float)(scaleY * (calcs[i].Average - minY));
					float x2 = (float)((i + 2)* xScale) + 5;
					float y2 = 5 + (float)(scaleY * (calcs[i + 2].Average - minY));

					float yFrom = 5 + (float)(scaleY * (calcs[i].Average - calcs[i].Sigma - minY));
					float yTo = 5 + (float)(scaleY * (calcs[i].Average + calcs[i].Sigma - minY));

					g.DrawLine(Pens.Red, x, yFrom, x, yTo);
					g.DrawLine(Pens.Black, x, y, x2, y2);
				}

				g.Save();
			}

			picFrameSpectrum.Refresh();
		}

		private void PlotSigmas(Dictionary<int, double> sigmas, double median, double variance)
		{
			minX = sigmas.Keys.Min();
			int maxX = sigmas.Keys.Max();

            if (maxX == minX) maxX = minX + 1;
            if (maxX == 0) maxX = 1;

			List<double> vals = sigmas.Values.Where(v => !double.IsNaN(v)).ToList();

			minY = vals.Min();
			double maxY = vals.Max();

            if (maxY == minY) maxY = 1.1 * minY;
            if (maxY == 0) maxY = 1;
			
			xScale = 1.0 * (picSigmas.Width - 10) / (maxX - minX);
			double yScale = (picSigmas.Height - 10) / (maxY - minY);

			using(Graphics g = Graphics.FromImage(picSigmas.Image))
			{
				g.Clear(Color.WhiteSmoke);

			    int prevGreenId = -1;
			    float prevX = 0;
			    int counter = -1;
				foreach(int key in sigmas.Keys)
				{
				    counter++;
					if (double.IsNaN(sigmas[key])) break;

					float x = 5 + (float)((key - minX) * xScale);
					float x2 = 4 + (float)((key + 1 - minX) * xScale);
					float y = (5 + (float)((sigmas[key] - minY) * yScale));

                    double residual = Math.Abs(median - sigmas[key]);
                    Brush brush = residual < variance ? Brushes.Red : Brushes.Green;
                    if (residual > variance)
				    {
				        if (prevGreenId != -1)
				        {
				            int interval = counter - prevGreenId;
				            if (interval > 4)
				            {
				                var textSize = g.MeasureString(interval.ToString(), DefaultFont);
                                g.DrawString(interval.ToString(), DefaultFont, Brushes.Black, (x2 + prevX - textSize.Width) / 2f, (float)(minY + 2));    
				            }
                            
				        }
				        prevGreenId = counter;
				        prevX = x;
				    }

					g.FillRectangle(brush, x, picSigmas.Height - y - 4, x2 - x, y + 4);
				}

				g.Save();
			}

			picSigmas.Refresh();
		}

		private void picSigmas_MouseDown(object sender, MouseEventArgs e)
		{
            if (e.Button == MouseButtons.Middle)
            {
                double key = (e.X - 5) / xScale + minX;
                List<AverageCalculator> cals = m_IntegrationDetectionController.GetCalculatorsForFrame((int)key);
                if (cals != null)
                {
                    Plot(cals);

                    pnlResult.Visible = false;
                    pnlResult.SendToBack();                    
                }
            }
		}

        private void btnAccept_Click(object sender, EventArgs e)
        {
            if (m_FoundIntegration == null)
            {
                DialogResult = DialogResult.Cancel;
                Close();
            }
            else
            {
                DialogResult = DialogResult.OK;
                Close();                
            }
        }

        private void btnCorrectInterlaced_Click(object sender, EventArgs e)
        {
            if (m_VideoController != null)
            {
                var frm = new frmCorrectInterlacedDefects(m_VideoController);
                if (frm.ShowDialog(this) == DialogResult.OK)
                {
                    StartMeasurements();
                }                
            }
        }
	}

    public interface IImagePixelProvider
    {
        int Width { get; }
        int Height { get; }
        int LastFrame { get; }
        int[,] GetPixelArray(int frameNo, Rectangle rect);
    }
}
