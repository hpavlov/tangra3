using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tangra.VideoOperations.LightCurves.InfoForms
{
	public class MagnitudeConverter
	{
		private double[] m_ReferenceMagnitudes;

		private int m_ReferenceIndex = -1;

		internal MagnitudeConverter(double[] referenceMagnitudes)
		{
			m_ReferenceMagnitudes = referenceMagnitudes;
		}

		public bool CanComputeMagnitudes
		{
			get { return m_ReferenceIndex > -1; }
		}

		internal double[] ComputeMagnitudes(LCMeasurement[] mesurements)
		{
			var rv = new double[mesurements.Length];

			if (CanComputeMagnitudes)
			{
				double refIntensity = mesurements[m_ReferenceIndex].AdjustedReading;
				double refMagnitude = m_ReferenceMagnitudes[m_ReferenceIndex];

				if (rv.Length > 0)
					rv[0] = refMagnitude - 2.5 * Math.Log10(mesurements[0].AdjustedReading / refIntensity);
				if (rv.Length > 1)
					rv[1] = refMagnitude - 2.5 * Math.Log10(mesurements[1].AdjustedReading / refIntensity);
				if (rv.Length > 2)
					rv[2] = refMagnitude - 2.5 * Math.Log10(mesurements[2].AdjustedReading / refIntensity);
				if (rv.Length > 3)
					rv[3] = refMagnitude - 2.5 * Math.Log10(mesurements[3].AdjustedReading / refIntensity);
			}
			else
				for (int i = 0; i < rv.Length; i++) rv[i] = double.NaN;

			return rv;
		}

		public void SetReferenceMagnitude(int referenceIndex, double referenceMag)
		{
			m_ReferenceIndex = -1;

			for (int i = 0; i < m_ReferenceMagnitudes.Length; i++)
			{
				if (i == referenceIndex - 1)
				{
					m_ReferenceMagnitudes[i] = referenceMag;
					m_ReferenceIndex = referenceIndex - 1;
				}
				else
					m_ReferenceMagnitudes[i] = double.NaN;
			}			
		}
	}
}
