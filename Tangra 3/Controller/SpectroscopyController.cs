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

        internal SpectraReductionContext SpectraReductionContext { get; private set; }

        internal SpectroscopyController(Form mainFormView, VideoController videoController)
		{
			m_MainFormView = mainFormView;
			m_VideoController = videoController;
			m_ViewSpectraForm = null;
            SpectraReductionContext = new SpectraReductionContext();
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

            if (//pixAbove50Perc[358] * 2 > pixAbove50Perc[359] && // Second best should have a lot smaller score than the top one
                Math.Abs((int)angles[358] - (int)angles[359]) != 1 && // or for large stars the two best can be sequential angles
                !roughStartingAngle.HasValue) // unless the user provided the first rough approximation
                return float.NaN;

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

		internal MasterSpectra ComputeResult(List<Spectra> allFramesSpectra, PixelCombineMethod frameCombineMethod, bool useFineAdjustments)
        {
			var masterSpectra = new MasterSpectra();

            if (allFramesSpectra.Count > 0)
            {
                masterSpectra.ZeroOrderPixelNo = allFramesSpectra[0].ZeroOrderPixelNo;
                masterSpectra.SignalAreaWidth = allFramesSpectra[0].SignalAreaWidth;
                masterSpectra.MaxPixelValue = allFramesSpectra[0].MaxPixelValue;
				masterSpectra.MaxSpectraValue = allFramesSpectra[0].MaxSpectraValue;
                masterSpectra.Points.AddRange(allFramesSpectra[0].Points);
                masterSpectra.CombinedMeasurements = 1;

                var originalMasterPoints = new List<SpectraPoint>();
                originalMasterPoints.AddRange(masterSpectra.Points);

	            if (frameCombineMethod == PixelCombineMethod.Average)
	            {
					for (int i = 1; i < allFramesSpectra.Count; i++)
					{
						Spectra nextSpectra = allFramesSpectra[i];
					    int bestOffset = 0;

                        if (useFineAdjustments)
                        {
                            float bestOffsetValue = float.MaxValue;

                            for (int probeOffset = -2; probeOffset <= 2; probeOffset++)
                            {
                                float currOffsetValue = 0;
                                for (int j = 0; j < masterSpectra.Points.Count; j++)
                                {
                                    int indexNextSpectra = nextSpectra.ZeroOrderPixelNo - masterSpectra.ZeroOrderPixelNo + j + probeOffset;
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
                            int indexNextSpectra = nextSpectra.ZeroOrderPixelNo - masterSpectra.ZeroOrderPixelNo + j + bestOffset;
                            if (indexNextSpectra >= 0 && indexNextSpectra < nextSpectra.Points.Count)
                            {
                                masterSpectra.Points[j].RawValue += nextSpectra.Points[indexNextSpectra].RawValue;
                                masterSpectra.Points[j].RawSignal += nextSpectra.Points[indexNextSpectra].RawSignal;
                                masterSpectra.Points[j].RawSignalPixelCount += nextSpectra.Points[indexNextSpectra].RawSignalPixelCount;
                            }
                        }
					}

					// Normalize per row width
					for (int i = 0; i < masterSpectra.Points.Count; i++)
					{
						masterSpectra.Points[i].RawValue = masterSpectra.Points[i].RawSignalPixelCount == 0 
							? 0 
							: masterSpectra.Points[i].RawValue * masterSpectra.SignalAreaWidth / masterSpectra.Points[i].RawSignalPixelCount;
					}
	            }
				else if (frameCombineMethod == PixelCombineMethod.Median)
				{
					var valueLists = new List<float>[masterSpectra.Points.Count];
					for (int j = 0; j < masterSpectra.Points.Count; j++) valueLists[j] = new List<float>();

					var signalLists = new List<float>[masterSpectra.Points.Count];
					for (int j = 0; j < masterSpectra.Points.Count; j++) signalLists[j] = new List<float>();

					for (int i = 1; i < allFramesSpectra.Count; i++)
					{
						Spectra nextSpectra = allFramesSpectra[i];

                        int bestOffset = 0;

                        if (useFineAdjustments)
                        {
                            float bestOffsetValue = float.MaxValue;

                            for (int probeOffset = -2; probeOffset <= 2; probeOffset++)
                            {
                                float currOffsetValue = 0;
                                for (int j = 0; j < masterSpectra.Points.Count; j++)
                                {
                                    int indexNextSpectra = nextSpectra.ZeroOrderPixelNo - masterSpectra.ZeroOrderPixelNo + j + probeOffset;
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
                            int indexNextSpectra = nextSpectra.ZeroOrderPixelNo - masterSpectra.ZeroOrderPixelNo + j + bestOffset;
							if (indexNextSpectra >= 0 && indexNextSpectra < nextSpectra.Points.Count)
							{
								valueLists[j].Add(nextSpectra.Points[indexNextSpectra].RawValue);
								signalLists[j].Add(nextSpectra.Points[indexNextSpectra].RawValue);
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
					}
				}
            }

			return masterSpectra;
        }

	    internal void DisplaySpectra(MasterSpectra masterSpectra)
	    {
		    EnsureViewSpectraForm();

	        m_CurrentSpectra = masterSpectra;
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

	    public void EnsureViewSpectraForm()
	    {
		    EnsureViewSpectraFormClosed();

			m_ViewSpectraForm = new frmViewSpectra(this);
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

		public SpectraFileHeader GetSpectraFileHeader()
	    {
		    var rv = new SpectraFileHeader()
		    {
			    PathToVideoFile = m_VideoController.CurrentVideoFileName,
                Width = TangraContext.Current.FrameWidth,
                Height = TangraContext.Current.FrameHeight,
                BitPix = m_VideoController.VideoBitPix,
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
					DisplaySpectra(spectraFile.Data);
				}

		        RegisterRecentSpectraFile(fileName);
		    }
	    }

        internal void RegisterRecentSpectraFile(string fileName)
        {
            m_VideoController.RegisterRecentFile(RecentFileType.Spectra, fileName);
        }

        internal void SetMarker(int pixelNo, float wavelength)
        {
            m_SpectraCalibrator.SetMarker(pixelNo, wavelength);
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
               BackgroundMethod = SpectraReductionContext.BackgroundMethod,
               FrameCombineMethod = SpectraReductionContext.FrameCombineMethod,
               UseFineAdjustments = SpectraReductionContext.UseFineAdjustments,
               UseLowPassFilter = SpectraReductionContext.UseLowPassFilter
            };
        }
    }
}
