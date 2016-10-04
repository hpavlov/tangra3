/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Controller;
using Tangra.Model.Astro;
using Tangra.Model.Context;
using Tangra.Model.ImageTools;
using Tangra.Model.Video;
using Tangra.Model.VideoOperations;
using Tangra.VideoOperations.MakeDarkFlatField;

namespace Tangra.VideoOperations.ConvertVideoToFits
{
    public enum ConvertVideoToFitsState
    {
        Configuring,
        Converting
    }

    public class ConvertVideoToFitsOperation : VideoOperationBase, IVideoOperation
    {
        private ConvertVideoToFitsController m_ConvertVideoToFitsController;
        private IVideoController m_VideoController;
        private ucConvertVideoToFits m_ControlPanel = null;
        private object m_SyncRoot = new object();

        private ConvertVideoToFitsState m_Status;

        public ConvertVideoToFitsOperation()
        { }

        public ConvertVideoToFitsOperation(ConvertVideoToFitsController convertVideoToFitsController)
        {
            m_ConvertVideoToFitsController = convertVideoToFitsController;
        }

        public bool InitializeOperation(IVideoController videoController, Panel controlPanel, IFramePlayer framePlayer, Form topForm)
        {
            m_Status = ConvertVideoToFitsState.Configuring;
            m_VideoController = videoController;

            // We don't allow loading of calibration frames for now. Doing so with complicate the export
            TangraContext.Current.CanLoadFlatFrame = false;
            TangraContext.Current.CanLoadDarkFrame = false;
            TangraContext.Current.CanLoadBiasFrame = false;

            if (m_ControlPanel == null)
            {
                lock (m_SyncRoot)
                {
                    if (m_ControlPanel == null)
                    {
                        m_ControlPanel = new ucConvertVideoToFits(this, (VideoController)videoController);
                    }
                }
            }

            controlPanel.Controls.Clear();
            controlPanel.Controls.Add(m_ControlPanel);
            m_ControlPanel.Dock = DockStyle.Fill;

            return true;
        }

        public void FinalizeOperation()
        { }

        public void PlayerStarted()
        { }

        public void NextFrame(int frameNo, MovementType movementType, bool isLastFrame, AstroImage astroImage, int firstFrameInIntegrationPeriod, string fileName)
        {
            if (m_Status == ConvertVideoToFitsState.Configuring)
            {
                m_ControlPanel.UpdateStartFrame(frameNo);
            }
        }

        public void ImageToolChanged(ImageTool newTool, ImageTool oldTool)
        {
            //TODO:
        }

        public void PreDraw(Graphics g)
        {
            //TODO:
        }

        public void PostDraw(Graphics g)
        {
            //TODO:
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
