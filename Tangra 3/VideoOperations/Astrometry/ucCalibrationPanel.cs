/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Data;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Windows.Forms;
using Tangra.Astrometry;
using Tangra.Astrometry.Recognition;
using Tangra.Controller;
using Tangra.Helpers;
using Tangra.ImageTools;
using Tangra.Model.Astro;
using Tangra.Model.Config;
using Tangra.Model.Context;
using Tangra.Model.Helpers;
using Tangra.Model.Image;
using Tangra.Model.VideoOperations;
using Tangra.VideoOperations.Astrometry.Engine;


namespace Tangra.VideoOperations.Astrometry
{
	public partial class ucCalibrationPanel : UserControl, ICalibrationRunner
	{
		private IPlateCalibrationTool m_CalibrationTool;
		private AstrometryController m_AstrometryController;
		private VideoController m_VideoController;

		private Pixelmap m_InitialPixelmap;

		internal ucCalibrationPanel(AstrometryController astrometryController, VideoController videoController, IPlateCalibrationTool configTool)
		{
			InitializeComponent();

			m_AstrometryController = astrometryController;
			m_VideoController = videoController;
			m_CalibrationTool = configTool;
			m_CalibrationTool.AreaChanged += m_CalibrationTool_AreaChanged;

			pnlDebugFits.Visible = false;

			m_AstrometryController.RegisterCalibrationRunner(this);

		    rbIdentify3Stars.Checked = true;
		    UpdateEnabledStateOfScrollControls();

			m_InitialPixelmap = m_VideoController.GetCurrentAstroImage(false).Pixelmap;
            m_VideoController.ApplyDisplayModeAdjustments(m_InitialPixelmap.DisplayBitmap, false, m_InitialPixelmap);

			m_CalibrationTool.ActivateOsdAreaSizing();

		    rbInclusion.Checked = TangraConfig.Settings.PlateSolve.SelectedScopeRecorderConfig.IsInclusionArea;
		}

		void m_CalibrationTool_AreaChanged()
		{
			TriggerStarMapRecalc();
		}


		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}

			m_AstrometryController.RegisterCalibrationRunner(null);

			base.Dispose(disposing);
		}

		private void tbRotation_ValueChanged(object sender, EventArgs e)
		{
			if (m_SendChangeNotifications)
				m_CalibrationTool.RotationChanged(tbRotation.Value);

			lblRotationValue.Text = string.Format("{0} deg", tbRotation.Value);
		}

		private void tbFocalLength_ValueChanged(object sender, EventArgs e)
		{
			if (m_SendChangeNotifications)
				m_CalibrationTool.FocalLengthChanged(tbFocalLength.Value);

			lblFocalLengthValue.Text = string.Format("{0} mm", tbFocalLength.Value);
		}

		private void tbAspect_ValueChanged(object sender, EventArgs e)
		{
			double aspect = 1;

			if (tbAspect.Value >= 0)
				aspect = 1 + tbAspect.Value / 100.0;
			else
				aspect = 1 / (1 - (tbAspect.Value / 100.0));

			if (m_SendChangeNotifications)
			{
				m_CalibrationTool.AspectChanged(aspect);
			}

			lblAspectValue.Text = aspect.ToString("0.000");
		}

		private void tbLimitMagnitude_ValueChanged(object sender, EventArgs e)
		{
			if (m_SendChangeNotifications)
				m_CalibrationTool.LimitMagnitudeChanged(tbLimitMagnitude.Value);

			bool oldVal = m_SendChangeNotifications;
			m_SendChangeNotifications = false;
			try
			{
				tbLimitMagnitude2.Value = tbLimitMagnitude.Value;
			}
			finally
			{
				m_SendChangeNotifications = oldVal;
			}			

			lblLimitMagnitude.Text = string.Format("{0} m", tbLimitMagnitude.Value);
			lblLimitMagnitude2.Text = string.Format("{0} m", tbLimitMagnitude.Value);
		}

		private void tbLimitMagnitude2_ValueChanged(object sender, EventArgs e)
		{
			if (m_SendChangeNotifications)
				m_CalibrationTool.LimitMagnitudeChanged(tbLimitMagnitude2.Value);

			bool oldVal = m_SendChangeNotifications;
			m_SendChangeNotifications = false;
			try
			{
				tbLimitMagnitude.Value = tbLimitMagnitude2.Value;
			}
			finally
			{
				m_SendChangeNotifications = oldVal;
			}
			
			lblLimitMagnitude.Text = string.Format("{0} m", tbLimitMagnitude.Value);
			lblLimitMagnitude2.Text = string.Format("{0} m", tbLimitMagnitude.Value);
		}

		private void cbxShowLabels2_CheckedChanged(object sender, EventArgs e)
		{
			if (m_SendChangeNotifications)
				m_CalibrationTool.ShowLabels(cbxShowLabels2.Checked);
		}

		private void cbxShowGrid_CheckedChanged(object sender, EventArgs e)
		{
			if (m_SendChangeNotifications)
				m_CalibrationTool.ShowGrid(cbxShowGrid.Checked);

			bool oldVal = m_SendChangeNotifications;
			m_SendChangeNotifications = false;
			try
			{
				cbxShowGrid2.Checked = cbxShowGrid.Checked;
			}
			finally
			{
				m_SendChangeNotifications = oldVal;
			}
		}


		private void cbxShowGrid2_CheckedChanged(object sender, EventArgs e)
		{
			if (m_SendChangeNotifications)
				m_CalibrationTool.ShowGrid(cbxShowGrid2.Checked);

			bool oldVal = m_SendChangeNotifications;
			m_SendChangeNotifications = false;
			try
			{
				cbxShowGrid.Checked = cbxShowGrid2.Checked;
			}
			finally
			{
				m_SendChangeNotifications = oldVal;
			}
		}

		internal void OnSuccessfulCalibration(CalibrationContext context)
		{
			UpdateState(3);

			if (context.ImprovedDistanceBasedFit != null)
			{
				double onePixX = context.ImprovedDistanceBasedFit.GetDistanceInArcSec(context.PlateConfig.CenterXImage, context.PlateConfig.CenterYImage,
				                                                     context.PlateConfig.CenterXImage + 1, context.PlateConfig.CenterYImage);
				double onePixY = context.ImprovedDistanceBasedFit.GetDistanceInArcSec(context.PlateConfig.CenterXImage, context.PlateConfig.CenterYImage,
																	 context.PlateConfig.CenterXImage, context.PlateConfig.CenterYImage + 1);

				lblPixSize.Text = string.Format("1 pix: (X) = {0}\"  Y = {1}\"", onePixX.ToString("0.00"), onePixY.ToString("0.00"));

				lblStdDevRa.Text = string.Format("Std.Dev.RA = {0}\"", context.ImprovedDistanceBasedFit.m_StdDevRAArcSec.ToString("0.00"));
				lblStdDevDe.Text = string.Format("Std.Dev.DE = {0}\"", context.ImprovedDistanceBasedFit.m_StdDevDEArcSec.ToString("0.00"));
			}
		}

	    private byte[] m_PlateToReport;

        internal void OnUnsuccessfulCalibration(CalibrationContext context, PlateCalibration plateCalibration)
        {
            m_PlateToReport = null;
            try
            {
                string tempFilename = Path.GetTempFileName();
                plateCalibration.SaveContext(tempFilename);
                string zippedFileName = Path.GetFullPath(tempFilename + ".zip");
                ZipUnzip.Zip(tempFilename, zippedFileName);
                m_PlateToReport = File.ReadAllBytes(zippedFileName);
                File.Delete(tempFilename);
                File.Delete(zippedFileName);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }
            
            if (m_PlateToReport != null)
            {
                double mbSize = m_PlateToReport.Length  * 1.0 / (1024 * 1024);
                btnSendProblemFit.Text = string.Format("Report Unsolved Plate ({0} Mb)", mbSize.ToString("0.00"));
                btnSendProblemFit.Visible = true;
            }
        }

		private PSFFit m_SelectedPSF;

		internal void SelectStar(PlateConstStarPair selectedPair, PSFFit psfFit)
		{
			if (selectedPair != null)
			{
				lblStarNo.Text = selectedPair.StarNo.ToString();
				lblRA.Text = AstroConvert.ToStringValue(selectedPair.RADeg / 15.0, "HH MM SS.TT");
				lblDE.Text = AstroConvert.ToStringValue(selectedPair.DEDeg, "+DD MM SS.T");
				lblX.Text = selectedPair.x.ToString("0.00");
				lblY.Text = selectedPair.y.ToString("0.00");
				lblResRA.Text = string.Format("{0}\"", selectedPair.FitInfo.ResidualRAArcSec.ToString("0.00"));
				lblResDE.Text = string.Format("{0}\"", selectedPair.FitInfo.ResidualDEArcSec.ToString("0.00"));

				m_SelectedPSF = psfFit;
			}
			else
			{
				m_SelectedPSF = null;
			}

			pnlSelectedStar.Visible = selectedPair != null;
		}

		internal void OnCalibrationFinished()
		{
			pnlDebugFits.Visible = true;
		}

		private bool m_SendChangeNotifications = true;

		public void SetupControls()
		{
			tbFocalLength.Minimum = tbFocalLength.Value / 2;
			tbFocalLength.Maximum = 3 * tbFocalLength.Value / 2;
			tbLimitMagnitude.Maximum = tbLimitMagnitude.Value;
			tbLimitMagnitude2.Maximum = tbLimitMagnitude.Value;
		}

		public void SetManualStarIdentificationMode(bool manualIdentification)
		{
			pnlSolve.Enabled = !manualIdentification;

            if (manualIdentification)
            {
		        pnlToleranceControl.Visible = true;
                pnlToleranceControl.Enabled = true;
		        pnlToleranceControl.BringToFront();                
            }
            else
                pnlToleranceControl.Visible = false;
		}

		public void UpdateAspect(double aspect)
		{
			m_SendChangeNotifications = false;

			try
			{
				lblAspectValue.Text = aspect.ToString("0.000");

			    int aspectValue = 1;
				if (aspect > 1)
                    aspectValue = (int)((aspect - 1.0) * 100);
				else
					aspectValue = (int)((1.0 - 1.0 / aspect) * 100);

                if (aspectValue >= tbAspect.Minimum &&
                    aspectValue <= tbAspect.Maximum)
                {
                    tbAspect.Value = aspectValue;
                }
			}
			finally
			{
				m_SendChangeNotifications = true;	
			}
		}

		public void UpdateFocalLength(int fl)
		{
			m_SendChangeNotifications = false;

			try
			{
				lblFocalLengthValue.Text = fl.ToString("0.0");
				if (fl > tbFocalLength.Maximum) tbFocalLength.Maximum = fl;
                if (fl < tbFocalLength.Minimum) tbFocalLength.Minimum = fl;
				tbFocalLength.Value = fl;
			}
			finally
			{
				m_SendChangeNotifications = true;
			}
		}

		public void UpdateRotation(int rot)
		{
			m_SendChangeNotifications = false;

			try
			{
                while (rot > 180) rot -= 360;
                while (rot < -180) rot += 360;

				lblRotationValue.Text = rot.ToString("0.0");
				tbRotation.Value = rot;
			}
			finally
			{
				m_SendChangeNotifications = true;
			}
		}

		public void UpdateLimitMagnitude(int mag)
		{
			m_SendChangeNotifications = false;

			try
			{
				lblLimitMagnitude.Text = mag.ToString("0");
				lblLimitMagnitude2.Text = mag.ToString("0");
				if (mag > tbLimitMagnitude.Maximum)
				{
					tbLimitMagnitude.Maximum = mag;
					tbLimitMagnitude2.Maximum = mag;
				}

				tbLimitMagnitude.Value = mag;
				tbLimitMagnitude2.Value = tbLimitMagnitude.Value;
			}
			finally
			{
				m_SendChangeNotifications = true;
			}
		}

		public void UpdateShowLabels(bool showLabels)
		{
			m_SendChangeNotifications = false;

			try
			{
				cbxShowLabels2.Checked = showLabels;

			}
			finally
			{
				m_SendChangeNotifications = true;
			}
		}

		public void UpdateShowMagnitudes(bool showMagnitudes)
		{
		}

		public void UpdateShowGrid(bool showGrid)
		{
			m_SendChangeNotifications = false;

			try
			{
				cbxShowGrid.Checked = showGrid;
			}
			finally
			{
				m_SendChangeNotifications = true;
			}
		}

		private void btnSolveConfiguration_Click(object sender, EventArgs e)
		{
			frmSolveConfiguration frmSolveConfig = new frmSolveConfiguration(m_AstrometryController, m_VideoController);
			frmSolveConfig.StartPosition = FormStartPosition.CenterParent;

			if (frmSolveConfig.ShowDialog(this.ParentForm) == DialogResult.OK)
			{
				AstrometryContext.Current.FieldSolveContext = frmSolveConfig.Context;

				UpdateState(1);

				m_CalibrationTool.ActivateCalibration();
			}
		}

		internal void UpdateState(int state)
		{
			if (state == 0)
			{
				// Preparing
				pnlSolve.Hide();
				pnlFinished.Hide();
				pnlPrepare.Top = 0;
				pnlPrepare.Left = 0;
                btnSendProblemFit.Visible = false;
				pnlPrepare.Show();

			}
			else if (state == 1)
			{
				// Activated
				pnlPrepare.Hide();
				pnlFinished.Hide();
				pnlSolve.Top = 0;
				pnlSolve.Left = 0;
				pnlSolve.Show();
                btnSendProblemFit.Visible = false;
			}
			else if (state == 2)
			{
				// Fitting
                btnSendProblemFit.Visible = false;
			}
			else if (state == 3)
			{
				// Solved
				pnlPrepare.Hide();
				pnlSolve.Hide();
				pnlFinished.Top = 0;
				pnlFinished.Left = 0;
				pnlFinished.Show();
			}
			else
			{
				// Uninitialized
				pnlSolve.Hide();
				pnlFinished.Hide();
				pnlPrepare.Hide();
                btnSendProblemFit.Visible = false;
			} 
		}

		private void btnSolve_MouseClick(object sender, MouseEventArgs e)
		{
			m_CalibrationTool.Solve(Control.ModifierKeys == Keys.Shift);
		}

		private void cbxShowMagnitudes2_CheckedChanged(object sender, EventArgs e)
		{
			if (m_SendChangeNotifications)
				m_CalibrationTool.ShowMagnitudes(cbxShowMagnitudes2.Checked);
		}


		private void ChangePlottedFit(object sender, EventArgs e)
		{
			List<RadioButton> btns = new List<RadioButton>(new RadioButton[] {rbPrelim, rbFirstFit, rbSecondFit, rbDist, rbDistImpr});
			m_CalibrationTool.ChangePlottedFit(btns.IndexOf(sender as RadioButton));
		}

		private void btnShowPSF_Click(object sender, EventArgs e)
		{
			if (m_SelectedPSF != null)
				m_SelectedPSF.Show();
		}


		public void RunCalibrationWithCurrentPreliminaryFit()
		{
			if (this.ParentForm != null)
				this.ParentForm.Update();
			else
				Update();

			if (m_CalibrationTool != null)
				m_CalibrationTool.Solve(false);
		}

		private void rbDoManualFit_CheckedChanged(object sender, EventArgs e)
		{
			if (rbDoManualFit.Checked)
			{
				btnSolve.Visible = true;
				cbxShowGrid.Visible = true;

				m_CalibrationTool.SetManualFitMode();
			}
			else
			{
				btnSolve.Visible = false;
				cbxShowGrid.Visible = false;
				cbxShowGrid.Checked = false;

				m_CalibrationTool.Set3StarIdMode();
			}
		}

        private void rbIdentify3Stars_CheckedChanged(object sender, EventArgs e)
        {
            UpdateEnabledStateOfScrollControls();
        }

        private void UpdateEnabledStateOfScrollControls()
        {
            tbLimitMagnitude.Enabled = !rbIdentify3Stars.Checked;
            tbAspect.Enabled = !rbIdentify3Stars.Checked;
            tbRotation.Enabled = !rbIdentify3Stars.Checked;
            tbFocalLength.Enabled = !rbIdentify3Stars.Checked;            
        }

        private void btnSendProblemFit_Click(object sender, EventArgs e)
        {
            if (m_PlateToReport != null)
            {
                Cursor = Cursors.WaitCursor;
                try
                {
                    SendProblemFitReport(m_PlateToReport);
                }
                finally
                {
                    Cursor = Cursors.Default;
                }
                btnSendProblemFit.Visible = false;
            }
        }

        internal static void SendProblemFitReport(byte[] plateToReport)
        {
	        MessageBox.Show("This is not implementd yet");
        }

		private void rbManual_CheckedChanged(object sender, EventArgs e)
		{
			trbarDepth.Visible = rbManual.Checked;

			TriggerStarMapRecalc();
		}

		private void trbarDepth_ValueChanged(object sender, EventArgs e)
		{
			TriggerStarMapRecalc();
		}

		private void TriggerStarMapRecalc()
		{
			timer1.Enabled = false;
			timer1.Enabled = true;
		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			timer1.Enabled = false;

			StarMapInternalConfig starMapConfig = StarMapInternalConfig.Default;
			
			if (rbAuto.Checked)
			{
				starMapConfig.OptimumStarsInField = (int) TangraConfig.Settings.Astrometry.PyramidOptimumStarsToMatch;
				starMapConfig.StarMapperTolerance = 2;
			}
			else
			{
				starMapConfig.OptimumStarsInField = -1;
				starMapConfig.StarMapperTolerance = trbarDepth.Value;
			}

			var starMap = new StarMap();

			var image = new AstroImage(m_InitialPixelmap);

			starMap.FindBestMap(
				starMapConfig,
				image,
				AstrometryContext.Current.OSDRectToExclude,
				AstrometryContext.Current.RectToInclude,
				AstrometryContext.Current.LimitByInclusion);

			AstrometryContext.Current.StarMap = starMap;
			AstrometryContext.Current.StarMapConfig = starMapConfig;

			m_VideoController.RedrawCurrentFrame(false, true, false);
		}

		private void AreaTypeSelectionChanged(object sender, EventArgs e)
		{
			AreaType selectedType = rbOSDExclusion.Checked ? AreaType.OSDExclusion : AreaType.Inclusion;
			m_CalibrationTool.SetAreaType(selectedType);
			TriggerStarMapRecalc();
        }

        private void tbarTolerance_ValueChanged(object sender, EventArgs e)
        {
            m_CalibrationTool.SetTolerance(tbarTolerance.Value);
        }
	}
}
