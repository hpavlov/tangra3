using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tangra.VideoOperations.LightCurves;

namespace Tangra.Helpers
{
    // TODO: Implement an Observer Pattern that notifies all views that are interested!
	public class NotificationManager
	{
		public static NotificationManager Instance = new NotificationManager();

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
            throw new NotImplementedException();
		}

        public void NotifyFileProgressManagerBeginFileOperation(int maxSteps)
        {
            throw new NotImplementedException();
        }

        public void NotifyFileProgressManagerFileOperationProgress(int currentProgress)
        {
            throw new NotImplementedException();
        }

        public void NotifyFileProgressManagerEndFileOperation()
        {
            throw new NotImplementedException();
        }
	}
}
