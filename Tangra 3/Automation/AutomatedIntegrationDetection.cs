using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tangra.Automation.IntegrationDetection;
using Tangra.Controller;
using Tangra.Video;

namespace Tangra.Automation
{
    public class AutomatedIntegrationDetection : IDisposable
    {
        private Dictionary<string, string> m_Arguments;
        private IntegrationDetectionController m_Controller;
        private IImagePixelProvider m_PixelImageProvider;

        private PixDetFile m_PixDetFile;
        private PotentialIntegrationFit m_ExpectedFit;

        private int m_StartFrameNo;
        private string m_RecordFileName;
        private string m_VideoFileName;
        private bool m_RecordPixelFile;

        private ManualResetEvent m_RunFinished = new ManualResetEvent(false);

        public AutomatedIntegrationDetection(Dictionary<string, string> arguments)
        {
            m_Arguments = arguments;
        }

        public void Run()
        {
            if (!m_Arguments.TryGetValue("f", out m_VideoFileName))
            {
                Console.Error.WriteLine("-f must be specified");
                return;
            }

            if (!File.Exists(m_VideoFileName))
            {
                Console.Error.WriteLine(string.Format("File '{0}' cannot be found.", m_VideoFileName));
                return;
            }

            if (".pixdet".Equals(Path.GetExtension(m_VideoFileName), StringComparison.InvariantCultureIgnoreCase))
            {
                var pixdet = new PixDetFile(m_VideoFileName, FileAccess.Read);
                m_ExpectedFit = pixdet.GetStoredIntegrationFit();
                m_PixelImageProvider = pixdet;
            }
            else
            {
                m_ExpectedFit = null;

                var videoPlayer = new AutomationVideoPlayer();
                if (!videoPlayer.OpenVideo(m_VideoFileName))
                {
                    Console.Error.WriteLine(string.Format("Cannot open file '{0}'.", m_VideoFileName));
                    return;
                }
                m_PixelImageProvider = videoPlayer;
            }

            string sfStr;
            m_StartFrameNo = 0;
            if (!m_Arguments.TryGetValue("sf", out sfStr) || !int.TryParse(sfStr, out m_StartFrameNo))
            {
                m_StartFrameNo = m_PixelImageProvider.FirstFrame;
            }

            string pixFile;
            m_RecordPixelFile = false;
            if (m_Arguments.TryGetValue("pix", out pixFile))
            {
                if (!Path.IsPathRooted(pixFile))
                    pixFile = Path.GetFullPath(@".\" + pixFile);
                var dirName = Path.GetDirectoryName(pixFile);

                if (!Directory.Exists(dirName))
                {
                    Console.Error.WriteLine(string.Format("Recording directory '{0}' does not exist", dirName));
                    return;                    
                }

                m_RecordFileName = pixFile;
                m_RecordPixelFile = true;
                m_PixDetFile = new PixDetFile(m_RecordFileName, FileAccess.Write);
                m_PixDetFile.WriteHeader(m_VideoFileName, m_StartFrameNo);
            }

            m_Controller = new IntegrationDetectionController(m_PixelImageProvider, m_StartFrameNo);
            m_Controller.OnPotentialIntegration += OnPotentialIntegration;
            m_Controller.OnFramePixels += m_Controller_OnFramePixels;   

            m_Controller.RunMeasurements();
            m_RunFinished.WaitOne();
        }

        private void m_Controller_OnFramePixels(int frameNo, int[,] pixels)
        {
            if (m_RecordPixelFile)
            {
                m_PixDetFile.AddFramePixels(frameNo, pixels);
            }
        }

        private void OnPotentialIntegration(PotentialIntegrationFit fit)
        {
            if (m_RecordPixelFile)
            {
                m_PixDetFile.WriteFooter(fit.Interval, fit.StartingAtFrame, fit.Certainty, fit.AboveSigmaRatio);
                m_PixDetFile.Dispose();
            }

            if (m_ExpectedFit != null)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine();
                Console.WriteLine("[Integration Detection] " + m_VideoFileName);

                Console.ForegroundColor = m_ExpectedFit.Interval == fit.Interval ? ConsoleColor.Green : ConsoleColor.Red;
                Console.WriteLine("[Detected Interval] Expected:{0} Actual:{1}", m_ExpectedFit.Interval, fit.Interval);

                Console.ForegroundColor = m_ExpectedFit.StartingAtFrame == fit.StartingAtFrame ? ConsoleColor.Green : ConsoleColor.Red;
                Console.WriteLine("[Starting At Frame] Expected:{0} Actual:{1}", m_ExpectedFit.StartingAtFrame, fit.StartingAtFrame);

                Console.ForegroundColor = Math.Abs(m_ExpectedFit.Certainty - fit.Certainty) < 0.1 ? ConsoleColor.Green : ConsoleColor.Red;
                Console.WriteLine("[Certainty] Expected:{0} Actual:{1}", m_ExpectedFit.Certainty, fit.Certainty);

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine();
            }
            else
            {
                // TODO: Console.Write (green = detected; red = failed to detect)
            }

            m_RunFinished.Set();
        }

        public void Dispose()
        {
            var disp = m_PixelImageProvider as IDisposable;
            if (disp != null)
            {
                disp.Dispose();
            }

            if (m_Controller != null)
            {
                m_Controller.OnFramePixels -= m_Controller_OnFramePixels;
                m_Controller.OnPotentialIntegration -= OnPotentialIntegration;
                m_Controller.Dispose();
                m_Controller = null;
            }
        }
    }
}
