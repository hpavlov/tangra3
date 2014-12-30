/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Helpers;
using Tangra.Model.Config;
using Tangra.Model.Image;

namespace Tangra.Video.AstroDigitalVideo
{
	public partial class frmAavStatusPopup : Form, IAavStatusPopupFormCustomizer
	{
		private FrameStateData m_FrameState;
		private AavSettings m_AavSettings;

		public frmAavStatusPopup(AavSettings aavSettings)
		{
			InitializeComponent();

            if (CurrentOS.IsMac)
                this.FormBorderStyle = FormBorderStyle.None;

			m_AavSettings = aavSettings;
		}

		public void UpdateSettings(AavSettings aavSettings)
		{
			m_AavSettings = aavSettings;
		}

		public void RefreshState()
		{
			if (!m_FrameState.IsEmpty())
				ShowStatus(m_FrameState);
		}

		public void ShowStatus(FrameStateData frameState)
		{
			m_FrameState = frameState;

			var statusText = new StringBuilder();

			if (m_AavSettings.Popup_Satellites)
				statusText.AppendLine(string.Format("Tracked Satellites: {0}", m_FrameState.NumberSatellites));
			
			if (m_AavSettings.Popup_Almanac)
			{
				statusText.AppendLine(string.Format("Almanac Status: {0}", m_FrameState.AlmanacStatus));
			}

			if (m_AavSettings.Popup_GPSFix)
			{
				int intStatus;
				string strStatus;
				if (int.TryParse(m_FrameState.GPSFixStatus, out intStatus))
					strStatus = AdvStatusValuesHelper.TranslateGpsFixStatus(intStatus);
				else
					strStatus = m_FrameState.GPSFixStatus;

				statusText.AppendLine(string.Format("GPS Fix: {0}", strStatus));
			}

			if (m_AavSettings.Popup_Satellites || m_AavSettings.Popup_Almanac || m_AavSettings.Popup_GPSFix)
				statusText.AppendLine();

			if (m_AavSettings.Popup_Timestamp)
				statusText.AppendLine(string.Format("Central Exposure Time: {0}",
                        m_FrameState.HasValidTimeStamp 
                            ? m_FrameState.CentralExposureTime.ToString("dd MMM yyyy HH:mm:ss.fff")
                            : "Timestamp Not Available"));

			if (m_AavSettings.Popup_Exposure)
                 statusText.AppendLine(m_FrameState.HasValidTimeStamp 
                            ? string.Format("Exposure Duration: {0} ms", m_FrameState.ExposureInMilliseconds.ToString("0"))
                            : "Exposure Duration: Unknown");

			if (m_AavSettings.Popup_Timestamp || m_AavSettings.Popup_Exposure)
                statusText.AppendLine();

			if (m_AavSettings.Popup_NtpTimestamp)
				statusText.AppendLine(string.Format("NTP Timestamp: {0}", m_FrameState.HasValidNtpTimeStamp ? m_FrameState.EndFrameNtpTime.ToString("dd MMM yyyy HH:mm:ss.fff") : ""));

            if (m_AavSettings.Popup_SystemTime)
				statusText.AppendLine(string.Format("PC Clock Time: {0}", m_FrameState.SystemTime.ToString("dd MMM yyyy HH:mm:ss.fff")));

			if (m_AavSettings.Popup_SystemTime)
				statusText.AppendLine();

			lblStatusCombined.Text = statusText.ToString();
            this.Height = 182 /* Nominal height of the form */ - 117 /* Nominal height of the label*/ + lblStatusCombined.Height;
		    this.Width = 213 /* Nominal width of the form */- 181 /* Nominal width of the label*/+ lblStatusCombined.Width;
            btnCopy.Top = 126  /* Nominal top position of the button */ - 117 /* Nominal height of the label*/ + lblStatusCombined.Height;
            btnCopy.Left = (this.Width - btnCopy.Width) / 2;
		}

        private void btnCopy_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(lblStatusCombined.Text);
        }
	}
}
