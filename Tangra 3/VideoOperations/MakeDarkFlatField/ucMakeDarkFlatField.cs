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

namespace Tangra.VideoOperations.MakeDarkFlatField
{

    public partial class ucMakeDarkFlatField : UserControl
    {
        private MakeDarkFlatOperation m_Operation;
        private bool m_Running = false;


        public ucMakeDarkFlatField(MakeDarkFlatOperation operation)
        {
            InitializeComponent();

            m_Operation = operation;
        }

        private void ucMakeDarkFlatField_Load(object sender, EventArgs e)
        {
            UpdateState();
        }

        private void btnProcess_Click(object sender, EventArgs e)
        {
			if (m_Running)
				m_Operation.CancelProducingFrame();
			else
			{
				if (m_Operation.CanStartProducingFrame(rbFlat.Checked, (int)nudNumFrames.Value))
					m_Operation.StartProducingFrame(rbDark.Checked, (int) nudNumFrames.Value);
			}
        }

        public void SetRunning(int maxFrames)
        {
            pbar.Maximum = maxFrames;
			pbar.Value = 0;
            pbar.Visible = true;

            pnlSettings.Enabled = false;
            m_Running = true;
            btnProcess.Text = "Cancel";
        }

        public void SetProgress(int numFrames)
        {
			pbar.Value = Math.Max(0, Math.Min(pbar.Maximum, numFrames));
            pbar.Refresh();
        }

        public void SetStopped()
        {
            pbar.Visible = false;
            pnlSettings.Enabled = true;

            m_Running = false;
            btnProcess.Text = "Produce Averaged Frame";
        }


        private void FrameTypeChanged(object sender, EventArgs e)
        {
            UpdateState();
        }

        private void UpdateState()
        {
            pnlExposure.Visible = !rbBias.Checked;

            FrameType frameType = FrameType.Bias;
            if (rbFlat.Checked) frameType = FrameType.Flat;
            else if (rbDark.Checked) frameType = FrameType.Dark;

            m_Operation.SelectedFrameTypeChanged(frameType);          
        }

    }
}
