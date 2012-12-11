using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Config;
using Tangra.Config.SettingPannels;
using Tangra.Helpers;
using Tangra.Model.Config;

namespace Tangra.Config.SettingPannels
{
	public partial class ucADVSVideo12bit : SettingsPannel
	{
		private IAdvStatusPopupFormCustomizer m_AdvPopupCustomizer;

		public ucADVSVideo12bit()
		{
			InitializeComponent();
		}

		public void SetAdvStatusPopupFormCustomizer(IAdvStatusPopupFormCustomizer advPopupCustomizer)
		{
			m_AdvPopupCustomizer = advPopupCustomizer;
		}

		public override void LoadSettings()
		{
			nudSaturation12bit.SetNUDValue((int)TangraConfig.Settings.Photometry.Saturation.Saturation12Bit);

			cbxAdvsOsdTimeStamp.Checked = TangraConfig.Settings.ADVS.OverlayTimestamp;
			cbxAdvsOsdGamma.Checked = TangraConfig.Settings.ADVS.OverlayGamma;
			cbxAdvsOsdGain.Checked = TangraConfig.Settings.ADVS.OverlayGain;
			cbxAdvsOsdMessages.Checked = TangraConfig.Settings.ADVS.OverlayAllMessages;

			cbxAdvsPopupTimeStamp.Checked = TangraConfig.Settings.ADVS.PopupTimestamp;
            cbxAdvsPopupExposure.Checked = TangraConfig.Settings.ADVS.PopupExposure;
			cbxAdvsPopupVideoCameraFrameId.Checked = TangraConfig.Settings.ADVS.PopupVideoCameraFrameId;
			cbxAdvsPopupSystemTime.Checked = TangraConfig.Settings.ADVS.PopupSystemTime;
			cbxAdvsPopupSatellites.Checked = TangraConfig.Settings.ADVS.PopupSatellites;
			cbxAdvsPopupOffset.Checked = TangraConfig.Settings.ADVS.PopupOffset;
			cbxAdvsPopupGamma.Checked = TangraConfig.Settings.ADVS.PopupGamma;
			cbxAdvsPopupGain.Checked = TangraConfig.Settings.ADVS.PopupGain;
			cbxAdvsPopupGPSFix.Checked = TangraConfig.Settings.ADVS.PopupGPSFix;
			cbxAdvsPopupAlmanac.Checked = TangraConfig.Settings.ADVS.PopupAlmanac;
		}

        public override void SaveSettings()
		{
			TangraConfig.Settings.Photometry.Saturation.Saturation12Bit = (uint)nudSaturation12bit.Value;

        	TangraConfig.Settings.ADVS.OverlayTimestamp = cbxAdvsOsdTimeStamp.Checked;
			TangraConfig.Settings.ADVS.OverlayGamma = cbxAdvsOsdGamma.Checked;
			TangraConfig.Settings.ADVS.OverlayGain = cbxAdvsOsdGain.Checked;
			TangraConfig.Settings.ADVS.OverlayAllMessages = cbxAdvsOsdMessages.Checked;

			TangraConfig.Settings.ADVS.PopupTimestamp = cbxAdvsPopupTimeStamp.Checked;
            TangraConfig.Settings.ADVS.PopupExposure = cbxAdvsPopupExposure.Checked;
        	TangraConfig.Settings.ADVS.PopupVideoCameraFrameId = cbxAdvsPopupVideoCameraFrameId.Checked;
            TangraConfig.Settings.ADVS.PopupSystemTime = cbxAdvsPopupSystemTime.Checked;
			TangraConfig.Settings.ADVS.PopupSatellites = cbxAdvsPopupSatellites.Checked;
			TangraConfig.Settings.ADVS.PopupOffset = cbxAdvsPopupOffset.Checked;
			TangraConfig.Settings.ADVS.PopupGamma = cbxAdvsPopupGamma.Checked;
			TangraConfig.Settings.ADVS.PopupGain = cbxAdvsPopupGain.Checked;
			TangraConfig.Settings.ADVS.PopupGPSFix = cbxAdvsPopupGPSFix.Checked;
			TangraConfig.Settings.ADVS.PopupAlmanac = cbxAdvsPopupAlmanac.Checked;

			if (m_AdvPopupCustomizer != null)
			{
				m_AdvPopupCustomizer.UpdateSettings(TangraConfig.Settings.ADVS);
				m_AdvPopupCustomizer.RefreshState();
			}
		}

		private void PopUpFormConfigChanged(object sender, EventArgs e)
		{
			// TODO: Would be nice if the Status Form changes as the individual show/hide checkboxes values change
		}
	}
}