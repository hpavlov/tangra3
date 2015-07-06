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
using Tangra.VideoOperations.LightCurves;
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

        internal float LocateSpectraAngle(PSFFit selectedStar, int? roughStartingAngle = null)
        {
            float x0 = (float)selectedStar.XCenter;
            float y0 = (float)selectedStar.YCenter;
            uint brigthness10 = (uint)(0.1 * selectedStar.Brightness);
            uint brigthness20 = (uint)(0.2 * selectedStar.Brightness);
            uint brigthness40 = (uint)(0.4 * selectedStar.Brightness);
            uint bgFromPsf = (uint)(selectedStar.I0); 

            int minDistance = (int)(10 * selectedStar.FWHM);
            int clearDist =  (int)(2 * selectedStar.FWHM);

            AstroImage image = m_VideoController.GetCurrentAstroImage(false);

            float width = image.Width;
            float height = image.Height;

            uint[] angles = new uint[360];
            uint[] sums = new uint[360];
            uint[] pixAbove10Perc = new uint[360];
            uint[] pixAbove20Perc = new uint[360];
            uint[] pixAbove40Perc = new uint[360];

            int diagonnalPixels = (int)Math.Ceiling(Math.Sqrt(image.Width * image.Width + image.Height * image.Height));

            int iFrom = 0;
            int iTo = 360;
            if (roughStartingAngle.HasValue)
            {
                iFrom = roughStartingAngle.Value - 10;
                iTo = roughStartingAngle.Value + 10;
            }

            bool peakFound = false;
            for (int i = iFrom; i < iTo; i++)
            {
                var mapper = new RotationMapper(image.Width, image.Height, i);
                PointF p1 = mapper.GetDestCoords(x0, y0);
                float x1 = p1.X;
                float y1 = p1.Y;

                uint rowSum = 0;
                uint pixAbove10 = 0;
                uint pixAbove10Max = 0;
                bool prevPixAbove10 = false;
                uint pixAbove20 = 0;
                uint pixAbove20Max = 0;
                bool prevPixAbove20 = false;
                uint pixAbove40 = 0;
                uint pixAbove40Max = 0;
                bool prevPixAbove40 = false;
                
                for (int d = minDistance; d < diagonnalPixels; d++)
                {
                    PointF p = mapper.GetSourceCoords(x1 + d, y1);

                    if (p.X >= 0 && p.X < width && p.Y >= 0 && p.Y < height)
                    {
                        uint value = image.Pixelmap[(int) p.X, (int) p.Y];
                        rowSum += value;
                        PointF pu = mapper.GetSourceCoords(x1 + d, y1 + clearDist);
                        PointF pd = mapper.GetSourceCoords(x1 + d, y1 - clearDist);
                        if (pu.X >= 0 && pu.X < width && pu.Y >= 0 && pu.Y < height &&
                            pd.X >= 0 && pd.X < width && pd.Y >= 0 && pd.Y < height)
                        {
                            uint value_u = image.Pixelmap[(int)pu.X, (int)pu.Y];
                            uint value_d = image.Pixelmap[(int)pd.X, (int)pd.Y];
                            if ((value - bgFromPsf) > brigthness10 && value > value_u && value > value_d)
                            {
                                if (prevPixAbove10) pixAbove10++;
                                prevPixAbove10 = true;
                            }
                            else
                            {
                                prevPixAbove10 = false;
                                if (pixAbove10Max < pixAbove10) pixAbove10Max = pixAbove10;
                                pixAbove10 = 0;
                                peakFound = true;
                            }

                            if ((value - bgFromPsf) > brigthness20 && value > value_u && value > value_d)
                            {
                                if (prevPixAbove20) pixAbove20++;
                                prevPixAbove20 = true;
                            }
                            else
                            {
                                prevPixAbove20 = false;
                                if (pixAbove20Max < pixAbove20) pixAbove20Max = pixAbove20;
                                pixAbove20 = 0;
                                peakFound = true;
                            }

                            if ((value - bgFromPsf) > brigthness40 && value > value_u && value > value_d)
                            {
                                if (prevPixAbove40) pixAbove40++;
                                prevPixAbove40 = true;
                            }
                            else
                            {
                                prevPixAbove40 = false;
                                if (pixAbove40Max < pixAbove40) pixAbove40Max = pixAbove40;
                                pixAbove40 = 0;
                                peakFound = true;
                            } 
                        }
                        else
                        {
                            prevPixAbove10 = false;
                            if (pixAbove10Max < pixAbove10) pixAbove10Max = pixAbove10;
                            pixAbove10 = 0;

                            prevPixAbove20 = false;
                            if (pixAbove20Max < pixAbove20) pixAbove20Max = pixAbove20;
                            pixAbove20 = 0;

                            prevPixAbove40 = false;
                            if (pixAbove40Max < pixAbove40) pixAbove40Max = pixAbove40;
                            pixAbove40 = 0;

                            peakFound = true;
                        }
                    }
                }

                angles[i] = (uint)i;
                sums[i] = rowSum;
                pixAbove10Perc[i] = pixAbove10Max;
                pixAbove20Perc[i] = pixAbove20Max;
                pixAbove40Perc[i] = pixAbove40Max;
            }

            if (!peakFound)
                return float.NaN;

            var angles10 = new List<uint>(angles).ToArray();
            var angles20 = new List<uint>(angles).ToArray();
            var angles40 = new List<uint>(angles).ToArray();

            Array.Sort(sums, angles);
            Array.Sort(pixAbove10Perc, angles10);
            Array.Sort(pixAbove20Perc, angles20);
            Array.Sort(pixAbove40Perc, angles40);

            uint roughAngle = angles[359];

            if (pixAbove10Perc[358] * 2 < pixAbove10Perc[359])
            {
                // If second best at 10% id a lot smaller score than the top 10% scopem then this is it
                roughAngle = angles10[359];
            }
            else
            {
                if (Math.Abs((int)angles[358] - (int)angles[359]) > 3)// or for large stars the two best can be sequential angles
                    return float.NaN;
            }

            uint bestSum = 0;
            float bestAngle = 0f;

            for (float a = roughAngle - 1; a < roughAngle + 1; a += 0.02f)
            {
                var mapper = new RotationMapper(image.Width, image.Height, a);
                PointF p1 = mapper.GetDestCoords(x0, y0);
                float x1 = p1.X;
                float y1 = p1.Y;

                uint rowSum = 0;

                for (int d = minDistance; d < diagonnalPixels; d++)
                {
                    PointF p = mapper.GetSourceCoords(x1 + d, y1);

                    if (p.X >= 0 && p.X < width && p.Y >= 0 && p.Y < height)
                    {
                        uint pixVal = image.Pixelmap[(int)p.X, (int)p.Y];
                        rowSum += pixVal;
                    }
                }

                if (rowSum > bestSum)
                {
                    bestSum = rowSum;
                    bestAngle = a;
                }
            }

            return bestAngle;
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

		internal void DisplaySpectra(MasterSpectra masterSpectra, TangraConfig.PersistedConfiguration configuration, TangraConfig.SpectraViewDisplaySettings displaySettings)
	    {
			EnsureViewSpectraForm(displaySettings);

	        m_CurrentSpectra = masterSpectra;
            m_Configuration = configuration;
            m_SpectraCalibrator = new SpectraCalibrator(masterSpectra);

			m_ViewSpectraForm.SetMasterSpectra(masterSpectra);
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
                Telescope =  "",
                Instrument = "",
                Recorder = "",
                Observer = "",
                RA = float.NaN,
                DEC = float.NaN,
                Longitude = float.NaN,
                Latitude = float.NaN
		    };

		    VideoFileFormat fileFormat = m_VideoController.GetVideoFileFormat();
            if (fileFormat == VideoFileFormat.AAV)
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
                    // TODO: Choose configuration ?? 
					DisplaySpectra(spectraFile.Data, null, m_DisplaySettings);

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
               UseLowPassFilter = SpectraReductionContext.UseLowPassFilter,
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


        internal void AddDatExportHeader(StringBuilder output, MasterSpectra spectra)
        {
            var dict = m_VideoController.GetVideoFileTags();
            
            float lng = float.NaN;
            float lat = float.NaN;
            float ra = float.NaN;
            float dec = float.NaN;
            DateTime? centralTime = null;

            if (dict.ContainsKey("Longitude"))
            {
                if (float.TryParse(dict["Longitude"], NumberStyles.Float, CultureInfo.InvariantCulture, out lng))
                    output.AppendFormat("# Longitude={0}\r\n", lng.ToString("0.0000", CultureInfo.InvariantCulture));
            }
            if (dict.ContainsKey("Latitude"))
            {
                if (float.TryParse(dict["Latitude"], NumberStyles.Float, CultureInfo.InvariantCulture, out lat))
                    output.AppendFormat("# Latitude={0}\r\n", lat.ToString("0.0000", CultureInfo.InvariantCulture));
            }
            if (dict.ContainsKey("RA"))
            {
                if (float.TryParse(dict["RA"], NumberStyles.Float, CultureInfo.InvariantCulture, out ra))
                    output.AppendFormat("# RA={0} # hours\r\n", ra.ToString("0.0000", CultureInfo.InvariantCulture));
            }
            if (dict.ContainsKey("DEC"))
            {
                if (float.TryParse(dict["DEC"], NumberStyles.Float, CultureInfo.InvariantCulture, out dec))
                    output.AppendFormat("# DEC={0} # degrees\r\n", dec.ToString("0.0000", CultureInfo.InvariantCulture));
            }
            if (spectra.MeasurementInfo.FirstFrameTimeStamp.HasValue && spectra.MeasurementInfo.LastFrameTimeStamp.HasValue)
            {
                centralTime = new DateTime((spectra.MeasurementInfo.FirstFrameTimeStamp.Value.Ticks + spectra.MeasurementInfo.LastFrameTimeStamp.Value.Ticks) / 2);
                double jd = JulianDayHelper.JDUtcAtDate(centralTime.Value);
                output.AppendFormat("# JD={0} # UT\r\n", jd.ToString("0.00000", CultureInfo.InvariantCulture));
            }

            if (!float.IsNaN(lng) && !float.IsNaN(lat) && !float.IsNaN(ra) && !float.IsNaN(dec) && centralTime.HasValue)
            {
                var extCalc = new AtmosphericExtinctionCalculator(ra, dec, lng, lat, 0);
                double airMass;
                double altitudeDeg;
                extCalc.CalculateExtinction(centralTime.Value, out altitudeDeg, out airMass);

                output.AppendFormat("# Z={0} # air mass\r\n", airMass.ToString("0.000", CultureInfo.InvariantCulture));
            }

            if (!float.IsNaN(spectra.MeasurementInfo.Gain)) output.AppendFormat("# Gain={0} # dB\r\n", spectra.MeasurementInfo.Gain.ToString("0.0", CultureInfo.InvariantCulture));
            if (!float.IsNaN(spectra.MeasurementInfo.ExposureSeconds)) output.AppendFormat("# Exposure={0} # sec\r\n", spectra.MeasurementInfo.ExposureSeconds.ToString("0.00", CultureInfo.InvariantCulture));

            if (dict.ContainsKey("ObjectName")) output.AppendFormat("# Target={0}\r\n", dict["ObjectName"]);
            if (dict.ContainsKey("Instrument")) output.AppendFormat("# Camera={0}\r\n", dict["Instrument"]);
            if (dict.ContainsKey("Telescope")) output.AppendFormat("# Telescope={0}\r\n", dict["Telescope"]);
            if (dict.ContainsKey("Recorder")) output.AppendFormat("# Recorder={0}\r\n", dict["Recorder"]);
            if (dict.ContainsKey("Observer")) output.AppendFormat("# Observer={0}\r\n", dict["Observer"]);

            if (spectra.IsCalibrated())
            {
                if (spectra.Calibration.PolynomialOrder == 1)
                    output.AppendFormat("# WavelengthCalibration=1-st order[{0},{1}]\r\n", spectra.Calibration.A, spectra.Calibration.B);
                else if (spectra.Calibration.PolynomialOrder == 2)
                    output.AppendFormat("# WavelengthCalibration=2-nd order[{0},{1},{2}]\r\n", spectra.Calibration.A, spectra.Calibration.B, spectra.Calibration.C);
                else if (spectra.Calibration.PolynomialOrder == 3)
                    output.AppendFormat("# WavelengthCalibration=3-rd order[{0},{1},{2},{3}]\r\n", spectra.Calibration.A, spectra.Calibration.B, spectra.Calibration.C, spectra.Calibration.D);

                output.AppendFormat("# Dispersion={0} # A/pix\r\n", spectra.Calibration.Dispersion.ToString("0.00", CultureInfo.InvariantCulture));
            }

            output.AppendLine();
        }
    }
}
