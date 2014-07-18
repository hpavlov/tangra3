using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OcrTester
{
    public static class TestCaseHelper
    {
        private static Regex FILE_MASK_REGEX = new Regex("(\\d+)\\-(even|odd)\\.bmp");

        public static List<string> LoadTestImages(string path, bool reverseOrder)
        {
            var rv = new List<string>();

            rv.AddRange(Directory.GetFiles(Path.GetFullPath(path), "*.bmp"));
            rv.Sort((x, y) =>
            {
                string fnX = Path.GetFileName(x);
                string fnY = Path.GetFileName(y);

                Match matchX = FILE_MASK_REGEX.Match(fnX);
                Match matchY = FILE_MASK_REGEX.Match(fnY);

                int xNum = int.Parse(matchX.Groups[1].Value);
                int yNum = int.Parse(matchY.Groups[1].Value);

                if (xNum == yNum)
                {
                    if (fnX.CompareTo(fnY) == 0)
                        return 0;

                    if (reverseOrder)
                        return matchX.Groups[2].Value == "odd" ? -1 : 1;
                    else
                        return x.CompareTo(y);
                }
                else
                    return xNum.CompareTo(yNum);
            });

            return rv;
        }

		public static void RebuildImageSequence(string folderName)
		{
			string frameImage = folderName + "\\full-frame.bmp";
			string[] timestampsEvent = Directory.GetFiles(folderName, "ORG-0000*-even.bmp");
			string[] timestampsOdd = Directory.GetFiles(folderName, "ORG-0000*-odd.bmp");
			string destFolder = folderName + "\\output";

			if (!File.Exists(frameImage))
			{
				MessageBox.Show(string.Format("'{0}' not found.", frameImage));
				return;
			}

			if (Directory.Exists(destFolder))
			{
				MessageBox.Show(string.Format("Ouput folder '{0}' already exists.", destFolder));
				return;
			}
			Directory.CreateDirectory(destFolder);

			if (timestampsEvent.Length == 0 || timestampsOdd.Length == 0 || timestampsEvent.Length != timestampsOdd.Length)
			{
				MessageBox.Show("Field timestamps (ORG-0000*-even.bmp|ORG-0000*-odd.bmp) not found ");
				return;
			}

			Bitmap bmp = (Bitmap)Bitmap.FromFile(frameImage);
			Bitmap bmp1 = (Bitmap)Bitmap.FromFile(timestampsEvent[0]);

			int blackFrom = bmp.Height - 4 * bmp1.Height;
			int blackTo = bmp.Height - 1;
			int timestampTop = 2 * (bmp.Height - 3 * bmp1.Height) / 2 + 1;
			int timestampBottom = timestampTop + 2 * bmp1.Height;
			int width = bmp.Width;

			for (int i = 0; i < timestampsEvent.Length; i++)
			{
				Bitmap bmpEven = (Bitmap)Bitmap.FromFile(timestampsEvent[i]);
				Bitmap bmpOdd = (Bitmap)Bitmap.FromFile(timestampsOdd[i]);
				bmp = (Bitmap)Bitmap.FromFile(frameImage);

				for (int y = blackFrom; y <= blackTo; y++)
				{
					bool useEven = y%2 == 1;
					int timestampLine = (y - timestampTop)/2;
					for (int x = 0; x < width; x++)
					{
						if (y < timestampTop || y >= timestampBottom)
							bmp.SetPixel(x, y, Color.Black);
						else if (useEven)
							bmp.SetPixel(x, y, bmpEven.GetPixel(x, timestampLine));
						else
							bmp.SetPixel(x, y, bmpOdd.GetPixel(x, timestampLine));
					}
				}

				bmp.Save(destFolder + "\\" + i.ToString("000") + ".bmp");
			}
		}
    }
}
