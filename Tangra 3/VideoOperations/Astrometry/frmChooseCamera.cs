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
using Tangra.Model.Video;

namespace Tangra.VideoOperations.Astrometry
{
	public partial class frmChooseCamera : Form
	{
		internal bool IsNewConfiguration;
		internal bool SolvePlateConstantsNow;

		public frmChooseCamera()
		{
			InitializeComponent();
		}

		public frmChooseCamera(int frameWidth, int frameHeight, int bitPix)
		{
			InitializeComponent();

			ucCameraSettings1.FrameWidth = frameWidth;
			ucCameraSettings1.FrameHeight = frameHeight;
			ucCameraSettings1.SetParentForm(this);
		}
	}
}
