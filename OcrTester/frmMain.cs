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

namespace OcrTester
{
	public partial class frmMain : Form
	{
		private List<string> m_InputFiles = new List<string>();
		private int m_CurrentIndex = -1;
		private Bitmap m_CurrentImage;
		private Pixelmap m_Pixelmap;
        private IotaVtiOcrProcessor m_Processor;
		private int m_Width = 0;
		private int m_Height = 0;

		public frmMain()
		{
			InitializeComponent();

            m_Processor = new IotaVtiOcrProcessor();
		}

		private void btnReload_Click(object sender, EventArgs e)
		{
			m_CurrentIndex = -1;
			m_InputFiles.Clear();
			m_InputFiles.AddRange(Directory.GetFiles(tbxInputFolder.Text, "*.bmp"));
            m_InputFiles.Sort((x, y) =>
                {
                    string fnX = Path.GetFileName(x);
                    string fnY = Path.GetFileName(y);

                    int xNum = int.Parse(fnX.Substring(0, 2));
                    int yNum = int.Parse(fnY.Substring(0, 2));

                    if (xNum == yNum)
                    {
                        if (fnX.CompareTo(fnY) == 0)
                            return 0;

                        if (cbxReverseEvenOdd.Checked)
                            return fnX[3] == 'o' ? -1 : 1;
                        else
                            return x.CompareTo(y);
                    }
                    else
                        return xNum.CompareTo(yNum);
                });

			if (m_InputFiles.Count > 0) m_CurrentIndex = 0;
			ProcessCurrentImage();
		}

		private void ProcessCurrentImage()
		{
			if (m_CurrentIndex >= 0 && m_CurrentIndex < m_InputFiles.Count)
			{
				string fileName = m_InputFiles[m_CurrentIndex];
			    lblDisplayedFile.Text = fileName;
				m_CurrentImage = (Bitmap)Bitmap.FromFile(fileName);
				m_Pixelmap = Pixelmap.ConstructFromBitmap(m_CurrentImage, TangraConfig.ColourChannel.Red);

				using (Graphics g = Graphics.FromImage(m_CurrentImage))
				{
					m_Processor.Process(m_Pixelmap, g);
					g.Flush();
				}

				picField.Image = m_CurrentImage;
				picField.Update();

			    lblBlockWidth.Text = m_Processor.BlockWidth.ToString();
                lblBlockHeight.Text = m_Processor.BlockHeight.ToString();
                lblBlockXOffs.Text = m_Processor.BlockOffsetX.ToString();
                lblBlockYOffs.Text = m_Processor.BlockOffsetY.ToString();

			    PlotDigitPatterns();
			}
		}

        private void PlotDigitPatterns()
        {
            if (m_Processor.ZeroDigitPattern != null && m_Processor.ZeroDigitPattern.Length > 0)
            {
                Bitmap bmpZero = Pixelmap.ConstructBitmapFromBitmapPixels(m_Processor.ZeroDigitPattern, m_Processor.BlockWidth, m_Processor.BlockHeight);
                picZero.Image = bmpZero;
                picZero.Update();
            }

            if (m_Processor.OneDigitPattern != null && m_Processor.OneDigitPattern.Length > 0)
            {
                Bitmap bmpOne = Pixelmap.ConstructBitmapFromBitmapPixels(m_Processor.OneDigitPattern, m_Processor.BlockWidth, m_Processor.BlockHeight);
                picOne.Image = bmpOne;
                picOne.Update();
            }

            if (m_Processor.TwoDigitPattern != null && m_Processor.TwoDigitPattern.Length > 0)
            {
                Bitmap bmpTwo = Pixelmap.ConstructBitmapFromBitmapPixels(m_Processor.TwoDigitPattern, m_Processor.BlockWidth, m_Processor.BlockHeight);
                picTwo.Image = bmpTwo;
                picTwo.Update();
            }

            if (m_Processor.ThreeDigitPattern != null && m_Processor.ThreeDigitPattern.Length > 0)
            {
                Bitmap bmpThree = Pixelmap.ConstructBitmapFromBitmapPixels(m_Processor.ThreeDigitPattern, m_Processor.BlockWidth, m_Processor.BlockHeight);
                picThree.Image = bmpThree;
                picThree.Update();
            }

            if (m_Processor.FourDigitPattern != null && m_Processor.FourDigitPattern.Length > 0)
            {
                Bitmap bmpFour = Pixelmap.ConstructBitmapFromBitmapPixels(m_Processor.FourDigitPattern, m_Processor.BlockWidth, m_Processor.BlockHeight);
                picFour.Image = bmpFour;
                picFour.Update();
            }

            if (m_Processor.FiveDigitPattern != null && m_Processor.FiveDigitPattern.Length > 0)
            {
                Bitmap bmpFive = Pixelmap.ConstructBitmapFromBitmapPixels(m_Processor.FiveDigitPattern, m_Processor.BlockWidth, m_Processor.BlockHeight);
                picFive.Image = bmpFive;
                picFive.Update();
            }

            if (m_Processor.SixDigitPattern != null && m_Processor.SixDigitPattern.Length > 0)
            {
                Bitmap bmpSix = Pixelmap.ConstructBitmapFromBitmapPixels(m_Processor.SixDigitPattern, m_Processor.BlockWidth, m_Processor.BlockHeight);
                picSix.Image = bmpSix;
                picSix.Update();
            }

            if (m_Processor.SevenDigitPattern != null && m_Processor.SevenDigitPattern.Length > 0)
            {
                Bitmap bmpSeven = Pixelmap.ConstructBitmapFromBitmapPixels(m_Processor.SevenDigitPattern, m_Processor.BlockWidth, m_Processor.BlockHeight);
                picSeven.Image = bmpSeven;
                picSeven.Update();
            }

            if (m_Processor.EightDigitPattern != null && m_Processor.EightDigitPattern.Length > 0)
            {
                Bitmap bmpEight = Pixelmap.ConstructBitmapFromBitmapPixels(m_Processor.EightDigitPattern, m_Processor.BlockWidth, m_Processor.BlockHeight);
                picEight.Image = bmpEight;
                picEight.Update();
            }

            if (m_Processor.NineDigitPattern != null && m_Processor.NineDigitPattern.Length > 0)
            {
                Bitmap bmpNine = Pixelmap.ConstructBitmapFromBitmapPixels(m_Processor.NineDigitPattern, m_Processor.BlockWidth, m_Processor.BlockHeight);
                picNine.Image = bmpNine;
                picNine.Update();
            }
        }

		private void btnNext_Click(object sender, EventArgs e)
		{
			if (m_CurrentIndex < m_InputFiles.Count - 1)
			{
				m_CurrentIndex++;
				ProcessCurrentImage();
			}
		}

		private void btnPrev_Click(object sender, EventArgs e)
		{
			if (m_CurrentIndex > 0)
			{
				m_CurrentIndex--;
				ProcessCurrentImage();
			}
		}

		private void picField_MouseDown(object sender, MouseEventArgs e)
		{
		    uint[] blockPixels = m_Processor.GetBlockAt(e.X, e.Y);
		    Bitmap bmpBlock = Pixelmap.ConstructBitmapFromBitmapPixels(blockPixels, m_Processor.BlockWidth, m_Processor.BlockHeight);
		    picBlock.Image = bmpBlock;
		    picBlock.Update();
		}
	}
}
