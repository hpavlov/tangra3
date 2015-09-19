using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using Tangra.Helpers;
using Tangra.Model.Config;
using Tangra.Model.Numerical;

namespace Tangra.VideoOperations.Spectroscopy.AbsFluxCalibration
{
	public class PlotContext
	{
		public bool ObservedFlux;
	}


	public enum LinearFitMethod
	{
		LinearAlgebra,
		Fast
	}

	public enum AbsFluxModel
	{
		Linear,
		NonLinearMag,
		NonLinearGain
	}

	public class CalibrationContext
	{
		public int FromWavelength;
		public int ToWavelength;
		public int WavelengthBinSize;
		public bool UseBlurring;
		public AbsFluxModel Model;
		public bool UseFwhmNormalisation;
	    public bool UseNonLinearityNormalisation;
	}

	public class AbsFluxCalibrator
	{
		public bool IsCalibrated { get; private set; }

		internal PlotContext PlotContext = new PlotContext();
		internal CalibrationContext Context = new CalibrationContext();

		private List<AbsFluxSpectra> m_SpectraList = new List<AbsFluxSpectra>();

		public int NumSpectra { get { return m_SpectraList.Count; } }

		public double Bias(int number)
		{
			var spectra = m_SpectraList.SingleOrDefault(x => x.Number == number);
			if (spectra != null) return spectra.AverageBiasPercentage;
			else return double.NaN;
		}

		internal AbsFluxCalibrator()
		{
			Context.FromWavelength = TangraConfig.Settings.Spectroscopy.MinWavelength;
			Context.ToWavelength = TangraConfig.Settings.Spectroscopy.MaxWavelength;
			Context.WavelengthBinSize = TangraConfig.Settings.Spectroscopy.AbsFluxResolution;
			Context.Model = AbsFluxModel.Linear;
		    Context.UseFwhmNormalisation = false;
		    Context.UseNonLinearityNormalisation = true;
            FIT_METHOD = LinearFitMethod.LinearAlgebra;

			IsCalibrated = false;
		}

		private void AssignNumbers()
		{
			for (int i = 0; i < m_SpectraList.Count; i++)
			{
				m_SpectraList[i].Number = i + 1;
			}	
		}

		internal void AddSpectra(AbsFluxSpectra spectra)
		{
			if (!m_SpectraList.Any(x => x.FullFilePath.Equals(spectra.FullFilePath, StringComparison.InvariantCultureIgnoreCase)))
			{
                spectra.RescaleToResolution(Context.FromWavelength, Context.ToWavelength, Context.WavelengthBinSize, Context.UseBlurring, Context.UseFwhmNormalisation, Context.UseNonLinearityNormalisation);
				m_SpectraList.Add(spectra);

				AssignNumbers();

				if (m_SpectraList.Count(x => x.IsComplete && x.IsStandard) > 2)
				{
					Calibrate();
				}
				else
				{
					IsCalibrated = false;
				}
			}
		}

		internal void RemoveSpectra(AbsFluxSpectra spectra)
		{
			m_SpectraList.RemoveAll(x => x.FullFilePath.Equals(spectra.FullFilePath, StringComparison.InvariantCultureIgnoreCase));

			AssignNumbers();

			if (m_SpectraList.Count(x => x.IsComplete && x.IsStandard) > 2)
			{
				Calibrate();
			}
			else
			{
				// Insificient observations to calibrate. Remove/Reset Calibration. 
				IsCalibrated = false;
			}
		}

		internal void PlotSpectra(AbsFluxSpectra spectra, bool plot)
		{
			spectra.PlotSpectra = plot;
		}

		internal void SetCalibrationContext(CalibrationContext context)
		{
			Context = context;
			foreach (AbsFluxSpectra spectra in m_SpectraList)
			{
                spectra.RescaleToResolution(Context.FromWavelength, Context.ToWavelength, Context.WavelengthBinSize, Context.UseBlurring, Context.UseFwhmNormalisation, Context.UseNonLinearityNormalisation);
			}
			Calibrate();
		}

		private List<double> m_MagnitudeCoefficients = new List<double>();
		private List<double> m_ExtinctionCoefficients = new List<double>();
		private List<double> m_SensitivityCoefficients = new List<double>();
		private List<double> m_Wavelengths = new List<double>();

		private void LinearFit_Fast(List<double> airmasses, List<double> magnitudes, out double Intercept, out double Slope)
		{
			double sumX = 0;
			double sumY = 0;
			double sumX2 = 0;
			double sumY2 = 0;
			double sumXY = 0;

			for (int i = 0; i < airmasses.Count; i++)
			{
				sumX = sumX + airmasses[i];
				sumY = sumY + magnitudes[i];
				sumX2 = sumX2 + airmasses[i] * airmasses[i];
				sumY2 = sumY2 + magnitudes[i] * magnitudes[i];
				sumXY = sumXY + airmasses[i] * magnitudes[i];
			}

			double delta = airmasses.Count * sumX2 - sumX * sumX;
			Intercept = (sumX2*sumY  - sumX*sumXY) / delta;
			Slope = (sumXY*airmasses.Count - sumX*sumY)/delta;
			double varnce = (sumY2 + Intercept * Intercept * airmasses.Count + Slope * Slope * sumX2 - 2.0 * (Intercept*sumY + Slope*sumXY - Intercept*Slope*sumX) ) / (airmasses.Count - 2.0);
			double Intercept_Uncertainty = Math.Sqrt(varnce * sumX2 / delta);
			double Slope_Uncertainty = Math.Sqrt(varnce * airmasses.Count / delta);
		}

		private LinearFitMethod FIT_METHOD;

		private void Calibrate()
		{
			List<AbsFluxSpectra> standards = m_SpectraList.Where(x => x.IsComplete && x.IsStandard).ToList();
			int numEquations = standards.Count;

			m_MagnitudeCoefficients.Clear();
			m_ExtinctionCoefficients.Clear();
			m_SensitivityCoefficients.Clear();
			m_Wavelengths.Clear();

			if (numEquations > 2)
			{
				CalibrateModelInternal(standards);

				Trace.WriteLine("------------------------------------------");

				for (int j = 0; j < numEquations; j++)
				{
					standards[j].Residuals.Clear();
					standards[j].ResidualPercentage.Clear();
					standards[j].ResidualPercentageFlux.Clear();
					standards[j].ResidualObsFlux.Clear();

					for (int i = 0; i < standards[0].DeltaMagnitiudes.Count; i++)
					{
						if (Context.Model == AbsFluxModel.Linear)
						{
							// DeltaM = KE * AirMass + KS = -2.5 * Math.Log10((ObservedFluxes[i] / exposure) / AbsoluteFluxes[i])
							double calculatedDeltaMag = m_ExtinctionCoefficients[i] * standards[j].InputFile.AirMass + m_SensitivityCoefficients[i];
							double calculatedFluxRatio = Math.Pow(10, calculatedDeltaMag / -2.5);
						    
                            double exposure = standards[j].InputFile.Exposure;
                            if (Context.UseFwhmNormalisation && !float.IsNaN(standards[j].InputFile.FHWM)) exposure *= standards[j].InputFile.FHWM;

                            double calculatedAbsoluteFlux = (standards[j].ObservedFluxes[i] / exposure) / calculatedFluxRatio;
                            double calculatedObservedFlux = standards[j].AbsoluteFluxes[i] * calculatedFluxRatio * exposure;
						    if (Context.UseNonLinearityNormalisation && !float.IsNaN(standards[j].InputFile.MagCoeff))
						    {
                                calculatedAbsoluteFlux = (Math.Pow(standards[j].ObservedFluxes[i], standards[j].InputFile.MagCoeff) / exposure) / calculatedFluxRatio;
                                calculatedObservedFlux = Math.Pow(standards[j].AbsoluteFluxes[i] * calculatedFluxRatio * exposure, 1 / standards[j].InputFile.MagCoeff);
						    }

							double residualAbsoluteFluxOC = calculatedAbsoluteFlux - standards[j].AbsoluteFluxes[i];

							standards[j].Residuals.Add(residualAbsoluteFluxOC);
							standards[j].ResidualPercentage.Add(100 * (calculatedDeltaMag - standards[j].DeltaMagnitiudes[i]) / standards[j].DeltaMagnitiudes[i]);
							standards[j].ResidualPercentageFlux.Add(100 * residualAbsoluteFluxOC / standards[j].AbsoluteFluxes[i]);
							standards[j].ResidualObsFlux.Add(calculatedObservedFlux - standards[j].ObservedFluxes[i]);
						}
						else if (Context.Model == AbsFluxModel.NonLinearMag || Context.Model == AbsFluxModel.NonLinearGain)
						{
							// AbsM = -2.5 * KM * Math.Log10((ObservedFluxes[i] / exposure)) + KE * AirMass + KS = -2.5 * Math.Log10(AbsoluteFluxes[i])
							double calculatedAbsMag = 
								-2.5 * m_MagnitudeCoefficients[i] * Math.Log10(standards[j].ObservedFluxes[i] / standards[j].InputFile.Exposure) 
								+ m_ExtinctionCoefficients[i] * standards[j].InputFile.AirMass 
								+ m_SensitivityCoefficients[i];

							double absMag = -2.5 * Math.Log10(standards[j].AbsoluteFluxes[i]);
							double calculatedAbsoluteFlux = Math.Pow(10, calculatedAbsMag / -2.5);
							double calculatedObservedFlux = standards[j].InputFile.Exposure * Math.Pow(10, (absMag - m_ExtinctionCoefficients[i] * standards[j].InputFile.AirMass - m_SensitivityCoefficients[i]) / (-2.5 * m_MagnitudeCoefficients[i]));

							double residualAbsoluteFluxOC = calculatedAbsoluteFlux - standards[j].AbsoluteFluxes[i];

							standards[j].Residuals.Add(residualAbsoluteFluxOC);
							standards[j].ResidualPercentage.Add(100 * (calculatedAbsMag - absMag) / absMag);
							standards[j].ResidualPercentageFlux.Add(100 * residualAbsoluteFluxOC / standards[j].AbsoluteFluxes[i]);
							standards[j].ResidualObsFlux.Add(calculatedObservedFlux - standards[j].ObservedFluxes[i]);
						}
					}

					standards[j].AverageBiasPercentage = standards[j].ResidualPercentageFlux.Where(x => !double.IsNaN(x)).ToList().Average() / 100;

					Trace.WriteLine(string.Format("{0}[{1} sec, {2}]: {3}%, AbsFlux: {4}%, Mag: {5}", 
						standards[j].ToString(),
						standards[j].InputFile.Exposure.ToString("0.00"),
						standards[j].InputFile.AirMass.ToString("0.000"),
						standards[j].AverageBiasPercentage.ToString("0.0"),
						standards[j].ResidualPercentageFlux.Where(x => !double.IsNaN(x)).ToList().Median().ToString("0.0"),
                        standards[j].m_CalSpecStar.MagV.ToString("0.000")));
				}

                #region Compute absolute fluxes of program objects
                List<AbsFluxSpectra> programStars = m_SpectraList.Where(x => x.IsComplete && !x.IsStandard).ToList();

                for (int j = 0; j < programStars.Count; j++)
                {
                    programStars[j].AbsoluteFluxes = new List<double>();

                    for (int i = 0; i < standards[0].DeltaMagnitiudes.Count; i++)
			        {
						if (Context.Model == AbsFluxModel.Linear)
				        {
							// DeltaM = KE * AirMass + KS = -2.5 * Math.Log10((ObservedFluxes[i] / exposure) / AbsoluteFluxes[i])
							double calculatedDeltaMag = m_ExtinctionCoefficients[i] * programStars[j].InputFile.AirMass + m_SensitivityCoefficients[i];
							double calculatedFluxRatio = Math.Pow(10, calculatedDeltaMag / -2.5);
                            
                            double exposure = programStars[j].InputFile.Exposure;
                            if (Context.UseFwhmNormalisation && !float.IsNaN(programStars[j].InputFile.FHWM)) exposure *= programStars[j].InputFile.FHWM;

                            double calculatedAbsoluteFlux = (programStars[j].ObservedFluxes[i] / exposure) / calculatedFluxRatio;

                            if (Context.UseNonLinearityNormalisation && !float.IsNaN(programStars[j].InputFile.MagCoeff))
                                calculatedAbsoluteFlux = (Math.Pow(programStars[j].ObservedFluxes[i], programStars[j].InputFile.MagCoeff)  / exposure) / calculatedFluxRatio;

							programStars[j].AbsoluteFluxes.Add(calculatedAbsoluteFlux);
				        }
						else if (Context.Model == AbsFluxModel.NonLinearMag || Context.Model == AbsFluxModel.NonLinearGain)
				        {
							// AbsM = -2.5 * KM * Math.Log10((ObservedFluxes[i] / exposure)) + KE * AirMass + KS = -2.5 * Math.Log10(AbsoluteFluxes[i])
							double calculatedAbsMag =
								-2.5 * m_MagnitudeCoefficients[i] * Math.Log10(programStars[j].ObservedFluxes[i] / programStars[j].InputFile.Exposure)
								+ m_ExtinctionCoefficients[i] * programStars[j].InputFile.AirMass
								+ m_SensitivityCoefficients[i];

							double calculatedAbsoluteFlux = Math.Pow(10, calculatedAbsMag / -2.5);

							programStars[j].AbsoluteFluxes.Add(calculatedAbsoluteFlux);
				        }
			        }
                }
                #endregion

                IsCalibrated = true;
			}
		}

		private void CalibrateModelInternal(List<AbsFluxSpectra> standards)
		{
			if (Context.Model == AbsFluxModel.Linear)
			{
				if (FIT_METHOD == LinearFitMethod.LinearAlgebra)
					CalibrateLinearModelInternal(standards);
				else if (FIT_METHOD == LinearFitMethod.Fast)
					CalibrateLinearModelInternalFast(standards);
			}
			else if (Context.Model == AbsFluxModel.NonLinearMag)
				CalibrateNonLinearMagModelInternal(standards);
			else if (Context.Model == AbsFluxModel.NonLinearGain)
				CalibrateNonLinearGainModelInternal(standards);
		}

		private void CalibrateNonLinearMagModelInternal(List<AbsFluxSpectra> standards)
		{
			m_MagnitudeCoefficients.Clear();
			m_ExtinctionCoefficients.Clear();
			m_SensitivityCoefficients.Clear();
			m_Wavelengths.Clear();

			for (int i = 0; i < standards[0].DeltaMagnitiudes.Count; i++)
			{
				var A = new SafeMatrix(standards.Count, 3);
				var X = new SafeMatrix(standards.Count, 1);

				bool containsNaNs = false;

				// MagAbs = A * MagInst + B * X + C = Km * MagInst + Ke * X + Ks
				for (int j = 0; j < standards.Count; j++)
				{
					A[j, 0] = -2.5 * Math.Log10(standards[j].ObservedFluxes[i] / standards[j].InputFile.Exposure);
					A[j, 1] = standards[j].InputFile.AirMass;
					A[j, 2] = 1;

					double absMag = -2.5 * Math.Log10(standards[j].AbsoluteFluxes[i]);
					X[j, 0] = absMag;
					if (double.IsNaN(absMag) || double.IsNaN(A[j, 0])) containsNaNs = true;
				}

				m_Wavelengths.Add(standards[0].ResolvedWavelengths[i]);
				if (!containsNaNs)
				{
					SafeMatrix a_T = A.Transpose();
					SafeMatrix aa = a_T * A;
					SafeMatrix aa_inv = aa.Inverse();
					SafeMatrix bx = (aa_inv * a_T) * X;

					float km = (float)bx[0, 0];
					float ke = (float)bx[1, 0];
					float ks = (float)bx[2, 0];

					m_MagnitudeCoefficients.Add(km);
					m_ExtinctionCoefficients.Add(ke);
					m_SensitivityCoefficients.Add(ks);
				}
				else
				{
					m_MagnitudeCoefficients.Add(double.NaN);
					m_ExtinctionCoefficients.Add(double.NaN);
					m_SensitivityCoefficients.Add(double.NaN);
				}
			}
		}

		private void CalibrateLinearModelInternalFast(List<AbsFluxSpectra> standards)
		{
			var airmasses = new List<double>();
			var magnitudes = new List<double>();

			for (int i = 0; i < standards[0].DeltaMagnitiudes.Count; i++)
			{
				bool containsNaNs = false;
				airmasses.Clear();
				magnitudes.Clear();
				double Intercept;
				double Slope;
				for (int j = 0; j < standards.Count; j++)
				{
					airmasses.Add(standards[j].InputFile.AirMass);

					double deltaMag = standards[j].DeltaMagnitiudes[i];
					if (double.IsNaN(deltaMag)) containsNaNs = true;
					magnitudes.Add(deltaMag);
				}

				LinearFit_Fast(airmasses, magnitudes, out Intercept, out Slope);

				if (!containsNaNs)
				{
					m_MagnitudeCoefficients.Add(1);
					m_ExtinctionCoefficients.Add((float)Slope);
					m_SensitivityCoefficients.Add((float)Intercept);
				}
				else
				{
					m_MagnitudeCoefficients.Add(double.NaN);
					m_ExtinctionCoefficients.Add(double.NaN);
					m_SensitivityCoefficients.Add(double.NaN);
				}
			}
		}

		private void CalibrateLinearModelInternal(List<AbsFluxSpectra> standards)
		{
			m_MagnitudeCoefficients.Clear();
			m_ExtinctionCoefficients.Clear();
			m_SensitivityCoefficients.Clear();
			m_Wavelengths.Clear();

			for (int i = 0; i < standards[0].DeltaMagnitiudes.Count; i++)
			{
				var A = new SafeMatrix(standards.Count, 2);
				var X = new SafeMatrix(standards.Count, 1);

				bool containsNaNs = false;

				// Delta_Mag = A * X + B = Ke * X + Ks 
				for (int j = 0; j < standards.Count; j++)
				{
					A[j, 0] = standards[j].InputFile.AirMass;
					A[j, 1] = 1;

					double deltaMag = standards[j].DeltaMagnitiudes[i];
					X[j, 0] = deltaMag;
					if (double.IsNaN(deltaMag)) containsNaNs = true;
				}

				m_Wavelengths.Add(standards[0].ResolvedWavelengths[i]);
				if (!containsNaNs)
				{
					SafeMatrix a_T = A.Transpose();
					SafeMatrix aa = a_T * A;
					SafeMatrix aa_inv = aa.Inverse();
					SafeMatrix bx = (aa_inv * a_T) * X;

					float ke = (float)bx[0, 0];
					float ks = (float)bx[1, 0];

					m_MagnitudeCoefficients.Add(1);
					m_ExtinctionCoefficients.Add(ke);
					m_SensitivityCoefficients.Add(ks);
				}
				else
				{
					m_MagnitudeCoefficients.Add(double.NaN);
					m_ExtinctionCoefficients.Add(double.NaN);
					m_SensitivityCoefficients.Add(double.NaN);
				}
			}
		}

		private void CalibrateNonLinearGainModelInternal(List<AbsFluxSpectra> standards)
		{
			CalibrateNonLinearMagModelInternal(standards);

			double medianKM = m_ExtinctionCoefficients.Median();
			m_ExtinctionCoefficients = m_ExtinctionCoefficients.Select(x => medianKM).ToList();
		}

		private const int PADDING = 10;
		private const int LONG_MARK = 6;
		private const int SHORT_MARK = 3;

		internal void PlotCalibration(Graphics g, int width, int height, int topHeaderHeight, TangraConfig.SpectraViewDisplaySettings displaySetting)
		{
			List<AbsFluxSpectra> plotObjects = m_SpectraList.Where(x => x.IsComplete && x.PlotSpectra).ToList();

			double maxFlux = double.MinValue;

			Func<int, List<double>> fluxFunc;
			Func<int, List<double>> fluxResidualFunc;
			if (PlotContext.ObservedFlux)
			{
				fluxFunc = x => plotObjects[x].ObservedFluxes;
				fluxResidualFunc = x => plotObjects[x].Residuals;
			}
			else
			{
				fluxFunc = x => plotObjects[x].AbsoluteFluxes;
				fluxResidualFunc = x => plotObjects[x].Residuals;
			}

			for (int j = 0; j < plotObjects.Count; j++)
			{
				double max = fluxFunc(j).Max();
				if (plotObjects[j].IsStandard && plotObjects[j].AverageBiasPercentage < 0)
					max = max * (1 + Math.Abs(plotObjects[j].AverageBiasPercentage));
				else if (plotObjects[j].IsStandard && plotObjects[j].AverageBiasPercentage > 0)
					max = max * (1 + Math.Abs(plotObjects[j].AverageBiasPercentage));

				if (max > maxFlux) maxFlux = max;
			}

            SizeF size = g.MeasureString("1234", displaySetting.LegendFont);

            float X_AXIS_LEGEND_HEIGHT = size.Height * 1.5f;

            double yInterval;
            double yMinorTickInterval;
		    string labelFormat;
            ComputeYAxisLabelScale(g, height, X_AXIS_LEGEND_HEIGHT, topHeaderHeight, maxFlux, displaySetting, out yInterval, out yMinorTickInterval, out labelFormat);
            size = g.MeasureString(((int)maxFlux).ToString(labelFormat), displaySetting.LegendFont);

		    float Y_AXIS_LEGEND_WIDTH = size.Width * 1.5f;

			// Scale 
			float scaleY = (float)((height - topHeaderHeight - 2 * PADDING - X_AXIS_LEGEND_HEIGHT - LONG_MARK) / maxFlux);
			float scaleX = ((width - 2 * PADDING - Y_AXIS_LEGEND_WIDTH) * 1.0f / (Context.ToWavelength - Context.FromWavelength));


			g.DrawRectangle(displaySetting.GridLinesPen, 
				PADDING + Y_AXIS_LEGEND_WIDTH, 
				PADDING + topHeaderHeight, 
				width - 2 * PADDING - Y_AXIS_LEGEND_WIDTH, 
				height - topHeaderHeight - 2 * PADDING - X_AXIS_LEGEND_HEIGHT);

			PointF prevPoint;
			PointF prevPointO;
			for (int j = 0; j < plotObjects.Count; j++)
			{
				prevPoint = PointF.Empty;
				prevPointO = PointF.Empty;

				int objIndex = plotObjects[j].Number - 1;
				Pen absFluxPen = objIndex >= 0 && objIndex < displaySetting.AbsFluxPen.Length
					? displaySetting.AbsFluxPen[objIndex]
					: displaySetting.AbsFluxPenDefault;

				Pen absFluxObsPen = objIndex >= 0 && objIndex < displaySetting.AbsFluxObsPen.Length
					? displaySetting.AbsFluxObsPen[objIndex]
					: displaySetting.AbsFluxObsPenDefault;

				for (int i = 0; i < fluxFunc(j).Count; i++)
				{
					double flux = fluxFunc(j)[i];
					double wavelength = plotObjects[j].ResolvedWavelengths[i];

					if (wavelength >= Context.FromWavelength && wavelength <= Context.ToWavelength && !double.IsNaN(flux))
					{
						float x = (float)(PADDING + Y_AXIS_LEGEND_WIDTH + (wavelength - Context.FromWavelength) * scaleX);
						float y = (float)(height - PADDING - X_AXIS_LEGEND_HEIGHT - flux * scaleY);

						var thisPoint = new PointF(x, y);
	
						if (prevPoint != Point.Empty)
							g.DrawLine(absFluxPen, prevPoint, thisPoint);

						prevPoint = thisPoint;

                        if (plotObjects[j].IsStandard)
                        {
                            double fluxObs = fluxFunc(j)[i] - fluxResidualFunc(j)[i];

                            if (!double.IsNaN(fluxObs))
                            {
                                float yO = (float)(height - PADDING - X_AXIS_LEGEND_HEIGHT - fluxObs * scaleY);
                                var thisPointO = new PointF(x, yO);
                                if (prevPointO != Point.Empty)
                                    g.DrawLine(absFluxObsPen, prevPointO, thisPointO);
                                prevPointO = thisPointO;
                            }                            
                        }
					}
				}
			}

			DrawXAxis(g, scaleX, width, Y_AXIS_LEGEND_WIDTH, height - X_AXIS_LEGEND_HEIGHT - PADDING, PADDING + topHeaderHeight, displaySetting);
			DrawYAxis(g, scaleY, height, X_AXIS_LEGEND_HEIGHT, PADDING + Y_AXIS_LEGEND_WIDTH, width - PADDING, maxFlux, yInterval, yMinorTickInterval, labelFormat, displaySetting);
		}

        private void ComputeYAxisLabelScale(Graphics g, int height, float X_AXIS_LEGEND_HEIGHT, int topHeaderHeight, double maxFlux, TangraConfig.SpectraViewDisplaySettings displaySetting, out double interval, out double minorTickInterval, out string labelFormat)
        {
            float verticalSpace = height - 2 * PADDING - X_AXIS_LEGEND_HEIGHT - topHeaderHeight;

            var probeIntervals = new List<decimal>();
            var minorTicks = new List<decimal>();

            if (PlotContext.ObservedFlux)
            {
                int maxPower = (int) Math.Ceiling(Math.Log10(maxFlux));

                for (int i = 0; i <= maxPower; i++)
                {
                    int factor = (int) Math.Round(Math.Pow(10, i));
                    probeIntervals.Add(1*factor);
                    probeIntervals.Add(2*factor);
                    probeIntervals.Add(5*factor);

                    int factorMinorTick = (int) Math.Round(Math.Pow(10, i - 1));
                    minorTicks.Add(2*factorMinorTick);
                    minorTicks.Add(5*factorMinorTick);
                    minorTicks.Add(10*factorMinorTick);
                }
                labelFormat = "";
            }
            else
            {
                probeIntervals.AddRange(new[] { 2E-16M, 5E-16M, 1E-15M, 2E-15M, 5E-15M, 5E-14M, 2E-14M, 5E-14M, 1E-13M, 2E-13M, 5E-13M, 1E-12M, 2E-12M, 5E-12M, 1E-11M});
                minorTicks.AddRange(new decimal[] { 1E-15M, 2E-15M, 5E-15M, 1E-14M, 2E-14M, 5E-14M, 1E-13M, 2E-13M, 5E-13M, 1E-12M, 2E-12M, 5E-12M, 1E-11M, 2E-11M, 5E-11M });
                labelFormat = "E1";
            }

            SizeF size = g.MeasureString("1234", displaySetting.LegendFont);

            interval = 0;
            minorTickInterval = 0;

            for (int i = 0; i < probeIntervals.Count; i++)
            {
                interval = (double)probeIntervals[i];
                minorTickInterval = (double)minorTicks[i];
                int numTicks = (int)Math.Ceiling(maxFlux / interval);
                if (size.Height * 3f * numTicks < verticalSpace)
                {
                    break;
                }
            }
        }

        private void DrawYAxis(Graphics g, float scaleY, int height, float X_AXIS_LEGEND_HEIGHT, float x0, float x1, double maxFlux, double interval, double minorTicksinterval, string labelFormat, TangraConfig.SpectraViewDisplaySettings displaySetting)
		{
			bool zeroMark = true;
            for (double f = 0; f < maxFlux; f += interval)
			{
				string markStr = zeroMark ? "0" : f.ToString(labelFormat);
				
				zeroMark = false;

                SizeF size = g.MeasureString(markStr, displaySetting.LegendFont);
				float tickPos = (float)(height - PADDING - X_AXIS_LEGEND_HEIGHT - f * scaleY);
				if (tickPos > height - X_AXIS_LEGEND_HEIGHT - PADDING) continue;

				g.DrawLine(displaySetting.GridLinesPen, x0, tickPos, x0 + LONG_MARK, tickPos);
				g.DrawLine(displaySetting.GridLinesPen, x1, tickPos, x1 - LONG_MARK, tickPos);

				g.DrawString(markStr, displaySetting.LegendFont, displaySetting.LegendBrush, x0 - size.Width - LONG_MARK, tickPos - size.Height / 2.0f);
			}

            for (double f = 0; f < maxFlux; f += minorTicksinterval)
			{
				float tickPos = (float)(height - PADDING - X_AXIS_LEGEND_HEIGHT - f * scaleY);

				g.DrawLine(displaySetting.GridLinesPen, x0, tickPos, x0 + SHORT_MARK, tickPos);
				g.DrawLine(displaySetting.GridLinesPen, x1, tickPos, x1 - SHORT_MARK, tickPos);
			}
		}

		private void DrawXAxis(Graphics g, float scaleX, int width, float Y_AXIS_LEGEND_WIDTH, float y0, float y1, TangraConfig.SpectraViewDisplaySettings displaySetting)
		{
			float horizontalSpace = width - 2 * PADDING - Y_AXIS_LEGEND_WIDTH;

			int[] minorTicks = new[] { 20, 50, 100, 100, 500, 500 };
			int[] xInterval = new [] { 100, 200, 500, 1000, 2000, 5000};
			SizeF size = g.MeasureString("5000", displaySetting.LegendFont);
			int INTERVAL = 5000;
			int MINOR_TICK_INTERVAL = 500;
			for (int i = 0; i < xInterval.Length; i++)
			{
				INTERVAL = xInterval[i];
				MINOR_TICK_INTERVAL = minorTicks[i];
				int numTicks = (Context.ToWavelength - Context.FromWavelength) / INTERVAL;
				if (size.Width * 1.3f * numTicks < horizontalSpace)
				{
					break;
				}
			}

			for (int w = INTERVAL * ((Context.FromWavelength / INTERVAL) - 1); w < Context.ToWavelength + INTERVAL; w += INTERVAL)
			{
				if (w < Context.FromWavelength) continue;
				string markStr = w.ToString();
				size = g.MeasureString(markStr, displaySetting.LegendFont);
				float tickPos = (float)(PADDING + Y_AXIS_LEGEND_WIDTH + (w - Context.FromWavelength) * scaleX);
				if (tickPos - size.Width / 2.0f < PADDING + Y_AXIS_LEGEND_WIDTH) continue;
				if (tickPos + size.Width / 2.0f > width - PADDING) break;

				g.DrawLine(displaySetting.GridLinesPen, tickPos, y0, tickPos, y0 - LONG_MARK);
				g.DrawLine(displaySetting.GridLinesPen, tickPos, y1, tickPos, y1 + LONG_MARK);

				g.DrawString(markStr, displaySetting.LegendFont, displaySetting.LegendBrush, tickPos - size.Width / 2.0f, y0 + LONG_MARK);
			}

			for (int w = INTERVAL * ((Context.FromWavelength / INTERVAL) - 1); w < Context.ToWavelength + INTERVAL; w += MINOR_TICK_INTERVAL)
			{
				if (w < Context.FromWavelength) continue;
				if (w % INTERVAL == 0) continue;

				float tickPos = (PADDING + Y_AXIS_LEGEND_WIDTH + (w - Context.FromWavelength) * scaleX);
				if (tickPos < PADDING + Y_AXIS_LEGEND_WIDTH) continue;
				if (tickPos > width - PADDING) break;

				g.DrawLine(displaySetting.GridLinesPen, tickPos, y0, tickPos, y0 - SHORT_MARK);
				g.DrawLine(displaySetting.GridLinesPen, tickPos, y1, tickPos, y1 + SHORT_MARK);
			}
		}

        public void ExportProgramStarsData(string fileName)
        {
            if (IsCalibrated)
            {
                var output = new StringBuilder();
                var programSpectras = m_SpectraList.Where(x => x.m_CalSpecStar == null).ToList();

                for (int i = 0; i < m_SpectraList[0].ResolvedWavelengths.Count; i++)
                {
                    double w = m_SpectraList[0].ResolvedWavelengths[i];
                    output.Append(w.ToString());

                    for (int j = 0; j < programSpectras.Count; j++)
                    {
                        output.Append(",");
                        output.Append(programSpectras[j].AbsoluteFluxes[i].ToString());
                    }
                    output.AppendLine();
                }

                File.WriteAllText(fileName, output.ToString());
            }
        }
	}
}
