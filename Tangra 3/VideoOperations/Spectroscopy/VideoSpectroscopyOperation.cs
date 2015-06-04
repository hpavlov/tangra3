using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Controller;
using Tangra.Helpers;
using Tangra.Model.Astro;
using Tangra.Model.Image;
using Tangra.Model.ImageTools;
using Tangra.Model.Video;
using Tangra.Model.VideoOperations;

namespace Tangra.VideoOperations.Spectroscopy
{
    public enum SpectroscopyState
    {
        ChoosingStar,
        StarConfirmed,
        RunningMeasurements
    }

    public class VideoSpectroscopyOperation : VideoOperationBase, IVideoOperation
    {
        private IVideoController m_VideoController;
        private SpectroscopyController m_SpectroscopyController;
        private ucSpectroscopy m_ControlPanel = null;
        private object m_SyncRoot = new object();

        private int m_OriginalWidth;
        private int m_OriginalHeight;
        private RectangleF m_OriginalVideoFrame;

        private SpectroscopyState m_OperationState = SpectroscopyState.ChoosingStar;
        private ImagePixel m_SelectedStar = null;
        private double m_SelectedStarFWHM;
        private float m_SelectedStarBestAngle;

        public VideoSpectroscopyOperation()
        { } 

        public VideoSpectroscopyOperation(SpectroscopyController spectroscopyController, bool debugMode)
        {
            m_SpectroscopyController = spectroscopyController;
        }

        public bool InitializeOperation(IVideoController videoContoller, Panel controlPanel, IFramePlayer framePlayer, Form topForm)
        {
            m_VideoController = videoContoller;

            if (m_ControlPanel == null)
            {
                lock (m_SyncRoot)
                {
                    if (m_ControlPanel == null)
                    {
                        m_ControlPanel = new ucSpectroscopy(this);
                    }
                }
            }

            controlPanel.Controls.Clear();
            controlPanel.Controls.Add(m_ControlPanel);
            m_ControlPanel.Dock = DockStyle.Fill;

            m_OperationState = SpectroscopyState.ChoosingStar;
            m_SelectedStar = null;

            m_OriginalWidth = framePlayer.Video.Width;
            m_OriginalHeight = framePlayer.Video.Height;
            m_OriginalVideoFrame = new Rectangle(0, 0, m_OriginalWidth, m_OriginalHeight);

            return true;
        }

        public void FinalizeOperation()
        {
            
        }

        public void PlayerStarted()
        {
            
        }

        public void NextFrame(int frameNo, MovementType movementType, bool isLastFrame, AstroImage astroImage, int firstFrameInIntegrationPeriod, string fileName)
        {
            
        }

        public void ImageToolChanged(ImageTool newTool, ImageTool oldTool)
        {
            
        }

        public void PreDraw(Graphics g)
        {
            
        }

        public void PostDraw(Graphics g)
        {
            if (m_SelectedStar != null)
            {
                g.DrawEllipse(Pens.Aqua, (float) m_SelectedStar.XDouble - 5, (float) m_SelectedStar.YDouble - 5, 10, 10);
                var mapper = new RotationMapper(m_OriginalWidth, m_OriginalHeight, m_SelectedStarBestAngle);
                float halfWidth = (float)m_SelectedStarFWHM;

                PointF p0 = mapper.GetDestCoords((float) m_SelectedStar.XDouble, (float) m_SelectedStar.YDouble);
                for (float i = p0.X - mapper.MaxDestDiagonal; i < p0.X + mapper.MaxDestDiagonal; i++)
                {
                    PointF p1 = mapper.GetSourceCoords(i, p0.Y - halfWidth);
                    PointF p2 = mapper.GetSourceCoords(i + 1, p0.Y - halfWidth);
                    if (m_OriginalVideoFrame.Contains(p1) && m_OriginalVideoFrame.Contains(p2)) g.DrawLine(Pens.Red, p1, p2);

                    PointF p3 = mapper.GetSourceCoords(i, p0.Y + halfWidth);
                    PointF p4 = mapper.GetSourceCoords(i + 1, p0.Y + halfWidth);
                    if (m_OriginalVideoFrame.Contains(p3) && m_OriginalVideoFrame.Contains(p4)) g.DrawLine(Pens.Red, p3, p4);
                }
            }
        }

        public override void MouseClick(ObjectClickEventArgs e)
        {
            if (m_OperationState == SpectroscopyState.ChoosingStar && e.Gausian != null && e.Gausian.IsSolved && e.Gausian.Certainty > 0.2)
            {
                float bestAngle = m_SpectroscopyController.LocateSpectraAngle(e.Gausian);

                m_SelectedStar = new ImagePixel(e.Gausian.XCenter, e.Gausian.YCenter);
                m_SelectedStarFWHM = e.Gausian.FWHM;
                m_SelectedStarBestAngle = bestAngle;

				var reader = new SpectraReader(m_VideoController.GetCurrentAstroImage(false), bestAngle);
				Spectra spectra = reader.ReadSpectra((float)e.Gausian.XCenter, (float)e.Gausian.YCenter, (int)Math.Ceiling(m_SelectedStarFWHM));
				m_ControlPanel.DisplaySpectra(spectra);

                m_VideoController.RedrawCurrentFrame(false, true);
            }
        }

        public bool HasCustomZoomImage
        {
            get { return false; }
        }

        public bool DrawCustomZoomImage(System.Drawing.Graphics g, int width, int height)
        {
            return false;
        }

        public bool AvoidImageOverlays
        {
            get
            {
                // No overlays allowed during the whole process
                return true;
            }
        }
    }
}
