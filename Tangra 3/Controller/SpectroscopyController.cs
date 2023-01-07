using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Helpers;
using Tangra.Model.Astro;
using Tangra.Model.Config;
using Tangra.Model.Context;
using Tangra.Model.Helpers;
using Tangra.Model.Video;
using Tangra.VideoOperations.Spectroscopy;
using Tangra.VideoOperations.Spectroscopy.Helpers;

namespace Tangra.Controller
{
    public class SpectroscopyController
    {
        private object m_SyncLock = new object();

        private VideoController m_VideoController;
        private frmViewSpectra m_ViewSpectraForm;
        private Form m_MainFormView;
        private SpectraCalibrator m_SpectraCalibrator;
        private MasterSpectra m_CurrentSpectra;
        private TangraConfig.PersistedConfiguration m_Configuration;

        private TangraConfig.SpectraViewDisplaySettings m_DisplaySettings;

        public TangraConfig.SpectraViewDisplaySettings DisplaySettings
        {
            get { return m_DisplaySettings; }
        }

        internal SpectraReductionContext SpectraReductionContext { get; private set; }

        internal SpectroscopyController(Form mainFormView, VideoController videoController)
        {
            m_MainFormView = mainFormView;
            m_VideoController = videoController;
            m_ViewSpectraForm = null;
            SpectraReductionContext = new SpectraReductionContext();

            m_DisplaySettings = new TangraConfig.SpectraViewDisplaySettings();
            m_DisplaySettings.Load();
            m_DisplaySettings.Initialize();
        }

        internal bool HasCalibratedConfiguration()
        {
            return m_Configuration != null && m_Configuration.IsCalibrated;
        }

        internal bool ScaleSpectraByZeroOrderImagePosition()
        {
            if (m_CurrentSpectra != null && m_Configuration != null && m_Configuration.IsCalibrated)
            {
                m_SpectraCalibrator = new SpectraCalibrator(m_CurrentSpectra);
                return m_SpectraCalibrator.LoadCalibration(m_Configuration);
            }

            return false;
        }

        internal float LocateSpectraAngle(int roughStartingAngle)
        {

            AstroImage image = m_VideoController.GetCurrentAstroImage(false);

            MorphologicalFilters blackAndWhiteImage = new MorphologicalFilters(image);
            blackAndWhiteImage.WriteOriginalImageToFile(@"C:\Temp\original.png");

            blackAndWhiteImage.BorderExclusionSize = 15;

            blackAndWhiteImage.ConvertGreyScaleImageToBlackAndWhite();
            //Console.WriteLine("fraction of white pixels = {0}", blackAndWhiteImage.FractionOfWhitePixels);
            //blackAndWhiteImage.WriteBlackAndWhiteImageToFile(@"C:\Temp\InitialBW.png");
            blackAndWhiteImage.ApplyDilationFilter();
            //blackAndWhiteImage.WriteBlackAndWhiteImageToFile(@"C:\Temp\DilatedBW.png");
            blackAndWhiteImage.ApplyErosionFilter();
            //blackAndWhiteImage.WriteBlackAndWhiteImageToFile(@"C:\Temp\ErodedBW.png");

            blackAndWhiteImage.AutoDetectAndRemoveTimeStamp();

            // Now apply the morphological thinning filter
            blackAndWhiteImage.ApplyThinningFilter();
            //blackAndWhiteImage.WriteBlackAndWhiteImageToFile(@"C:\Temp\ThinnedBW.png");

            // Now identify the line segment(s) using the Hough transform

            int roughHoughAngle = blackAndWhiteImage.CalculateRoughHoughAngle((int)roughStartingAngle);
            //Console.WriteLine("Debugging - checking spectra to hough angle calculation: rough starting angle = {0}, hough angle = {1}", roughStartingAngle, roughHoughAngle);
            //Stopwatch s = Stopwatch.StartNew();
            blackAndWhiteImage.CalculateHoughTransformOfImage(roughHoughAngle);
            //s.Stop();
            //Console.WriteLine("Hough Transform took {0} ms", s.ElapsedMilliseconds);
            //blackAndWhiteImage.WriteHoughImageToFile(@"C:\Temp\Hough.png");
            blackAndWhiteImage.MaximumAllowedNumberOfLines = 1;
            double houghAngle = blackAndWhiteImage.GetHoughAngle(roughHoughAngle);
            float bestAngle0 = blackAndWhiteImage.CalculateBestAngle(houghAngle);
            //Console.WriteLine("Debugging - checking hough to spectra angles: hough angle = {0}, spectra angle = {1}", houghAngle, bestAngle0);
            //blackAndWhiteImage.WriteHoughImageToFile(@"C:\Temp\HoughWithPeaks.png");
            return bestAngle0;
        }

        internal MasterSpectra ComputeResult(
            List<Spectra> allFramesSpectra,
            PixelCombineMethod frameCombineMethod,
            bool useFineAdjustments,
            int? alignmentAbsorptionLinePos,
            int startingFrameIndex = 1,
            int? frameCountToProcess = null)
        {
            var masterSpectra = new MasterSpectra();

            if (allFramesSpectra.Count > 0)
            {
                masterSpectra.ZeroOrderPixelNo = allFramesSpectra[0].ZeroOrderPixelNo;
                masterSpectra.ZeroOrderFWHM = allFramesSpectra[0].ZeroOrderFWHM;
                masterSpectra.SignalAreaWidth = allFramesSpectra[0].SignalAreaWidth;
                masterSpectra.BackgroundAreaHalfWidth = allFramesSpectra[0].BackgroundAreaHalfWidth;
                masterSpectra.BackgroundAreaGap = allFramesSpectra[0].BackgroundAreaGap;
                masterSpectra.MaxPixelValue = allFramesSpectra[0].MaxPixelValue;
                masterSpectra.MaxSpectraValue = allFramesSpectra[0].MaxSpectraValue;
                for (int i = 0; i < allFramesSpectra[0].Points.Count; i++)
                    masterSpectra.Points.Add(new SpectraPoint(allFramesSpectra[0].Points[i]));

                int pixelArrWidth = allFramesSpectra[0].Pixels.GetLength(0);
                int pixelArrHeight = allFramesSpectra[0].Pixels.GetLength(1);

                masterSpectra.InitialisePixelArray(pixelArrWidth);
                masterSpectra.CombinedMeasurements = 1;

                var originalMasterPoints = new List<SpectraPoint>();
                originalMasterPoints.AddRange(masterSpectra.Points);

                if (frameCountToProcess == null && startingFrameIndex == 1)
                    frameCountToProcess = allFramesSpectra.Count;

                if (frameCombineMethod == PixelCombineMethod.Average)
                {
                    float fwhmSum = 0;
                    int fwhmCount = 0;

                    for (int i = startingFrameIndex; i < startingFrameIndex + frameCountToProcess - 1; i++)
                    {
                        if (i < 0 || i > allFramesSpectra.Count - 1) continue;

                        Spectra nextSpectra = allFramesSpectra[i];
                        masterSpectra.RawMeasurements.Add(nextSpectra);
                        if (!float.IsNaN(nextSpectra.ZeroOrderFWHM))
                        {
                            fwhmSum += nextSpectra.ZeroOrderFWHM;
                            fwhmCount++;
                        }

                        int nextSpectraFirstPixelNo = nextSpectra.Points[0].PixelNo;
                        int deltaIndex = nextSpectra.ZeroOrderPixelNo - masterSpectra.ZeroOrderPixelNo + masterSpectra.Points[0].PixelNo - nextSpectraFirstPixelNo;

                        int lineAlignOffset = 0;
                        if (alignmentAbsorptionLinePos.HasValue)
                        {
                            int roughLinePos = deltaIndex + alignmentAbsorptionLinePos.Value;
                            List<SpectraPoint> pointsInRegion = nextSpectra.Points.Where(x => Math.Abs(x.PixelNo - roughLinePos) < 10).ToList();
                            float[] arrPixelNo = pointsInRegion.Select(x => (float)x.PixelNo).ToArray();
                            float[] arrPixelValues = pointsInRegion.Select(x => x.RawValue).ToArray();
                            Array.Sort(arrPixelValues, arrPixelNo);
                            lineAlignOffset = (int)arrPixelNo[0] - roughLinePos;
                        }

                        int bestOffset = 0;

                        if (useFineAdjustments)
                        {
                            float bestOffsetValue = float.MaxValue;

                            for (int probeOffset = -2; probeOffset <= 2; probeOffset++)
                            {
                                float currOffsetValue = 0;
                                for (int j = 0; j < masterSpectra.Points.Count; j++)
                                {
                                    int indexNextSpectra = deltaIndex + j + probeOffset + lineAlignOffset;
                                    if (indexNextSpectra >= 0 && indexNextSpectra < nextSpectra.Points.Count)
                                    {
                                        currOffsetValue += Math.Abs(originalMasterPoints[j].RawValue - nextSpectra.Points[indexNextSpectra].RawValue);
                                    }
                                }

                                if (currOffsetValue < bestOffsetValue)
                                {
                                    bestOffsetValue = currOffsetValue;
                                    bestOffset = probeOffset;
                                }
                            }
                        }

                        for (int j = 0; j < masterSpectra.Points.Count; j++)
                        {
                            int indexNextSpectra = deltaIndex + j + bestOffset + lineAlignOffset;
                            if (indexNextSpectra >= 0 && indexNextSpectra < nextSpectra.Points.Count)
                            {
                                masterSpectra.Points[j].RawValue += nextSpectra.Points[indexNextSpectra].RawValue;
                                masterSpectra.Points[j].RawSignal += nextSpectra.Points[indexNextSpectra].RawSignal;
                                masterSpectra.Points[j].RawSignalPixelCount += nextSpectra.Points[indexNextSpectra].RawSignalPixelCount;

                                for (int h = 0; h < pixelArrHeight; h++)
                                {
                                    masterSpectra.Pixels[j, h] += nextSpectra.Pixels[indexNextSpectra, h];
                                }
                            }
                        }
                    }

                    masterSpectra.ZeroOrderFWHM = fwhmSum / fwhmCount;

                    // Normalize per row width
                    for (int i = 0; i < masterSpectra.Points.Count; i++)
                    {
                        if (Math.Abs(masterSpectra.Points[i].RawSignalPixelCount) < 0.0001)
                        {
                            masterSpectra.Points[i].RawValue = 0;
                            for (int h = 0; h < pixelArrHeight; h++) masterSpectra.Pixels[i, h] = 0;
                        }
                        else
                        {
                            masterSpectra.Points[i].RawValue = masterSpectra.Points[i].RawValue * masterSpectra.SignalAreaWidth / masterSpectra.Points[i].RawSignalPixelCount;
                            for (int h = 0; h < pixelArrHeight; h++) masterSpectra.Pixels[i, h] /= masterSpectra.Points[i].RawSignalPixelCount;
                        }

                    }
                }
                else if (frameCombineMethod == PixelCombineMethod.Median)
                {
                    var valueLists = new List<float>[masterSpectra.Points.Count];
                    for (int j = 0; j < masterSpectra.Points.Count; j++) valueLists[j] = new List<float>();

                    var signalLists = new List<float>[masterSpectra.Points.Count];
                    for (int j = 0; j < masterSpectra.Points.Count; j++) signalLists[j] = new List<float>();

                    var fwhmList = new List<float>();
                    for (int j = 0; j < masterSpectra.Points.Count; j++) fwhmList.Add(float.NaN);

                    for (int i = startingFrameIndex; i < startingFrameIndex + frameCountToProcess - 1; i++)
                    {
                        if (i < 0 || i > allFramesSpectra.Count - 1) continue;

                        Spectra nextSpectra = allFramesSpectra[i];
                        masterSpectra.RawMeasurements.Add(nextSpectra);

                        int nextSpectraFirstPixelNo = nextSpectra.Points[0].PixelNo;
                        int deltaIndex = nextSpectra.ZeroOrderPixelNo - masterSpectra.ZeroOrderPixelNo + masterSpectra.Points[0].PixelNo - nextSpectraFirstPixelNo;

                        int lineAlignOffset = 0;
                        if (alignmentAbsorptionLinePos.HasValue)
                        {
                            int roughLinePos = deltaIndex + alignmentAbsorptionLinePos.Value;
                            List<SpectraPoint> pointsInRegion = nextSpectra.Points.Where(x => Math.Abs(x.PixelNo - roughLinePos) < 10).ToList();
                            float[] arrPixelNo = pointsInRegion.Select(x => (float)x.PixelNo).ToArray();
                            float[] arrPixelValues = pointsInRegion.Select(x => x.RawValue).ToArray();
                            Array.Sort(arrPixelValues, arrPixelNo);
                            lineAlignOffset = (int)arrPixelNo[0] - roughLinePos;
                        }

                        int bestOffset = 0;

                        if (useFineAdjustments)
                        {
                            float bestOffsetValue = float.MaxValue;

                            for (int probeOffset = -2; probeOffset <= 2; probeOffset++)
                            {
                                float currOffsetValue = 0;
                                for (int j = 0; j < masterSpectra.Points.Count; j++)
                                {
                                    int indexNextSpectra = deltaIndex + j + probeOffset + lineAlignOffset;
                                    if (indexNextSpectra >= 0 && indexNextSpectra < nextSpectra.Points.Count)
                                    {
                                        currOffsetValue += Math.Abs(originalMasterPoints[j].RawValue - nextSpectra.Points[indexNextSpectra].RawValue);
                                    }
                                }

                                if (currOffsetValue < bestOffsetValue)
                                {
                                    bestOffsetValue = currOffsetValue;
                                    bestOffset = probeOffset;
                                }
                            }
                        }

                        for (int j = 0; j < masterSpectra.Points.Count; j++)
                        {
                            fwhmList[j] = nextSpectra.ZeroOrderFWHM;

                            int indexNextSpectra = deltaIndex + j + bestOffset + lineAlignOffset;
                            if (indexNextSpectra >= 0 && indexNextSpectra < nextSpectra.Points.Count)
                            {
                                valueLists[j].Add(nextSpectra.Points[indexNextSpectra].RawValue);
                                signalLists[j].Add(nextSpectra.Points[indexNextSpectra].RawValue);

                                for (int h = 0; h < pixelArrHeight; h++)
                                {
                                    masterSpectra.Pixels[j, h] += nextSpectra.Pixels[indexNextSpectra, h];
                                }
                            }
                        }
                    }

                    fwhmList.Sort();
                    masterSpectra.ZeroOrderFWHM = fwhmList.Count == 0 ? float.NaN : fwhmList[fwhmList.Count / 2];

                    for (int i = 0; i < masterSpectra.Points.Count; i++)
                    {
                        valueLists[i].Sort();
                        signalLists[i].Sort();

                        masterSpectra.Points[i].RawValue = valueLists[i].Count == 0 ? 0 : valueLists[i][valueLists[i].Count / 2];
                        masterSpectra.Points[i].RawSignal = signalLists[i].Count == 0 ? 0 : signalLists[i][signalLists[i].Count / 2];
                        masterSpectra.Points[i].RawSignalPixelCount = signalLists[i].Count;

                        if (Math.Abs(masterSpectra.Points[i].RawSignalPixelCount) < 0.0001)
                            for (int h = 0; h < pixelArrHeight; h++) masterSpectra.Pixels[i, h] = 0;
                        else
                            for (int h = 0; h < pixelArrHeight; h++) masterSpectra.Pixels[i, h] /= masterSpectra.Points[i].RawSignalPixelCount;
                    }
                }
            }

            return masterSpectra;
        }

        internal void DisplaySpectra(MasterSpectra masterSpectra, TangraConfig.PersistedConfiguration configuration, TangraConfig.SpectraViewDisplaySettings displaySettings, string fileName = null)
        {
            EnsureViewSpectraForm(displaySettings);

            m_CurrentSpectra = masterSpectra;
            m_Configuration = configuration;
            m_SpectraCalibrator = new SpectraCalibrator(masterSpectra);

            m_ViewSpectraForm.SetMasterSpectra(masterSpectra, fileName);
            m_ViewSpectraForm.StartPosition = FormStartPosition.CenterParent;
            m_ViewSpectraForm.Show(m_MainFormView);
        }

        public void EnsureViewSpectraFormClosed()
        {
            if (m_ViewSpectraForm != null)
            {
                try
                {
                    if (m_ViewSpectraForm.Visible) m_ViewSpectraForm.Close();
                    m_ViewSpectraForm.Dispose();
                }
                catch
                { }

                try
                {
                    m_ViewSpectraForm.Dispose();
                }
                catch
                { }

                m_ViewSpectraForm = null;
            }
        }

        public void EnsureViewSpectraForm(TangraConfig.SpectraViewDisplaySettings displaySettings)
        {
            EnsureViewSpectraFormClosed();

            m_ViewSpectraForm = new frmViewSpectra(this, m_VideoController, displaySettings);
        }

        public void ConfigureSaveSpectraFileDialog(SaveFileDialog saveFileDialog)
        {
            try
            {
                saveFileDialog.InitialDirectory = Path.GetDirectoryName(m_VideoController.CurrentVideoFileName);
                saveFileDialog.Filter = "Tangra Spectra Reduction (*.spectra)|*.spectra";
                saveFileDialog.FileName = Path.ChangeExtension(Path.GetFileName(m_VideoController.CurrentVideoFileName), ".spectra");
            }
            catch { /* In some rare cases m_VideoController.CurrentVideoFileName may throw an error. We want to ignore it. */ }
        }

        public void ConfigureExportSpectraFileDialog(SaveFileDialog saveFileDialog, string videoFileName)
        {
            try
            {
                saveFileDialog.InitialDirectory = Path.GetDirectoryName(videoFileName);
                saveFileDialog.Filter = "Tab-Separated Series (*.dat)|*.dat";
                saveFileDialog.FileName = Path.ChangeExtension(Path.GetFileName(m_VideoController.CurrentVideoFileName), ".dat");
            }
            catch { /* In some rare cases m_VideoController.CurrentVideoFileName may throw an error. We want to ignore it. */ }
        }

        public SpectraFileHeader GetSpectraFileHeader()
        {
            var rv = new SpectraFileHeader()
            {
                PathToVideoFile = m_VideoController.CurrentVideoFileName,
                Width = TangraContext.Current.FrameWidth,
                Height = TangraContext.Current.FrameHeight,
                BitPix = m_VideoController.VideoBitPix,
                DataAav16NormVal = m_VideoController.VideoAav16NormVal,
                SourceInfo = string.Format("Video ({0})", m_VideoController.CurrentVideoFileType),
                ObjectName = "",
                Telescope = "",
                Instrument = "",
                Recorder = "",
                Observer = "",
                RA = float.NaN,
                DEC = float.NaN,
                Longitude = float.NaN,
                Latitude = float.NaN
            };

            VideoFileFormat fileFormat = m_VideoController.GetVideoFileFormat();
            if (fileFormat.IsAAV())
            {
                Dictionary<string, string> tags = m_VideoController.GetVideoFileTags();
                tags.TryGetValue("ObjectName", out rv.ObjectName);
                tags.TryGetValue("Telescope", out rv.Telescope);
                tags.TryGetValue("Instrument", out rv.Instrument);
                tags.TryGetValue("Recorder", out rv.Recorder);
                tags.TryGetValue("Observer", out rv.Observer);

                string ra, dec, lng, lat;
                tags.TryGetValue("RA", out ra);
                tags.TryGetValue("DEC", out dec);
                tags.TryGetValue("Longitude", out lng);
                tags.TryGetValue("Latitude", out lat);

                if (!string.IsNullOrEmpty(ra)) float.TryParse(ra, NumberStyles.Float, CultureInfo.InvariantCulture, out rv.RA);
                if (!string.IsNullOrEmpty(dec)) float.TryParse(ra, NumberStyles.Float, CultureInfo.InvariantCulture, out rv.DEC);
                if (!string.IsNullOrEmpty(lng)) float.TryParse(ra, NumberStyles.Float, CultureInfo.InvariantCulture, out rv.Longitude);
                if (!string.IsNullOrEmpty(lat)) float.TryParse(ra, NumberStyles.Float, CultureInfo.InvariantCulture, out rv.Latitude);
            }

            return rv;
        }

        public void LoadSpectraFile()
        {
            var openFileDialog = new OpenFileDialog()
            {
                Filter = "Tangra Spectra Reduction (*.spectra)|*.spectra",
                CheckFileExists = true
            };

            if (openFileDialog.ShowDialog(m_MainFormView) == DialogResult.OK)
            {
                OpenSpectraFile(openFileDialog.FileName);
            }
        }

        internal void OpenSpectraFile(string fileName)
        {
            if (File.Exists(fileName))
            {
                SpectraFile spectraFile = SpectraFile.Load(fileName);
                if (spectraFile != null)
                {
                    DisplaySpectra(spectraFile.Data, null, m_DisplaySettings, fileName);

                    string videoFile = m_VideoController.GetVideoFileMatchingLcFile(spectraFile.Header.PathToVideoFile, fileName);
                    if (!string.IsNullOrEmpty(videoFile) &&
                        File.Exists(videoFile))
                    {
                        if (m_VideoController.OpenVideoFile(videoFile))
                        {
                            TangraContext.Current.CanPlayVideo = false;
                        }
                    }
                    else
                    {
                        // NOTE: No video file found, just show the saved averaged frame
                        TangraContext.Current.Reset();

                        if (spectraFile.Data.MeasurementInfo.FrameBitmapPixels != null &&
                            spectraFile.Data.MeasurementInfo.FrameBitmapPixels.Length > 0)
                        {
                            if (m_VideoController.SingleBitmapFile(spectraFile.Data.MeasurementInfo.FrameBitmapPixels, spectraFile.Header.Width, spectraFile.Header.Height))
                            {
                                TangraContext.Current.CanPlayVideo = false;
                                m_VideoController.UpdateViews();

                                PSFFit.SetDataRange(spectraFile.Header.BitPix, spectraFile.Header.DataAav16NormVal);
                            }
                        }

                        TangraContext.Current.CanPlayVideo = false;
                        TangraContext.Current.CanScrollFrames = false;
                    }

                    TangraContext.Current.FileName = Path.GetFileName(fileName);
                    TangraContext.Current.FileFormat = spectraFile.Header.SourceInfo;
                    m_VideoController.UpdateViews();
                }

                RegisterRecentSpectraFile(fileName);
            }
        }

        internal void RegisterRecentSpectraFile(string fileName)
        {
            m_VideoController.RegisterRecentFile(RecentFileType.Spectra, fileName);
        }

        internal void SetMarker(float pixelPos, float wavelength, bool attemptCalibration, int polynomialOrder)
        {
            m_SpectraCalibrator.SetMarker(pixelPos, wavelength, attemptCalibration, polynomialOrder);
        }

        internal void SetFirstOrderDispersion(float dispersion)
        {
            m_SpectraCalibrator.SetFirstOrderDispersion(dispersion);
        }


        internal bool IsCalibrated()
        {
            return m_SpectraCalibrator.IsCalibrated();
        }

        internal SpectraCalibrator GetSpectraCalibrator()
        {
            return m_SpectraCalibrator;
        }

        internal void SelectPixel(int pixelNo)
        {
            float waveLength = m_SpectraCalibrator.ResolveWavelength(pixelNo);
            m_ViewSpectraForm.DisplaySelectedDataPoint(pixelNo, waveLength);
        }

        internal void DeselectPixel()
        {
            m_ViewSpectraForm.ClearSelectedDataPoint();
        }

        internal SpectraCalibration GetSpectraCalibration()
        {
            if (IsCalibrated())
                return m_SpectraCalibrator.ToSpectraCalibration();

            return null;
        }

        internal MeasurementInfo GetMeasurementInfo()
        {
            return new MeasurementInfo()
            {
                FramesToMeasure = SpectraReductionContext.FramesToMeasure,
                MeasurementAreaWing = SpectraReductionContext.MeasurementAreaWing,
                BackgroundAreaWing = SpectraReductionContext.BackgroundAreaWing,
                BackgroundAreaGap = SpectraReductionContext.BackgroundAreaGap,
                BackgroundMethod = SpectraReductionContext.BackgroundMethod,
                FrameCombineMethod = SpectraReductionContext.FrameCombineMethod,
                UseFineAdjustments = SpectraReductionContext.UseFineAdjustments,
                AlignmentAbsorptionLinePos = SpectraReductionContext.AlignmentAbsorptionLinePos
            };
        }

        public void OnSpectraViewerClosed()
        {
            m_VideoController.CloseOpenedVideoFile();
        }

        internal void SaveCalibratedConfiguration()
        {
            if (m_Configuration != null && !m_Configuration.IsCalibrated)
            {
                SpectraCalibration calibration = GetSpectraCalibration();
                TangraConfig.PersistedConfiguration persistedConfiguration = TangraConfig.Settings.Spectroscopy.PersistedConfigurations.SingleOrDefault(x => x.Name == m_Configuration.Name);
                if (persistedConfiguration != null)
                {
                    persistedConfiguration.A = calibration.A;
                    persistedConfiguration.B = calibration.B;
                    persistedConfiguration.C = calibration.C;
                    persistedConfiguration.D = calibration.D;
                    persistedConfiguration.RMS = calibration.RMS;
                    persistedConfiguration.Order = calibration.PolynomialOrder;
                    persistedConfiguration.Dispersion = calibration.Dispersion;
                    persistedConfiguration.IsCalibrated = true;

                    TangraConfig.Settings.Save();
                }
            }
        }

        internal string ExportToDat(MasterSpectra spectra)
        {
            SpectraCalibrator calibrator = GetSpectraCalibrator();
            var bld = new StringBuilder();

            AddDatExportHeader(bld, spectra);

            foreach (SpectraPoint point in spectra.Points)
            {
                float wavelength = calibrator.ResolveWavelength(point.PixelNo);
                if (wavelength < 0) continue;
                bld.AppendFormat("{0}\t{1}\r\n", wavelength, point.RawValue);
            }

            return bld.ToString();
        }

        internal void PopulateMasterSpectraObservationDetails(MasterSpectra spectra)
        {
            var dict = m_VideoController.GetVideoFileTags();

            float lng = float.NaN;
            float lat = float.NaN;
            float ra = float.NaN;
            float dec = float.NaN;
            DateTime? centralTime = null;

            if (spectra.ObservationInfo == null) spectra.ObservationInfo = new ObservationInfo();
            spectra.ObservationInfo.Reset();

            if (dict.ContainsKey("Longitude"))
            {
                if (float.TryParse(dict["Longitude"], NumberStyles.Float, CultureInfo.InvariantCulture, out lng))
                    spectra.ObservationInfo.AddProperty("Longitude", lng.ToString("0.0000", CultureInfo.InvariantCulture));
            }
            if (dict.ContainsKey("Latitude"))
            {
                if (float.TryParse(dict["Latitude"], NumberStyles.Float, CultureInfo.InvariantCulture, out lat))
                    spectra.ObservationInfo.AddProperty("Latitude", lat.ToString("0.0000", CultureInfo.InvariantCulture));
            }
            if (dict.ContainsKey("RA"))
            {
                if (float.TryParse(dict["RA"], NumberStyles.Float, CultureInfo.InvariantCulture, out ra))
                    spectra.ObservationInfo.AddProperty("RA", ra.ToString("0.0000", CultureInfo.InvariantCulture), "hours");
            }
            if (dict.ContainsKey("DEC"))
            {
                if (float.TryParse(dict["DEC"], NumberStyles.Float, CultureInfo.InvariantCulture, out dec))
                    spectra.ObservationInfo.AddProperty("DEC", dec.ToString("0.0000", CultureInfo.InvariantCulture), "degrees");
            }
            if (spectra.MeasurementInfo.FirstFrameTimeStamp.HasValue && spectra.MeasurementInfo.LastFrameTimeStamp.HasValue)
            {
                centralTime = new DateTime((spectra.MeasurementInfo.FirstFrameTimeStamp.Value.Ticks + spectra.MeasurementInfo.LastFrameTimeStamp.Value.Ticks) / 2);
                double jd = JulianDayHelper.JDUtcAtDate(centralTime.Value);
                spectra.ObservationInfo.AddProperty("JD", jd.ToString("0.00000", CultureInfo.InvariantCulture), "UT");
            }

            if (!float.IsNaN(lng) && !float.IsNaN(lat) && !float.IsNaN(ra) && !float.IsNaN(dec) && centralTime.HasValue)
            {
                var extCalc = new AtmosphericExtinctionCalculator(ra, dec, lng, lat, 0);
                double airMass;
                double altitudeDeg;
                extCalc.CalculateExtinction(centralTime.Value, out altitudeDeg, out airMass);

                spectra.ObservationInfo.AddProperty("X", airMass.ToString("0.000", CultureInfo.InvariantCulture), "air mass");
            }

            if (!float.IsNaN(spectra.ZeroOrderFWHM)) spectra.ObservationInfo.AddProperty("FWHM", spectra.ZeroOrderFWHM.ToString("0.00"), "zero order image FWHM");

            if (!float.IsNaN(spectra.MeasurementInfo.Gain)) spectra.ObservationInfo.AddProperty("Gain", spectra.MeasurementInfo.Gain.ToString("0.0", CultureInfo.InvariantCulture), "dB");
            if (!float.IsNaN(spectra.MeasurementInfo.ExposureSeconds)) spectra.ObservationInfo.AddProperty("Exposure", spectra.MeasurementInfo.ExposureSeconds.ToString("0.000", CultureInfo.InvariantCulture), "sec");

            if (dict.ContainsKey("ObjectName")) spectra.ObservationInfo.AddProperty("Target", dict["ObjectName"]);
            if (dict.ContainsKey("Instrument")) spectra.ObservationInfo.AddProperty("Camera", dict["Instrument"]);
            if (dict.ContainsKey("Telescope")) spectra.ObservationInfo.AddProperty("Telescope", dict["Telescope"]);
            if (dict.ContainsKey("Recorder")) spectra.ObservationInfo.AddProperty("Recorder", dict["Recorder"]);
            if (dict.ContainsKey("Observer")) spectra.ObservationInfo.AddProperty("Observer", dict["Observer"]);

            if (spectra.IsCalibrated())
            {
                if (spectra.Calibration.PolynomialOrder == 1)
                    spectra.ObservationInfo.AddProperty("WavelengthCalibration", string.Format("1-st order[{0},{1}]", spectra.Calibration.A, spectra.Calibration.B));
                else if (spectra.Calibration.PolynomialOrder == 2)
                    spectra.ObservationInfo.AddProperty("WavelengthCalibration", string.Format("2-nd order[{0},{1},{2}]", spectra.Calibration.A, spectra.Calibration.B, spectra.Calibration.C));
                else if (spectra.Calibration.PolynomialOrder == 3)
                    spectra.ObservationInfo.AddProperty("WavelengthCalibration", string.Format("3-rd order[{0},{1},{2},{3}]", spectra.Calibration.A, spectra.Calibration.B, spectra.Calibration.C, spectra.Calibration.D));
                else if (spectra.Calibration.PolynomialOrder == 4)
                    spectra.ObservationInfo.AddProperty("WavelengthCalibration", string.Format("4-th order[{0},{1},{2},{3},{4}]", spectra.Calibration.A, spectra.Calibration.B, spectra.Calibration.C, spectra.Calibration.D, spectra.Calibration.E));

                spectra.ObservationInfo.AddProperty("Dispersion", spectra.Calibration.Dispersion.ToString("0.00", CultureInfo.InvariantCulture), "A/pix");
            }
        }

        internal void AddDatExportHeader(StringBuilder output, MasterSpectra spectra)
        {
            if (spectra.ObservationInfo.IsEmpty)
                PopulateMasterSpectraObservationDetails(spectra);

            string[] propNames = spectra.ObservationInfo.GetPropertyNames();

            foreach (string name in propNames)
            {
                string value = spectra.ObservationInfo.GetProperty(name);
                string comment = spectra.ObservationInfo.GetPropertyComment(name);

                if (!string.IsNullOrEmpty(comment))
                    output.AppendFormat("# {0}={1} #{2}\r\n", name, value, comment);
                else
                    output.AppendFormat("# {0}={1}\r\n", name, value);
            }
            output.AppendLine();
        }
    }
}
