using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tangra.VideoOperations.Spectroscopy.Helpers
{
	public class SpectraCalibrator
    {
	    private MasterSpectra m_MasterSpectra;

	    private int? m_PixelNo1;
	    private float m_Wavelength1;

        private int? m_PixelNo2;
        private float m_Wavelength2;


	    private int m_ZeroPixelNo;
        private float m_ZeroWavelength;
	    private float m_AperPixels;

	    public SpectraCalibrator(MasterSpectra masterSpectra)
	    {
	        m_MasterSpectra = masterSpectra;
	    }

	    public void SetMarker(int pixelNo, float wavelength)
        {
            if (!m_PixelNo1.HasValue)
            {
                m_PixelNo1 = pixelNo;
                m_Wavelength1 = wavelength;
            }
            else if (!m_PixelNo2.HasValue && m_PixelNo1.Value != pixelNo && Math.Abs(m_Wavelength1-wavelength) > 0.1)
            {
                m_PixelNo2 = pixelNo;
                m_Wavelength2 = wavelength;

                Calibrate();
            }
        }

        public void SetDispersion(float dispersion)
        {
            if (m_PixelNo1.HasValue)
            {
                m_PixelNo2 = m_PixelNo1.Value + 1000;
                m_Wavelength2 = dispersion * 1000;

                Calibrate();
            }
        }

        private void Calibrate()
        {
            if (m_PixelNo1.HasValue && m_PixelNo2.HasValue)
            {
                if (m_PixelNo1.Value > m_PixelNo2.Value)
                {
                    m_ZeroPixelNo = m_PixelNo2.Value;
                    m_ZeroWavelength = m_Wavelength2;
                    m_AperPixels = (m_Wavelength1 - m_Wavelength2) / (m_PixelNo1.Value - m_PixelNo2.Value);
                }
                else
                {
                    m_ZeroPixelNo = m_PixelNo1.Value;
                    m_ZeroWavelength = m_Wavelength1;
                    m_AperPixels = (m_Wavelength2 - m_Wavelength1) / (m_PixelNo2.Value - m_PixelNo1.Value);
                }
            }
        }

        public bool IsCalibrated()
        {
            return m_PixelNo1.HasValue && m_PixelNo2.HasValue;
        }

		public SpectraCalibration ToSpectraCalibration()
		{
			if (IsCalibrated())
			{
				return new SpectraCalibration()
				{
					Pixel1 = m_PixelNo1.Value,
					Pixel2 = m_PixelNo2.Value,
					Wavelength1 = m_Wavelength1,
					Wavelength2 = m_Wavelength2,
					Dispersion = m_AperPixels,
					ZeroPixel = m_ZeroPixelNo
				};
			}

			return null;
		}

        public float GetCalibrationScale()
        {
            return m_AperPixels;
        }

        public void Reset()
        {
            m_PixelNo1 = null;
            m_PixelNo2 = null;
        }

        public float ResolveWavelength(int pixelNo)
        {
	        if (IsCalibrated())
	        {
				SpectraPoint point = m_MasterSpectra.Points.SingleOrDefault(x => x.PixelNo == pixelNo);
				if (point != null)
					return m_ZeroWavelength + (point.PixelNo - m_ZeroPixelNo) * m_AperPixels;
			}

            return float.NaN;
        }

        public int ResolvePixelNo(float wavelength)
        {
            return (int)Math.Round(m_ZeroPixelNo + (wavelength - m_ZeroWavelength)/m_AperPixels); 
        }

        public void PopulateWaveLengths(Spectra spectra)
        {
            foreach (SpectraPoint point in spectra.Points)
            {
                point.Wavelength = ResolveWavelength(point.PixelNo);
            }
        }
    }
}
