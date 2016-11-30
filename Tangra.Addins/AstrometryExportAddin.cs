using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.SDK;

namespace Tangra.Addins
{
    [Serializable]
    public class AstrometryExportAddin : MarshalByRefObject, ITangraAddinAction
    {
        private ITangraHost m_Host;

        public void Initialise(ITangraHost host)
        {
            m_Host = host;
        }

        public void Finalise()
        { }

        public string DisplayName
        {
            get { return "Astrometry CSV Export"; }
        }

        public AddinActionType ActionType
        {
            get { return AddinActionType.Astrometry; }
        }

        public IntPtr Icon
        {
            get { return IntPtr.Zero; }
        }

        public int IconTransparentColorARGB
        {
            get { return Color.Transparent.ToArgb(); }
        }

        private ITangraAstrometricSolution2 m_LastSolution;

        public void Execute()
        {
            ITangraAstrometricSolution2 solution = m_Host.GetAstrometryProvider().GetCurrentFrameAstrometricSolution() as ITangraAstrometricSolution2;
            if (solution != null)
                m_LastSolution = solution;
        }

        internal void OnBeginMultiFrameAstrometry()
        {
            m_LastSolution = null;
        }

        internal void OnEndMultiFrameAstrometry()
        {
            if (m_LastSolution != null)
            {
				var output = new StringBuilder();
				output.Append("FrameNo, TimeUTC, RADeg, DEDeg, StdDevRAArcSec, StdDevDEArcSec, Mag\r\n");

                var meaList = m_LastSolution.GetAllMeasurements();
                foreach (var mea in meaList)
                {
                    output.AppendFormat("{0}, {1}, {2}, {3}, {4}, {5}, {6}\r\n", 
                        mea.FrameNo, mea.OCRedTimeStamp.HasValue ? (double?)mea.OCRedTimeStamp.Value.TimeOfDay.TotalDays : null, 
                        mea.RADeg, mea.DEDeg, mea.StdDevRAArcSec, mea.StdDevDEArcSec, mea.Mag);
				}

				var dialog = new SaveFileDialog();
				dialog.Filter = "Comma Separated Values (*.csv)|*.csv|All Files (*.*)|*.*";
				dialog.DefaultExt = "csv";
				dialog.Title = "Export Tangra Astrometry";

				if (dialog.ShowDialog(m_Host.ParentWindow) == DialogResult.OK)
				{
					File.WriteAllText(dialog.FileName, output.ToString());
					Process.Start(dialog.FileName);
				}
            }
        }
    }
}
