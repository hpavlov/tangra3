using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Tangra.Helpers
{
	public class UsageStats
	{
		public UsageStats()
		{
			UniqueId = Guid.NewGuid().ToString();
			OperatingSystem = CurrentOS.Name;
		}

		private static object s_SyncLock = new object();
		private XmlSerializer m_Serializer = new XmlSerializer(typeof(UsageStats));

		static UsageStats()
		{
			lock (s_SyncLock)
			{
				try
				{
					var ser = new XmlSerializer(typeof (UsageStats));
					Instance = (UsageStats)ser.Deserialize(new StringReader(Properties.Settings.Default.UsageStatistics));
				}
				catch (Exception)
				{
					Instance = new UsageStats();
				}
			}
		}

		public static void ClearStats()
		{
			string oldId = Instance.UniqueId;
			Instance = new UsageStats();
			Instance.UniqueId = oldId;
			Instance.Save();
		}

		public void Save()
		{
			var serData = new StringBuilder();
			using (var memStr = new StringWriter(serData))
			{
				m_Serializer.Serialize(memStr, this);
			}
			Properties.Settings.Default.UsageStatistics = serData.ToString();
			Properties.Settings.Default.Save();
		}

		public static UsageStats Instance;

		public string UniqueId { get; set; }
		public string OperatingSystem { get; set; }

		public int ProcessedAviFiles { get; set; }
		public int ProcessedAdvFiles { get; set; }
		public int ProcessedAavFiles { get; set; }
		public int ProcessedSerFiles { get; set; }
		public int ProcessedFitsFolderFiles { get; set; }
		public int SavedLightCurves { get; set; }
		public int LightCurvesOpened { get; set; }
		public int DirectShowUsed { get; set; }
		public int VideoForWindowsUsed { get; set; }
		public int DarkFramesUsed { get; set; }
		public int FlatFramesUsed { get; set; }

		public int TrackedAsteroidals { get; set; }
		public int TrackingWithRecoverUsed { get; set; }
		public int SimplifiedTrackingUsed { get; set; }
		public int AutomaticTrackingUsed { get; set; }
		public int TrackedMutualEvents { get; set; }
		public int UntrackedMeasurements { get; set; }
		public int TotalLunarMeasurements { get; set; }
		public int FullDisappearanceFlag { get; set; }
		public int WindOrShakingFlag { get; set; }
		public int FlickeringFlag { get; set; }
		public int FieldRotationFlag { get; set; }
		public int DriftThroughFlag { get; set; }
		public int SoftwareIntegrationUsed { get; set; }
		public int PreProcessingUsed { get; set; }
		public int ReverseGammaUsed { get; set; }
		public int DigitalFilterUsed { get; set; }
		public int AperturePhotometry { get; set; }
		public int PSFPhotometryUsed { get; set; }
		public int OptimalExtractionUsed { get; set; }
		public int AverageBackgroundUsed { get; set; }
		public int BackgroundModeUsed { get; set; }
		public int _3DPolynomialFitUsed { get; set; }
		public int PSFFittingBackgroundUsed { get; set; }
		public int MedianBackgroundUsed { get; set; }
		public int TrackedFrames { get; set; }
		public int TrackedObjects { get; set; }

		public int FramesWithBadTracking { get; set; }
		public int FramesIOTATimeStampRead { get; set; }
		public int FramesIOTATimeStampReadingErrored { get; set; }

		public int AddinActionsInvoked { get; set; }
		public int QuickReprocessInvoked { get; set; }
		public int FramePixelDistributionInvoked { get; set; }
		public int ReducedDataDistributoinInvoked { get; set; }
		public int MagnitudeCalculationsInvoked { get; set; }

		public int ModelVideosGenerated { get; set; }
		public int FSTSFileViewerInvoked { get; set; }
		public int FSTFCropInvoked { get; set; }
		public int FSTSToAVIInvoked { get; set; }
		public int FSTSToCSVInvoked { get; set; }

		public int DarkFramesProduced { get; set; }
		public int FlatFramesProduced { get; set; }

		public int HueIntensityModeUsed { get; set; }
		public int InvertedModeUsed { get; set; }
		public int CustomDynamicRangeUsed { get; set; }
		public int HighGammaModeUsed { get; set; }
		public int LowGammaModeUsed { get; set; }
		public int NoGammaModeUsed { get; set; }
		
		public int ExportToFITSUsed { get; set; }
		public int ExportToBMPUsed { get; set; }
		public int ExportToCSVUsed { get; set; }
		public int TargetPSFViewerFormShown { get; set; }
		public int FileInformationMenuUsed { get; set; }
		public int OnlineHelpMenuUsed { get; set; }
	}
}
