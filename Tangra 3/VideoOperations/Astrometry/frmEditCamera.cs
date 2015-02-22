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
using Tangra.Astrometry;
using Tangra.Config;
using Tangra.Model.Config;

namespace Tangra.VideoOperations.Astrometry
{
	public partial class frmEditCamera : Form
	{
		private VideoCamera m_Camera;
		private bool m_New = false;
		private string m_InitialCameraModel;

		public frmEditCamera(VideoCamera camera)
		{
			InitializeComponent();

			if (camera == null)
			{
				m_New = true;
				m_Camera = new VideoCamera();
				m_Camera.Model = "<new camera>";
				m_InitialCameraModel = null;
			}
			else
			{
				m_New = false;
				m_Camera = camera;
				m_InitialCameraModel = m_Camera.Model;
			}

			tbxCameraName.Text = m_Camera.Model;
			tbxPixelX.Text = m_Camera.CCDMetrics.CellWidth.ToString("0.0");
			tbxPixelY.Text = m_Camera.CCDMetrics.CellWidth.ToString("0.0");
			tbxMatrixWidth.Text = m_Camera.CCDMetrics.MatrixWidth.ToString();
			tbxMatrixHeight.Text = m_Camera.CCDMetrics.MatrixHeight.ToString();
			cbxUsesIntegration.Checked = m_Camera.Integrating;

			tbxCameraName.Enabled = !m_Camera.ReadOnly;
			tbxPixelX.Enabled = !m_Camera.ReadOnly;
			tbxPixelY.Enabled = !m_Camera.ReadOnly;
			tbxMatrixWidth.Enabled = !m_Camera.ReadOnly;
			tbxMatrixHeight.Enabled = !m_Camera.ReadOnly;
			cbxUsesIntegration.Enabled = !m_Camera.ReadOnly;
		}

		public VideoCamera Camera
		{
			get { return m_Camera; }
		}

		private bool ValidateControls()
		{
			if (tbxCameraName.Text.Length == 0)
			{
				MessageBox.Show(this, "Specify camera model", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

				tbxCameraName.Focus();
				tbxCameraName.SelectAll();

				return false;
			}

			if (tbxCameraName.Text != m_InitialCameraModel &&
				TangraConfig.Settings.PlateSolve.VideoCameras.Exists((c) => c.Model == tbxCameraName.Text))
			{
				MessageBox.Show(this, "This name already exists. Specify a different camera model", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				tbxCameraName.Focus();
				tbxCameraName.SelectAll();

				return false;
			}

			try
			{
				double dbl = double.Parse(tbxPixelX.Text);
				if (dbl <= 0) throw new FormatException();
			}
			catch (FormatException)
			{
				MessageBox.Show(this, "Pleaase specify a possitive number", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				tbxPixelX.Focus();
				tbxPixelX.SelectAll();

				return false;
			}

			try
			{
				double dbl = double.Parse(tbxPixelY.Text);
				if (dbl <= 0) throw new FormatException();
			}
			catch (FormatException)
			{
				MessageBox.Show(this, "Pleaase specify a possitive number", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				tbxPixelY.Focus();
				tbxPixelY.SelectAll();

				return false;
			}

			try
			{
				short dbl = short.Parse(tbxMatrixWidth.Text);
				if (dbl <= 0) throw new FormatException();
			}
			catch (FormatException)
			{
				MessageBox.Show(this, "Pleaase specify a possitive integer number", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				tbxMatrixWidth.Focus();
				tbxMatrixWidth.SelectAll();

				return false;
			}

			try
			{
				short dbl = short.Parse(tbxMatrixHeight.Text);
				if (dbl <= 0) throw new FormatException();
			}
			catch (FormatException)
			{
				MessageBox.Show(this, "Pleaase specify a possitive integer number", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				tbxMatrixHeight.Focus();
				tbxMatrixHeight.SelectAll();

				return false;
			}

			return true;
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			if (ValidateControls())
			{
				Camera.Model = tbxCameraName.Text;
				Camera.CCDMetrics.CellWidth = double.Parse(tbxPixelX.Text);
				Camera.CCDMetrics.CellHeight = double.Parse(tbxPixelY.Text);
				Camera.CCDMetrics.MatrixWidth = short.Parse(tbxMatrixWidth.Text);
				Camera.CCDMetrics.MatrixHeight = short.Parse(tbxMatrixHeight.Text);
				Camera.Integrating = cbxUsesIntegration.Checked;

				if (m_New)
					TangraConfig.Settings.PlateSolve.VideoCameras.Add(Camera);

				TangraConfig.Settings.Save();

				DialogResult = DialogResult.OK;
				Close();
			}
		}
	}
}
