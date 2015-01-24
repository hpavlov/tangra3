/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tangra.VideoOperations.LightCurves.InfoForms
{
	public class MagnitudeConverter
	{
		private double[] m_ReferenceMagnitudes;
	    private double m_ReferenceIntensity;

		private int m_ReferenceIndex = -1;

		internal MagnitudeConverter(double[] referenceMagnitudes, double referenceIntensity)
		{
			m_ReferenceMagnitudes = referenceMagnitudes;
		    m_ReferenceIntensity = referenceIntensity;
		}

		public bool CanComputeMagnitudes
		{
			get { return m_ReferenceIndex > -1; }
		}

        public double[] GetReferenceMagnitudes()
        {
            return m_ReferenceMagnitudes;
        }

		internal double[] ComputeMagnitudes(LCMeasurement[] mesurements)
		{
			var rv = new double[mesurements.Length];

			if (CanComputeMagnitudes)
			{
				double refIntensity = mesurements[m_ReferenceIndex].AdjustedReading;
				double refMagnitude = m_ReferenceMagnitudes[m_ReferenceIndex];

				if (rv.Length > 0)
					rv[0] = refMagnitude - 2.5 * Math.Log10(Math.Max(1, mesurements[0].AdjustedReading) / refIntensity);
				if (rv.Length > 1)
					rv[1] = refMagnitude - 2.5 * Math.Log10(Math.Max(1, mesurements[1].AdjustedReading) / refIntensity);
				if (rv.Length > 2)
					rv[2] = refMagnitude - 2.5 * Math.Log10(Math.Max(1, mesurements[2].AdjustedReading) / refIntensity);
				if (rv.Length > 3)
					rv[3] = refMagnitude - 2.5 * Math.Log10(Math.Max(1, mesurements[3].AdjustedReading) / refIntensity);
			}
			else
				for (int i = 0; i < rv.Length; i++) rv[i] = double.NaN;

			return rv;
		}

	    internal double ComputeMagnitude(double adjustedReading)
	    {
	        if (CanComputeMagnitudes)
	        {
                double refIntensity = m_ReferenceIntensity;
	            double refMagnitude = m_ReferenceMagnitudes[m_ReferenceIndex];
                return refMagnitude - 2.5 * Math.Log10(Math.Max(1, adjustedReading) / refIntensity);
	        }
            else
                return double.NaN;
	    }

        internal double ComputeFlux(double magnitude)
        {
            if (CanComputeMagnitudes)
            {
                double refIntensity = m_ReferenceIntensity;
                double refMagnitude = m_ReferenceMagnitudes[m_ReferenceIndex];
                return Math.Max(1, refIntensity * Math.Pow(10, (refMagnitude - magnitude) / 2.5));
            }
            else
                return double.NaN;
        }

	    public void SetReferenceMagnitude(int referenceIndex, double referenceMag, double referenceIntensity)
		{
			m_ReferenceIndex = -1;
	        m_ReferenceIntensity = referenceIntensity;

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
