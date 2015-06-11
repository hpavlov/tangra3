using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.VideoOperations.Spectroscopy.Helpers;

namespace Tangra.VideoOperations.Spectroscopy.ViewSpectraStates
{
	public class SpectraViewerStateCalibrate : SpectraViewerStateBase
	{
		private SpectraPoint m_SelectedPoint;

		public override void Initialise(SpectraViewerStateManager manager, PictureBox view)
		{
			base.Initialise(manager, view);

			view.Cursor = Cursors.Cross;
		}

		public override void MouseClick(object sender, MouseEventArgs e)
		{
			int x1 = m_StateManager.GetSpectraPixelNoFromMouseCoordinates(e.Location);

			m_SelectedPoint = m_MasterSpectra.Points.SingleOrDefault(x => x.PixelNo == x1);

			m_StateManager.Redraw();
		}

		public override void PreDraw(Graphics g)
		{
			if (m_SelectedPoint != null)
			{
				float x1 = m_StateManager.GetMouseXFromSpectraPixel(m_SelectedPoint.PixelNo);
				g.DrawLine(Pens.Green, x1, 0, x1, m_View.Height);
			}
		}
	}
}
