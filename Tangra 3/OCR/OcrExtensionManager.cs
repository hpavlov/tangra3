/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tangra.Model.Config;

namespace Tangra.OCR
{
	public class OcrExtensionManager
	{
		public static ITimestampOcr GetCurrentOCR()
		{
			if (TangraConfig.Settings.Generic.OcrEngine != null &&
				TangraConfig.Settings.Generic.OcrEngine.StartsWith("IOTA-VTI"))
			{
                return new IotaVtiOrcManaged();
			}

			return null;
		}
	}
}
