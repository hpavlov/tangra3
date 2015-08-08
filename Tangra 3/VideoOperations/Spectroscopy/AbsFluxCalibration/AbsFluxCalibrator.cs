using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
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
		internal bool ObservedFlux;
	}

	public class AbsFluxCalibrator
	{
		public int FromWavelength { get; private set; }
		public int ToWavelength { get; private set; }
		public int WavelengthBinSize { get; private set; }

		public bool IsCalibrated { get; private set; }

		internal PlotContext PlotContext = new PlotContext();

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
			FromWavelength = TangraConfig.Settings.Spectroscopy.MinWavelength;
			ToWavelength = TangraConfig.Settings.Spectroscopy.MaxWavelength;
			WavelengthBinSize = TangraConfig.Settings.Spectroscopy.AbsFluxResolution;
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
				spectra.RescaleToResolution(FromWavelength, ToWavelength, WavelengthBinSize);
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

		private List<double> m_ExtinctionCoefficients = new List<double>();
		private List<double> m_SensitivityCoefficients = new List<double>();
		private List<double> m_Wavelengths = new List<double>();

		private void Calibrate()
		{
			List<AbsFluxSpectra> standards = m_SpectraList.Where(x => x.IsComplete && x.IsStandard).ToList();
			int numEquations = standards.Count;

			m_ExtinctionCoefficients.Clear();
			m_SensitivityCoefficients.Clear();
			m_Wavelengths.Clear();

			if (numEquations > 2)
			{
				for (int i = 0; i < standards[0].DeltaMagnitiudes.Count; i++)
				{
					var A = new SafeMatrix(numEquations, 2);
					var X = new SafeMatrix(numEquations, 1);

					bool containsNaNs = false;

					// Delta_Mag = A * X + B = Ke * X + Ks 
					for (int j = 0; j < numEquations; j++)
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
						SafeMatrix aa = a_T*A;
						SafeMatrix aa_inv = aa.Inverse();
						SafeMatrix bx = (aa_inv*a_T)*X;

						float ke = (float) bx[0, 0];
						float ks = (float) bx[1, 0];

						m_ExtinctionCoefficients.Add(ke);
						m_SensitivityCoefficients.Add(ks);
					}
					else
					{
						m_ExtinctionCoefficients.Add(double.NaN);
						m_SensitivityCoefficients.Add(double.NaN);						
					}
				}

				Trace.WriteLine("------------------------------------------");
				for (int j = 0; j < numEquations; j++)
				{
					standards[j].Residuals.Clear();
					standards[j].ResidualPercentage.Clear();
					standards[j].ResidualPercentageFlux.Clear();
					standards[j].ResidualObsFlux.Clear();

					for (int i = 0; i < standards[0].DeltaMagnitiudes.Count; i++)
					{
						// DeltaM = KE * AirMass + KS = -2.5 * Math.Log10((ObservedFluxes[i] / exposure) / AbsoluteFluxes[i])
						double calculatedDeltaMag = m_ExtinctionCoefficients[i] * standards[j].InputFile.AirMass + m_SensitivityCoefficients[i];
						double calculatedFluxRatio = Math.Pow(10, calculatedDeltaMag / -2.5);
						double calculatedAbsoluteFlux = (standards[j].ObservedFluxes[i] / standards[j].InputFile.Exposure) / calculatedFluxRatio;
						double calculatedObservedFlux = standards[j].AbsoluteFluxes[i] * calculatedFluxRatio * standards[j].InputFile.Exposure;

						double residualAbsoluteFluxOC = calculatedAbsoluteFlux - standards[j].AbsoluteFluxes[i];

						standards[j].Residuals.Add(residualAbsoluteFluxOC);
						standards[j].ResidualPercentage.Add(100 * (calculatedDeltaMag - standards[j].DeltaMagnitiudes[i]) / standards[j].DeltaMagnitiudes[i]);
						standards[j].ResidualPercentageFlux.Add(100 * residualAbsoluteFluxOC / standards[j].AbsoluteFluxes[i]);
						standards[j].ResidualObsFlux.Add(calculatedObservedFlux - standards[j].ObservedFluxes[i]);
					}

					standards[j].AverageBiasPercentage = standards[j].ResidualPercentageFlux.Where(x => !double.IsNaN(x)).ToList().Average() / 100;

					Trace.WriteLine(string.Format("{0}[{1} sec, {2}]: {3}%, AbsFlux: {4}%", 
						standards[j].ToString(),
						standards[j].InputFile.Exposure.ToString("0.00"),
						standards[j].InputFile.AirMass.ToString("0.000"),
						standards[j].AverageBiasPercentage.ToString("0.0"),
						standards[j].ResidualPercentageFlux.Where(x => !double.IsNaN(x)).ToList().Median().ToString("0.0")));
				}

				IsCalibrated = true;
			}			
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

			SizeF size = g.MeasureString("1.3E-12", displaySetting.LegendFont);
			float X_AXIS_LEGEND_HEIGHT = size.Height * 1.5f;
			float Y_AXIS_LEGEND_WIDTH = size.Width * 1.5f;

			// Scale 
			float scaleY = (float)((height - topHeaderHeight - 2 * PADDING - X_AXIS_LEGEND_HEIGHT - LONG_MARK) / maxFlux);
			float scaleX = ((width - 2 * PADDING - Y_AXIS_LEGEND_WIDTH) * 1.0f / (ToWavelength - FromWavelength));


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
					double fluxObs = fluxFunc(j)[i] - fluxResidualFunc(j)[i];

					if (wavelength >= FromWavelength && wavelength <= ToWavelength && !double.IsNaN(flux) && !double.IsNaN(fluxObs))
					{
						float x = (float)(PADDING + Y_AXIS_LEGEND_WIDTH + (wavelength - FromWavelength) * scaleX);
						float y = (float)(height - PADDING - X_AXIS_LEGEND_HEIGHT - flux * scaleY);
						float yO = (float)(height - PADDING - X_AXIS_LEGEND_HEIGHT - fluxObs * scaleY);

						var thisPoint = new PointF(x, y);
						var thisPointO = new PointF(x, yO);
	
						if (prevPoint != Point.Empty)
							g.DrawLine(absFluxPen, prevPoint, thisPoint);

						if (prevPointO != Point.Empty)
							g.DrawLine(absFluxObsPen, prevPointO, thisPointO);

						prevPoint = thisPoint;
						prevPointO = thisPointO;
					}
				}
			}

			DrawXAxis(g, scaleX, width, Y_AXIS_LEGEND_WIDTH, height - X_AXIS_LEGEND_HEIGHT - PADDING, PADDING + topHeaderHeight, displaySetting);

			if (!PlotContext.ObservedFlux)
				DrawYAxis(g, scaleY, height, X_AXIS_LEGEND_HEIGHT, topHeaderHeight, PADDING + Y_AXIS_LEGEND_WIDTH, width - PADDING, maxFlux, displaySetting);
		}

		private void DrawYAxis(Graphics g, float scaleY, int height, float X_AXIS_LEGEND_HEIGHT, int topHeaderHeight, float x0, float x1, double maxFlux, TangraConfig.SpectraViewDisplaySettings displaySetting)
		{
			float verticalSpace = height - 2 * PADDING - X_AXIS_LEGEND_HEIGHT - topHeaderHeight;

			decimal[] minorTicks = new[] { 2E-16M, 5E-16M, 1E-15M, 2E-15M, 5E-15M, 5E-14M, 2E-14M, 5E-14M, 1E-13M, 2E-13M, 5E-13M, 1E-12M, 2E-12M, 5E-12M, 1E-11M };
			decimal[] yInterval = new decimal[] { 1E-15M, 2E-15M, 5E-15M, 1E-14M, 2E-14M, 5E-14M, 1E-13M, 2E-13M, 5E-13M, 1E-12M, 2E-12M, 5E-12M, 1E-11M, 2E-11M, 5E-11M };
			SizeF size = g.MeasureString("1E-14", displaySetting.LegendFont);
			double INTERVAL = 5E-12;
			double MINOR_TICK_INTERVAL = 5E-13;
			for (int i = 0; i < yInterval.Length; i++)
			{
				INTERVAL = (double)yInterval[i];
				MINOR_TICK_INTERVAL = (double)minorTicks[i];
				int numTicks = (int)Math.Ceiling(maxFlux / INTERVAL);
				if (size.Height * 3f * numTicks < verticalSpace)
				{
					break;
				}
			}

			bool zeroMark = true;
			for (double f = 0; f < maxFlux; f += INTERVAL)
			{
				string markStr = zeroMark ? "0" : f.ToString("E1");
				
				zeroMark = false;

				size = g.MeasureString(markStr, displaySetting.LegendFont);
				float tickPos = (float)(height - PADDING - X_AXIS_LEGEND_HEIGHT - f * scaleY);
				//if (tickPos < topHeaderHeight + PADDING) continue;
				if (tickPos > height - X_AXIS_LEGEND_HEIGHT - PADDING) continue;

				g.DrawLine(displaySetting.GridLinesPen, x0, tickPos, x0 + LONG_MARK, tickPos);
				g.DrawLine(displaySetting.GridLinesPen, x1, tickPos, x1 - LONG_MARK, tickPos);

				g.DrawString(markStr, displaySetting.LegendFont, displaySetting.LegendBrush, x0 - size.Width - LONG_MARK, tickPos - size.Height / 2.0f);
			}

			for (double f = 0; f < maxFlux; f += MINOR_TICK_INTERVAL)
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
				int numTicks = (ToWavelength - FromWavelength) / INTERVAL;
				if (size.Width * 1.3f * numTicks < horizontalSpace)
				{
					break;
				}
			}

			for (int w = INTERVAL * ((FromWavelength / INTERVAL) - 1); w < ToWavelength + INTERVAL; w += INTERVAL)
			{
				if (w < FromWavelength) continue;
				string markStr = w.ToString();
				size = g.MeasureString(markStr, displaySetting.LegendFont);
				float tickPos = (float)(PADDING + Y_AXIS_LEGEND_WIDTH + (w - FromWavelength) * scaleX);
				if (tickPos - size.Width / 2.0f < PADDING + Y_AXIS_LEGEND_WIDTH) continue;
				if (tickPos + size.Width / 2.0f > width - PADDING) break;

				g.DrawLine(displaySetting.GridLinesPen, tickPos, y0, tickPos, y0 - LONG_MARK);
				g.DrawLine(displaySetting.GridLinesPen, tickPos, y1, tickPos, y1 + LONG_MARK);

				g.DrawString(markStr, displaySetting.LegendFont, displaySetting.LegendBrush, tickPos - size.Width / 2.0f, y0 + LONG_MARK);
			}

			for (int w = INTERVAL * ((FromWavelength / INTERVAL) - 1); w < ToWavelength + INTERVAL; w += MINOR_TICK_INTERVAL)
			{
				if (w < FromWavelength) continue;
				if (w % INTERVAL == 0) continue;

				float tickPos = (PADDING + Y_AXIS_LEGEND_WIDTH + (w - FromWavelength) * scaleX);
				if (tickPos < PADDING + Y_AXIS_LEGEND_WIDTH) continue;
				if (tickPos > width - PADDING) break;

				g.DrawLine(displaySetting.GridLinesPen, tickPos, y0, tickPos, y0 - SHORT_MARK);
				g.DrawLine(displaySetting.GridLinesPen, tickPos, y1, tickPos, y1 + SHORT_MARK);
			}
		}
	}
}
