/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Tangra.Model.Config;
using Tangra.Model.Context;
using Tangra.Model.Image;
using Tangra.Model.Video;
using Tangra.Video.SER;

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
        private bool m_ObjectInfoDisplayed = false;
        private int m_FirstFrameNo = -1;
        private int m_FramesLeftToDiplayMessage = -1;
	    private string m_LastMessage = null;

	    private AdvFileMetadataInfo m_AdvFileMetadataInfo;
		private GeoLocationInfo m_GeoLocation;
		private SerEquipmentInfo m_SerEquipmentInfo;

		public void Reset()
		{
            m_EquipmentInfoDisplayed = false;
		    m_ObjectInfoDisplayed = false;
		}

		public void InitAdvFile(AdvFileMetadataInfo fileMetadataInfo, GeoLocationInfo geoLocation, int firstFrameNo)
        {
            m_AdvFileMetadataInfo = fileMetadataInfo;
			m_GeoLocation = geoLocation;
            m_FirstFrameNo = firstFrameNo;
        }

		public void InitSerFile(SerEquipmentInfo equipmentInfo, int firstFrameNo)
		{
			m_SerEquipmentInfo = equipmentInfo;
			m_FirstFrameNo = firstFrameNo;
		}


        public void OverlayStateForFrame(Bitmap currentImage, FrameStateData state, RenderFrameContext context, bool isAstroDigitalVideo, bool isAstroAnalogueVideo)
		{			
			using (Graphics g = Graphics.FromImage(currentImage))
			{
                if (m_CurrentImageWidth != currentImage.Width)
                    ComputeFontSizeAndTimeStampPosition(g, currentImage.Width, currentImage.Height);

				if (isAstroDigitalVideo && !isAstroAnalogueVideo)
                    OverlayADVState(g, currentImage, state, context);	
				else if (isAstroAnalogueVideo)
                    OverlayAAVState(g, currentImage, state, context);
			}

            m_LastFrameNo = context.CurrentFrameIndex;
		}

        private void OverlayADVState(Graphics g, Bitmap currentImage, FrameStateData state, RenderFrameContext context)
		{
			if (TangraConfig.Settings.ADVS.OverlayTimestamp)
			{
				string timeStampStr = state.HasValidTimeStamp
						? string.Format("{0}.{1}", state.CentralExposureTime.ToString("dd MMM yyyy HH:mm:ss"), state.CentralExposureTime.Millisecond.ToString().PadLeft(3, '0'))
						: "Embedded Timestamp Not Found";
				g.DrawString(timeStampStr, s_TimeStampFont, s_TimeStampBrush, m_XPos, currentImage.Height - m_YPosUpper);
				g.Save();
			}

			int numTopLines = 0;

            if (TangraConfig.Settings.ADVS.OverlayObjectName && m_AdvFileMetadataInfo != null && !string.IsNullOrWhiteSpace(m_AdvFileMetadataInfo.ObjectName) &&
                (!m_ObjectInfoDisplayed || m_FirstFrameNo == context.CurrentFrameIndex))
            {
                string objectNameStr = string.Format(" {0}", m_AdvFileMetadataInfo.ObjectName);
                g.DrawString(objectNameStr, s_PropertiesFont, s_PropertiesGreenBrush, 10, 10 + numTopLines * (s_PropertiesFont.Size + 5));
                g.Save();
                numTopLines++;
                m_ObjectInfoDisplayed = true;
            }

            if (m_LastFrameNo + 1 != context.CurrentFrameIndex)
				// When frames jump we stop displaying the message
				m_FramesLeftToDiplayMessage = 0;

			if (TangraConfig.Settings.ADVS.OverlayAllMessages &&
				!string.IsNullOrEmpty(state.Messages) &&
				state.Messages.Trim().Length > 0)
			{
				m_LastMessage = state.Messages.Trim();
                m_FramesLeftToDiplayMessage = context.MovementType == MovementType.Step ? 1 : 10;
			}

			if ((TangraConfig.Settings.ADVS.OverlayAdvsInfo || TangraConfig.Settings.ADVS.OverlayCameraInfo || TangraConfig.Settings.ADVS.OverlayGeoLocation) &&
                (!m_EquipmentInfoDisplayed || m_FirstFrameNo == context.CurrentFrameIndex))
			{
				int numLines = 0;
				int lineNo = 0;
				if (TangraConfig.Settings.ADVS.OverlayCameraInfo) numLines += 2;
				if (TangraConfig.Settings.ADVS.OverlayAdvsInfo) numLines += 2;
				if (TangraConfig.Settings.ADVS.OverlayGeoLocation) numLines += 1;

				float startingY = currentImage.Height - numLines * (s_PropertiesFont.Size + 5) - 10;
				if (TangraConfig.Settings.ADVS.OverlayTimestamp) startingY -= m_YPosUpper;

				if (TangraConfig.Settings.ADVS.OverlayAdvsInfo && m_AdvFileMetadataInfo != null)
				{
                    if (!string.IsNullOrEmpty(m_AdvFileMetadataInfo.AdvrVersion) && !string.IsNullOrEmpty(m_AdvFileMetadataInfo.Recorder) && string.IsNullOrEmpty(m_AdvFileMetadataInfo.HtccFirmareVersion))
				    {
                        g.DrawString(string.Format("{0} v{1}", m_AdvFileMetadataInfo.Recorder, m_AdvFileMetadataInfo.AdvrVersion), s_PropertiesFont, s_PropertiesWhiteBrush, 10, startingY + lineNo * (s_PropertiesFont.Size + 5));
				    }
                    else if (!string.IsNullOrEmpty(m_AdvFileMetadataInfo.AdvrVersion) && !string.IsNullOrEmpty(m_AdvFileMetadataInfo.HtccFirmareVersion))
					{
						g.DrawString(
							string.Format("ADVR v{0} HTCC v{1}", m_AdvFileMetadataInfo.AdvrVersion, m_AdvFileMetadataInfo.HtccFirmareVersion),
							s_PropertiesFont, s_PropertiesWhiteBrush, 10, startingY + lineNo * (s_PropertiesFont.Size + 5));
					}
                    else
                    {
                        g.DrawString(m_AdvFileMetadataInfo.Recorder, s_PropertiesFont, s_PropertiesWhiteBrush, 10, startingY + lineNo * (s_PropertiesFont.Size + 5));
                    }
					lineNo++;
				}

				if (TangraConfig.Settings.ADVS.OverlayCameraInfo && m_AdvFileMetadataInfo != null)
				{
					if (!string.IsNullOrEmpty(m_AdvFileMetadataInfo.SensorInfo))
						g.DrawString(m_AdvFileMetadataInfo.SensorInfo, s_PropertiesFont, s_PropertiesWhiteBrush, 10, startingY + lineNo * (s_PropertiesFont.Size + 5));
					lineNo++;
                    if (!string.IsNullOrEmpty(m_AdvFileMetadataInfo.Camera) && m_AdvFileMetadataInfo.Camera != m_AdvFileMetadataInfo.SensorInfo)
						g.DrawString(m_AdvFileMetadataInfo.Camera, s_PropertiesFont, s_PropertiesWhiteBrush, 10, startingY + lineNo * (s_PropertiesFont.Size + 5));
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
				if (TangraConfig.Settings.ADVS.OverlayObjectName)
					numTopLines++;

				g.DrawString(m_LastMessage, s_PropertiesFont, s_PropertiesYellowBrush, 10, 10 + numTopLines * (s_PropertiesFont.Size + 5));
				g.Save();
				m_FramesLeftToDiplayMessage--;
			}
			
		}

        private void OverlayAAVState(Graphics g, Bitmap currentImage, FrameStateData state, RenderFrameContext context)
		{
			if (TangraConfig.Settings.AAV.Overlay_Timestamp)
			{
				string timeStampStr = state.HasValidTimeStamp
						? string.Format("{0}.{1}", state.CentralExposureTime.ToString("dd MMM yyyy HH:mm:ss"), state.CentralExposureTime.Millisecond.ToString().PadLeft(3, '0'))
                        : "Embedded Timestamp Not Found";
				g.DrawString(timeStampStr, s_TimeStampFont, s_TimeStampBrush, m_XPos, currentImage.Height - m_YPosUpper - 15);
				g.Save();
			}

            int numTopLines = 0;

            if (TangraConfig.Settings.AAV.Overlay_ObjectName && m_AdvFileMetadataInfo != null && !string.IsNullOrWhiteSpace(m_AdvFileMetadataInfo.ObjectName) &&
                (!m_ObjectInfoDisplayed || m_FirstFrameNo == context.CurrentFrameIndex))
            {
                string objectNameStr = string.Format(" {0}", m_AdvFileMetadataInfo.ObjectName);
                g.DrawString(objectNameStr, s_PropertiesFont, s_PropertiesGreenBrush, 10, 10 + numTopLines * (s_PropertiesFont.Size + 5));
                g.Save();
                numTopLines++;
                m_ObjectInfoDisplayed = true;
            }

            if (m_LastFrameNo + 1 != context.CurrentFrameIndex)
				// When frames jump we stop displaying the message
				m_FramesLeftToDiplayMessage = 0;

			if (TangraConfig.Settings.AAV.Overlay_AllMessages &&
				!string.IsNullOrEmpty(state.Messages) &&
				state.Messages.Trim().Length > 0)
			{
				m_LastMessage = state.Messages.Trim();
                m_FramesLeftToDiplayMessage = context.MovementType == MovementType.Step ? 1 : 10;
			}

			if ((TangraConfig.Settings.AAV.Overlay_AdvsInfo || TangraConfig.Settings.AAV.Overlay_CameraInfo) &&
                (!m_EquipmentInfoDisplayed || m_FirstFrameNo == context.CurrentFrameIndex))
			{
				int numLines = 0;
				int lineNo = 0;
				if (TangraConfig.Settings.AAV.Overlay_CameraInfo) numLines += 1;
				if (TangraConfig.Settings.AAV.Overlay_AdvsInfo) numLines += 1;

				float startingY = currentImage.Height - numLines * (s_PropertiesFont.Size + 5) - 35;
				if (TangraConfig.Settings.AAV.Overlay_Timestamp) startingY -= m_YPosUpper;

				if (TangraConfig.Settings.AAV.Overlay_AdvsInfo && m_AdvFileMetadataInfo != null)
				{
					g.DrawString(m_AdvFileMetadataInfo.Recorder, s_PropertiesFont, s_PropertiesWhiteBrush, 10, startingY + lineNo * (s_PropertiesFont.Size + 5));
					lineNo++;
				}

				if (TangraConfig.Settings.AAV.Overlay_CameraInfo && m_AdvFileMetadataInfo != null)
				{
					if (!string.IsNullOrEmpty(m_AdvFileMetadataInfo.Camera))
						g.DrawString(m_AdvFileMetadataInfo.Camera, s_PropertiesFont, s_PropertiesWhiteBrush, 10, startingY + lineNo * (s_PropertiesFont.Size + 5));
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
