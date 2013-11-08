using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Model.Image;
using Tangra.Model.Video;

namespace Tangra.Model.Video
{
	public delegate void RenderFrameCallback(
		int currentFrameIndex,
		Pixelmap currentPixelmap,
		MovementType movementType,
		bool isLastFrame,
		int milisecondsToWait,
        int firstFrameInIntegrationPeriod);



	public interface IVideoFrameRenderer
	{
		void PlayerStarted();
		void PlayerStopped();
        void RenderFrame(int currentFrameIndex, Pixelmap currentPixelmap, MovementType movementType, bool isLastFrame, int msToWait, int firstFrameInIntegrationPeriod);
	}

	public interface IFrameStream
	{
		int Width { get; }
		int Height { get; }
		int BitPix { get; }
		int FirstFrame { get; }
		int LastFrame { get; }
		int CountFrames { get; }
		double FrameRate { get; }
		double MillisecondsPerFrame { get; }
		Pixelmap GetPixelmap(int index);
		int RecommendedBufferSize { get; }
		string VideoFileType { get; }

		Pixelmap GetIntegratedFrame(int startFrameNo, int framesToIntegrate, bool isSlidingIntegration, bool isMedianAveraging);
		string Engine { get; }
		string FileName { get; }
	}

	public class GeoLocationInfo
	{
		public GeoLocationInfo()
		{ }

		public GeoLocationInfo(GeoLocationInfo clone)
		{
			Longitude = clone.Longitude;
			Latitude = clone.Latitude;

			Altitude = clone.Altitude;
			MslWgs84Offset = clone.MslWgs84Offset;
			GpsHdop = clone.GpsHdop;
		}

		public string Longitude;
		public string Latitude;

		public string Altitude;
		public string MslWgs84Offset;
		public string GpsHdop;

		public string GetFormattedGeoLocation()
		{
			return string.Format("{0} {1} {2}{3}", Longitude, Latitude, Altitude, MslWgs84Offset).Replace("*", "°");
		}
	}

	public interface IFramePlayer
	{
		void SetFrameRenderer(IVideoFrameRenderer frameRenderer);
		bool IsRunning { get; }
		bool IsAstroDigitalVideo { get; }
		bool IsAstroAnalogueVideo { get; }
        bool AstroAnalogueVideoHasOcrData { get; }
        int AstroAnalogueVideoIntegratedAAVFrames { get; }
		string AstroVideoCameraModel { get; }
		string AstroVideoNativeVideoStandard { get; }
		GeoLocationInfo GeoLocation { get; }
		void Start(FramePlaySpeed mode, uint step);
		void Stop();
		void StepForward();
		void StepBackward();
		void StepForward(int seconds);
		void StepBackward(int seconds);
        void MoveToFrame(int frameNo);
		Pixelmap GetFrame(int frameNo, bool noIntegrate);
        Pixelmap GetIntegratedFrame(int startFrameNo, int framesToIntegrate, bool isSlidingIntegration, bool isMedianAveraging);
		FrameStateData GetFrameStatusChannel(int frameId);
		void DisposeResources();
		void OpenVideo(IFrameStream frameStream);
		void CloseVideo();
		IFrameStream Video { get; }
		int FrameStep { get; }
		void RefreshCurrentFrame();
		void SetupFrameIntegration(int framesToIntegrate, FrameIntegratingMode frameMode, PixelIntegrationType pixelIntegrationType);
        FrameIntegratingMode FrameIntegratingMode { get; }
        int FramesToIntegrate { get; }
	}

	public class PlayerContext
	{
		private Control m_MainThreadControl;
		private IVideoFrameRenderer m_FrameRenderer;

		public Control MainThreadControl
		{
			get { return m_MainThreadControl; }
		}

		public IVideoFrameRenderer FrameRenderer
		{
			get { return m_FrameRenderer; }
		}

		public PlayerContext(object hostForm)
		{
			m_MainThreadControl = hostForm as Control;
			m_FrameRenderer = hostForm as IVideoFrameRenderer;
		}
	}
}
