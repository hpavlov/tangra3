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
				m_VideoController.DisplayCursorImageCoordinates(new Point(imageTool.MouseX, imageTool.MouseY));
			}
		}

	}
}
