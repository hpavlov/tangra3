using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Tangra.Model.Config;
using Tangra.Model.Image;
using Tangra.Model.Video;

namespace Tangra.Video.AstroDigitalVideo
{
	public class AdvOverlayManager
	{
		private static Font s_TimeStampFont;
		private static Brush s_TimeStampBrush = new SolidBrush(Color.LimeGreen);

		private static Font s_PropertiesFont;
		private static Brush s_PropertiesGreenBrush = new SolidBrush(Color.LimeGreen);
		private static Brush s_PropertiesRedBrush = new SolidBrush(Color.Red);
		private static Brush s_PropertiesYellowBrush = new SolidBrush(Color.Yellow);
        private static Brush s_PropertiesWhiteBrush = new SolidBrush(Color.White);

		private int m_CurrentImageWidth = 0;
		private float m_CurrentFontSize = 28.0f;
		private float m_XPos = 10;
		private float m_YPosUpper = 50;

	    private int m_LastFrameNo = -1;
	    private bool m_EquipmentInfoDisplayed = false;
        private int m_FirstFrameNo = -1;
        private int m_FramesLeftToDiplayMessage = -1;
	    private string m_LastMessage = null;

	    private AdvEquipmentInfo m_AdvEquipmentInfo;
		private GeoLocationInfo m_GeoLocation;
		private SerEquipmentInfo m_SerEquipmentInfo;

		public void Reset()
		{
            m_EquipmentInfoDisplayed = false;
		}

		public void InitAdvFile(AdvEquipmentInfo equipmentInfo, GeoLocationInfo geoLocation, int firstFrameNo)
        {
            m_AdvEquipmentInfo = equipmentInfo;
			m_GeoLocation = geoLocation;
            m_FirstFrameNo = firstFrameNo;
        }

		public void InitSerFile(SerEquipmentInfo equipmentInfo, int firstFrameNo)
		{
			m_SerEquipmentInfo = equipmentInfo;
			m_FirstFrameNo = firstFrameNo;
		}


		public void OverlayStateForFrame(Bitmap currentImage, FrameStateData state, int currentFrameNo, bool isAstroDigitalVideo, bool isAstroAnalogueVideo)
		{			
			using (Graphics g = Graphics.FromImage(currentImage))
			{
                if (m_CurrentImageWidth != currentImage.Width)
                    ComputeFontSizeAndTimeStampPosition(g, currentImage.Width, currentImage.Height);

				if (isAstroDigitalVideo)
					OverlayADVState(g, currentImage, state, currentFrameNo);	
				else if (isAstroAnalogueVideo)
					OverlayAAVState(g, currentImage, state, currentFrameNo);
			}

		    m_LastFrameNo = currentFrameNo;
		}

		private void OverlayADVState(Graphics g, Bitmap currentImage, FrameStateData state, int currentFrameNo)
		{
			if (TangraConfig.Settings.ADVS.OverlayTimestamp)
			{
				string timeStampStr = state.HasValidTimeStamp
						? string.Format("{0}.{1}", state.CentralExposureTime.ToString("dd MMM yyyy HH:mm:ss"), state.CentralExposureTime.Millisecond.ToString().PadLeft(3, '0'))
						: "Timestamp Not Available";
				g.DrawString(timeStampStr, s_TimeStampFont, s_TimeStampBrush, m_XPos, currentImage.Height - m_YPosUpper);
				g.Save();
			}

			int numTopLines = 0;
			if (TangraConfig.Settings.ADVS.OverlayGamma)
			{
				string gammaStr = string.Format("Gamma: {0} {1}", state.Gamma.ToString("0.000"), AdvStatusValuesHelper.GetWellKnownGammaForValue(state.Gamma));
				g.DrawString(gammaStr, s_PropertiesFont, s_PropertiesGreenBrush, 10, 10 + numTopLines * (s_PropertiesFont.Size + 5));
				g.Save();
				numTopLines++;
			}

			if (TangraConfig.Settings.ADVS.OverlayGain)
			{
				string gammaStr = state.IsGainKnown
					? string.Format(" Gain: {0} dB", state.Gain.ToString("0"))
					: " Gain: Unknown";
				g.DrawString(gammaStr, s_PropertiesFont, s_PropertiesGreenBrush, 10, 10 + numTopLines * (s_PropertiesFont.Size + 5));
				g.Save();
				numTopLines++;
			}

			if (m_LastFrameNo + 1 != currentFrameNo)
				// When frames jump we stop displaying the message
				m_FramesLeftToDiplayMessage = 0;

			if (TangraConfig.Settings.ADVS.OverlayAllMessages &&
				!string.IsNullOrEmpty(state.Messages) &&
				state.Messages.Trim().Length > 0)
			{
				m_LastMessage = state.Messages.Trim();
				m_FramesLeftToDiplayMessage = 10;
			}

			if ((TangraConfig.Settings.ADVS.OverlayAdvsInfo || TangraConfig.Settings.ADVS.OverlayCameraInfo || TangraConfig.Settings.ADVS.OverlayGeoLocation) &&
				(!m_EquipmentInfoDisplayed || m_FirstFrameNo == currentFrameNo))
			{
				int numLines = 0;
				int lineNo = 0;
				if (TangraConfig.Settings.ADVS.OverlayCameraInfo) numLines += 2;
				if (TangraConfig.Settings.ADVS.OverlayAdvsInfo) numLines += 2;
				if (TangraConfig.Settings.ADVS.OverlayGeoLocation) numLines += 1;

				float startingY = currentImage.Height - numLines * (s_PropertiesFont.Size + 5) - 10;
				if (TangraConfig.Settings.ADVS.OverlayTimestamp) startingY -= m_YPosUpper;

				if (TangraConfig.Settings.ADVS.OverlayAdvsInfo && m_AdvEquipmentInfo != null)
				{
					g.DrawString(m_AdvEquipmentInfo.Recorder, s_PropertiesFont, s_PropertiesWhiteBrush, 10, startingY + lineNo * (s_PropertiesFont.Size + 5));
					lineNo++;
					if (!string.IsNullOrEmpty(m_AdvEquipmentInfo.AdvrVersion))
					{
						g.DrawString(
							string.Format("ADVR v{0} HTCC v{1}", m_AdvEquipmentInfo.AdvrVersion, m_AdvEquipmentInfo.HtccFirmareVersion),
							s_PropertiesFont, s_PropertiesWhiteBrush, 10, startingY + lineNo * (s_PropertiesFont.Size + 5));
					}
					lineNo++;
				}

				if (TangraConfig.Settings.ADVS.OverlayCameraInfo && m_AdvEquipmentInfo != null)
				{
					if (!string.IsNullOrEmpty(m_AdvEquipmentInfo.SensorInfo))
						g.DrawString(m_AdvEquipmentInfo.SensorInfo, s_PropertiesFont, s_PropertiesWhiteBrush, 10, startingY + lineNo * (s_PropertiesFont.Size + 5));
					lineNo++;
					if (!string.IsNullOrEmpty(m_AdvEquipmentInfo.Camera))
						g.DrawString(m_AdvEquipmentInfo.Camera, s_PropertiesFont, s_PropertiesWhiteBrush, 10, startingY + lineNo * (s_PropertiesFont.Size + 5));
					lineNo++;
				}

				if (TangraConfig.Settings.ADVS.OverlayGeoLocation && m_GeoLocation != null)
				{

					g.DrawString(m_GeoLocation.GetFormattedGeoLocation(), s_PropertiesFont, s_PropertiesWhiteBrush, 10, startingY + lineNo * (s_PropertiesFont.Size + 5));
					lineNo++;
				}

				g.Save();

				m_EquipmentInfoDisplayed = true;
			}

			if (m_FramesLeftToDiplayMessage > 0)
			{
				if (TangraConfig.Settings.ADVS.OverlayGamma || TangraConfig.Settings.ADVS.OverlayGain)
					numTopLines++;

				g.DrawString(m_LastMessage, s_PropertiesFont, s_PropertiesYellowBrush, 10, 10 + numTopLines * (s_PropertiesFont.Size + 5));
				g.Save();
				m_FramesLeftToDiplayMessage--;
			}
			
		}

		private void OverlayAAVState(Graphics g, Bitmap currentImage, FrameStateData state, int currentFrameNo)
		{
			if (TangraConfig.Settings.AAV.Overlay_Timestamp)
			{
				string timeStampStr = state.HasValidTimeStamp
						? string.Format("{0}.{1}", state.CentralExposureTime.ToString("dd MMM yyyy HH:mm:ss"), state.CentralExposureTime.Millisecond.ToString().PadLeft(3, '0'))
						: "Timestamp Not Available";
				g.DrawString(timeStampStr, s_TimeStampFont, s_TimeStampBrush, m_XPos, currentImage.Height - m_YPosUpper - 15);
				g.Save();
			}

			int numTopLines = 0;

			if (m_LastFrameNo + 1 != currentFrameNo)
				// When frames jump we stop displaying the message
				m_FramesLeftToDiplayMessage = 0;

			if (TangraConfig.Settings.AAV.Overlay_AllMessages &&
				!string.IsNullOrEmpty(state.Messages) &&
				state.Messages.Trim().Length > 0)
			{
				m_LastMessage = state.Messages.Trim();
				m_FramesLeftToDiplayMessage = 10;
			}

			if ((TangraConfig.Settings.AAV.Overlay_AdvsInfo || TangraConfig.Settings.AAV.Overlay_CameraInfo) &&
				(!m_EquipmentInfoDisplayed || m_FirstFrameNo == currentFrameNo))
			{
				int numLines = 0;
				int lineNo = 0;
				if (TangraConfig.Settings.AAV.Overlay_CameraInfo) numLines += 1;
				if (TangraConfig.Settings.AAV.Overlay_AdvsInfo) numLines += 1;

				float startingY = currentImage.Height - numLines * (s_PropertiesFont.Size + 5) - 35;
				if (TangraConfig.Settings.AAV.Overlay_Timestamp) startingY -= m_YPosUpper;

				if (TangraConfig.Settings.AAV.Overlay_AdvsInfo && m_AdvEquipmentInfo != null)
				{
					g.DrawString(m_AdvEquipmentInfo.Recorder, s_PropertiesFont, s_PropertiesWhiteBrush, 10, startingY + lineNo * (s_PropertiesFont.Size + 5));
					lineNo++;
				}

				if (TangraConfig.Settings.AAV.Overlay_CameraInfo && m_AdvEquipmentInfo != null)
				{
					if (!string.IsNullOrEmpty(m_AdvEquipmentInfo.Camera))
						g.DrawString(m_AdvEquipmentInfo.Camera, s_PropertiesFont, s_PropertiesWhiteBrush, 10, startingY + lineNo * (s_PropertiesFont.Size + 5));
					lineNo++;
				}

				g.Save();

				m_EquipmentInfoDisplayed = true;
			}
		}

		private void ComputeFontSizeAndTimeStampPosition(Graphics g, int imageWidth, int imageHeight)
		{
			string TEST_TIMESTAMP = "04 Nov 2011 17:43:14.805";

			float maxWidth = (imageWidth - 20) * 0.9f;
			float maxHeight = imageHeight * 0.25f;

			if (s_TimeStampFont	 != null) s_TimeStampFont.Dispose();
			if (s_PropertiesFont != null) s_PropertiesFont.Dispose();
			s_TimeStampFont = null;
			s_PropertiesFont = null;

			for (float size = 28; size > 9; size-=0.5f)
			{
				s_TimeStampFont = new Font(FontFamily.GenericMonospace, size, FontStyle.Bold);
				s_PropertiesFont = new Font(FontFamily.GenericMonospace, size / 2, FontStyle.Bold);

				SizeF textSize = g.MeasureString(TEST_TIMESTAMP, s_TimeStampFont);
				if (textSize.Width <= maxWidth && textSize.Height <= maxHeight)
				{
					m_XPos = (imageWidth - textSize.Width) / 2.0f;
					m_YPosUpper = textSize.Height * 1.05f;
					m_CurrentFontSize = size;
					m_CurrentImageWidth = imageWidth;

					break;
				}

				s_TimeStampFont.Dispose();
				s_TimeStampFont = null;

				s_PropertiesFont.Dispose();
				s_PropertiesFont = null;
			}

			if (s_TimeStampFont == null)
			{
				// Font size could not be determined automatically. Use a default size
				s_TimeStampFont = new Font(FontFamily.GenericMonospace, 9, FontStyle.Bold);
				s_PropertiesFont = new Font(FontFamily.GenericMonospace, 4.5f, FontStyle.Bold);
			}

		}
	}
}
