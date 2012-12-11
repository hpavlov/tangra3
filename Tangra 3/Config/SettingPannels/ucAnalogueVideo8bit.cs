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
using Tangra.PInvoke;

namespace Tangra.Config.SettingPannels
{
	public partial class ucAnalogueVideo8bit : SettingsPannel
	{
		public ucAnalogueVideo8bit()
		{
			InitializeComponent();

			cbxRenderingEngineAttemptOrder.Items.Clear();
			
			string[] availableRenderingEngines = TangraVideo.EnumVideoEngines();
			cbxRenderingEngineAttemptOrder.Items.AddRange(availableRenderingEngines);
		}

		private bool m_GammaWillChange;

		public override void LoadSettings()
		{
			nudSaturation8bit.SetNUDValue(TangraConfig.Settings.Photometry.Saturation.Saturation8Bit);

			cbxRenderingEngineAttemptOrder.SetCBXIndex(TangraConfig.Settings.Generic.PreferredRenderingEngineIndex);
			if (cbxRenderingEngineAttemptOrder.SelectedIndex == -1 && cbxRenderingEngineAttemptOrder.Items.Count > 0)
				cbxRenderingEngineAttemptOrder.SelectedIndex = 0;

			cbxColourChannel.SetCBXIndex((int)TangraConfig.Settings.Photometry.ColourChannel);

			nudGamma.SetNUDValue((decimal)TangraConfig.Settings.Photometry.EncodingGamma);
			cbxGammaTheFullFrame.Checked = TangraConfig.Settings.Generic.GammaCorrectFullFrame;

			m_GammaWillChange = false;
		}

		public override void SaveSettings()
		{
			TangraConfig.Settings.Photometry.Saturation.Saturation8Bit = (byte)nudSaturation8bit.Value;
			TangraConfig.Settings.Generic.PreferredRenderingEngineIndex = cbxRenderingEngineAttemptOrder.SelectedIndex;
			TangraConfig.Settings.Photometry.ColourChannel = (TangraConfig.ColourChannel)cbxColourChannel.SelectedIndex;

			m_GammaWillChange = Math.Round(Math.Abs(TangraConfig.Settings.Photometry.EncodingGamma - (float)nudGamma.Value)) >= 0.01;

			TangraConfig.Settings.Photometry.EncodingGamma = (float)nudGamma.Value;
			TangraConfig.Settings.Generic.GammaCorrectFullFrame = cbxGammaTheFullFrame.Checked;
		}

		public override void OnPostSaveSettings()
		{
			if (m_GammaWillChange)
				NotificationManager.Instance.NotifyGammaChanged();
		}

	}
}
