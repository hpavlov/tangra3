using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Controller;
using Tangra.Model.Config;

namespace Tangra.VideoOperations.LightCurves.AdjustApertures
{
	internal class AdjustAperturesController
	{
		internal TangraConfig.LightCurvesDisplaySettings DisplaySettings;
		private LCStateMachine m_StateMachine;
		private VideoController m_VideoController;

		internal void AdjustApertures(Form parentForm, TangraConfig.LightCurvesDisplaySettings displaySettings, LCStateMachine stateMachine, VideoController videoController)
		{
			DisplaySettings = displaySettings;
			m_StateMachine = stateMachine;
			m_VideoController = videoController;

			var frm = new frmAdjustApertures();

			frm.Controller = this;
			frm.Model = new AdjustAperturesViewModel(m_StateMachine.MeasuringApertures, m_StateMachine.MeasuringStars);
			
			frm.StartPosition = FormStartPosition.CenterParent;
			frm.ShowDialog(parentForm);
		}

		internal uint[,] GetPixels(int xCenter, int yCenter)
		{
			return m_VideoController.GetCurrentAstroImage(false).GetMeasurableAreaPixels(xCenter, yCenter, 35);
		}
	}
}
