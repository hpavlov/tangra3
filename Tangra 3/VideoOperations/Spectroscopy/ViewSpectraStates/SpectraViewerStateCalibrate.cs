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
	public class SpectraViewerStateCalibrate : SpectraViewerStateBase
	{
		private SpectraPoint m_SelectedPoint;

        public override void Initialise(SpectraViewerStateManager manager, PictureBox view, SpectroscopyController spectroscopyController)
		{
            base.Initialise(manager, view, spectroscopyController);

			view.Cursor = Cursors.Arrow;
		}

		public override void MouseClick(object sender, MouseEventArgs e)
		{
			int x1 = m_StateManager.GetSpectraPixelNoFromMouseCoordinates(e.Location);

			m_SelectedPoint = m_MasterSpectra.Points.SingleOrDefault(x => x.PixelNo == x1);

            if (m_SelectedPoint != null)
            {
                if (Control.ModifierKeys != Keys.Control)
                {
                    // Find the local maximum or minimum
                    List<SpectraPoint> pointsInArea = m_MasterSpectra.Points.Where(x => x.PixelNo >= m_SelectedPoint.PixelNo - 5 && x.PixelNo <= m_SelectedPoint.PixelNo + 5).ToList();
                    float maxValue = float.MinValue;
                    int maxPixelNo = m_SelectedPoint.PixelNo;
                    float minValue = float.MaxValue;
                    int minPixelNo = m_SelectedPoint.PixelNo;
                    foreach (var spectraPoint in pointsInArea)
                    {
                        if (spectraPoint.RawValue > maxValue)
                        {
                            maxValue = spectraPoint.RawValue;
                            maxPixelNo = spectraPoint.PixelNo;
                        }

                        if (spectraPoint.RawValue < minValue)
                        {
                            minValue = spectraPoint.RawValue;
                            minPixelNo = spectraPoint.PixelNo;
                        }
                    }

                    if (Math.Abs(minPixelNo - m_SelectedPoint.PixelNo) < Math.Abs(maxPixelNo - m_SelectedPoint.PixelNo))
                        m_SelectedPoint = m_MasterSpectra.Points.Single(x => x.PixelNo == minPixelNo);
                    else
                        m_SelectedPoint = m_MasterSpectra.Points.Single(x => x.PixelNo == maxPixelNo);
                }

                m_StateManager.Redraw();

                var frm = new frmEnterWavelength();
                if (frm.ShowDialog(m_View) == DialogResult.OK)
                {
                    m_SpectroscopyController.SetMarker(m_SelectedPoint.PixelNo, frm.SelectedWaveLength);
                    if (m_SpectroscopyController.IsCalibrated())
                        m_StateManager.ChangeState<SpectraViewerStateCalibrated>();
                }
            }
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
