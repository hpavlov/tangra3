using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Helpers;
using Tangra.Model.Astro;
using Tangra.Model.Config;
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

		internal SpectroscopyController(Form mainFormView, VideoController videoController)
		{
			m_MainFormView = mainFormView;
			m_VideoController = videoController;
			m_ViewSpectraForm = null;
		}

        internal float LocateSpectraAngle(PSFFit selectedStar)
        {
            float x0 = (float)selectedStar.XCenter;
            float y0 = (float)selectedStar.YCenter;
            uint brigthness20 = (uint)(selectedStar.Brightness / 5);
            uint bgFromPsf = (uint)(selectedStar.I0); 

            int minDistance = (int)(10 * selectedStar.FWHM);
            int clearDist =  (int)(2 * selectedStar.FWHM);

            AstroImage image = m_VideoController.GetCurrentAstroImage(false);

            float width = image.Width;
            float height = image.Height;

            uint[] angles = new uint[360];
            uint[] sums = new uint[360];
            uint[] pixAbove50Perc = new uint[360];

            int diagonnalPixels = (int)Math.Ceiling(Math.Sqrt(image.Width * image.Width + image.Height * image.Height));
            for (int i = 0; i < 360; i++)
            {
                var mapper = new RotationMapper(image.Width, image.Height, i);
                PointF p1 = mapper.GetDestCoords(x0, y0);
                float x1 = p1.X;
                float y1 = p1.Y;

                uint rowSum = 0;
                uint pixAbove50 = 0;
                uint pixAbove50Max = 0;
                bool prevPixAbove50 = false;

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
                            if ((value - bgFromPsf) > brigthness20 && value > value_u && value > value_d)
                            {
                                if (prevPixAbove50) pixAbove50++;
                                prevPixAbove50 = true;
                            }
                            else
                            {
                                prevPixAbove50 = false;
                                if (pixAbove50Max < pixAbove50) pixAbove50Max = pixAbove50;
                                pixAbove50 = 0;
                            }                            
                        }
                        else
                        {
                            prevPixAbove50 = false;
                            if (pixAbove50Max < pixAbove50) pixAbove50Max = pixAbove50;
                            pixAbove50 = 0;
                        }
                    }
                }

                angles[i] = (uint)i;
                sums[i] = rowSum;
                pixAbove50Perc[i] = pixAbove50Max;
            }

            Array.Sort(pixAbove50Perc, angles);

            uint roughAngle = angles[359];

            if (pixAbove50Perc[358] * 2 > pixAbove50Perc[359] && // Second best should have a lot smaller score than the top one
                Math.Abs((int)angles[358] - (int)angles[359]) != 1) // or for large stars the two best can be sequential angles
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

		internal MasterSpectra ComputeResult(List<Spectra> allFramesSpectra, PixelCombineMethod frameCombineMethod)
        {
			var masterSpectra = new MasterSpectra();

            if (allFramesSpectra.Count > 0)
            {
                masterSpectra.ZeroOrderPixelNo = allFramesSpectra[0].ZeroOrderPixelNo;
                masterSpectra.SignalAreaWidth = allFramesSpectra[0].SignalAreaWidth;
                masterSpectra.MaxPixelValue = allFramesSpectra[0].MaxPixelValue;
                masterSpectra.Points.AddRange(allFramesSpectra[0].Points);
                masterSpectra.CombinedMeasurements = 1;

	            if (frameCombineMethod == PixelCombineMethod.Average)
	            {
					for (int i = 1; i < allFramesSpectra.Count; i++)
					{
						Spectra nextSpectra = allFramesSpectra[i];
						for (int j = 0; j < masterSpectra.Points.Count; j++)
						{
							int indexNextSpectra = nextSpectra.ZeroOrderPixelNo - masterSpectra.ZeroOrderPixelNo + j;
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
						masterSpectra.Points[i].RawValue = masterSpectra.Points[i].RawValue * masterSpectra.SignalAreaWidth / masterSpectra.Points[i].RawSignalPixelCount;
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
						for (int j = 0; j < masterSpectra.Points.Count; j++)
						{
							int indexNextSpectra = nextSpectra.ZeroOrderPixelNo - masterSpectra.ZeroOrderPixelNo + j;
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

			m_ViewSpectraForm.SetMasterSpectra(masterSpectra);
			m_ViewSpectraForm.Show();
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
				saveFileDialog.FileName = Path.ChangeExtension(Path.GetFileName(m_VideoController.CurrentVideoFileName), ".spectra");
			}
			catch { /* In some rare cases m_VideoController.CurrentVideoFileName may throw an error. We want to ignore it. */ }
	    }

		public SpectraFileHeader GetSpectraFileHeader()
	    {
		    return new SpectraFileHeader()
		    {
			    PathToVideoFile = m_VideoController.CurrentVideoFileName
		    };
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

				m_VideoController.RegisterRecentFile(RecentFileType.Spectra, fileName);
		    }
	    }
    }
}
