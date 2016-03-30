/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

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
		private bool m_CameraResponseReverseWillChange;

		public override void LoadSettings()
		{
			nudGamma.SetNUDValue((decimal)TangraConfig.Settings.Photometry.RememberedEncodingGammaNotForDirectUse);
			cbxGammaTheFullFrame.Checked = TangraConfig.Settings.Generic.ReverseGammaCorrection;
	
			cbxKnownResponse.SetCBXIndex((int)TangraConfig.Settings.Photometry.KnownCameraResponse);
			cbxCameraResponseFullFrame.Checked = TangraConfig.Settings.Generic.ReverseCameraResponse;
            DeserializeCameraResponseParameters(TangraConfig.Settings.Photometry.KnownCameraResponseParams);

			m_GammaWillChange = false;
			m_CameraResponseReverseWillChange = false;

			UpdateControlState();
		}

	    public override bool ValidateSettings()
	    {
	        if (cbxCameraResponseFullFrame.Checked && cbxKnownResponse.SelectedIndex < 1)
	        {
                cbxKnownResponse.Focus();

                MessageBox.Show(
                    this,
                    "Please select a camera response model",
                    "Tangra",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return false;
	        }

            return true;
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

			if (cbxCameraResponseFullFrame.Checked)
			{
				m_CameraResponseReverseWillChange = (int)TangraConfig.Settings.Photometry.KnownCameraResponse != cbxKnownResponse.SelectedIndex;
				TangraConfig.Settings.Generic.ReverseCameraResponse = true;
				TangraConfig.Settings.Photometry.KnownCameraResponse = (TangraConfig.KnownCameraResponse)cbxKnownResponse.SelectedIndex;
			    TangraConfig.Settings.Photometry.KnownCameraResponseParams = SerializeCameraResponseParameters();
			}
			else
			{
				m_CameraResponseReverseWillChange = false;
				TangraConfig.Settings.Generic.ReverseCameraResponse = false;
                TangraConfig.Settings.Photometry.KnownCameraResponse = TangraConfig.KnownCameraResponse.Undefined;
			}
		}

		public override void OnPostSaveSettings()
		{
			if (m_GammaWillChange)
				NotificationManager.Instance.NotifyGammaChanged();

			if (m_CameraResponseReverseWillChange)
				NotificationManager.Instance.CameraResponseReverseChanged();
		}

		private void cbxGammaTheFullFrame_CheckedChanged(object sender, EventArgs e)
		{
			UpdateControlState();
		}

		private void UpdateControlState()
		{
			pnlEnterGammaValue.Enabled = cbxGammaTheFullFrame.Checked;
			pnlChooseKnownResponse.Enabled = cbxCameraResponseFullFrame.Checked;
            pnlWAT910Response.Visible = cbxKnownResponse.SelectedIndex == 1 && cbxCameraResponseFullFrame.Checked;
		}

		public override void Reset()
		{		
			NotificationManager.Instance.NotifyGammaChanged();
			NotificationManager.Instance.CameraResponseReverseChanged();
		}

		private void cbxCameraResponseFullFrame_CheckedChanged(object sender, EventArgs e)
		{
			UpdateControlState();
		}

        private void cbxKnownResponse_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateControlState();
        }

	    private int[] SerializeCameraResponseParameters()
	    {
            // At the moment we only support the WAT-910BD dual knee model
	        var rv = new int[9];
	        rv[0] = (int)nud910F0.Value;
            rv[1] = (int)nud910F1.Value;
            rv[2] = (int)nud910F2.Value;
            rv[3] = (int)nud910F3.Value;
            rv[4] = (int)nud910V0.Value;
            rv[5] = (int)nud910V1.Value;
            rv[6] = (int)nud910V2.Value;
            rv[7] = (int)nud910V3.Value;
            rv[8] = (int)nud910Max.Value;
            return rv;
	    }

	    private void DeserializeCameraResponseParameters(int[] valueArray)
	    {
            // At the moment we only support the WAT-910BD dual knee model
	        if (valueArray != null && valueArray.Length == 9)
	        {
	            nud910F0.SetNUDValue(valueArray[0]);
                nud910F1.SetNUDValue(valueArray[1]);
                nud910F2.SetNUDValue(valueArray[2]);
                nud910F3.SetNUDValue(valueArray[3]);
                nud910V0.SetNUDValue(valueArray[4]);
                nud910V1.SetNUDValue(valueArray[5]);
                nud910V2.SetNUDValue(valueArray[6]);
                nud910V3.SetNUDValue(valueArray[7]);
                nud910Max.SetNUDValue(valueArray[8]);
	        }
	    }
	    
	}
}
