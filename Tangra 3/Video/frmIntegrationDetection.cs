using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Tangra.Model.Image;
using Tangra.Model.Numerical;

namespace Tangra.Video
{
	public partial class frmIntegrationDetection : Form
	{
		private IImagePixelProvider m_ImageProvider;

		private List<List<AverageCalculator>> m_AverageCalculatorsPerFrame = new List<List<AverageCalculator>>();

		private int m_StartFrameId;

        public frmIntegrationDetection(IImagePixelProvider imageProvider, int startFrameId)
		{
			InitializeComponent();

			m_ImageProvider = imageProvider;
			m_StartFrameId = startFrameId;

			picFrameSpectrum.Image = new Bitmap(picFrameSpectrum.Width, picFrameSpectrum.Height);
			picSigmas.Image = new Bitmap(picSigmas.Width, picSigmas.Height);

			pnlResult.Visible = false;
			pnlResult.SendToBack();
		}

		private void frmIntegrationDetection_Load(object sender, EventArgs e)
		{
			timer.Interval = 50;
			timer.Enabled = true;
		}

		private void timer_Tick(object sender, EventArgs e)
		{
			timer.Enabled = false;
			DoMeasurements();
		}

		private Dictionary<int, double> m_SigmaDict;

		private void DoMeasurements()
		{
			m_SigmaDict = new Dictionary<int, double>();

			Rectangle testRect = new Rectangle(
				(m_ImageProvider.Width / 2) - 16,
				(m_ImageProvider.Height / 2) - 16,
				32, 32);

			int framesToMeasure = 65;
			
			int firstFrame = m_StartFrameId;
			int lastFrame = Math.Min(m_ImageProvider.LastFrame, m_StartFrameId + framesToMeasure);

			progressBar1.Minimum = firstFrame;
			progressBar1.Maximum = lastFrame - 1;
			progressBar1.Style = ProgressBarStyle.Marquee;

			int[,] pixels1 = null;
			int[,] pixels2 = null;

			for (int i = firstFrame; i < lastFrame - 1; i++)
				m_SigmaDict.Add(i, double.NaN);

			PotentialIntegrationFit foundIntegration = null;

			for (int k = 0; k < 4; k++)
			{
				for (int i = firstFrame; i < lastFrame - 1; i++)
				{
					progressBar1.Value = i;
					progressBar1.Refresh();

					List<AverageCalculator> calcList = new List<AverageCalculator>();

					for (int x = 0; x < 32; x++)
						for (int y = 0; y < 32; y++)
							calcList.Add(new AverageCalculator());

					if (pixels1 == null)
					{
						pixels1 = m_ImageProvider.GetPixelArray(i, testRect);
					}
					else
					{
						for (int x = 0; x < 32; x++)
							for (int y = 0; y < 32; y++)
							{
								pixels1[x, y] = pixels2[x, y];
							}
					}

                    pixels2 = m_ImageProvider.GetPixelArray(i + 1, testRect);

				    double sigmaSum = 0;
                    for (int x = 0; x < 32; x++)
                    {
                        for (int y = 0; y < 32; y++)
                        {
                            int value = pixels1[x, y];
                            int value2 = pixels2[x, y];

                            calcList[32 * y + x].AddDataPoint(value);
                            calcList[32 * y + x].AddDataPoint(value2);

                            sigmaSum += Math.Abs(value - value2) / 2.0;
                        }
                    }

                    calcList.ForEach(c => c.Compute());
                    m_SigmaDict[i] = sigmaSum / 1024;

					Plot(calcList);
					PlotSigmas(m_SigmaDict);

					Refresh();

					m_AverageCalculatorsPerFrame.Add(calcList);
				}

				foundIntegration = ComputeIntegration();

				if (foundIntegration != null)
				{
					if (k == 0)
					{
						if (foundIntegration.Interval < 32 && foundIntegration.Certainty > 1)
							break;
					}
					else if (k == 1)
					{
                        if (foundIntegration.Interval < 64 && foundIntegration.Certainty > 1)
							break;
					}
					else if (k == 2)
					{
                        if (foundIntegration.Interval < 128 && foundIntegration.Certainty > 1)
							break;
					}
				}

				if (k < 3)
				{
					if (k < 2)
						framesToMeasure += 65;
					else
						framesToMeasure += 129;

					firstFrame = lastFrame - 1;
					lastFrame = Math.Min(m_ImageProvider.LastFrame, m_StartFrameId + framesToMeasure);

					progressBar1.Maximum = lastFrame - 1;

					int currSize = m_SigmaDict.Count;
					for (int i = currSize + m_StartFrameId; i < lastFrame - 1; i++)
						m_SigmaDict.Add(i, double.NaN);
				}
			}

			progressBar1.Value = progressBar1.Maximum;
			progressBar1.Style = ProgressBarStyle.Continuous;
			progressBar1.Refresh();

			pnlResult.Visible = true;
			pnlResult.BringToFront();

			
			DisplayFoundIntegration(foundIntegration);
		}

		private PotentialIntegrationFit m_FoundIntegration;

		private void DisplayFoundIntegration(PotentialIntegrationFit foundIntegration)
		{
			m_FoundIntegration = foundIntegration;

			if (foundIntegration != null)
			{
				lblIntegrationFrames.Text = string.Format("{0} frames", foundIntegration.Interval);
				lblStartingAt.Text = foundIntegration.StartingAtFrame.ToString();
				lblCertainty.Text = foundIntegration.CertaintyText;

				btnAccept.Visible = true;
				btnReject.Text = "Reject";
			}
			else
			{
				lblIntegrationFrames.Text = "Detection Failed";
				lblStartingAt.Text = "N/A";
				lblCertainty.Text = "N/A";

				btnAccept.Visible = false;
				btnReject.Text = "Close";
			}
		}

		internal PotentialIntegrationFit IntegratedFrames
		{
			get { return m_FoundIntegration; }
		}

		private double xScale;
		private double minY;
		private int minX;
		private double m_Variance;
		private double m_Median;

		private void Plot(List<AverageCalculator> calcs)
		{
			double minY = calcs.Min(a => a.Average - a.Sigma);
			double maxY = calcs.Max(a => a.Average + a.Sigma);

			double scaleY = (picFrameSpectrum.Height - 10) / (maxY - minY);
			double xScale = 1.0 * (picFrameSpectrum.Width - 10) / (0.5 * calcs.Count); 
			
			using (Graphics g = Graphics.FromImage(picFrameSpectrum.Image))
			{
				g.Clear(Color.WhiteSmoke);

				for (int i = 0; i < calcs.Count - 2; i+=2)
				{
					float x = (float)(i * xScale) + 5;
					float y = 5 + (float)(scaleY * (calcs[i].Average - minY));
					float x2 = (float)((i + 2)* xScale) + 5;
					float y2 = 5 + (float)(scaleY * (calcs[i + 2].Average - minY));

					float yFrom = 5 + (float)(scaleY * (calcs[i].Average - calcs[i].Sigma - minY));
					float yTo = 5 + (float)(scaleY * (calcs[i].Average + calcs[i].Sigma - minY));

					g.DrawLine(Pens.Red, x, yFrom, x, yTo);
					g.DrawLine(Pens.Black, x, y, x2, y2);
				}

				g.Save();
			}

			picFrameSpectrum.Refresh();
		}

		private void PlotSigmas(Dictionary<int, double> sigmas)
		{
			minX = sigmas.Keys.Min();
			int maxX = sigmas.Keys.Max();

            if (maxX == minX) maxX = minX + 1;
            if (maxX == 0) maxX = 1;

			List<double> vals = sigmas.Values.Where(v => !double.IsNaN(v)).ToList();

			minY = vals.Min();
			double maxY = vals.Max();

            if (maxY == minY) maxY = 1.1 * minY;
            if (maxY == 0) maxY = 1;

			vals.Sort();

			m_Median = vals.Count % 2 == 1
				? vals[vals.Count / 2]
				: (vals[vals.Count / 2] + vals[(vals.Count / 2) - 1]) / 2;

			List<double> residuals = vals.Select(v => Math.Abs(m_Median - v)).ToList();

			m_Variance = residuals.Select(r => r * r).Sum();
			if (residuals.Count > 1)
			{
				m_Variance = Math.Sqrt(m_Variance/(residuals.Count - 1));
			}
			else
				m_Variance = double.NaN;
			
			xScale = 1.0 * (picSigmas.Width - 10) / (maxX - minX);
			double yScale = (picSigmas.Height - 10) / (maxY - minY);

			using(Graphics g = Graphics.FromImage(picSigmas.Image))
			{
				g.Clear(Color.WhiteSmoke);

				foreach(int key in sigmas.Keys)
				{
					if (double.IsNaN(sigmas[key])) break;

					float x = 5 + (float)((key - minX) * xScale);
					float x2 = 4 + (float)((key + 1 - minX) * xScale);
					float y = (5 + (float)((sigmas[key] - minY) * yScale));

					double residual = Math.Abs(m_Median - sigmas[key]);
					Brush brush = residual < m_Variance ? Brushes.Red : Brushes.Green;

					g.FillRectangle(brush, x, picSigmas.Height - y - 4, x2 - x, y + 4);
				}

				g.Save();
			}

			picSigmas.Refresh();
		}

		private void picSigmas_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Middle)
			{
				double key = (e.X - 5) / xScale + minX;
				List<AverageCalculator> cals = m_AverageCalculatorsPerFrame[(int)key];
				Plot(cals);

				pnlResult.Visible = false;
				pnlResult.SendToBack();				
			}
		}

		internal class PotentialIntegrationFit
		{
			public int StartingAtFrame;
			public int Interval;
			public double Certainty;
            public double AboveSigmaRatio;
			public string CertaintyText;
		}

		private PotentialIntegrationFit ComputeIntegration()
		{
			List<int> INTERVALS = new List<int>(new int[] {2, 4, 8, 16, 32, 64, 128});
			var fitValues = new Dictionary<int, double>();
			var fitFlags = new Dictionary<int, bool>();
            var fitAboveSigmaRatios = new Dictionary<int, double>();

			List<PotentialIntegrationFit> guessesMinFrame = new List<PotentialIntegrationFit>();
			List<PotentialIntegrationFit> guessesMaxOffset = new List<PotentialIntegrationFit>();

			foreach(int interval in INTERVALS)
			{
				fitValues.Clear();
				fitFlags.Clear();
                fitAboveSigmaRatios.Clear();

				for (int i = 0; i < interval; i++)
				{
					if (interval >= m_SigmaDict.Count) 
						continue; // Not all have 128 measurements

					int k = 0;
					double fitVal = 0;
					bool allAboveSigma = true;
				    int aboveSigma = 0;
				    double aboveSigmaRatio = 1;
                    double aboveSigmaRatioSum = 0;
					do
					{
                        double testVal = Math.Abs(m_SigmaDict[minX + i + k * interval] - m_Median);
						fitVal += testVal;
                        if (testVal >= m_Variance)
                        {
                            aboveSigma++;
                            aboveSigmaRatioSum += m_SigmaDict[minX + i + k * interval] / m_Median;
                        }
						k++;
					}
					while (i + k * interval < m_SigmaDict.Count);

                    // If more than all but 1 are above sigma this is a candidate
					allAboveSigma = aboveSigma > 0 && aboveSigma + 1 >= k;
                    aboveSigmaRatio = aboveSigma > 0 ? aboveSigmaRatioSum / aboveSigma : 0;

					fitValues.Add(i, fitVal / k);
					fitFlags.Add(i, allAboveSigma);
                    fitAboveSigmaRatios.Add(i, aboveSigmaRatio);
				}	

				// Find the best guess for this interval

				Dictionary<int, double> aboveSigmaFits = fitValues
					.Where(kvp => fitFlags[kvp.Key])
					.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

				if (aboveSigmaFits.Count > 0)
				{
					KeyValuePair<int, double> bestBet = aboveSigmaFits
						.Where(kvp => kvp.Key == aboveSigmaFits.Keys.Min()).First();

					// Potential ingtegration with interval [interval], starting at frame bestBet.Key and with fitted value of bestFit.Value
					guessesMinFrame.Add(
						new PotentialIntegrationFit()
							{
								StartingAtFrame = minX + bestBet.Key + 1,
								Interval = interval,
								Certainty = bestBet.Value,
                                AboveSigmaRatio = fitAboveSigmaRatios[bestBet.Key]
							});

					bestBet = aboveSigmaFits
						.Where(kvp => kvp.Value == aboveSigmaFits.Values.Max()).First();

					// Potential ingtegration with interval [interval], starting at frame bestBet.Key and with fitted value of bestFit.Value
					guessesMaxOffset.Add(
						new PotentialIntegrationFit()
						{
							StartingAtFrame = minX + bestBet.Key + 1,
							Interval = interval,
							Certainty = bestBet.Value,
                            AboveSigmaRatio = fitAboveSigmaRatios[bestBet.Key]
						});
				}
			}

			List<List<PotentialIntegrationFit>> guessesList = new List<List<PotentialIntegrationFit>>();
			guessesList.Add(guessesMaxOffset);
			guessesList.Add(guessesMinFrame);

			foreach (List<PotentialIntegrationFit> guesses in guessesList)
			{
				if (guesses.Count > 0)
				{
					// Sort by increasing interval to check for resonances
					guesses.Sort((g, h) => g.Interval.CompareTo(h.Interval));

					bool isResonance = true;
					do
					{
						int prevInterval = guesses[0].Interval;
						int firstInterval = guesses[0].Interval;
						int firstFrame = guesses[0].StartingAtFrame;

						for (int i = 1; i < guesses.Count; i++)
						{
							if (guesses[i].Interval != 2 * prevInterval ||
                                Math.Abs(firstFrame - guesses[i].StartingAtFrame) % prevInterval != 0)
							{
								isResonance = false;
								break;
							}

							prevInterval = guesses[i].Interval;
						}

						if (!isResonance && guesses.Count > 1)
						{
							guesses.RemoveAt(0);
							isResonance = true;
						}
						else
							break;
					}
					while (true);

					if (isResonance)
					{
						if (IsGuessWithSimilarSigmaAsOtherNonGuesses(guesses[0]))
							continue;

						// found 2, 4, 8, 16 and 32 OR
						// found 4, 8, 16 and 32 OR
						// found 8, 16 and 32 OR
						// found 16 and 32

                        PotentialIntegrationFit smallerOrSameFit = SearchSmallerFit(guesses[0]);
						
                        return smallerOrSameFit;
					}
					else
					{
						if (IsGuessWithSimilarSigmaAsOtherNonGuesses(guesses[guesses.Count - 1]))
							continue;

						guesses.Sort((g, h) => -1 * g.Certainty.CompareTo(h.Certainty));
						guesses[guesses.Count - 1].CertaintyText = "Possible";
					    guesses[guesses.Count - 1].Certainty = 1;

						return guesses[guesses.Count - 1];
					}
				}				
			}

			return null;
		}

        private PotentialIntegrationFit SearchSmallerFit(PotentialIntegrationFit largestFit)
        {
            int integration = largestFit.Interval / 2;
            List<PotentialIntegrationFit> smallerFitsToCheckFor = new List<PotentialIntegrationFit>();

            while (integration > 2)
            {
                int potentialStartingFrame = largestFit.StartingAtFrame;
                while (potentialStartingFrame > m_StartFrameId + integration - 1) potentialStartingFrame -= integration;

                while (potentialStartingFrame < minX) potentialStartingFrame += integration;

                smallerFitsToCheckFor.Insert(0, new PotentialIntegrationFit() { Interval = integration, StartingAtFrame = potentialStartingFrame, AboveSigmaRatio = largestFit.AboveSigmaRatio});

                integration /= 2;
            }

            foreach(PotentialIntegrationFit fit in smallerFitsToCheckFor)
            {
                int k = 0;
                int aboveTwoSigma = 0;
				double fitVal = 0;
				do
				{
				    int frameId = minX + fit.StartingAtFrame - 1 + k*fit.Interval;
                    if (frameId >= 0 && frameId < m_SigmaDict.Count)
                    {
                        double deviation = Math.Abs(m_SigmaDict[frameId] - m_Median);
                        fitVal += deviation;
                        if (deviation > m_Variance * 2) aboveTwoSigma++;
                    }
                    k++;
				}
                while (fit.StartingAtFrame + k * fit.Interval < m_SigmaDict.Count);

                if (k == aboveTwoSigma && k > 2)
                {
                    fit.CertaintyText = "Average";
                    fit.Certainty = 3;
                    return fit;
                }

                if (k == aboveTwoSigma + 1 && k > 4)
                {
                    fit.CertaintyText = "Average";
                    fit.Certainty = 3;
                    return fit;
                }

                if (k == aboveTwoSigma + 2 && k > 8)
                {
                    fit.CertaintyText = "Below Average";
                    fit.Certainty = 2;
                    return fit;
                }

                if (k == aboveTwoSigma + 3 && k > 16)
                {
                    fit.CertaintyText = "Below Average";
                    fit.Certainty = 2;
                    return fit;
                }
            }

            largestFit.CertaintyText = largestFit.AboveSigmaRatio > 3.5 ? "Good" :  "Average";
            largestFit.Certainty = 4;

            return largestFit;
        }

		private bool IsGuessWithSimilarSigmaAsOtherNonGuesses(PotentialIntegrationFit guess)
		{
			int frameToCheck = guess.StartingAtFrame - 1;
			int cnt = 0;
			double sigmaSum = 0;
			while (frameToCheck > 0 && frameToCheck < m_SigmaDict.Count)
			{
				sigmaSum += Math.Abs(m_SigmaDict[frameToCheck]);
				cnt++;
				frameToCheck += guess.Interval;
			}
			double averageSigmaForGuess = sigmaSum / cnt;
			Dictionary<int, double> sameOrHigherPoints = m_SigmaDict
				.Where(kvp => (kvp.Key - guess.StartingAtFrame + 1) % guess.Interval != 0 &&
							  kvp.Value >= averageSigmaForGuess)
				.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
			int numPointsNotInGuessWithSameOrHigherSigmas = sameOrHigherPoints.Count();

			if (numPointsNotInGuessWithSameOrHigherSigmas > (1.0 * m_SigmaDict.Count / guess.Interval) - 1)
			{
				// Too many other points with similar sigma
				return true;
			}

			return false;
		}

        private void btnAccept_Click(object sender, EventArgs e)
        {
            if (m_FoundIntegration == null)
            {
                DialogResult = DialogResult.Cancel;
                Close();
            }
            else
            {
                DialogResult = DialogResult.OK;
                Close();                
            }
        }
	}

    public interface IImagePixelProvider
    {
        int Width { get; }
        int Height { get; }
        int LastFrame { get; }
        int[,] GetPixelArray(int frameNo, Rectangle rect);
    }
}
