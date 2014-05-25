using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.ImageTools;
using Tangra.Model.Config;

namespace Tangra.VideoOperations.LightCurves
{
	public partial class ucCorrSelection : UserControl
	{
		private TangraConfig.LightCurvesDisplaySettings m_DisplaySettings = new TangraConfig.LightCurvesDisplaySettings();

		public ucCorrSelection()
		{
			InitializeComponent();

			CorrectTrackingTool = null;
		}

		internal CorrectTrackingTool CorrectTrackingTool { get; set; }

		internal void Reset()
		{
			rbCorrAll.Checked = true;

			m_DisplaySettings.Load();
			m_DisplaySettings.Initialize();

			if (CorrectTrackingTool != null)
			{
				rbCorr2.Visible = false;
				rbCorr3.Visible = false;
				rbCorr4.Visible = false;
				lbCorr2.Visible = false;
				lbCorr3.Visible = false;
				lbCorr4.Visible = false;
				pnlCorr2.Visible = false;
				pnlCorr3.Visible = false;
				pnlCorr4.Visible = false;

				int trackedObjects = CorrectTrackingTool.Tracker.TrackedObjects.Count;

				pnlCorr1.BackColor = m_DisplaySettings.Target1Color;

				if (trackedObjects > 1)
				{
					rbCorr2.Visible = true;
					lbCorr2.Visible = true;
					pnlCorr2.Visible = true;
					pnlCorr2.BackColor = m_DisplaySettings.Target2Color;
				}
				if (trackedObjects > 2)
				{
					rbCorr3.Visible = true;
					lbCorr3.Visible = true;
					pnlCorr3.Visible = true;
					pnlCorr3.BackColor = m_DisplaySettings.Target3Color;
				}
				if (trackedObjects > 3)
				{
					rbCorr4.Visible = true;
					lbCorr4.Visible = true;
					pnlCorr4.Visible = true;
					pnlCorr4.BackColor = m_DisplaySettings.Target4Color;
				}
			}
		}

		private void CorrModeRadioButtonChanged(object sender, EventArgs e)
		{
			if (CorrectTrackingTool != null)
			{
				if (sender == rbCorrAll)
					CorrectTrackingTool.SetCorrectionMode(CorrectTrackingTool.CorrectTrackingMode.All);
				else if (sender == rbCorr1)
					CorrectTrackingTool.SetCorrectionMode(CorrectTrackingTool.CorrectTrackingMode.Target1);
				else if (sender == rbCorr2)
					CorrectTrackingTool.SetCorrectionMode(CorrectTrackingTool.CorrectTrackingMode.Target2);
				else if (sender == rbCorr3)
					CorrectTrackingTool.SetCorrectionMode(CorrectTrackingTool.CorrectTrackingMode.Target3);
				else if (sender == rbCorr4)
					CorrectTrackingTool.SetCorrectionMode(CorrectTrackingTool.CorrectTrackingMode.Target4);
			}
		}
	}
}
