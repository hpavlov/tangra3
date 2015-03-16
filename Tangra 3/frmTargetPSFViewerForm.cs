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
using Tangra.Model.Astro;
using Tangra.Model.Config;
using Tangra.Model.Image;
using Tangra.Model.VideoOperations;
using Tangra.VideoOperations.LightCurves;

namespace Tangra
{
	public partial class frmTargetPSFViewerForm : Form
	{
		private PSFFit m_PSFFit;
		private int m_Bpp;
		private uint m_NormVal;
	    private MeasurementsHelper m_Measurer;
	    private IVideoController m_VideoController;

        private TangraConfig.BackgroundMethod m_BackgroundMethod = TangraConfig.BackgroundMethod.PSFBackground;
        private TangraConfig.PhotometryReductionMethod m_SignalMethod = TangraConfig.PhotometryReductionMethod.PsfPhotometry;

		public frmTargetPSFViewerForm()
		{
			InitializeComponent();
		}

        public frmTargetPSFViewerForm(IVideoController videoController)
        {
            InitializeComponent();

            m_VideoController = videoController;
            m_PSFFit = null;
            cbMeaMethod.SelectedIndex = 0;
        }

		public void ShowTargetPSF(PSFFit psfFit, int bpp, uint aav16Normval, uint[,] backgroundPixels)
		{
			m_PSFFit = psfFit;
			m_Bpp = bpp;
			m_NormVal = bpp == 16 ? aav16Normval : 0;

			picFIT.Image = new Bitmap(picFIT.Width, picFIT.Height);

			if (m_PSFFit != null)
			{
				using (Graphics g = Graphics.FromImage(picFIT.Image))
				{
					m_PSFFit.DrawGraph(g, new Rectangle(0, 0, picFIT.Image.Width, picFIT.Image.Height), m_Bpp, m_NormVal);
					g.Save();
				}

				int side = m_PSFFit.MatrixSize * 16;
				picPixels.Width = side;
				picPixels.Height = side;
				picPixels.Image = new Bitmap(side, side);

				lblI0.Text = m_PSFFit.I0.ToString("0.0");
				lblIMax.Text = m_PSFFit.IMax.ToString("0.0");
				lblIAmp.Text = (m_PSFFit.IMax - m_PSFFit.I0).ToString("0.0");
				lblX.Text = m_PSFFit.XCenter.ToString("0.0");
				lblY.Text = m_PSFFit.YCenter.ToString("0.0");
			    
                nudMeasuringAperture.Enabled = true;
			    SetMeasurementMethods();

                EnsureMeasurer(2);
			    MeasureCurrentPSF();
			}
			else
			{
				using (Graphics g = Graphics.FromImage(picFIT.Image))
				{
					g.Clear(Color.DimGray);
					g.Save();
				}

				picPixels.Image = new Bitmap(16 * 17, 16 * 17);
				using (Graphics g = Graphics.FromImage(picPixels.Image))
				{
					g.Clear(Color.DimGray);
					g.Save();
				}

				lblI0.Text = "N/A";
				lblIMax.Text = "N/A";
				lblIAmp.Text = "N/A";
				lblY.Text = "N/A";
				lblX.Text = "N/A";

                nudMeasuringAperture.Enabled = false;

			    tbxBg.Text = "";
                tbxSmBG.Text = "";
			}

			CalculateAndDisplayBackground(backgroundPixels);

			picFIT.Refresh();
			picPixels.Refresh();
		}

        private void SetMeasurementMethods()
        {
            if (cbMeaMethod.SelectedIndex == 0)
            {
                m_BackgroundMethod = TangraConfig.BackgroundMethod.BackgroundMedian;
                m_SignalMethod = TangraConfig.PhotometryReductionMethod. AperturePhotometry;                
            }
            else if (cbMeaMethod.SelectedIndex == 1)
            {
                m_BackgroundMethod = TangraConfig.BackgroundMethod.PSFBackground;
                m_SignalMethod = TangraConfig.PhotometryReductionMethod.PsfPhotometry;            
            }
        }

        private void MeasureCurrentPSF()
        {
            if (m_PSFFit == null) return;

            float aperture = (float) nudMeasuringAperture.Value;

			using (Graphics g = Graphics.FromImage(picPixels.Image))
			{
                m_PSFFit.DrawDataPixels(g, new Rectangle(0, 0, picPixels.Width, picPixels.Height), aperture, Pens.Lime, m_Bpp, m_NormVal);
				g.Save();
			}
            picPixels.Invalidate();

            int centerX = (int)Math.Round(m_PSFFit.XCenter);
            int centerY = (int)Math.Round(m_PSFFit.YCenter);

            uint[,] data = m_VideoController.GetCurrentAstroImage(false).GetMeasurableAreaPixels(centerX, centerY, 17);
            uint[,] bagPixels = m_VideoController.GetCurrentAstroImage(false).GetMeasurableAreaPixels(centerX, centerY, 35);

            NotMeasuredReasons rv = MeasureObject(
                new ImagePixel(m_PSFFit.XCenter, m_PSFFit.YCenter),
                data,
                bagPixels,
                m_VideoController.VideoBitPix,
                m_Measurer,
                TangraConfig.PreProcessingFilter.NoFilter,
                m_SignalMethod,
                TangraConfig.PsfQuadrature.NumericalInAperture,
                TangraConfig.Settings.Photometry.PsfFittingMethod,
                aperture,
                m_PSFFit.FWHM,
                (float)m_PSFFit.FWHM,
                new FakeIMeasuredObject(m_PSFFit),
                null,
                null,
                false);

            double reading = m_Measurer.TotalReading;
            double bgReading = m_Measurer.TotalBackground;

            if (rv == NotMeasuredReasons.MeasuredSuccessfully)
            {
                tbxSmBG.Text = (reading - bgReading).ToString("0.000");
                tbxBg.Text = bgReading.ToString("0.000");
            }
            else
            {
                Trace.WriteLine("TargetPSFViewer.NotMeasuredReasons: " + rv.ToString());
                tbxBg.Text = "ERR";
                tbxSmBG.Text = "ERR";
            }
        }

		private void CalculateAndDisplayBackground(uint[,] backgroundPixels)
		{
			if (backgroundPixels != null)
			{
				int bgWidth = backgroundPixels.GetLength(0);
				int bgHeight = backgroundPixels.GetLength(1);

				var bgPixels = new List<uint>();

				for(int x = 0; x < bgWidth; x++)
				for(int y = 0; y < bgHeight; y++)
				{
					if (m_PSFFit == null || !m_PSFFit.IsSolved ||
						ImagePixel.ComputeDistance(m_PSFFit.XCenter, x + bgWidth - m_PSFFit.MatrixSize, m_PSFFit.YCenter, y + bgHeight - m_PSFFit.MatrixSize) > 3 * m_PSFFit.FWHM)
					{
						bgPixels.Add(backgroundPixels[x, y]);
					}
				}

				bgPixels.Sort();
				double background = 0;
				if (bgPixels.Count > 1)
				{
					background = m_Bpp < 12
						? bgPixels[bgPixels.Count / 2] // for 8 bit videos Median background works better
						: bgPixels.Average(x => x); // for 12+bit videos average background works better					
				}

				double residualsSquareSum = 0;
				foreach (uint bgPixel in bgPixels)
				{
					residualsSquareSum += (background - bgPixel) * (background - bgPixel);
				}
				double noise = Math.Sqrt(residualsSquareSum / (bgPixels.Count - 1));
				
				lblBackground.Text = background.ToString("0.0");
				lblNoise.Text = noise.ToString("0.0");

				if (m_PSFFit != null && noise != 0)
				{
					double snr = (m_PSFFit.IMax - m_PSFFit.I0) / noise;
					lblSNR.Text = snr.ToString("0.0");
				}
				else
					lblSNR.Text = "N/A";

				if (m_PSFFit != null)
				{
					lblFitVariance.Text = m_PSFFit.GetVariance().ToString("0.0");
					lblFWHM.Text = m_PSFFit.FWHM.ToString("0.0");
				}
				else
				{
					lblFitVariance.Text = "N/A";
					lblFWHM.Text = "N/A";
				}
			}
			else
			{
				lblBackground.Text = "N/A";
				lblNoise.Text = "N/A";
				lblSNR.Text = "N/A";
			}
		}

		private void btnCopy_Click(object sender, EventArgs e)
		{
			Clipboard.SetText(string.Format("Base={0}  Maximum={1}  Amplitude={2}  X={3}  Y={4}\r\nBackground={5} 1-sigma Noise={6} SNR={7}", lblI0.Text, lblIMax.Text, lblIAmp.Text, lblX.Text, lblY.Text, lblBackground.Text, lblNoise.Text, lblSNR.Text));
		}

        private NotMeasuredReasons MeasureObject(
            IImagePixel center,
            uint[,] data,
            uint[,] backgroundPixels,
            int bpp,
            MeasurementsHelper measurer,
            TangraConfig.PreProcessingFilter filter,
            TangraConfig.PhotometryReductionMethod reductionMethod,
            TangraConfig.PsfQuadrature psfQuadrature,
            TangraConfig.PsfFittingMethod psfFittngMethod,
            float aperture,
            double refinedFWHM,
            float refinedAverageFWHM,
            IMeasurableObject measurableObject,
            IImagePixel[] groupCenters,
            float[] aperturesInGroup,
            bool fullDisappearance
            )
        {
            return measurer.MeasureObject(
                center, data, backgroundPixels, bpp, filter, reductionMethod, psfQuadrature, psfFittngMethod,
                aperture, refinedFWHM, refinedAverageFWHM, measurableObject, groupCenters, aperturesInGroup, fullDisappearance);
        }

        private void EnsureMeasurer(float positionTolerance)
        {
            m_Measurer = new MeasurementsHelper(
                                m_VideoController.VideoBitPix,
                                m_BackgroundMethod,
                                TangraConfig.Settings.Photometry.SubPixelSquareSize,
                                TangraConfig.Settings.Photometry.Saturation.GetSaturationForBpp(m_VideoController.VideoBitPix));

            m_Measurer.SetCoreProperties(
                TangraConfig.Settings.Photometry.AnnulusInnerRadius,
                TangraConfig.Settings.Photometry.AnnulusMinPixels,
                TangraConfig.PhotometrySettings.REJECTION_BACKGROUND_PIXELS_STD_DEV,
                positionTolerance);
        }

        private void nudMeasuringAperture_ValueChanged(object sender, EventArgs e)
        {
            MeasureCurrentPSF();
        }

        private void cbMeaMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetMeasurementMethods();
            EnsureMeasurer(2);
            MeasureCurrentPSF();
        }
	}
}
