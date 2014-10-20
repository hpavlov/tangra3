/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tangra.Model.Image;
using Tangra.OCR.IotaVtiOsdProcessor;

namespace Tangra.OCR.IotaVtiOsdProcessor
{
	internal class IotaVtiOcrProcessor
	{
		internal const int MAX_POSITIONS = 29;
		internal const int FIRST_FRAME_NO_DIGIT_POSITIONS = 22;
		internal const float FIELD_DURATION_PAL = 20.00f;
		internal const float FIELD_DURATION_NTSC = 16.68f;
		internal const float COEFF_FIRST_FRAME_NO_DIGIT_POSITION = 555.0f / 720.0f;

		public uint[] ZeroDigitPattern;
		public uint[] OneDigitPattern;
		public uint[] TwoDigitPattern;
		public uint[] ThreeDigitPattern;
		public uint[] FourDigitPattern;
		public uint[] FiveDigitPattern;
		public uint[] SixDigitPattern;
		public uint[] SevenDigitPattern;
		public uint[] EightDigitPattern;
		public uint[] NineDigitPattern;

		public uint[] SixEightXorPattern;
		public uint[] NineEightXorPattern;
		public uint[] ThreeEightXorPattern;
		public int SixEightXorPatternFactor;
		public int NineEightXorPatternFactor;
		public int ThreeEightXorPatternFactor;

		private IotaVtiOcrState m_CurrentSate;

		internal uint[] CurrentImage;

		public int CurrentImageWidth { get; private set; }

		public int CurrentImageHeight { get; private set; }

		public int BlockWidth { get; set; }

		public int BlockHeight { get; set; }

		public int BlockOffsetX { get; set; }

		public int[] BlockOffsetsX { get; set; }

		public int LastBlockOffsetsX { get; set; }

		public int SecondLastBlockOffsetsX { get; set; }

		public int BlockOffsetYOdd { get; set; }

		public int BlockOffsetYEven { get; set; }

		public int BlockOffsetY(bool isOddField)
		{
			return isOddField ? BlockOffsetYOdd : BlockOffsetYEven;
		}

		public string CurrentOcredString { get; set; }

		public IotaVtiTimeStamp CurrentOcredTimeStamp { get; set; }

		public int LastFrameNoDigitPosition { get; set; }

		public bool IsTvSafeMode { get; set; }

		public bool IsCalibrated
		{
			get { return m_CurrentSate is IotaVtiOcrCalibratedState; }
		}

		public IotaVtiOcrProcessor(bool isTVSafeMode)
		{
			ChangeState<IotaVtiOcrCalibratingState>();
			IsTvSafeMode = isTVSafeMode;
		}

		public void Process(uint[] pixels, int width, int height, Graphics g, int frameNo, bool isOddField)
		{
			CurrentImage = pixels;
			CurrentImageWidth = width;
			CurrentImageHeight = height;

			m_CurrentSate.Process(this, g, frameNo, isOddField);
		}

		internal void ChangeState<T>() where T : IotaVtiOcrState, new()
		{
			if (m_CurrentSate != null)
				m_CurrentSate.FinaliseState(this);

			m_CurrentSate = new T();
			m_CurrentSate.InitialiseState(this);
		}

		public uint[] GetBlockAt(int x0, int y0, bool isOddField)
		{
			return GetBlockAt(CurrentImage, x0, y0, isOddField);
		}

		public uint[] GetBlockAt(uint[] pixelmap, int x0, int y0, bool isOddField)
		{
			int blockOffsetY = isOddField ? BlockOffsetYOdd : BlockOffsetYEven;

			if (y0 >= blockOffsetY && y0 <= blockOffsetY + BlockHeight)
			{
				for (int i = 0; i < MAX_POSITIONS - 1; i++)
				{
					if (x0 >= BlockOffsetsX[i] && x0 < BlockOffsetsX[i + 1])
					{
						return GetBlockAtXOffset(pixelmap, BlockOffsetsX[i], isOddField);
					}
				}
			}

			return null;
		}

		public bool IsMatchingSignature(int nuberMarkedPixels)
		{
			return 10 * nuberMarkedPixels < BlockWidth * BlockHeight;
		}

		public uint[] GetBlockAtPosition(int positionIndex, bool isOddField)
		{
			return GetBlockAtPosition(CurrentImage, positionIndex, isOddField);
		}

		public uint[] GetBlockAtPosition(uint[] pixelmap, int positionIndex, bool isOddField)
		{
			return GetBlockAtXOffset(pixelmap, BlockOffsetsX[positionIndex], isOddField);
		}

		public uint[] GetBlockAtXOffset(uint[] pixelmap, int xOffset, bool isOddField)
		{
			uint[] blockPixels = new uint[BlockWidth * BlockHeight];
			int blockOffsetY = isOddField ? BlockOffsetYOdd : BlockOffsetYEven;
			int MAX_INDEX = pixelmap.Length - 1;
			for (int y = 0; y < BlockHeight; y++)
			{
				for (int x = 0; x < BlockWidth; x++)
				{
					int pixelMapIndex = xOffset + x + (blockOffsetY + y) * CurrentImageWidth;
					if (pixelMapIndex <= MAX_INDEX)
						blockPixels[x + y * BlockWidth] = pixelmap[pixelMapIndex];
				}
			}
			return blockPixels;
		}

		public bool SwapFieldsOrder { get; set; }

        public bool EvenBeforeOdd { get; set; }

		public VideoFormat? VideoFormat { get; set; }

		public void LearnDigitPattern(uint[] pattern, int digit)
		{
			if (digit == 0)
				ZeroDigitPattern = pattern;
			else if (digit == 1)
				OneDigitPattern = pattern;
			else if (digit == 2)
				TwoDigitPattern = pattern;
			else if (digit == 3)
				ThreeDigitPattern = pattern;
			else if (digit == 4)
				FourDigitPattern = pattern;
			else if (digit == 5)
				FiveDigitPattern = pattern;
			else if (digit == 6)
				SixDigitPattern = pattern;
			else if (digit == 7)
				SevenDigitPattern = pattern;
			else if (digit == 8)
				EightDigitPattern = pattern;
			else if (digit == 9)
				NineDigitPattern = pattern;
		}

		public void SetOcredString(IotaVtiTimeStampStrings ocredValue)
		{
			CurrentOcredString = string.Format("{0}|{1}:{2}:{3}|{4} {5}|{6}", ocredValue.NumSat, ocredValue.HH, ocredValue.MM, ocredValue.SS, ocredValue.FFFF1, ocredValue.FFFF2, ocredValue.FRAMENO);

			if (ocredValue.AllCharsPresent())
				CurrentOcredTimeStamp = new IotaVtiTimeStamp(ocredValue);
			else
				CurrentOcredTimeStamp = null;
		}
	}

}
