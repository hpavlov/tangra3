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
using Tangra.Model.Config;
using Tangra.Model.Helpers;
using Tangra.Model.Image;
using Tangra.OCR.IotaVtiOsdProcessor;
using Tangra.OCR.TimeExtraction;

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
        private List<OcrCalibrationFrame> m_CalibrationFrames = new List<OcrCalibrationFrame>();
        private List<Tuple<decimal[,], decimal[,]>> m_CalibrationBlocks = new List<Tuple<decimal[,], decimal[,]>>();

        private int m_FrameWidth;
        private int m_FrameHeight;

        private int m_BlockWidth;
        private int m_BlockHeight;

        static int[] TWOLINE_LINE1_INDEXES = new int[] { 0, 1, 3, 4, 6, 7, 9, 10, 12, 13, 15, 16 };
        static int[] TWOLINE_LINE2_INDEXES = new int[] { 0, 1, 2, 3, 5, 9, 10, 11, 12, 14 };

        static int MAX_UNRECOGNIZED_SIGNATURES_PERCENT = 8;
        static int MIN_RECOGNIZED_DIGITS_ZERO_TO_NINE = 6;

        private double m_UnrecognSignPerc = double.NaN;

        public GpsBoxSpriteOcrProcessor(List<OcrCalibrationFrame> calibrationFrames, Tuple<int, int>[] topBottoms, Tuple<decimal, decimal>[] leftWidths, int frameWidth, int frameHeight)
        {
            m_FrameWidth = frameWidth;
            m_FrameHeight = frameHeight;

            if (topBottoms.Length == 2)
            {
                if (IsToLineConfiguration(calibrationFrames, topBottoms))
                    CreateTwoLineConfiguration(topBottoms, leftWidths);
            }
            else
            {
                return;
            }

            m_CalibrationFrames.AddRange(calibrationFrames);

            foreach (var configFrame in calibrationFrames)
                ExtractBlockNumbers(configFrame.ProcessedPixels);


            m_Signatures.Clear();
            var allBlocks = m_CalibrationBlocks.Select(x => x.Item1).Union(m_CalibrationBlocks.Select(x => x.Item2)).ToList();
            CategorizeInitialCalibrationBlocks(allBlocks);
            AttemptCalibration();
        }

        private void AttemptCalibration()
        {
            OCRBlockSignatures();            
            CheckSignaturesAndCompleteCalibrationIfPossible();
        }

        private void CheckSignaturesAndCompleteCalibrationIfPossible()
        {
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

                    if (arrVals[arrVals.Length - 1] > 0.66 * unrecSig.AllBlocks.Count)
                    {
                        unrecSig.Digit = arrKeys[arrKeys.Length - 1];
                    }
                }
            }

            m_UnrecognSignPerc = m_Signatures.Count(x => x.Digit == null) * 100.0 / m_Signatures.Count;
            if (m_UnrecognSignPerc < MAX_UNRECOGNIZED_SIGNATURES_PERCENT)
            {
                var dictChars = m_Signatures.Where(x => x.Digit != null).GroupBy(x => x.Digit).ToDictionary(x => x.Key, y => y.ToList());
                var sigLocated94Percent = dictChars.Keys.Min() >= 0 && dictChars.Keys.Count >= MIN_RECOGNIZED_DIGITS_ZERO_TO_NINE;
                if (sigLocated94Percent)
                {
                    // Remove the potentially bad matches from the signatures
                    m_Signatures = m_Signatures.Where(x => x.Digit != null).ToList();

                    if (CompleteOcrCalibration())
                    {
                        IsCalibrated = true;
                    }
                }
                else
                {
                    //GetBlockSignaturesImage().Save(@"C:\Work\gbs-debug.bmp");
                }
            }
        }

        private bool CompleteOcrCalibration()
        {
            var allOCRedTimeStamps = new List<Tuple<GpxBoxSpriteVtiTimeStamp, GpxBoxSpriteVtiTimeStamp>>();
            foreach (var frame in m_CalibrationFrames)
            {
                Process(frame.ProcessedPixels, -1, false);
                allOCRedTimeStamps.Add(Tuple.Create(CurrentOcredOddFieldTimeStamp.Clone(), CurrentOcredEvenFieldTimeStamp.Clone()));
            }

            #region Determine Field Duration

            var oddFieldDur = allOCRedTimeStamps.Select(x => Math.Abs(x.Item1.Milliseconds10First - x.Item1.Milliseconds10Second)).ToList();
            var evenFieldDur = allOCRedTimeStamps.Select(x => Math.Abs(x.Item2.Milliseconds10First - x.Item2.Milliseconds10Second)).ToList();
            var allFieldDur = new List<int>();
            allFieldDur.AddRange(oddFieldDur);
            allFieldDur.AddRange(evenFieldDur);

            var dict = allFieldDur.ToLookup(x => x)
                        .ToDictionary(x => x.Key, y => y.Count());

            double fieldDurationMS = 0;
            if (dict.Count == 1)
            {
                fieldDurationMS = dict.Keys.First() / 10.0;
            }
            else
            {
                var arrKeys = dict.Select(x => x.Key).ToArray();
                var arrVals = dict.Select(x => x.Value).ToArray();
                Array.Sort(arrVals, arrKeys);
                fieldDurationMS = arrKeys[arrKeys.Length - 1] / 10.0;
            }

            if (Math.Abs(Math.Abs(fieldDurationMS) - VtiTimeStampComposer.FIELD_DURATION_PAL) < 0.15)
                VideoFormat = TimeExtraction.VideoFormat.PAL;
            else if (Math.Abs(Math.Abs(fieldDurationMS) - VtiTimeStampComposer.FIELD_DURATION_NTSC) < 0.15)
                VideoFormat = TimeExtraction.VideoFormat.NTSC;
            else
                VideoFormat = null;
            #endregion;

            var lstSecMinFirst =
                allOCRedTimeStamps.Select(
                    x =>
                        Math.Max(x.Item2.Milliseconds10First, x.Item2.Milliseconds10Second) -
                        Math.Max(x.Item1.Milliseconds10First, x.Item1.Milliseconds10Second)).ToList();

            int cntNegative = lstSecMinFirst.Count(x => x < 0);
            int cntPositive = lstSecMinFirst.Count(x => x > 0);
            int cntZeroes = lstSecMinFirst.Count(x => x == 0);

            if (cntZeroes > cntNegative && cntZeroes > cntPositive)
            {
                var numSameTS = allOCRedTimeStamps.Count(x => x.Item1.Milliseconds10First == x.Item2.Milliseconds10First);
                if (numSameTS > allOCRedTimeStamps.Count/2)
                {
                    // Duplicated fields
                    DuplicatedFields = true;
                }
            }
            else if (cntNegative > cntZeroes && cntNegative > cntPositive)
            {
                EvenBeforeOdd = true;
            }

            return true;
        }

        internal VideoFormat? VideoFormat { get; private set; }
        
        internal bool EvenBeforeOdd { get; private set; }

        internal bool DuplicatedFields { get; private set; }

        internal uint[] CurrentImage;

        private void CategorizeInitialCalibrationBlocks(List<decimal[,]> allBlocksList)
        {
            var allBlocks = allBlocksList.ToArray();
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

        private void CategorizeCalibrationBlocks(List<Tuple<decimal[,], decimal[,]>> blocks)
        {
            var allBlocks = blocks.Select(x => x.Item1).Union(m_CalibrationBlocks.Select(x => x.Item2)).ToArray();
            m_BlockHeight = allBlocks[0].GetLength(0);
            m_BlockWidth = allBlocks[0].GetLength(1);

            var diffMatrixKeys = new List<Tuple<int, int>>();
            var diffMatrix = new List<decimal>();

            for (int i = 0; i < allBlocks.Length; i++)
            {
                for (int j = 0; j < m_Signatures.Count; j++)
                {
                    var diff = ComputeBlockDifference(allBlocks[i], m_Signatures[j].Signature);

                    diffMatrixKeys.Add(Tuple.Create(i, j));
                    diffMatrix.Add(diff);
                }
            }

            var diffArr = diffMatrix.ToArray();
            var diffKeys = diffMatrixKeys.ToArray();
            Array.Sort(diffArr, diffKeys);

            var unmatchedToSigIndexes = new List<int>();
            var matchedToSigIndexes = new List<int>();
            var sigIndexesToRecalc = new List<int>();
            for (int i = 0; i < diffKeys.Length; i++)
            {
                var key = diffKeys[i];

                if (diffArr[i] > 10 * 255)
                {
                    if (!unmatchedToSigIndexes.Contains(key.Item1))
                        unmatchedToSigIndexes.Add(key.Item1);
                    break; /* max diff of 10 pixels*/
                }

                if (!matchedToSigIndexes.Contains(key.Item1))
                    matchedToSigIndexes.Add(key.Item1);

                m_Signatures[key.Item2].AllBlocks.Add(allBlocks[key.Item1]);
                if (!sigIndexesToRecalc.Contains(key.Item2)) sigIndexesToRecalc.Add(key.Item2);
            }

            foreach(var sigIndex in sigIndexesToRecalc)
                m_Signatures[sigIndex].ComputeSignature();

            var unmatched = unmatchedToSigIndexes.Where(x => !matchedToSigIndexes.Contains(x)).ToList();
            if (unmatched.Count > 0)
            {
                var remaining = new List<decimal[,]>();
                foreach (var idx in unmatched)
                    remaining.Add(allBlocks[idx]);

                CategorizeInitialCalibrationBlocks(remaining);
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
                            for (int i = idx - 1; i > 0 + 1 /* need to me more than 1 pixel */; i--)
                            {
                                if (!flags[i]) numBlacks++;
                                else
                                {
                                    whiteOnLeft = true;
                                    break;
                                }
                            }
                            for (int i = idx + 1; i < width - 1 /* need to me more than 1 pixel */; i++)
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
                        var wbFlags = string.Join("", flags.Select(x => x ? 'w' : 'b'));
                        var trimmedBs = wbFlags.Trim('b');
                        if (trimmedBs.Length >= hor80Percent &&
                            trimmedBs.Trim('w').Length > 0 &&
                            trimmedBs.Trim('w').Trim('b').Length == 0)
                        {
                            ch = 'o';
                        }
                    }

                    if (ch == ' ')
                    {
                        Trace.WriteLine(string.Format("Cannot match fill pattern: {0}", string.Join("", flags.Select(x => x ? 'w' : 'b'))));
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

        private static Regex s_Regex1 = new Regex("^[I\\[\\]]+[IW]*?$", RegexOptions.Compiled | RegexOptions.Singleline);
        private static Regex s_Regex7 = new Regex("^W+o*?\\]+[I\\[\\]]+$", RegexOptions.Compiled | RegexOptions.Singleline);
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

        internal void PrepareCalibrationFailedReport(ICalibrationErrorProcessor errorProcessor)
        {
            errorProcessor.AddErrorText(string.Format("{0:0.00}% of all signatures have been recognized.", 100 - m_UnrecognSignPerc));
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

        private List<Tuple<decimal[,], decimal[,]>> ExtractBlockNumbers(uint[] processedPixels)
        {
            var rv = new List<Tuple<decimal[,], decimal[,]>>();

            foreach (var line in m_Lines)
            {
                var lineBlocks = line.ExtractBlockNumbers(processedPixels, m_FrameWidth, m_FrameHeight);
                m_CalibrationBlocks.AddRange(lineBlocks);
                rv.AddRange(lineBlocks);
            }

            return rv;
        }

        public void ProcessCalibrationFrame(OcrCalibrationFrame calibFrame)
        {
            m_CalibrationFrames.Add(calibFrame);

            var newBlocks = ExtractBlockNumbers(calibFrame.ProcessedPixels);

            CategorizeCalibrationBlocks(newBlocks);
            AttemptCalibration();
        }

        internal GpxBoxSpriteVtiTimeStamp CurrentOcredOddFieldTimeStamp { get; private set; }

        internal GpxBoxSpriteVtiTimeStamp CurrentOcredEvenFieldTimeStamp { get; private set; }

        private Bitmap _debugBitmap;

        internal Bitmap GetDebugBitmap()
        {
            return _debugBitmap;
        }

        public void Process(uint[] processedPixels, int frameNo, bool debug)
        {
            CurrentImage = processedPixels;

            var oddFrameLines = new List<List<int?>>();
            var evenFrameLines = new List<List<int?>>();

            _debugBitmap = null;
            Graphics g = null;
            int lineNo = 0;

            var lineBlocks = m_Lines.Select(line => line.ExtractBlockNumbers(processedPixels, m_FrameWidth, m_FrameHeight)).ToArray();

            for (int i = 0; i < m_Lines.Count; i++)
            {
                var line = m_Lines[i];
                var oddDigits = new List<int?>();
                var evenDigits = new List<int?>();
                

                int blockNo = 0;
                foreach (var block in lineBlocks[i])
                {
                    if (debug && _debugBitmap == null)
                    {
                        var maxWidthInBlocks = lineBlocks.Max(x => x.Count);
                        _debugBitmap = new Bitmap(line.BlockIntWidth * maxWidthInBlocks, 2 * 2 * m_Lines.Count * line.BlockIntHeight, PixelFormat.Format24bppRgb);
                        g = Graphics.FromImage(_debugBitmap);
                    }

                    var digit1 = OCRBlockDigit(block.Item1, line.BlockIntWidth, line.BlockIntHeight);
                    var digit2 = OCRBlockDigit(block.Item2, line.BlockIntWidth, line.BlockIntHeight);

                    // Note: EvenBeforeOdd is used later to determine the correct order
                    oddDigits.Add(digit1);
                    evenDigits.Add(digit2);

                    if (debug)
                    {
                        AddOcrDebugDigit(g, block.Item1, digit1, lineNo, blockNo, 0);
                        AddOcrDebugDigit(g, block.Item2, digit2, lineNo, blockNo, 1);
                    }

                    blockNo++;
                }

                lineNo++;

                oddFrameLines.Add(oddDigits);
                evenFrameLines.Add(evenDigits);
            }

            if (debug)
                g.Save();

            if (m_Lines.Count == 2)
                ExtractTwoLineLayoutTime(oddFrameLines, evenFrameLines);
        }

        private static Pen[] s_Pens = Enumerable.Range(0, 256).Select(x => new Pen(Color.FromArgb(255, x, x, x))).ToArray();
        private static Font s_Font = new Font(FontFamily.GenericMonospace, 8);

        private void AddOcrDebugDigit(Graphics g, decimal[,] block, int? ocredDigit, int lineNo, int blockNo, int fieldNo)
        {
            try
            {
                int x0 = blockNo * m_BlockWidth;
                int y0 = (4 * lineNo + 2 * fieldNo) * m_BlockHeight;

                for (int y = 0; y < block.GetLength(0); y++)
                {
                    for (int x = 0; x < block.GetLength(1); x++)
                    {
                        byte pixel = (byte)block[y, x];
                        g.DrawLine(s_Pens[pixel], x0 + x, y0 + y, x0 + x + 1, y0 + y);
                    }
                }

                g.DrawString(ocredDigit != -1 ? ocredDigit.ToString() : " ", s_Font, Brushes.Lime, x0, y0 + m_BlockHeight);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.GetFullStackTrace());
            }
        }

        private int GetTwoDigitInteger(int? tens, int? ones)
        {
            return (tens ?? 0) * 10 + (ones ?? 0);
        }

        private void ExtractTwoLineLayoutTime(List<List<int?>> oddLines, List<List<int?>> evenLines)
        {
            if (oddLines[0].Count == 12 && evenLines[0].Count == 12 &&
                oddLines[1].Count == 10 && evenLines[1].Count == 10)
            {
                int yearOdd = GetTwoDigitInteger(oddLines[0][10], oddLines[0][11]) + 2000;
                int monthOdd = GetTwoDigitInteger(oddLines[0][8], oddLines[0][9]);
                int dayOdd = GetTwoDigitInteger(oddLines[0][6], oddLines[0][7]);
                int hourOdd = GetTwoDigitInteger(oddLines[0][0], oddLines[0][1]);
                int minOdd = GetTwoDigitInteger(oddLines[0][2], oddLines[0][3]);
                int secOdd = GetTwoDigitInteger(oddLines[0][4], oddLines[0][5]);
                int ms10Odd1 = (GetTwoDigitInteger(oddLines[1][0], oddLines[1][1]) * 100 + GetTwoDigitInteger(oddLines[1][2], oddLines[1][3])) * 10 + (oddLines[1][4] ?? 0);
                int ms10Odd2 = (GetTwoDigitInteger(oddLines[1][5], oddLines[1][6]) * 100 + GetTwoDigitInteger(oddLines[1][7], oddLines[1][8])) * 10 + (oddLines[1][9] ?? 0);

                string oddOcredChars =
                    string.Format("{0}{1}:{2}{3}:{4}{5} {6}{7}/{8}{9}/{10}{11} {12}{13}{14}{15}.{16} {17}{18}{19}{20}.{21}",
                        oddLines[0][0].Char(), oddLines[0][1].Char(), oddLines[0][2].Char(), oddLines[0][3].Char(), oddLines[0][4].Char(), oddLines[0][5].Char(),
                        oddLines[0][6].Char(), oddLines[0][7].Char(), oddLines[0][8].Char(), oddLines[0][9].Char(), oddLines[0][10].Char(), oddLines[0][11].Char(),
                        oddLines[1][0].Char(),oddLines[1][1].Char(),oddLines[1][2].Char(),oddLines[1][3].Char(),oddLines[1][4].Char(),
                        oddLines[1][5].Char(),oddLines[1][6].Char(),oddLines[1][7].Char(),oddLines[1][8].Char(),oddLines[1][9].Char());

                int yearEven = GetTwoDigitInteger(evenLines[0][10], evenLines[0][11]) + 2000;
                int monthEven = GetTwoDigitInteger(evenLines[0][8], evenLines[0][9]);
                int dayEven = GetTwoDigitInteger(evenLines[0][6], evenLines[0][7]);
                int hourEven = GetTwoDigitInteger(evenLines[0][0], evenLines[0][1]);
                int minEven = GetTwoDigitInteger(evenLines[0][2], evenLines[0][3]);
                int secEven = GetTwoDigitInteger(evenLines[0][4], evenLines[0][5]);
                int ms10Even1 = (GetTwoDigitInteger(evenLines[1][0], evenLines[1][1]) * 100 + GetTwoDigitInteger(evenLines[1][2], evenLines[1][3])) * 10 + (evenLines[1][4] ?? 0);
                int ms10Even2 = (GetTwoDigitInteger(evenLines[1][5], evenLines[1][6]) * 100 + GetTwoDigitInteger(evenLines[1][7], evenLines[1][8])) * 10 + (evenLines[1][9] ?? 0);

                string evenOcredChars =
                    string.Format("{0}{1}:{2}{3}:{4}{5} {6}{7}/{8}{9}/{10}{11} {12}{13}{14}{15}.{16} {17}{18}{19}{20}.{21}",
                        evenLines[0][0].Char(), evenLines[0][1].Char(), evenLines[0][2].Char(), evenLines[0][3].Char(), evenLines[0][4].Char(), evenLines[0][5].Char(),
                        evenLines[0][6].Char(), evenLines[0][7].Char(), evenLines[0][8].Char(), evenLines[0][9].Char(), evenLines[0][10].Char(), evenLines[0][11].Char(),
                        evenLines[1][0].Char(), evenLines[1][1].Char(), evenLines[1][2].Char(), evenLines[1][3].Char(), evenLines[1][4].Char(),
                        evenLines[1][5].Char(), evenLines[1][6].Char(), evenLines[1][7].Char(), evenLines[1][8].Char(), evenLines[1][9].Char());

                CurrentOcredOddFieldTimeStamp = new GpxBoxSpriteVtiTimeStamp(yearOdd, monthOdd, dayOdd, hourOdd, minOdd, secOdd, ms10Odd1, ms10Odd2, oddOcredChars, !EvenBeforeOdd, DuplicatedFields);
                CurrentOcredEvenFieldTimeStamp = new GpxBoxSpriteVtiTimeStamp(yearEven, monthEven, dayEven, hourEven, minEven, secEven, ms10Even1, ms10Even2, evenOcredChars, EvenBeforeOdd, DuplicatedFields);
            }
        }

        private int? OCRBlockDigit(decimal[,] pixels, int width, int height)
        {
            // 1) OCR the single block directly
            var singleBlockOCR = OCRBlockSignatureDigit(pixels, width, height);
            var ocrDigit = singleBlockOCR.Item1;

            if (ocrDigit != null)
                return ocrDigit;

            // 2) Find the closest signature
            var diffs = m_Signatures.Select(x => Tuple.Create(ComputeBlockDifference(x.Signature, pixels), x.Digit));
            var sigDiffs = diffs.Select(x => x.Item1).ToArray();
            var sigDigits = diffs.Select(x => x.Item2).ToArray();
            Array.Sort(sigDiffs, sigDigits);
            var ocrDigitSig = sigDigits[0];
            var ocrDigitSig2 = sigDigits[1];
            var ocrDigitSig3 = sigDigits[2];

            return ocrDigitSig;
        }

        public void DrawLegend(Graphics graphics)
        {
            foreach (var line in m_Lines)
                line.DrawLegend(graphics);
        }
    }

    public static class DigitExtensions
    {
        public static char Char(this int? digit)
        {
            if (digit.HasValue)
                return digit.ToString().Last();
            else
                return ' ';
        }
    }

}
