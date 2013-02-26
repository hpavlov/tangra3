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
	public partial class frmAdvStatusPopup : Form, IAdvStatusPopupFormCustomizer
	{
		private FrameStateData m_FrameState;
		private AdvsSettings m_AdvSettings;

		public frmAdvStatusPopup(AdvsSettings advSettings)
		{
			InitializeComponent();

            if (CurrentOS.IsMac)
                this.FormBorderStyle = FormBorderStyle.None;

			m_AdvSettings = advSettings;
		}

		public void UpdateSettings(AdvsSettings advSettings)
		{
			m_AdvSettings = advSettings;
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

			if (m_AdvSettings.PopupSatellites)
				statusText.AppendLine(string.Format("Tracked Satellites: {0}", m_FrameState.NumberSatellites));
			
			if (m_AdvSettings.PopupAlmanac)
			{
				statusText.AppendLine(string.Format("Almanac Status: {0}", m_FrameState.AlmanacStatus));
				statusText.AppendLine(string.Format("Almanac Offset: {0}", m_FrameState.AlmanacOffset));
			}

			if (m_AdvSettings.PopupGPSFix)
			{
				int intStatus;
				string strStatus;
				if (int.TryParse(m_FrameState.GPSFixStatus, out intStatus))
					strStatus = AdvStatusValuesHelper.TranslateGpsFixStatus(intStatus);
				else
					strStatus = m_FrameState.GPSFixStatus;

				statusText.AppendLine(string.Format("GPS Fix: {0}", strStatus));
			}

			if (m_AdvSettings.PopupSatellites || m_AdvSettings.PopupAlmanac || m_AdvSettings.PopupGPSFix)
				statusText.AppendLine();

			if (m_AdvSettings.PopupVideoCameraFrameId)
				statusText.AppendLine(string.Format("Camera Frame #: {0}", m_FrameState.VideoCameraFrameId.ToString("###,###,###,##0")));
			if (m_AdvSettings.PopupTimestamp)
				statusText.AppendLine(string.Format("Central Exposure Time: {0}",
                        m_FrameState.HasValidTimeStamp 
                            ? m_FrameState.CentralExposureTime.ToString("dd MMM yyyy HH:mm:ss.fff")
                            : "Timestamp Not Available"));

			if (m_AdvSettings.PopupExposure)
                 statusText.AppendLine(m_FrameState.HasValidTimeStamp 
                            ? string.Format("Exposure Duration: {0} ms", m_FrameState.ExposureInMilliseconds.ToString("0"))
                            : "Exposure Duration: Unknown");

			if (m_AdvSettings.PopupTimestamp || m_AdvSettings.PopupExposure || m_AdvSettings.PopupVideoCameraFrameId)
                statusText.AppendLine();

            if (m_AdvSettings.PopupSystemTime)
				statusText.AppendLine(string.Format("System Time: {0}", m_FrameState.SystemTime.ToString("dd MMM yyyy HH:mm:ss.fff")));

			if (m_AdvSettings.PopupSystemTime)
				statusText.AppendLine();

			if (m_AdvSettings.PopupGamma)
				statusText.AppendLine(string.Format("Gamma: {0:0.000} {1}", m_FrameState.Gamma, AdvStatusValuesHelper.GetWellKnownGammaForValue(m_FrameState.Gamma)));
			if (m_AdvSettings.PopupGain)
				statusText.AppendLine(m_FrameState.IsGainKnown 
                    ? string.Format("Gain: {0:0} dB", m_FrameState.Gain)
                    : "Gain: Unknown");
			if (m_AdvSettings.PopupOffset)
				statusText.AppendLine(string.Format("Offset: {0:0.00} %", m_FrameState.Offset));

			lblStatusCombined.Text = statusText.ToString();
            this.Height = 197 /* Nominal height of the form */ - 117 /* Nominal height of the label*/ + lblStatusCombined.Height;
		    this.Width = 213 /* Nominal width of the form */- 181 /* Nominal width of the label*/+ lblStatusCombined.Width;
            btnCopy.Top = 141  /* Nominal top position of the button */ - 117 /* Nominal height of the label*/ + lblStatusCombined.Height;
            btnCopy.Left = (this.Width - btnCopy.Width) / 2;
		}

        private void btnCopy_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(lblStatusCombined.Text);
        }
	}
}
