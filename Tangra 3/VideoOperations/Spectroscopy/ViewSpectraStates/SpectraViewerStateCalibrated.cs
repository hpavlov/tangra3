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
    public class SpectraViewerStateCalibrated : SpectraViewerStateBase
    {
        private SpectraPoint m_SelectedPoint;

        public override void Initialise(SpectraViewerStateManager manager, PictureBox view, SpectroscopyController spectroscopyController)
        {
            base.Initialise(manager, view, spectroscopyController);

            view.Cursor = Cursors.Arrow;

	        if (!m_MasterSpectra.IsCalibrated())
	        {
		        m_MasterSpectra.Calibration = m_SpectroscopyController.GetSpectraCalibration();
                m_SpectroscopyController.GetSpectraCalibrator().PopulateWaveLengths(m_MasterSpectra);
	        }
			

            m_StateManager.Redraw();
        }

        private static Pen s_SelectedLinePen = new Pen(Color.BlueViolet);

        public override void MouseClick(object sender, MouseEventArgs e)
        {
            int x1 = m_StateManager.GetSpectraPixelNoFromMouseCoordinates(e.Location);

            m_SelectedPoint = m_MasterSpectra.Points.SingleOrDefault(x => x.PixelNo == x1);

            if (m_SelectedPoint != null)
            {
                m_StateManager.Redraw();
                m_SpectroscopyController.SelectPixel(m_SelectedPoint.PixelNo);
            }
        }

        public override void PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
            {
                m_SelectedPoint = m_MasterSpectra.Points.SingleOrDefault(x => x.PixelNo == m_SelectedPoint.PixelNo - 1);
            }
            else if (e.KeyCode == Keys.Right)
            {
                m_SelectedPoint = m_MasterSpectra.Points.SingleOrDefault(x => x.PixelNo == m_SelectedPoint.PixelNo + 1);
            }

            if (m_SelectedPoint != null)
            {
                m_StateManager.Redraw();
                m_SpectroscopyController.SelectPixel(m_SelectedPoint.PixelNo);
            }
        }

        public override void PreDraw(Graphics g)
        {
            if (m_SelectedPoint != null)
            {
                float x1 = m_StateManager.GetMouseXFromSpectraPixel(m_SelectedPoint.PixelNo);
                float y1 = m_StateManager.YMin;
                float y2 = m_StateManager.YMax;
                g.DrawLine(s_SelectedLinePen, x1, y1, x1, y2);

                if (Math.Abs(y2 - m_View.Height) > 10)
                    g.DrawLine(s_SelectedLinePen, x1, m_View.Height - 10, x1, m_View.Height);
            }
        }
    }
}
