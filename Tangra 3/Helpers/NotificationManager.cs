/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tangra.Controller;
using Tangra.Model.Config;
using Tangra.PInvoke;
using Tangra.VideoOperations.LightCurves;

namespace Tangra.Helpers
{
    // TODO: Implement an Observer Pattern that notifies all views that are interested!
	public class NotificationManager
	{
		public static NotificationManager Instance = new NotificationManager();

		private VideoController m_VideoController;

		public void SetVideoController(VideoController videoController)
		{
			m_VideoController = videoController;
		}

        public void NotifyCurrentFrameChanged(int currentFrameId)
        {
            // TODO: Who wants to be notified of this?
        }

        public void NotifyUserRequestToChangeCurrentFrame(object measurements)
        {
            // TODO: This is used in the following places
            // NOTE: This should be all done via the LightCurveController
            //F:\WORK\Tangra\Tangra\VideoOperations\LightCurves\Debug\frmBackgroundHistograms.cs(66):				message.MessageId == frmMain.MSG_ID_FRAME_CHANGED)
            //F:\WORK\Tangra\Tangra\VideoOperations\LightCurves\Debug\frmPSFFits.cs(65):				message.MessageId == frmMain.MSG_ID_FRAME_CHANGED)
            //F:\WORK\Tangra\Tangra\VideoOperations\LightCurves\Debug\frmZoomedPixels.cs(289):				message.MessageId == frmMain.MSG_ID_FRAME_CHANGED)
        }

        public void NotifyLightCurveFormClosed()
        {
            // TODO: This should be all done via the LightCurveController
        }

		public void NotifyGammaChanged()
		{
			TangraCore.PreProcessors.AddGammaCorrection(TangraConfig.Settings.Photometry.EncodingGamma);
		}

		public void CameraResponseReverseChanged()
		{
			TangraCore.PreProcessors.AddCameraResponseCorrection(TangraConfig.Settings.Photometry.KnownCameraResponse);
		}

        public void NotifyFileProgressManagerBeginFileOperation(int maxSteps)
        {
	        if (m_VideoController != null)
		        m_VideoController.NotifyFileProgress(-1, maxSteps);
        }

        public void NotifyFileProgressManagerFileOperationProgress(int currentProgress)
        {
			if (m_VideoController != null)
				m_VideoController.NotifyFileProgress(currentProgress, 0);
        }

        public void NotifyFileProgressManagerEndFileOperation()
        {
			if (m_VideoController != null)
				m_VideoController.NotifyFileProgress(-1, -1);
		}

		public void NotifyBeginLongOperation()
		{
			if (m_VideoController != null)
				m_VideoController.NotifyBeginLongOperation();
		}

		public void NotifyEndLongOperation()
		{
			if (m_VideoController != null)
				m_VideoController.NotifyEndLongOperation();
		}
	}
}
