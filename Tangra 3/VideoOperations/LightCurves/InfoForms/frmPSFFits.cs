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

namespace Tangra.VideoOperations.LightCurves.InfoForms
{
	public partial class frmPSFFits : Form
	{
		private PictureBox[] m_TargetBoxes;

		private LightCurveContext m_Context;
		private LCFile m_LCFile;
		private LCMeasurement[] m_SelectedMeasurements;
		private TangraConfig.LightCurvesDisplaySettings m_DisplaySettings;

		internal frmPSFFits(LightCurveContext context, LCFile lcFile, TangraConfig.LightCurvesDisplaySettings displaySettings)
		{
			InitializeComponent();

			m_Context = context;
			m_LCFile = lcFile;
			m_DisplaySettings = displaySettings;

			picTarget1PSF.Image = new Bitmap(picTarget1PSF.Width, picTarget1PSF.Height);
			picTarget2PSF.Image = new Bitmap(picTarget2PSF.Width, picTarget2PSF.Height);
			picTarget3PSF.Image = new Bitmap(picTarget3PSF.Width, picTarget3PSF.Height);
			picTarget4PSF.Image = new Bitmap(picTarget4PSF.Width, picTarget4PSF.Height);

			m_TargetBoxes = new PictureBox[] { picTarget1PSF, picTarget2PSF, picTarget3PSF, picTarget4PSF};
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

						int x0Int = (int)Math.Round(reading.X0);
						int y0Int = (int)Math.Round(reading.Y0);

						updatedReading.PsfFit = new PSFFit(x0Int, y0Int);
						updatedReading.PsfFit.FittingMethod = PSFFittingMethod.NonLinearFit;
						int pixelDataWidth = updatedReading.PixelData.GetLength(0);
						int pixelDataHeight = updatedReading.PixelData.GetLength(1);

						uint[,] pixelData = GetPixelData(updatedReading.PixelData);
						updatedReading.PsfFit.Fit(
							pixelData,
							m_LCFile.Footer.TrackedObjects[updatedReading.TargetNo].PsfFitMatrixSize,
							x0Int - updatedReading.PixelDataX0 + (pixelDataWidth / 2) + 1,
							y0Int - updatedReading.PixelDataY0 + (pixelDataHeight / 2) + 1,
							false);

						PlotSingleGaussian(
						m_TargetBoxes[reading.TargetNo],
						updatedReading,
						m_DisplaySettings.TargetBrushes,
						m_LCFile.Footer.TrackedObjects[updatedReading.TargetNo].ApertureInPixels,
						m_LCFile.Footer.ReductionContext.BitPix);
					}
                    else
                    {
                        Bitmap bmp = m_TargetBoxes[i].Image as Bitmap;
                        if (bmp != null)
                        {
                            using (Graphics g = Graphics.FromImage(bmp))
                            {
                                g.Clear(m_DisplaySettings.BackgroundColor);
                                g.Save();
                            }
                        }
                        m_TargetBoxes[i].Refresh();
                    }
				}
			}			
		}

		internal static void PlotSingleGaussian(
			PictureBox pictureBox,
			LCMeasurement reading,
			Brush[] allBrushes,
			double aperture,
			int bpp)
		{
			Bitmap bmp = pictureBox.Image as Bitmap;
			if (bmp != null)
			{
				Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);

				using (Graphics g = Graphics.FromImage(bmp))
				{
					reading.PsfFit.DrawInternalPoints(g, rect, (float)aperture, allBrushes[reading.TargetNo], bpp);
					g.Save();
				}
			}

			pictureBox.Refresh();
		}

		private void rbPreProcessedData_CheckedChanged(object sender, EventArgs e)
		{
		}

		private uint[,] GetPixelData(uint[,] sourcePixels)
		{
			return sourcePixels;
		}
	}
}
