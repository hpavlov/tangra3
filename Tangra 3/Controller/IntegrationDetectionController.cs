using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tangra.Model.Helpers;
using Tangra.Model.Numerical;
using Tangra.Video;

namespace Tangra.Controller
{
    public class PotentialIntegrationFit
    {
        public int StartingAtFrame;
        public int Interval;
        public double Certainty;
        public double AboveSigmaRatio;
        public string CertaintyText;
    }

    public class IntegrationDetectionController : IDisposable
    {
        private IImagePixelProvider m_ImageProvider;

        private Dictionary<int, double> m_SigmaDict;

        public int StartFrameId { get; private set; }

        private List<List<AverageCalculator>> m_AverageCalculatorsPerFrame = new List<List<AverageCalculator>>();

        private double m_Variance;

        private double m_Median;

        private int m_MinX;

        public event Action<PotentialIntegrationFit> OnPotentialIntegration;

        public event Action<List<AverageCalculator>, Dictionary<int, double>, double, double> OnFrameData;

        public event Action<int, int> OnBeginProgress;

        public event Action<int> OnProgress;

        public event Action<int, int[,]> OnFramePixels;

        private bool m_IsRunning;

        private IntegrationDetectionController()
        { }

        public IntegrationDetectionController(IImagePixelProvider imagePixelProvider, int startFrameId)
        {
            m_ImageProvider = imagePixelProvider;
            StartFrameId = startFrameId;
        }

        private void RaiseOnPotentialIntegration(PotentialIntegrationFit fit)
        {
            Action<PotentialIntegrationFit> invoker = OnPotentialIntegration;
            if (invoker != null && m_IsRunning)
            {
                invoker.Invoke(fit);
            }
        }

        private void RaiseOnFrameData(List<AverageCalculator> calculators, Dictionary<int, double> sigmas)
        {
            m_MinX = sigmas.Keys.Min();

            List<double> vals = sigmas.Values.Where(v => !double.IsNaN(v)).ToList();

            vals.Sort();

            m_Median = vals.Count % 2 == 1
                ? vals[vals.Count / 2]
                : (vals[vals.Count / 2] + vals[(vals.Count / 2) - 1]) / 2;

            List<double> residuals = vals.Select(v => Math.Abs(m_Median - v)).ToList();

            m_Variance = residuals.Select(r => r * r).Sum();
            if (residuals.Count > 1)
            {
                m_Variance = Math.Sqrt(m_Variance / (residuals.Count - 1));
            }
            else
                m_Variance = double.NaN;

            Action<List<AverageCalculator>, Dictionary<int, double>, double, double> invoker = OnFrameData;
            if (invoker != null && m_IsRunning)
            {
                invoker.Invoke(calculators, sigmas, m_Median, m_Variance);
            }
        }

        private void RaiseOnBeginProgress(int min, int max)
        {
            Action<int, int> invoker = OnBeginProgress;
            if (invoker != null && m_IsRunning)
            {
                invoker.Invoke(min, max);
            }
        }

        private void RaiseOnProgress(int val)
        {
            Action<int> invoker = OnProgress;
            if (invoker != null && m_IsRunning)
            {
                invoker.Invoke(val);
            }
        }

        private void RaiseOnFramePixels(int framNo, int[,] pixels)
        {
            Action<int, int[,]> invoker = OnFramePixels;
            if (invoker != null && m_IsRunning)
            {
                invoker.Invoke(framNo, pixels);
            }
        }

        public void RunMeasurements()
        {
            m_IsRunning = true;

            ThreadPool.QueueUserWorkItem(DoMeasurements);
        }

        private void DoMeasurements(object state)
        {
            try
            {
                m_SigmaDict = new Dictionary<int, double>();

                Rectangle testRect = new Rectangle(
                    (m_ImageProvider.Width/2) - 16,
                    (m_ImageProvider.Height/2) - 16,
                    32, 32);

                int framesToMeasure = 65;

                int firstFrame = Math.Max(0, StartFrameId);
                int lastFrame = Math.Min(m_ImageProvider.LastFrame, StartFrameId + framesToMeasure);

                RaiseOnBeginProgress(firstFrame, lastFrame - 1);

                int[,] pixels1 = null;
                int[,] pixels2 = null;

                for (int i = firstFrame; i < lastFrame - 1; i++)
                    m_SigmaDict.Add(i, double.NaN);

                PotentialIntegrationFit foundIntegration = null;

                for (int k = 0; k < 4; k++)
                {
                    if (!m_IsRunning)
                    {
                        break;
                    }

                    for (int i = firstFrame; i < lastFrame - 1; i++)
                    {
                        if (!m_IsRunning)
                        {
                            break;
                        }

                        RaiseOnProgress(i);

                        List<AverageCalculator> calcList = new List<AverageCalculator>();

                        for (int x = 0; x < 32; x++)
                            for (int y = 0; y < 32; y++)
                                calcList.Add(new AverageCalculator());

                        if (pixels1 == null)
                        {
                            pixels1 = m_ImageProvider.GetPixelArray(i, testRect);
                            RaiseOnFramePixels(i, pixels1);
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
                        RaiseOnFramePixels(i + 1, pixels1);

                        double sigmaSum = 0;
                        for (int x = 0; x < 32; x++)
                        {
                            for (int y = 0; y < 32; y++)
                            {
                                int value = pixels1[x, y];
                                int value2 = pixels2[x, y];

                                calcList[32*y + x].AddDataPoint(value);
                                calcList[32*y + x].AddDataPoint(value2);

                                sigmaSum += Math.Abs(value - value2)/2.0;
                            }
                        }

                        calcList.ForEach(c => c.Compute());
                        m_SigmaDict[i] = sigmaSum/1024;

                        RaiseOnFrameData(calcList, m_SigmaDict);

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
                        lastFrame = Math.Min(m_ImageProvider.LastFrame, StartFrameId + framesToMeasure);

                        RaiseOnProgress(lastFrame - 1);

                        int currSize = m_SigmaDict.Count;
                        for (int i = currSize + StartFrameId; i < lastFrame - 1; i++)
                            m_SigmaDict.Add(i, double.NaN);
                    }
                }

                if (m_IsRunning)
                {
                    RaiseOnPotentialIntegration(foundIntegration);
                }

            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.GetFullStackTrace());
            }
            finally
            {
                m_IsRunning = false;
            }
        }


        private PotentialIntegrationFit ComputeIntegration()
        {
            List<int> INTERVALS = new List<int>(new int[] { 2, 4, 8, 16, 32, 64, 128 });
            var result = ComputeIntegration(INTERVALS);
            if (result != null)
                return result;

            List<int> ODD_INTERVALS_MINTRON = new List<int>(new int[] { 3, 6, 12, 24, 48 });
            return ComputeIntegration(ODD_INTERVALS_MINTRON);
        }

        private PotentialIntegrationFit ComputeIntegration(List<int> probeIntervals)
        {
            List<PotentialIntegrationFit> guessesMinFrame = new List<PotentialIntegrationFit>();
            List<PotentialIntegrationFit> guessesMaxOffset = new List<PotentialIntegrationFit>();

            foreach (int interval in probeIntervals)
            {
                if (interval == 2)
                {
                    if (ProbeIntegration2Interval(guessesMinFrame, guessesMaxOffset))
                        break;
                }
                else
                    ProbeIntegrationInterval(interval, guessesMinFrame, guessesMaxOffset);
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
                                Math.Abs(firstFrame - guesses[i].StartingAtFrame) % firstInterval != 0)
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

        public List<AverageCalculator> GetCalculatorsForFrame(int key)
        {
            if (key >= 0 && key < m_AverageCalculatorsPerFrame.Count)
                return m_AverageCalculatorsPerFrame[key];
            else
                return null;
        }

        private bool ProbeIntegration2Interval(List<PotentialIntegrationFit> guessesMinFrame, List<PotentialIntegrationFit> guessesMaxOffset)
        {
            var oddSigmas = m_SigmaDict.Where(kvp => kvp.Key % 2 == 1).Select(kvp => kvp.Value).ToList();
            var evenSigmas = m_SigmaDict.Where(kvp => kvp.Key % 2 == 0).Select(kvp => kvp.Value).ToList();

            var oddMedian = oddSigmas.Median();
            var oddVariance = Math.Sqrt(oddSigmas.Sum(x => (x - oddMedian) * (x - oddMedian)) / (oddSigmas.Count - 1));

            var evenMedian = evenSigmas.Median();
            var evenVariance = Math.Sqrt(evenSigmas.Sum(x => (x - evenMedian) * (x - evenMedian)) / (evenSigmas.Count - 1));

            if (oddMedian > evenMedian)
            {
                // Could this be a x2 integration starting from an odd frame?
                if (oddMedian - 3 * oddVariance > evenMedian + 3 * evenVariance)
                {
                    // Yes!
                    var potentialFit = new PotentialIntegrationFit()
                    {
                        Interval = 2,
                        StartingAtFrame = m_SigmaDict.Where(kvp => kvp.Key % 2 == 1).Min(kvp => kvp.Key) + 1,
                        AboveSigmaRatio = oddSigmas.Sum() / (oddMedian * oddSigmas.Count),
                        Certainty = 3,
                        CertaintyText = "Average"
                    };

                    guessesMinFrame.Add(potentialFit);
                    guessesMaxOffset.Add(potentialFit);
                    return true;
                }
            }
            else if (evenMedian > oddMedian)
            {
                // Could this be a x2 integration starting from an even frame?
                if (evenMedian - 3 * evenVariance > oddMedian + 3 * oddVariance)
                {
                    // Yes!
                    var potentialFit = new PotentialIntegrationFit()
                    {
                        Interval = 2,
                        StartingAtFrame = m_SigmaDict.Where(kvp => kvp.Key % 2 == 0).Min(kvp => kvp.Key) + 1,
                        AboveSigmaRatio = evenSigmas.Sum() / (evenMedian * evenSigmas.Count),
                        Certainty = 3,
                        CertaintyText = "Average"
                    };

                    guessesMinFrame.Add(potentialFit);
                    guessesMaxOffset.Add(potentialFit);
                    return true;
                }
            }

            return false;
        }

        private void ProbeIntegrationInterval(int interval, List<PotentialIntegrationFit> guessesMinFrame, List<PotentialIntegrationFit> guessesMaxOffset)
        {
            var fitValues = new Dictionary<int, double>();
            var fitFlags = new Dictionary<int, bool>();
            var fitAboveSigmaRatios = new Dictionary<int, double>();

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
                    double testVal = Math.Abs(m_SigmaDict[m_MinX + i + k * interval] - m_Median);
                    fitVal += testVal;
                    if (testVal >= m_Variance)
                    {
                        aboveSigma++;
                        aboveSigmaRatioSum += m_SigmaDict[m_MinX + i + k * interval] / m_Median;
                    }
                    k++;
                } while (i + k * interval < m_SigmaDict.Count);

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
                        StartingAtFrame = m_MinX + bestBet.Key + 1,
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
                        StartingAtFrame = m_MinX + bestBet.Key + 1,
                        Interval = interval,
                        Certainty = bestBet.Value,
                        AboveSigmaRatio = fitAboveSigmaRatios[bestBet.Key]
                    });
            }
        }

        private PotentialIntegrationFit SearchSmallerFit(PotentialIntegrationFit largestFit)
        {
            int integration = largestFit.Interval / 2;
            List<PotentialIntegrationFit> smallerFitsToCheckFor = new List<PotentialIntegrationFit>();

            while (integration > 2)
            {
                int potentialStartingFrame = largestFit.StartingAtFrame;
                while (potentialStartingFrame > StartFrameId + integration - 1) potentialStartingFrame -= integration;

                while (potentialStartingFrame < m_MinX) potentialStartingFrame += integration;

                smallerFitsToCheckFor.Insert(0, new PotentialIntegrationFit() { Interval = integration, StartingAtFrame = potentialStartingFrame, AboveSigmaRatio = largestFit.AboveSigmaRatio });

                integration /= 2;
            }

            foreach (PotentialIntegrationFit fit in smallerFitsToCheckFor)
            {
                int k = 0;
                int aboveTwoSigma = 0;
                double fitVal = 0;
                do
                {
                    int frameId = m_MinX + fit.StartingAtFrame - 1 + k * fit.Interval;
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

            largestFit.CertaintyText = largestFit.AboveSigmaRatio > 3.5 ? "Good" : "Average";
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

        public void Dispose()
        {
            m_IsRunning = false;
        }
    }
}
