/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Controller;
using Tangra.Helpers;
using Tangra.Model.Astro;
using Tangra.Model.Config;
using Tangra.Model.Context;
using Tangra.Model.Image;
using Tangra.Model.ImageTools;
using Tangra.Model.Video;
using Tangra.Model.VideoOperations;
using nom.tam.fits;
using nom.tam.util;

namespace Tangra.VideoOperations.MakeDarkFlatField
{
    enum FrameType
    {
        Dark,
        MasterDark,
        MasterFlat,
        MasterBias
    }

    enum AveragingType
    {
        Median,
        Mean
    }

    public class MakeDarkFlatOperation : VideoOperationBase, IVideoOperation
    {
	    private DarkFlatFrameController m_MakeDarkFlatController;
	    private IVideoController m_VideoController;
        private ucMakeDarkFlatField m_ControlPanel = null;
        private object m_SyncRoot = new object();

        private int m_NumFrames = 64;
        private int m_FramesDone = 0;
        private float m_ExposureSeconds = 0;
        private bool m_Running = false;
        private FrameType m_FrameType;
	    private int m_FrameNumber = 0;

	    private AstroImage m_CurrentAstroImage;

        private float[,] m_AveragedData;

	    public MakeDarkFlatOperation()
	    { }

	    public MakeDarkFlatOperation(DarkFlatFrameController makeDarkFlatController)
		{
			m_MakeDarkFlatController = makeDarkFlatController;
		}

        #region IVideoOperation Members

		public bool InitializeOperation(IVideoController videoController, Panel controlPanel, IFramePlayer framePlayer, Form topForm)
        {
            TangraContext.Current.CanLoadFlatFrame = false;
            TangraContext.Current.CanLoadDarkFrame = false;
            TangraContext.Current.CanLoadBiasFrame = false;

            m_VideoController = videoController;

            if (m_ControlPanel == null)
            {
                lock(m_SyncRoot)
                {
                    if (m_ControlPanel == null)
                    {
                        m_ControlPanel = new ucMakeDarkFlatField(this);
                    }
                }
            }

            controlPanel.Controls.Clear();
            controlPanel.Controls.Add(m_ControlPanel);
            m_ControlPanel.Dock = DockStyle.Fill;

            return true;
        }

        public void PlayerStarted()
        { }

        public void NextFrame(int frameNo, MovementType movementType, bool isLastFrame, AstroImage astroImage, int firstFrameInIntegrationPeriod, string frameFileName)
		{
			m_CurrentAstroImage = astroImage;
	        m_FrameNumber = frameNo;
            TangraContext.Current.CrashReportInfo.FrameNumber = frameNo;

            if (m_Running)
            {
                if (m_FramesDone < m_NumFrames)
                {
					Pixelmap currPixelmap = astroImage.Pixelmap;
                    if (m_FramesDone == 0)
                        m_AveragedData = new float[currPixelmap.Width, currPixelmap.Height];

                    for (int x = 0; x < currPixelmap.Width; x++)
                        for (int y = 0; y < currPixelmap.Height; y++)
                        {
                            m_AveragedData[x, y] += currPixelmap[x, y];
                        }

                    m_FramesDone++;
                    m_ControlPanel.SetProgress(m_FramesDone);

                    if (m_FramesDone == m_NumFrames)
                    {
						// Averaging
						for (int x = 0; x < currPixelmap.Width; x++)
						for (int y = 0; y < currPixelmap.Height; y++)
						{
							m_AveragedData[x, y] = (m_AveragedData[x, y] / (ulong)m_FramesDone);
						}

						m_ControlPanel.SetStopped();

						string fileName = string.Format("{0}_{1}.fit", Path.GetFileNameWithoutExtension(m_VideoController.CurrentVideoFileName), m_FrameType.ToString());

						if (m_VideoController.ShowSaveFileDialog(
                            "Save " + m_FrameType.ToString() + " fame", 
							"FITS Image (*.fit)|*.fit", 
							ref fileName) == DialogResult.OK)
						{
							string notes = string.Format("{0} generated from {1}", m_FrameType.ToString(), Path.GetFileNameWithoutExtension(m_VideoController.CurrentVideoFileName));

							SaveDarkOrFlatFrame(fileName, currPixelmap.Width, currPixelmap.Height, notes, m_AveragedData, m_ExposureSeconds, m_NumFrames);
						}

						m_Running = false;
						m_VideoController.StopVideo();

						if (m_FrameType == FrameType.Dark) UsageStats.Instance.DarkFramesProduced++;
						if (m_FrameType == FrameType.MasterFlat) UsageStats.Instance.FlatFramesProduced++;

						UsageStats.Instance.Save();
                    }
                }
            }
        }

		internal static void SaveDarkOrFlatFrame(string fileName, int width, int height, string notes, float[,] averagedData, float exposureSeconds, int numFrames)
		{
			Fits f = new Fits();

			object data = SaveImageData(width, height, averagedData);

			BasicHDU imageHDU = Fits.MakeHDU(data);

			nom.tam.fits.Header hdr = imageHDU.Header;
			hdr.AddValue("SIMPLE", "T", null);

			hdr.AddValue("BITPIX", -32 /* Floating Point Data*/, null);
			hdr.AddValue("NAXIS", 2, null);
			hdr.AddValue("NAXIS1", width, null);
			hdr.AddValue("NAXIS2", height, null);

			
			if (notes.Length > HeaderCard.MAX_VALUE_LENGTH) notes = notes.Substring(0, HeaderCard.MAX_VALUE_LENGTH);
			hdr.AddValue("NOTES", notes, null);

			if (exposureSeconds > 0)
			{
				hdr.AddValue("EXPOSURE", exposureSeconds.ToString("0.000", CultureInfo.InvariantCulture), null);
				hdr.AddValue("EXPTIME", exposureSeconds.ToString("0.000", CultureInfo.InvariantCulture), null);
			}

			hdr.AddValue("SNAPSHOT", numFrames.ToString(), null);
			hdr.AddValue("TANGRAVE", string.Format("{0} v{1}", VersionHelper.AssemblyProduct, VersionHelper.AssemblyFileVersion), null);
			if (TangraConfig.Settings.Generic.ReverseGammaCorrection)
				hdr.AddValue("TANGAMMA", TangraConfig.Settings.Photometry.EncodingGamma.ToString("0.0000", CultureInfo.InvariantCulture), null);
			if (TangraConfig.Settings.Generic.ReverseCameraResponse)
				hdr.AddValue("TANCMRSP", ((int)TangraConfig.Settings.Photometry.KnownCameraResponse).ToString(CultureInfo.InvariantCulture), null);

			f.AddHDU(imageHDU);

			// Write a FITS file.
			using (BufferedFile bf = new BufferedFile(fileName, FileAccess.ReadWrite, FileShare.ReadWrite))
			{
				f.Write(bf);
				bf.Flush();
			}
	    }

		private static float[][] SaveImageData(int width, int height, float[,] data)
		{
			float[][] bimg = new float[height][];

			for (int y = 0; y < height; y++)
			{
				bimg[y] = new float[width];

				for (int x = 0; x < width; x++)
				{
                    bimg[y][x] = (float)Math.Max(0, data[x, height - y - 1]);
				}
			}

			return bimg;
		}

        public void ImageToolChanged(ImageTool newTool, ImageTool oldTool)
        { }

        public void PreDraw(System.Drawing.Graphics g)
        { }

        public void PostDraw(System.Drawing.Graphics g)
        { }

        public bool HasCustomZoomImage { get { return false; } }

        public bool DrawCustomZoomImage(Graphics g, int width, int height)
        {
	        return false;
        }

        #endregion

		internal void SelectedFrameTypeChanged(FrameType frameType)
		{
            TangraContext.Current.CanLoadDarkFrame = frameType == FrameType.MasterFlat;
            TangraContext.Current.CanLoadBiasFrame = frameType != FrameType.MasterBias && frameType != FrameType.Dark;

			m_VideoController.UpdateViews();
		}

        internal bool CanStartProducingFrame(FrameType frameType, int numFramesToTake)
		{
            if (frameType == FrameType.MasterFlat &&
				!m_MakeDarkFlatController.HasDarkFrameBytes)
			{
				m_VideoController.ShowMessageBox(
					"Please load a dark frame from the 'Reduction' -> 'Load Calibration Frame' menu in order to dark frame correct the produced master flat frame. You may need to produce your dark frame first.", 
					"Dark frame required",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error);
				return false;
			}

            if (frameType == FrameType.MasterDark &&
                !m_MakeDarkFlatController.HasBiasFrameBytes)
            {
                m_VideoController.ShowMessageBox(
                    "Please load a bias frame from the 'Reduction' -> 'Load Calibration Frame' menu in order to bias frame correct the produced master dark frame. You may need to produce your bias frame first.",
                    "Bias frame required",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return false;
            }

			if (m_VideoController.VideoLastFrame - m_FrameNumber < numFramesToTake)
			{
				m_VideoController.ShowMessageBox(
					"The operation cannot start because there are insufficient number of frames remaining until the end of the video.",
					"Dark frame required",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error);
				return false;
			}

			return true;
		}

        internal void StartProducingFrame(FrameType frameType, int framesToAverage, float effectiveExposureSeconds)
        {
            m_NumFrames = framesToAverage;
            m_FramesDone = 0;
            m_ExposureSeconds = effectiveExposureSeconds;
            m_FrameType = frameType;

            m_Running = true;
            m_VideoController.PlayVideo();
			
			TangraContext.Current.CanScrollFrames = false;
			TangraContext.Current.CanPlayVideo = false;
			m_VideoController.UpdateViews();

            m_ControlPanel.SetRunning(m_NumFrames);
        }

        internal void CancelProducingFrame()
        {
            m_Running = false;
            m_ControlPanel.SetStopped();

			m_VideoController.StopVideo();

			TangraContext.Current.CanScrollFrames = true;
			TangraContext.Current.CanPlayVideo = true;
			m_VideoController.UpdateViews();
        }


		public void FinalizeOperation()
		{ }

		public bool AvoidImageOverlays
		{
			get
			{
				// No overlays allowed during the whole process
				return true;
			}
		}
	}
}
