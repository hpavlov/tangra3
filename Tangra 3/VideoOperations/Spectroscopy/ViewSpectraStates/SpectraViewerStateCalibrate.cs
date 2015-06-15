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
		private SpectraPoint m_FirstCalibrationPoint;
        private frmEnterWavelength m_frmEnterWavelength = null;

		private static int SEARCH_AREA_WING = 5;

        public override void Initialise(SpectraViewerStateManager manager, PictureBox view, SpectroscopyController spectroscopyController)
		{
            base.Initialise(manager, view, spectroscopyController);

			view.Cursor = Cursors.Arrow;
		}

        public override void Finalise()
        {
            if (m_frmEnterWavelength != null)
            {
                try
                {
                    if (m_frmEnterWavelength.Visible)
                        m_frmEnterWavelength.Close();
                }
                catch
                { }

                try
                {
                    m_frmEnterWavelength.Dispose();
                }
                catch
                { }

                m_frmEnterWavelength = null;
            }

            base.Finalise();
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
					List<SpectraPoint> pointsInArea = m_MasterSpectra.Points.Where(x => x.PixelNo >= m_SelectedPoint.PixelNo - SEARCH_AREA_WING && x.PixelNo <= m_SelectedPoint.PixelNo + SEARCH_AREA_WING).ToList();
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

					// Check if local maximum or minimum
					if (minPixelNo != m_SelectedPoint.PixelNo && minPixelNo > m_SelectedPoint.PixelNo - SEARCH_AREA_WING && minPixelNo < m_SelectedPoint.PixelNo + SEARCH_AREA_WING)
						m_SelectedPoint = m_MasterSpectra.Points.Single(x => x.PixelNo == minPixelNo);
					else if (maxPixelNo != m_SelectedPoint.PixelNo && maxPixelNo > m_SelectedPoint.PixelNo - SEARCH_AREA_WING && maxPixelNo < m_SelectedPoint.PixelNo + SEARCH_AREA_WING)
						m_SelectedPoint = m_MasterSpectra.Points.Single(x => x.PixelNo == maxPixelNo);
                }

                m_StateManager.Redraw();
				m_SpectroscopyController.SelectPixel(m_SelectedPoint.PixelNo);
                EnsureEnterWavelengthForm();
            }
		}

        private void EnsureEnterWavelengthForm()
        {
            if (m_frmEnterWavelength == null)
                m_frmEnterWavelength = new frmEnterWavelength(this);

			var parentForm = m_View.FindForm();
			if (parentForm != null)
			{
				if (!m_frmEnterWavelength.Visible)
				{
					try
					{
						m_frmEnterWavelength.Show(m_View);
					}
					catch (ObjectDisposedException)
					{
						m_frmEnterWavelength = new frmEnterWavelength(this);
						m_frmEnterWavelength.Show(m_View);
					}

					m_frmEnterWavelength.Top = parentForm.Top;
					m_frmEnterWavelength.Left = parentForm.Right;

					parentForm.Focus();
				}
			}
        }

        internal void CalibrationPointSelected(float selectedWaveLength)
        {
            m_SpectroscopyController.SetMarker(m_SelectedPoint.PixelNo, selectedWaveLength);

	        if (m_SpectroscopyController.IsCalibrated())
		        m_StateManager.ChangeState<SpectraViewerStateCalibrated>();
	        else
	        {
		        m_FirstCalibrationPoint = m_SelectedPoint;
		        m_SelectedPoint = null;

				m_StateManager.Redraw();
	        }
        }

		internal void CalibrationPointSelectionCancelled()
		{
			m_SelectedPoint = null;
			m_SpectroscopyController.DeselectPixel();
			m_StateManager.Redraw();
		}

		public override void PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			if (m_SelectedPoint != null)
			{
				if (e.KeyCode == Keys.Left)
				{
					int newPixNo = m_SelectedPoint.PixelNo - 1;
					if (newPixNo >= m_MasterSpectra.Points[0].PixelNo)
						m_SelectedPoint = m_MasterSpectra.Points.Single(x => x.PixelNo == newPixNo);

					m_SpectroscopyController.SelectPixel(m_SelectedPoint.PixelNo);
					m_StateManager.Redraw();
				}
				else if (e.KeyCode == Keys.Right)
				{
					int newPixNo = m_SelectedPoint.PixelNo + 1;
					if (newPixNo <= m_MasterSpectra.Points[m_MasterSpectra.Points.Count - 1].PixelNo)
						m_SelectedPoint = m_MasterSpectra.Points.Single(x => x.PixelNo == newPixNo);

					m_SpectroscopyController.SelectPixel(m_SelectedPoint.PixelNo);
					m_StateManager.Redraw();
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

			if (m_FirstCalibrationPoint != null)
			{

				float x1 = m_StateManager.GetMouseXFromSpectraPixel(m_FirstCalibrationPoint.PixelNo);
				g.DrawLine(Pens.Gray, x1, 0, x1, m_View.Height);
			}
		}
	}
}
