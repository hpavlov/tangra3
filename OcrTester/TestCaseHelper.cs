using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
    }
}
