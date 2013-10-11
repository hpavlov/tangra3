using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tangra.SDK;
using Tangra.VideoOperations.LightCurves;

namespace Tangra.Addins
{
	[Serializable]
	public class MarshalByRefLightCurveDataProvider : ILightCurveDataProvider
	{
		private ISingleMeasurement[] m_TargetMeasurements;
		private ISingleMeasurement[] m_Comp1Measurements;
		private ISingleMeasurement[] m_Comp2Measurements;
		private ISingleMeasurement[] m_Comp3Measurements;
		private int m_IntegrationRate;
		private int m_FirstIntegratingFrame;

		internal MarshalByRefLightCurveDataProvider(ILightCurveDataProvider localProvider)
		{
			FileName = localProvider.FileName;
			NumberOfMeasuredComparisonObjects = localProvider.NumberOfMeasuredComparisonObjects;
			m_TargetMeasurements = localProvider.GetTargetMeasurements();
			if (NumberOfMeasuredComparisonObjects > 0)
				m_Comp1Measurements = localProvider.GetComparisonObjectMeasurements(0);
			if (NumberOfMeasuredComparisonObjects > 1)
				m_Comp1Measurements = localProvider.GetComparisonObjectMeasurements(1);
			if (NumberOfMeasuredComparisonObjects > 2)
				m_Comp1Measurements = localProvider.GetComparisonObjectMeasurements(2);
			localProvider.GetIntegrationRateAndFirstFrame(out m_IntegrationRate, out m_FirstIntegratingFrame);
		}

		public string FileName { get; private set; }

		public int NumberOfMeasuredComparisonObjects { get; private set; }

		public ISingleMeasurement[] GetTargetMeasurements()
		{
			return m_TargetMeasurements;
		}

		public ISingleMeasurement[] GetComparisonObjectMeasurements(int comparisonObjectId)
		{
			switch (comparisonObjectId)
			{
				case 0:
					return m_Comp1Measurements;
				case 1:
					return m_Comp2Measurements;
				case 2:
					return m_Comp3Measurements;
			}

			throw new IndexOutOfRangeException();
		}

		public void GetIntegrationRateAndFirstFrame(out int integrationRate, out int firstIntegratingFrame)
		{
			integrationRate = m_IntegrationRate;
			firstIntegratingFrame = m_FirstIntegratingFrame;
		}
	}
}
