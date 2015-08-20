/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Helpers;
using Tangra.Model.Config;
using Tangra.Model.Helpers;
using Tangra.Model.Image;
using Tangra.Model.Video;
using Tangra.PInvoke;
using Tangra.Video.SER;

namespace Tangra.Video.SER
{
	public class SerEquipmentInfo
	{
		public string Observer;
		public string Instrument; 
		public string Telescope;
	}

	public enum SerUseTimeStamp
	{
		None,
		UtcTime,
		LocalTime
	}

	public class SERVideoStream : IFrameStream
	{
		public static SERVideoStream OpenFile(string fileName, IWin32Window parentForm, out SerEquipmentInfo equipmentInfo)
		{
			var fileInfo = new SerFileInfo();
			equipmentInfo = new SerEquipmentInfo();

			byte[] observer = new byte[40];
			byte[] instrument = new byte[40];
			byte[] telescope = new byte[40];

			TangraCore.SEROpenFile(fileName, ref fileInfo, observer, instrument, telescope, false);

			UsageStats.Instance.ProcessedSerFiles++;
			UsageStats.Instance.Save();

			var frmInfo = new frmEnterSERFileInfo(fileInfo);
			if (frmInfo.ShowDialog(parentForm) == DialogResult.OK)
			{
				TangraCore.SERCloseFile();

				var rv = new SERVideoStream(fileName, frmInfo.FrameRate, frmInfo.BitPix, frmInfo.UseEmbeddedTimeStamps);

				equipmentInfo.Instrument = rv.Instrument;
				equipmentInfo.Observer = rv.Observer;
				equipmentInfo.Telescope = rv.Telescope;

				return rv;
			}
			return null;
		}


		private SerFileInfo m_FileInfo;
		private string m_FileName;
		private SerUseTimeStamp m_UseTimeStamp;

		private SerFrameInfo m_CurrentFrameInfo;

		private SERVideoStream(string fileName, double frameRate, int cameraBitPix, SerUseTimeStamp useTimeStamp)
		{
			m_FileInfo = new SerFileInfo();

			byte[] observer = new byte[40];
			byte[] instrument = new byte[40];
			byte[] telescope = new byte[40];

			TangraCore.SEROpenFile(fileName, ref m_FileInfo, observer, instrument, telescope, false);

			m_FileName = fileName;

			BitPix = cameraBitPix;
			FrameRate = frameRate;
			MillisecondsPerFrame = 1000 / frameRate;
			m_UseTimeStamp = useTimeStamp;

			Observer = Encoding.UTF8.GetString(observer).Trim();
			Instrument = Encoding.UTF8.GetString(instrument).Trim();
			Telescope = Encoding.UTF8.GetString(telescope).Trim();

			if (useTimeStamp != SerUseTimeStamp.None)
			{
				HasTimeStamps =
					m_FileInfo.SequenceStartTimeHi != 0 &&
					m_FileInfo.SequenceStartTimeHi >> 0x1F == 0;

				HasUTCTimeStamps =
					m_FileInfo.SequenceStartTimeUTCHi != 0 &&
					m_FileInfo.SequenceStartTimeUTCHi >> 0x1F == 0;				
			}
			else
			{
				HasTimeStamps = false;
				HasUTCTimeStamps = false;
			}
		}

		public string Observer { get; private set; }

		public string Instrument { get; private set; }

		public string Telescope { get; private set; }

		public bool HasTimeStamps { get; private set; }

		public bool HasUTCTimeStamps { get; private set; }

		public DateTime SequenceStartTime
		{
			get { return new DateTime((long) m_FileInfo.SequenceStartTimeLo + ((long) m_FileInfo.SequenceStartTimeHi << 32)); }
		}

		public DateTime SequenceStartTimeUTC
		{
			get { return new DateTime((long)m_FileInfo.SequenceStartTimeUTCLo + ((long)m_FileInfo.SequenceStartTimeUTCHi << 32)); }
		}

		public int Width
		{
			get { return m_FileInfo.Width; }
		}

		public int Height
		{
			get { return m_FileInfo.Height; }
		}

		public int BitPix { get; private set; }

		public int FirstFrame
		{
			get { return 0; }
		}

		public int LastFrame
		{
			get { return m_FileInfo.CountFrames - 1; }
		}

		public int CountFrames
		{
			get { return m_FileInfo.CountFrames; }
		}

		public double FrameRate { get; private set; }

		public double MillisecondsPerFrame { get; private set; }


		public Pixelmap GetPixelmap(int index)
		{
			if (index < FirstFrame || index > LastFrame)
				throw new ApplicationException("Invalid frame position: " + index);

			uint[] pixels = new uint[Width * Height];
            uint[] unprocessedPixels = new uint[Width * Height];
			byte[] displayBitmapBytes = new byte[Width * Height];
			byte[] rawBitmapBytes = new byte[(Width * Height * 3) + 40 + 14 + 1];

			var frameInfo = new SerNativeFrameInfo();

            TangraCore.SERGetFrame(index, pixels, unprocessedPixels, rawBitmapBytes, displayBitmapBytes, (ushort)BitPix, ref frameInfo);

			m_CurrentFrameInfo = new SerFrameInfo(frameInfo);

			using (var memStr = new MemoryStream(rawBitmapBytes))
			{
				Bitmap displayBitmap;

				try
				{
					displayBitmap = (Bitmap)Bitmap.FromStream(memStr);
				}
				catch (Exception ex)
				{
					Trace.WriteLine(ex.GetFullStackTrace());
					displayBitmap = new Bitmap(Width, Height);
				}

				var rv = new Pixelmap(Width, Height, BitPix, pixels, displayBitmap, displayBitmapBytes);
			    rv.UnprocessedPixels = unprocessedPixels;
				rv.FrameState = new FrameStateData()
				{
					SystemTime = m_CurrentFrameInfo.TimeStamp
				};

				if (m_UseTimeStamp != SerUseTimeStamp.None)
					rv.FrameState.CentralExposureTime = m_UseTimeStamp == SerUseTimeStamp.UtcTime
						? m_CurrentFrameInfo.TimeStampUtc
						: m_CurrentFrameInfo.TimeStamp;

				return rv;
			}
		}

		public int RecommendedBufferSize
		{
            get { return Math.Min(8, CountFrames); }
		}

        public bool SupportsSoftwareIntegration
        {
            get { return true; }
        }

		public string VideoFileType
		{
			get { return string.Format("SER.{0}", m_FileInfo.PixelDepthPerPlane); }
		}

		public Pixelmap GetIntegratedFrame(int startFrameNo, int framesToIntegrate, bool isSlidingIntegration, bool isMedianAveraging)
		{
			if (startFrameNo < FirstFrame || startFrameNo > LastFrame)
				throw new ApplicationException("Invalid frame position: " + startFrameNo);

			int actualFramesToIntegrate = Math.Min(startFrameNo + framesToIntegrate, LastFrame - 1) - startFrameNo;

			uint[] pixels = new uint[Width * Height];
            uint[] unprocessedPixels = new uint[Width * Height];
			byte[] displayBitmapBytes = new byte[Width * Height];
			byte[] rawBitmapBytes = new byte[(Width * Height * 3) + 40 + 14 + 1];
			var frameInfo = new SerNativeFrameInfo();

            TangraCore.SERGetIntegratedFrame(startFrameNo, actualFramesToIntegrate, isSlidingIntegration, isMedianAveraging, pixels, unprocessedPixels, rawBitmapBytes, displayBitmapBytes, (ushort)BitPix, ref frameInfo);

			m_CurrentFrameInfo = new SerFrameInfo(frameInfo);

			using (var memStr = new MemoryStream(rawBitmapBytes))
			{
				Bitmap displayBitmap = (Bitmap)Bitmap.FromStream(memStr);

				var rv = new Pixelmap(Width, Height, BitPix, pixels, displayBitmap, displayBitmapBytes);
                rv.UnprocessedPixels = unprocessedPixels;
				rv.FrameState = new FrameStateData()
				{
					SystemTime = m_CurrentFrameInfo.TimeStamp
				};

				return rv;
			}
		}

		public string Engine
		{
			get { return "SER"; }
		}

		public string FileName
		{
			get { return m_FileName; }
		}

		public uint GetAav16NormVal()
		{
			return m_FileInfo.NormalisationValue;
		}

        public string GetFrameFileName(int index)
        {
            throw new NotSupportedException();
        }

        public bool SupportsFrameFileNames
        {
            get { return false; }
        }
	}
}
