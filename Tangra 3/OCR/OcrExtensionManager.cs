/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Controller;
using Tangra.Model.Config;
using Tangra.SDK;

namespace Tangra.OCR
{
	public class OcrExtensionManager
	{
        public const string IOTA_VTI = "IOTA-VTI";
        public const string GPS_BOX_SPRITE = "GPS_BOX_SPRITE";
        public const string KIWI_OSD = "KIWI-OSD";

        private AddinsController m_AddinsController;

        public OcrExtensionManager(AddinsController addinsController)
        {
            m_AddinsController = addinsController;
        }

        public void LoadAvailableOcrEngines(ComboBox cbxOcrEngine)
        {
            var supported = new List<object>();
            supported.Add(IOTA_VTI);
            supported.Add(GPS_BOX_SPRITE);
            //supported.Add(KIWI_OSD);

            List<ITangraAddin> addins;
            List<ITangraAddinAction> actions = m_AddinsController.GetTimestampOcrActions(out addins);
            actions.ForEach(x => supported.Add(x.DisplayName));

            supported.Sort();           

            cbxOcrEngine.Items.Clear();
            cbxOcrEngine.Items.AddRange(supported.ToArray());

            if (!string.IsNullOrEmpty(TangraConfig.Settings.Generic.OcrEngine))
                cbxOcrEngine.SelectedIndex = cbxOcrEngine.Items.IndexOf(TangraConfig.Settings.Generic.OcrEngine);

            if (cbxOcrEngine.SelectedIndex == -1)
                cbxOcrEngine.SelectedIndex = 0;
        }

		public ITimestampOcr GetCurrentOcr()
		{
            if (TangraConfig.Settings.Generic.OcrEngine != null)
            {
                if (TangraConfig.Settings.Generic.OcrEngine.StartsWith(IOTA_VTI))
                    return new IotaVtiOrcManaged();

                if (TangraConfig.Settings.Generic.OcrEngine.StartsWith(KIWI_OSD))
                    return new KiwiOsdOcr();

                if (TangraConfig.Settings.Generic.OcrEngine.StartsWith(GPS_BOX_SPRITE))
                    return new GpsBoxSpriteOcr();

                List<ITangraAddin> addins;
                List<ITangraAddinAction> actions = m_AddinsController.GetTimestampOcrActions(out addins);
                ITangraAddinAction ocrEngine = actions.SingleOrDefault(x => TangraConfig.Settings.Generic.OcrEngine.StartsWith(x.DisplayName));
                if (ocrEngine != null)
                {
                    // TODO: Instantiate an ORC engine implemented as an Add-in
                }
            }

			return null;
		}
	}
}
