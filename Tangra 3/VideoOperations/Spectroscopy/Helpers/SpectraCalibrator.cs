using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

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

	    private int? m_PixelNo1;
	    private float m_Wavelength1;

        private int? m_PixelNo2;
        private float m_Wavelength2;

		private int? m_PixelNo3;
		private float m_Wavelength3;

		private int? m_PixelNo4;
		private float m_Wavelength4;

		private int? m_PixelNo5;
		private float m_Wavelength5;

		private int? m_PixelNo6;
		private float m_Wavelength6;

		private int? m_PixelNo7;
		private float m_Wavelength7;

		private int? m_PixelNo8;
		private float m_Wavelength8;

		private IWavelengthCalibration m_WavelengthCalibration;

	    public SpectraCalibrator(MasterSpectra masterSpectra)
	    {
	        m_MasterSpectra = masterSpectra;
			m_WavelengthCalibration = null;
	    }

	    public void SetMarker(int pixelNo, float wavelength, bool attemptCalibration, int polynomialOrder)
        {
            if (!m_PixelNo1.HasValue)
            {
                m_PixelNo1 = pixelNo;
                m_Wavelength1 = wavelength;
            }
            else if (!m_PixelNo2.HasValue && !IsRepeatedLine(pixelNo, wavelength, 2))
            {
                m_PixelNo2 = pixelNo;
                m_Wavelength2 = wavelength;
            }
			else if (!m_PixelNo3.HasValue && !IsRepeatedLine(pixelNo, wavelength, 3))
			{
				m_PixelNo3 = pixelNo;
				m_Wavelength3 = wavelength;
			}
			else if (!m_PixelNo4.HasValue && !IsRepeatedLine(pixelNo, wavelength, 4))
			{
				m_PixelNo4 = pixelNo;
				m_Wavelength4 = wavelength;
			}
			else if (!m_PixelNo5.HasValue && !IsRepeatedLine(pixelNo, wavelength, 5))
			{
				m_PixelNo5 = pixelNo;
				m_Wavelength5 = wavelength;
			}
			else if (!m_PixelNo6.HasValue && !IsRepeatedLine(pixelNo, wavelength, 6))
			{
				m_PixelNo6 = pixelNo;
				m_Wavelength6 = wavelength;
			}
			else if (!m_PixelNo7.HasValue && !IsRepeatedLine(pixelNo, wavelength, 7))
			{
				m_PixelNo7 = pixelNo;
				m_Wavelength7 = wavelength;
			}
			else if (!m_PixelNo8.HasValue && !IsRepeatedLine(pixelNo, wavelength, 8))
			{
				m_PixelNo8 = pixelNo;
				m_Wavelength8 = wavelength;
			}

			if (attemptCalibration)
				Calibrate(polynomialOrder);
        }

		private bool IsRepeatedLine(int pixelNo, float wavelength, int index)
		{
			if (index > 7)
			{
				if (m_PixelNo7.HasValue && (m_PixelNo7.Value == pixelNo || Math.Abs(m_Wavelength7- wavelength) > 0.1)) return true;

				if (index > 6)
				{
					if (m_PixelNo6.HasValue && (m_PixelNo6.Value == pixelNo || Math.Abs(m_Wavelength6 - wavelength) > 0.1)) return true;

					if (index > 5)
					{
						if (m_PixelNo5.HasValue && (m_PixelNo5.Value == pixelNo || Math.Abs(m_Wavelength5 - wavelength) > 0.1)) return true;

						if (index > 4)
						{
							if (m_PixelNo4.HasValue && (m_PixelNo4.Value == pixelNo || Math.Abs(m_Wavelength4 - wavelength) > 0.1)) return true;

							if (index > 3)
							{
								if (m_PixelNo3.HasValue && (m_PixelNo3.Value == pixelNo || Math.Abs(m_Wavelength3 - wavelength) > 0.1)) return true;

								if (index > 2)
								{
									if (m_PixelNo2.HasValue && (m_PixelNo2.Value == pixelNo || Math.Abs(m_Wavelength2 - wavelength) > 0.1)) return true;

									if (index > 1)
									{
										if (m_PixelNo1.HasValue && (m_PixelNo1.Value == pixelNo || Math.Abs(m_Wavelength1 - wavelength) > 0.1)) return true;
									}
								}
							}
						}
					}
				}
			}

			return false;
		}

        public void SetDispersion(float dispersion)
        {
            if (m_PixelNo1.HasValue)
            {
                m_PixelNo2 = m_PixelNo1.Value + 1000;
                m_Wavelength2 = dispersion * 1000;

                Calibrate(1);
            }
        }

		private void Calibrate(int polynomialOrder)
        {
			if (polynomialOrder == 1)
			{
				if (m_PixelNo1.HasValue && m_PixelNo2.HasValue)
				{
					var calibration = new LinearWavelengthCalibration();
					calibration.Calibrate(m_PixelNo1.Value, m_PixelNo2.Value, m_Wavelength1, m_Wavelength2);

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
			return m_PixelNo1.HasValue;
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
            m_PixelNo1 = null;
            m_PixelNo2 = null;
			m_PixelNo3 = null;
			m_PixelNo4 = null;
			m_PixelNo5 = null;
			m_PixelNo6 = null;
			m_PixelNo7 = null;
			m_PixelNo8 = null;
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
}
