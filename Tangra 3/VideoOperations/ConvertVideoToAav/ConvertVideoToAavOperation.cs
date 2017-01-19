using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Controller;
using Tangra.Model.Context;
using Tangra.Model.ImageTools;
using Tangra.Model.VideoOperations;

namespace Tangra.VideoOperations.ConvertVideoToAav
{
    public class ConvertVideoToAavOperation : VideoOperationBase, IVideoOperation
    {
        private IVideoController m_VideoController;
        private ConvertVideoToAavController m_ConvertVideoToAavController;

        private ucConvertVideoToAav m_ControlPanel;
        private object m_SyncRoot = new object();

        private bool m_DebugMode;
        private bool m_Converting;

        public ConvertVideoToAavOperation()
        {

        }

        public ConvertVideoToAavOperation(ConvertVideoToAavController convertVideoToAavController, bool debugMode)
        {
            m_ConvertVideoToAavController = convertVideoToAavController;
            m_DebugMode = debugMode;
        }        

        public bool InitializeOperation(IVideoController videoController, System.Windows.Forms.Panel controlPanel, Model.Video.IFramePlayer framePlayer, System.Windows.Forms.Form topForm)
        {
            m_VideoController = (VideoController)videoController;

            if (m_ControlPanel == null)
            {
                lock (m_SyncRoot)
                {
                    if (m_ControlPanel == null)
                    {
                        m_ControlPanel = new ucConvertVideoToAav(this, (VideoController)videoController);
                    }
                }
            }

            controlPanel.Controls.Clear();
            controlPanel.Controls.Add(m_ControlPanel);
            m_ControlPanel.Dock = DockStyle.Fill;

            TangraContext.Current.CanPlayVideo = false;
            m_VideoController.UpdateViews();

            m_Converting = false;

            return true;
        }

        public void FinalizeOperation()
        { }

        public void PlayerStarted()
        { }

        public void NextFrame(int frameNo, Model.Video.MovementType movementType, bool isLastFrame, Model.Astro.AstroImage astroImage, int firstFrameInIntegrationPeriod, string fileName)
        {
            if (m_Converting)
            {
                m_ConvertVideoToAavController.ProcessFrame(frameNo, astroImage);

                if (isLastFrame)
                {
                    m_ConvertVideoToAavController.FinishedConversion();
                    m_ControlPanel.EndConversion();
                }
                else
                    m_ControlPanel.UpdateProgress(frameNo, m_VideoController.VideoLastFrame);
            }
        }

        public void StartConversion(string fileName, int topVtiOsdRow, int bottomVtiOsdRow, int firstIntegrationFrameNo, int integrationInterval, string cameraModel, string sensorInfo, bool swapTimestampFields)
        {
            m_Converting = true;

            m_ConvertVideoToAavController.StartConversion(fileName, topVtiOsdRow, bottomVtiOsdRow, firstIntegrationFrameNo, integrationInterval, cameraModel, sensorInfo, swapTimestampFields);

            m_VideoController.PlayVideo(firstIntegrationFrameNo);
        }

        public void EndConversion()
        {
            m_ConvertVideoToAavController.FinishedConversion();
            m_VideoController.StopVideo();
            m_ControlPanel.EndConversion();
        }

        public void ImageToolChanged(ImageTool newTool, ImageTool oldTool)
        {
            //TODO:
        }

        public void PreDraw(Graphics g)
        {
            //TODO:
        }

        internal int PreserveVTIFirstRow;
        internal int PreserveVTILastRow;

        public void PostDraw(Graphics g)
        {
            g.DrawRectangle(Pens.Lime, 0, PreserveVTIFirstRow, TangraContext.Current.FrameWidth - 3, PreserveVTILastRow - PreserveVTIFirstRow - 3);
        }

        public bool HasCustomZoomImage
        {
            get
            {
                return false;
            }
        }

        public bool DrawCustomZoomImage(Graphics g, int width, int height)
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
