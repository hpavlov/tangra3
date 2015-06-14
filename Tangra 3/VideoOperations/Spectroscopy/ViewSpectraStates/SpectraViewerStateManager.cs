using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Controller;
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

		public const int X_AXIS_WIDTH = 100;
		public const int Y_AXIS_WIDTH = 50;
		public const int BORDER_GAP = 10;

		private int m_XOffset;
		private float m_XCoeff;
		private float m_ColorCoeff;
		private float m_YCoeff;
		private float m_XCoeffCalibrated;
		private float m_YCoeffCalibrated;

	    private bool m_ShowCommonLines = false;

        private static Brush[] s_GreyBrushes = new Brush[256];
        private static Font s_LegendFont = new Font(FontFamily.GenericSansSerif, 8, FontStyle.Bold);
	    private static Pen s_KnownLinePen = new Pen(Color.FromArgb(60, 0, 0, 255));
        private static Brush s_KnownLineBrush = new SolidBrush(Color.FromArgb(60, 0, 0, 255));
		private static Pen s_GridLinesPen = new Pen(Color.FromArgb(180, 180, 180));
		private static Pen s_SpectraPen = new Pen(Color.Aqua, 1);
		private static Brush s_KnownLineLabelBrush = Brushes.Blue;
		
		static SpectraViewerStateManager()
	    {
		    for (int i = 0; i < 256; i++)
		    {
                s_GreyBrushes[i] = new SolidBrush(Color.FromArgb(i, i, i));
		    }
	    }

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
            m_CurrentState.Initialise(this, m_View, m_SpectroscopyController);
			m_CurrentState.SetMasterSpectra(m_MasterSpectra);
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
			m_XCoeff = m_View.Width * 1.0f / (masterSpectra.Points.Count - masterSpectra.ZeroOrderPixelNo);
			m_XCoeffCalibrated = (m_View.Width - X_AXIS_WIDTH) * 1.0f / (masterSpectra.Points.Count - masterSpectra.ZeroOrderPixelNo);
			m_ColorCoeff = 255.0f / masterSpectra.MaxPixelValue;
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

        internal void ShowCommonLines(bool show)
        {
            m_ShowCommonLines = show;
            Redraw();
        }

		public void DrawSpectra(PictureBox picSpectra, PictureBox picSpectraGraph)
		{
			PointF prevPoint = PointF.Empty;
			bool isCalibrated = m_SpectroscopyController.IsCalibrated();
			int xAxisOffset = isCalibrated ? X_AXIS_WIDTH: 0;
			int yAxisOffset = isCalibrated ? Y_AXIS_WIDTH : 0;
			float xCoeff = isCalibrated ? m_XCoeffCalibrated : m_XCoeff;
			float yCoeff = isCalibrated ? m_YCoeffCalibrated : m_YCoeff;

			using (Graphics g = Graphics.FromImage(picSpectra.Image))
			using (Graphics g2 = Graphics.FromImage(picSpectraGraph.Image))
			{
                g2.Clear(SystemColors.ControlDark);

				if (m_CurrentState != null)
					m_CurrentState.PreDraw(g2);

				if (m_ShowCommonLines && isCalibrated)
                    ShowCommonLines(g2);

				foreach (SpectraPoint point in m_MasterSpectra.Points)
				{
					byte clr = (byte)(Math.Max(0, Math.Min(255, Math.Round(point.RawValue * m_ColorCoeff))));
					float x = xAxisOffset + xCoeff * (point.PixelNo - m_XOffset);
					if (x >= 0)
					{
						g.FillRectangle(s_GreyBrushes[clr], x, 0, xCoeff, picSpectra.Height);

						PointF graphPoint = new PointF(x, picSpectraGraph.Image.Height - BORDER_GAP - (float)Math.Round(point.RawValue * yCoeff) - yAxisOffset);
						if (prevPoint != PointF.Empty)
						{
							g2.DrawLine(s_SpectraPen, prevPoint, graphPoint);

							//if (graphPoint.X > 0 && graphPoint.X < picSpectraGraph.Image.Width && graphPoint.Y > 0 &&
							//	graphPoint.Y < picSpectraGraph.Image.Height &&
							//	prevPoint.X > 0 && prevPoint.X < picSpectraGraph.Image.Width && prevPoint.Y > 0 &&
							//	prevPoint.Y < picSpectraGraph.Image.Height)
							//{
							//	g2.DrawLine(s_SpectraPen, prevPoint, graphPoint);
							//}
						}
						prevPoint = graphPoint;
					}
				}

				if (m_CurrentState != null)
					m_CurrentState.PostDraw(g2);

				if (isCalibrated)
					DrawAxis(g2);

				g.Save();
				g2.Save();
			}

			picSpectra.Invalidate();
			picSpectraGraph.Invalidate();
		}

		private void DrawAxis(Graphics g)
		{
			g.DrawRectangle(s_GridLinesPen, X_AXIS_WIDTH, BORDER_GAP, m_View.Width - X_AXIS_WIDTH - BORDER_GAP, m_View.Height - Y_AXIS_WIDTH - BORDER_GAP);
		}

        private void ShowCommonLines(Graphics g)
        {
            SpectraCalibrator calibrator = m_SpectroscopyController.GetSpectraCalibrator();

            SizeF measuredLabel = g.MeasureString("H", s_LegendFont);
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

					g.FillRectangle(s_KnownLineBrush, x1, i * verticalSpacing + BORDER_GAP, x2 - x1, m_View.Height - i * verticalSpacing - BORDER_GAP - Y_AXIS_WIDTH);
                }
                else
                {
                    int pixelNo = calibrator.ResolvePixelNo(line.FromWavelength);
                    x2 = GetMouseXFromSpectraPixel(pixelNo);

					g.DrawLine(s_KnownLinePen, x2, i * verticalSpacing + BORDER_GAP, x2, m_View.Height - Y_AXIS_WIDTH);
                }

				g.DrawString(line.Designation != null ? string.Format("{0} ({1})", line.Element, line.Designation) : line.Element, s_LegendFont, s_KnownLineLabelBrush, x2 + 3, i * verticalSpacing + BORDER_GAP);
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
	}
}
