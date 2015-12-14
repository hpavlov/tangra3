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
        private AddinsController m_AddinsController;

        public OcrExtensionManager(AddinsController addinsController)
        {
            m_AddinsController = addinsController;
        }

        public void LoadAvailableOcrEngines(ComboBox cbxOcrEngine)
        {
            var supported = new List<object>();
            supported.Add("IOTA-VTI");
            //supported.Add("KIWI-OSD");

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
                if (TangraConfig.Settings.Generic.OcrEngine.StartsWith("IOTA-VTI"))
                    return new IotaVtiOrcManaged();

                if (TangraConfig.Settings.Generic.OcrEngine.StartsWith("KIWI-OSD"))
                    return new KiwiOsdOcr();

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
