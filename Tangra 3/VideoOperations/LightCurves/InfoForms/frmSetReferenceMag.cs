using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Controller;
using Tangra.Model.Config;

namespace Tangra.VideoOperations.LightCurves.InfoForms
{
	public partial class frmSetReferenceMag : Form
	{
		private Color[] m_AllTargetColors;
		private double[] m_Intensities = new double[4];

		private LightCurveContext m_Context;
		private LCMeasurement[] m_Measurements;

		public frmSetReferenceMag()
		{
			InitializeComponent();
		}

		internal void SetCurrentMeasurements(LCMeasurement[] measurements, LightCurveContext context)
		{
			m_Context = context;

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
				// If there is current selection then use the first reading from the LC file
				m_Measurements = new LCMeasurement[context.ObjectCount];
				m_Measurements[0] = context.AllReadings[0][0];
				if (m_Measurements.Length > 1) m_Measurements[1] = context.AllReadings[1][0];
				if (m_Measurements.Length > 2) m_Measurements[2] = context.AllReadings[2][0];
				if (m_Measurements.Length > 3) m_Measurements[3] = context.AllReadings[3][0];
			}
			else
				m_Measurements = measurements;

			m_Intensities = new double[m_Measurements.Length];

			if (m_Measurements.Length > 0)
			{
				pnl1.Visible = true;
				m_Intensities[0] = m_Measurements[0].AdjustedReading;
				label1.Text = m_Intensities[0].ToString("0.0");
			}

			if (m_Measurements.Length > 1)
			{
				pnl2.Visible = true;
				m_Intensities[1] = m_Measurements[1].AdjustedReading;
				label2.Text = m_Intensities[1].ToString("0.0");
			}

			if (m_Measurements.Length > 2)
			{
				pnl3.Visible = true;
				m_Intensities[2] = m_Measurements[2].AdjustedReading;
				label3.Text = m_Intensities[2].ToString("0.0");
			}

			if (m_Measurements.Length > 3)
			{
				pnl4.Visible = true;
				m_Intensities[3] = m_Measurements[3].AdjustedReading;
				label4.Text = m_Intensities[3].ToString("0.0");
			}
		}

	}
}
