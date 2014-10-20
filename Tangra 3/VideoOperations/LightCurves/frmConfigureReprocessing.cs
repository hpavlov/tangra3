/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Controller;
using Tangra.Model.Astro;
using Tangra.Model.Config;
using Tangra.Model.Image;

namespace Tangra.VideoOperations.LightCurves
{
    public partial class frmConfigureReprocessing : Form
    {
        private LCMeasurementHeader m_Header;
		private LCMeasurementFooter m_Footer;
        private LightCurveContext m_Context;
        private Color[] m_AllColors;
        private Brush[] m_AllBrushes;
        private Pen[] m_AllPens;
        private uint m_Saturation;

        private LCMeasurement[] m_SelectedMeasurements = new LCMeasurement[4];

        internal frmConfigureReprocessing(
            LCMeasurementHeader header,
			LCMeasurementFooter footer, 
            LightCurveContext context,
            Color[] allColors,
            Brush[] allBrushes,
            Pen[] allPens)
        {
            InitializeComponent();

            m_Context = context;
            m_Header = header;
        	m_Footer = footer;
            m_AllColors = allColors;
            m_AllBrushes = allBrushes;
            m_AllPens = allPens;

        	SetComboboxIndexFromBackgroundMethod(m_Context.BackgroundMethod);
			SetComboboxIndexFromPhotometryReductionMethod(m_Context.SignalMethod);
	        SetComboboxIndexFromPsfQuadratureMethod(m_Context.PsfQuadratureMethod);
        	cbxDigitalFilter.SelectedIndex = (int)m_Context.Filter;
        	nudGamma.Value = (decimal)m_Context.EncodingGamma;

			int maxApertureSize = m_Footer.ReductionContext.GetMaxApertureSize();

			nudAperture1.Maximum = maxApertureSize;
			nudAperture2.Maximum = maxApertureSize;
			nudAperture3.Maximum = maxApertureSize;
        	nudAperture4.Maximum = maxApertureSize;

            m_Saturation = TangraConfig.Settings.Photometry.Saturation.GetSaturationForBpp(context.BitPix);

            SetupControls();
        }

        private void SetupControls()
        {
            if (m_Header.ObjectCount < 4)
            {
                nudAperture4.Value = 1;
                nudAperture4.Enabled = false;
                lblFWHM4.Visible = false;
            }
            if (m_Header.ObjectCount < 3)
            {
                nudAperture3.Value = 1;
                nudAperture3.Enabled = false;
                lblFWHM3.Visible = false;
            }
            if (m_Header.ObjectCount < 2)
            {
                nudAperture2.Value = 1;
                nudAperture2.Enabled = false;
                lblFWHM2.Visible = false;
            }

            if (m_Header.MeasuredFrames > 0)
            {
                if (m_Header.ObjectCount > 0)
                {
                    SetNUDSafe(nudAperture1, m_Header.MeasurementApertures[0]);
                    SetNUDSafe(nudFitArea1, m_Header.PsfFitMatrixSizes[0]);
                    picTarget1Pixels.Image = new Bitmap(68, 68);
                    picTarget1PSF.Image = new Bitmap(68, 68);
                    DrawCollorPanel(pnlColor1, 0);
                }

                if (m_Header.ObjectCount > 1)
                {
					SetNUDSafe(nudAperture2, m_Header.MeasurementApertures[1]);
                    SetNUDSafe(nudFitArea2, m_Header.PsfFitMatrixSizes[1]);
                    picTarget2Pixels.Image = new Bitmap(68, 68);
                    picTarget2PSF.Image = new Bitmap(68, 68);
                    DrawCollorPanel(pnlColor2, 1);
                }

                if (m_Header.ObjectCount > 2)
                {
					SetNUDSafe(nudAperture3, m_Header.MeasurementApertures[2]);
                    SetNUDSafe(nudFitArea3, m_Header.PsfFitMatrixSizes[2]);
                    picTarget3Pixels.Image = new Bitmap(68, 68);
                    picTarget3PSF.Image = new Bitmap(68, 68);
                    DrawCollorPanel(pnlColor3, 2);
                }

                if (m_Header.ObjectCount > 3)
                {
					SetNUDSafe(nudAperture4, m_Header.MeasurementApertures[3]);
                    SetNUDSafe(nudFitArea4, m_Header.PsfFitMatrixSizes[3]);
                    picTarget4Pixels.Image = new Bitmap(68, 68);
                    picTarget4PSF.Image = new Bitmap(68, 68);
                    DrawCollorPanel(pnlColor4, 3);
                }
            }

            sbDataFrames.Minimum = (int)m_Header.MinFrame;
            sbDataFrames.Maximum = (int)m_Header.MaxFrame;
            sbDataFrames.Value = sbDataFrames.Minimum;
            sbDataFrames_ValueChanged(sbDataFrames, EventArgs.Empty);

            // All controll settings would have set the values back to the m_Context and would have marked it as dirty
            // So clean it up now!
            m_Context.MarkClean();
        }

        private void SetNUDSafe(NumericUpDown nud, double val)
        {
            if ((double)nud.Minimum > val) val = (double)nud.Minimum;
            if ((double)nud.Maximum < val) val = (double)nud.Maximum;
            nud.Value = (decimal) val;
        }

        private MeasurementsHelper.Filter GetCurrentFilter()
        {
            switch(m_Context.Filter)
            {
                case LightCurveContext.FilterType.NoFilter:
                    return MeasurementsHelper.Filter.None;

                case LightCurveContext.FilterType.LowPass:
                    return MeasurementsHelper.Filter.LP;

                case LightCurveContext.FilterType.LowPassDifference:
                    return MeasurementsHelper.Filter.LPD;
            }

            throw new IndexOutOfRangeException();
        }

        private void RecomputeData()
        {
            NumericUpDown[] targetApertures = new NumericUpDown[] { nudAperture1, nudAperture2, nudAperture3, nudAperture4 };
            NumericUpDown[] targetFitAreas = new NumericUpDown[] { nudFitArea1, nudFitArea2, nudFitArea3, nudFitArea4 };
			PictureBox[] psfBoxes = new PictureBox[] { picTarget1PSF, picTarget2PSF, picTarget3PSF, picTarget4PSF };

            MeasurementsHelper measurer = new MeasurementsHelper(
                m_Context.BitPix,
                m_Context.BackgroundMethod,
				TangraConfig.Settings.Photometry.SubPixelSquareSize,
                TangraConfig.Settings.Photometry.Saturation.GetSaturationForBpp(m_Context.BitPix));

            for (int i = 0; i < m_Header.ObjectCount; i++)
            {
                // Apply the selected filter, compute the PSF and then draw the data
                LCMeasurement reading = m_SelectedMeasurements[i];
                if (!LCMeasurement.IsEmpty(reading))
                {
                    LCMeasurement updatedReading = reading.Clone();

                    int x0Int = (int)Math.Round(reading.X0);
                    int y0Int = (int)Math.Round(reading.Y0);

                    updatedReading.PsfFit = new PSFFit(x0Int, y0Int);
                    updatedReading.PsfFit.FittingMethod = PSFFittingMethod.NonLinearFit;
                    int pixelDataWidth = updatedReading.PixelData.GetLength(0);
                    int pixelDataHeight = updatedReading.PixelData.GetLength(1);
                    updatedReading.PsfFit.Fit(
                        updatedReading.PixelData,
                        m_Footer.TrackedObjects[updatedReading.TargetNo].PsfFitMatrixSize,
                        x0Int - updatedReading.PixelDataX0 + (pixelDataWidth / 2) + 1,
                        y0Int - updatedReading.PixelDataY0 + (pixelDataHeight / 2) + 1,
                        false);


                    int fitArea = (int)targetFitAreas[i].Value;
					measurer.FindBestFit(
                        reading.X0, reading.Y0,
                        GetCurrentFilter(), reading.PixelData, m_Context.BitPix,
						ref fitArea, m_Header.FixedApertureFlags[i]);

                    updatedReading.PsfFit = measurer.FoundBestPSFFit;

                    updatedReading.PixelData = measurer.PixelData;
                    m_SelectedMeasurements[i] = updatedReading;

                    psfBoxes[reading.TargetNo].Visible = true;
                    PlotSingleGaussian(
                        psfBoxes[reading.TargetNo],
                        updatedReading,
                        m_AllBrushes,
                        (float)targetApertures[i].Value,
                        m_Footer.ReductionContext.BitPix);
                }
            }
        }

        private void PlotCurrentFrameData()
        {
            PlotMeasuredPixels();
            PlotGaussians();
        }

        private void DrawCollorPanel(PictureBox pbox, int no)
        {
            pbox.Image = new Bitmap(16, 16);
            using (Graphics g = Graphics.FromImage(pbox.Image))
            {
                if (no < 0 || no > 3)
                {
                    g.Clear(SystemColors.Control);
                }
                else
                {
                    g.Clear(m_AllColors[no]);
                    g.DrawRectangle(Pens.Black, 0, 0, 15, 15);
                }
            }
        }

        private void PlotGaussians()
        {
            PictureBox[] psfBoxes = new PictureBox[] { picTarget1PSF, picTarget2PSF, picTarget3PSF, picTarget4PSF };
            NumericUpDown[] targetApertures = new NumericUpDown[] { nudAperture1, nudAperture2, nudAperture3, nudAperture4 };

            Label[] fwhmLabels = new Label[] { lblFWHM1, lblFWHM2, lblFWHM3, lblFWHM4 };

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
                    updatedReading.PsfFit.Fit(
                        updatedReading.PixelData,
                        m_Footer.TrackedObjects[updatedReading.TargetNo].PsfFitMatrixSize,
                        x0Int - updatedReading.PixelDataX0 + (pixelDataWidth / 2) + 1,
                        y0Int - updatedReading.PixelDataY0 + (pixelDataHeight / 2) + 1,
                        false);

                    psfBoxes[reading.TargetNo].Visible = true;
                    PlotSingleGaussian(
                        psfBoxes[reading.TargetNo],
                        updatedReading,
                        m_AllBrushes,
                        (float)targetApertures[i].Value,
                        m_Footer.ReductionContext.BitPix);

                    fwhmLabels[updatedReading.TargetNo].Text =
                        string.Format("{0} px = {1} FWHM",
                        targetApertures[updatedReading.TargetNo].Value.ToString("0.00"),
                        ((double)targetApertures[updatedReading.TargetNo].Value / updatedReading.PsfFit.FWHM).ToString("0.0"));
                }
                else
                {
                    psfBoxes[i].Visible = false;
                    fwhmLabels[i].Text = string.Empty;
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
            if (bmp != null && reading.PsfFit != null)
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

        private void PlotMeasuredPixels()
        {
            PictureBox[] targetBoxes = new PictureBox[] { picTarget1Pixels, picTarget2Pixels, picTarget3Pixels, picTarget4Pixels };
            NumericUpDown[] targetApertures = new NumericUpDown[] { nudAperture1, nudAperture2, nudAperture3, nudAperture4 };
            NumericUpDown[] targetFitAreas = new NumericUpDown[] { nudFitArea1, nudFitArea2, nudFitArea3, nudFitArea4 };

            uint allObjectsPeak = 0;
            for (int i = 0; i < m_SelectedMeasurements.Length; i++)
            {
                LCMeasurement reading = m_SelectedMeasurements[i];
                if (!LCMeasurement.IsEmpty(reading) &&
                    reading.TargetNo >= 0 &&
                    reading.TargetNo <= 3)
                {
                    for (int x = 0; x < 17; x++)
                        for (int y = 0; y < 17; y++)
                        {
                            // This is what is needed to get a 17x17 area in the middle of the 35x35 pixel data
                            uint pix = reading.PixelData[x + 9, y + 9];

                            if (allObjectsPeak < pix)
                                allObjectsPeak = pix;
                        }
                }
            }

            for (int i = 0; i < m_SelectedMeasurements.Length; i++)
            {
                LCMeasurement reading = m_SelectedMeasurements[i];
                if (!LCMeasurement.IsEmpty(reading) &&
                    reading.TargetNo >= 0 &&
                    reading.TargetNo <= 3)
                {
                    PlotSingleTargetPixels(
                        targetBoxes[reading.TargetNo],
                        reading,
                        (double)targetApertures[reading.TargetNo].Value,
                        (int)targetFitAreas[reading.TargetNo].Value,
                         allObjectsPeak);
                }
            }
        }

        private void PlotSingleTargetPixels(
            PictureBox pictureBox, LCMeasurement reading,
            double aperture, int psfFitArea, uint allObjectsPeak)
        {
            int MAGN = 4;

            Bitmap image = new Bitmap(35 * MAGN, 35 * MAGN, PixelFormat.Format24bppRgb);

            int pixelsCenterX = (int)Math.Round(reading.X0) - 1;
            int pixelsCenterY = (int)Math.Round(reading.Y0) - 1;

            uint peak = 0;
            for (int x = 0; x < 17; x++)
                for (int y = 0; y < 17; y++)
                {
                    // This is what is needed to get a 17x17 area in the middle of the 35x35 pixel data
                    uint pix = reading.PixelData[x + 9, y + 9];

                    if (peak < pix) peak = pix;
                }

            for (int x = 0; x < 17; x++)
                for (int y = 0; y < 17; y++)
                {
                    // This is what is needed to get a 17x17 area in the middle of the 35x35 pixel data
                    uint pixelValue = reading.PixelData[x + 9, y + 9];

                    Color pixelcolor = SystemColors.Control;

                    if (pixelValue < m_Saturation)
                    {
                        uint modifiedValue = (uint)Math.Min(m_Context.MaxPixelValue, (pixelValue + Math.Round((m_Context.MaxPixelValue - pixelValue) * (pixelValue * 1.0 / allObjectsPeak) * tbIntensity.Value / 100.0)));
                        byte byteValue = (byte)Math.Round((modifiedValue * 255.0 / m_Context.MaxPixelValue));
                        pixelcolor = Color.FromArgb(byteValue, byteValue, byteValue);
                    }
                    else
                        pixelcolor = TangraConfig.Settings.Color.Saturation;


                    // TODO: THIS IS SLOW, but pixels are not too many
                    for (int i = 0; i < MAGN; i++)
                    {
                        for (int j = 0; j < MAGN; j++)
                        {
                            image.SetPixel(MAGN * x + i, MAGN * y + j, pixelcolor);
                        }
                    }
                }


            pictureBox.Image = image;
            using (Graphics g = Graphics.FromImage(pictureBox.Image))
            {
                try
                {
                    // ??
                }
                catch (OverflowException)
                {
                    float radius = (float)aperture * 4;
                    float shift = 7.5f;
                    if (reading.PsfFit != null)
                    {
                        g.DrawEllipse(
                            m_AllPens[reading.TargetNo],
                            (float)(4 * (reading.PsfFit.XCenter - pixelsCenterX + shift) - radius),
                            (float)(4 * (reading.PsfFit.YCenter - pixelsCenterY + shift) - radius),
                            2 * radius,
                            2 * radius);

                        g.DrawRectangle(
                            m_AllPens[reading.TargetNo],
                            (float)(4 * (reading.PsfFit.XCenter - pixelsCenterX + shift) - psfFitArea * 2),
                            (float)(4 * (reading.PsfFit.YCenter - pixelsCenterY + shift) - psfFitArea * 2),
                            4 * psfFitArea,
                            4 * psfFitArea);

                    }
                    else
                    {
                        g.DrawEllipse(
                            m_AllPens[reading.TargetNo],
                            (float)(4 * (reading.X0 - pixelsCenterX + shift) - radius),
                            (float)(4 * (reading.Y0 - pixelsCenterY + shift) - radius),
                            2 * radius,
                            2 * radius);

                        g.DrawRectangle(
                            m_AllPens[reading.TargetNo],
                            (float)(4 * (reading.X0 - pixelsCenterX + shift) - psfFitArea * 2),
                            (float)(4 * (reading.Y0 - pixelsCenterY + shift) - psfFitArea * 2),
                            4 * psfFitArea,
                            4 * psfFitArea);
                    }

                }

                g.Save();
            }

            pictureBox.Refresh();
            //}
        }

        private void sbDataFrames_ValueChanged(object sender, EventArgs e)
        {
            uint frameNo = (uint)sbDataFrames.Value;
            lblFrame.Text = string.Format("Frame No: {0}", frameNo);
            for (int i = 0; i < 4; i++)
            {
                m_SelectedMeasurements[i] = LCMeasurement.Empty;

				foreach (LCMeasurement reading in m_Context.AllReadings[i])
                {
                    if (reading.CurrFrameNo == frameNo)
                    {
                        m_SelectedMeasurements[i] = reading;
                        break;
                    }
                }
            }

            RecomputeData();

            PlotCurrentFrameData();
        }

        private void ApertureChanged(object sender, EventArgs e)
        {
            PlotCurrentFrameData();

            if (sender == nudAperture1)
                m_Context.ReProcessApertures[0] = (float)nudAperture1.Value;
            else if (sender == nudAperture2)
                m_Context.ReProcessApertures[1] = (float)nudAperture2.Value;
            else if (sender == nudAperture3)
                m_Context.ReProcessApertures[2] = (float)nudAperture3.Value;
            else if (sender == nudAperture4)
                m_Context.ReProcessApertures[3] = (float)nudAperture4.Value;

            m_Context.MarkDirtyWithFullReprocessing();
        }

        private void FitAreaChanged(object sender, EventArgs e)
        {
            PlotCurrentFrameData();

            if (sender == nudFitArea1)
                m_Context.ReProcessFitAreas[0] = ((int)nudFitArea1.Value / 2) * 2 + 1;
            else if (sender == nudFitArea2)
                m_Context.ReProcessFitAreas[1] = ((int)nudFitArea2.Value / 2) * 2 + 1;
            else if (sender == nudFitArea3)
                m_Context.ReProcessFitAreas[2] = ((int)nudFitArea3.Value / 2) * 2 + 1;
            else if (sender == nudFitArea4)
                m_Context.ReProcessFitAreas[3] = ((int)nudFitArea4.Value / 2) * 2 + 1;

            m_Context.MarkDirtyWithFullReprocessing();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
			m_Context.BackgroundMethod = ComboboxIndexToBackgroundMethod();
			m_Context.SignalMethod = ComboboxIndexToPhotometryReductionMethod();
			m_Context.PsfQuadratureMethod = ComboboxIndexToPsfQuadratureMethod();
			m_Context.Filter = (LightCurveContext.FilterType)cbxDigitalFilter.SelectedIndex;
			m_Context.EncodingGamma = (double)nudGamma.Value;

            DialogResult = DialogResult.OK;
            Close();
        }

        private void tbIntensity_ValueChanged(object sender, EventArgs e)
        {
            PlotCurrentFrameData();
        }

        public TangraConfig.PhotometryReductionMethod ComboboxIndexToPhotometryReductionMethod()
		{
			if (cbxReductionType.SelectedIndex == 0)
                return TangraConfig.PhotometryReductionMethod.AperturePhotometry;
			else if (cbxReductionType.SelectedIndex == 1)
                return TangraConfig.PhotometryReductionMethod.PsfPhotometry;
			else if (cbxReductionType.SelectedIndex == 2)
                return TangraConfig.PhotometryReductionMethod.OptimalExtraction;
			else
                return TangraConfig.PhotometryReductionMethod.AperturePhotometry;
		}

        public void SetComboboxIndexFromPhotometryReductionMethod(TangraConfig.PhotometryReductionMethod method)
		{
			switch (method)
			{
				case TangraConfig.PhotometryReductionMethod.AperturePhotometry:
					cbxReductionType.SelectedIndex = 0;
					break;

                case TangraConfig.PhotometryReductionMethod.PsfPhotometry:
					cbxReductionType.SelectedIndex = 1;
					break;

                case TangraConfig.PhotometryReductionMethod.OptimalExtraction:
					cbxReductionType.SelectedIndex = 2;
					break;

				default:
					cbxReductionType.SelectedIndex = 0;
					break;
			}
		}

        public TangraConfig.BackgroundMethod ComboboxIndexToBackgroundMethod()
		{
			if (cbxBackgroundMethod.SelectedIndex == 0)
				return TangraConfig.BackgroundMethod.AverageBackground;
			else if (cbxBackgroundMethod.SelectedIndex == 1)
				return TangraConfig.BackgroundMethod.BackgroundMode;
			else if (cbxBackgroundMethod.SelectedIndex == 2)
				return TangraConfig.BackgroundMethod.Background3DPolynomial;
			else if (cbxBackgroundMethod.SelectedIndex == 3)
				return TangraConfig.BackgroundMethod.PSFBackground;
			else if (cbxBackgroundMethod.SelectedIndex == 4)
				return TangraConfig.BackgroundMethod.BackgroundMedian;
			else
				return TangraConfig.BackgroundMethod.AverageBackground;
		}

		public void SetComboboxIndexFromBackgroundMethod(TangraConfig.BackgroundMethod method)
		{
			switch (method)
			{
                case TangraConfig.BackgroundMethod.AverageBackground:
					cbxBackgroundMethod.SelectedIndex = 0;
					break;

                case TangraConfig.BackgroundMethod.BackgroundMode:
					cbxBackgroundMethod.SelectedIndex = 1;
					break;

                case TangraConfig.BackgroundMethod.Background3DPolynomial:
					cbxBackgroundMethod.SelectedIndex = 2;
					break;

                case TangraConfig.BackgroundMethod.PSFBackground:
					cbxBackgroundMethod.SelectedIndex = 3;
					break;

                case TangraConfig.BackgroundMethod.BackgroundMedian:
					cbxBackgroundMethod.SelectedIndex = 4;
					break;
			}
		}

		public TangraConfig.PsfQuadrature ComboboxIndexToPsfQuadratureMethod()
		{
			if (cbxPsfQuadrature.SelectedIndex == 0)
				return TangraConfig.PsfQuadrature.NumericalInAperture;
			else if (cbxPsfQuadrature.SelectedIndex == 1)
				return TangraConfig.PsfQuadrature.Analytical;
			else
				return TangraConfig.PsfQuadrature.NumericalInAperture;
		}

		public void SetComboboxIndexFromPsfQuadratureMethod(TangraConfig.PsfQuadrature method)
		{
			switch (method)
			{
				case TangraConfig.PsfQuadrature.NumericalInAperture:
					cbxPsfQuadrature.SelectedIndex = 0;
					break;

				case TangraConfig.PsfQuadrature.Analytical:
					cbxPsfQuadrature.SelectedIndex = 1;
					break;

				default:
					cbxPsfQuadrature.SelectedIndex = 0;
					break;
			}
		}

		private void cbxReductionType_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (ComboboxIndexToPhotometryReductionMethod() == TangraConfig.PhotometryReductionMethod.PsfPhotometry)
			{
				pnlPsfQuadrature.Visible = true;
				if (cbxPsfQuadrature.SelectedIndex == -1) cbxPsfQuadrature.SelectedIndex = 0;

				EnsurePsfBackgroundControlsStatus();
			}
			else
			{
				pnlPsfQuadrature.Visible = false;
			}			

			if (m_Footer.ReductionContext.LightCurveReductionType == LightCurveReductionType.UntrackedMeasurement)
			{
				if (cbxReductionType.SelectedIndex != 0)
				{
					MessageBox.Show(
						"Only aperture photometry can be used with Untracked Measurements. Please change the measurement type if you want to use a different reduction method.",
						"Invalid Reduction Method", MessageBoxButtons.OK, MessageBoxIcon.Error);

					cbxReductionType.SelectedIndex = 0;
				}
			}
		}

		private void cbxPsfQuadrature_SelectedIndexChanged(object sender, EventArgs e)
		{
			EnsurePsfBackgroundControlsStatus();
		}

		private void EnsurePsfBackgroundControlsStatus()
		{
			if (ComboboxIndexToPhotometryReductionMethod() == TangraConfig.PhotometryReductionMethod.PsfPhotometry &&
				ComboboxIndexToPsfQuadratureMethod() == TangraConfig.PsfQuadrature.Analytical)
			{
				SetComboboxIndexFromBackgroundMethod(TangraConfig.BackgroundMethod.PSFBackground);
				cbxBackgroundMethod.Enabled = false;
			}
			else
			{
				cbxBackgroundMethod.Enabled = true;
			}			
		}

		private void cbxBackgroundMethod_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (m_Footer.ReductionContext.LightCurveReductionType == LightCurveReductionType.UntrackedMeasurement)
			{
				if (cbxBackgroundMethod.SelectedIndex > 1)
				{
					MessageBox.Show(
						"Only average background and background mode can be used with Untracked Measurements. Please change the measurement type if you want to use a different background method.",
						"Invalid Background Method", MessageBoxButtons.OK, MessageBoxIcon.Error);

					cbxBackgroundMethod.SelectedIndex = 0;
				}
			}
		}

    }
}
