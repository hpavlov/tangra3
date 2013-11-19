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

                //if (TangraConfig.Settings.Generic.OcrEngine == "IOTA-VTI Non TV-Safe" ||
                //    TangraConfig.Settings.Generic.OcrEngine == "IOTA-VTI TV-Safe")
                //{
                //    return new IotaVtiOrcManaged(TangraConfig.Settings.Generic.OcrEngine == "IOTA-VTI TV-Safe");	
                //}

			}

			return null;
		}
	}
}
