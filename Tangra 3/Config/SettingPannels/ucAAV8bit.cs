/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

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

			cbxAdvsOsdTimeStamp.Checked = TangraConfig.Settings.AAV.Overlay_Timestamp;
			cbxAdvsOsdMessages.Checked = TangraConfig.Settings.AAV.Overlay_AllMessages;
            cbxAdvsOsdCameraInfo.Checked = TangraConfig.Settings.AAV.Overlay_CameraInfo;
            cbxAdvsOsdSystemInfo.Checked = TangraConfig.Settings.AAV.Overlay_AdvsInfo;

			cbxAdvsPopupTimeStamp.Checked = TangraConfig.Settings.AAV.Popup_Timestamp;
            cbxAdvsPopupExposure.Checked = TangraConfig.Settings.AAV.Popup_Exposure;
			cbxAdvsPopupSystemTime.Checked = TangraConfig.Settings.AAV.Popup_SystemTime;
			cbxAdvsPopupSatellites.Checked = TangraConfig.Settings.AAV.Popup_Satellites;
			cbxAdvsPopupGPSFix.Checked = TangraConfig.Settings.AAV.Popup_GPSFix;
			cbxAdvsPopupAlmanac.Checked = TangraConfig.Settings.AAV.Popup_Almanac;
			cbxAdvsPopupGain.Checked = TangraConfig.Settings.AAV.Popup_Gain;
			cbxAdvsPopupGamma.Checked = TangraConfig.Settings.AAV.Popup_Gamma;

			cbxAavSplitFieldsOSD.Checked = TangraConfig.Settings.AAV.SplitFieldsOSD;

			cbxNtpDebugFlag.Checked = TangraConfig.Settings.AAV.NtpTimeDebugFlag;
			cbxAdvsPopupNTPTime.Checked = TangraConfig.Settings.AAV.Popup_NtpTimestamp;

			cbxNtpUsageType.SelectedIndex = TangraConfig.Settings.AAV.NtpTimeUseDirectTimestamps ? 1 : 0;
		}

        public override void SaveSettings()
		{			
        	TangraConfig.Settings.AAV.Overlay_Timestamp = cbxAdvsOsdTimeStamp.Checked;
			TangraConfig.Settings.AAV.Overlay_AllMessages = cbxAdvsOsdMessages.Checked;
            TangraConfig.Settings.AAV.Overlay_CameraInfo = cbxAdvsOsdCameraInfo.Checked;
            TangraConfig.Settings.AAV.Overlay_AdvsInfo = cbxAdvsOsdSystemInfo.Checked;

			TangraConfig.Settings.AAV.Popup_Timestamp = cbxAdvsPopupTimeStamp.Checked;
            TangraConfig.Settings.AAV.Popup_Exposure = cbxAdvsPopupExposure.Checked;
            TangraConfig.Settings.AAV.Popup_SystemTime = cbxAdvsPopupSystemTime.Checked;
			TangraConfig.Settings.AAV.Popup_Satellites = cbxAdvsPopupSatellites.Checked;
			TangraConfig.Settings.AAV.Popup_GPSFix = cbxAdvsPopupGPSFix.Checked;
			TangraConfig.Settings.AAV.Popup_Almanac = cbxAdvsPopupAlmanac.Checked;
	        TangraConfig.Settings.AAV.Popup_Gain = cbxAdvsPopupGain.Checked;
	        TangraConfig.Settings.AAV.Popup_Gamma = cbxAdvsPopupGamma.Checked;

			TangraConfig.Settings.AAV.SplitFieldsOSD = cbxAavSplitFieldsOSD.Checked;

			TangraConfig.Settings.AAV.NtpTimeDebugFlag = cbxNtpDebugFlag.Checked;
			TangraConfig.Settings.AAV.Popup_NtpTimestamp = cbxAdvsPopupNTPTime.Checked;

			TangraConfig.Settings.AAV.NtpTimeUseDirectTimestamps = cbxNtpUsageType.SelectedIndex == 1;

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
				Popup_Timestamp = cbxAdvsPopupTimeStamp.Checked,
				Popup_Exposure = cbxAdvsPopupExposure.Checked,
				Popup_SystemTime = cbxAdvsPopupSystemTime.Checked,
				Popup_NtpTimestamp = cbxAdvsPopupNTPTime.Checked,
				Popup_Satellites = cbxAdvsPopupSatellites.Checked,
				Popup_GPSFix = cbxAdvsPopupGPSFix.Checked,
				Popup_Almanac = cbxAdvsPopupAlmanac.Checked,
				Popup_Gain = cbxAdvsPopupGain.Checked,
				Popup_Gamma = cbxAdvsPopupGamma.Checked,
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
			ShellHelper.OpenUrl("http://www.hristopavlov.net/OccuRec");
		}
	}
}