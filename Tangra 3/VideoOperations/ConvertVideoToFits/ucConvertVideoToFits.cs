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
    }
}
