using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Controller;
using Tangra.SDK;

namespace Tangra.VideoOperations.Astrometry.MotionFitting
{
    public static class AstrometryExporter
    {
        public static string ExportAstrometry(ITangraAstrometricSolution2 solution, VideoController videoController)
        {
            string fileName = Path.ChangeExtension(videoController.FileName, ".csv");
            var meaList = solution.GetAllMeasurements();

            var output = new StringBuilder();
            output.AppendLine("Tangra Astrometry Export v1.0");
            output.AppendLine("FilePath, Date, InstrumentalDelay, DelayUnits, IntegratedFrames, IntegratedExposure(sec), FrameTimeType, NativeVideoFormat, ObservatoryCode, Object, ArsSecsInPixel");
            output.AppendLine(string.Format("\"{0}\",{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}",
                fileName,
                meaList.Count > 0 && meaList[0].UncorrectedTimeStamp.HasValue ? meaList[0].UncorrectedTimeStamp.Value.ToString("yyyy-MM-dd") : null,
                solution.InstrumentalDelay,
                solution.InstrumentalDelayUnits,
                solution.IntegratedFramesCount,
                solution.IntegratedExposureSeconds,
                solution.FrameTimeType,
                solution.NativeVideoFormat,
                solution.ObservatoryCode,
                solution.ObjectDesignation,
                solution.ArcSecsInPixel));

            output.Append("FrameNo, TimeUTC(Uncorrected), Timestamp, RADeg, DEDeg, Mag, SolutionUncertaintyRA*Cos(DE)[arcsec], SolutionUncertaintyDE[arcsec], FWHM[arcsec], DetectionCertainty, SNR\r\n");

            foreach (var mea in meaList)
            {
                output.AppendFormat("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}\r\n",
                    mea.FrameNo,
                    mea.UncorrectedTimeStamp.HasValue ? (double?)mea.UncorrectedTimeStamp.Value.TimeOfDay.TotalDays : null,
                    mea.UncorrectedTimeStamp.HasValue ? mea.UncorrectedTimeStamp.Value.ToString("[HH:mm:ss.fff]") : null,
                    mea.RADeg, mea.DEDeg, mea.Mag, mea.SolutionUncertaintyRACosDEArcSec, mea.SolutionUncertaintyDEArcSec, mea.FWHMArcSec, mea.Detection, mea.SNR);
            }

            if (videoController.ShowSaveFileDialog(
                "Export Tangra Astrometry",
                "Comma Separated Values (*.csv)|*.csv|All Files (*.*)|*.*",
                ref fileName) == DialogResult.OK)
            {
                File.WriteAllText(fileName, output.ToString());
                Process.Start(fileName);
                return fileName;
            }
            else
            {
                fileName = Path.ChangeExtension(Path.GetTempFileName(), ".csv");
                File.WriteAllText(fileName, output.ToString());
                return fileName;
            }
        }

    }
}
