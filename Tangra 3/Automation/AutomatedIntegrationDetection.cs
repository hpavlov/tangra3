using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tangra.Controller;

namespace Tangra.Automation
{
    public class AutomatedIntegrationDetection : IDisposable
    {
        private Dictionary<string, string> m_Arguments;
        private IntegrationDetectionController m_Controller;
        private AutomationVideoPlayer m_VideoPlayer;

        private ManualResetEvent m_RunFinished = new ManualResetEvent(false);

        public AutomatedIntegrationDetection(Dictionary<string, string> arguments)
        {
            m_Arguments = arguments;
            m_VideoPlayer = new AutomationVideoPlayer();
        }

        public void Run()
        {
            string videoFile;
            if (!m_Arguments.TryGetValue("f", out videoFile))
            {
                Console.Error.WriteLine("-f must be specified");
                return;
            }

            if (!File.Exists(videoFile))
            {
                Console.Error.WriteLine(string.Format("File '{0}' cannot be found.", videoFile));
                return;
            }

            if (!m_VideoPlayer.OpenVideo(videoFile))
            {
                Console.Error.WriteLine(string.Format("Cannot open file '{0}'.", videoFile));
                return;                
            }

            string sfStr;
            int startFrame = 0;
            if (!m_Arguments.TryGetValue("sf", out sfStr) || !int.TryParse(sfStr, out startFrame))
            {
                startFrame = m_VideoPlayer.FirstFrame;
            }

            m_Controller = new IntegrationDetectionController(m_VideoPlayer, startFrame);
            m_Controller.OnPotentialIntegration += OnPotentialIntegration;

            m_Controller.RunMeasurements();
            m_RunFinished.WaitOne();
        }

        void OnPotentialIntegration(PotentialIntegrationFit fit)
        {
            m_RunFinished.Set();
        }

        public void Dispose()
        {
            m_VideoPlayer.Dispose();

            if (m_Controller != null)
            {
                m_Controller.OnPotentialIntegration -= OnPotentialIntegration;
                m_Controller.Dispose();
                m_Controller = null;
            }
        }
    }
}
