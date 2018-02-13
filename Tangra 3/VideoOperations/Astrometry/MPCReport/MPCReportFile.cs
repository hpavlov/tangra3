/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Config;
using Tangra.Model.Config;
using Tangra.Model.Helpers;

namespace Tangra.VideoOperations.Astrometry.MPCReport
{
    public interface IMPCReportFileManager
    {
        void CloseReportFile();
    }

	public class MPCReportFile
	{
        private static List<string> HEADER_TOKENS = new List<string>(new string[] { "COD", "CON", "OBS", "MEA", "TEL", "ACK", "AC2", "COM", "NET" });

		private string lastObjectDesignation;

		public string ReportFileName;
	    public MPCObsHeader Header;
	    public List<MPCObsLine> ObsLines = new List<MPCObsLine>();

	    public RovingObsLocation RovingObservatoryLocation = new RovingObsLocation() { IsValid = false };

        public MPCReportFile(string fileName, Func<RovingObsLocation> rovingObsLocationProvider)
        {
            ReportFileName = fileName;
            string[] allLines = File.ReadAllLines(fileName);

            Header = new MPCObsHeader(allLines);
            if (Header.COD == MPCObsLine.ROVING_OBS_CODE)
            {
                RovingObservatoryLocation = rovingObsLocationProvider();
            }
            ObsLines.Clear();

            int strippedLines = 0;

            foreach(string line in allLines)
            {
                if (line.Length > 3 && (ObsLines.Count > 0 || HEADER_TOKENS.IndexOf(line.Substring(0, 3)) == -1))
                {
                    // Parse observation line
                    MPCObsLine obsLine = MPCObsLine.Parse(line);
                    if (obsLine != null)
                        ObsLines.Add(obsLine);
                    else
                        strippedLines++;
                }
            }

            if (strippedLines > 0)
                MessageBox.Show(string.Format("{0} lines could not be parsed are have been stripped from the file.", strippedLines), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

	    private MPCReportFile()
	    { }

	    public MPCReportFile(string fileName, MPCObsHeader header, RovingObsLocation rovingObsLocation)
		{
		    ReportFileName = fileName;
		    Header = header;
		    RovingObservatoryLocation = rovingObsLocation;
		}

		public string LastObjectDesignation
		{
			get { return lastObjectDesignation; }
		}

		public void Save()
		{
		    string content = ToAscii();
            File.WriteAllText(ReportFileName, content);
		}

        public bool AddObservation(
			string designation, double raHours, double deDeg, 
			DateTime utcTime, TimeSpan timePrecision, double mag,
            MagnitudeBand band, bool isVideoNormalPosition, double? raUncertaintyArcSec, double? deUncertaintyArcSec)
		{
			lastObjectDesignation = designation;

			MPCObsLine obsLine = new MPCObsLine(Header.COD);
            obsLine.SetObject(designation);
            obsLine.SetPosition(raHours, deDeg, utcTime, isVideoNormalPosition);
            obsLine.SetMagnitude(mag, band);
            obsLine.SetUncertainty(raUncertaintyArcSec, deUncertaintyArcSec);

            if (Header.COD == MPCObsLine.ROVING_OBS_CODE)
            {
                string newLine1 = obsLine.BuildRovingObserverLine1();
                if (ObsLines.Exists(l => newLine1.Equals(l.BuildObservationASCIILine(), StringComparison.Ordinal)))
                    return false;
            }
            else
            {
                string newLine = obsLine.BuildObservationASCIILine();
                if (ObsLines.Exists(l => newLine.Equals(l.BuildObservationASCIILine(), StringComparison.Ordinal)))
                    return false;
            }

            ObsLines.Add(obsLine);
            return true;
		}

	    public bool AddObservation(string reportLine)
	    {
	        try
	        {
	            if (reportLine.Length > 80) 
                    reportLine = reportLine.Substring(0, 80);

                var obsLine = MPCObsLine.Parse(reportLine);
                if (obsLine != null)
                    ObsLines.Add(obsLine);

                return true;
	        }
	        catch (Exception ex)
	        {
	            Trace.WriteLine(ex.GetFullStackTrace());
	            return false;
	        }
	    }

        public string ToAscii()
        {
            StringBuilder output = new StringBuilder();

            output.AppendLine(Header.ToAscii());

            output.AppendLine("");

            foreach (MPCObsLine obsLine in ObsLines)
            {
                if (Header.COD == MPCObsLine.ROVING_OBS_CODE && RovingObservatoryLocation != null && RovingObservatoryLocation.IsValid)
                {
                    output.AppendLine(obsLine.BuildRovingObserverLine1());
                    output.AppendLine(obsLine.BuildRovingObserverLine2(RovingObservatoryLocation));
                }
                else
                {
                    output.AppendLine(obsLine.BuildObservationASCIILine());
                }
            }

            return output.ToString();
        }

        private frmDisplayMPCReport m_DisplayMPCReportForm;

		public void Present(IMPCReportFileManager manager)
		{
            if (m_DisplayMPCReportForm == null || !m_DisplayMPCReportForm.Visible)
            {
                m_DisplayMPCReportForm = new frmDisplayMPCReport(this, manager);
                m_DisplayMPCReportForm.Show();
            }
            else
                m_DisplayMPCReportForm.ParseReportFile(this, manager);
		}
	}
}
