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

	public class SpectraCalibrator
    {
	    private MasterSpectra m_MasterSpectra;

        //private int? m_PixelNo1;
        //private float m_Wavelength1;

        //private int? m_PixelNo2;
        //private float m_Wavelength2;

        //private int? m_PixelNo3;
        //private float m_Wavelength3;

        //private int? m_PixelNo4;
        //private float m_Wavelength4;

        //private int? m_PixelNo5;
        //private float m_Wavelength5;

        //private int? m_PixelNo6;
        //private float m_Wavelength6;

        //private int? m_PixelNo7;
        //private float m_Wavelength7;

        //private int? m_PixelNo8;
        //private float m_Wavelength8;

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
            if (!m_PixelNos[0].HasValue)
            {
                m_PixelNos[0] = pixelNo;
                m_Wavelengths[0] = wavelength;
            }
            else if (!m_PixelNos[1].HasValue && !IsRepeatedLine(pixelNo, wavelength))
            {
                m_PixelNos[1] = pixelNo;
                m_Wavelengths[1] = wavelength;
            }
            else if (!m_PixelNos[2].HasValue && !IsRepeatedLine(pixelNo, wavelength))
			{
                m_PixelNos[2] = pixelNo;
                m_Wavelengths[2] = wavelength;
			}
            else if (!m_PixelNos[3].HasValue && !IsRepeatedLine(pixelNo, wavelength))
			{
                m_PixelNos[3] = pixelNo;
                m_Wavelengths[3] = wavelength;
			}
            else if (!m_PixelNos[4].HasValue && !IsRepeatedLine(pixelNo, wavelength))
			{
                m_PixelNos[4] = pixelNo;
                m_Wavelengths[4] = wavelength;
			}
            else if (!m_PixelNos[5].HasValue && !IsRepeatedLine(pixelNo, wavelength))
			{
                m_PixelNos[5] = pixelNo;
                m_Wavelengths[5] = wavelength;
			}
            else if (!m_PixelNos[6].HasValue && !IsRepeatedLine(pixelNo, wavelength))
			{
                m_PixelNos[6] = pixelNo;
                m_Wavelengths[6] = wavelength;
			}
            else if (!m_PixelNos[7].HasValue && !IsRepeatedLine(pixelNo, wavelength))
			{
                m_PixelNos[7] = pixelNo;
                m_Wavelengths[7] = wavelength;
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

        public void SetDispersion(float dispersion)
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
                    var calibration = new QuadraticWavelengthRegressionCalibration();
                    for (int i = 0; i < 8; i++)
                    {
                        if (m_PixelNos[i].HasValue)
                            calibration.AddDataPoint(m_PixelNos[i].Value, m_Wavelengths[i]);
                    }
                    calibration.Calibrate();

                    m_WavelengthCalibration = calibration;
                }
            }
			else
				MessageBox.Show("Calibration of this order hasn't been implemented yet");
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
    }

    public class QuadraticWavelengthCalibration : IWavelengthCalibration
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

        public void Calibrate(int x1, int x2, int x3, float w1, float w2, float w3)
        {
            m_PixelNo1 = x1;
            m_PixelNo2 = x2;
            m_PixelNo3 = x3;
            m_Wavelength1 = w1;
            m_Wavelength2 = w2;
            m_Wavelength3 = w3;


            // x1 = a + b*w1 + c*w1*w1
            // x2 = a + b*w2 + c*w2*w2
            // x3 = a + b*w3 + c*w3*w3

            // x1-x2 = b (w1 - w2) + c (w1*w1 - w2*w2)
            // x2-x3 = b (w2 - w3) + c (w2*w2 - w3*w3)

            //(x1-x2) / (w1 - w2) = b + c (w1*w1 - w2*w2) / (w1 - w2)
            //(x2-x3) / (w2 - w3) = b + c (w2*w2 - w3*w3) / (w2 - w3)

            //(x1-x2) / (w1 - w2) - (x2-x3) / (w2 - w3) = c[ ((w1*w1 - w2*w2) / (w1 - w2)) - ((w2*w2 - w3*w3) / (w2 - w3)) ]
            // c = [(x1-x2) / (w1 - w2) - (x2-x3) / (w2 - w3)] / [ ((w1*w1 - w2*w2) / (w1 - w2)) - ((w2*w2 - w3*w3) / (w2 - w3)) ]

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
                ZeroPixel = m_A
            };
        }
    }

    public class LinearWavelengthCalibration : IWavelengthCalibration
	{
		private int m_PixelNo1;
		private int m_PixelNo2;
		private float m_Wavelength1;
		private float m_Wavelength2;
		private int m_ZeroPixelNo;
		private float m_ZeroWavelength;
		private float m_AperPixels;

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
				ZeroPixel = m_ZeroPixelNo
			};
		}
	}

    public class QuadraticWavelengthRegressionCalibration : IWavelengthCalibration
    {
        private List<int> m_PixelNos = new List<int>();
        private List<float> m_Wavelengths = new List<float>();

        private float m_A;
        private float m_B;
        private float m_C;

        public void AddDataPoint(int pixelNo, float wavelength)
        {
            m_PixelNos.Add(pixelNo);
            m_Wavelengths.Add(wavelength);
        }

        public void Calibrate()
        {
            if (m_PixelNos.Count < 4)
                throw new InvalidOperationException("Cannot get a fit from less than 4 points.");

            SafeMatrix A = new SafeMatrix(m_PixelNos.Count, 3);
            SafeMatrix X = new SafeMatrix(m_PixelNos.Count, 1);

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

        private float ComputePixelNo(float wavelength)
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
            return m_RMS / m_B;
        }

        private List<double> m_Residuals;
        private float m_StdDev = float.NaN;
        private float m_RMS = float.NaN;
        private float m_ChiSquare = float.NaN;

        private void EnsureResiduals()
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

        public SpectraCalibration GetSpectraCalibration()
        {
            return new SpectraCalibration()
            {
                Pixel1 = m_PixelNos[0],
                Pixel2 = m_PixelNos[1],
                Wavelength1 = m_Wavelengths[0],
                Wavelength2 = m_Wavelengths[1],
                Dispersion = 1 / m_B,
                ZeroPixel = m_C
            };
        }
    }
}
