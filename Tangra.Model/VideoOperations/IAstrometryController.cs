using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Model.Astro;
using Tangra.Model.Config;

namespace Tangra.Model.VideoOperations
{
	public interface INotificationReceiver
	{
		void ReceieveMessage(object notification);
	}

	public interface IAstrometryController
	{
		void Subscribe(INotificationReceiver receiver, Type notificationType);
		void Unsubscribe(INotificationReceiver receiver);
		void SendNotification(object notification);
		void NotifyBeginLongOperation(string description);
		void NotifyEndLongOperation();
		Rectangle OSDRectToExclude { get; }

		AstroPlate GetCurrentAstroPlate();
		void RunCalibrationWithCurrentPreliminaryFit();
	}
}
