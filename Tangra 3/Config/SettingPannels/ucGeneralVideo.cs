using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Model.Config;
using Tangra.Helpers;

namespace Tangra.Config.SettingPannels
{
	public partial class ucGeneralVideo : SettingsPannel
	{
		public ucGeneralVideo()
		{
			InitializeComponent();
		}

		private bool m_GammaWillChange;

		public override void LoadSettings()
		{
			nudGamma.SetNUDValue((decimal)TangraConfig.Settings.Photometry.RememberedEncodingGammaNotForDirectUse);
			cbxGammaTheFullFrame.Checked = TangraConfig.Settings.Generic.ReverseGammaCorrection;			

			m_GammaWillChange = false;

			UpdateControlState();
		}

		public override void SaveSettings()
		{
			TangraConfig.Settings.Photometry.RememberedEncodingGammaNotForDirectUse = (float)nudGamma.Value;			

			if (cbxGammaTheFullFrame.Checked)
			{
				m_GammaWillChange = 
					!TangraConfig.Settings.Generic.ReverseGammaCorrection || 
					Math.Round(Math.Abs(TangraConfig.Settings.Photometry.EncodingGamma - TangraConfig.Settings.Photometry.RememberedEncodingGammaNotForDirectUse)) >= 0.01;

				TangraConfig.Settings.Generic.ReverseGammaCorrection = true;
				TangraConfig.Settings.Photometry.EncodingGamma = TangraConfig.Settings.Photometry.RememberedEncodingGammaNotForDirectUse;
			}
			else
			{
				m_GammaWillChange = TangraConfig.Settings.Generic.ReverseGammaCorrection;
				TangraConfig.Settings.Generic.ReverseGammaCorrection = false;
				TangraConfig.Settings.Photometry.EncodingGamma = 1;
			}
		}

		public override void OnPostSaveSettings()
		{
			if (m_GammaWillChange)
				NotificationManager.Instance.NotifyGammaChanged();
		}

		private void cbxGammaTheFullFrame_CheckedChanged(object sender, EventArgs e)
		{
			UpdateControlState();
		}

		private void UpdateControlState()
		{
			pnlEnterGammaValue.Enabled = cbxGammaTheFullFrame.Checked;			
		}

		public override void Reset()
		{		
			NotificationManager.Instance.NotifyGammaChanged();
		}
	}
}
