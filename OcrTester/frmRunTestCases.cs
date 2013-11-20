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
            string[] testCases = Directory.GetDirectories(Path.GetFullPath(@"..\..\AutomatedTestingImages"));

            pbar.Maximum = pbarSuccess.Maximum = pbarError.Maximum =  testCases.Length * 2;
            pbar.Minimum = pbarSuccess.Minimum = pbarError.Minimum = 0;
            pbar.Value = pbarSuccess.Value = pbarError.Value = 0;
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

                    for (int i = 0; i < testFiles.Count / 2; i++)
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

        //private void PlotDigitPatterns()
        //{
        //    if (m_Processor.ZeroDigitPattern != null && m_Processor.ZeroDigitPattern.Length > 0)
        //    {
        //        Bitmap bmpZero = Pixelmap.ConstructBitmapFromBitmapPixels(m_Processor.ZeroDigitPattern, m_Processor.BlockWidth, m_Processor.BlockHeight);
        //        picZero.Image = bmpZero;
        //        picZero.Update();
        //    }

        //    if (m_Processor.OneDigitPattern != null && m_Processor.OneDigitPattern.Length > 0)
        //    {
        //        Bitmap bmpOne = Pixelmap.ConstructBitmapFromBitmapPixels(m_Processor.OneDigitPattern, m_Processor.BlockWidth, m_Processor.BlockHeight);
        //        picOne.Image = bmpOne;
        //        picOne.Update();
        //    }

        //    if (m_Processor.TwoDigitPattern != null && m_Processor.TwoDigitPattern.Length > 0)
        //    {
        //        Bitmap bmpTwo = Pixelmap.ConstructBitmapFromBitmapPixels(m_Processor.TwoDigitPattern, m_Processor.BlockWidth, m_Processor.BlockHeight);
        //        picTwo.Image = bmpTwo;
        //        picTwo.Update();
        //    }

        //    if (m_Processor.ThreeDigitPattern != null && m_Processor.ThreeDigitPattern.Length > 0)
        //    {
        //        Bitmap bmpThree = Pixelmap.ConstructBitmapFromBitmapPixels(m_Processor.ThreeDigitPattern, m_Processor.BlockWidth, m_Processor.BlockHeight);
        //        picThree.Image = bmpThree;
        //        picThree.Update();
        //    }

        //    if (m_Processor.FourDigitPattern != null && m_Processor.FourDigitPattern.Length > 0)
        //    {
        //        Bitmap bmpFour = Pixelmap.ConstructBitmapFromBitmapPixels(m_Processor.FourDigitPattern, m_Processor.BlockWidth, m_Processor.BlockHeight);
        //        picFour.Image = bmpFour;
        //        picFour.Update();
        //    }

        //    if (m_Processor.FiveDigitPattern != null && m_Processor.FiveDigitPattern.Length > 0)
        //    {
        //        Bitmap bmpFive = Pixelmap.ConstructBitmapFromBitmapPixels(m_Processor.FiveDigitPattern, m_Processor.BlockWidth, m_Processor.BlockHeight);
        //        picFive.Image = bmpFive;
        //        picFive.Update();
        //    }

        //    if (m_Processor.SixDigitPattern != null && m_Processor.SixDigitPattern.Length > 0)
        //    {
        //        Bitmap bmpSix = Pixelmap.ConstructBitmapFromBitmapPixels(m_Processor.SixDigitPattern, m_Processor.BlockWidth, m_Processor.BlockHeight);
        //        picSix.Image = bmpSix;
        //        picSix.Update();
        //    }

        //    if (m_Processor.SevenDigitPattern != null && m_Processor.SevenDigitPattern.Length > 0)
        //    {
        //        Bitmap bmpSeven = Pixelmap.ConstructBitmapFromBitmapPixels(m_Processor.SevenDigitPattern, m_Processor.BlockWidth, m_Processor.BlockHeight);
        //        picSeven.Image = bmpSeven;
        //        picSeven.Update();
        //    }

        //    if (m_Processor.EightDigitPattern != null && m_Processor.EightDigitPattern.Length > 0)
        //    {
        //        Bitmap bmpEight = Pixelmap.ConstructBitmapFromBitmapPixels(m_Processor.EightDigitPattern, m_Processor.BlockWidth, m_Processor.BlockHeight);
        //        picEight.Image = bmpEight;
        //        picEight.Update();
        //    }

        //    if (m_Processor.NineDigitPattern != null && m_Processor.NineDigitPattern.Length > 0)
        //    {
        //        Bitmap bmpNine = Pixelmap.ConstructBitmapFromBitmapPixels(m_Processor.NineDigitPattern, m_Processor.BlockWidth, m_Processor.BlockHeight);
        //        picNine.Image = bmpNine;
        //        picNine.Update();
        //    }
        //}
    }
}
