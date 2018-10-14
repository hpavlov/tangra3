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
        private int m_LastFrameNo;
        private int m_FirstFrameNo;

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

                if (isLastFrame || frameNo > m_LastFrameNo)
                {
                    m_ConvertVideoToAavController.FinishedConversion();
                    m_ControlPanel.EndConversion();

                    m_VideoController.StopVideo();
                    m_ControlPanel.UpdateProgress(m_LastFrameNo, m_LastFrameNo);
                }
                else
                    m_ControlPanel.UpdateProgress(frameNo - m_FirstFrameNo, m_LastFrameNo);
            }
        }

        public void StartConversion(
            string fileName, int topVtiOsdRow, int bottomVtiOsdRow, int leftVtiOsdCol, int rightVtiOsdCol,
            int firstIntegrationFrameNo, int integrationInterval, int lastFrameNo,
            string cameraModel, string sensorInfo, bool swapTimestampFields, bool manualIntegrationConfig, List<HeaderValuePair> additionalHeaders)
        {
            m_Converting = true;
            m_LastFrameNo = lastFrameNo;
            m_FirstFrameNo = firstIntegrationFrameNo;

            m_ConvertVideoToAavController.StartConversion(fileName, topVtiOsdRow, bottomVtiOsdRow, leftVtiOsdCol, rightVtiOsdCol, firstIntegrationFrameNo, integrationInterval, cameraModel, sensorInfo, swapTimestampFields, manualIntegrationConfig, additionalHeaders);

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
        { }

        public void PostDraw(Graphics g)
        { }

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
