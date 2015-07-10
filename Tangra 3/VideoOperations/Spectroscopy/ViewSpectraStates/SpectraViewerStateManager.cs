using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Controller;
using Tangra.Model.Config;
using Tangra.VideoOperations.Spectroscopy.Helpers;

namespace Tangra.VideoOperations.Spectroscopy.ViewSpectraStates
{
	public class SpectraViewerStateManager
	{
		private SpectraViewerStateBase m_CurrentState;
		private PictureBox m_View;
		private MasterSpectra m_MasterSpectra;
	    private frmViewSpectra m_frmViewSpectra;
	    private SpectroscopyController m_SpectroscopyController;

		public const int X_AXIS_WIDTH = 60;
		public const int Y_AXIS_WIDTH = 50;
		public const int BORDER_GAP = 10;

		private int m_XOffset;
		private float m_XCoeff;
		private float m_ColorCoeff;
		private float m_YCoeff;
		private float m_XCoeffCalibrated;
		private float m_YCoeffCalibrated;

	    private bool m_ShowCommonLines = false;


		internal SpectraViewerStateManager(SpectroscopyController spectroscopyController, PictureBox view, frmViewSpectra frmViewSpectra)
		{
			m_View = view;
		    m_frmViewSpectra = frmViewSpectra;
		    m_SpectroscopyController = spectroscopyController;
		}

		public void ChangeState<TNewState>() where TNewState : SpectraViewerStateBase, new()
		{
			if (m_CurrentState != null) 
				m_CurrentState.Finalise();
			
			m_CurrentState = null;

			m_CurrentState = new TNewState();
			m_CurrentState.SetMasterSpectra(m_MasterSpectra);
            m_CurrentState.Initialise(this, m_View, m_SpectroscopyController);
		}

		internal void Redraw()
		{
		    m_frmViewSpectra.PlotSpectra();
            m_View.Invalidate();
            m_View.Update();
		}

		public void SetMasterSpectra(MasterSpectra masterSpectra)
		{
			m_MasterSpectra = masterSpectra;
			if (m_CurrentState != null)
				m_CurrentState.SetMasterSpectra(masterSpectra);

			m_XOffset = masterSpectra.ZeroOrderPixelNo - 10;
		    int pointToFit = masterSpectra.Points.Count - masterSpectra.ZeroOrderPixelNo - masterSpectra.Points[0].PixelNo;
            m_XCoeff = m_View.Width * 1.0f / pointToFit;
            m_XCoeffCalibrated = (m_View.Width - X_AXIS_WIDTH - BORDER_GAP) * 1.0f / pointToFit;
            m_ColorCoeff = 255.0f / masterSpectra.MaxSpectraValue;
			m_YCoeff = (m_View.Height - 2 * BORDER_GAP) * 1.0f / masterSpectra.MaxSpectraValue;
			m_YCoeffCalibrated = (m_View.Height - Y_AXIS_WIDTH - 2 * BORDER_GAP) * 1.0f / masterSpectra.MaxSpectraValue;
		}

		internal int GetSpectraPixelNoFromMouseCoordinates(Point point)
		{
			if (m_SpectroscopyController.IsCalibrated())
				return (int)Math.Round((point.X - X_AXIS_WIDTH) / m_XCoeffCalibrated + m_XOffset);
			else
				return (int)Math.Round(point.X / m_XCoeff + m_XOffset);
		}

		internal float GetMouseXFromSpectraPixel(int pixelNo)
		{
			if (m_SpectroscopyController.IsCalibrated())
				return X_AXIS_WIDTH + m_XCoeffCalibrated * (pixelNo - m_XOffset);
			else
				return m_XCoeff * (pixelNo - m_XOffset);
		}

        internal float YMin
        {
            get
            {
                bool isCalibrated = m_SpectroscopyController.IsCalibrated();
                return isCalibrated ? BORDER_GAP : 0;
            }
        }

        internal float YMax
        {
            get
            {
                bool isCalibrated = m_SpectroscopyController.IsCalibrated();
                return isCalibrated ? m_View.Height - Y_AXIS_WIDTH : m_View.Height;
            }
        }

        internal void ShowCommonLines(bool show)
        {
            m_ShowCommonLines = show;
            Redraw();
        }

		public void DrawSpectra(PictureBox picSpectra, PictureBox picSpectraGraph, bool drawPixels, TangraConfig.SpectraViewDisplaySettings displaySettings)
		{
			PointF prevPoint = PointF.Empty;
			bool isCalibrated = m_SpectroscopyController.IsCalibrated();
			int xAxisOffset = isCalibrated ? X_AXIS_WIDTH : 0;
			int yAxisOffset = isCalibrated ? Y_AXIS_WIDTH : 0;
			float xCoeff = isCalibrated ? m_XCoeffCalibrated : m_XCoeff;
			float yCoeff = isCalibrated ? m_YCoeffCalibrated : m_YCoeff;

			int pixelArrWidth = m_MasterSpectra.Pixels.GetLength(0);
			int pixelArrHeight = m_MasterSpectra.Pixels.GetLength(1);
			float xCoeffPixels = picSpectra.Image.Width  * 1f / pixelArrWidth;
			float yCoeffPixels = picSpectra.Image.Height  * 1f / pixelArrHeight;

			using (Graphics g = Graphics.FromImage(picSpectra.Image))
			using (Graphics g2 = Graphics.FromImage(picSpectraGraph.Image))
			{
                g2.Clear(displaySettings.PlotBackgroundColor);

				if (m_CurrentState != null)
					m_CurrentState.PreDraw(g2);

				if (m_ShowCommonLines && isCalibrated)
					ShowCommonLines(g2, displaySettings);

                #region Clear both ends of the stipes spectra
                float xStripsBeg = xAxisOffset + xCoeff * (m_MasterSpectra.Points[0].PixelNo - m_XOffset) - 1;
                float xStripsEnd = xAxisOffset + xCoeff * (m_MasterSpectra.Points[m_MasterSpectra.Points.Count - 1].PixelNo - m_XOffset) + 1;
				if (xStripsBeg > 0) g.FillRectangle(displaySettings.GreyBrushes[0], 0, 0, xStripsBeg, picSpectra.Height);
				if (xStripsEnd < picSpectra.Width) g.FillRectangle(displaySettings.GreyBrushes[0], xStripsBeg, 0, picSpectra.Width - xStripsBeg, picSpectra.Height);
                #endregion

                RectangleF calibratedGraphArea = new RectangleF(X_AXIS_WIDTH, BORDER_GAP, m_View.Width - X_AXIS_WIDTH - BORDER_GAP, m_View.Height - Y_AXIS_WIDTH - BORDER_GAP);

				for (int i = 0; i < m_MasterSpectra.Points.Count; i++)
				{
					SpectraPoint point = m_MasterSpectra.Points[i];

					byte clr = (byte)(Math.Max(0, Math.Min(255, Math.Round(point.ProcessedValue * m_ColorCoeff))));
					float x = xAxisOffset + xCoeff * (point.PixelNo - m_XOffset);
					if (x >= 0)
					{
						if (drawPixels)
						{
							for (int y = 0; y < pixelArrHeight; y++)
							{
								byte pixClr = (byte)(Math.Max(0, Math.Min(255, Math.Round(m_MasterSpectra.Pixels[i, y] * m_ColorCoeff))));
								float xx = xAxisOffset + i * xCoeffPixels;
								float yy = y * yCoeffPixels;
								g.FillRectangle(displaySettings.GreyBrushes[pixClr], xx, yy, 1, 1);
							}
						}
						else
							g.FillRectangle(displaySettings.GreyBrushes[clr], x, 0, xCoeff, picSpectra.Height);

                        PointF graphPoint = new PointF(x, picSpectraGraph.Image.Height - BORDER_GAP - (float)Math.Round(point.ProcessedValue * yCoeff) - yAxisOffset);
						if (prevPoint != PointF.Empty)
						{
                            if (!isCalibrated || (prevPoint.X > calibratedGraphArea.Left && prevPoint.X < calibratedGraphArea.Right && graphPoint.X > calibratedGraphArea.Left && graphPoint.X < calibratedGraphArea.Right))
								g2.DrawLine(displaySettings.SpectraPen, prevPoint, graphPoint);

						}
						prevPoint = graphPoint;
					}
				}

                g2.FillRectangle(SystemBrushes.ControlDark, X_AXIS_WIDTH, 0, m_View.Width - X_AXIS_WIDTH - BORDER_GAP + 1, BORDER_GAP);

				if (m_CurrentState != null)
					m_CurrentState.PostDraw(g2);

				if (isCalibrated)
					DrawAxis(picSpectraGraph, g2, calibratedGraphArea, displaySettings);

				g.Save();
				g2.Save();
			}

			picSpectra.Invalidate();
			picSpectraGraph.Invalidate();
		}

		private void DrawAxis(PictureBox picSpectraGraph, Graphics g, RectangleF calibratedGraphArea, TangraConfig.SpectraViewDisplaySettings displaySettings)
		{
			g.DrawRectangle(displaySettings.GridLinesPen, calibratedGraphArea.Left, calibratedGraphArea.Top, calibratedGraphArea.Width, calibratedGraphArea.Height);
			g.DrawRectangle(displaySettings.GridLinesPen, calibratedGraphArea.Left, m_View.Height - 10, calibratedGraphArea.Width, m_View.Height);

		    SpectraCalibrator calibrator = m_SpectroscopyController.GetSpectraCalibrator();

			DrawXAxisMarks(g, calibratedGraphArea, calibrator, displaySettings);
			DrawYAxisMarks(g, calibratedGraphArea, picSpectraGraph, displaySettings);
		}

		private void DrawXAxisMarks(Graphics g, RectangleF calibratedGraphArea, SpectraCalibrator calibrator, TangraConfig.SpectraViewDisplaySettings displaySettings)
		{
			// Draw X axis marks, from 0, every 250 A with a label every 1000 A/2000 A
			int pixel0 = calibrator.ResolvePixelNo(0);
			int pixel1000 = calibrator.ResolvePixelNo(1000);
			float mousePosX0 = GetMouseXFromSpectraPixel(pixel0);
			float mousePosX1000 = GetMouseXFromSpectraPixel(pixel1000);
			float scalePer1000 = (mousePosX1000 - mousePosX0);
			SizeF xLabelSize = g.MeasureString("9000xx", displaySettings.LegendFont);

			int labelIncrement = 1000;
			if (xLabelSize.Width < scalePer1000 / 4)
				labelIncrement = 250;
			else if (xLabelSize.Width < scalePer1000 / 2)
				labelIncrement = 500;
			else if (xLabelSize.Width >= scalePer1000 / 2 && xLabelSize.Width < scalePer1000)
				labelIncrement = 1000;
			else if (xLabelSize.Width >= scalePer1000 && xLabelSize.Width < 2 * scalePer1000)
				labelIncrement = 2000;

			float maxWaveLengthInSeries = m_MasterSpectra.Points[m_MasterSpectra.Points.Count - 1].Wavelength;
			int currWaveLength = 0;
			float currXTickPos = GetMouseXFromSpectraPixel(calibrator.ResolvePixelNo(currWaveLength));
			do
			{
				int tickSize = currWaveLength % 1000 == 0 ? 6 : 3;
				g.DrawLine(displaySettings.GridLinesPen, currXTickPos, m_View.Height - 10, currXTickPos, m_View.Height - 10 - tickSize);
				g.DrawLine(displaySettings.GridLinesPen, currXTickPos, YMax, currXTickPos, YMax + tickSize);

				if (currWaveLength % labelIncrement == 0)
				{
					string labelText = currWaveLength.ToString(CultureInfo.CurrentCulture);
					xLabelSize = g.MeasureString(labelText, displaySettings.LegendFont);
					g.DrawString(labelText, displaySettings.LegendFont, displaySettings.LegendBrush, currXTickPos - (xLabelSize.Width / 2f), m_View.Height - 35);
				}

				currWaveLength += 250;
				currXTickPos = GetMouseXFromSpectraPixel(calibrator.ResolvePixelNo(currWaveLength));
			}
			while (currXTickPos < calibratedGraphArea.Width + X_AXIS_WIDTH && currWaveLength < maxWaveLengthInSeries + 1);
		}

		private void DrawYAxisMarks(Graphics g, RectangleF calibratedGraphArea, PictureBox picSpectraGraph, TangraConfig.SpectraViewDisplaySettings displaySettings)
		{
			float maxDisplayedValue = (calibratedGraphArea.Height - BORDER_GAP) / m_YCoeffCalibrated;
			SizeF yLabelSize = g.MeasureString("9000xx", displaySettings.LegendFont);

			int[] probeIntervals = new[] { 1, 2, 5, 10, 25, 50, 100, 250, 500, 1000, 2500, 5000, 10000, 25000, 50000, 100000, 250000, 500000 };

			int probeInt = 100;
			for(int i = 0; i < probeIntervals.Length; i++)
			{
				probeInt = probeIntervals[i];
				float ticks = maxDisplayedValue / probeInt;
				if (ticks > 5 && ticks < 15)
				{
					break;
				}
			}

			float currYTickPos = 0;
			int currYTickValue = 0;
			do
			{
				currYTickPos = picSpectraGraph.Image.Height - BORDER_GAP - m_YCoeffCalibrated * currYTickValue - Y_AXIS_WIDTH;
				string labelCaption = currYTickValue.ToString();
				yLabelSize = g.MeasureString(labelCaption, displaySettings.LegendFont);
				g.DrawLine(displaySettings.GridLinesPen, X_AXIS_WIDTH, currYTickPos, X_AXIS_WIDTH - 6, currYTickPos);
				g.DrawString(labelCaption, displaySettings.LegendFont, displaySettings.LegendBrush, X_AXIS_WIDTH - 12 - yLabelSize.Width, currYTickPos - yLabelSize.Height / 2);
				currYTickValue += probeInt;
			} 
			while (currYTickPos > BORDER_GAP && currYTickValue < maxDisplayedValue);
		}

		private void ShowCommonLines(Graphics g, TangraConfig.SpectraViewDisplaySettings displaySettings)
        {
            SpectraCalibrator calibrator = m_SpectroscopyController.GetSpectraCalibrator();

			SizeF measuredLabel = g.MeasureString("H", displaySettings.LegendFont);
            float verticalSpacing = measuredLabel.Height * 1.3f;

            for (int i = 0; i < SpectraLineLibrary.CommonLines.Count; i++)
            {
                LineEntry line = SpectraLineLibrary.CommonLines[i];
                float x2 = 0;

                if (line.IsWideArea)
                {
                    int pixelNo = calibrator.ResolvePixelNo(line.FromWavelength);
                    float x1 = GetMouseXFromSpectraPixel(pixelNo);
                    pixelNo = calibrator.ResolvePixelNo(line.ToWavelength);
                    x2 = GetMouseXFromSpectraPixel(pixelNo);

					g.FillRectangle(displaySettings.KnownLineBrush, x1, i * verticalSpacing + 2 * BORDER_GAP, x2 - x1, m_View.Height - i * verticalSpacing - 2 * BORDER_GAP - Y_AXIS_WIDTH);
					g.FillRectangle(displaySettings.KnownLineBrush, x1, m_View.Height - 10, x2 - x1, m_View.Height);
                }
                else
                {
                    int pixelNo = calibrator.ResolvePixelNo(line.FromWavelength);
                    x2 = GetMouseXFromSpectraPixel(pixelNo);

					g.DrawLine(displaySettings.KnownLinePen, x2, i * verticalSpacing + 2 * BORDER_GAP, x2, m_View.Height - Y_AXIS_WIDTH);
					g.DrawLine(displaySettings.KnownLinePen, x2, m_View.Height - 10, x2, m_View.Height);

                }

				g.DrawString(line.Designation != null 
                    ? string.Format("{0} ({1})", line.Element, line.Designation)
					: line.Element, displaySettings.LegendFont, displaySettings.KnownLineLabelBrush, x2 + 3, i * verticalSpacing + 2 * BORDER_GAP);
            }
        }

		public void MouseClick(object sender, MouseEventArgs e)
		{
			if (m_CurrentState != null) 
				m_CurrentState.MouseClick(sender, e);
		}

		public void MouseDown(object sender, MouseEventArgs e)
		{
			if (m_CurrentState != null)
				m_CurrentState.MouseDown(sender, e);
		}

		public void MouseEnter(object sender, EventArgs e)
		{
			if (m_CurrentState != null)
				m_CurrentState.MouseEnter(sender, e);
		}

		public void MouseLeave(object sender, EventArgs e)
		{
			if (m_CurrentState != null)
				m_CurrentState.MouseLeave(sender, e);
		}

		public void MouseMove(object sender, MouseEventArgs e)
		{
			if (m_CurrentState != null)
				m_CurrentState.MouseMove(sender, e);
		}

		public void MouseUp(object sender, MouseEventArgs e)
		{
			if (m_CurrentState != null)
				m_CurrentState.MouseUp(sender, e);
		}

        public void PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (m_CurrentState != null)
                m_CurrentState.PreviewKeyDown(sender, e);
        }

        private static float FWHM_COEFF = (float)(4 * Math.Log(2));

        private float GetGaussianValue(float fwhm, float distance)
        {
            // FWHM * FWHM = 4 * (2 ln(2)) * c * c => 2*c*c = FWHM*FWHM / (4 * ln(2))
            return (float)Math.Exp(-FWHM_COEFF * distance * distance / (fwhm * fwhm));
        }

        public void ApplyGaussianBlur(float fwhm)
        {
            if (Math.Abs(fwhm) < 0.0001 || float.IsNaN(fwhm))
            {
                foreach (SpectraPoint point in m_MasterSpectra.Points)
                    point.ProcessedValue = point.RawValue;

                m_MasterSpectra.ProcessingInfo.GaussianBlur10Fwhm = null;
            }
            else
            {
                float[] kernel = new float[21];
                for (int i = 0; i < 10; i++)
                {
                    kernel[10 + i] = kernel[10 - i] = GetGaussianValue(fwhm, i);
                }

                for (int i = 0; i < m_MasterSpectra.Points.Count; i++)
                {
                    SpectraPoint point = m_MasterSpectra.Points[i];
                    float sum = 0;
                    float weight = 0;
                    for (int j = -10; j <= 10; j++)
                    {
                        if (j + i > 0 && j + i < m_MasterSpectra.Points.Count - 1)
                        {
                            weight += kernel[j + 10];
                            sum += kernel[j + 10] * m_MasterSpectra.Points[j + i].RawValue;
                        }
                    }

                    if (weight > 0)
                        point.ProcessedValue = sum / weight;
                    else
                        point.ProcessedValue = point.RawValue;
                }

                m_MasterSpectra.ProcessingInfo.GaussianBlur10Fwhm = (int)Math.Round(fwhm * 10);
            }

            Redraw();
        }
	}
}
