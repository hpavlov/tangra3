/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
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
    public class MakeDarkFlatOperation : VideoOperationBase, IVideoOperation
    {
        enum FrameType
        {
            Dark,
            Flat
        }

        enum AveragingType
        {
            Median,
            Mean
        }

	    private DarkFlatFrameController m_MakeDarkFlatController;
	    private IVideoController m_VideoController;
        private ucMakeDarkFlatField m_ControlPanel = null;
        private object m_SyncRoot = new object();

        private int m_NumFrames = 64;
        private int m_FramesDone = 0;
        private bool m_Running = false;
        private FrameType m_FrameType;
        private AveragingType m_AveragingType;

	    private AstroImage m_CurrentAstroImage;

        private ulong[,] m_SummedBitmap;

	    public MakeDarkFlatOperation()
	    { }

	    public MakeDarkFlatOperation(DarkFlatFrameController makeDarkFlatController)
		{
			m_MakeDarkFlatController = makeDarkFlatController;
		}

        #region IVideoOperation Members

		public bool InitializeOperation(IVideoController videoController, Panel controlPanel, IFramePlayer framePlayer, Form topForm)
        {
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

			TangraContext.Current.CanLoadFlatFrame = false;
			TangraContext.Current.CanLoadDarkFrame = false;
        
            return true;
        }

        public void PlayerStarted()
        { }

        public void NextFrame(int frameNo, MovementType movementType, bool isLastFrame, AstroImage astroImage, int firstFrameInIntegrationPeriod, string frameFileName)
		{
			m_CurrentAstroImage = astroImage;
            TangraContext.Current.CrashReportInfo.FrameNumber = frameNo;

            if (m_Running)
            {
                if (m_FramesDone < m_NumFrames)
                {
					Pixelmap currPixelmap = astroImage.Pixelmap;
                    if (m_FramesDone == 0)
                        m_SummedBitmap = new ulong[currPixelmap.Width, currPixelmap.Height];

                    for (int x = 0; x < currPixelmap.Width; x++)
                        for (int y = 0; y < currPixelmap.Height; y++)
                        {
                            m_SummedBitmap[x, y] += currPixelmap[x, y];
                        }

                    m_FramesDone++;
                    m_ControlPanel.SetProgress(m_FramesDone);

                    if (m_FramesDone == m_NumFrames)
                    {
						if (currPixelmap.BitPixCamera <= 8)
						{
							Bitmap field = new Bitmap(currPixelmap.Width, currPixelmap.Height);

							// Averaging
							for (int x = 0; x < currPixelmap.Width; x++)
								for (int y = 0; y < currPixelmap.Height; y++)
								{
									byte bt = (byte)(m_SummedBitmap[x, y] / (uint)m_FramesDone);

									field.SetPixel(x, y, Color.FromArgb(bt, bt, bt));
								}

							m_ControlPanel.SetStopped();

							string fileName = string.Format(
								"{0}_{1}.bmp",
								Path.GetFileNameWithoutExtension(m_VideoController.CurrentVideoFileName),
								m_FrameType == FrameType.Dark ? "DARK" : "FLAT");

							if (m_VideoController.ShowSaveFileDialog(
								"Save " + (m_FrameType == FrameType.Dark ? "dark" : "flat") + " fame", 
								"Bitmaps (*.bmp)|*.bmp",
								 ref fileName) == DialogResult.OK)
							{
								field.Save(fileName, ImageFormat.Bmp);
								Process.Start(fileName);
							}

							m_Running = false;
							m_VideoController.StopVideo();					
						}
						else
						{
							// Averaging
							for (int x = 0; x < currPixelmap.Width; x++)
							for (int y = 0; y < currPixelmap.Height; y++)
							{
								m_SummedBitmap[x, y] = (m_SummedBitmap[x, y] / (ulong)m_FramesDone);
							}

							m_ControlPanel.SetStopped();

							string fileName = string.Format(
								"{0}_{1}.fit",
								Path.GetFileNameWithoutExtension(m_VideoController.CurrentVideoFileName),
								m_FrameType == FrameType.Dark ? "DARK" : "FLAT");

							if (m_VideoController.ShowSaveFileDialog(
								"Save " + (m_FrameType == FrameType.Dark ? "dark" : "flat") + " fame", 
								"FITS Image 16 bit (*.fit)|*.fit", 
								ref fileName) == DialogResult.OK)
							{
								Fits f = new Fits();
								
								object data = SaveImageData(m_SummedBitmap); 

								BasicHDU imageHDU = Fits.MakeHDU(data);

								nom.tam.fits.Header hdr = imageHDU.Header;
								hdr.AddValue("SIMPLE", "T", null);

								hdr.AddValue("BITPIX", 16, null);
								hdr.AddValue("NAXIS", 2, null);
								hdr.AddValue("NAXIS1", currPixelmap.Width, null);
								hdr.AddValue("NAXIS2", currPixelmap.Height, null);
								hdr.AddValue("NOTES", string.Format(
									"{0} generated from {1}", 
									m_FrameType == FrameType.Dark ? "DARK" : "FLAT", 
									Path.GetFileNameWithoutExtension(m_VideoController.CurrentVideoFileName)), null);

								f.AddHDU(imageHDU);

								// Write a FITS file.
								using (BufferedFile bf = new BufferedFile(fileName, FileAccess.ReadWrite, FileShare.ReadWrite))
								{
									f.Write(bf);
									bf.Flush();
								}
							}

							m_Running = false;
							m_VideoController.StopVideo();
						}

						if (m_FrameType == FrameType.Dark) UsageStats.Instance.DarkFramesProduced++;
						if (m_FrameType == FrameType.Flat) UsageStats.Instance.FlatFramesProduced++;

						UsageStats.Instance.Save();
                    }
                }
            }
        }

		private short[][] SaveImageData(ulong[,] data)
		{
            short[][] bimg = new short[m_CurrentAstroImage.Height][];

			for (int y = 0; y < m_CurrentAstroImage.Height; y++)
			{
                bimg[y] = new short[m_CurrentAstroImage.Width];

				for (int x = 0; x < m_CurrentAstroImage.Width; x++)
				{
                    bimg[y][x] = (short)Math.Min(ushort.MaxValue, Math.Max(0, data[x, m_CurrentAstroImage.Height - y - 1]));
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

		internal void SelectedFrameTypeChanged(bool isFlatFrame)
		{
			TangraContext.Current.CanLoadDarkFrame = isFlatFrame;
			m_VideoController.UpdateViews();
		}

		internal bool CanStartProducingFrame(bool isFlatFrame)
		{
			if (isFlatFrame &&
				!m_MakeDarkFlatController.HasDarkFrameBytes)
			{
				m_VideoController.ShowMessageBox(
					"Please load a dark frame from the 'Video Actions' -> 'Load a Dark Frame' menu in order to dark frame correct the produced flat frame. You may need to produce your dark frame first.", 
					"Dark frame required",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error);
				return false;
			}

			return true;
		}

        internal void StartProducingFrame(bool darkRatherThanFlat, bool meanRatherThanMedianm, int framesToAverage)
        {
            m_NumFrames = framesToAverage;
            m_FramesDone = 0;
            m_FrameType = darkRatherThanFlat ? FrameType.Dark : FrameType.Flat;
            m_AveragingType = meanRatherThanMedianm ? AveragingType.Mean : AveragingType.Median;

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
