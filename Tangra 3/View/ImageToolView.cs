/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Tangra.Controller;
using Tangra.Model.Config;
using Tangra.Model.Context;
using Tangra.Model.ImageTools;
using Tangra.Model.VideoOperations;

namespace Tangra.View
{
	public class ImageToolView : IImageToolView
	{
		private frmMain m_MainForm;
		private VideoController m_VideoController;

		public ImageToolView(frmMain mainForm)
		{
			m_MainForm = mainForm;
		}

		internal void SetVideoController(VideoController videoController)
		{
			m_VideoController = videoController;
		}
		public void Update(ImageTool imageTool)
		{
			if (imageTool != null &&
				m_VideoController != null)
			{
				m_VideoController.DisplayCursorPositionDetails(new Point(imageTool.MouseX, imageTool.MouseY));
			}
		}

	}
}
