using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;

namespace Tangra.OCR.GpsBoxSprite
{
    internal class BlockSignature
    {
        public BlockSignature(int blockWidth, int blockHeight)
        {
            m_BlockWidth = blockWidth;
            m_BlockHeight = blockHeight;
        }

        public int m_BlockHeight;
        public int m_BlockWidth;
        public List<decimal[,]> AllBlocks = new List<decimal[,]>();
        public decimal[,] Signature;
        public int? Digit;
        public string HFactors;
        public string VFactors;

        internal void ComputeSignature()
        {
            int denom = 0;
            var data = new decimal[m_BlockHeight, m_BlockWidth];
            Signature = new decimal[m_BlockHeight, m_BlockWidth];

            for (int i = 0; i < AllBlocks.Count; i++)
            {
                denom++;
                for (int y = 0; y < m_BlockHeight; y++)
                {
                    for (int x = 0; x < m_BlockWidth; x++)
                    {
                        data[y, x] += AllBlocks[i][y, x];
                    }
                }
            }
            for (int y = 0; y < m_BlockHeight; y++)
            {
                for (int x = 0; x < m_BlockWidth; x++)
                {
                    Signature[y, x] = (uint)Math.Round(data[y, x] / denom);
                }
            }
        }
    }

    public class GpsBoxSpriteOcrProcessor
    {
        private List<GpsBoxSpriteLineOcr> m_Lines = new List<GpsBoxSpriteLineOcr>();
        private List<BlockSignature> m_Signatures = new List<BlockSignature>(); 

        private int m_FrameWidth;
        private int m_FrameHeight;

        private int m_BlockWidth;
        private int m_BlockHeight;

        static int[] TWOLINE_LINE1_INDEXES = new int[] { 0, 1, 3, 4, 6, 7, 9, 10, 12, 13, 15, 16 };
        static int[] TWOLINE_LINE2_INDEXES = new int[] { 0, 1, 2, 3, 5, 9, 10, 11, 12, 14 };

        public static GpsBoxSpriteOcrProcessor CreateAndCalibrateProcessor(List<OcrCalibrationFrame> lineConfigs, Tuple<int, int>[] topBottoms, Tuple<decimal, decimal>[] leftWidths, int frameWidth, int frameHeight)
        {
            var processor = new GpsBoxSpriteOcrProcessor(lineConfigs, topBottoms, leftWidths, frameWidth, frameHeight);
            if (processor.IsCalibrated)
                return processor;

            return null;
        }

        private GpsBoxSpriteOcrProcessor(List<OcrCalibrationFrame> lineConfigs, Tuple<int, int>[] topBottoms, Tuple<decimal, decimal>[] leftWidths, int frameWidth, int frameHeight)
        {
            m_FrameWidth = frameWidth;
            m_FrameHeight = frameHeight;

            if (topBottoms.Length == 2)
            {
                if (IsToLineConfiguration(lineConfigs, topBottoms))
                    CreateTwoLineConfiguration(topBottoms, leftWidths);
            }
            else
            {
                return;
            }

            var allBlocks = new List<Tuple<decimal[,], decimal[,]>>();
            foreach (var configFrame in lineConfigs)
            {
                ExtractBlockNumbers(configFrame.ProcessedPixels, allBlocks);
            }
            CategorizeInitialBlocks(allBlocks);
            OCRBlockSignatures();

            foreach (var unrecSig in m_Signatures.Where(x => x.Digit == null))
            {
                var individChars = new List<int>();
                if (unrecSig.AllBlocks.Count > 10)
                {
                    foreach (var block in unrecSig.AllBlocks)
                    {
                        var ocrRes = OCRBlockSignatureDigit(block, unrecSig.m_BlockWidth, unrecSig.m_BlockHeight);
                        individChars.Add(ocrRes.Item1 ?? -1);
                    }
                    var dict = individChars.GroupBy(x => x).ToDictionary(x => x.Key, y => y.Count());
                    var arrKeys = dict.Keys.ToArray();
                    var arrVals = dict.Values.ToArray();
                    Array.Sort(arrVals, arrKeys);

                    if (arrVals[arrVals.Length - 1] > 0.66*unrecSig.AllBlocks.Count)
                    {
                        unrecSig.Digit = arrKeys[arrKeys.Length - 1];
                    }
                }
            }

            if (m_Signatures.Count(x => x.Digit == null) < m_Signatures.Count * 0.06)
            {
                var dictChars = m_Signatures.Where(x => x.Digit != null).GroupBy(x => x.Digit).ToDictionary(x => x.Key, y => y.ToList());
                IsCalibrated = dictChars.Keys.Min() == 0 && dictChars.Keys.Max() == 9 && dictChars.Keys.Count == 10;
                if (IsCalibrated)
                {
                    // Remove the potentially bad matches from the signatures
                    m_Signatures = m_Signatures.Where(x => x.Digit != null).ToList();
                }
            }
        }

        private void CategorizeInitialBlocks(List<Tuple<decimal[,], decimal[,]>> blocks)
        {
            var allBlocks = blocks.Select(x => x.Item1).Union(blocks.Select(x => x.Item2)).ToArray();
            m_BlockHeight = allBlocks[0].GetLength(0);
            m_BlockWidth = allBlocks[0].GetLength(1);

            var diffMatrixKeys = new List<Tuple<int, int>>();
            var diffMatrix = new List<decimal>();

            for (int i = 0; i < allBlocks.Length; i++)
            {
                for (int j = i + 1; j < allBlocks.Length; j++)
                {
                    var diff = ComputeBlockDifference(allBlocks[i], allBlocks[j]);

                    diffMatrixKeys.Add(Tuple.Create(i, j));
                    diffMatrix.Add(diff);
                }
            }

            var diffArr = diffMatrix.ToArray();
            var diffKeys = diffMatrixKeys.ToArray();
            Array.Sort(diffArr, diffKeys);

            var blockGroups = new List<List<int>>();
            var groupCounter = allBlocks.Length;
            for (int i = 0; i < diffKeys.Length; i++)
            {
                if (diffArr[i] > 10*255)
                {
                    break; /* max diff of 10 pixels*/
                }
                var key = diffKeys[i];
                var lists = blockGroups.Where(x => x.Contains(key.Item1) || x.Contains(key.Item2)).ToArray();
                var listsCount = lists.Count();
                List<int> list1;
                if (listsCount == 2)
                {
                    // Merge two lists
                    list1 = lists.First();
                    var l2 = lists.Skip(1).First();
                    list1.AddRange(l2);
                    blockGroups.Remove(l2);

                }
                else if (listsCount == 1)
                {
                    list1 = lists.First();
                }
                else if (listsCount == 0)
                {
                    list1 = new List<int>();
                    blockGroups.Add(list1);
                }
                else
                {
                    throw new ApplicationException();
                }

                if (!list1.Contains(key.Item1))
                {
                    list1.Add(key.Item1);
                    groupCounter--;
                }
                if (!list1.Contains(key.Item2))
                {
                    list1.Add(key.Item2);
                    groupCounter--;
                }

                if (groupCounter <= 0)
                    break;
            }

            for (int i = 0; i < blockGroups.Count; i++)
            {
                var sig = new BlockSignature(m_BlockWidth, m_BlockHeight);
                m_Signatures.Add(sig);
                for (int j = 0; j < blockGroups[i].Count; j++)
                {
                    sig.AllBlocks.Add(allBlocks[blockGroups[i][j]]);    
                }                
                sig.ComputeSignature();
            }
        }

        private void OCRBlockSignatures()
        {
            for (int i = 0; i < m_Signatures.Count; i++)
            {
                var sig = m_Signatures[i];
                var ocrRes = OCRBlockSignatureDigit(sig.Signature, sig.m_BlockWidth, sig.m_BlockHeight);
                sig.Digit = ocrRes.Item1;
                sig.HFactors = ocrRes.Item2;
                sig.VFactors = ocrRes.Item3;
            }
        }

        private static int OCR_WHITE_LEVEL = 64; // ?? We are mostly interested in somewhat white spaces

        private Tuple<int?, string, string> OCRBlockSignatureDigit(decimal[,] data, int width, int height)
        {
            char[] horizontals = new char[height];
            char[] verticals = new char[width];

            int[] midIndexes = width % 2 == 0
                ? new int[] { width / 2, (width / 2) - 1 }
                : new int[] { width / 2 };

            int hor80Percent = (int)Math.Round(width * 0.8);
            int hor60Percent = (int)Math.Round(width * 0.6);
            int hor40Percent = (int)Math.Round(width * 0.4);
            for (int y = 0; y < height; y++)
            {
                bool[] flags = new bool[width];
                for (int x = 0; x < width; x++)
                {
                    bool isWhite = data[y, x] >= OCR_WHITE_LEVEL; 
                    flags[x] = isWhite;
                }

                char ch = ' ';
                int numWhite = flags.Count(x => x);
                if (numWhite > hor80Percent)
                    ch = 'W';
                else if (flags.Length - numWhite > hor80Percent)
                    ch = 'B';
                else
                {
                    foreach (int idx in midIndexes)
                    {
                        if (!flags[idx])
                        {
                            // black in the middle. Check if surrounded by white
                            int numBlacks = 1;
                            bool whiteOnLeft = false;
                            bool whiteOnRight = false;
                            for (int i = idx - 1; i > 0; i--)
                            {
                                if (!flags[i]) numBlacks++;
                                else
                                {
                                    whiteOnLeft = true;
                                    break;
                                }
                            }
                            for (int i = idx + 1; i < width; i++)
                            {
                                if (!flags[i]) numBlacks++;
                                else
                                {
                                    whiteOnRight = true;
                                    break;
                                }
                            }

                            if (whiteOnLeft && whiteOnRight)
                            {
                                if (numBlacks > hor40Percent)
                                    ch = 'O'; //large black space in the middle surrounded by white on the sides
                                else
                                    ch = 'o'; //small black space in the middle surrounded by white on the sides
                            }
                            else if (numBlacks > hor40Percent)
                            {
                                if (whiteOnLeft)
                                    ch = '['; //large black space on the right and white on the left
                                else if (whiteOnRight)
                                    ch = ']'; //large black space on the left and white on the right
                            }
                        }
                    }

                    if (ch == ' ')
                    {
                        foreach (int idx in midIndexes)
                        {
                            if (flags[idx])
                            {
                                // White in the middle 
                                int numWhites = 1;
                                bool blackOnLeft = true;
                                bool blackOnRight = true;
                                for (int i = idx - 1; i > 0; i--)
                                {
                                    if (flags[i]) numWhites++;
                                    else
                                    {
                                        blackOnLeft = flags.Take(i).Count(x => x) == 0;
                                        break;
                                    }
                                }
                                for (int i = idx + 1; i < width; i++)
                                {
                                    if (flags[i]) numWhites++;
                                    else
                                    {
                                        blackOnRight = flags.Skip(i).Count(x => x) == 0;
                                        break;
                                    }
                                }
                                if (blackOnLeft && blackOnRight && numWhite < hor40Percent)
                                {
                                    ch = 'I'; //small white space in the middle surrounded by black on the sides
                                }
                            }
                        }
                    }

                    if (ch == ' ')
                    {
                        // Searching for [, ], W or I  starting from the ends and moving towards the center
                        // allowing for one extra black spot at the side of the white end
                        if (flags[0] || flags[1])
                        {
                            int idx = flags[0] ? 1 : 2;
                            bool lookingForBlack = true;
                            bool lookingForWhite = false;
                            bool failed = true;
                            int whiteBlocks = 0;
                            for (int i = idx; i < width - 1 /* allow the last char to be whatever */; i++)
                            {
                                if (lookingForBlack)
                                {
                                    if (!flags[i])
                                    {
                                        lookingForBlack = false;
                                        lookingForWhite = true;
                                        failed = false;
                                    }
                                    else
                                        whiteBlocks++;
                                }
                                else if (lookingForWhite && flags[i])
                                {
                                    failed = true;
                                    break;
                                }
                            }
                            if (!failed) ch = whiteBlocks <= hor40Percent ? '[' : 'W';
                        }

                        if (ch == ' ' && flags[width - 1] || flags[width - 2])
                        {
                            int idx = flags[width - 1] ? width - 2 : width - 3;
                            bool lookingForBlack = true;
                            bool lookingForWhite = false;
                            bool failed = true;
                            int whiteBlocks = 0;
                            for (int i = idx; i > 0 /* allow the first char to be whatever */; i--)
                            {
                                if (lookingForBlack)
                                {
                                    if (!flags[i])
                                    {
                                        lookingForBlack = false;
                                        lookingForWhite = true;
                                        failed = false;
                                    }
                                    else
                                        whiteBlocks++;
                                }
                                else if (lookingForWhite && flags[i])
                                {
                                    failed = true;
                                    break;
                                }
                            }
                            if (!failed) ch = whiteBlocks <= hor40Percent ? ']' : 'W'; 
                        }

                        if (ch == ' ' && (!flags[0] || !flags[1]) && (!flags[width - 1] || !flags[width - 2]))
                        {
                            int startIdx = flags[0] ? 1 : 2;
                            int endIdx = flags[width - 1] ? width - 2 : width - 3;
                            bool lookingForBlack = false;
                            bool lookingForWhite = true;
                            bool failed = true;
                            int whiteBlocks = 0;
                            for (int i = startIdx; i <= endIdx; i++)
                            {
                                if (lookingForWhite && flags[i])
                                {
                                    lookingForBlack = true;
                                    lookingForWhite = false;
                                    failed = false;
                                }
                                else if (lookingForBlack)
                                {
                                    if (flags[i])
                                        whiteBlocks++;
                                    else
                                    {
                                        failed = flags.Skip(i).Take(endIdx - i).Any(x => x);
                                        break;
                                    }
                                }
                            }
                            if (!failed) ch = whiteBlocks <= hor40Percent ? 'I' : 'W';
                        }
                    }

                    if (ch == ' ')
                    {
                        Trace.WriteLine(flags);
                    }
                }
                horizontals[y] = ch;
            }

            var hStr = new string(horizontals);
            var vStr = new string(verticals);
            var digit = OCRDigitBlock(hStr, vStr);

            return Tuple.Create(digit, hStr, vStr);
        }

        private static char[] ALL_BUT_O = " W[]IB".ToCharArray();
        private static Regex s_Regex9 = new Regex("^o+W+\\]+o*?$", RegexOptions.Compiled | RegexOptions.Singleline);
        private static Regex s_Regex6 = new Regex("^o*?\\[+W+o+$", RegexOptions.Compiled | RegexOptions.Singleline);
        private static Regex s_Regex3 = new Regex("^o*?\\]+[IW]*?\\]+o*?$", RegexOptions.Compiled | RegexOptions.Singleline);
        private static Regex s_Regex2 = new Regex("^o*?\\]+I+\\[*?$", RegexOptions.Compiled | RegexOptions.Singleline);
        private static Regex s_Regex8 = new Regex("^o+W+o+$", RegexOptions.Compiled | RegexOptions.Singleline);
        private Regex s_Regex0;
        private static Regex s_Regex5 = new Regex("^\\[+W+\\]+o*?$", RegexOptions.Compiled | RegexOptions.Singleline);

        private static Regex s_Regex1 = new Regex("^[I\\[\\]]+W*?$", RegexOptions.Compiled | RegexOptions.Singleline);
        private static Regex s_Regex7 = new Regex("^W+\\]+[I\\[\\]]+$", RegexOptions.Compiled | RegexOptions.Singleline);
        private static Regex s_Regex4 = new Regex("^[\\]I]+W*o+W+[\\]I]+$", RegexOptions.Compiled | RegexOptions.Singleline);

        private int? OCRDigitBlock(string horizontals, string verticals)
        {
            int hE = horizontals.Length - 1;
            int h60Percent = (int)Math.Round(horizontals.Length * 0.6);
            int v40Percent = (int)Math.Round(verticals.Length * 0.4);

            if (s_Regex0 == null)
                s_Regex0 = new Regex(string.Format("^o{{{0}}}o*?$", v40Percent), RegexOptions.Compiled | RegexOptions.Singleline);

            // Remove top or bottom fully black lines
            horizontals = horizontals.Trim('B');

            string trimmedI = horizontals.Trim(new char[] { 'I', ' ' });
            if (trimmedI.Length > 4 && trimmedI[0] == 'W' && trimmedI[trimmedI.Length - 1] == 'W')
            {
                var tIWrO = trimmedI.Trim('W').Replace('O', 'o');

                if (s_Regex0.IsMatch(tIWrO)) return 0;
                if (s_Regex8.IsMatch(tIWrO)) return 8;
                if (s_Regex5.IsMatch(tIWrO)) return 5;
                if (s_Regex9.IsMatch(tIWrO)) return 9;
                if (s_Regex6.IsMatch(tIWrO)) return 6;
                if (s_Regex3.IsMatch(tIWrO)) return 3;
                if (s_Regex2.IsMatch(tIWrO)) return 2;
            }

            if (s_Regex1.IsMatch(horizontals)) return 1;
            if (s_Regex7.IsMatch(horizontals)) return 7;
            if (s_Regex4.IsMatch(horizontals)) return 4;

            return null;
        }

        private static Font s_DebugFont = new Font(FontFamily.GenericSerif, 8);
        private static Font s_DebugFont2 = new Font(FontFamily.GenericSerif, 10);
 
        internal Bitmap GetBlockSignaturesImage()
        {
            Bitmap bmp = new Bitmap(m_Signatures.Count * 2 * m_BlockWidth, (3 + m_BlockHeight) * m_BlockHeight, PixelFormat.Format32bppArgb);

            for (int i = 0; i < m_Signatures.Count; i++)
            {
                var sig = m_Signatures[i];
                int x0 = (int)Math.Round((i + 0.5) * m_BlockWidth);
                int y0 = m_BlockHeight / 2;
                for (int y = 0; y < m_BlockHeight; y++)
                {
                    for (int x = 0; x < m_BlockWidth; x++)
                    {
                        byte pix = (byte)sig.Signature[y, x];
                        bmp.SetPixel(x + x0, y + y0, Color.FromArgb(pix, pix, pix));
                    }
                }
            }

            using (Graphics g = Graphics.FromImage(bmp))
            {
                for (int i = 0; i < m_Signatures.Count; i++)
                {
                    var sig = m_Signatures[i];
                    int x0 = (int) Math.Round((i + 0.5)*m_BlockWidth);
                    int y0 = (int)(1.5 * m_BlockHeight);

                    g.DrawString(Convert.ToString(sig.Digit), s_DebugFont2, Brushes.Lime, x0, y0);
                    for (int j = 0; j < sig.HFactors.Length; j++)
                    {
                        g.DrawString(sig.HFactors[j] + "", s_DebugFont, Brushes.Yellow, x0, y0 + (j + 1) * m_BlockHeight);    
                    }
                }

                g.Save();
            }

            return bmp;
        }

        private decimal ComputeBlockDifference(decimal[,] block1, decimal[,] block2)
        {
            decimal diff = 0;
            for (int y = 0; y < m_BlockHeight; y++)
            {
                for (int x = 0; x < m_BlockWidth; x++)
                {
                    diff += Math.Abs(block1[y, x] - block2[y, x]);
                }
            }
            return diff;
        }

        public bool IsCalibrated { get; private set; }

        private bool IsToLineConfiguration(List<OcrCalibrationFrame> lineConfigs, Tuple<int, int>[] topBottoms)
        {
            var twoLineSamples = lineConfigs.Select(x => x.DetectedOsdLines).Where(x => x != null && x.Length == 2);
            
            var line1Samples = twoLineSamples.SelectMany(x => x).Where(x => x.Top - topBottoms[0].Item1 < x.BoxHeight / 3).ToArray();
            var line2Samples = twoLineSamples.SelectMany(x => x).Where(x => x.Top - topBottoms[1].Item1 < x.BoxHeight / 3).ToArray();

            double line1Score = 0;
            for (int i = 0; i < TWOLINE_LINE1_INDEXES.Length; i++)
            {
                line1Score += line1Samples.Count(x => x.BoxIndexes.Contains(TWOLINE_LINE1_INDEXES[i]));
            }
            line1Score /= (TWOLINE_LINE1_INDEXES.Length*line1Samples.Length);

            double line1InvScore = 0;
            int total = 0;
            foreach (var cfg in line1Samples)
            {
                for (int i = 0; i < cfg.BoxIndexes.Length; i++)
                {
                    if (TWOLINE_LINE1_INDEXES.Contains(cfg.BoxIndexes[i])) line1InvScore ++;
                    total++;
                }
            }
            line1InvScore /= total;

            double line2Score = 0;
            for (int i = 0; i < TWOLINE_LINE2_INDEXES.Length; i++)
            {
                line2Score += line2Samples.Count(x => x.BoxIndexes.Contains(TWOLINE_LINE2_INDEXES[i]));
            }
            line2Score /= (TWOLINE_LINE2_INDEXES.Length*line2Samples.Length);

            double line2InvScore = 0;
            total = 0;
            foreach (var cfg in line2Samples)
            {
                for (int i = 0; i < cfg.BoxIndexes.Length; i++)
                {
                    if (TWOLINE_LINE2_INDEXES.Contains(cfg.BoxIndexes[i])) line2InvScore++;
                    total++;
                }
            }
            line2InvScore /= total;

            // The detected box positions should match at least 60% of the expected positions in order to accept this as recognized configuration
            return line1Score > 0.6 && line2Score > 0.6 && line1InvScore > 0.6 && line2InvScore > 0.6;
        }

        private void CreateTwoLineConfiguration(Tuple<int, int>[] topBottoms, Tuple<decimal, decimal>[] leftWidths)
        {
            m_Lines.Add(new GpsBoxSpriteLineOcr(topBottoms[0].Item1, topBottoms[0].Item2, leftWidths[0].Item1, leftWidths[0].Item2, TWOLINE_LINE1_INDEXES));
            m_Lines.Add(new GpsBoxSpriteLineOcr(topBottoms[1].Item1, topBottoms[1].Item2, leftWidths[1].Item1, leftWidths[1].Item2, TWOLINE_LINE2_INDEXES));
        }

        private void ExtractBlockNumbers(uint[] processedPixels, List<Tuple<decimal[,], decimal[,]>> allBlocks)
        {
            foreach (var line in m_Lines)
            {
                var lineBlocks = line.ExtractBlockNumbers(processedPixels, m_FrameWidth, m_FrameHeight);
                allBlocks.AddRange(lineBlocks);
            }
        }

        public void ProcessCalibrationFrame(OcrCalibrationFrame frame)
        {

        }

        public DateTime ExtractTime(int frameNo, int frameStep, uint[] processedPixels)
        {
            var oddFrameLines = new List<List<int?>>();
            var evenFrameLines = new List<List<int?>>();

            foreach (var line in m_Lines)
            {
                var oddDigits = new List<int?>();
                var evenDigits = new List<int?>();
                var lineBlocks = line.ExtractBlockNumbers(processedPixels, m_FrameWidth, m_FrameHeight);
                foreach (var block in lineBlocks)
                {
                    var oddDigit = OCRBlockDigit(block.Item1, line.BlockIntWidth, line.BlockIntHeight);
                    var evenDigit = OCRBlockDigit(block.Item1, line.BlockIntWidth, line.BlockIntHeight);

                    oddDigits.Add(oddDigit);
                    evenDigits.Add(evenDigit);
                }

                oddFrameLines.Add(oddDigits);
                evenFrameLines.Add(evenDigits);
            }

            if (m_Lines.Count == 2)
                return ExtractTwoLineLayoutTime(oddFrameLines, evenFrameLines);

            return DateTime.MinValue;
        }

        private DateTime ExtractTwoLineLayoutTime(List<List<int?>> oddLines, List<List<int?>> evenLines)
        {
            var dateTimeEven = DateTime.MinValue;
            var dateTimeOdd = DateTime.MinValue;

            // TODO: Make this fail-safe and working even when some digits haven't been recognized or have been recognized incorrectly

            if (oddLines[0].Count == 12 && evenLines[0].Count == 12 && oddLines[0].All(x => x.HasValue) && evenLines[0].All(x => x.HasValue))
            {
                dateTimeEven = new DateTime(evenLines[0][10].Value * 10 + evenLines[0][11].Value + 2000, evenLines[0][8].Value * 10 + evenLines[0][9].Value, evenLines[0][6].Value * 10 + evenLines[0][7].Value,
                                            evenLines[0][0].Value * 10 + evenLines[0][1].Value, evenLines[0][2].Value * 10 + evenLines[0][3].Value, evenLines[0][4].Value * 10 + evenLines[0][5].Value);
                dateTimeOdd = new DateTime(oddLines[0][10].Value * 10 + oddLines[0][11].Value + 2000, oddLines[0][8].Value * 10 + oddLines[0][9].Value, oddLines[0][6].Value * 10 + oddLines[0][7].Value,
                                            oddLines[0][0].Value * 10 + oddLines[0][1].Value, oddLines[0][2].Value * 10 + oddLines[0][3].Value, oddLines[0][4].Value * 10 + oddLines[0][5].Value);

                if (oddLines[1].Count == 10 && evenLines[1].Count == 10 && oddLines[1].All(x => x.HasValue) && evenLines[1].All(x => x.HasValue))
                {
                    // TODO: Add milliseconds
                }
            }

            return dateTimeEven;
        }

        private int? OCRBlockDigit(decimal[,] pixels, int width, int height)
        {
            // 1) Find the closest signature
            var diffs = m_Signatures.Select(x => Tuple.Create(ComputeBlockDifference(x.Signature, pixels), x.Digit));
            var sigDiffs = diffs.Select(x => x.Item1).ToArray();
            var sigDigits = diffs.Select(x => x.Item2).ToArray();
            Array.Sort(sigDiffs, sigDigits);
            var ocrDigitSig = sigDigits[0];
            var ocrDigitSig2 = sigDigits[1];
            var ocrDigitSig3 = sigDigits[2];

            // 2) OCR the single block directly
            var singleBlockOCR = OCRBlockSignatureDigit(pixels, width, height);
            var ocrDigit = singleBlockOCR.Item1;

            if (ocrDigitSig == ocrDigit)
                return ocrDigit;

            if (ocrDigitSig2 == ocrDigit)
                return ocrDigit;

            if (ocrDigitSig == ocrDigitSig2)
                return ocrDigitSig;

            if (ocrDigitSig == ocrDigitSig3)
                return ocrDigitSig;

            if (ocrDigitSig2 == ocrDigitSig3)
                return ocrDigitSig2;

            if (ocrDigit != null)
                return ocrDigit;

            return ocrDigitSig;
        }

        public void DrawLegend(Graphics graphics)
        {
            foreach (var line in m_Lines)
                line.DrawLegend(graphics);
        }
    }
}
