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
using Tangra.Controller;
using Tangra.ImageTools;
using Tangra.Model.Context;
using Tangra.Model.VideoOperations;

namespace Tangra.VideoOperations.ConvertVideoToFits
{
    public partial class ucConvertVideoToFits : UserControl
    {
        private ConvertVideoToFitsOperation m_Operation;
        private VideoController m_VideoController;

        public ucConvertVideoToFits()
        {
            InitializeComponent();
        }

        public ucConvertVideoToFits(ConvertVideoToFitsOperation operation, VideoController videoController)
            : this ()
        {
            m_Operation = operation;
            m_VideoController = videoController;

            nudFirstFrame.Minimum = videoController.VideoFirstFrame;
            nudFirstFrame.Maximum = videoController.VideoLastFrame - 1;
            nudFirstFrame.Value = nudFirstFrame.Minimum;

            nudLastFrame.Minimum = videoController.VideoFirstFrame;
            nudLastFrame.Maximum = videoController.VideoLastFrame - 1;
            nudLastFrame.Value = nudLastFrame.Maximum;
        }

        internal void UpdateStartFrame(int currFrame)
        {
            nudFirstFrame.Value = currFrame;
        }

        private void rbROI_CheckedChanged(object sender, EventArgs e)
        {
            var roiSelector = m_VideoController.CurrentImageTool as RoiSelector;
            if (roiSelector != null)
                roiSelector.Enabled = rbROI.Checked;
        }

        internal void UpdateProgress(int frameNo)
        {
            pbar.Value = Math.Max(Math.Min(frameNo, pbar.Maximum), pbar.Minimum);
            pbar.Update();
        }

        internal void ExportFinished()
        {
            pbar.Value = pbar.Maximum;
            pbar.Update();

            btnCancel.Enabled = false;
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            var roiSelector = m_VideoController.CurrentImageTool as RoiSelector;
            Rectangle rect = roiSelector != null ? roiSelector.SelectedROI : new Rectangle(0, 0, TangraContext.Current.FrameWidth, TangraContext.Current.FrameHeight);

            if (rect.Width < 1 || rect.Height < 1)
            {
                m_VideoController.ShowMessageBox("Error in RIO size.", "Tangra", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (folderBrowserDialog.ShowDialog(this) == DialogResult.OK)
            {
                btnExport.Enabled = false;
                if (roiSelector != null) roiSelector.Enabled = false;
                gbxFormat.Enabled = false;
                gbxFrameSize.Enabled = false;
                gbxSection.Enabled = false;
                pbar.Minimum = (int) nudFirstFrame.Value;
                pbar.Maximum = (int) nudLastFrame.Value;
                pbar.Visible = true;

                m_Operation.StartExport(
                    folderBrowserDialog.SelectedPath, 
                    rbCube.Checked,
                    (int)nudFirstFrame.Value, 
                    (int)nudLastFrame.Value,
                    rect);                
            }
        }
    }
}
