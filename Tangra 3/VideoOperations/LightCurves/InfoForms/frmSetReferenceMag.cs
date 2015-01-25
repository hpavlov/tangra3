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
using Tangra.Controller;
using Tangra.Helpers;
using Tangra.Model.Config;

namespace Tangra.VideoOperations.LightCurves.InfoForms
{
	public partial class frmSetReferenceMag : Form
	{
		private Color[] m_AllTargetColors;
        private int[] m_Intensities = new int[4];
	    private int m_OccultedStarIndex;

		private LightCurveContext m_Context;

		public frmSetReferenceMag()
		{
			InitializeComponent();
		}

		internal void SetCurrentMeasurements(LCMeasurement[] measurements, LightCurveContext context, int occultedStarIndex)
		{
			m_Context = context;
		    m_OccultedStarIndex = occultedStarIndex;

			m_AllTargetColors = new Color[]
		    {
			    TangraConfig.Settings.Color.Target1,
			    TangraConfig.Settings.Color.Target2,
			    TangraConfig.Settings.Color.Target3,
			    TangraConfig.Settings.Color.Target4
		    };

			pb1.BackColor = m_AllTargetColors[0];
			pb2.BackColor = m_AllTargetColors[1];
			pb3.BackColor = m_AllTargetColors[2];
			pb4.BackColor = m_AllTargetColors[3];

			pnl1.Visible = false;
			pnl2.Visible = false;
			pnl3.Visible = false;
			pnl4.Visible = false;

			if (measurements == null || measurements.Length == 0)
			{
			    int numFramesMedian = Math.Min(context.AllReadings[1].Count, TangraConfig.Settings.Photometry.SNFrameWindow);

				// If there is current selection then use the first reading from the LC file
				m_Intensities = new int[context.ObjectCount];
                m_Intensities[0] = context.AllReadings[0].Take(numFramesMedian).Select(x => x.AdjustedReading).ToList().Median();
                if (context.ObjectCount > 1) m_Intensities[1] = context.AllReadings[1].Take(numFramesMedian).Select(x => x.AdjustedReading).ToList().Median();
                if (context.ObjectCount > 2) m_Intensities[2] = context.AllReadings[2].Take(numFramesMedian).Select(x => x.AdjustedReading).ToList().Median();
                if (context.ObjectCount > 3) m_Intensities[3] = context.AllReadings[3].Take(numFramesMedian).Select(x => x.AdjustedReading).ToList().Median();
			}
		}

		private void ReferenceStartRadioButtonChanged(object sender, EventArgs e)
		{
			RadioButton rb = sender as RadioButton;
			if (rb == null || !rb.Checked) return;

			if (rb == rb1)
			{
				nudMag1.Enabled = true;

				rb2.Checked = false;
				rb3.Checked = false;
				rb4.Checked = false;
				nudMag2.Enabled = false;
				nudMag3.Enabled = false;
				nudMag4.Enabled = false;
			}
			else if (rb == rb2)
			{
				nudMag2.Enabled = true;

				rb1.Checked = false;
				rb3.Checked = false;
				rb4.Checked = false;
				nudMag1.Enabled = false;
				nudMag3.Enabled = false;
				nudMag4.Enabled = false;
			}
			else if (rb == rb3)
			{
				nudMag3.Enabled = true;

				rb1.Checked = false;
				rb2.Checked = false;
				rb4.Checked = false;
				nudMag1.Enabled = false;
				nudMag2.Enabled = false;
				nudMag4.Enabled = false;
			}
			else if (rb == rb4)
			{
				nudMag4.Enabled = true;

				rb1.Checked = false;
				rb2.Checked = false;
				rb3.Checked = false;
				nudMag1.Enabled = false;
				nudMag2.Enabled = false;
				nudMag3.Enabled = false;
			}

			RecalculateMagnitudes();
		}

		double m_RefMag = 12.0;
		double m_RefIntensity = 1;
		int m_RefIndex = 0;

		private void RecalculateMagnitudes()
		{
			if (m_Intensities == null || m_Intensities.Length < 2) 
				return;

			if (nudMag1.Enabled)
			{
				m_RefMag = (double)nudMag1.Value;
				m_RefIntensity = m_Intensities[0];
				m_RefIndex = 1;
			}
			else if (nudMag2.Enabled)
			{
				m_RefMag = (double)nudMag2.Value;
				m_RefIntensity = m_Intensities[1];
				m_RefIndex = 2;
			}
			else if (nudMag3.Enabled)
			{
				m_RefMag = (double)nudMag3.Value;
				m_RefIntensity = m_Intensities[2];
				m_RefIndex = 3;
			}
			else if (nudMag4.Enabled)
			{
				m_RefMag = (double)nudMag4.Value;
				m_RefIntensity = m_Intensities[3];
				m_RefIndex = 4;
			}

			if (m_RefIndex != 1)
				nudMag1.Value = (decimal)(m_RefMag - 2.5 * Math.Log10(Math.Max(1, m_Intensities[0]) / m_RefIntensity));
			if (m_Intensities.Length > 1 && m_RefIndex != 2)
				nudMag2.Value = (decimal)(m_RefMag - 2.5 * Math.Log10(Math.Max(1, m_Intensities[1]) / m_RefIntensity));
			if (m_Intensities.Length > 2 && m_RefIndex != 3)
				nudMag3.Value = (decimal)(m_RefMag - 2.5 * Math.Log10(Math.Max(1, m_Intensities[2]) / m_RefIntensity));
			if (m_Intensities.Length > 3 && m_RefIndex != 4)
				nudMag4.Value = (decimal)(m_RefMag - 2.5 * Math.Log10(Math.Max(1, m_Intensities[3]) / m_RefIntensity));
		}

		private void ReferenceMagnitudeChanged(object sender, EventArgs e)
		{
			NumericUpDown nud = sender as NumericUpDown;
			if (nud == null || !nud.Enabled)
				return;

			RecalculateMagnitudes();
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			if (m_RefIndex > 0)
			{
                m_Context.MagnitudeConverter.SetReferenceMagnitude(m_RefIndex, m_RefMag, m_RefIntensity);
				DialogResult = DialogResult.OK;
				Close();
			}
        }

        private void frmSetReferenceMag_Shown(object sender, EventArgs e)
        {
            double[] referenceMags = new double[] { double.NaN, double.NaN, double.NaN, double.NaN };
            double[] calculatedMags = new double[] { double.NaN, double.NaN, double.NaN, double.NaN };

            if (m_Context.MagnitudeConverter.CanComputeMagnitudes)
            {
                referenceMags = m_Context.MagnitudeConverter.GetReferenceMagnitudes();
                calculatedMags = m_Context.MagnitudeConverter.ComputeMagnitudes(m_Intensities);
            }

            if (m_Intensities.Length > 0)
            {
                pnl1.Visible = true;
                label1.Text = m_Intensities[0].ToString("0.0");
            }

            if (m_Intensities.Length > 1)
            {
                pnl2.Visible = true;
                label2.Text = m_Intensities[1].ToString("0.0");
            }

            if (m_Intensities.Length > 2)
            {
                pnl3.Visible = true;
                label3.Text = m_Intensities[2].ToString("0.0");
            }

            if (m_Intensities.Length > 3)
            {
                pnl4.Visible = true;
                label4.Text = m_Intensities[3].ToString("0.0");
            }

            rb1.Checked = false;
            rb2.Checked = false;
            rb3.Checked = false;
            rb4.Checked = false;

            if (m_Intensities.Length > 0 && !double.IsNaN(referenceMags[0]))
            {
                nudMag1.Value = (decimal)calculatedMags[0];
            }
            if (m_Intensities.Length > 1 && !double.IsNaN(referenceMags[1]))
            {
                nudMag2.Value = (decimal)calculatedMags[1];
            }
            if (m_Intensities.Length > 2 && !double.IsNaN(referenceMags[2]))
            {
                nudMag3.Value = (decimal)calculatedMags[2];
            }
            if (m_Intensities.Length > 3 && !double.IsNaN(referenceMags[3]))
            {
                nudMag4.Value = (decimal)calculatedMags[3];
            }

            if (m_OccultedStarIndex == 0 && m_Intensities.Length > 0)
                rb2.Checked = true;
            else
                rb1.Checked = true;
        }
	}
}
