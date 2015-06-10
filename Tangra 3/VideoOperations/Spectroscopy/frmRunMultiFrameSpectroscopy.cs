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
using Tangra.Model.Image;
using Tangra.Model.Video;
using Tangra.VideoOperations.Spectroscopy.Helpers;

namespace Tangra.VideoOperations.Spectroscopy
{
	public partial class frmRunMultiFrameSpectroscopy : Form
	{
		public int NumberOfMeasurements { get; private set; }
        public int MeasurementAreaWing { get; private set; }
		public int BackgroundAreaWing { get; private set; }
	    public PixelCombineMethod BackgroundMethod { get; private set; }
		public PixelCombineMethod FrameCombineMethod { get; private set; }

		private bool m_Initialised = false;

		private VideoSpectroscopyOperation m_VideoOperation;
		private AstroImage m_AstroImage;
		private Bitmap m_ZoomedRawImage = null;

		private static Brush[] s_GreyBrushes = new Brush[256];
		private static Color[] s_GreyColors = new Color[256];

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

			picAreas.Image = new Bitmap(picAreas.Width, picAreas.Height, PixelFormat.Format24bppRgb);
		}

        public frmRunMultiFrameSpectroscopy(IFramePlayer framePlayer, VideoSpectroscopyOperation videoOperation, AstroImage astroImage)
			: this()
        {
	        m_VideoOperation = videoOperation;
	        m_AstroImage = astroImage;

			nudNumberMeasurements.Maximum = framePlayer.Video.LastFrame - framePlayer.CurrentFrameIndex;
			nudNumberMeasurements.Value = Math.Min(200, nudNumberMeasurements.Maximum);
            nudAreaWing.Value = videoOperation.SpectraReaderHalfWidth;
			nudBackgroundWing.Value = videoOperation.SpectraReaderBackgroundHalfWidth;
		    cbxBackgroundMethod.SelectedIndex = 0;
	        cbxCombineMethod.SelectedIndex = 0;

	        m_Initialised = true;
        }

		private void btnNext_Click(object sender, EventArgs e)
		{
			NumberOfMeasurements = (int)nudNumberMeasurements.Value;
            MeasurementAreaWing = (int)nudAreaWing.Value;
			BackgroundAreaWing = (int)nudBackgroundWing.Value;
            BackgroundMethod = (PixelCombineMethod)cbxBackgroundMethod.SelectedIndex;
			FrameCombineMethod = (PixelCombineMethod)cbxCombineMethod.SelectedIndex;

			DialogResult = DialogResult.OK;
			Close();
		}

		private void frmRunMultiFrameSpectroscopy_Load(object sender, EventArgs e)
		{
			PlotMeasurementAreas();
		}

		private void nudAreaWing_ValueChanged(object sender, EventArgs e)
		{
			if (m_Initialised)
			{
				m_VideoOperation.UpdateMeasurementAreasDisplay((int)nudAreaWing.Value, (int)nudBackgroundWing.Value);
				PlotMeasurementAreas();
			}
		}

		private void nudBackgroundWing_ValueChanged(object sender, EventArgs e)
		{
			if (m_Initialised)
			{
				m_VideoOperation.UpdateMeasurementAreasDisplay((int)nudAreaWing.Value, (int)nudBackgroundWing.Value);
				PlotMeasurementAreas();
			}
		}

		private int m_StartDestXValue;
		private int m_DestHorizontalPixelCount;
		private int m_DestVerticalPixelCount;

		private static int m_ZoomRatio = 2;

		private void PrepareRawImage()
		{
			m_ZoomedRawImage = new Bitmap(picAreas.Width, picAreas.Height, PixelFormat.Format24bppRgb);

			IImagePixel starCenter = m_VideoOperation.SelectedStar;
			var reader = new SpectraReader(m_AstroImage, m_VideoOperation.SelectedStarBestAngle);
			Spectra spectra = reader.ReadSpectra((float)starCenter.XDouble, (float)starCenter.YDouble, (int)nudAreaWing.Value, (int)nudBackgroundWing.Value, PixelCombineMethod.Average);

			var mapper = new RotationMapper(m_AstroImage.Width, m_AstroImage.Height, m_VideoOperation.SelectedStarBestAngle);
			
			// Find the peak to the right of the zero order image
			PointF destCenter = mapper.GetDestCoords((float)starCenter.XDouble, (float)starCenter.YDouble);
			int x0 = spectra.Points[0].PixelNo;
			float maxValue = float.MinValue;
			int maxValuePixelNo = -1;
			for (int i = (int)destCenter.X + 2 * (int)nudAreaWing.Value; i < mapper.MaxDestDiagonal; i++)
			{
				int idx = i - x0;
				if (idx >= 0 && idx < spectra.Points.Count)
				{
					if (spectra.Points[idx].RawValue > maxValue)
					{
						maxValue = spectra.Points[idx].RawValue;
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
					PointF pt = mapper.GetSourceCoords(x, destCenter.Y + y);
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

					float y3 = (float)((int)nudAreaWing.Value + (int)nudBackgroundWing.Value + m_DestVerticalPixelCount) * m_ZoomRatio - m_ZoomRatio / 2.0f;
					float y4 = (float)((int)-nudAreaWing.Value - (int)nudBackgroundWing.Value + m_DestVerticalPixelCount) * m_ZoomRatio + m_ZoomRatio / 2.0f;

					g.DrawLine(s_SpectraBackgroundPen, 0, y3, m_ZoomedRawImage.Width, y3);
					g.DrawLine(s_SpectraBackgroundPen, 0, y4, m_ZoomedRawImage.Width, y4);

					g.Save();
				}
			}

			picAreas.Invalidate();
		}
	}
}
