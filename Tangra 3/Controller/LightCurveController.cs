using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Helpers;
using Tangra.ImageTools;
using Tangra.Model.Config;
using Tangra.Model.Context;
using Tangra.Model.Video;
using Tangra.Video;
using Tangra.Video.AstroDigitalVideo;
using Tangra.VideoOperations.LightCurves;

namespace Tangra.Controller
{
    public class LightCurveController
    {
        private Form m_MainFormView;
        private VideoController m_VideoController;

        private frmLightCurve m_LightCurveForm;
	    private AddinsController m_AddinsController;
        private bool m_lcFileLoaded;

		public LightCurveController(Form mainFormView, VideoController videoController, AddinsController addinsController)
        {
            m_MainFormView = mainFormView;
            m_VideoController = videoController;
			m_AddinsController = addinsController;

            m_lcFileLoaded = false;
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

        private bool TestWhetherVideoFileMatchesLcHeader(string fileName, LCMeasurementHeader header)
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

                lcFile = LCFile.Load(fileName);
                if (lcFile != null)
                {
					ReduceLightCurveOperation operation = (ReduceLightCurveOperation)m_VideoController.SetOperation<ReduceLightCurveOperation>(this, false);

                    string videoFile = GetVideoFileMatchingLcFile(lcFile, fileName);
                    if (!string.IsNullOrEmpty(videoFile) &&
						File.Exists(videoFile))
                    {
						if (m_VideoController.OpenVideoFile(videoFile))
						{							
							TangraContext.Current.CanPlayVideo = false;
							m_VideoController.UpdateViews();							
						}
                    }
                    else
                    {
                        // NOTE: No video file found, just show the saved averaged frame
						TangraContext.Current.Reset();

						if (lcFile.Footer.AveragedFrameBytes != null)
						{
							if (m_VideoController.SingleBitmapFile(lcFile))
							{							
								TangraContext.Current.CanPlayVideo = false;
								m_VideoController.UpdateViews();
							}
						}

                        TangraContext.Current.CanPlayVideo = false;
                        TangraContext.Current.CanScrollFrames = false;
                        m_VideoController.UpdateViews();
                    }

                    m_lcFileLoaded = true;

					m_LightCurveForm = new frmLightCurve(this, m_AddinsController, lcFile, fileName);
					m_LightCurveForm.SetGeoLocation(m_VideoController.GeoLocation);
                    m_LightCurveForm.Show(m_MainFormView);
                    m_LightCurveForm.Update();

					// TODO: Review the VideoController-LightCurveController-ReduceLightCurveOperation relation and how they are initialized
					// TODO: Provide a clean way of initializing the controller/operation state when opening an .lc file!
					operation.EnterViewLightCurveMode(lcFile, m_VideoController, m_VideoController.m_pnlControlerPanel);

                    RegisterRecentFile(RecentFileType.LightCurve, fileName);

					if (!string.IsNullOrEmpty(m_VideoController.CurrentVideoFileType))
					{
						// Move to the first frame in the light curve
						m_VideoController.MoveToFrame((int)lcFile.Header.MinFrame);
					}
                }
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
            saveFileDialog.InitialDirectory = Path.GetDirectoryName(m_VideoController.CurrentVideoFileName);
            saveFileDialog.FileName = Path.ChangeExtension(Path.GetFileName(m_VideoController.CurrentVideoFileName), ".lc");
        }
    }
}
