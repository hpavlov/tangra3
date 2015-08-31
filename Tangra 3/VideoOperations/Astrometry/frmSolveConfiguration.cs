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
using Tangra.Config;
using Tangra.Controller;
using Tangra.Model.Astro;
using Tangra.Model.Config;
using Tangra.Model.Helpers;
using Tangra.Model.VideoOperations;
using Tangra.VideoOperations.Astrometry.Engine;

namespace Tangra.VideoOperations.Astrometry
{
	public partial class frmSolveConfiguration : Form
	{
		internal FieldSolveContext Context;

		private IAstrometryController m_AstrometryController;
		private VideoController m_VideoController;


		public frmSolveConfiguration()
		{
			InitializeComponent();
		}

		public frmSolveConfiguration(IAstrometryController astrometryController, VideoController videoController)
		{
			InitializeComponent();

			m_AstrometryController = astrometryController;
			m_VideoController = videoController;

			if (TangraConfig.Settings.PlateSolve.SelectedScopeRecorderConfig != null)
				tbxLimitMag.Text = TangraConfig.Settings.PlateSolve.SelectedScopeRecorderConfig.LimitingMagnitudes[TangraConfig.Settings.PlateSolve.SelectedCameraModel].ToString("0.0");


			if (!string.IsNullOrEmpty(TangraConfig.Settings.LastUsed.AstrRAHours) &&
				!string.IsNullOrEmpty(TangraConfig.Settings.LastUsed.AstrDEDeg))
			{
				cbxRA.Text = TangraConfig.Settings.LastUsed.AstrRAHours;
				cbxDE.Text = TangraConfig.Settings.LastUsed.AstrDEDeg;
			}

			dtpEpoch.Value = TangraConfig.Settings.LastUsed.ObservationEpoch;
			tbxFocalLength.Text = TangraConfig.Settings.LastUsed.FocalLength.ToString("0");

			DateTime? timeStamp = m_VideoController.GetCurrentFrameTime();
			if (timeStamp != null && timeStamp != DateTime.MinValue)
				dtpEpoch.Value = timeStamp.Value;
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			double raHours, deDeg, /*errDeg,*/ foclen, limmag;

			try
			{
				raHours = AstroConvert.ToRightAcsension(cbxRA.Text);
			}
			catch
			{
				MessageBox.Show(this, "Enter a valid RA value", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				cbxRA.Focus();
				cbxRA.Select();
				return;
			}

			try
			{
				deDeg = AstroConvert.ToDeclination(cbxDE.Text);
			}
			catch
			{
				MessageBox.Show(this, "Enter a valid DE value", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				cbxDE.Focus();
				cbxDE.Select();
				return;
			}

			try
			{
				foclen = double.Parse(tbxFocalLength.Text);
				if (foclen < 100 || foclen > 100000) throw new FormatException();
			}
			catch
			{
				MessageBox.Show(this, "Enter a valid positive floating point value", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				tbxFocalLength.Focus();
				tbxFocalLength.Select();
				return;
			}

			try
			{
				limmag = double.Parse(tbxLimitMag.Text);
				if (limmag < 5 || limmag > 20) throw new FormatException();
			}
			catch
			{
				MessageBox.Show(this, "Enter a valid magnitude int the range ot 5 to 20", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				tbxLimitMag.Focus();
				tbxLimitMag.Select();
				return;
			}


			AstroPlate image = m_AstrometryController.GetCurrentAstroPlate();
			image.EffectiveFocalLength = foclen;

			Context = new FieldSolveContext();

			Context.RADeg = raHours * 15;
			Context.DEDeg = deDeg;
			Context.FocalLength = foclen;
			Context.LimitMagn = limmag;
			Context.Epoch = dtpEpoch.Value.Year + (dtpEpoch.Value.Month / 12.0);
			Context.ErrFoVs = 2.5;
			Context.DataBaseServer = null;
			Context.DataBaseName = null;
			Context.Method = RecognitionMethod.KnownCenter;

			cbxRA.Persist();
			cbxDE.Persist();

			TangraConfig.Settings.LastUsed.FocalLength = foclen;
			TangraConfig.Settings.LastUsed.AstrRAHours = cbxRA.Text;
			TangraConfig.Settings.LastUsed.AstrDEDeg = cbxDE.Text;
			TangraConfig.Settings.LastUsed.ObservationEpoch = dtpEpoch.Value;
			TangraConfig.Settings.Save();

			DialogResult = DialogResult.OK;
			Close();
		}
	}
}
