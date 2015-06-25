using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Model.Numerical;

namespace Tangra.VideoOperations.Spectroscopy.Helpers
{
	public interface IWavelengthCalibration
	{
		float ResolveWavelength(SpectraPoint point);
		int ResolvePixelNo(float wavelength);
		float GetCalibrationScale();
		int GetCalibrationOrder();
		float GetCalibrationRMS();
		SpectraCalibration GetSpectraCalibration();
	}

    public interface IRegressionWavelengthCalibration
    {
        void Calibrate();
        void AddDataPoint(int pointNo, float wavelength);
    }

    public class SpectraCalibrator
    {
	    private MasterSpectra m_MasterSpectra;

	    private int?[] m_PixelNos = new int?[8];
        private float[] m_Wavelengths = new float[8];

		private IWavelengthCalibration m_WavelengthCalibration;

	    public SpectraCalibrator(MasterSpectra masterSpectra)
	    {
	        m_MasterSpectra = masterSpectra;
			m_WavelengthCalibration = null;
	    }

	    public void SetMarker(int pixelNo, float wavelength, bool attemptCalibration, int polynomialOrder)
        {
	        for (int i = 0; i < 8; i++)
	        {
	            if (!m_PixelNos[i].HasValue)
	            {
	                if (!IsRepeatedLine(pixelNo, wavelength))
	                {
                        m_PixelNos[i] = pixelNo;
                        m_Wavelengths[i] = wavelength;
	                }

	                break;
	            }
	        }

			if (attemptCalibration)
				Calibrate(polynomialOrder);
        }

		private bool IsRepeatedLine(int pixelNo, float wavelength)
		{
		    for (int i = 0; i < 8; i++)
		    {
                if (m_PixelNos[i].HasValue && (m_PixelNos[i].Value == pixelNo || Math.Abs(m_Wavelengths[i] - wavelength) < 0.1))
                    return true;
		    }

			return false;
		}

        public void SetFirstOrderDispersion(float dispersion)
        {
            if (m_PixelNos[0].HasValue)
            {
                m_PixelNos[1] = m_PixelNos[0].Value + 1000;
                m_Wavelengths[1] = dispersion * 1000;

                Calibrate(1);
            }
        }

		private void Calibrate(int polynomialOrder)
        {
			if (polynomialOrder == 1)
			{
                if (m_PixelNos[0].HasValue && m_PixelNos[1].HasValue && !m_PixelNos[2].HasValue)
                {
                    var calibration = new LinearWavelengthCalibration();
                    calibration.Calibrate(m_PixelNos[0].Value, m_PixelNos[1].Value, m_Wavelengths[0], m_Wavelengths[1]);

                    m_WavelengthCalibration = calibration;
                }
                else if (m_PixelNos[0].HasValue && m_PixelNos[1].HasValue && m_PixelNos[2].HasValue)
                {
                    CalibrateWithRegression<LinearWavelengthRegressionCalibration>();
                }
			}
            else if (polynomialOrder == 2)
            {
                if (m_PixelNos[0].HasValue && m_PixelNos[1].HasValue && m_PixelNos[2].HasValue && !m_PixelNos[3].HasValue)
                {
                    var calibration = new QuadraticWavelengthCalibration();
                    calibration.Calibrate(m_PixelNos[0].Value, m_PixelNos[1].Value, m_PixelNos[2].Value, m_Wavelengths[0], m_Wavelengths[1], m_Wavelengths[2]);

                    m_WavelengthCalibration = calibration;
                }
                else if (m_PixelNos[0].HasValue && m_PixelNos[1].HasValue && m_PixelNos[2].HasValue && m_PixelNos[3].HasValue)
                {
                    CalibrateWithRegression<QuadraticWavelengthRegressionCalibration>();
                }
            }
            else if (polynomialOrder == 3)
            {
                if (m_PixelNos[0].HasValue && m_PixelNos[1].HasValue && m_PixelNos[2].HasValue && m_PixelNos[3].HasValue && !m_PixelNos[4].HasValue)
                {
                    MessageBox.Show("Not implemented yet");
                }
                else if (m_PixelNos[0].HasValue && m_PixelNos[1].HasValue && m_PixelNos[2].HasValue && m_PixelNos[3].HasValue && m_PixelNos[4].HasValue)
                {
                    CalibrateWithRegression<CubicWavelengthRegressionCalibration>();
                }
            }

            else
                MessageBox.Show("Calibration of this order hasn't been implemented yet");
        }

        private void CalibrateWithRegression<TCalibration>() where TCalibration : IWavelengthCalibration, IRegressionWavelengthCalibration, new()
        {
            var calibration = new TCalibration();
            for (int i = 0; i < 8; i++)
            {
                if (m_PixelNos[i].HasValue)
                    calibration.AddDataPoint(m_PixelNos[i].Value, m_Wavelengths[i]);
            }
            calibration.Calibrate();

            m_WavelengthCalibration = calibration;
        }
		

        public bool IsCalibrated()
        {
			return m_WavelengthCalibration != null;
        }

		public bool HasSelectedCalibrationPoints()
		{
			return m_PixelNos[0].HasValue;
		}

		public SpectraCalibration ToSpectraCalibration()
		{
			if (m_WavelengthCalibration != null)
				return m_WavelengthCalibration.GetSpectraCalibration();

			return null;
		}

        public float GetCalibrationScale()
        {
	        if (m_WavelengthCalibration != null)
		        return m_WavelengthCalibration.GetCalibrationScale();
	        else
		        return float.NaN;
        }

		public int GetCalibrationOrder()
		{
			if (m_WavelengthCalibration != null)
				return m_WavelengthCalibration.GetCalibrationOrder();
			else
				return 0;
		}

		public float GetCalibrationRMS()
		{
			if (m_WavelengthCalibration != null)
				return m_WavelengthCalibration.GetCalibrationRMS();
			else
				return float.NaN;
		}

        public void Reset()
        {
            for (int i = 0; i < 8; i++)
                m_PixelNos[i] = null;

			m_WavelengthCalibration = null;
        }

        public float ResolveWavelength(int pixelNo)
        {
			if (m_WavelengthCalibration != null)
	        {
				SpectraPoint point = m_MasterSpectra.Points.SingleOrDefault(x => x.PixelNo == pixelNo);
				return m_WavelengthCalibration.ResolveWavelength(point);
			}

            return float.NaN;
        }

        public int ResolvePixelNo(float wavelength)
        {
			if (m_WavelengthCalibration != null)
				return m_WavelengthCalibration.ResolvePixelNo(wavelength);

			return int.MinValue;
        }

        public void PopulateWaveLengths(Spectra spectra)
        {
            foreach (SpectraPoint point in spectra.Points)
            {
                point.Wavelength = ResolveWavelength(point.PixelNo);
            }
        }

		public bool LoadCalibration(SpectraCalibration calibration)
		{
			LoadPoints(calibration);

			switch (calibration.FitType)
			{
				case "LinearWavelengthCalibration":
					m_WavelengthCalibration = new LinearWavelengthCalibration(calibration);
					return true;

				case "QuadraticWavelengthCalibration":
					m_WavelengthCalibration = new QuadraticWavelengthCalibration(calibration);
					return true;

				case "LinearWavelengthRegressionCalibration":
					m_WavelengthCalibration = new LinearWavelengthRegressionCalibration(calibration);
					return true;

				case "QuadraticWavelengthRegressionCalibration":
					m_WavelengthCalibration = new QuadraticWavelengthRegressionCalibration(calibration);
					return true;

				case "CubicWavelengthRegressionCalibration":
					m_WavelengthCalibration = new CubicWavelengthRegressionCalibration(calibration);
					return true;
			}

			return false;
		}

		private void LoadPoints(SpectraCalibration props)
	    {
			int[] pixelsArr = new int[] { props.Pixel1, props.Pixel2, props.Pixel3, props.Pixel4, props.Pixel5, props.Pixel6, props.Pixel7, props.Pixel8 };
			float[] wavelengthsArr = new float[] { props.Wavelength1, props.Wavelength2, props.Wavelength3, props.Wavelength4, props.Wavelength5, props.Wavelength6, props.Wavelength7, props.Wavelength8 };

			for (int i = 0; i < 8; i++)
			{
				if (pixelsArr[i] != 0 && !float.IsNaN(wavelengthsArr[i]))
				{
					m_PixelNos[i] = pixelsArr[i];
					m_Wavelengths[i] = wavelengthsArr[i];
				}
				else
				{
					m_PixelNos[i] = null;
					m_Wavelengths[i] = float.NaN;
				}
			}
	    }
    }

    internal class QuadraticWavelengthCalibration : IWavelengthCalibration
    {
        private int m_PixelNo1;
        private int m_PixelNo2;
        private int m_PixelNo3;
        private float m_Wavelength1;
        private float m_Wavelength2;
        private float m_Wavelength3;

        private float m_A;
        private float m_B;
        private float m_C;

		public QuadraticWavelengthCalibration()
		{ }

        public void Calibrate(int x1, int x2, int x3, float w1, float w2, float w3)
        {
            m_PixelNo1 = x1;
            m_PixelNo2 = x2;
            m_PixelNo3 = x3;
            m_Wavelength1 = w1;
            m_Wavelength2 = w2;
            m_Wavelength3 = w3;

            m_C = ((x1 - x2) / (w1 - w2) - (x2 - x3) / (w2 - w3)) / (((w1 * w1 - w2 * w2) / (w1 - w2)) - ((w2 * w2 - w3 * w3) / (w2 - w3)));

            float b1 = (x1 - x2) / (w1 - w2) - m_C * (w1 * w1 - w2 * w2) / (w1 - w2);
            float b2 = (x2 - x3) / (w2 - w3) - m_C * (w2 * w2 - w3 * w3) / (w2 - w3);
            m_B = (b1 + b2) / 2f;

            float a1 = x1 - m_B * w1 - m_C * w1 * w1;
            float a2 = x2 - m_B * w2 - m_C * w2 * w2;
            float a3 = x3 - m_B * w3 - m_C * w3 * w3;
            m_A = (a1 + a2 + a3) / 3f;
        }

        public float ResolveWavelength(SpectraPoint point)
        {
            if (point != null)
                return ResolveWavelength(point.PixelNo);
            else
                return float.NaN;
        }

        public float ResolveWavelength(int x)
        {
            float d = (float)Math.Sqrt(m_B * m_B - 4 * (m_A - x) * m_C);
            float x1 = (-m_B + d) / (2 * m_C);
            float x2 = (-m_B - d) / (2 * m_C);

            if (x1 >= 0)
                return (int)Math.Round(x1);
            else
                return (int)Math.Round(x2);
        }

        public int ResolvePixelNo(float wavelength)
        {
            return (int)Math.Round(m_A + m_B * wavelength + m_C * wavelength * wavelength);
        }

        public float GetCalibrationScale()
        {
            return 1f / m_B;
        }

        public int GetCalibrationOrder()
        {
            return 2;
        }

        public float GetCalibrationRMS()
        {
            return float.NaN;
        }

        public SpectraCalibration GetSpectraCalibration()
        {
            return new SpectraCalibration()
            {
                Pixel1 = m_PixelNo1,
                Pixel2 = m_PixelNo2,
                Wavelength1 = m_Wavelength1,
                Wavelength2 = m_Wavelength2,
                Dispersion = 1 / m_B,
                ZeroPixel = m_A,
				RMS = GetCalibrationRMS(),
				PolynomialOrder = GetCalibrationOrder(),
				FitType = this.GetType().Name
			};
		}

	    public QuadraticWavelengthCalibration(SpectraCalibration props)
	    {
			Calibrate((int)props.Pixel1, (int)props.Pixel2, (int)props.Pixel3, props.Wavelength1, props.Wavelength2, props.Wavelength3);
	    }
    }

    internal class LinearWavelengthCalibration : IWavelengthCalibration
	{
		private int m_PixelNo1;
		private int m_PixelNo2;
		private float m_Wavelength1;
		private float m_Wavelength2;
		private int m_ZeroPixelNo;
		private float m_ZeroWavelength;
		private float m_AperPixels;

		public LinearWavelengthCalibration()
		{ }

		public void Calibrate(int pixel1, int pixel2, float wavelength1, float wavelength2)
		{
			m_PixelNo1 = pixel1;
			m_PixelNo2 = pixel2;
			m_Wavelength1 = wavelength1;
			m_Wavelength2 = wavelength2;

			if (pixel1 > pixel2)
			{
				m_ZeroPixelNo = pixel2;
				m_ZeroWavelength = wavelength2;
				m_AperPixels = (wavelength1 - wavelength2) / (pixel1 - pixel2);
			}
			else
			{
				m_ZeroPixelNo = pixel1;
				m_ZeroWavelength = wavelength1;
				m_AperPixels = (wavelength2 - wavelength1) / (pixel2 - pixel1);
			}
		}

		public float ResolveWavelength(SpectraPoint point)
		{			
			if (point != null)
				return m_ZeroWavelength + (point.PixelNo - m_ZeroPixelNo) * m_AperPixels;
			else
				return float.NaN;
		}

		public int ResolvePixelNo(float wavelength)
		{
			return (int)Math.Round(m_ZeroPixelNo + (wavelength - m_ZeroWavelength) / m_AperPixels); 
		}

		public float GetCalibrationScale()
		{
			return m_AperPixels;
		}

		public int GetCalibrationOrder()
		{
			return 1;
		}

		public float GetCalibrationRMS()
		{
			return float.NaN;
		}

		public SpectraCalibration GetSpectraCalibration()
		{
			return new SpectraCalibration()
			{
				Pixel1 = m_PixelNo1,
				Pixel2 = m_PixelNo2,
				Wavelength1 = m_Wavelength1,
				Wavelength2 = m_Wavelength2,
				Dispersion = m_AperPixels,
				ZeroPixel = m_ZeroPixelNo,
				RMS = GetCalibrationRMS(),
				PolynomialOrder = GetCalibrationOrder(),
				FitType = this.GetType().Name
			};
		}

	    public LinearWavelengthCalibration(SpectraCalibration props)
	    {
			Calibrate((int)props.Pixel1, (int)props.Pixel2, props.Wavelength1, props.Wavelength2);
	    }
	}

    internal abstract class RegressionCalibrationBase
    {
        protected List<int> m_PixelNos = new List<int>();
        protected List<float> m_Wavelengths = new List<float>();

        protected List<double> m_Residuals;
        protected float m_StdDev = float.NaN;
        protected float m_RMS = float.NaN;
        protected float m_ChiSquare = float.NaN;

        protected abstract float ComputePixelNo(float wavelength);

        public void AddDataPoint(int pixelNo, float wavelength)
        {
            m_PixelNos.Add(pixelNo);
            m_Wavelengths.Add(wavelength);
        }

        protected void EnsureResiduals()
        {
            if (m_Residuals == null)
            {
                m_Residuals = new List<double>();
                m_StdDev = 0;

                for (int i = 0; i < m_Wavelengths.Count; i++)
                {
                    double residual = m_PixelNos[i] - ComputePixelNo(m_Wavelengths[i]);
                    m_Residuals.Add(residual);
                    m_StdDev += (float)(residual * residual);
                }

                m_RMS = (float)Math.Sqrt(m_StdDev / m_Wavelengths.Count);
                m_StdDev = (float)Math.Sqrt(m_StdDev / (m_Wavelengths.Count - 1));

                m_ChiSquare = 0;
                double stdsq = m_StdDev * m_StdDev;
                for (int i = 0; i < m_Residuals.Count; i++)
                {
                    m_ChiSquare += (float)((m_Residuals[i] * m_Residuals[i]) / stdsq);
                }
                m_ChiSquare = m_ChiSquare / (m_Residuals.Count - 3);
            }
        }

		#region Loading and Saving of Pixels/Wavelength Arrays
		protected void LoadPixels(SpectraCalibration props)
	    {
			int[] pixelsArr = new int[] { props.Pixel1, props.Pixel2, props.Pixel3, props.Pixel4, props.Pixel5, props.Pixel6, props.Pixel7, props.Pixel8 };
			float[] wavelengthsArr = new float[] { props.Wavelength1, props.Wavelength2, props.Wavelength3, props.Wavelength4, props.Wavelength5, props.Wavelength6, props.Wavelength7, props.Wavelength8 };

		    for (int i = 0; i < 8; i++)
		    {
				if (pixelsArr[i] != 0 && !float.IsNaN(wavelengthsArr[i]))
				{
					m_PixelNos.Add(pixelsArr[i]);
					m_Wavelengths.Add(wavelengthsArr[i]);
				}    
		    }
	    }

		protected void SavePixels(SpectraCalibration props)
	    {
			if (m_PixelNos.Count > 0)
			{
				props.Pixel1 = m_PixelNos[0];
				props.Wavelength1 = m_Wavelengths[0];

				if (m_PixelNos.Count > 1)
				{
					props.Pixel2 = m_PixelNos[1];
					props.Wavelength2 = m_Wavelengths[1];

					if (m_PixelNos.Count > 2)
					{
						props.Pixel3 = m_PixelNos[2];
						props.Wavelength3 = m_Wavelengths[2];

						if (m_PixelNos.Count > 3)
						{
							props.Pixel4 = m_PixelNos[3];
							props.Wavelength4 = m_Wavelengths[3];

							if (m_PixelNos.Count > 4)
							{
								props.Pixel5 = m_PixelNos[4];
								props.Wavelength5 = m_Wavelengths[4];

								if (m_PixelNos.Count > 5)
								{
									props.Pixel6 = m_PixelNos[5];
									props.Wavelength6 = m_Wavelengths[5];

									if (m_PixelNos.Count > 6)
									{
										props.Pixel7 = m_PixelNos[6];
										props.Wavelength7 = m_Wavelengths[6];

										if (m_PixelNos.Count > 7)
										{
											props.Pixel8 = m_PixelNos[7];
											props.Wavelength8 = m_Wavelengths[7];
										}
										else
										{
											props.Pixel8 = 0;
											props.Wavelength8 = float.NaN;
										}
									}
									else
									{
										props.Pixel7 = 0;
										props.Pixel8 = 0;
										props.Wavelength7 = float.NaN;
										props.Wavelength8 = float.NaN;
									}
								}
								else
								{
									props.Pixel6 = 0;
									props.Pixel7 = 0;
									props.Pixel8 = 0;
									props.Wavelength6 = float.NaN;
									props.Wavelength7 = float.NaN;
									props.Wavelength8 = float.NaN;
								}
							}
							else
							{
								props.Pixel5 = 0;
								props.Pixel6 = 0;
								props.Pixel7 = 0;
								props.Pixel8 = 0;
								props.Wavelength5 = float.NaN;
								props.Wavelength6 = float.NaN;
								props.Wavelength7 = float.NaN;
								props.Wavelength8 = float.NaN;
							}
						}
						else
						{
							props.Pixel4 = 0;
							props.Pixel5 = 0;
							props.Pixel6 = 0;
							props.Pixel7 = 0;
							props.Pixel8 = 0;
							props.Wavelength4 = float.NaN;
							props.Wavelength5 = float.NaN;
							props.Wavelength6 = float.NaN;
							props.Wavelength7 = float.NaN;
							props.Wavelength8 = float.NaN;
						}
					}
					else
					{
						props.Pixel3 = 0;
						props.Pixel4 = 0;
						props.Pixel5 = 0;
						props.Pixel6 = 0;
						props.Pixel7 = 0;
						props.Pixel8 = 0;
						props.Wavelength3 = float.NaN;
						props.Wavelength4 = float.NaN;
						props.Wavelength5 = float.NaN;
						props.Wavelength6 = float.NaN;
						props.Wavelength7 = float.NaN;
						props.Wavelength8 = float.NaN;
					}
				}
				else
				{
					props.Pixel2 = 0;
					props.Pixel3 = 0;
					props.Pixel4 = 0;
					props.Pixel5 = 0;
					props.Pixel6 = 0;
					props.Pixel7 = 0;
					props.Pixel8 = 0;
					props.Wavelength2 = float.NaN;
					props.Wavelength3 = float.NaN;
					props.Wavelength4 = float.NaN;
					props.Wavelength5 = float.NaN;
					props.Wavelength6 = float.NaN;
					props.Wavelength7 = float.NaN;
					props.Wavelength8 = float.NaN;
				}
			}
		}
		#endregion
	}

    internal class LinearWavelengthRegressionCalibration : RegressionCalibrationBase, IRegressionWavelengthCalibration, IWavelengthCalibration
    {
        private float m_A;
        private float m_B;

		public LinearWavelengthRegressionCalibration()
		{ }

        public void Calibrate()
        {
            if (m_PixelNos.Count < 3)
                throw new InvalidOperationException("Cannot get a fit from less than 4 points.");

            var A = new SafeMatrix(m_PixelNos.Count, 2);
            var X = new SafeMatrix(m_PixelNos.Count, 1);

            for (int i = 0; i < m_PixelNos.Count; i++)
            {
                A[i, 0] = m_Wavelengths[i];
                A[i, 1] = 1;

                X[i, 0] = m_PixelNos[i];
            }

            SafeMatrix a_T = A.Transpose();
            SafeMatrix aa = a_T * A;
            SafeMatrix aa_inv = aa.Inverse();
            SafeMatrix bx = (aa_inv * a_T) * X;

            m_A = (float)bx[0, 0];
            m_B = (float)bx[1, 0];
        }

        public float ResolveWavelength(SpectraPoint point)
        {
            if (point != null)
                return ResolveWavelength(point.PixelNo);
            else
                return float.NaN;
        }

        public float ResolveWavelength(int x)
        {
            return (x - m_B) / m_A;
        }

        public int ResolvePixelNo(float wavelength)
        {
            return (int)Math.Round(ComputePixelNo(wavelength));
        }

        protected override float ComputePixelNo(float wavelength)
        {
            return m_A * wavelength + m_B;
        }

        public float GetCalibrationScale()
        {
            return 1f / m_A;
        }

        public int GetCalibrationOrder()
        {
            return 2;
        }

        public float GetCalibrationRMS()
        {
            EnsureResiduals();
            return m_RMS;
        }

        public SpectraCalibration GetSpectraCalibration()
        {
            var rv =  new SpectraCalibration()
            {
                Dispersion = 1 / m_A,
                ZeroPixel = m_B,
				RMS = GetCalibrationRMS(),
				PolynomialOrder = GetCalibrationOrder(),
				FitType = this.GetType().Name,
				A = m_A,
				B = m_B
			};

			base.SavePixels(rv);

			return rv;
		}

		public LinearWavelengthRegressionCalibration(SpectraCalibration props)
		{
			LoadPixels(props);
			m_A = props.A;
			m_B = props.B;
			EnsureResiduals();
			m_RMS = props.RMS;
		}
    }

    internal class CubicWavelengthRegressionCalibration : RegressionCalibrationBase, IRegressionWavelengthCalibration, IWavelengthCalibration
    {
        private float m_A;
        private float m_B;
        private float m_C;
        private float m_D;

		public CubicWavelengthRegressionCalibration()
		{ }

        public void Calibrate()
        {
            if (m_PixelNos.Count < 5)
                throw new InvalidOperationException("Cannot get a fit from less than 4 points.");

            var A = new SafeMatrix(m_PixelNos.Count, 4);
            var X = new SafeMatrix(m_PixelNos.Count, 1);

            for (int i = 0; i < m_PixelNos.Count; i++)
            {
                A[i, 0] = m_Wavelengths[i] * m_Wavelengths[i] * m_Wavelengths[i];
                A[i, 1] = m_Wavelengths[i] * m_Wavelengths[i];
                A[i, 2] = m_Wavelengths[i];
                A[i, 3] = 1;

                X[i, 0] = m_PixelNos[i];
            }

            SafeMatrix a_T = A.Transpose();
            SafeMatrix aa = a_T * A;
            SafeMatrix aa_inv = aa.Inverse();
            SafeMatrix bx = (aa_inv * a_T) * X;

            m_A = (float)bx[0, 0];
            m_B = (float)bx[1, 0];
            m_C = (float)bx[2, 0];
            m_D = (float)bx[3, 0];
        }

        public float ResolveWavelength(SpectraPoint point)
        {
            if (point != null)
                return ResolveWavelength(point.PixelNo);
            else
                return float.NaN;
        }

        public float ResolveWavelength(int x)
        {
            float a = m_B / m_A;
            float b = m_C / m_A;
            float c = (m_D - x) / m_A;

            float Q = (a * a - 3 * b) / 9;
            float R = (2 * a * a * a - 9 * a * b + 27 * c) / 54;


            if (R * R < Q * Q * Q)
            {
                float tita = (float)Math.Acos(R / Math.Sqrt(Q * Q * Q));
                float rQ = (float)Math.Sqrt(Q);

                float[] warr = new float[3];
                warr[0] = (float)(-2 * rQ * Math.Cos(tita / 3) - a / 3);
                warr[1] = (float)(-2 * rQ * Math.Cos((tita + 2 * Math.PI) / 3) - a / 3);
                warr[2] = (float)(-2 * rQ * Math.Cos((tita - 2 * Math.PI) / 3) - a / 3);

                float[] warr_abs = new float[3];
                warr_abs[0] = Math.Abs(warr[0]);
                warr_abs[1] = Math.Abs(warr[1]);
                warr_abs[2] = Math.Abs(warr[2]);

                Array.Sort(warr_abs, warr);

                return warr[0];
            }

            return float.NaN;
        }

        public int ResolvePixelNo(float wavelength)
        {
            return (int)Math.Round(ComputePixelNo(wavelength));
        }

        protected override float ComputePixelNo(float wavelength)
        {
            return m_A * wavelength * wavelength * wavelength + m_B * wavelength * wavelength + m_C * wavelength + m_D;
        }

        public float GetCalibrationScale()
        {
            return 1f / m_C;
        }

        public int GetCalibrationOrder()
        {
            return 3;
        }

        public float GetCalibrationRMS()
        {
            EnsureResiduals();
            return m_RMS;
        }

        public SpectraCalibration GetSpectraCalibration()
        {
            var rv = new SpectraCalibration()
            {
                Dispersion = 1 / m_C,
                ZeroPixel = m_D,
				RMS = GetCalibrationRMS(),
				PolynomialOrder = GetCalibrationOrder(),
				FitType = this.GetType().Name,
				A = m_A,
				B = m_B,
				C = m_C,
				D = m_D
			};

			base.SavePixels(rv);

	        return rv;
        }

		public CubicWavelengthRegressionCalibration(SpectraCalibration props)
		{
			LoadPixels(props);
			m_A = props.A;
			m_B = props.B;
			m_C = props.C;
			m_D = props.D;
			EnsureResiduals();
			m_RMS = props.RMS;
		}
    }

    internal class QuadraticWavelengthRegressionCalibration : RegressionCalibrationBase, IRegressionWavelengthCalibration, IWavelengthCalibration
    {
        private float m_A;
        private float m_B;
        private float m_C;

		public QuadraticWavelengthRegressionCalibration()
		{ }

        public void Calibrate()
        {
            if (m_PixelNos.Count < 4)
                throw new InvalidOperationException("Cannot get a fit from less than 4 points.");

            var A = new SafeMatrix(m_PixelNos.Count, 3);
            var X = new SafeMatrix(m_PixelNos.Count, 1);

            for (int i = 0; i < m_PixelNos.Count; i++)
            {
                A[i, 0] = m_Wavelengths[i] * m_Wavelengths[i];
                A[i, 1] = m_Wavelengths[i];
                A[i, 2] = 1;

                X[i, 0] = m_PixelNos[i];
            }

            SafeMatrix a_T = A.Transpose();
            SafeMatrix aa = a_T * A;
            SafeMatrix aa_inv = aa.Inverse();
            SafeMatrix bx = (aa_inv * a_T) * X;

            m_A = (float)bx[0, 0];
            m_B = (float)bx[1, 0];
            m_C = (float)bx[2, 0];   
        }

        public float ResolveWavelength(SpectraPoint point)
        {
            if (point != null)
                return ResolveWavelength(point.PixelNo);
            else
                return float.NaN;
        }

        public float ResolveWavelength(int x)
        {
            float d = (float)Math.Sqrt(m_B * m_B - 4 * (m_C - x) * m_A);
            float x1 = (-m_B + d) / (2 * m_A);
            float x2 = (-m_B - d) / (2 * m_A);

            if (x1 >= 0)
                return (int)Math.Round(x1);
            else
                return (int)Math.Round(x2);
        }

        public int ResolvePixelNo(float wavelength)
        {
            return (int)Math.Round(ComputePixelNo(wavelength));
        }

        protected override float ComputePixelNo(float wavelength)
        {
            return m_A * wavelength * wavelength + m_B * wavelength + m_C;
        }

        public float GetCalibrationScale()
        {
            return 1f / m_B;
        }

        public int GetCalibrationOrder()
        {
            return 2;
        }

        public float GetCalibrationRMS()
        {
            EnsureResiduals();
            return m_RMS;
        }

        public SpectraCalibration GetSpectraCalibration()
        {
            var rv = new SpectraCalibration()
            {
                Dispersion = 1 / m_B,
                ZeroPixel = m_C,
				RMS = GetCalibrationRMS(),
				PolynomialOrder = GetCalibrationOrder(),
				FitType = this.GetType().Name,
				A = m_A,
				B = m_B,
				C = m_C
			};

			base.SavePixels(rv);

	        return rv;
        }

		public QuadraticWavelengthRegressionCalibration(SpectraCalibration props)
		{
			LoadPixels(props);
			m_A = props.A;
			m_B = props.B;
			m_C = props.C;
			EnsureResiduals();
			m_RMS = props.RMS;
		}
    }
}
