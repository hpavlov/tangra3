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
        bool CameraCorrectionsHaveBeenApplied { get; }
        bool HasReliableTimeBase { get; }
		ISingleMeasurement[] GetTargetMeasurements();
		ISingleMeasurement[] GetComparisonObjectMeasurements(int comparisonObjectId);
		void GetIntegrationRateAndFirstFrame(out int integrationRate, out int firstIntegratingFrame);
		void SetFoundOccultationEvent(int eventId, float dFrame, float rFrame, float dFrameErrorMinus, float dFrameErrorPlus, float rFrameErrorMinus, float rFrameErrorPlus, string dTime, string rTime);
		void SetNoOccultationEvents();
        void SetTimeExtractionEngine(string engineAndVersion);
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
