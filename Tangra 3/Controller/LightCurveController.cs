/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Helpers;
using Tangra.ImageTools;
using Tangra.Model.Astro;
using Tangra.Model.Config;
using Tangra.Model.Context;
using Tangra.Model.Image;
using Tangra.Model.Video;
using Tangra.OCR;
using Tangra.Video;
using Tangra.Video.AstroDigitalVideo;
using Tangra.VideoOperations.LightCurves;
using Tangra.VideoOperations.LightCurves.Tracking;

namespace Tangra.Controller
{
    public class LightCurveController
    {
        private Form m_MainFormView;
        private VideoController m_VideoController;

        private frmLightCurve m_LightCurveForm;
	    private AddinsController m_AddinsController;
		private LCFile m_lcFile = null;

		private LightCurveContext m_Context;
        private OcrExtensionManager m_OcrExtensionManager;

        public LightCurveController(Form mainFormView, VideoController videoController, AddinsController addinsController, OcrExtensionManager ocrExtensionManager)
        {
            m_MainFormView = mainFormView;
            m_VideoController = videoController;
			m_AddinsController = addinsController;
            m_OcrExtensionManager = ocrExtensionManager;

            m_LightCurveForm = null;
        }

        public void MoveToFrameNoIntegrate(int selectedFrameNo)
        {
			m_VideoController.MoveToFrame(selectedFrameNo);
        }

        public void LoadLightCurve()
        {
            var openFileDialog = new OpenFileDialog()
            {
                Filter = "Tangra Light Curve (*.lc)|*.lc",
                CheckFileExists = true
            };

            if (openFileDialog.ShowDialog(m_MainFormView) == DialogResult.OK)
            {
				m_VideoController.CloseOpenedVideoFile();

                OpenLcFile(openFileDialog.FileName);
            }
        }

        private string GetVideoFileMatchingLcFile(LCFile lcFile, string pathToLCFile)
        {
            if (File.Exists(lcFile.Header.PathToVideoFile) &&
                TestWhetherVideoFileMatchesLcHeader(lcFile.Header.PathToVideoFile, lcFile.Header))
                return lcFile.Header.PathToVideoFile;


            string nextGuess = Path.GetFullPath(Path.GetDirectoryName(pathToLCFile) + "\\" + Path.GetFileName(lcFile.Header.PathToVideoFile));
            if (File.Exists(nextGuess) &&
                TestWhetherVideoFileMatchesLcHeader(nextGuess, lcFile.Header))
                return nextGuess;

            nextGuess =
                Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + "\\" + Path.GetFileName(lcFile.Header.PathToVideoFile));
            if (File.Exists(nextGuess) &&
                TestWhetherVideoFileMatchesLcHeader(nextGuess, lcFile.Header))
                return nextGuess;

            return null;
        }

        private static bool TestWhetherVideoFileMatchesLcHeader(string fileName, LCMeasurementHeader header)
        {
            //using (FramePlayer player = new FramePlayer())
            //{
            //    player.OpenVideo(fileName);
            //    if (header.FirstFrameInVideoFile == player.Video.FirstFrame &&
            //        header.CountFrames == player.Video.CountFrames)
            //    {
            //        return true;
            //    }
            //}

            return true;
        }

        public void OpenLcFile(string fileName)
        {
	        var fi = new FileInfo(fileName);
	        double expectedMemoryMbNeeded = 500 /* For Tangra to operate*/ + 20*fi.Length/(1024*1024) /* For the .lc file to be unpacked and loaded in memory */;

			double availableMemoryMb = CrossPlaform.GetAvailableMemoryInMegabytes();

			if (expectedMemoryMbNeeded > availableMemoryMb)
			{
				if (MessageBox.Show(
					m_MainFormView,
					string.Format("Opening this file will require at least {0}Gb of free memory. Do you wish to continue?", (Math.Ceiling(expectedMemoryMbNeeded/512.0)/2).ToString("0.0")),
					"Warning",
					MessageBoxButtons.YesNo,
					MessageBoxIcon.Warning) == DialogResult.No)
				{
					return;
				}
			}
			
            m_MainFormView.Cursor = Cursors.WaitCursor;
            LCFile lcFile = null;
            try
            {
                m_MainFormView.Update();

                lcFile = LCFile.Load(fileName, m_VideoController);
                if (lcFile != null)
                {
                    ReduceLightCurveOperation operation = (ReduceLightCurveOperation)m_VideoController.SetOperation<ReduceLightCurveOperation>(this, m_OcrExtensionManager, false);
                    operation.SetLCFile(lcFile);

                    string videoFile = GetVideoFileMatchingLcFile(lcFile, fileName);
                    if (!string.IsNullOrEmpty(videoFile) &&
                        File.Exists(videoFile))
                    {
                        if (m_VideoController.OpenVideoFile(videoFile, new TangraOpenFileArgs { FrameRate = lcFile.Header.FramesPerSecond, BitPix = lcFile.Footer.DataBitPix, SerTiming = lcFile.Header.SerTimingType }))
                        {
                            TangraContext.Current.CanPlayVideo = false;
                            m_VideoController.UpdateViews();
                        }
                    }
                    else
                    {
                        // NOTE: No video file found, just show the saved averaged frame
                        bool oldCanProcessLightCurvePixels = TangraContext.Current.CanProcessLightCurvePixels;
                        TangraContext.Current.Reset();
                        TangraContext.Current.CanProcessLightCurvePixels = oldCanProcessLightCurvePixels;

                        if (lcFile.Footer.AveragedFrameBytes != null)
                        {
                            if (m_VideoController.SingleBitmapFile(lcFile))
                            {
                                TangraContext.Current.CanPlayVideo = false;
                                m_VideoController.UpdateViews();

                                PSFFit.SetDataRange(lcFile.Footer.DataBitPix, lcFile.Footer.DataAav16NormVal);
                            }
                        }

                        TangraContext.Current.CanPlayVideo = false;
                        TangraContext.Current.CanScrollFrames = false;
                        m_VideoController.UpdateViews();
                    }

                    m_Context = new LightCurveContext(lcFile);
                    m_LightCurveForm = new frmLightCurve(this, m_AddinsController, lcFile, fileName);
                    m_LightCurveForm.SetGeoLocation(m_VideoController.GeoLocation);
                    m_LightCurveForm.Show(m_MainFormView);
                    m_LightCurveForm.Update();

                    // TODO: Review the VideoController-LightCurveController-ReduceLightCurveOperation relation and how they are initialized
                    // TODO: Provide a clean way of initializing the controller/operation state when opening an .lc file!
					operation.EnterViewLightCurveMode(lcFile, m_VideoController, m_VideoController.ControlerPanel);

                    RegisterRecentFile(RecentFileType.LightCurve, fileName);

                    if (!string.IsNullOrEmpty(m_VideoController.CurrentVideoFileType))
                    {
                        // Move to the first frame in the light curve
                        m_VideoController.MoveToFrame((int) lcFile.Header.MinFrame);
                    }

                    TangraContext.Current.FileName = Path.GetFileName(fileName);
                    TangraContext.Current.FileFormat = m_lcFile.Header.SourceInfo;
                    m_VideoController.UpdateViews();
                }
            }
            catch (IOException ioex)
            {
                MessageBox.Show(ioex.Message, "Tangra", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                m_MainFormView.Cursor = Cursors.Default;
            }
        }

		public void OnLightCurveClosed()
		{
			m_VideoController.CloseOpenedVideoFile();
		}

        public void RegisterRecentFile(RecentFileType recentFileType, string fileName)
        {
			m_VideoController.RegisterRecentFile(RecentFileType.LightCurve, fileName);
        }

        public DialogResult ShowMessageBox(string message, string title, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
			return m_VideoController.ShowMessageBox(message, title, buttons, icon);
        }

		internal void SetLcFile(LCFile lcFile)
		{
			m_lcFile = lcFile;
			m_Context = new LightCurveContext(lcFile);

			m_LightCurveForm.SetNewLcFile(lcFile);

			m_LightCurveForm.SetGeoLocation(m_VideoController.GeoLocation);
		}

		public void EnsureLightCurveFormClosed()
		{
			try
			{
				if (m_LightCurveForm != null &&
					m_LightCurveForm.Visible)
				{
					// TODO: Ask if the user wants to save it /* Yes/No options only. No 'Cancel' option*/
					// TODO: This should be moved to a different place and should be tested from all: (1) Closing Tangra's main form, (2) Closing LightCurve form, (3) Switching to a different operation
					m_LightCurveForm.CloseFormDontSendMessage();
				}
			}
			finally
			{
				m_LightCurveForm = null;
			}            
		}

		internal void EnsureLightCurveForm()
		{
			m_LightCurveForm = new frmLightCurve(this, m_AddinsController);

			m_LightCurveForm.Show(m_MainFormView);
			m_LightCurveForm.Update();
		}

		internal void OnNewSelectedMeasurements(LCMeasurement[] selectedMeasurements)
		{
			if (selectedMeasurements != null &&
				selectedMeasurements.Length > 0)
			{
				m_LightCurveForm.OnNewSelectedMeasurements(selectedMeasurements);	
			}
		}

	    internal IAddinContainer LightCurveFormAddinContainer
	    {
		    get { return m_LightCurveForm != null ? m_LightCurveForm as IAddinContainer : null; }
	    }

        internal void ConfigureSaveLcFileDialog(SaveFileDialog saveFileDialog)
        {
            try
			{
				saveFileDialog.InitialDirectory = Path.GetDirectoryName(m_VideoController.CurrentVideoFileName);
				saveFileDialog.FileName = Path.ChangeExtension(Path.GetFileName(m_VideoController.CurrentVideoFileName), ".lc");
			}
            catch { /* In some rare cases m_VideoController.CurrentVideoFileName may throw an error. We want to ignore it. */ }
        }

	    internal LightCurveContext Context
	    {
			get { return m_Context; }
	    }

	    internal LCFile LcFile
	    {
			get { return m_lcFile; }
	    }
		
		internal void ClearContext()
		{
			m_Context = null;
			
		}

		internal void ReloadAllReadingsFromLcFile()
		{
			if (m_lcFile != null)
			{
				if (m_lcFile.Header.MeasuredFrames > 0)
				{
					for (int i = 0; i < m_lcFile.Header.ObjectCount; i++)
					{
						m_Context.AllReadings[i] = m_lcFile.Data[i];

						/* This compares the pixel values saved in the .LC file with those from the video file (when the video is present)
						TrackedObjectConfig cfg = m_lcFile.Footer.TrackedObjects[i];
						for (uint j = m_lcFile.Header.MinFrame; j <= m_lcFile.Header.MaxFrame; j++)
						{
							LCMeasurement mea = m_Context.AllReadings[i][(int)(j - m_lcFile.Header.MinFrame)];
							Pixelmap frame = m_VideoController.GetFrame((int) j);
							AstroImage img = new AstroImage(frame);

							uint[,] videoPixels = img.GetMeasurableAreaPixels(mea.PixelDataX0, mea.PixelDataY0, 35);

							for (int x = 0; x < 35; x++)
							for (int y = 0; y < 35; y++)
							{
								uint videoPixel = videoPixels[x, y];
								uint meaPixel = mea.PixelData[x, y];
								if (videoPixel != meaPixel)
								{
									Trace.Assert(false);
								}
							}
						}
						*/
					}
				}
			}
		}

		internal void ApplyDisplayModeAdjustments(Bitmap displayBitmap)
		{
			m_VideoController.ApplyDisplayModeAdjustments(displayBitmap);
		}

    }
}
