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
using Tangra.Controller;
using Tangra.Model.Config;
using Tangra.Model.Video;
using Tangra.VideoOperations.Astrometry.Engine;

namespace Tangra.VideoOperations.Astrometry
{
	public partial class frmSelectInitialSettings : Form
	{
		internal bool IsNewConfiguration;
		internal bool SolvePlateConstantsNow;
	    private VideoController m_VideoController;

		public frmSelectInitialSettings()
		{
			InitializeComponent();
		}

        public frmSelectInitialSettings(int frameWidth, int frameHeight, VideoController videoController, IFramePlayer framePlayer)
		{
			InitializeComponent();

            m_VideoController = videoController;
			ucCameraSettings1.FrameWidth = frameWidth;
			ucCameraSettings1.FrameHeight = frameHeight;
            ucCameraSettings1.OnUpdateEnabledDisabledState += ucCameraSettings1_OnUpdateEnabledDisabledState;

            ucStretching.Initialize(framePlayer.Video, framePlayer.CurrentFrameIndex);
            ucImageDefectSettings1.Initialize(framePlayer.Video, framePlayer.CurrentFrameIndex);

            FrameAdjustmentsPreview.Instance.ParentForm = this;
            FrameAdjustmentsPreview.Instance.FramePlayer = framePlayer;
            FrameAdjustmentsPreview.Instance.Reset(m_VideoController, framePlayer.CurrentFrameIndex);
		}

        void ucCameraSettings1_OnUpdateEnabledDisabledState(bool isConfigDefined)
        {
            btnOK.Enabled = isConfigDefined;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            ucChooseCalibratedConfiguration.ChooseConfigurationResult result = ucCameraSettings1.VerifyCameraSettings();
            if (result != null)
            {
                SolvePlateConstantsNow = result.SolvePlateConstantsNow;
                
                TangraConfig.Settings.PlateSolve.SelectedCameraModel = result.SelectedCameraModel;
                TangraConfig.Settings.PlateSolve.SelectedScopeRecorder = result.SelectedConfigName;

                if (!result.Recalibrate)
                {
                    IsNewConfiguration = result.IsNew;
                }

                TangraConfig.Settings.PlateSolve.UseLowPassForAstrometry = ucImageDefectSettings1.miLowPass.Checked;
                TangraConfig.Settings.Save();

                m_VideoController.RefreshCurrentFrame();

                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void frmSelectInitialSettings_Move(object sender, EventArgs e)
        {
            FrameAdjustmentsPreview.Instance.MoveForm(this.Right, this.Top);
        }

    }
}
