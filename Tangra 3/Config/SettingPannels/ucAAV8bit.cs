using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
	public partial class ucAAV8bit : SettingsPannel
	{
		private IAavStatusPopupFormCustomizer m_AavPopupCustomizer;

		public ucAAV8bit()
		{
			InitializeComponent();

			cbxADVEngine.SelectedIndex = 0;
		}

		public void SetAdvStatusPopupFormCustomizer(IAavStatusPopupFormCustomizer aavPopupCustomizer)
		{
			m_AavPopupCustomizer = aavPopupCustomizer;			
		}

		public override void LoadSettings()
		{
			nudSaturation8bit.SetNUDValue((int)TangraConfig.Settings.Photometry.Saturation.Saturation8Bit);

			cbxAdvsOsdTimeStamp.Checked = TangraConfig.Settings.AAV.OverlayTimestamp;
			cbxAdvsOsdMessages.Checked = TangraConfig.Settings.AAV.OverlayAllMessages;
            cbxAdvsOsdCameraInfo.Checked = TangraConfig.Settings.AAV.OverlayCameraInfo;
            cbxAdvsOsdSystemInfo.Checked = TangraConfig.Settings.AAV.OverlayAdvsInfo;

			cbxAdvsPopupTimeStamp.Checked = TangraConfig.Settings.AAV.PopupTimestamp;
            cbxAdvsPopupExposure.Checked = TangraConfig.Settings.AAV.PopupExposure;
			cbxAdvsPopupSystemTime.Checked = TangraConfig.Settings.AAV.PopupSystemTime;
			cbxAdvsPopupSatellites.Checked = TangraConfig.Settings.AAV.PopupSatellites;
			cbxAdvsPopupGPSFix.Checked = TangraConfig.Settings.AAV.PopupGPSFix;
			cbxAdvsPopupAlmanac.Checked = TangraConfig.Settings.AAV.PopupAlmanac;

			cbxAavSplitFieldsOSD.Checked = TangraConfig.Settings.AAV.SplitFieldsOSD;
			cbxSplitOSDParity.SelectedIndex = TangraConfig.Settings.AAV.SplitFieldsOSDParity % 2;
		}

        public override void SaveSettings()
		{			
        	TangraConfig.Settings.AAV.OverlayTimestamp = cbxAdvsOsdTimeStamp.Checked;
			TangraConfig.Settings.AAV.OverlayAllMessages = cbxAdvsOsdMessages.Checked;
            TangraConfig.Settings.AAV.OverlayCameraInfo = cbxAdvsOsdCameraInfo.Checked;
            TangraConfig.Settings.AAV.OverlayAdvsInfo = cbxAdvsOsdSystemInfo.Checked;

			TangraConfig.Settings.AAV.PopupTimestamp = cbxAdvsPopupTimeStamp.Checked;
            TangraConfig.Settings.AAV.PopupExposure = cbxAdvsPopupExposure.Checked;
            TangraConfig.Settings.AAV.PopupSystemTime = cbxAdvsPopupSystemTime.Checked;
			TangraConfig.Settings.AAV.PopupSatellites = cbxAdvsPopupSatellites.Checked;
			TangraConfig.Settings.AAV.PopupGPSFix = cbxAdvsPopupGPSFix.Checked;
			TangraConfig.Settings.AAV.PopupAlmanac = cbxAdvsPopupAlmanac.Checked;

			TangraConfig.Settings.AAV.SplitFieldsOSD = cbxAavSplitFieldsOSD.Checked;
	        TangraConfig.Settings.AAV.SplitFieldsOSDParity = cbxSplitOSDParity.SelectedIndex;

			if (m_AavPopupCustomizer != null)
			{
				m_AavPopupCustomizer.UpdateSettings(TangraConfig.Settings.AAV);
				m_AavPopupCustomizer.RefreshState();
			}
		}

		private AavSettings BuildCurrentSettings()
		{
			var rv = new AavSettings()
			{
				PopupTimestamp = cbxAdvsPopupTimeStamp.Checked,
				PopupExposure = cbxAdvsPopupExposure.Checked,
				PopupSystemTime = cbxAdvsPopupSystemTime.Checked,
				PopupSatellites = cbxAdvsPopupSatellites.Checked,
				PopupGPSFix = cbxAdvsPopupGPSFix.Checked,
				PopupAlmanac = cbxAdvsPopupAlmanac.Checked,
				SplitFieldsOSD = cbxAavSplitFieldsOSD.Checked,
			};

			return rv;
		}

		private void OnAdvPopupSettingChanged(object sender, EventArgs e)
		{
			UpdateAdvPopupCustomizer();
		}

		public override void Reset()
		{
			UpdateAdvPopupCustomizer();
		}

		private void UpdateAdvPopupCustomizer()
		{
			if (m_AavPopupCustomizer != null)
			{
				m_AavPopupCustomizer.UpdateSettings(BuildCurrentSettings());
				m_AavPopupCustomizer.RefreshState();
			}			
		}

		private void linkLabelAAV_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start("http://www.hristopavlov.net/OccuRec");
		}
	}
}