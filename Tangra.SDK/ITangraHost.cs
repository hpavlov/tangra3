using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tangra.SDK
{
	public interface ISettingsStorageProvider
	{
		string ReadSettings();
		void WriteSettings(string settings);

	}

	public interface ILightCurveDataProvider
	{
		string FileName { get; }
		int NumberOfMeasuredComparisonObjects { get; }
		ISingleMeasurement[] GetTargetMeasurements();
		ISingleMeasurement[] GetComparisonObjectMeasurements(int comparisonObjectId);
		void GetIntegrationRateAndFirstFrame(out int integrationRate, out int firstIntegratingFrame);
	}

	public interface ITangraHost
	{
		ISettingsStorageProvider GetSettingsProvider();
		ILightCurveDataProvider GetLightCurveDataProvider();
		IWin32Window ParentWindow { get; }
	}
 
	public interface ISingleMeasurement
	{
		int CurrFrameNo { get; }
		byte TargetNo { get; }

		float Measurement { get; }
        DateTime Timestamp { get; }
        bool IsCorrectedForInstrumentalDelay { get; }
	}
}
