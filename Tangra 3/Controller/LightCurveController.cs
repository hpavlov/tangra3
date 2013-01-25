using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.ImageTools;
using Tangra.Model.Config;
using Tangra.Model.Context;
using Tangra.VideoOperations.LightCurves;

namespace Tangra.Controller
{
    public class LightCurveController
    {
        private Form m_MainFormView;
        private VideoController m_VideoController;

        private frmLightCurve m_LightCurveForm;
        private bool m_lcFileLoaded;

        public LightCurveController(Form mainFormView, VideoController videoController)
        {
            m_MainFormView = mainFormView;
            m_VideoController = videoController;

            m_lcFileLoaded = false;
            m_LightCurveForm = null;
        }

        public void MoveToFrameNoIntegrate(int selectedFrameNo)
        {
            throw new NotImplementedException();
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

        private void OpenLcFile(string fileName)
        {
            m_MainFormView.Cursor = Cursors.WaitCursor;
            LCFile lcFile = null;
            try
            {
                m_MainFormView.Update();

                lcFile = LCFile.Load(fileName);
                if (lcFile != null)
                {
                    string videoFile = GetVideoFileMatchingLcFile(lcFile, fileName);
                    if (!string.IsNullOrEmpty(videoFile))
                    {
                        //NOTE: File found - open it!

                        //OpenAviFile(videoFile, false, false, dontUseWinAVI);

                        //LightCurves glcOp = new LightCurves();
                        //glcOp.EnterViewLightCurveMode(this, lcFile);
                        //m_CurrentOperation = glcOp;
                        //m_FramePreprocessor.Clear();
                        //ActivateImageTool<ArrowTool>();

                        //ApplicationState.Current.CanPlayVideo = false;
                        //ApplicationState.Current.UpdateControlsState();
                    }
                    else
                    {
                        // NOTE: No video file found, just show the saved averaged frame
                        //ApplicationState.Current.Reset();
                        //if (m_CurrentOperation != null)
                        //    m_CurrentOperation.FinalizeOperation();
                        //m_CurrentOperation = null;
                        //m_DefaultArrowImageTool = null;
                        //m_FramePreprocessor.Clear();
                        //pictureBox.Image = null;
                        //VideoContext.Current.Reset();

                        //if (lcFile.Footer.AveragedFrameBytes != null)
                        //{
                        //    IFrameStream videoStream = new SingleBitmapFileFrameStream(lcFile);
                        //    m_FramePlayer.OpenVideo(fileName, videoStream);
                        //    m_TangraApplicationImpl.SetOpenVideo(new TangraOpenVideoImpl(videoStream));

                        //    int width = Math.Max(800, m_FramePlayer.Video.Width + m_ExtraWidth);
                        //    int height = Math.Max(600, m_FramePlayer.Video.Height + m_ExtraHeight);
                        //    this.Width = width;
                        //    this.Height = height;

                        //    SetScrollBarArea();

                        //    VideoContext.Current.FirstFrameIndex = m_FramePlayer.Video.FirstFrame;
                        //    VideoContext.Current.VideoStream = m_FramePlayer.Video;

                        //    if (m_FramePlayer.IsAstroDigitalVideo)
                        //    {
                        //        m_AdvStatusForm = new frmAdvStatusPopup(TangraConfig.Settings.ADVS);
                        //        m_AdvStatusForm.Show(this);
                        //        PositionAdvstatusForm();
                        //    }

                        //    m_FramePlayer.StepForward();

                        //    SetMainFormTitle(m_FramePlayer.FileName, m_FramePlayer.Video.SourceInfo);

                        //    VideoContext.Current.OSDRectToExclude = Rectangle.Empty;

                        //    ApplicationState.Current.HasVideoLoaded = true;

                        //    // Display the saved averaged frame with the targets
                        //    ActivateOperation<MissingVideoViewer>(lcFile);
                        //    ActivateImageTool<ArrowTool>();
                        //}

                        TangraContext.Current.CanPlayVideo = false;
                        TangraContext.Current.CanScrollFrames = false;
                        m_VideoController.UpdateViews();
                    }

                    // Move to the first frame in the light curve
                    m_VideoController.MoveToFrame((int)lcFile.Header.MinFrame);

                    m_lcFileLoaded = true;

                    m_LightCurveForm = new frmLightCurve(this, lcFile, fileName);

                    m_LightCurveForm.Show(m_MainFormView);
                    m_LightCurveForm.Update();

                    RegisterRecentFile(RecentFileType.LightCurve, fileName);
                }
            }
            finally
            {
                m_MainFormView.Cursor = Cursors.Default;
            }
        }

        public void RegisterRecentFile(RecentFileType recentFileType, string fileName)
        {
            throw new NotImplementedException();
            //(m_Host as frmMain).RegisterRecentFile(frmMain.RecentFileType.LightCurve, saveFileDialog.FileName);
        }

        public DialogResult ShowMessageBox(string message, string title, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            throw new NotImplementedException();

            // NOTE: Use method in a shared location
        }
    }
}
