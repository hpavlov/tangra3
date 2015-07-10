using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Model.Config;
using Tangra.Model.Helpers;
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
        void AddDataPoint(float pointNo, float wavelength);
    }

    public class SpectraCalibrator
    {
	    private MasterSpectra m_MasterSpectra;

	    private float[] m_PixelPos = new float[8];
        private float[] m_Wavelengths = new float[8];

		private IWavelengthCalibration m_WavelengthCalibration;

	    public SpectraCalibrator(MasterSpectra masterSpectra)
	    {
	        m_MasterSpectra = masterSpectra;
			m_WavelengthCalibration = null;
		    Reset();
	    }

		public void SetMarker(float pixelPos, float wavelength, bool attemptCalibration, int polynomialOrder)
        {
	        for (int i = 0; i < 8; i++)
	        {
				if (float.IsNaN(m_PixelPos[i]))
	            {
					if (!IsRepeatedLine(pixelPos, wavelength))
	                {
						m_PixelPos[i] = pixelPos;
                        m_Wavelengths[i] = wavelength;
	                }

	                break;
	            }
	        }

			if (attemptCalibration)
				Calibrate(polynomialOrder);
        }

		private bool IsRepeatedLine(float pixelPos, float wavelength)
		{
		    for (int i = 0; i < 8; i++)
		    {
				if (!float.IsNaN(m_PixelPos[i]) && (Math.Abs(m_PixelPos[i] - pixelPos) <= 1 || Math.Abs(m_Wavelengths[i] - wavelength) < 0.1))
                    return true;
		    }

			return false;
		}

        public void SetFirstOrderDispersion(float dispersion)
        {
			if (!float.IsNaN(m_PixelPos[0]))
            {
				m_PixelPos[1] = m_PixelPos[0] + 1000;
                m_Wavelengths[1] = dispersion * 1000;

                Calibrate(1);
            }
        }

		private void Calibrate(int polynomialOrder)
        {
			if (polynomialOrder == 1)
			{
				if (!float.IsNaN(m_PixelPos[0]) && !float.IsNaN(m_PixelPos[1]) && float.IsNaN(m_PixelPos[2]))
                {
                    var calibration = new LinearWavelengthCalibration();
					calibration.Calibrate(m_PixelPos[0], m_PixelPos[1], m_Wavelengths[0], m_Wavelengths[1]);

                    m_WavelengthCalibration = calibration;
                }
				else if (!float.IsNaN(m_PixelPos[0]) && !float.IsNaN(m_PixelPos[1]) && !float.IsNaN(m_PixelPos[2]))
                {
                    CalibrateWithRegression<LinearWavelengthRegressionCalibration>();
                }
			}
            else if (polynomialOrder == 2)
            {
				if (!float.IsNaN(m_PixelPos[0]) && !float.IsNaN(m_PixelPos[1]) && !float.IsNaN(m_PixelPos[2]) && float.IsNaN(m_PixelPos[3]))
                {
                    var calibration = new QuadraticWavelengthCalibration();
					calibration.Calibrate(m_PixelPos[0], m_PixelPos[1], m_PixelPos[2], m_Wavelengths[0], m_Wavelengths[1], m_Wavelengths[2]);

                    m_WavelengthCalibration = calibration;
                }
				else if (!float.IsNaN(m_PixelPos[0]) && !float.IsNaN(m_PixelPos[1]) && !float.IsNaN(m_PixelPos[2]) && !float.IsNaN(m_PixelPos[3]))
                {
                    CalibrateWithRegression<QuadraticWavelengthRegressionCalibration>();
                }
            }
            else if (polynomialOrder == 3)
            {
				if (!float.IsNaN(m_PixelPos[0]) && !float.IsNaN(m_PixelPos[1]) && !float.IsNaN(m_PixelPos[2]) && !float.IsNaN(m_PixelPos[3]) && float.IsNaN(m_PixelPos[4]))
                {
                    MessageBox.Show("At least 5 poins are required for a 3-rd order fit.", "Tangra", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
				else if (!float.IsNaN(m_PixelPos[0]) && !float.IsNaN(m_PixelPos[1]) && !float.IsNaN(m_PixelPos[2]) && !float.IsNaN(m_PixelPos[3]) && !float.IsNaN(m_PixelPos[4]))
                {
                    CalibrateWithRegression<CubicWavelengthRegressionCalibration>();
                }
            }
			else if (polynomialOrder == 4)
			{
				if (!float.IsNaN(m_PixelPos[0]) && !float.IsNaN(m_PixelPos[1]) && !float.IsNaN(m_PixelPos[2]) && !float.IsNaN(m_PixelPos[3]) && !float.IsNaN(m_PixelPos[4]) && float.IsNaN(m_PixelPos[5]))
				{
					MessageBox.Show("At least 6 poins are required for a 4-rd order fit.", "Tangra", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
				else if (!float.IsNaN(m_PixelPos[0]) && !float.IsNaN(m_PixelPos[1]) && !float.IsNaN(m_PixelPos[2]) && !float.IsNaN(m_PixelPos[3]) && !float.IsNaN(m_PixelPos[4]) && !float.IsNaN(m_PixelPos[5]))
				{
					CalibrateWithRegression<QuarticWavelengthRegressionCalibration>();
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
				if (!float.IsNaN(m_PixelPos[i]))
					calibration.AddDataPoint(m_PixelPos[i], m_Wavelengths[i]);
            }

	        try
	        {
				calibration.Calibrate();
	        }
	        catch (Exception ex)
	        {
		        Trace.WriteLine(ex.GetFullStackTrace());
				MessageBox.Show("Calibration failed. Try a lower order polynomial.", "Tangra", MessageBoxButtons.OK, MessageBoxIcon.Error);
		        return;
	        }

			m_WavelengthCalibration = calibration;
        }

        public bool IsCalibrated()
        {
			return m_WavelengthCalibration != null;
        }

		public bool HasSelectedCalibrationPoints()
		{
			return !float.IsNaN(m_PixelPos[0]);
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
                m_PixelPos[i] = float.NaN;

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

        public bool LoadCalibration(TangraConfig.PersistedConfiguration configuration)
        {
            float maxVal = m_MasterSpectra.Points.Max(x => x.RawValue);
            SpectraPoint peakPoint = m_MasterSpectra.Points.SingleOrDefault(x => Math.Abs(maxVal - x.RawValue) < 0.00001);
            if (peakPoint != null)
            {
                switch (configuration.Order)
                {
                    case 1:
                        m_WavelengthCalibration = new LinearWavelengthCalibration(configuration.A, peakPoint.PixelNo, configuration.RMS);
                        return true;

                    case 2:
                        m_WavelengthCalibration = new QuadraticWavelengthCalibration(configuration.A, configuration.B, peakPoint.PixelNo, configuration.RMS);
                        return true;

                    case 3:
                        m_WavelengthCalibration = new CubicWavelengthCalibration(configuration.A, configuration.B, configuration.C, peakPoint.PixelNo, configuration.RMS);
                        return true;
                }                
            }

            return false;
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

				case "QuarticWavelengthRegressionCalibration":
					m_WavelengthCalibration = new QuarticWavelengthRegressionCalibration(calibration);
					return true;					

                case "CubicWavelengthCalibration":
                    m_WavelengthCalibration = new CubicWavelengthCalibration(calibration);
                    return true;
			}

			return false;
		}

		private void LoadPoints(SpectraCalibration props)
	    {
			float[] pixelsArr = new float[] { props.Pixel1, props.Pixel2, props.Pixel3, props.Pixel4, props.Pixel5, props.Pixel6, props.Pixel7, props.Pixel8 };
			float[] wavelengthsArr = new float[] { props.Wavelength1, props.Wavelength2, props.Wavelength3, props.Wavelength4, props.Wavelength5, props.Wavelength6, props.Wavelength7, props.Wavelength8 };

			for (int i = 0; i < 8; i++)
			{
				if (!float.IsNaN(pixelsArr[i]) && !float.IsNaN(wavelengthsArr[i]))
				{
					m_PixelPos[i] = pixelsArr[i];
					m_Wavelengths[i] = wavelengthsArr[i];
				}
				else
				{
					m_PixelPos[i] = float.NaN;
					m_Wavelengths[i] = float.NaN;
				}
			}
	    }
    }

    internal class CubicWavelengthCalibration : IWavelengthCalibration
    {
        private float m_A;
        private float m_B;
        private float m_C;
        private float m_D;
        private float m_RMS;

        public CubicWavelengthCalibration()
		{ }

        public CubicWavelengthCalibration(SpectraCalibration calibration)
            : this(calibration.A, calibration.B, calibration.C, calibration.D, calibration.RMS)
        { }

        public CubicWavelengthCalibration(float a, float b, float c, float d, float rms)
        {
            m_A = a;
            m_B = b;
            m_C = c;
            m_D = d;
            m_RMS = rms;
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

        protected float ComputePixelNo(float wavelength)
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
            return m_RMS;
        }

        public SpectraCalibration GetSpectraCalibration()
        {
            return new SpectraCalibration()
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
        }
    }

    internal class QuadraticWavelengthCalibration : IWavelengthCalibration
    {
		private float m_PixelNo1;
		private float m_PixelNo2;
		private float m_PixelNo3;
        private float m_Wavelength1;
        private float m_Wavelength2;
        private float m_Wavelength3;

        private float m_A;
        private float m_B;
        private float m_C;

		public QuadraticWavelengthCalibration()
		{ }

		public void Calibrate(float x1, float x2, float x3, float w1, float w2, float w3)
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

        public QuadraticWavelengthCalibration(float a, float b, float c, float rms)
        {
            m_A = c;
            m_B = b;
            m_C = a;
        }
    }

    internal class LinearWavelengthCalibration : IWavelengthCalibration
	{
		private float m_PixelNo1;
		private float m_PixelNo2;
		private float m_Wavelength1;
		private float m_Wavelength2;
		private float m_ZeroPixelNo;
		private float m_ZeroWavelength;
		private float m_AperPixels;

		public LinearWavelengthCalibration()
		{ }

        public LinearWavelengthCalibration(float a, float b, float rms)
        {
            m_ZeroPixelNo = (int)Math.Round(b);

            m_Wavelength1 = m_ZeroWavelength = 0;
            m_PixelNo1 = m_ZeroPixelNo;
            
            m_Wavelength2 = 5000;
            m_PixelNo2 = (int)Math.Round(m_Wavelength2 * a + b);

            m_AperPixels = (m_Wavelength2 - m_Wavelength1) / (m_PixelNo2 - m_PixelNo1);
        }

		public void Calibrate(float pixel1, float pixel2, float wavelength1, float wavelength2)
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
				FitType = this.GetType().Name,
                A = 1 / m_AperPixels,
                B = m_ZeroPixelNo
			};
		}

	    public LinearWavelengthCalibration(SpectraCalibration props)
	    {
			Calibrate((int)props.Pixel1, (int)props.Pixel2, props.Wavelength1, props.Wavelength2);
	    }
	}

    internal abstract class RegressionCalibrationBase
    {
		protected List<float> m_PixelPos = new List<float>();
        protected List<float> m_Wavelengths = new List<float>();

        protected List<double> m_Residuals;
        protected float m_StdDev = float.NaN;
        protected float m_RMS = float.NaN;
        protected float m_ChiSquare = float.NaN;

        protected abstract float ComputePixelNo(float wavelength);

        public void AddDataPoint(float pixelNo, float wavelength)
        {
			m_PixelPos.Add(pixelNo);
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
					double residual = m_PixelPos[i] - ComputePixelNo(m_Wavelengths[i]);
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

        protected float CalculateDispersion()
        {
            float pixel0 = ComputePixelNo(0);
            float pixel10000 = ComputePixelNo(10000);
            return 10000.0f / (pixel10000 - pixel0);
        }

		#region Loading and Saving of Pixels/Wavelength Arrays
		protected void LoadPixels(SpectraCalibration props)
	    {
			float[] pixelsArr = new float[] { props.Pixel1, props.Pixel2, props.Pixel3, props.Pixel4, props.Pixel5, props.Pixel6, props.Pixel7, props.Pixel8 };
			float[] wavelengthsArr = new float[] { props.Wavelength1, props.Wavelength2, props.Wavelength3, props.Wavelength4, props.Wavelength5, props.Wavelength6, props.Wavelength7, props.Wavelength8 };

		    for (int i = 0; i < 8; i++)
		    {
				if (!float.IsNaN(pixelsArr[i]) && !float.IsNaN(wavelengthsArr[i]))
				{
					m_PixelPos.Add(pixelsArr[i]);
					m_Wavelengths.Add(wavelengthsArr[i]);
				}    
		    }
	    }

		protected void SavePixels(SpectraCalibration props)
	    {
			if (m_PixelPos.Count > 0)
			{
				props.Pixel1 = m_PixelPos[0];
				props.Wavelength1 = m_Wavelengths[0];

				if (m_PixelPos.Count > 1)
				{
					props.Pixel2 = m_PixelPos[1];
					props.Wavelength2 = m_Wavelengths[1];

					if (m_PixelPos.Count > 2)
					{
						props.Pixel3 = m_PixelPos[2];
						props.Wavelength3 = m_Wavelengths[2];

						if (m_PixelPos.Count > 3)
						{
							props.Pixel4 = m_PixelPos[3];
							props.Wavelength4 = m_Wavelengths[3];

							if (m_PixelPos.Count > 4)
							{
								props.Pixel5 = m_PixelPos[4];
								props.Wavelength5 = m_Wavelengths[4];

								if (m_PixelPos.Count > 5)
								{
									props.Pixel6 = m_PixelPos[5];
									props.Wavelength6 = m_Wavelengths[5];

									if (m_PixelPos.Count > 6)
									{
										props.Pixel7 = m_PixelPos[6];
										props.Wavelength7 = m_Wavelengths[6];

										if (m_PixelPos.Count > 7)
										{
											props.Pixel8 = m_PixelPos[7];
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
			if (m_PixelPos.Count < 3)
                throw new InvalidOperationException("Cannot get a fit from less than 3 points.");

			var A = new SafeMatrix(m_PixelPos.Count, 2);
			var X = new SafeMatrix(m_PixelPos.Count, 1);

			for (int i = 0; i < m_PixelPos.Count; i++)
            {
                A[i, 0] = m_Wavelengths[i];
                A[i, 1] = 1;

				X[i, 0] = m_PixelPos[i];
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
                Dispersion = CalculateDispersion(),
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

	internal class QuarticWavelengthRegressionCalibration : RegressionCalibrationBase, IRegressionWavelengthCalibration, IWavelengthCalibration
	{
		private float m_A;
		private float m_B;
		private float m_C;
		private float m_D;
		private float m_E;

		public QuarticWavelengthRegressionCalibration()
		{ }

		public void Calibrate()
		{
			if (m_PixelPos.Count < 6)
				throw new InvalidOperationException("Cannot get a fit from less than 6 points.");

			var A = new SafeMatrix(m_PixelPos.Count, 6);
			var X = new SafeMatrix(m_PixelPos.Count, 1);

			for (int i = 0; i < m_PixelPos.Count; i++)
			{
				A[i, 0] = m_Wavelengths[i] * m_Wavelengths[i] * m_Wavelengths[i] * m_Wavelengths[i];
				A[i, 1] = m_Wavelengths[i] * m_Wavelengths[i] * m_Wavelengths[i];
				A[i, 2] = m_Wavelengths[i] * m_Wavelengths[i];
				A[i, 3] = m_Wavelengths[i];
				A[i, 4] = 1;

				X[i, 0] = m_PixelPos[i];
			}

			SafeMatrix a_T = A.Transpose();
			SafeMatrix aa = a_T * A;
			SafeMatrix aa_inv = aa.Inverse();
			SafeMatrix bx = (aa_inv * a_T) * X;

			m_A = (float)bx[0, 0];
			m_B = (float)bx[1, 0];
			m_C = (float)bx[2, 0];
			m_D = (float)bx[3, 0];
			m_E = (float)bx[4, 0];
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
			float p = (8 * m_A * m_C - 3 * m_B * m_B) / (8 * m_A* m_A);
			float q = (m_B * m_B * m_B - 4 * m_A * m_B * m_C + 8 * m_A * m_A * m_D) / (8 * m_A * m_A * m_A);
			float D0 = m_C * m_C - 3 * m_B * m_D + 12 * m_A * (m_E - x);
			float D1 = 2 * m_C * m_C * m_C - 9 * m_B * m_C * m_D + 27 * m_B * m_B * (m_E - x) + 27 * m_A * m_D * m_D - 72 * m_A * m_C * (m_E - x);

			float Q = (float)Math.Pow((D1 + Math.Sqrt(D1 * D1 - 4 * D0 * D0 * D0)) / 2.0f, 1 / 3.0f);
			float S = (float)(0.5f * Math.Sqrt(-2 * p / 3 + 1 / (3 * m_A * (Q + D0 / Q))));
 
			float D = -4 * S * S - 2 * p - q / S;

			if (D > 0)
			{
				float[] warr = new float[4];

				warr[0] = (float)(-m_B / (4 * m_A) - S + 0.5 * Math.Sqrt(D));
				warr[1] = (float)(-m_B / (4 * m_A) - S - 0.5 * Math.Sqrt(D));
				warr[3] = (float)(-m_B / (4 * m_A) + S + 0.5 * Math.Sqrt(D));
				warr[4] = (float)(-m_B / (4 * m_A) + S - 0.5 * Math.Sqrt(D));

				float[] warr_abs = new float[4];
				warr_abs[0] = Math.Abs(warr[0]);
				warr_abs[1] = Math.Abs(warr[1]);
				warr_abs[2] = Math.Abs(warr[2]);
				warr_abs[4] = Math.Abs(warr[4]);

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
			return m_A * wavelength * wavelength * wavelength * wavelength + m_B * wavelength * wavelength * wavelength + m_C * wavelength * wavelength + m_D * wavelength + m_E;
		}

		public float GetCalibrationScale()
		{
			return 1f / m_D;
		}

		public int GetCalibrationOrder()
		{
			return 4;
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
				Dispersion = CalculateDispersion(),
				ZeroPixel = m_E,
				RMS = GetCalibrationRMS(),
				PolynomialOrder = GetCalibrationOrder(),
				FitType = this.GetType().Name,
				A = m_A,
				B = m_B,
				C = m_C,
				D = m_D,
				E = m_E
			};

			base.SavePixels(rv);

			return rv;
		}

		public QuarticWavelengthRegressionCalibration(SpectraCalibration props)
		{
			LoadPixels(props);
			m_A = props.A;
			m_B = props.B;
			m_C = props.C;
			m_D = props.D;
			m_E = props.E;
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
			if (m_PixelPos.Count < 5)
                throw new InvalidOperationException("Cannot get a fit from less than 5 points.");

			var A = new SafeMatrix(m_PixelPos.Count, 4);
			var X = new SafeMatrix(m_PixelPos.Count, 1);

			for (int i = 0; i < m_PixelPos.Count; i++)
            {
                A[i, 0] = m_Wavelengths[i] * m_Wavelengths[i] * m_Wavelengths[i];
                A[i, 1] = m_Wavelengths[i] * m_Wavelengths[i];
                A[i, 2] = m_Wavelengths[i];
                A[i, 3] = 1;

				X[i, 0] = m_PixelPos[i];
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
                Dispersion = CalculateDispersion(),
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
			if (m_PixelPos.Count < 4)
                throw new InvalidOperationException("Cannot get a fit from less than 4 points.");

			var A = new SafeMatrix(m_PixelPos.Count, 3);
			var X = new SafeMatrix(m_PixelPos.Count, 1);

			for (int i = 0; i < m_PixelPos.Count; i++)
            {
                A[i, 0] = m_Wavelengths[i] * m_Wavelengths[i];
                A[i, 1] = m_Wavelengths[i];
                A[i, 2] = 1;

				X[i, 0] = m_PixelPos[i];
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
                Dispersion = CalculateDispersion(),
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

	internal class MinimaFinder : RegressionCalibrationBase
	{
		private float m_A;
		private float m_B;
		private float m_C;

		public MinimaFinder()
		{ }

		public void Calibrate()
		{
			if (m_PixelPos.Count < 5)
				throw new InvalidOperationException("Cannot get a fit from less than 5 points.");

			var A = new SafeMatrix(m_PixelPos.Count, 3);
			var X = new SafeMatrix(m_PixelPos.Count, 1);

			for (int i = 0; i < m_PixelPos.Count; i++)
			{
				A[i, 0] = m_Wavelengths[i] * m_Wavelengths[i];
				A[i, 1] = m_Wavelengths[i];
				A[i, 2] = 1;

				X[i, 0] = m_PixelPos[i];
			}

			SafeMatrix a_T = A.Transpose();
			SafeMatrix aa = a_T * A;
			SafeMatrix aa_inv = aa.Inverse();
			SafeMatrix bx = (aa_inv * a_T) * X;

			m_A = (float)bx[0, 0];
			m_B = (float)bx[1, 0];
			m_C = (float)bx[2, 0];

			EnsureResiduals();
		}

		public float GetMinimaCloseTo(int x0)
		{
			return (float)-m_B / (2 * m_A);
			//// Two minimas are found by solving the first derivative quadratic equation
			//// y(x) = A * X^3 + B * X^2 + C * X + D   => y'(x) = (3*A) * X^2 + (2*B) * X + C => a = 3 * A; b = 2 * B; c = C
			//double discr = 4 * m_B * m_B - 4 * 3 * m_A * m_C;
			//if (discr > 0)
			//{
			//	double xx1 = (-m_B + Math.Sqrt(discr)) / (2 * 3 * m_A);
			//	double xx2 = (-m_B - Math.Sqrt(discr)) / (2 * 3 * m_A);

			//	if (Math.Abs(xx1 - x0) < Math.Abs(xx2 - x0))
			//		return (float)xx1;
			//	else
			//		return (float)xx2;
			//}

			//return float.NaN;
		}

		protected override float ComputePixelNo(float wavelength)
		{
			return m_A * wavelength * wavelength + m_B * wavelength + m_C;
		}
	}
}
