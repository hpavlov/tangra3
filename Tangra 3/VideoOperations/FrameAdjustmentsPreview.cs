using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Controller;
using Tangra.Model.Config;
using Tangra.Model.Image;
using Tangra.Model.Video;
using Tangra.Model.VideoOperations;
using Tangra.PInvoke;

namespace Tangra.VideoOperations
{
	public class FrameAdjustmentsPreview
	{
		public static byte MSG_NO_PREPROCESSING = 0;

		private int m_CurrFrameNo;

		private int m_Brightness;
		private sbyte m_Contrast;
		private bool m_UseBrightnessContrast = false;

		private ushort m_FromByte;
		private ushort m_ToByte;
		private bool m_UseClipping = false;
		private bool m_UseStretching = false;

		private bool m_UseIntegration = false;

		private int m_FramesToIntegrate;
		private FrameIntegratingMode m_IntegrationMode;
		private PixelIntegrationType m_IntegrationType;

		public IFramePlayer FramePlayer { get; set; }
		public Form ParentForm { get; set; }

		private VideoController m_VideoController;

		public static FrameAdjustmentsPreview Instance = new FrameAdjustmentsPreview();

		private void ResetPreprocessingFlags()
		{
			m_UseBrightnessContrast = false;
			m_UseClipping = false;
			m_UseStretching = false;
		}

		public void Reset(VideoController videoController, int startingFrameNo)
		{
			m_UseIntegration = false;
			ResetPreprocessingFlags();
			m_VideoController = videoController;
			m_CurrFrameNo = startingFrameNo;
			m_CurrFrame = null;

			frmFullSizePreview.EnsureFullPreviewHidden();
		}

		public void MoveToFrame(int frameId)
		{
			m_CurrFrameNo = frameId;
			Update();
		}

		public void NoPreProcessing()
		{
			if (!m_UseStretching &&
				!m_UseClipping &&
				!m_UseBrightnessContrast)
				return;

			ResetPreprocessingFlags();

			Update();
		}

		public void BrightnessContrast(int brightness, sbyte contrast)
		{
			if (m_Brightness == brightness &&
				m_Contrast == contrast &&
				m_UseBrightnessContrast)
				return;

			ResetPreprocessingFlags();
			m_UseBrightnessContrast = true;

			m_Brightness = brightness;
			m_Contrast = contrast;

			Update();
		}

		public void Stretching(ushort fromByte, ushort toByte)
		{
			if (m_FromByte == fromByte &&
				m_ToByte == toByte &&
				m_UseStretching)
				return;

			ResetPreprocessingFlags();
			m_UseStretching = true;

			m_FromByte = fromByte;
			m_ToByte = toByte;

			Update();
		}

		public void Clipping(ushort fromByte, ushort toByte)
		{
			if (m_FromByte == fromByte &&
				m_ToByte == toByte &&
				m_UseClipping)
				return;

			ResetPreprocessingFlags();
			m_UseClipping = true;

			m_FromByte = fromByte;
			m_ToByte = toByte;

			Update();
		}

		public void Integration(int framesToIntegrate, FrameIntegratingMode mode, PixelIntegrationType type)
		{
			m_UseIntegration = true;
			m_FramesToIntegrate = framesToIntegrate;
			m_IntegrationMode = mode;
			m_IntegrationType = type;

			Update();
		}

		public void NoIntegration()
		{
			if (!m_UseIntegration) return;

			m_UseIntegration = false;

			Update();
		}

		private Pixelmap m_CurrFrame = null;

		internal void Update()
		{
			if (m_UseIntegration ||
			    m_UseClipping ||
			    m_UseStretching ||
			    m_UseBrightnessContrast)
			{


				if (m_UseIntegration)
					m_VideoController.SetupFrameIntegration(m_FramesToIntegrate, m_IntegrationMode, m_IntegrationType);
				else
					m_VideoController.SetupFrameIntegration(1, FrameIntegratingMode.NoIntegration, PixelIntegrationType.Mean);

				TangraCore.PreProcessors.ClearAll();

				if (m_UseStretching)
				{
					TangraCore.PreProcessors.AddStretching(m_FromByte, m_ToByte);
				}
				else if (m_UseClipping)
				{
					TangraCore.PreProcessors.AddClipping(m_FromByte, m_ToByte);
				}
				if (m_UseBrightnessContrast)
				{
					TangraCore.PreProcessors.AddBrightnessContrast(m_Brightness, m_Contrast);
				}

				if (Math.Abs(TangraConfig.Settings.Photometry.EncodingGamma - 1) > 0.01)
				{
					TangraCore.PreProcessors.AddGammaCorrection(TangraConfig.Settings.Photometry.EncodingGamma);
				}

				//if (VideoContext.Current.DarkFrameBytes != null)
				//{
				//    Core.PreProcessors.AddDarkFrame(VideoContext.Current.DarkFrameBytes);
				//    //m_FramePreProcessor.AddPreProcessor(
				//    //    new FrameDarkFlatCorrector(
				//    //        VideoContext.Current.DarkFrameBytes,
				//    //        VideoContext.Current.FlatFrameMedian,
				//    //        true));
				//}

				//if (VideoContext.Current.FlatFrameBytes != null)
				//{
				//    Core.PreProcessors.AddFlatFrame(VideoContext.Current.FlatFrameBytes, VideoContext.Current.FlatFrameMedian);
				//}

				//m_FramePreProcessor.OnPreProcess(m_CurrFrame);

				m_CurrFrame = FramePlayer.GetFrame(m_CurrFrameNo, !m_UseIntegration);

				if (m_CurrFrame != null)
				{
					frmFullSizePreview.EnsureFullPreviewVisible(m_CurrFrame, ParentForm);
					frmFullSizePreview.Update(m_CurrFrame);					
				}
			}
			else
			{
				TangraCore.PreProcessors.ClearAll();

				frmFullSizePreview.EnsureFullPreviewHidden();
			}
		}

		public void MoveForm(int left, int top)
		{
			frmFullSizePreview.MoveForm(left, top);
		}

		public Pixelmap CurrentFrame
		{
			get { return m_CurrFrame; }
		}
	}
}
