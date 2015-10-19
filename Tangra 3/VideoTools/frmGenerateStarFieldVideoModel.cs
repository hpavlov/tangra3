using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Tangra.Astrometry;
using Tangra.Astrometry.Recognition;
using Tangra.Helpers;
using Tangra.Model.Astro;
using Tangra.Model.Config;
using Tangra.Model.Context;
using Tangra.Model.Helpers;
using Tangra.Model.Image;
using Tangra.Model.Numerical;
using Tangra.Model.VideoOperations;
using Tangra.Photometry;
using Tangra.PInvoke;
using Tangra.StarCatalogues;
using Tangra.StarCatalogues.UCAC4;
using Tangra.VideoOperations.Astrometry;
using Tangra.VideoOperations.MakeDarkFlatField;

namespace Tangra.VideoTools
{
    public partial class frmGenerateStarFieldVideoModel : Form
    {
        private double m_RA;
        private double m_DE;

        internal enum ModelledFilter
        {
            B,
            V,
            Sloan_g,
            Sloan_r,
            Sloan_i
        }
        internal class ModelConfig
        {
            public string FileName;
            public bool IsAAVFile;
            public int Integration;
            public int TotalFrames;
            public int FrameWidth;
            public int FrameHeight;
            public int NoiseMean;
	        public int DarkFrameMean;
            public uint MaxPixelValue;
            public int NoiseStdDev;
            public int FlickeringStdDev;
            public double BrighestUnsaturatedStarMag;
            public double LimitStarMag;
            public double FWHM;
            public double RADeg2000;
            public double DEDeg2000;
            public double PlatePixWidth;
            public double PlatePixHeight;
            public double PlateFocLength;
			public double PlateRotAngleRadians;
			public double MatrixToImageScaleX;
			public double MatrixToImageScaleY;
            public ModelledFilter PhotometricFilter;
            public double BVSlope;
            public double LinearityCoefficient;
			public bool CheckMagnitudes;

            public string InfoLine1;
            public string InfoLine2;
            public string InfoLine3;
            public string InfoLine4;
        }


	    private int[,] m_SimulatedDarkFrame;
	    private IVideoController m_VideoController;

	    public frmGenerateStarFieldVideoModel()
	    {
		    InitializeComponent();
	    }

	    public frmGenerateStarFieldVideoModel(IVideoController videoController)
			: this()
	    {
		    m_VideoController = videoController;

            cbxVideoFormat.SelectedIndex = 0;
            cbxAAVIntegration.SelectedIndex = 5;
            cbxPhotometricFilter.SelectedIndex = 3;
	        m_SimulatedDarkFrame = null;
        }

        private void cbxVideoFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxVideoFormat.SelectedIndex == 1)
            {
                cbxAAVIntegration.SelectedIndex = 0;
                cbxAAVIntegration.Enabled = false;
            }
            else
                cbxAAVIntegration.Enabled = true;
        }

        private bool ValidateParameters()
        {
            try
            {
                m_RA = AstroConvert.ToRightAcsension(tbxRA.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Tangra", MessageBoxButtons.OK, MessageBoxIcon.Error);
                tbxRA.Focus();
                return false;
            }

            try
            {
                m_DE = AstroConvert.ToDeclination(tbxDE.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Tangra", MessageBoxButtons.OK, MessageBoxIcon.Error);
                tbxDE.Focus();
                return false;
            }

            return true;
        }

        private void btnGenerateVideo_Click(object sender, EventArgs e)
        {
            if (!ValidateParameters())
                return;

            if (cbxVideoFormat.SelectedIndex == 0)
            {
                saveFileDialog.Filter = "AAV Files (*.aav)|*.aav";
                saveFileDialog.DefaultExt = "aav";
            }
            else if (cbxVideoFormat.SelectedIndex == 1)
            {
                saveFileDialog.Filter = "AVI Files (*.avi)|*.avi";
                saveFileDialog.DefaultExt = "avi";
            }

            if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                UsageStats.Instance.ModelVideosGenerated++;
                UsageStats.Instance.Save();

                var config = new ModelConfig()
                {
                    FileName = saveFileDialog.FileName,
                    IsAAVFile = cbxVideoFormat.SelectedIndex == 0,
                    Integration = (int)Math.Pow(2, cbxAAVIntegration.SelectedIndex),
                    TotalFrames = (int)nudTotalFrames.Value,
                    FrameWidth = (int)nudFrameWidth.Value,
                    FrameHeight = (int)nudFrameHeight.Value,
                    NoiseMean = (int)nudNoiseMean.Value,
                    NoiseStdDev = (int)nudNoiseStdDev.Value,
                    FlickeringStdDev = (int)nudStarFlickering.Value,
                    FWHM = (double)nudStar1FWHM.Value,
                    BrighestUnsaturatedStarMag = (double)nudFirstSaturatedMag.Value,
                    LimitStarMag = (double)nudLimitMag.Value,
                    RADeg2000 = m_RA * 15,
                    DEDeg2000 = m_DE,
                    PlatePixWidth = (double)nudPlatePixWidth.Value,
                    PlatePixHeight = (double)nudPlatePixHeight.Value,
                    PlateFocLength = (double)nudPlateFocLength.Value,
                    PhotometricFilter = (ModelledFilter)cbxPhotometricFilter.SelectedIndex,
                    BVSlope = (double)nudBVSlope.Value,
                    LinearityCoefficient = (double)nudNonLinearity.Value,
					DarkFrameMean = cbxProduceDark.Checked ? (int)nudDarkMean.Value : 0,
					CheckMagnitudes = cbxCheckMagnitudes.Checked
                };

				config.PlateRotAngleRadians = (double)nudPlateRotation.Value * Math.PI / 180.0; 
				config.MatrixToImageScaleX = config.FrameWidth / (double)nudMatrixWidth.Value;
				config.MatrixToImageScaleY = config.FrameHeight / (double)nudMatrixHeight.Value;

                config.InfoLine1 = string.Format("Model Video Generated by Tangra v.{0}", ((AssemblyFileVersionAttribute)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyFileVersionAttribute), true)[0]).Version);
                config.InfoLine2 = string.Format("Noise: {0} +/- {1}, Flickering: {2}, FWHM: {3}", config.NoiseMean, config.NoiseStdDev, config.FlickeringStdDev, config.FWHM.ToString("0.0"));
                config.InfoLine3 = String.Format("Field Center: RA={0} DE={1}", AstroConvert.ToStringValue(m_RA, "HHh MMm SSs"), AstroConvert.ToStringValue(m_DE, "+DDo MM' SS\""));
                config.InfoLine4 = "";

                ThreadPool.QueueUserWorkItem(new WaitCallback(GenerateSimulatedVideo), config);
            }
        }

        private delegate void UpdateUIDelegate(int pbarId, int percent, bool show);

        private void UpdateUI(int pbarId, int percent, bool show)
        {
            pbar.Value = percent;

            if (show && !pbar.Visible)
            {
                pbar.Visible = true;
                pnlConfig.Enabled = false;
            }
            else if (!show && pbar.Visible)
            {
                pbar.Visible = false;
                pnlConfig.Enabled = true;
            }

            pbar.Update();

            Update();
            Application.DoEvents();
        }

        private void InvokeUpdateUI(int pbarId, int percentDone, bool show)
        {
            try
            {
                Invoke(new UpdateUIDelegate(UpdateUI), new object[] { pbarId, percentDone, show });
            }
            catch (InvalidOperationException)
            { }
        }

	    private delegate void SaveDarkFrameDelegate(ModelConfig modelConfig, float[,] averagedFrame);

		private void SaveDarkFrame(ModelConfig modelConfig, float[,] averagedFrame)
	    {
			string fileName = null;
			if (m_VideoController.ShowSaveFileDialog(
				"Save Simulated Dark fame",
				"FITS Image (*.fit)|*.fit",
				ref fileName, this) == DialogResult.OK)
			{
				MakeDarkFlatOperation.SaveDarkOrFlatFrame(fileName, modelConfig.FrameWidth, modelConfig.FrameHeight, string.Format("Simulated dark frame from Mean {0} +/- 1", modelConfig.DarkFrameMean), averagedFrame, 0, modelConfig.TotalFrames);
			}

	    }

	    private void InvokeSaveDarkFrame(ModelConfig modelConfig, float[,] averagedFrame)
	    {
			try
			{
				Invoke(new SaveDarkFrameDelegate(SaveDarkFrame), new object[] { modelConfig, averagedFrame });
			}
			catch (InvalidOperationException)
			{ }
	    }

	    private void GenerateSimulatedVideo(object state)
        {
            InvokeUpdateUI(2, 0, true);

            try
            {
                ModelConfig modelConfig = (ModelConfig)state;
                
                modelConfig.MaxPixelValue = modelConfig.IsAAVFile ? (uint)modelConfig.Integration * 255 : 255;

                double dxRad = modelConfig.FrameWidth * modelConfig.PlatePixWidth / (modelConfig.PlateFocLength * 1000.0);
                double dyRad = modelConfig.FrameHeight* modelConfig.PlatePixHeight / (modelConfig.PlateFocLength * 1000.0);
                double fovDeg = AngleUtility.Elongation(0, 0, dxRad * 180 / Math.PI, dyRad * 180 / Math.PI);

                var catFac = new StarCatalogueFacade(TangraConfig.Settings.StarCatalogue);
                List<IStar> stars = catFac.GetStarsInRegion(m_RA * 15, m_DE, 2 * fovDeg, modelConfig.LimitStarMag, 2000);

				if (modelConfig.IsAAVFile)
					GenerateAAVVideo(modelConfig, stars);
				else
					GenerateAVIVideo(modelConfig, stars);

				if (modelConfig.DarkFrameMean > 0 && cbxPhotometricFilter != null)
				{
					// Save the dark frame 
					float[,] averagedFrame = new float[modelConfig.FrameWidth, modelConfig.FrameHeight];
					for (int x = 0; x < modelConfig.FrameWidth; x++)
					for (int y = 0; y < modelConfig.FrameHeight; y++)
					{
						averagedFrame[x, y] = m_SimulatedDarkFrame[x, y];
					}

					InvokeSaveDarkFrame(modelConfig, averagedFrame);
				}
            }
            finally
            {
                InvokeUpdateUI(2, 100, false);
            }
        }

		private void GenerateAVIVideo(ModelConfig modelConfig, List<IStar> stars)
	    {
			TangraVideo.CloseAviFile();
			TangraVideo.StartNewAviFile(modelConfig.FileName, modelConfig.FrameWidth, modelConfig.FrameHeight, 8, 25, false);

			m_MagnitudeToPeakDict = null;

			try
			{
				//Pixelmap pixmap = new Pixelmap(modelConfig.FrameWidth, modelConfig.FrameHeight, bitPix, new uint[modelConfig.FrameWidth * modelConfig.FrameHeight], null, null);
				//AddOnScreenText(bmp, modelConfig, "The simulated video stars from the next frame");
				//TangraVideo.AddAviVideoFrame(pixmap, modelConfig.Gamma, null);

				uint maxSignalValue = (uint)(255 * modelConfig.Integration);

				Random rndGen = new Random((int)DateTime.Now.Ticks);
				m_SimulatedDarkFrame = new int[modelConfig.FrameWidth, modelConfig.FrameHeight];
				for (int x = 0; x < modelConfig.FrameWidth; x++)
					for (int y = 0; y < modelConfig.FrameHeight; y++)
					{
						if (modelConfig.DarkFrameMean > 0)
						{
							double randomPeak = rndGen.Next(0, 100) == 66 ? 255 : 0;
							double darkPixel = Math.Abs(VideoModelUtils.Random((modelConfig.DarkFrameMean + randomPeak)* modelConfig.Integration, 1));
							double bgPixel = Math.Min(maxSignalValue, Math.Max(0, darkPixel));
							m_SimulatedDarkFrame[x, y] = (int)bgPixel;
						}
						else
							m_SimulatedDarkFrame[x, y] = 0;
					}

				for (int i = 0; i <= modelConfig.TotalFrames; i++)
				{
					using (Pixelmap pixmap = new Pixelmap(modelConfig.FrameWidth, modelConfig.FrameHeight, 16, new uint[modelConfig.FrameWidth * modelConfig.FrameHeight], null, null))
					{
						pixmap.SetMaxSignalValue(maxSignalValue);

						VideoModelUtils.GenerateNoise(pixmap, m_SimulatedDarkFrame, modelConfig.NoiseMean * modelConfig.Integration, modelConfig.NoiseStdDev * modelConfig.Integration);
						GenerateFrame(pixmap, stars, modelConfig);

						TangraVideo.AddAviVideoFrame(pixmap, modelConfig.LinearityCoefficient, (int)pixmap.MaxSignalValue);
					}

					InvokeUpdateUI(2, (int)(100.0 * i / modelConfig.TotalFrames), true);
				}
			}
			finally
			{
				TangraVideo.CloseAviFile();
			}
	    }

		private void GenerateAAVVideo(ModelConfig modelConfig, List<IStar> stars)
	    {
			AavFileCreator.CloseFile();
			AavFileCreator.StartNewFile(modelConfig.FileName, modelConfig.FrameWidth, modelConfig.FrameHeight, modelConfig.Integration);

			m_MagnitudeToPeakDict = null;

			try
			{
				//Pixelmap pixmap = new Pixelmap(modelConfig.FrameWidth, modelConfig.FrameHeight, bitPix, new uint[modelConfig.FrameWidth * modelConfig.FrameHeight], null, null);
				//AddOnScreenText(bmp, modelConfig, "The simulated video stars from the next frame");
				//TangraVideo.AddAviVideoFrame(pixmap, modelConfig.Gamma, null);
				DateTime zeroFrameDT = DateTime.UtcNow;

				uint maxSignalValue = (uint)(255 * modelConfig.Integration);

				Random rndGen = new Random((int)DateTime.Now.Ticks);
				m_SimulatedDarkFrame = new int[modelConfig.FrameWidth, modelConfig.FrameHeight];
				for (int x = 0; x < modelConfig.FrameWidth; x++)
					for (int y = 0; y < modelConfig.FrameHeight; y++)
					{
						if (modelConfig.DarkFrameMean > 0)
						{
							double randomPeak = rndGen.Next(0, 100) == 66 ? 255 : 0;
							double darkPixel = Math.Abs(VideoModelUtils.Random((modelConfig.DarkFrameMean + randomPeak) * modelConfig.Integration, 1));
							double bgPixel = Math.Min(maxSignalValue, Math.Max(0, darkPixel));
							m_SimulatedDarkFrame[x, y] = (int)bgPixel;
						}
						else
							m_SimulatedDarkFrame[x, y] = 0;
					}

				for (int i = 0; i <= modelConfig.TotalFrames; i++)
				{
					using (Pixelmap pixmap = new Pixelmap(modelConfig.FrameWidth, modelConfig.FrameHeight, 16, new uint[modelConfig.FrameWidth * modelConfig.FrameHeight], null, null))
					{
						pixmap.SetMaxSignalValue(maxSignalValue);

						VideoModelUtils.GenerateNoise(pixmap, m_SimulatedDarkFrame, modelConfig.NoiseMean * modelConfig.Integration, modelConfig.NoiseStdDev * modelConfig.Integration);
						GenerateFrame(pixmap, stars, modelConfig);

						DateTime startDT = zeroFrameDT.AddMilliseconds(40 * modelConfig.Integration * i);
						if (Math.Abs(modelConfig.LinearityCoefficient - 1) > 0.0001)
						{
							uint maxVal = pixmap.MaxSignalValue;
							double gammaCoeff = maxVal / Math.Pow((double)maxVal, modelConfig.LinearityCoefficient);
							for (int x = 0; x < pixmap.Width; x++)
							{
								for (int y = 0; y < pixmap.Height; y++)
								{
									uint nonLinVal = (uint)Math.Round(gammaCoeff * Math.Pow(pixmap[x, y], modelConfig.LinearityCoefficient));
									pixmap[x, y] = Math.Min(maxVal, Math.Max(0, nonLinVal));
								}
							}
						}
						AavFileCreator.AddVideoFrame(startDT, startDT.AddMilliseconds(40 * modelConfig.Integration), pixmap);
					}

					InvokeUpdateUI(2, (int)(100.0 * i / modelConfig.TotalFrames), true);
				}
			}
			finally
			{
				AavFileCreator.CloseFile();
			}
	    }

		private void GetOnPlateCoordinates(double raDeg, double deDeg, ModelConfig modelConfig, out double x, out double y)
	    {
			double tangentalX, tangentalY;
			TangentPlane.CelestialToTangent(raDeg, deDeg, modelConfig.RADeg2000, modelConfig.DEDeg2000, out tangentalX, out tangentalY);

			tangentalY = -tangentalY;

			double plateX = tangentalX * (modelConfig.PlateFocLength * 1000.0 / modelConfig.PlatePixWidth);
			double plateY = tangentalY * (modelConfig.PlateFocLength * 1000.0 / modelConfig.PlatePixHeight);

			double mtxX = Math.Cos(modelConfig.PlateRotAngleRadians) * plateX + Math.Sin(modelConfig.PlateRotAngleRadians) * plateY;
			double mtxY = Math.Cos(modelConfig.PlateRotAngleRadians) * plateY - Math.Sin(modelConfig.PlateRotAngleRadians) * plateX;

			x = (modelConfig.FrameWidth / 2) + mtxX * modelConfig.MatrixToImageScaleX;
			y = (modelConfig.FrameHeight / 2) + mtxY * modelConfig.MatrixToImageScaleY;
	    }

		private const float APERTURE = 6.0f;
		private const float GAP = 4.0f;
		private const float ANNULUS = 19.0f;

        private void GenerateFrame(Pixelmap pixmap, List<IStar> stars, ModelConfig modelConfig)
        {
			var mea = new MeasurementsHelper(
				pixmap.BitPixCamera, 
				TangraConfig.BackgroundMethod.BackgroundMedian, 
				TangraConfig.Settings.Photometry.SubPixelSquareSize,
				TangraConfig.Settings.Photometry.Saturation.GetSaturationForBpp(pixmap.BitPixCamera, pixmap.MaxSignalValue));

			float apertureSize = APERTURE;
			float annulusInnerRadius = (GAP + APERTURE) / APERTURE;
			int annulusMinPixels = (int)(Math.PI * (Math.Pow(ANNULUS + GAP + APERTURE, 2) - Math.Pow(GAP + APERTURE, 2)));

			mea.SetCoreProperties(annulusInnerRadius, annulusMinPixels, CorePhotometrySettings.Default.RejectionBackgroundPixelsStdDev, 2 /* TODO: This must be configurable */);

	        var measurements = new Dictionary<IStar, double>();

            foreach (IStar star in stars)
            {
                double x, y;

	            GetOnPlateCoordinates(star.RADeg, star.DEDeg, modelConfig, out x, out y);

                if (x < 0 || x > modelConfig.FrameWidth || y < 0 || y > modelConfig.FrameHeight)
                    continue;

                float starMag = GetStarMag(star, modelConfig.PhotometricFilter);

				float iMax = ModelStarAmplitude(star, starMag, modelConfig, pixmap.BitPixCamera, pixmap.MaxSignalValue);

                //TODO: Model the amplitude for 6px aperture rather than using a PSF amplitude
                
                VideoModelUtils.GenerateStar(pixmap, (float)x, (float)y, (float)modelConfig.FWHM, iMax, 0 /*Use Gaussian */);

	            if (modelConfig.CheckMagnitudes)
	            {
					var image = new AstroImage(pixmap);
					uint[,] data = image.GetMeasurableAreaPixels((int)x, (int)y, 17);
					uint[,] backgroundPixels = image.GetMeasurableAreaPixels((int)x, (int)y, 35);

					PSFFit fit = new PSFFit((int) x, (int) y);
					fit.Fit(data);

					var result = mea.MeasureObject(new ImagePixel(x, y), data, backgroundPixels, pixmap.BitPixCamera,
						TangraConfig.PreProcessingFilter.NoFilter,
						TangraConfig.PhotometryReductionMethod.AperturePhotometry, TangraConfig.PsfQuadrature.NumericalInAperture,
						TangraConfig.PsfFittingMethod.DirectNonLinearFit,
						apertureSize, modelConfig.FWHM, (float)modelConfig.FWHM,
						new FakeIMeasuredObject(fit), 
						null, null,
						false);

					if (result == NotMeasuredReasons.TrackedSuccessfully && !mea.HasSaturatedPixels)
					{
						// Add value for fitting
						measurements.Add(star, mea.TotalReading - mea.TotalBackground);
					}
	            }
            }

	        if (modelConfig.CheckMagnitudes)
				CalculateGagnitudeFit(measurements, modelConfig.BVSlope);
        }

	    internal class MagFitEntry
	    {
		    public double MedianIntensity;
		    public double MedianIntensityError;
			public double APASS_BV_Colour;
		    public double APASS_Sloan_r;
			public double InstrMag;
			public double InstrMagErr;
		    public double Residual;
	    }

		private void CalculateGagnitudeFit(Dictionary<IStar, double> measurements, double fixedColourCoeff)
	    {
			List<MagFitEntry> datapoints  = new List<MagFitEntry>();

			foreach (IStar star in measurements.Keys)
			{
				UCAC4Entry ucac4 = (UCAC4Entry) star;
				if (!double.IsNaN(ucac4.Mag_r) && Math.Abs(ucac4.Mag_r) > 0.00001 && !double.IsNaN(ucac4.MagB) && !double.IsNaN(ucac4.MagV))
				{
					datapoints.Add(new MagFitEntry()
					{
						APASS_Sloan_r = ucac4.Mag_r,
						APASS_BV_Colour = ucac4.MagB - ucac4.MagV,
						MedianIntensity = measurements[star],
						MedianIntensityError = 0.05 * measurements[star]
					});
				}
			}

			float FIXED_COLOUR_COEFF = (float)fixedColourCoeff;
			for (int i = 0; i < datapoints.Count; i++)
			{
				datapoints[i].InstrMag = -2.5 * Math.Log10(datapoints[i].MedianIntensity) + 32 - datapoints[i].APASS_BV_Colour * FIXED_COLOUR_COEFF;
				datapoints[i].InstrMagErr = Math.Abs(-2.5 * Math.Log10((datapoints[i].MedianIntensity + datapoints[i].MedianIntensityError) / datapoints[i].MedianIntensity));
			}

			datapoints = datapoints.Where(x => !double.IsNaN(x.InstrMag) && x.InstrMagErr < 0.2).ToList();

			if (datapoints.Count < 4) return;

			double variance = 0;
			double Ka = 0;
			double Kb = 0;

			int MAX_ITTER = 2;
			for (int itt = 0; itt <= MAX_ITTER; itt++)
			{
				SafeMatrix A = new SafeMatrix(datapoints.Count, 2);
				SafeMatrix X = new SafeMatrix(datapoints.Count, 1);

				int idx = 0;
				for (int i = 0; i < datapoints.Count; i++)
				{
					A[idx, 0] = datapoints[i].InstrMag;
					A[idx, 1] = 1;

					X[idx, 0] = datapoints[i].APASS_Sloan_r;

					idx++;
				}

				SafeMatrix a_T = A.Transpose();
				SafeMatrix aa = a_T * A;
				SafeMatrix aa_inv = aa.Inverse();
				SafeMatrix bx = (aa_inv * a_T) * X;

				Ka = bx[0, 0];
				Kb = bx[1, 0];

				double resSum = 0;
				for (int i = 0; i < datapoints.Count; i++)
				{
					double computedMag = Ka * datapoints[i].InstrMag + Kb;

					double diff = computedMag - datapoints[i].APASS_Sloan_r;

					resSum += diff * diff;
					datapoints[i].Residual = diff;
				}

				variance = Math.Sqrt(resSum / datapoints.Count);

				if (itt < MAX_ITTER)
					datapoints.RemoveAll(x => Math.Abs(x.Residual) > 2 * variance || Math.Abs(x.Residual) > 0.2);
			}

			Trace.WriteLine(string.Format("r' + {3} * (B-V) +  = {0} * M + {1} +/- {2}", Ka.ToString("0.0000"), Kb.ToString("0.0000"), variance.ToString("0.00"), FIXED_COLOUR_COEFF.ToString("0.00000")));

	    }

        private float ModelStarAmplitude(IStar star, float starMag, ModelConfig modelConfig, int bitPix, uint maxSignalValue)
        {
            if (Math.Abs(modelConfig.BVSlope) > 0.001)
            {
                UCAC4Entry ucac4Star = star as UCAC4Entry;
                if (ucac4Star != null)
                {
                    double bv = ucac4Star.MagB - ucac4Star.MagV;
                    if (double.IsNaN(bv)) bv = 0.5; // Default value

                    starMag += (float)(modelConfig.BVSlope * bv);
                }
            }

			InitStarAmplitudeModelling(modelConfig, 0.02f, bitPix, maxSignalValue);
	        double starMagPeak = ModelStarAmplitudePeak(modelConfig, starMag);

			return (float)starMagPeak;
        }

	    private Dictionary<double, int> m_MagnitudeToPeakDict = null;
	    private List<double> m_MagnitudeToPeakMags = null;
		private List<int> m_MagnitudeToPeakPeaks = null;

		private double ModelStarAmplitudePeak(ModelConfig modelConfig, double starMag)
	    {
			for (int i = 0; i < m_MagnitudeToPeakMags.Count - 1; i++)
			{
				if (m_MagnitudeToPeakMags[i] < starMag && m_MagnitudeToPeakMags[i + 1] > starMag)
				{
					double interval = Math.Pow(10, m_MagnitudeToPeakMags[i + 1] - m_MagnitudeToPeakMags[i]);

					return m_MagnitudeToPeakPeaks[i + 1] + Math.Log10(Math.Pow(10, starMag - m_MagnitudeToPeakMags[i]) * interval) * (m_MagnitudeToPeakPeaks[i] - m_MagnitudeToPeakPeaks[i + 1]);
				}
			}

			return double.NaN;
	    }

		private void InitStarAmplitudeModelling(ModelConfig modelConfig, float accuracy, int bitPix, uint maxSignalValue)
		{
			if (m_MagnitudeToPeakDict != null) 
				return;

			m_MagnitudeToPeakDict = new Dictionary<double, int>();
			m_MagnitudeToPeakMags = new List<double>();
			m_MagnitudeToPeakPeaks = new List<int>();

			var mea = new MeasurementsHelper(
				bitPix, 
				TangraConfig.BackgroundMethod.BackgroundMedian, 
				TangraConfig.Settings.Photometry.SubPixelSquareSize,
				TangraConfig.Settings.Photometry.Saturation.GetSaturationForBpp(bitPix, maxSignalValue));

			float apertureSize = APERTURE;
			float annulusInnerRadius = (GAP + APERTURE) / APERTURE;
			int annulusMinPixels = (int)(Math.PI * (Math.Pow(ANNULUS + GAP + APERTURE, 2) - Math.Pow(GAP + APERTURE, 2)));

			mea.SetCoreProperties(annulusInnerRadius, annulusMinPixels, CorePhotometrySettings.Default.RejectionBackgroundPixelsStdDev, 2 /* TODO: This must be configurable */);

			int peak = (int)(maxSignalValue - (modelConfig.NoiseMean + modelConfig.DarkFrameMean) * modelConfig.Integration);
			int TOTAL_STEPS = 30;
			double step = Math.Log10(peak) / TOTAL_STEPS;
			double zeroMag = double.NaN;
			for (int ii = 0; ii < TOTAL_STEPS; ii++)
			{
				int amplitude = (int)Math.Round(Math.Pow(10, Math.Log10(peak) - ii * step));
				Pixelmap pixmap = new Pixelmap(64, 64, bitPix, new uint[64 * 64], null, null);
				VideoModelUtils.GenerateStar(pixmap, 32, 32, (float)modelConfig.FWHM, amplitude, 0 /* Gaussian */);
				PSFFit fit = new PSFFit(32, 32);
				AstroImage img = new AstroImage(pixmap);
				uint[,] data = img.GetMeasurableAreaPixels(32, 32, 17);
				uint[,] backgroundPixels = img.GetMeasurableAreaPixels(32, 32, 35);

				fit.Fit(data);

				var result = mea.MeasureObject(new ImagePixel(32, 32), data, backgroundPixels, pixmap.BitPixCamera,
					TangraConfig.PreProcessingFilter.NoFilter,
					TangraConfig.PhotometryReductionMethod.AperturePhotometry, TangraConfig.PsfQuadrature.NumericalInAperture,
					TangraConfig.PsfFittingMethod.DirectNonLinearFit,
					apertureSize, modelConfig.FWHM, (float)modelConfig.FWHM,
					new FakeIMeasuredObject(fit),
					null, null,
					false);

				if (result == NotMeasuredReasons.TrackedSuccessfully && !mea.HasSaturatedPixels)
				{
					// Add value for fitting
					double measurement = mea.TotalReading - mea.TotalBackground;
					if (double.IsNaN(zeroMag))
						zeroMag = modelConfig.BrighestUnsaturatedStarMag + 2.5 * Math.Log10(measurement);
					double magnitude = -2.5 * Math.Log10(measurement) + zeroMag;

					m_MagnitudeToPeakDict[magnitude] = amplitude;
					m_MagnitudeToPeakMags.Add(magnitude);
					m_MagnitudeToPeakPeaks.Add(amplitude);
				}
			}
	    }

        private float GetStarMag(IStar star, ModelledFilter filter)
        {
            UCAC4Entry ucac4Star = star as UCAC4Entry;
            if (ucac4Star != null)
            {
                switch (filter)
                {
                    case ModelledFilter.B:
                        return double.IsNaN(ucac4Star.MagB) || ucac4Star.MagB > 25 ? (float)ucac4Star.Mag : (float)ucac4Star.MagB;

                    case ModelledFilter.V:
                        return double.IsNaN(ucac4Star.MagV) || ucac4Star.MagV > 25 ? (float)ucac4Star.Mag : (float)ucac4Star.MagV;

                    case ModelledFilter.Sloan_g:
                        return double.IsNaN(ucac4Star.Mag_g) || ucac4Star.Mag_g > 25 ? (float)ucac4Star.Mag : (float)ucac4Star.Mag_g;

                    case ModelledFilter.Sloan_r:
                        return double.IsNaN(ucac4Star.Mag_r) || ucac4Star.Mag_r > 25 ? (float)ucac4Star.Mag : (float)ucac4Star.Mag_r;

                    case ModelledFilter.Sloan_i:
                        return double.IsNaN(ucac4Star.Mag_i) || ucac4Star.Mag_i > 25 ? (float)ucac4Star.Mag : (float)ucac4Star.Mag_i;
                }
            }
            else
            {
                switch (filter)
                {
                    case ModelledFilter.B:
                        return double.IsNaN(star.MagB) || star.MagB > 25 ? (float)star.Mag : (float)star.MagB;

                    case ModelledFilter.V:
                    case ModelledFilter.Sloan_g:
                        return double.IsNaN(star.MagV) || star.MagV > 25 ? (float)star.Mag : (float)star.MagV;

                    case ModelledFilter.Sloan_r:
                        return double.IsNaN(star.MagR) || star.MagR > 25 ? (float)star.Mag : (float)star.MagR;

                    case ModelledFilter.Sloan_i:
                        return (float)star.Mag;
                }
            }

            return (float)star.Mag;
        }

		private void cbxProduceDark_CheckedChanged(object sender, EventArgs e)
		{
			nudDarkMean.Enabled = cbxProduceDark.Checked;
		}
    }
}
