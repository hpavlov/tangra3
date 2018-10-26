using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Tangra.Model.Helpers;

namespace Tangra.Automation
{
    public enum AutomationCommand
    {
        None,
        IntegrationDetection
    }

    public class AutomationManager
    {
        private Dictionary<string, string> m_Arguments;

        private AutomationCommand m_CurrentCommand;

        public AutomationManager(string[] args)
        {
            m_Arguments = CommandLineParser.Parse(args);
        }

        public bool IsAutomationCommand
        {
            get { return m_Arguments.ContainsKey("a") && IsKnownAutomationCommand(m_Arguments["a"]); }
        }

        private bool IsKnownAutomationCommand(string command)
        {
            foreach (var cmd in Enum.GetValues(typeof (AutomationCommand)))
            {
                if (string.Equals(cmd.ToString(), command, StringComparison.InvariantCultureIgnoreCase))
                {
                    m_CurrentCommand = (AutomationCommand)cmd;
                    return true;
                }                
            }

            return false;
        }

        public void Run()
        {
            switch (m_CurrentCommand)
            {
                case AutomationCommand.IntegrationDetection:
                    RunIntegrationDetection();
                    break;
            }
        }

        private void RunIntegrationDetection()
        {
            using (var instrDetection = new AutomatedIntegrationDetection(m_Arguments))
            {
                instrDetection.Run();
            }
        }
    }
}
