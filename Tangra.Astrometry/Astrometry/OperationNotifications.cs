/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tangra.Astrometry
{
	public enum NotificationType
	{
		SearchStarted,
		SearchProgressed,
		SearchFinished,
		AbortSearch,
		SearchTakingLonger,
		ConfigSolveStarted,
		ConfigSolveFinished,
		ConfigSolveContinueStartup,
		NewAstrometricFit,
		NewSelectedObject,
		NewAstrometricMeasurement,
		EndOfMeasurementLastFrame,
		AbortCurrentOprtation
	}

	public class OperationNotifications
	{
		public NotificationType Notification;
		public object Argument;
		public object Argument2;

		public OperationNotifications(NotificationType type, object arg, object arg2 = null)
		{
			Notification = type;
			Argument = arg;
			Argument2 = arg2;
		}
	}
}
