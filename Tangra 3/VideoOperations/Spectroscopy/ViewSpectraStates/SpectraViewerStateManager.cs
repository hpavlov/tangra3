using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.VideoOperations.Spectroscopy.Helpers;

namespace Tangra.VideoOperations.Spectroscopy.ViewSpectraStates
{
	public class SpectraViewerStateManager
	{
		private SpectraViewerStateBase m_CurrentState;
		private PictureBox m_View;
		private MasterSpectra m_MasterSpectra;

		private int m_XOffset;
		private float m_XCoeff;
		private float m_ColorCoeff;
		private float m_YCoeff;

        private static Brush[] s_GreyBrushes = new Brush[256];

		static SpectraViewerStateManager()
	    {
		    for (int i = 0; i < 256; i++)
		    {
                s_GreyBrushes[i] = new SolidBrush(Color.FromArgb(i, i, i));
		    }
	    }

		internal SpectraViewerStateManager(PictureBox view)
		{
			m_View = view;
		}

		public void ChangeState<TNewState>() where TNewState : SpectraViewerStateBase, new()
		{
			if (m_CurrentState != null) 
				m_CurrentState.Finalise();
			
			m_CurrentState = null;

			m_CurrentState = new TNewState();
			m_CurrentState.Initialise(this, m_View);
			m_CurrentState.SetMasterSpectra(m_MasterSpectra);
		}

		internal void Redraw()
		{
			m_View.Parent.Parent.Refresh();
		}

		public void SetMasterSpectra(MasterSpectra masterSpectra)
		{
			m_MasterSpectra = masterSpectra;
			if (m_CurrentState != null)
				m_CurrentState.SetMasterSpectra(masterSpectra);

			m_XOffset = masterSpectra.ZeroOrderPixelNo - 10;
			m_XCoeff = m_View.Width * 1.0f / masterSpectra.Points.Count;
			m_ColorCoeff = 1.5f * 256.0f / masterSpectra.MaxPixelValue;
			m_YCoeff = (m_View.Width - 20) * 1.0f / masterSpectra.MaxPixelValue;
		}

		internal int GetSpectraPixelNoFromMouseCoordinates(Point point)
		{
			return (int)Math.Round(point.X/m_XCoeff + m_XOffset);
		}

		internal float GetMouseXFromSpectraPixel(int pixelNo)
		{
			return m_XCoeff * (pixelNo - m_XOffset);
		}

		public void DrawSpectra(PictureBox picSpectra, PictureBox picSpectraGraph)
		{
			PointF prevPoint = PointF.Empty;

			using (Graphics g = Graphics.FromImage(picSpectra.Image))
			using (Graphics g2 = Graphics.FromImage(picSpectraGraph.Image))
			{
				g2.Clear(Color.WhiteSmoke);

				if (m_CurrentState != null)
					m_CurrentState.PreDraw(g2);

				foreach (SpectraPoint point in m_MasterSpectra.Points)
				{
					byte clr = (byte)(Math.Round(point.RawValue * m_ColorCoeff));
					float x = m_XCoeff * (point.PixelNo - m_XOffset);
					if (x >= 0)
					{
						g.FillRectangle(s_GreyBrushes[clr], x, 0, m_XCoeff, picSpectra.Width);

						PointF graphPoint = new PointF(x, picSpectraGraph.Image.Height - 10 - (float)Math.Round(point.RawValue * m_YCoeff));
						if (prevPoint != PointF.Empty)
						{
							if (graphPoint.X > 0 && graphPoint.X < picSpectraGraph.Image.Width && graphPoint.Y > 0 &&
								graphPoint.Y < picSpectraGraph.Image.Height &&
								prevPoint.X > 0 && prevPoint.X < picSpectraGraph.Image.Width && prevPoint.Y > 0 &&
								prevPoint.Y < picSpectraGraph.Image.Height)
							{
								g2.DrawLine(Pens.Red, prevPoint, graphPoint);
							}
						}
						prevPoint = graphPoint;
					}
				}

				if (m_CurrentState != null)
					m_CurrentState.PostDraw(g2);

				g.Save();
				g2.Save();
			}

			picSpectra.Invalidate();
			picSpectraGraph.Invalidate();
		}


		public virtual void MouseClick(object sender, MouseEventArgs e)
		{
			if (m_CurrentState != null) 
				m_CurrentState.MouseClick(sender, e);
		}

		public virtual void MouseDown(object sender, MouseEventArgs e)
		{
			if (m_CurrentState != null)
				m_CurrentState.MouseDown(sender, e);
		}

		public virtual void MouseEnter(object sender, EventArgs e)
		{
			if (m_CurrentState != null)
				m_CurrentState.MouseEnter(sender, e);
		}

		public virtual void MouseLeave(object sender, EventArgs e)
		{
			if (m_CurrentState != null)
				m_CurrentState.MouseLeave(sender, e);
		}

		public virtual void MouseMove(object sender, MouseEventArgs e)
		{
			if (m_CurrentState != null)
				m_CurrentState.MouseMove(sender, e);
		}

		public virtual void MouseUp(object sender, MouseEventArgs e)
		{
			if (m_CurrentState != null)
				m_CurrentState.MouseUp(sender, e);
		}
	}
}
