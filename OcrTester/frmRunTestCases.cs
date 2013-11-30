using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tangra.Model.Config;
using Tangra.Model.Image;
using Tangra.OCR;

namespace OcrTester
{
    public partial class frmRunTestCases : Form
    {
        public frmRunTestCases()
        {
            InitializeComponent();

            //
        }

        private void frmRunTestCases_Shown(object sender, EventArgs e)
        {
            string noiseTestSource = Path.GetFullPath(@"..\..\AutomatedTestingImages\NoiseChunksRemoval\Source");
            string noiseTestResults = Path.GetFullPath(@"..\..\AutomatedTestingImages\NoiseChunksRemoval\ExpectedResult");
            bool hasNoiseTests = Directory.Exists(noiseTestSource) && Directory.Exists(noiseTestResults);

            string[] testCases = Directory.GetDirectories(Path.GetFullPath(@"..\..\AutomatedTestingImages"), "0*");

            pbar.Maximum = pbarSuccess.Maximum = pbarError.Maximum = testCases.Length * 2 + (hasNoiseTests ? 1 : 0);
            pbar.Minimum = pbarSuccess.Minimum = pbarError.Minimum = 0;
            pbar.Value = pbarSuccess.Value = pbarError.Value = 0;

            RunOCRTestCases(testCases);

            if (hasNoiseTests)
                RunNoiseChunkRemovalTestCases(noiseTestSource, noiseTestResults);
        }

        private void RunOCRTestCases(string[] testCases)
        {
            lbErrors.Items.Clear();

            foreach (string folder in testCases)
            {
                string folderNameOnly = Path.GetFileName(folder);
                bool isTvSafe = folderNameOnly.EndsWith("_tvsafe");
                bool[] REVERSE_OPTIONS = new bool[] { false, true };

                for (int option = 0; option <= 1; option++)
                {
                    lvlTestCaseDescription.Text = string.Format("Test case {0} {1}", folderNameOnly, (option == 0 ? "" : " (Reversed)"));
                    lvlTestCaseDescription.Update();

                    List<string> testFiles = TestCaseHelper.LoadTestImages(folder, REVERSE_OPTIONS[option]);

                    var ocrEngine = new IotaVtiOrcManaged();
                    bool isSuccess = false;
                    bool calibrated = false;
                    bool digitsPlotted = false;

                    for (int i = 0; i < testFiles.Count; i++)
                    {
                        Bitmap bmpOdd = (Bitmap)Bitmap.FromFile(testFiles[i]);
                        i++;
                        Bitmap bmpEven = (Bitmap)Bitmap.FromFile(testFiles[i]);

                        Pixelmap pixelmapOdd = Pixelmap.ConstructFromBitmap(bmpOdd, TangraConfig.ColourChannel.Red);
                        Pixelmap pixelmapEven = Pixelmap.ConstructFromBitmap(bmpEven, TangraConfig.ColourChannel.Red);

                        if (!calibrated)
                            calibrated = ocrEngine.ProcessCalibrationFrame(i / 2, pixelmapOdd.Pixels, pixelmapEven.Pixels, bmpOdd.Width, bmpOdd.Height, isTvSafe);

                        if (calibrated)
                        {
                            if (!digitsPlotted)
                            {
                                PlotDigitPatterns(ocrEngine);
                                digitsPlotted = true;
                            }

                            DateTime dt;
                            isSuccess = ocrEngine.ExtractTime(i / 2, pixelmapOdd.Pixels, pixelmapEven.Pixels, bmpOdd.Width, bmpOdd.Height, out dt);
                            if (!isSuccess)
                                break;
                        }
                    }

                    pbar.Value++;

                    if (isSuccess)
                        pbarSuccess.Value++;
                    else
                    {
                        pbarError.Value++;
                        lbErrors.Items.Add(string.Format("{0}{1} - {2}", folderNameOnly, (option == 0 ? "" : " (Reversed)"), calibrated ? "Error extracting times" : " Failed to calibrate"));
                    }

                    lblError.Text = string.Format("Errored {0}/{1}", pbarError.Value, pbar.Value);
                    lblSuccessful.Text = string.Format("Successful {0}/{1}", pbarSuccess.Value, pbar.Value);

                    Application.DoEvents();
                }
            }

            lvlTestCaseDescription.Text = "";
        }

        public void RunNoiseChunkRemovalTestCases(string noiseTestSource, string noiseTestResults)
        {
            string[] testCases = Directory.GetFiles(noiseTestSource, "*.bmp");
            bool hasErrors = false;
            foreach (string file in testCases)
            {
                string expectedResult = Path.GetFullPath(noiseTestResults + "\\" + Path.GetFileName(file));

                lvlTestCaseDescription.Text = string.Format("Test case NoiseChunksRemoval\\{0}", Path.GetFileName(file));
                lvlTestCaseDescription.Update();

                Pixelmap pix = Pixelmap.ConstructFromBitmap((Bitmap)Bitmap.FromFile(file), TangraConfig.ColourChannel.Red);

                LargeChunkDenoiser.Process(pix.Pixels, pix.Width, pix.Height, 0, 255);

                Pixelmap pixExpected = Pixelmap.ConstructFromBitmap((Bitmap)Bitmap.FromFile(expectedResult), TangraConfig.ColourChannel.Red);

                for (int i = 0; i < pix.Pixels.Length; i++)
                {
                    if (pix.Pixels[i] != pixExpected.Pixels[i])
                    {
                        lbErrors.Items.Add(string.Format("NoiseChunk Removal Failed for {0}", Path.GetFileName(file)));
                        hasErrors = true;
                        break;
                    }
                }
            }

            pbar.Value++;

            if (!hasErrors)
                pbarSuccess.Value++;
            else
                pbarError.Value++;

            lblError.Text = string.Format("Errored {0}/{1}", pbarError.Value, pbar.Value);
            lblSuccessful.Text = string.Format("Successful {0}/{1}", pbarSuccess.Value, pbar.Value);

            Application.DoEvents();
        }

        private void PlotDigitPatterns(IotaVtiOrcManaged ocrEngine)
        {
            List<uint[]> patterns = ocrEngine.GetLearntDigitPatterns();

            if (patterns.Count > 12)
            {
                Bitmap bmpZero = Pixelmap.ConstructBitmapFromBitmapPixels(patterns[0], ocrEngine.BlockWidth, ocrEngine.BlockHeight);
                picZero.Image = bmpZero;
                picZero.Update();

                Bitmap bmpOne = Pixelmap.ConstructBitmapFromBitmapPixels(patterns[1], ocrEngine.BlockWidth, ocrEngine.BlockHeight);
                picOne.Image = bmpOne;
                picOne.Update();

                Bitmap bmpTwo = Pixelmap.ConstructBitmapFromBitmapPixels(patterns[2], ocrEngine.BlockWidth, ocrEngine.BlockHeight);
                picTwo.Image = bmpTwo;
                picTwo.Update();

                Bitmap bmpThree = Pixelmap.ConstructBitmapFromBitmapPixels(patterns[3], ocrEngine.BlockWidth, ocrEngine.BlockHeight);
                picThree.Image = bmpThree;
                picThree.Update();

                Bitmap bmpFour = Pixelmap.ConstructBitmapFromBitmapPixels(patterns[4], ocrEngine.BlockWidth, ocrEngine.BlockHeight);
                picFour.Image = bmpFour;
                picFour.Update();

                Bitmap bmpFive = Pixelmap.ConstructBitmapFromBitmapPixels(patterns[5], ocrEngine.BlockWidth, ocrEngine.BlockHeight);
                picFive.Image = bmpFive;
                picFive.Update();

                Bitmap bmpSix = Pixelmap.ConstructBitmapFromBitmapPixels(patterns[6], ocrEngine.BlockWidth, ocrEngine.BlockHeight);
                picSix.Image = bmpSix;
                picSix.Update();

                Bitmap bmpSeven = Pixelmap.ConstructBitmapFromBitmapPixels(patterns[7], ocrEngine.BlockWidth, ocrEngine.BlockHeight);
                picSeven.Image = bmpSeven;
                picSeven.Update();

                Bitmap bmpEight = Pixelmap.ConstructBitmapFromBitmapPixels(patterns[8], ocrEngine.BlockWidth, ocrEngine.BlockHeight);
                picEight.Image = bmpEight;
                picEight.Update();

                Bitmap bmpNine = Pixelmap.ConstructBitmapFromBitmapPixels(patterns[9], ocrEngine.BlockWidth, ocrEngine.BlockHeight);
                picNine.Image = bmpNine;
                picNine.Update();

                Bitmap bmp83 = Pixelmap.ConstructBitmapFromBitmapPixels(patterns[10], ocrEngine.BlockWidth, ocrEngine.BlockHeight);
                pic83.Image = bmp83;
                pic83.Update();

                Bitmap bmp86 = Pixelmap.ConstructBitmapFromBitmapPixels(patterns[11], ocrEngine.BlockWidth, ocrEngine.BlockHeight);
                pic86.Image = bmp86;
                pic86.Update();

                Bitmap bmp89 = Pixelmap.ConstructBitmapFromBitmapPixels(patterns[12], ocrEngine.BlockWidth, ocrEngine.BlockHeight);
                pic89.Image = bmp89;
                pic89.Update();

            }
        }
    }
}
