/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using Tangra.Model.Config;
using Tangra.Model.Image;
using Tangra.Model.Video;

namespace Tangra.Video
{
	internal class BMPFileSequenceStream : IDisposable, IFrameStream
	{
		public const string ENGINE = "BMP::SEQ";

		private List<string> m_BmpFilesList = new List<string>();

		public static BMPFileSequenceStream OpenFolder(string[] fitsFiles)
		{
			var rv = new BMPFileSequenceStream(fitsFiles);
			rv.FileName = Path.GetDirectoryName(fitsFiles[0]);
			return rv;
		}

		private BMPFileSequenceStream(string[] fitsFiles)
        {
			m_BmpFilesList.AddRange(fitsFiles);
			m_BmpFilesList.Sort();

            FirstFrame = 0;
			LastFrame = m_BmpFilesList.Count - 1;
			CountFrames = m_BmpFilesList.Count;

			Bitmap bmp = (Bitmap)Bitmap.FromFile(m_BmpFilesList[0]);

			Width = bmp.Width;
			Height = bmp.Height;
            BitPix = 8;

	        HasUTCTimeStamps = false;

			VideoFileType = ENGINE;
        }

		public int Width { get; private set; }

		public int Height { get; private set; }

		public int BitPix { get; private set; }

		public int FirstFrame { get; private set; }

		public int LastFrame { get; private set; }

		public int CountFrames { get; private set; }

		public double FrameRate
		{
			get { return double.NaN; }
		}

		public double MillisecondsPerFrame
		{
			get { return 0; }
		}

		public Pixelmap GetPixelmap(int index)
		{
			Bitmap bmp = (Bitmap)Bitmap.FromFile(m_BmpFilesList[index]);
			return Pixelmap.ConstructFromBitmap(bmp, TangraConfig.ColourChannel.Red);
		}

		public int RecommendedBufferSize
		{
            get { return Math.Min(8, CountFrames); }
		}

		public bool SupportsSoftwareIntegration
		{
			get { return false; }
		}

		public string VideoFileType { get; private set; }

		public Pixelmap GetIntegratedFrame(int startFrameNo, int framesToIntegrate, bool isSlidingIntegration, bool isMedianAveraging)
		{
			throw new NotImplementedException();
		}

		public string Engine
		{
			get { return ENGINE; }
		}

		public string FileName { get; private set; }

		public uint GetAav16NormVal()
		{
			return 0;
		}

        public string GetFrameFileName(int index)
        {
            if (index >= 0 && index < m_BmpFilesList.Count)
                return Path.GetFileName(m_BmpFilesList[index]);

            return null;
        }

        public bool SupportsFrameFileNames
        {
            get { return true; }
        }

		public void Dispose()
		{

		}

		public bool HasUTCTimeStamps { get; private set; }
	}
}
