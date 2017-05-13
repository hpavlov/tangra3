/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

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
using OcrTester.IotaVtiOsdProcessor;
using Tangra.Model.Config;
using Tangra.Model.Image;
using Tangra.OCR;
using VideoFormat = Tangra.OCR.IotaVtiOsdProcessor.VideoFormat;

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
            RunCorrectionTests();

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
                bool isTvSafeGuess = folderNameOnly.EndsWith("_tvsafe");
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
							calibrated = ocrEngine.ProcessCalibrationFrame(i / 2, pixelmapOdd.Pixels, pixelmapEven.Pixels, bmpOdd.Width, bmpOdd.Height, isTvSafeGuess);

                        if (calibrated)
                        {
                            if (!digitsPlotted)
                            {
                                PlotDigitPatterns(ocrEngine);
                                digitsPlotted = true;
                            }

                            DateTime dt;
                            isSuccess = ocrEngine.ExtractTime(i / 2, 1, pixelmapOdd.Pixels, pixelmapEven.Pixels, bmpOdd.Width, bmpOdd.Height, out dt);
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
	        bool[] options = new bool[] {false, true};
	        string[] optionNames = new string[] { "Managed", "Native"};
			for (int j = 0; j < options.Length; j++)
	        {
				foreach (string file in testCases)
				{
					string expectedResult = Path.GetFullPath(noiseTestResults + "\\" + Path.GetFileName(file));

					lvlTestCaseDescription.Text = string.Format("Test case NoiseChunksRemoval\\{0} ({1})", Path.GetFileName(file), optionNames[j]);
					lvlTestCaseDescription.Update();

					Pixelmap pix = Pixelmap.ConstructFromBitmap((Bitmap)Bitmap.FromFile(file), TangraConfig.ColourChannel.Red);

					uint[] pixels = pix.Pixels;

					LargeChunkDenoiser.Process(options[j], pixels, pix.Width, pix.Height);

					Pixelmap pixExpected = Pixelmap.ConstructFromBitmap((Bitmap)Bitmap.FromFile(expectedResult), TangraConfig.ColourChannel.Red);

					for (int i = 0; i < pix.Pixels.Length; i++)
					{
						if (pix.Pixels[i] != pixExpected.Pixels[i])
						{
							lbErrors.Items.Add(string.Format("NoiseChunk Removal Failed for {0} ({1})", Path.GetFileName(file), optionNames[j]));
						    //Bitmap bmp = Pixelmap.ConstructBitmapFromBitmapPixels(pixels, pix.Width, pix.Height);
						    //bmp.Save(Path.ChangeExtension(expectedResult, ".errbmp"));
							hasErrors = true;
							break;
						}
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

        private void RunCorrectionTests()
        {
            //ORC ERR: FrameNo: 227, Odd Timestamp: 3:55:8.995 140698, Even Timestamp: 3:55:8.1152 140699, NTSC field is not 16.68 ms. It is 15 ms. IOTA-VTI Correction Attempt for Frame 227. 03:55:08.0995 (140698) - 03:55:08.1152 (140699)
            //ORC ERR: FrameNo: 228, Odd Timestamp: 3:55:8.1319 140700, Even Timestamp: 3:55:8.1496 140701, NTSC field is not 16.68 ms. It is 18 ms. IOTA-VTI Correction Attempt for Frame 228. 03:55:08.1319 (140700) - 03:55:08.1496 (140701)
            //ORC ERR: FrameNo: 230, Odd Timestamp: 3:55:8.1996 140704, Even Timestamp: 3:55:8.2153 140705, NTSC field is not 16.68 ms. It is 15 ms. IOTA-VTI Correction Attempt for Frame 230. 03:55:08.1996 (140704) - 03:55:08.2153 (140705)
            //ORC ERR: FrameNo: 231, Odd Timestamp: 3:55:8.2320 140706, Even Timestamp: 3:55:8.2497 140707, NTSC field is not 16.68 ms. It is 18 ms. IOTA-VTI Correction Attempt for Frame 231. 03:55:08.2320 (140706) - 03:55:08.2497 (140707)
            //ORC ERR: FrameNo: 721, Odd Timestamp: 3:55:24.5818 141686, Even Timestamp: 3:55:24.5995 141687, NTSC field is not 16.68 ms. It is 18 ms. IOTA-VTI Correction Attempt for Frame 721. 03:55:24.5818 (141686) - 03:55:24.5995 (141687)
            //ORC ERR: FrameNo: 724, Odd Timestamp: 3:55:24.6819 141692, Even Timestamp: 3:55:24.6996 141693, NTSC field is not 16.68 ms. It is 18 ms. IOTA-VTI Correction Attempt for Frame 724. 03:55:24.6819 (141692) - 03:55:24.6996 (141693)
            //ORC ERR: FrameNo: 727, Odd Timestamp: 3:55:24.7820 141698, Even Timestamp: 3:55:24.7997 141699, NTSC field is not 16.68 ms. It is 18 ms. IOTA-VTI Correction Attempt for Frame 727. 03:55:24.7820 (141698) - 03:55:24.7997 (141699)
            //ORC ERR: FrameNo: 730, Odd Timestamp: 3:55:24.8821 141704, Even Timestamp: 3:55:24.8998 141705, NTSC field is not 16.68 ms. It is 18 ms. IOTA-VTI Correction Attempt for Frame 730. 03:55:24.8821 (141704) - 03:55:24.8998 (141705)
            //ORC ERR: FrameNo: 733, Odd Timestamp: 3:55:24.9822 141710, Even Timestamp: 3:55:24.9999 141711, NTSC field is not 16.68 ms. It is 18 ms. IOTA-VTI Correction Attempt for Frame 733. 03:55:24.9822 (141710) - 03:55:24.9999 (141711)
            //ORC ERR: FrameNo: 1219, Odd Timestamp: 3:55:41.1995 142682, Even Timestamp: 3:55:41.2152 142683, NTSC field is not 16.68 ms. It is 15 ms. IOTA-VTI Correction Attempt for Frame 1219. 03:55:41.1995 (142682) - 03:55:41.2152 (142683)

            //ORC ERR: FrameNo: 724, Odd Timestamp: 3:55:24.6819 141692, Even Timestamp: 3:55:24.6996 141693, NTSC field is not 16.68 ms. It is 18 ms. IOTA-VTI Correction Attempt for Frame 724. 03:55:24.6819 (141692) - 03:55:24.6996 (141693)
            
            var processor = new IotaVtiOrcManaged();
            processor.m_Processor = new Tangra.OCR.IotaVtiOsdProcessor.IotaVtiOcrProcessor(true);
            processor.m_Processor.VideoFormat = VideoFormat.NTSC;

            var oddFieldOSD = new Tangra.OCR.IotaVtiOsdProcessor.IotaVtiTimeStampStrings() { HH = "3", MM = "55", SS = "24", FFFF1 = "6819", FFFF2 = "", FRAMENO = "141692", NumSat = '8' };
            var evenFieldOSD = new Tangra.OCR.IotaVtiOsdProcessor.IotaVtiTimeStampStrings() { HH = "3", MM = "55", SS = "24", FFFF1 = "", FFFF2 = "6996", FRAMENO = "141693", NumSat = '8' };

            //processor.m_Corrector.m_PrevEvenTicks = new DateTime(1, 1, 1, 3, 55, 24, 665).Ticks;
            //processor.m_Corrector.m_PrevOddTicks = new DateTime(1, 1, 1, 3, 55, 24, 649).Ticks;
            //DateTime dateTime = processor.ExtractDateTime(724, 1, new Tangra.OCR.IotaVtiOsdProcessor.IotaVtiTimeStamp(oddFieldOSD), new Tangra.OCR.IotaVtiOsdProcessor.IotaVtiTimeStamp(evenFieldOSD));
        }
    }
}
