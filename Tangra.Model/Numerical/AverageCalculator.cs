using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tangra.Model.Numerical
{

	public class AverageCalculator
	{
		private List<double> m_Data = new List<double>();

		private double m_Average;
		private double m_Sigma;

		public void ClearDataPoints()
		{
			m_Data.Clear();
		}

		public double Average
		{
			get { return m_Average; }
		}

		public double Sigma
		{
			get { return m_Sigma; }
		}

		public void AddDataPoint(double x)
		{
			m_Data.Add(x);
		}

		public void Compute()
		{
			if (m_Data.Count > 1)
			{
				m_Average = m_Data.Average();
				double sumRes = m_Data.Select(d => (d - m_Average) * (d - m_Average)).Sum();
				m_Sigma = Math.Sqrt(sumRes / (m_Data.Count - 1));
			}
			else
			{
				m_Average = m_Data.Count == 1 ? m_Data[0] : double.NaN;
				m_Sigma = 0;
			}
		}
	}
}
