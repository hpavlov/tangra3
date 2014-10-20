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
using Tangra.Model.Astro;
using Tangra.Model.Config;

namespace Tangra.VideoOperations.LightCurves.InfoForms
{
	public partial class frmBackgroundHistograms : Form
	{
		private PictureBox[] m_TargetBoxes;

		private LightCurveContext m_Context;
		private LCFile m_LCFile;
		private LCMeasurement[] m_SelectedMeasurements;
		private TangraConfig.LightCurvesDisplaySettings m_DisplaySettings;

		internal frmBackgroundHistograms(LightCurveContext context, LCFile lcFile, TangraConfig.LightCurvesDisplaySettings displaySettings)
		{
			InitializeComponent();

			m_Context = context;
			m_LCFile = lcFile;
			m_DisplaySettings = displaySettings;

			picTarget1Hist.Image = new Bitmap(picTarget1Hist.Width, picTarget1Hist.Height);
			picTarget2Hist.Image = new Bitmap(picTarget2Hist.Width, picTarget2Hist.Height);
			picTarget3Hist.Image = new Bitmap(picTarget3Hist.Width, picTarget3Hist.Height);
			picTarget4Hist.Image = new Bitmap(picTarget4Hist.Width, picTarget4Hist.Height);

			m_TargetBoxes = new PictureBox[] { picTarget1Hist, picTarget2Hist, picTarget3Hist, picTarget4Hist };
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
            base.Dispose(disposing);

            m_LCFile = null;
            m_SelectedMeasurements = null;
        }

		internal void HandleNewSelectedFrame(LCMeasurement[] selectedMeasurements)
		{
			m_SelectedMeasurements = selectedMeasurements;

			if (m_SelectedMeasurements != null)
			{
				for (int i = 0; i < m_SelectedMeasurements.Length; i++)
				{
					LCMeasurement reading = m_SelectedMeasurements[i];
					if (!LCMeasurement.IsEmpty(reading) &&
					    reading.TargetNo >= 0 &&
					    reading.TargetNo <= 3)
					{
						LCMeasurement updatedReading = reading.Clone();

						int pixelDataWidth = updatedReading.PixelData.GetLength(0);
						int pixelDataHeight = updatedReading.PixelData.GetLength(1);
						
						 

						Dictionary<uint, int> histogram = new Dictionary<uint, int>();
						for (uint j = 0; j <= m_Context.MaxPixelValue; j++) histogram.Add(j, 0);

						for (int x = 0; x < pixelDataWidth; x++)
						{
							for (int y = 0; y < pixelDataHeight; y++)
							{
								histogram[updatedReading.PixelData[x, y]]++;
							}
						}

						// TODO: Plot the histogram, then the values for the PSF, Average and Background Mode on it

						int x0Int = (int)Math.Round(reading.X0);
						int y0Int = (int)Math.Round(reading.Y0);

						updatedReading.PsfFit = new PSFFit(x0Int, y0Int);
						updatedReading.PsfFit.FittingMethod = PSFFittingMethod.NonLinearFit;

						updatedReading.PsfFit.Fit(
							updatedReading.PixelData,
							m_LCFile.Footer.TrackedObjects[updatedReading.TargetNo].PsfFitMatrixSize,
							x0Int - updatedReading.PixelDataX0 + (pixelDataWidth / 2) + 1,
							y0Int - updatedReading.PixelDataY0 + (pixelDataHeight / 2) + 1,
							false);

						double psfBackground = updatedReading.PsfFit.I0;

						float aperture = m_Context.ReProcessApertures[reading.TargetNo];
						

						List<uint> allKeys = histogram.Keys.ToList();
						foreach (uint key in allKeys)
							histogram[key] = (int)Math.Round(100 * Math.Log(histogram[key] + 1));

						DrawHistogram(m_TargetBoxes[reading.TargetNo], histogram);
					}
				}
			}
		}

		private void DrawHistogram(PictureBox picHistogram, Dictionary<uint, int> data)
		{
			float maxVal = data.Values.Max();
			int XGAP = 0;
			int YGAP = 0;

			using (Graphics g = Graphics.FromImage(picHistogram.Image))
			{
				float xScale = (picHistogram.Image.Width - 2 * XGAP) * 1.0f / m_Context.MaxPixelValue;
				float yScale = (picHistogram.Image.Height - 2 * YGAP) * 1.0f / maxVal;

				g.FillRectangle(SystemBrushes.ControlDark,
								new Rectangle(0, 0, picHistogram.Image.Width, picHistogram.Image.Height));
				g.DrawRectangle(Pens.Black, XGAP, YGAP, picHistogram.Image.Width - 2 * XGAP + 1, picHistogram.Image.Height - 2 * YGAP);

				foreach (byte key in data.Keys)
				{
					float xFrom = XGAP + key * xScale + 1;
					float xSize = xScale;
					float yFrom = picHistogram.Image.Height - YGAP - data[key] * yScale;
					float ySize = data[key] * yScale;

					g.FillRectangle(Brushes.LimeGreen, xFrom, yFrom, xSize, ySize);
				}

				g.Save();
			}

			picHistogram.Refresh();			
		}
	}
}
