﻿using System;
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

			if (m_AavSettings.PopupSatellites)
				statusText.AppendLine(string.Format("Tracked Satellites: {0}", m_FrameState.NumberSatellites));
			
			if (m_AavSettings.PopupAlmanac)
			{
				statusText.AppendLine(string.Format("Almanac Status: {0}", m_FrameState.AlmanacStatus));
			}

			if (m_AavSettings.PopupGPSFix)
			{
				int intStatus;
				string strStatus;
				if (int.TryParse(m_FrameState.GPSFixStatus, out intStatus))
					strStatus = AdvStatusValuesHelper.TranslateGpsFixStatus(intStatus);
				else
					strStatus = m_FrameState.GPSFixStatus;

				statusText.AppendLine(string.Format("GPS Fix: {0}", strStatus));
			}

			if (m_AavSettings.PopupSatellites || m_AavSettings.PopupAlmanac || m_AavSettings.PopupGPSFix)
				statusText.AppendLine();

			if (m_AavSettings.PopupVideoCameraFrameId)
				statusText.AppendLine(string.Format("Camera Frame #: {0}", m_FrameState.VideoCameraFrameId.ToString("###,###,###,##0")));
			if (m_AavSettings.PopupTimestamp)
				statusText.AppendLine(string.Format("Central Exposure Time: {0}",
                        m_FrameState.HasValidTimeStamp 
                            ? m_FrameState.CentralExposureTime.ToString("dd MMM yyyy HH:mm:ss.fff")
                            : "Timestamp Not Available"));

			if (m_AavSettings.PopupExposure)
                 statusText.AppendLine(m_FrameState.HasValidTimeStamp 
                            ? string.Format("Exposure Duration: {0} ms", m_FrameState.ExposureInMilliseconds.ToString("0"))
                            : "Exposure Duration: Unknown");

			if (m_AavSettings.PopupTimestamp || m_AavSettings.PopupExposure || m_AavSettings.PopupVideoCameraFrameId)
                statusText.AppendLine();

            if (m_AavSettings.PopupSystemTime)
				statusText.AppendLine(string.Format("System Time: {0}", m_FrameState.SystemTime.ToString("dd MMM yyyy HH:mm:ss.fff")));

			if (m_AavSettings.PopupSystemTime)
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