using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Model.Astro;
using Tangra.Model.Config;
using Tangra.Model.Image;
using Tangra.Model.Video;
using Tangra.PInvoke;
using Tangra.VideoOperations.Spectroscopy.Helpers;

namespace Tangra.VideoOperations.Spectroscopy
{
    public partial class frmRunMultiFrameSpectroscopy : Form
    {
        public int NumberOfMeasurements { get; private set; }
        public int MeasurementAreaWing { get; private set; }
        public int BackgroundAreaWing { get; private set; }
        public int BackgroundAreaGap { get; private set; }
        public float PixelValueCoefficient { get; private set; }
        public PixelCombineMethod BackgroundMethod { get; private set; }
        public PixelCombineMethod FrameCombineMethod { get; private set; }
        public bool UseFineAdjustments { get; private set; }
        public bool UseLowPassFilter { get; private set; }
        public int? AlignmentAbsorptionLinePos { get; private set; }


        private bool m_Initialised = false;
        private int m_CurrentPageNo = 0;

        private VideoSpectroscopyOperation m_VideoOperation;
        private AstroImage m_AstroImage;
        private Bitmap m_ZoomedRawImage = null;

        private static Brush[] s_GreyBrushes = new Brush[256];
        private static Color[] s_GreyColors = new Color[256];

        private SpectraReader m_SpectraReader;
        private Spectra m_Spectra;
        private RotationMapper m_Mapper;
        private PSFFit m_ZeroOrderPsf;
        private float? m_SelectedAlignLine;

        static frmRunMultiFrameSpectroscopy()
        {
            for (int i = 0; i < 256; i++)
            {
                s_GreyBrushes[i] = new SolidBrush(Color.FromArgb(0, i, i, i));
                s_GreyColors[i] = Color.FromArgb(0, i, i, i);
            }
        }

        public frmRunMultiFrameSpectroscopy()
        {
            InitializeComponent();

            Width = 546;
            m_CurrentPageNo = 0;
            PixelValueCoefficient = 1;
            m_SelectedAlignLine = null;
            AlignmentAbsorptionLinePos = null;

            picAreas.Image = new Bitmap(picAreas.Width, picAreas.Height, PixelFormat.Format24bppRgb);
            picAlignTarget.Image = new Bitmap(picAlignTarget.Width, picAlignTarget.Height, PixelFormat.Format24bppRgb);
        }

        public frmRunMultiFrameSpectroscopy(IFramePlayer framePlayer, VideoSpectroscopyOperation videoOperation, AstroImage astroImage)
            : this()
        {
            m_VideoOperation = videoOperation;
            m_AstroImage = astroImage;

            nudNumberMeasurements.Maximum = framePlayer.Video.LastFrame - framePlayer.CurrentFrameIndex;
            nudNumberMeasurements.Value = Math.Min(200, nudNumberMeasurements.Maximum);
            nudAreaWing.Value = Math.Min(videoOperation.MeasurementAreaWing, nudAreaWing.Maximum);
            nudBackgroundWing.Value = Math.Min(videoOperation.BackgroundAreaWing, nudBackgroundWing.Maximum);
            cbxBackgroundMethod.SelectedIndex = 1; /* Median */
            cbxCombineMethod.SelectedIndex = 1; /* Median */

            m_Initialised = true;
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (m_CurrentPageNo == 0)
                SwitchToMeasurementPage();
            else if (m_CurrentPageNo == 1)
                SwitchToNormalisationPage();
            else if (m_CurrentPageNo == 2)
                RunMeasurements();
        }


        private void btnPrevious_Click(object sender, EventArgs e)
        {
            if (m_CurrentPageNo == 2)
                SwitchToMeasurementPage();
            else if (m_CurrentPageNo == 1)
                SwitchToAlignmentPage();
            else
            {
                DialogResult = DialogResult.Cancel;
                Close();
            }
        }

        private void SwitchToMeasurementPage()
        {
            gbMeasurement.Top = 12;
            gbMeasurement.Left = 12;
            gbxAlignment.Visible = false;
            gbNormalisation.Visible = false;
            gbMeasurement.Visible = true;
            gbMeasurement.BringToFront();

            m_CurrentPageNo = 1;
            btnNext.Text = "Next";
            btnPrevious.Text = "Previous";
        }

        private void SwitchToAlignmentPage()
        {
            gbxAlignment.Top = 12;
            gbxAlignment.Left = 12;
            gbMeasurement.Visible = false;
            gbNormalisation.Visible = false;
            gbxAlignment.Visible = true;
            gbxAlignment.BringToFront();

            m_CurrentPageNo = 0;
            btnNext.Text = "Next";
            btnPrevious.Text = "Cancel";
        }

        private void SwitchToNormalisationPage()
        {
            gbNormalisation.Top = 12;
            gbNormalisation.Left = 12;
            gbxAlignment.Visible = false;
            gbMeasurement.Visible = false;
            gbNormalisation.Visible = true;
            gbNormalisation.BringToFront();

            m_CurrentPageNo = 2;
            btnNext.Text = "Start";
            btnPrevious.Text = "Previous";
        }


        private void RunMeasurements()
        {
            NumberOfMeasurements = (int)nudNumberMeasurements.Value;
            MeasurementAreaWing = (int)nudAreaWing.Value;
            BackgroundAreaWing = (int)nudBackgroundWing.Value;
            BackgroundAreaGap = (int)nudBackgroundGap.Value;

            if (cbxNormalisation.Checked)
                PixelValueCoefficient = (float)nudMultiplier.Value / (float)nudDivisor.Value;
            else
                PixelValueCoefficient = 1;

            BackgroundMethod = (PixelCombineMethod)cbxBackgroundMethod.SelectedIndex;
            FrameCombineMethod = (PixelCombineMethod)cbxCombineMethod.SelectedIndex;
            UseFineAdjustments = cbxFineAdjustments.Checked;
            UseLowPassFilter = cbxUseLowPassFilter.Checked;
            if (m_SelectedAlignLine.HasValue) AlignmentAbsorptionLinePos = (int)Math.Round(m_SelectedAlignLine.Value);

            DialogResult = DialogResult.OK;
            Close();
        }

		private void frmRunMultiFrameSpectroscopy_Load(object sender, EventArgs e)
		{
            IImagePixel starCenter = m_VideoOperation.SelectedStar;
            m_SpectraReader = new SpectraReader(m_AstroImage, m_VideoOperation.SelectedStarBestAngle, 1);
            m_Spectra = m_SpectraReader.ReadSpectra((float)starCenter.XDouble, (float)starCenter.YDouble, (int)nudAreaWing.Value, (int)nudBackgroundWing.Value, (int)nudBackgroundGap.Value, PixelCombineMethod.Average);

            m_Mapper = new RotationMapper(m_AstroImage.Width, m_AstroImage.Height, m_VideoOperation.SelectedStarBestAngle);

		    uint[,] pixels = m_AstroImage.GetMeasurableAreaPixels(starCenter.X, starCenter.Y, 35);
            m_ZeroOrderPsf = new PSFFit(starCenter.X, starCenter.Y);
            m_ZeroOrderPsf.Fit(pixels);

			PlotMeasurementAreas();
		    PlotAlignTarget();

		    nudDivisor.Value = m_AstroImage.Pixelmap.MaxSignalValue;
		    nudMultiplier.Value = 1024;
		}

        private void UpdateMeasurementAreasDisplay()
        {
            if (m_Initialised)
            {
                m_VideoOperation.UpdateMeasurementAreasDisplay((int)nudAreaWing.Value, (int)nudBackgroundWing.Value, (int)nudBackgroundGap.Value);
                PlotMeasurementAreas();
            }
        }

		private void nudAreaWing_ValueChanged(object sender, EventArgs e)
		{
		    UpdateMeasurementAreasDisplay();
		}

		private void nudBackgroundWing_ValueChanged(object sender, EventArgs e)
		{
            UpdateMeasurementAreasDisplay();
		}

		private int m_StartDestXValue;
		private int m_DestHorizontalPixelCount;
		private int m_DestVerticalPixelCount;

		private static int m_ZoomRatio = 1;

		private void PrepareRawImage()
		{
			m_ZoomedRawImage = new Bitmap(picAreas.Width, picAreas.Height, PixelFormat.Format24bppRgb);
			
			// Find the peak to the right of the zero order image
            PointF destCenter = m_Mapper.GetDestCoords((float)m_VideoOperation.SelectedStar.XDouble, (float)m_VideoOperation.SelectedStar.YDouble);
            int x0 = m_Spectra.Points[0].PixelNo;
			float maxValue = float.MinValue;
			int maxValuePixelNo = -1;
            for (int i = (int)destCenter.X + 2 * (int)nudAreaWing.Value; i < m_Mapper.MaxDestDiagonal; i++)
			{
				int idx = i - x0;
                if (idx >= 0 && idx < m_Spectra.Points.Count)
				{
                    if (m_Spectra.Points[idx].RawValue > maxValue)
					{
                        maxValue = m_Spectra.Points[idx].RawValue;
						maxValuePixelNo = i;
					}
				}
			}

			m_DestHorizontalPixelCount = 1 + m_ZoomedRawImage.Width / (2 * m_ZoomRatio);
			m_DestVerticalPixelCount = 1 + m_ZoomedRawImage.Height / (2 * m_ZoomRatio);
			m_StartDestXValue = (int)(maxValuePixelNo - (m_ZoomedRawImage.Width / (2 * m_ZoomRatio)));

			RectangleF imageRect = new RectangleF(0, 0, m_AstroImage.Width, m_AstroImage.Height);
			RectangleF zoomedImageRect = new RectangleF(0, 0, m_ZoomedRawImage.Width, m_ZoomedRawImage.Height);

			for (int x = m_StartDestXValue; x <= m_StartDestXValue + 2 * m_DestHorizontalPixelCount; x++)
			{
				for (int y = -m_DestVerticalPixelCount; y <= m_DestVerticalPixelCount; y++)
				{
                    PointF pt = m_Mapper.GetSourceCoords(x, destCenter.Y + y);
					if (imageRect.Contains(pt))
					{
						int x1 = (int)pt.X;
						int y1 = (int)pt.Y;

						uint pixelVal = m_AstroImage.Pixelmap[x1, y1];

						byte clr = (byte)Math.Min(255, Math.Max(0, (int)Math.Round(255.0 * pixelVal / m_AstroImage.Pixelmap.MaxSignalValue)));

						int xx = (x - m_StartDestXValue) * m_ZoomRatio;
						int yy = (y + m_DestVerticalPixelCount) * m_ZoomRatio;

						for (int ix = 0; ix < m_ZoomRatio; ix++)
						for (int iy = 0; iy < m_ZoomRatio; iy++)
						{
							if (zoomedImageRect.Contains(xx + ix, yy + iy))
								m_ZoomedRawImage.SetPixel(xx + ix, yy + iy, s_GreyColors[clr]);
						}
					}
				}
			}
		}

		private static Pen s_SpectraBackgroundPen = new Pen(Color.FromArgb(70, 255, 0, 0));
        private static Pen s_SelectedAlignmentLinePen = new Pen(Color.FromArgb(70, 0, 255, 255));

		private void PlotMeasurementAreas()
		{
			if (m_ZoomedRawImage == null)
				PrepareRawImage();

			if (m_ZoomedRawImage != null)
			{
				using (Graphics g = Graphics.FromImage(picAreas.Image))
				{
					g.DrawImage(m_ZoomedRawImage, 0, 0);

					float y1 = (float)((int)nudAreaWing.Value + m_DestVerticalPixelCount) * m_ZoomRatio - m_ZoomRatio / 2.0f;
					float y2 = (float)((int)-nudAreaWing.Value + m_DestVerticalPixelCount) * m_ZoomRatio + m_ZoomRatio / 2.0f;

					g.DrawLine(Pens.Red, 0, y1, m_ZoomedRawImage.Width, y1);
					g.DrawLine(Pens.Red, 0, y2, m_ZoomedRawImage.Width, y2);

                    float y3 = (float)((int)nudAreaWing.Value + (int)nudBackgroundGap.Value + m_DestVerticalPixelCount) * m_ZoomRatio - m_ZoomRatio / 2.0f;
                    float y4 = (float)((int)-nudAreaWing.Value - (int)nudBackgroundGap.Value + m_DestVerticalPixelCount) * m_ZoomRatio + m_ZoomRatio / 2.0f;

					g.DrawLine(s_SpectraBackgroundPen, 0, y3, m_ZoomedRawImage.Width, y3);
					g.DrawLine(s_SpectraBackgroundPen, 0, y4, m_ZoomedRawImage.Width, y4);

                    float y5 = (float)((int)nudAreaWing.Value + (int)nudBackgroundWing.Value + (int)nudBackgroundGap.Value + m_DestVerticalPixelCount) * m_ZoomRatio - m_ZoomRatio / 2.0f;
                    float y6 = (float)((int)-nudAreaWing.Value - (int)nudBackgroundWing.Value - (int)nudBackgroundGap.Value + m_DestVerticalPixelCount) * m_ZoomRatio + m_ZoomRatio / 2.0f;

                    g.DrawLine(s_SpectraBackgroundPen, 0, y5, m_ZoomedRawImage.Width, y5);
                    g.DrawLine(s_SpectraBackgroundPen, 0, y6, m_ZoomedRawImage.Width, y6);


                    if (m_SelectedAlignLine.HasValue)
                    {
                        float xx = (m_SelectedAlignLine.Value - m_StartDestXValue) * m_ZoomRatio;
                        g.DrawLine(s_SelectedAlignmentLinePen, xx, 0, xx, picAreas.Image.Height);
                    }

					g.Save();
				}
			}

			picAreas.Invalidate();
		}
	
        private void PlotAlignTarget()
        {
            if (rbAlignZeroOrder.Checked)
                PlotZeroOrderPSF();

        }

        private void PlotZeroOrderPSF()
        {
            using (Graphics g = Graphics.FromImage(picAlignTarget.Image))
            {
                m_ZeroOrderPsf.DrawGraph(g, new Rectangle(0, 0, picAlignTarget.Width, picAlignTarget.Height), m_AstroImage.Pixelmap.BitPixCamera, m_AstroImage.Pixelmap.MaxSignalValue);
                g.Save();
            }

            picAlignTarget.Invalidate();
        }

        private void picAreas_MouseDown(object sender, MouseEventArgs e)
        {
            if (m_CurrentPageNo == 0 && !rbAlignZeroOrder.Checked)
            {
                int candiateLine = (int)Math.Round(m_StartDestXValue + e.X * 1.0 / m_ZoomRatio);
                List<SpectraPoint> pointsInRegion = m_Spectra.Points.Where(x => Math.Abs(x.PixelNo - candiateLine) < 10).ToList();

                float[] arrPixelNo = pointsInRegion.Select(x => (float)x.PixelNo).ToArray();
                float[] arrPixelValues = pointsInRegion.Select(x => x.RawValue).ToArray();
                Array.Sort(arrPixelValues, arrPixelNo);

                if (rbAlignAbsorptionLine.Checked)
                    m_SelectedAlignLine = arrPixelNo[0];
                else if (rbAlignEmissionLine.Checked)
                    m_SelectedAlignLine = arrPixelNo[arrPixelNo.Length - 1];

                PlotMeasurementAreas();
            }
        }

        private void nudBackgroundGap_ValueChanged(object sender, EventArgs e)
        {
            UpdateMeasurementAreasDisplay();
        }
    }
}
