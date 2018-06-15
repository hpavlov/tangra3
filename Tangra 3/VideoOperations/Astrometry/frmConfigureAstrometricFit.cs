/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Astrometry;
using Tangra.Astrometry.Recognition;
using Tangra.AstroServices;
using Tangra.Config;
using Tangra.Controller;
using Tangra.Model.Astro;
using Tangra.Model.Config;
using Tangra.Model.Context;
using Tangra.Model.Controls;
using Tangra.Model.Helpers;
using Tangra.StarCatalogues;
using Tangra.VideoOperations.Astrometry.Engine;

namespace Tangra.VideoOperations.Astrometry
{
	public partial class frmConfigureAstrometricFit : Form
	{
		public FieldSolveContext Context = new FieldSolveContext();
		private VideoController m_VideoController;
		private AstroPlate m_Image;
        private double m_RAHours;
		private double m_DEDeg;
		private double m_MaxRefValue;
	    private List<IStar> m_CatalogueStars;

		public frmConfigureAstrometricFit(VideoController videoController, AstroPlate image)
		{
			m_VideoController = videoController;
			m_Image = image;

			InitializeComponent();

			rbKnownCenter.Checked = TangraConfig.Settings.PlateSolve.StarIDSettings.Method == 0;

#if ASTROMETRY_DEBUG
			Trace.Assert(TangraConfig.Settings.PlateSolve.SelectedScopeRecorderConfig != null);
#endif

			if (!string.IsNullOrEmpty(TangraConfig.Settings.LastUsed.AstrRAHours) &&
				!string.IsNullOrEmpty(TangraConfig.Settings.LastUsed.AstrDEDeg))
			{
				cbxRA.Text = TangraConfig.Settings.LastUsed.AstrRAHours;
				cbxDE.Text = TangraConfig.Settings.LastUsed.AstrDEDeg;
			}

			if (!double.IsNaN(TangraConfig.Settings.LastUsed.AstrErrFoVs))
				SetNUDValue(nudError, (decimal)TangraConfig.Settings.LastUsed.AstrErrFoVs);

			int lastSearchTypeIdx = TangraConfig.Settings.LastUsed.AstrSearchTypeIndex;
			switch (lastSearchTypeIdx)
			{
				case 0:
					rbKnownObject.Checked = true;
					break;

				default:
					rbKnownCenter.Checked = true;
					break;
			}

			Context.FoundObject = null;

			utcTime.OnDateTimeChanged += new EventHandler<DateTimeChangeEventArgs>(ucTime_OnDateTimeChanged);

		    if (TangraConfig.Settings.LastUsed.LastAstrometryUTCDate.Date.Year == 1)
		        TangraConfig.Settings.LastUsed.LastAstrometryUTCDate = DateTime.Now;

			DateTime? timeStamp = videoController.GetCurrentFrameTime();
		    if (timeStamp != null && timeStamp != DateTime.MinValue)
		    {
		        if (timeStamp.Value.Year == 1)
		        {
		            // OCR-ed timestamp that doesn't contain a year
		            utcTime.DateTimeUtc = TangraConfig.Settings.LastUsed.LastAstrometryUTCDate.Date.AddDays(timeStamp.Value.TimeOfDay.TotalDays);
		            lblOCRTimeWarning.Visible = true;
		        }
		        else
		            utcTime.DateTimeUtc = timeStamp.Value;
		    }
		    else
		    {
		        utcTime.DateTimeUtc = m_VideoController.GetBestGuessDateTimeForCurrentFrame(TangraConfig.Settings.LastUsed.LastAstrometryUTCDate);
		    }
		        
		    DisplayEnterTimePage();

			UpdateErrorInDeg();

			m_MaxRefValue = TangraConfig.Settings.PlateSolve.SelectedScopeRecorderConfig.LimitingMagnitudes[TangraConfig.Settings.PlateSolve.SelectedCameraModel];
			nudFaintestMag.Value = (decimal)Math.Round(m_MaxRefValue);

			pnlSelectedLimitMagnitude.Enabled = false;
			rbAutomaticLimitMagnitude.Checked = true;
		}

        private void SetNUDValue(NumericUpDown nud, decimal value)
        {
            if (value >= nud.Minimum && value <= nud.Maximum)
                nud.Value = value;
            else if (value < nud.Minimum)
                nud.Value = nud.Minimum;
            else if (value > nud.Maximum)
                nud.Value = nud.Maximum;

        }
		private void ucTime_OnDateTimeChanged(object sender, DateTimeChangeEventArgs arg)
		{
			rbKnownObject.Text = string.Format("Known object in the field (Position computed at {0})", arg.UtcDateTime.ToString("dd MMM yyyy HH:mm UTC"));
		}

		private enum ConfigPage
		{
			EnterTime,
			EnterCoords,
            SetupPyramidAlignment
		}

		private ConfigPage m_CurrentPage = ConfigPage.EnterTime;

		private void DisplayEnterTimePage()
		{
			m_CurrentPage = ConfigPage.EnterTime;

			btnCancel.Text = "Cancel";
			btnOK.Text = "Next >>";
			pnlObject.Visible = false;

			pnlTime.Visible = true;
			pnlTime.BringToFront();
		}

		private void DisplayEnterCoordsPage()
		{
			m_CurrentPage = ConfigPage.EnterCoords;

			btnCancel.Text = "<< Back";
            btnOK.Text = "OK";
			pnlTime.Visible = false;
		    tbxObsCode.Text = TangraConfig.Settings.Astrometry.MPCObservatoryCode;

			pnlObject.Visible = true;
			pnlObject.BringToFront();
		}

		private void SaveDefaults()
		{
			TangraConfig.Settings.LastUsed.AstrRAHours = cbxRA.Text;
			TangraConfig.Settings.LastUsed.AstrDEDeg = cbxDE.Text;
			TangraConfig.Settings.LastUsed.AstrErrFoVs = (double)nudError.Value;

			if (rbKnownObject.Checked)
				TangraConfig.Settings.LastUsed.AstrSearchTypeIndex = 0;
			else
				TangraConfig.Settings.LastUsed.AstrSearchTypeIndex = 1;

			TangraConfig.Settings.Save();
		}

		private void rbKnownObject_CheckedChanged(object sender, EventArgs e)
		{
			pnlKnownObject.Enabled = rbKnownObject.Checked;
			pnlKnownField.Enabled = rbKnownCenter.Checked;
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			if (m_CurrentPage == ConfigPage.EnterTime)
			{
				DialogResult = DialogResult.Cancel;
				Close();
			}
			else if (m_CurrentPage == ConfigPage.EnterCoords)
			{
				DisplayEnterTimePage();
			}
            else if (m_CurrentPage == ConfigPage.SetupPyramidAlignment)
            {
                DisplayEnterCoordsPage();
            }
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			if (m_CurrentPage == ConfigPage.EnterTime)
			{
				Context.UtcTime = utcTime.DateTimeUtc;
				Context.FrameNoOfUtcTime = m_VideoController.CurrentFrameIndex;
				TangraConfig.Settings.LastUsed.LastAstrometryUTCDate = Context.UtcTime;
				TangraConfig.Settings.Save();

				DisplayEnterCoordsPage();
			}
            else if (m_CurrentPage == ConfigPage.EnterCoords)
            {
	            if (!CheckInputAndLoadStars())
		            return;

                Context.PyramidMinMag = CorePyramidConfig.Default.DefaultMinPyramidMagnitude;
                Context.PyramidMaxMag = m_MaxRefValue;

                SaveDefaults();

                DialogResult = DialogResult.OK;
                Close();  
			}
		}

        private bool CheckInputAndLoadStars()
		{
            if (!TangraContext.Current.HasImageLoaded)
            {
                MessageBox.Show(this, "There is no loaded frame/image", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }

            m_RAHours = double.NaN;
            m_DEDeg = double.NaN;

            Context.ObsCode = tbxObsCode.Text;

			if (rbKnownCenter.Checked)
			{
				try
				{
                    m_RAHours = AstroConvert.ToRightAcsension(cbxRA.Text);
				}
				catch
				{
					MessageBox.Show(this, "Enter a valid RA value", "Validation Error", MessageBoxButtons.OK,
									MessageBoxIcon.Error);
					cbxRA.Focus();
					cbxRA.Select();
					return false;
				}

				try
				{
                    m_DEDeg = AstroConvert.ToDeclination(cbxDE.Text);
				}
				catch
				{
					MessageBox.Show(this, "Enter a valid DE value", "Validation Error", MessageBoxButtons.OK,
									MessageBoxIcon.Error);
					cbxDE.Focus();
					cbxDE.Select();
					return false;
				}

				cbxRA.Persist();
				cbxDE.Persist();
			}
			else if (rbKnownObject.Checked)
			{
				// Connect to MPC to find the object coordinates
				MPEph2.MPEphEntry position = null;
				Cursor = Cursors.WaitCursor;
			    Enabled = false;
				try
				{
                    if (!TangraConfig.Settings.HasSiteCoordinatesOrCOD && string.IsNullOrEmpty(tbxObsCode.Text))
					{
						m_VideoController.ShowTangraSettingsDialog(false, true);

						if (!TangraConfig.Settings.HasSiteCoordinatesOrCOD)
							return false;
					}


                    Refresh();

                    frmIdentifyObjects frm;

                    if (TangraConfig.Settings.Astrometry.UseMPCCode || !string.IsNullOrEmpty(tbxObsCode.Text))
                    {
                        Context.ObsCode = tbxObsCode.Text;
                        frm = new frmIdentifyObjects(cbxObject.Text, Context.UtcTime, Context.ObsCode);
                    }
                    else
                    {
                        frm = new frmIdentifyObjects(
                            cbxObject.Text, Context.UtcTime,
                            TangraConfig.Settings.Generic.Longitude,
                            TangraConfig.Settings.Generic.Latitude);
                    }


                    if (m_VideoController.ShowDialog(frm) == DialogResult.OK)
                    {
                        position = frm.Position;
                    }

				}
				finally
				{
                    Enabled = true;
					Cursor = Cursors.Default;
				}

				if (position == null)
				{
					MessageBox.Show(
						 this,
						 string.Format(string.Format("Could not retrieve the position of '{0}' from MPC. Is your internet connection active? Is the designation correct?", cbxObject.Text)),
						 "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

					cbxObject.Focus();
					cbxObject.Select();
					return false;
				}
				else
				{
                    m_RAHours = position.RAHours;
                    m_DEDeg = position.DEDeg;

					cbxObject.Persist();

					Context.FoundObject = position;
				}
			}

            Context.RADeg = m_RAHours * 15;
            Context.DEDeg = m_DEDeg;
            Context.ErrFoVs = (double)nudError.Value;

            Context.Method = RecognitionMethod.KnownCenter;

            Context.LimitMagn = (double)nudFaintestMag.Value;

            float epoch = Context.UtcTime.Year + Context.UtcTime.DayOfYear / 365.25f;


            if (TangraConfig.Settings.TraceLevels.PlateSolving.TraceInfo())
                Trace.WriteLine(string.Format("Loading stars in region ({0}, {1})",
                                              AstroConvert.ToStringValue(Context.RADeg / 15, "REC"),
                                              AstroConvert.ToStringValue(Context.DEDeg, "DEC")));

            var facade = new StarCatalogueFacade(TangraConfig.Settings.StarCatalogue);
            m_CatalogueStars = facade.GetStarsInRegion(
                Context.RADeg,
                Context.DEDeg,
                (Context.ErrFoVs + 1.0) * m_Image.GetMaxFOVInArcSec() / 3600.0,
                Context.LimitMagn,
                epoch);

            Context.CatalogueStars = m_CatalogueStars;
            Context.StarCatalogueFacade = facade;

			Context.DetermineAutoLimitMagnitude = rbAutomaticLimitMagnitude.Checked;
        	Context.AutoLimitMagnitude = double.NaN;

	        return true;
		}

		private void nudError_ValueChanged(object sender, EventArgs e)
		{
			UpdateErrorInDeg();
		}

		private void UpdateErrorInDeg()
		{
			double errDeg = ((double)nudError.Value + 1.0) * m_Image.GetMaxFOVInArcSec() / 3600.0;
			lblErrorDegrees.Text = string.Format("({0:0.00} deg)", errDeg);
		}

        private void btnCurrDateTime_Click(object sender, EventArgs e)
        {
            utcTime.DateTimeUtc = DateTime.Now.ToUniversalTime();
        }

        private void btnLastUsedDateTime_Click(object sender, EventArgs e)
        {
            utcTime.DateTimeUtc = TangraConfig.Settings.LastUsed.LastAstrometryUTCDate;
        }

		private void rbAutomaticLimitMagnitude_CheckedChanged(object sender, EventArgs e)
		{
			pnlSelectedLimitMagnitude.Enabled = !rbAutomaticLimitMagnitude.Checked;
		}
	}
}
