using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tangra.Model.Config;
using Tangra.Model.Context;
using Tangra.Model.ImageTools;
using Tangra.Model.VideoOperations;

namespace Tangra.View
{
	public class ImageToolView : IImageToolView
	{
		private frmMain m_MainForm;

		public ImageToolView(frmMain mainForm)
		{
			m_MainForm = mainForm;
		}

		public void Update(ImageTool imageTool)
		{

			if (imageTool != null && 
				TangraConfig.Settings.Generic.ShowCursorPosition &&
				imageTool.MouseX >= 0 &&
				imageTool.MouseY >= 0)
			{
				m_MainForm.ssMoreInfo.Text = string.Format("X={0} Y={1}", imageTool.MouseX, imageTool.MouseY);
				m_MainForm.ssMoreInfo.Visible = true;
			}
			else
			{
				m_MainForm.ssMoreInfo.Visible = false;
			}			
		}

	}
}
