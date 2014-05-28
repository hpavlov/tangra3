using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tangra.VideoOperations.LightCurves.InfoForms
{
	public class MagnitudeConverter
	{
		private double[] m_ReferenceMagnitudes;

		internal MagnitudeConverter(double[] referenceMagnitudes)
		{
			m_ReferenceMagnitudes = referenceMagnitudes;
		}
	}
}
