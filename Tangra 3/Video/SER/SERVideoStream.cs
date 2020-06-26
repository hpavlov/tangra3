/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Tangra.Controller;
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
		SerEmbeddedUtcTime,
        FireCaptureLog
	}

    public enum SerTimeStampReference
    {
        StartFrame,
        MidFrame,
        EndFrame
    }

    public class SERVideoStream : IFrameStream, IDynamicRangeStream
	{
        private uint? m_MinPixelValueFirstImage;
        
        private uint? m_MaxPixelValueFirstImage;

        public static SERVideoStream OpenFile(string fileName, IWin32Window parentForm, TangraOpenFileArgs args, out SerEquipmentInfo equipmentInfo)
		{
			var fileInfo = new SerFileInfo();
			equipmentInfo = new SerEquipmentInfo();

			byte[] observer = new byte[40];
			byte[] instrument = new byte[40];
			byte[] telescope = new byte[40];

			TangraCore.SEROpenFile(fileName, ref fileInfo, observer, instrument, telescope, false, false);

            if (fileInfo.CountFrames == 0)
            {
                MessageBox.Show(parentForm, "This file has no video frames in it.", "Error opening SER file", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

		    string fireCaptureLogFileName = Path.ChangeExtension(fileName, ".txt");
		    var fireCaptureTimeStamps = new Dictionary<int, DateTime>();
		    if (File.Exists(fireCaptureLogFileName))
		    {
                string[] fireCaptureLogLines = File.ReadAllLines(fireCaptureLogFileName);
                fireCaptureLogLines = fireCaptureLogLines.Where(x => x != null && x.StartsWith("Frame ")).ToArray();
		        if (fireCaptureLogLines.Any())
		        {
		            // Parse FireCapture timestamps
		            Regex timestampRegEx = new Regex(@"Frame (\d+):\s+(UT)?\s+(\d\d\d\d\d\d)\s+(\d\d\d\d\d\d)\.(\d+)");
		            foreach (string line in fireCaptureLogLines)
		            {
		                Match match = timestampRegEx.Match(line);
		                if (match.Success)
		                {
		                    string frameNo = match.Groups[1].Value;
                            bool isUT = match.Groups[2].Value == "UT";
                            string ddmmyy = match.Groups[isUT ? 3 : 2].Value;
                            string hhmmss = match.Groups[isUT ? 4 : 3].Value;
                            double fffff = double.Parse("0." + match.Groups[isUT ? 5 : 4].Value, CultureInfo.InvariantCulture);

		                    DateTime dt = new DateTime(
		                        2000 + int.Parse(ddmmyy.Substring(4, 2)),
		                        int.Parse(ddmmyy.Substring(2, 2)),
		                        int.Parse(ddmmyy.Substring(0, 2)),
		                        int.Parse(hhmmss.Substring(0, 2)),
		                        int.Parse(hhmmss.Substring(2, 2)),
		                        int.Parse(hhmmss.Substring(4, 2))).AddMilliseconds(fffff * 1000);

		                    fireCaptureTimeStamps.Add(int.Parse(frameNo), dt);
		                }
		            }
		        }
		    }

			UsageStats.Instance.ProcessedSerFiles++;
			UsageStats.Instance.Save();

            int bitPix;
            double frameRate;
            SerUseTimeStamp serTiming;
            bool grayScaleRGB;
            if (args != null)
            {
                bitPix = args.BitPix;
                frameRate = args.FrameRate;
                serTiming = args.SerTiming;
                grayScaleRGB = args.GrayScaleRGB;
            }
            else
            {
                var frmInfo = new frmEnterSERFileInfo(fileInfo, fireCaptureTimeStamps.Count > 0);
                if (frmInfo.ShowDialog(parentForm) != DialogResult.OK)
                {
                    return null;
                }
                frameRate = frmInfo.FrameRate;
                bitPix = frmInfo.BitPix;
                serTiming = frmInfo.UseEmbeddedTimeStamps;
                grayScaleRGB = frmInfo.UseGrayScaleRGB;
            }

            TangraCore.SERCloseFile();

            var rv = new SERVideoStream(fileName, frameRate, bitPix, grayScaleRGB, serTiming, fireCaptureTimeStamps);

            equipmentInfo.Instrument = rv.Instrument;
            equipmentInfo.Observer = rv.Observer;
            equipmentInfo.Telescope = rv.Telescope;

            if (args == null)
            {
                // Only check timestamp integrity if not opened together with existing .lc file
                if (rv.HasTimeStamps || rv.HasUTCTimeStamps || rv.HasFireCaptureTimeStamps)
                {
                    var frmCheckTS = new frmCheckTimeStampsIntegrity(rv);
                    frmCheckTS.ShowDialog(parentForm);

                    if (rv.HasTimeStamps || rv.HasUTCTimeStamps)
                    {
                        var frmTsExp = new frmSerTimestampExposure(frmCheckTS.MedianExposureMs, frmCheckTS.OneSigmaExposureMs, frmCheckTS.DroppedFrames, frmCheckTS.DroppedFramesPercentage, frmCheckTS.HasTooManyDroppedFrames);
                        frmTsExp.ShowDialog(parentForm);

                        var derivedFrameRate = 1000.0 / frmTsExp.ConfirmedExposure;
                        rv = new SERVideoStream(fileName, derivedFrameRate, bitPix, grayScaleRGB, serTiming, fireCaptureTimeStamps, frmTsExp.ConfirmedTimeReference);
                    }
                }
            }
            else if (rv.HasTimeStamps || rv.HasUTCTimeStamps)
            {
                rv = new SERVideoStream(fileName, args.FrameRate, bitPix, grayScaleRGB, serTiming, fireCaptureTimeStamps, args.ConfirmedTimeReference);
            }

            return rv;
		}


		private SerFileInfo m_FileInfo;
		private string m_FileName;
	    internal SerUseTimeStamp UseTimeStamp { get; private set; }
        internal SerTimeStampReference TimeStampReference { get; private set; }

		private SerFrameInfo m_CurrentFrameInfo;
	    private Dictionary<int, DateTime> m_FireCaptureTimeStamps;

        private SERVideoStream(string fileName, double frameRate, int cameraBitPix, bool grayScaleRGB, SerUseTimeStamp useTimeStamp, Dictionary<int, DateTime> fireCaptureTimeStamps, SerTimeStampReference timeStampReference = SerTimeStampReference.MidFrame)
		{
			m_FileInfo = new SerFileInfo();

			byte[] observer = new byte[40];
			byte[] instrument = new byte[40];
			byte[] telescope = new byte[40];

            TangraCore.SEROpenFile(fileName, ref m_FileInfo, observer, instrument, telescope, false, grayScaleRGB);

			m_FileName = fileName;

			BitPix = cameraBitPix;
			FrameRate = frameRate;
			MillisecondsPerFrame = 1000.0 / frameRate;
			UseTimeStamp = useTimeStamp;
            TimeStampReference = timeStampReference;
            m_FireCaptureTimeStamps = fireCaptureTimeStamps;

			Observer = Encoding.UTF8.GetString(observer).Trim();
			Instrument = Encoding.UTF8.GetString(instrument).Trim();
			Telescope = Encoding.UTF8.GetString(telescope).Trim();

            HasTimeStamps = false;
            HasUTCTimeStamps = false;
            HasFireCaptureTimeStamps = false;

			if (useTimeStamp != SerUseTimeStamp.None)
			{
			    HasFireCaptureTimeStamps = useTimeStamp == SerUseTimeStamp.FireCaptureLog;

			    if (!HasFireCaptureTimeStamps)
			    {
                    HasTimeStamps =
                        m_FileInfo.SequenceStartTimeHi != 0 &&
                        m_FileInfo.SequenceStartTimeHi >> 0x1F == 0;

                    HasUTCTimeStamps =
                        m_FileInfo.SequenceStartTimeUTCHi != 0 &&
                        m_FileInfo.SequenceStartTimeUTCHi >> 0x1F == 0;
			    }
			}
		}

		public string Observer { get; private set; }

		public string Instrument { get; private set; }

		public string Telescope { get; private set; }

		public bool HasTimeStamps { get; private set; }

		public bool HasUTCTimeStamps { get; private set; }

        public bool HasFireCaptureTimeStamps { get; private set; }

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
			byte[] rawBitmapBytes = new byte[Pixelmap.GetBitmapBIRGBPixelArraySize(24, Width, Height) + 40 + 14 + 1];

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
                    SystemTime = m_CurrentFrameInfo.TimeStampUtc
				};

			    if (UseTimeStamp != SerUseTimeStamp.None)
			    {
			        if (UseTimeStamp == SerUseTimeStamp.FireCaptureLog)
			        {
                        rv.FrameState.SerTimeStampReference = "FireCapture Log";
			            DateTime dt;
			            if (m_FireCaptureTimeStamps.TryGetValue(1 + index, out dt))
			                rv.FrameState.CentralExposureTime = dt;
			        }
                    else if (UseTimeStamp == SerUseTimeStamp.SerEmbeddedUtcTime)
			        {
			            switch (TimeStampReference)
			            {
			                case SerTimeStampReference.StartFrame:
                                rv.FrameState.SerTimeStampReference = "Start Frame";
                                rv.FrameState.CentralExposureTime = m_CurrentFrameInfo.TimeStampUtc.AddMilliseconds(MillisecondsPerFrame * 0.5);
			                    break;

                            case SerTimeStampReference.EndFrame:
                                rv.FrameState.SerTimeStampReference = "End Frame";
                                rv.FrameState.CentralExposureTime = m_CurrentFrameInfo.TimeStampUtc.AddMilliseconds(MillisecondsPerFrame * -0.5);
			                    break;

                            default:
                                rv.FrameState.SerTimeStampReference = "Mid Frame";
                                rv.FrameState.CentralExposureTime = m_CurrentFrameInfo.TimeStampUtc;
			                    break;
			            }
			        }
			    }

				return rv;
			}
		}

        public SerFrameInfo SerFrameInfoOnly(int index)
	    {
            if (UseTimeStamp == SerUseTimeStamp.FireCaptureLog)
            {
                DateTime dt;
                if (m_FireCaptureTimeStamps.TryGetValue(1 + index, out dt))
                    return new SerFrameInfo(dt);
                    
                return new SerFrameInfo(DateTime.MinValue);
            }
            else
            {
                var frameInfo = new SerNativeFrameInfo();

                TangraCore.SERGetFrameInfo(index, ref frameInfo);

                return new SerFrameInfo(frameInfo);
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

        public bool SupportsIntegrationByMedian
        {
            get { return false; }
        }

		public string VideoFileType
		{
			get { return string.Format("SER.{0}", m_FileInfo.PixelDepthPerPlane); }
		}

		public Pixelmap GetIntegratedFrame(int startFrameNo, int framesToIntegrate, bool isSlidingIntegration, bool isMedianAveraging)
		{
			if (startFrameNo < FirstFrame || startFrameNo > LastFrame)
				throw new ApplicationException("Invalid frame position: " + startFrameNo);

			int actualFramesToIntegrate = Math.Max(1, Math.Min(startFrameNo + framesToIntegrate, LastFrame - 1) - startFrameNo);

			uint[] pixels = new uint[Width * Height];
            uint[] unprocessedPixels = new uint[Width * Height];
			byte[] displayBitmapBytes = new byte[Width * Height];
			byte[] rawBitmapBytes = new byte[Pixelmap.GetBitmapBIRGBPixelArraySize(24, Width, Height) + 40 + 14 + 1];
			var frameInfo = new SerNativeFrameInfo();

            TangraCore.SERGetIntegratedFrame(startFrameNo, actualFramesToIntegrate, isSlidingIntegration, isMedianAveraging, pixels, unprocessedPixels, rawBitmapBytes, displayBitmapBytes, (ushort)BitPix, ref frameInfo);

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
					SystemTime = m_CurrentFrameInfo.TimeStampUtc
				};

                if (UseTimeStamp != SerUseTimeStamp.None)
                {
                    if (UseTimeStamp == SerUseTimeStamp.FireCaptureLog)
                    {
                        DateTime dt, dt2;
                        if (actualFramesToIntegrate % 2 == 0 && actualFramesToIntegrate > 1)
                        {
                            if (
                                m_FireCaptureTimeStamps.TryGetValue(startFrameNo + actualFramesToIntegrate/2, out dt) &&
                                m_FireCaptureTimeStamps.TryGetValue(1 + startFrameNo + actualFramesToIntegrate/2, out dt2))
                            {
                                rv.FrameState.CentralExposureTime = dt.AddTicks((dt2 - dt).Ticks/2);
                            }
                        }
                        else
                        {
                            if (m_FireCaptureTimeStamps.TryGetValue(1 + startFrameNo + actualFramesToIntegrate / 2, out dt))
                                rv.FrameState.CentralExposureTime = dt;                            
                        }
                    }
                    else if (UseTimeStamp == SerUseTimeStamp.SerEmbeddedUtcTime)
                    {
                        rv.FrameState.CentralExposureTime = m_CurrentFrameInfo.TimeStampUtc;
                    }
                }

				return rv;
			}
		}

        private void GetFirstFramePixelStats()
        {
            uint[] pixels = new uint[Width * Height];
            uint[] unprocessedPixels = new uint[Width * Height];
            byte[] displayBitmapBytes = new byte[Width * Height];
            byte[] rawBitmapBytes = new byte[Pixelmap.GetBitmapBIRGBPixelArraySize(24, Width, Height) + 40 + 14 + 1];

            var frameInfo = new SerNativeFrameInfo();

            TangraCore.SERGetFrame(FirstFrame, pixels, unprocessedPixels, rawBitmapBytes, displayBitmapBytes, (ushort)BitPix, ref frameInfo);

            var pixMin = unprocessedPixels.Min();
            var pixMax = unprocessedPixels.Max();

            DynamicRangeHelper.SuggestUpperLowerAutoDynamicRangeLimit(pixels, pixMin, pixMax, out m_MinPixelValueFirstImage, out m_MaxPixelValueFirstImage);
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

        public uint MinPixelValue
        {
            get
            {
                if (m_MinPixelValueFirstImage == null)
                {
                    GetFirstFramePixelStats();
                }

                return m_MinPixelValueFirstImage ?? 0;
            }
        }

        public uint MaxPixelValue
        {
            get
            {
                if (m_MaxPixelValueFirstImage == null)
                {
                    GetFirstFramePixelStats();
                }

                return m_MaxPixelValueFirstImage ?? 0;
            }
        }

        public bool CanDetermineAutoDynamicRange
        {
            get { return true; }
        }
	}
}
