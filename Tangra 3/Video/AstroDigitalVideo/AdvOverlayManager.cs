using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Tangra.Model.Config;
using Tangra.Model.Image;

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

		private int m_CurrentImageWidth = 0;
		private float m_CurrentFontSize = 28.0f;
		private float m_XPos = 10;
		private float m_YPosUpper = 50;

	    private int m_LastFrameNo = -1;
        private int m_FramesLeftToDiplayMessage = -1;
	    private string m_LastMessage = null;


		public void Reset()
		{
			
		}

		public void OverlayStateForFrame(Bitmap currentImage, FrameStateData state, int currentFrameNo)
		{			
			using (Graphics g = Graphics.FromImage(currentImage))
			{
                if (m_CurrentImageWidth != currentImage.Width)
                    ComputeFontSizeAndTimeStampPosition(g, currentImage.Width, currentImage.Height);

                if (TangraConfig.Settings.ADVS.OverlayTimestamp)
                {
                    string timeStampStr = string.Format("{0}.{1}", state.CentralExposureTime.ToString("dd MMM yyyy HH:mm:ss"), state.CentralExposureTime.Millisecond.ToString().PadLeft(3, '0'));
                    g.DrawString(timeStampStr, s_TimeStampFont, s_TimeStampBrush, m_XPos, currentImage.Height - m_YPosUpper);
                    g.Save();                    
                }

                if (TangraConfig.Settings.ADVS.OverlayGamma)
                {
					string gammaStr = string.Format("Gamma: {0} {1}", state.Gamma.ToString("0.000"), AdvStatusValuesHelper.GetWellKnownGammaForValue(state.Gamma));
					g.DrawString(gammaStr, s_PropertiesFont, s_PropertiesGreenBrush, 10, 10);
					g.Save();
                }

                if (TangraConfig.Settings.ADVS.OverlayGain)
                {
					string gammaStr = string.Format(" Gain: {0} dB", state.Gain.ToString("0"));
					g.DrawString(gammaStr, s_PropertiesFont, s_PropertiesGreenBrush, 10, 10 + s_PropertiesFont.Size + 5);
					g.Save();
                }

                if (m_LastFrameNo + 1 != currentFrameNo)
                    // When frames jump we stop displaying the message
                    m_FramesLeftToDiplayMessage = 0;

				if (TangraConfig.Settings.ADVS.OverlayAllMessages &&
					!string.IsNullOrEmpty(state.Messages) &&
                    state.Messages.Trim().Length > 0)
				{
				    m_LastMessage = state.Messages;
				    m_FramesLeftToDiplayMessage = 10;
				}

                if (m_FramesLeftToDiplayMessage > 0)
                {
                    g.DrawString(m_LastMessage, s_PropertiesFont, s_PropertiesYellowBrush, 10, 10 + 2 * (s_PropertiesFont.Size + 5));
                    g.Save();
                    m_FramesLeftToDiplayMessage--;
                }
			}

		    m_LastFrameNo = currentFrameNo;
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

		}
	}
}
