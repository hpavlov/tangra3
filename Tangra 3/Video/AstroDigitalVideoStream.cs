using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Tangra.Model.Config;
using Tangra.Model.Context;
using Tangra.Model.Helpers;
using Tangra.Model.Image;
using Tangra.Model.Numerical;
using Tangra.Model.Video;
using Tangra.PInvoke;
using Tangra.Video.AstroDigitalVideo;

namespace Tangra.Video
{
	public class ADVFormatException : Exception
	{
		public ADVFormatException(string message)
			: base(message)
		{ }
	}

    public class AdvEquipmentInfo
    {
        public string Recorder;
        public string AdvrVersion;
        public string HtccFirmareVersion;
        public string Camera;
        public string SensorInfo;
        public string Engine;
    }

	public class AstroDigitalVideoStream : IFrameStream
	{
		public static AstroDigitalVideoStream OpenFile(string fileName, out AdvEquipmentInfo equipmentInfo, out GeoLocationInfo geoLocation)
		{
            equipmentInfo = new AdvEquipmentInfo();
			geoLocation = new GeoLocationInfo();
			try
			{
				var rv = new AstroDigitalVideoStream(fileName, ref equipmentInfo, ref geoLocation);

                TangraContext.Current.RenderingEngine = equipmentInfo.Engine == "AAV" ? "AstroAnalogueVideo" : "AstroDigitalVideo";

			    return rv;
			}
			catch(ADVFormatException ex)
			{
				MessageBox.Show(ex.Message, "Error opening ADV/AAV file", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

			return null;
		}

		private string m_FileName;
		private int m_Width;
		private int m_Height;
		private double m_FrameRate;

		private int m_FirstFrame;
		private int m_CountFrames;

		private int m_BitPix;
		private bool m_FileOpened;
	    private string m_Engine;
		private string m_CameraModel;
		private string m_VideoStandard;
		private double m_NativeFrameRate;
	    private double m_EffectiveFrameRate;
	    private int m_IntegratedAAVFrames;
		private int m_OsdFirstLine = 0;
		private int m_OsdLastLine = 0;

		private int m_AlmanacOffsetLastFrame;
		private bool m_AlamanacOffsetLastFrameIsGood;

		private AdvFrameInfo m_CurrentFrameInfo;

		private AstroDigitalVideoStream(string fileName, ref AdvEquipmentInfo equipmentInfo, ref GeoLocationInfo geoLocation)
		{
			CheckAdvFileFormat(fileName, ref equipmentInfo, ref geoLocation);

			m_FileName = fileName;
			var fileInfo = new AdvFileInfo();
			
			TangraCore.ADVOpenFile(fileName, ref fileInfo);

			m_FirstFrame = 0;
			m_CountFrames = fileInfo.CountFrames;

			m_BitPix = fileInfo.Bpp;
			m_Width = fileInfo.Width;
			m_Height = fileInfo.Height;

			m_FrameRate = fileInfo.FrameRate;
			

			// Get the last frame in the video and read the Almanac Offset and Almanac Status so they are applied 
			// to the frames that didn't have Almanac Status = Updated
			if (m_CountFrames > 0)
			{
				GetPixelmap(m_FirstFrame + m_CountFrames - 1);

				m_AlamanacOffsetLastFrameIsGood = m_CurrentFrameInfo.AlmanacStatusIsGood;
				m_AlmanacOffsetLastFrame = m_CurrentFrameInfo.GetSignedAlamancOffset();
			}
			else
			{
				m_AlamanacOffsetLastFrameIsGood = false;
				m_AlmanacOffsetLastFrame = 0;
			}

		    m_Engine = equipmentInfo.Engine;
			m_CameraModel = equipmentInfo.Camera;

			if (m_Engine == "AAV")
			{
				m_VideoStandard = GetFileTag("NATIVE-VIDEO-STANDARD");
				double.TryParse(GetFileTag("NATIVE-FRAME-RATE"), out m_NativeFrameRate);

				int.TryParse(GetFileTag("OSD-FIRST-LINE"), out m_OsdFirstLine);
				int.TryParse(GetFileTag("OSD-LAST-LINE"), out m_OsdLastLine);

			    if (m_OsdLastLine > m_Height) m_OsdLastLine = m_Height;
                if (m_OsdFirstLine < 0) m_OsdFirstLine = 0;

			    m_IntegratedAAVFrames = -1;

                if (double.TryParse(GetFileTag("EFFECTIVE-FRAME-RATE"), out m_EffectiveFrameRate) && m_NativeFrameRate != 0)
                {
					m_IntegratedAAVFrames = (int)Math.Round(m_NativeFrameRate / m_EffectiveFrameRate);
                }
			}
			else
			{
				m_OsdFirstLine = 0;
				m_OsdLastLine = 0;
			}

			m_FileOpened = true;
		    m_OcrDataAvailable = null;
		}

		public string FileName
		{
			get { return m_FileName; }
		}

		public int Width
		{
			get { return m_Width; }
		}

		public int Height
		{
			get { return m_Height; }
		}

		public int BitPix
		{
			get { return m_BitPix; }
		}

		public int FirstFrame
		{
			get { return 0; }
		}

		public int LastFrame
		{
			get { return m_CountFrames - 1; }
		}

		public int CountFrames
		{
			get { return m_CountFrames; }
		}

		public double FrameRate
		{
			get { return m_FrameRate; }
		}

		public double NativeFrameRate
		{
			get { return m_NativeFrameRate; }
		}

        public int IntegratedAAVFrames
		{
            get { return m_IntegratedAAVFrames; }
		}

		public string VideoStandard
		{
			get { return m_VideoStandard; }
		}
		
		public double MillisecondsPerFrame
		{
			get { return (1000 / m_FrameRate); }
		}

		public AdvFrameInfo GetStatusChannel(int index)
		{
			var frameInfo = new AdvFrameInfoNative();

			byte[] gpsFix = new byte[256 * 16];
			byte[] userCommand = new byte[256 * 16];
			byte[] systemError = new byte[256 * 16];

			TangraCore.ADVGetFrameStatusChannel(index, frameInfo, gpsFix, userCommand, systemError);

			var rv = new AdvFrameInfo(frameInfo)
			{
				UserCommandString = AdvFrameInfo.GetStringFromBytes(userCommand),
				SystemErrorString = AdvFrameInfo.GetStringFromBytes(systemError),
				GPSFixString = AdvFrameInfo.GetStringFromBytes(gpsFix)		
			};

			return rv;
		}

		public Pixelmap GetPixelmap(int index)
		{
			if (index >= m_FirstFrame + m_CountFrames)
				throw new ApplicationException("Invalid frame position: " + index);

			uint[] pixels = new uint[m_Width * m_Height];
			byte[] displayBitmapBytes = new byte[m_Width * m_Height];
			byte[] rawBitmapBytes = new byte[(m_Width * m_Height * 3) + 40 + 14 + 1];
			var frameInfo = new AdvFrameInfoNative();

			byte[] gpsFix = new byte[256 * 16];
			byte[] userCommand = new byte[256 * 16];
			byte[] systemError = new byte[256 * 16];

			TangraCore.ADVGetFrame(index, pixels, rawBitmapBytes, displayBitmapBytes, frameInfo, gpsFix, userCommand, systemError);

			m_CurrentFrameInfo = new AdvFrameInfo(frameInfo);
			m_CurrentFrameInfo.UserCommandString = AdvFrameInfo.GetStringFromBytes(userCommand);
			m_CurrentFrameInfo.SystemErrorString = AdvFrameInfo.GetStringFromBytes(systemError);
			m_CurrentFrameInfo.GPSFixString = AdvFrameInfo.GetStringFromBytes(gpsFix);

			if (m_Engine == "AAV" && m_CurrentFrameInfo.IntegratedFrames > 0 && TangraConfig.Settings.AAV.SplitFieldsOSD && m_OsdFirstLine * m_OsdLastLine != 0)
			{
				TangraCore.BitmapSplitFieldsOSD(rawBitmapBytes, m_OsdFirstLine, m_OsdLastLine);
			}

			using (MemoryStream memStr = new MemoryStream(rawBitmapBytes))
			{
			    Bitmap displayBitmap;
                try
                {
                    displayBitmap = (Bitmap) Bitmap.FromStream(memStr);
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.GetFullStackTrace());
                    displayBitmap = new Bitmap(m_Width, m_Height);
                }

                var rv = new Pixelmap(m_Width, m_Height, m_BitPix, pixels, displayBitmap, displayBitmapBytes);
				rv.FrameState = GetCurrentFrameState(index);
				return rv;
			}
		}

		public string GetFileTag(string tagName)
		{
			byte[] tagValue = new byte[256 * 2];

			TangraCore.ADVGetFileTag(tagName, tagValue);

			return AdvFrameInfo.GetStringFromBytes(tagValue);
		}

		public FrameStateData GetFrameStatusChannel(int index)
		{
			if (index >= m_FirstFrame + m_CountFrames)
				throw new ApplicationException("Invalid frame position: " + index);
			
			var frameInfo = new AdvFrameInfoNative();

			byte[] gpsFix = new byte[256 * 16];
			byte[] userCommand = new byte[256 * 16];
			byte[] systemError = new byte[256 * 16];

			TangraCore.ADVGetFrameStatusChannel(index, frameInfo, gpsFix, userCommand, systemError);

			var frameStatusChannel = new AdvFrameInfo(frameInfo);
			frameStatusChannel.UserCommandString = AdvFrameInfo.GetStringFromBytes(userCommand);
			frameStatusChannel.SystemErrorString = AdvFrameInfo.GetStringFromBytes(systemError);
			frameStatusChannel.GPSFixString = AdvFrameInfo.GetStringFromBytes(gpsFix);

			return AdvFrameInfoToFrameStateData(frameStatusChannel, index);
		}

		public int RecommendedBufferSize
		{
			get { return 8; }
		}

		public string VideoFileType
		{
            get { return string.Format("{0}.{1}", m_Engine, BitPix); }
		}

		public string CameraModel
		{
			get { return m_CameraModel; }
		}

		private void AddExtraNtpDebugTimes(FrameStateData stateData, AdvFrameInfo frameInfo)
		{
			if (stateData.AdditionalProperties == null)
				stateData.AdditionalProperties = new SafeDictionary<string, object>();

			stateData.AdditionalProperties.Add("MidTimeNTPRaw", stateData.EndFrameNtpTime.AddMilliseconds(-0.5 * stateData.ExposureInMilliseconds));
			stateData.AdditionalProperties.Add("MidTimeNTPFitted", stateData.CentralExposureTime);
			stateData.AdditionalProperties.Add("MidTimeWindowsRaw", frameInfo.EndExposureSecondaryTimeStamp.AddMilliseconds(-0.5 * stateData.ExposureInMilliseconds));
		}

		private FrameStateData GetCurrentFrameState(int frameNo)
		{
			if (m_CurrentFrameInfo != null)
			{
				var rv = new FrameStateData();
				rv.VideoCameraFrameId = m_CurrentFrameInfo.VideoCameraFrameId;
				rv.CentralExposureTime = m_CurrentFrameInfo.MiddleExposureTimeStamp;
				rv.SystemTime = m_CurrentFrameInfo.SystemTime;
				rv.EndFrameNtpTime = m_CurrentFrameInfo.EndExposureNtpTimeStamp;
				rv.ExposureInMilliseconds = m_CurrentFrameInfo.Exposure10thMs / 10.0f;

				rv.NumberIntegratedFrames = (int)m_CurrentFrameInfo.IntegratedFrames;

				int almanacStatus = m_CurrentFrameInfo.GPSAlmanacStatus;
				int almanacOffset = m_CurrentFrameInfo.GetSignedAlamancOffset();

				if (!m_CurrentFrameInfo.AlmanacStatusIsGood && m_AlamanacOffsetLastFrameIsGood)
				{
					// When the current almanac is not good, but last frame is, then apply the known almanac offset automatically
					almanacOffset = m_AlmanacOffsetLastFrame;
					rv.CentralExposureTime = rv.CentralExposureTime.AddSeconds(m_AlmanacOffsetLastFrame);
					almanacStatus = 2; // Certain
				}
				

				rv.Gain = m_CurrentFrameInfo.Gain;
				rv.Gamma = m_CurrentFrameInfo.Gamma;
				rv.Offset = m_CurrentFrameInfo.Offset;

				rv.NumberSatellites = m_CurrentFrameInfo.GPSTrackedSattelites;

				rv.AlmanacStatus = AdvStatusValuesHelper.TranslateGpsAlmanacStatus(almanacStatus);

				rv.AlmanacOffset = AdvStatusValuesHelper.TranslateGpsAlmanacOffset(almanacStatus, almanacOffset, almanacStatus > 0);

				rv.GPSFixStatus = m_CurrentFrameInfo.GPSFixStatus.ToString("#");

				rv.Messages = string.Empty;
				if (m_CurrentFrameInfo.SystemErrorString != null)
					rv.Messages = string.Concat(rv.Messages, m_CurrentFrameInfo.SystemErrorString, "\r\n");
				if (m_CurrentFrameInfo.UserCommandString != null)
					rv.Messages = string.Concat(rv.Messages, m_CurrentFrameInfo.UserCommandString, "\r\n");
				if (m_CurrentFrameInfo.GPSFixString != null)
					rv.Messages = string.Concat(rv.Messages, m_CurrentFrameInfo.GPSFixString, "\r\n");

				if (m_UseNtpTimeAsCentralExposureTime)
				{
					rv.CentralExposureTime = ComputeCentralExposureTimeFromNtpTime(frameNo);
					rv.ExposureInMilliseconds = (float)(1000 / m_FrameRate);
				}

				if (NtpDataAvailable && !OcrDataAvailable && m_UseNtpTimeAsCentralExposureTime)
					AddExtraNtpDebugTimes(rv, m_CurrentFrameInfo);

				return rv;
			}
			else
				return new FrameStateData();
		}

		private FrameStateData AdvFrameInfoToFrameStateData(AdvFrameInfo frameInfo, int frameIndex)
		{
			if (frameInfo != null)
			{
				var rv = new FrameStateData();
				rv.VideoCameraFrameId = frameInfo.VideoCameraFrameId;
				rv.CentralExposureTime = frameInfo.MiddleExposureTimeStamp;
				rv.SystemTime = frameInfo.SystemTime;
				rv.EndFrameNtpTime = frameInfo.EndExposureNtpTimeStamp;
				rv.ExposureInMilliseconds = frameInfo.Exposure10thMs / 10.0f;

				rv.NumberIntegratedFrames = (int)frameInfo.IntegratedFrames;

				int almanacStatus = frameInfo.GPSAlmanacStatus;
				int almanacOffset = frameInfo.GetSignedAlamancOffset();

				if (!frameInfo.AlmanacStatusIsGood && frameInfo.AlmanacStatusIsGood)
				{
					// When the current almanac is not good, but last frame is, then apply the known almanac offset automatically
					almanacOffset = frameInfo.GPSAlmanacOffset;
					rv.CentralExposureTime = rv.CentralExposureTime.AddSeconds(frameInfo.GPSAlmanacOffset);
					almanacStatus = 2; // Certain
				}


				rv.Gain = frameInfo.Gain;
				rv.Gamma = frameInfo.Gamma;
				rv.Offset = frameInfo.Offset;

				rv.NumberSatellites = frameInfo.GPSTrackedSattelites;

				rv.AlmanacStatus = AdvStatusValuesHelper.TranslateGpsAlmanacStatus(almanacStatus);

				rv.AlmanacOffset = AdvStatusValuesHelper.TranslateGpsAlmanacOffset(almanacStatus, almanacOffset, almanacStatus > 0);

				rv.GPSFixStatus = frameInfo.GPSFixStatus.ToString("#");

				rv.Messages = string.Empty;
				if (frameInfo.SystemErrorString != null)
					rv.Messages = string.Concat(rv.Messages, frameInfo.SystemErrorString, "\r\n");
				if (frameInfo.UserCommandString != null)
					rv.Messages = string.Concat(rv.Messages, frameInfo.UserCommandString, "\r\n");
				if (frameInfo.GPSFixString != null)
					rv.Messages = string.Concat(rv.Messages, frameInfo.GPSFixString, "\r\n");

				if (m_UseNtpTimeAsCentralExposureTime)
				{
					rv.CentralExposureTime = ComputeCentralExposureTimeFromNtpTime(frameIndex);
					rv.ExposureInMilliseconds = (float)(1000 / m_FrameRate);
				}

				if (NtpDataAvailable && !OcrDataAvailable && m_UseNtpTimeAsCentralExposureTime)
					AddExtraNtpDebugTimes(rv, frameInfo);

				return rv;
			}
			else
				return new FrameStateData();
		}

		public Pixelmap GetIntegratedFrame(int startFrameNo, int framesToIntegrate, bool isSlidingIntegration, bool isMedianAveraging)
		{
			if (startFrameNo < 0 || startFrameNo >= m_FirstFrame + m_CountFrames)
				throw new ApplicationException("Invalid frame position: " + startFrameNo);

			int actualFramesToIntegrate = Math.Min(startFrameNo + framesToIntegrate, m_FirstFrame + m_CountFrames - 1) - startFrameNo;

			uint[] pixels = new uint[m_Width * m_Height];
			byte[] displayBitmapBytes = new byte[m_Width * m_Height];
			byte[] rawBitmapBytes = new byte[(m_Width * m_Height * 3) + 40 + 14 + 1];
			var frameInfo = new AdvFrameInfoNative();

			TangraCore.ADVGetIntegratedFrame(startFrameNo, actualFramesToIntegrate, isSlidingIntegration, isMedianAveraging, pixels, rawBitmapBytes, displayBitmapBytes, frameInfo);

			m_CurrentFrameInfo = new AdvFrameInfo(frameInfo);

			using (MemoryStream memStr = new MemoryStream(rawBitmapBytes))
			{
				Bitmap displayBitmap = (Bitmap)Bitmap.FromStream(memStr);

                return new Pixelmap(m_Width, m_Height, m_BitPix, pixels, displayBitmap, displayBitmapBytes);
			}
		}

	    private string engine = null;

		public string Engine
		{
            get { return engine; }
		}

        public static long MISSING_TIMESTAMP_TICKS = 633979008000000000;

        public string OcrEngine = null;
	    public bool? m_OcrDataAvailable = null;

        public bool OcrDataAvailable
        {
			get
			{
			    if (string.IsNullOrEmpty(OcrEngine))
			        return false;

				if (TangraConfig.Settings.AAV.NtpDebugFlag)
					return false;

                if (!m_OcrDataAvailable.HasValue)
                {
                    m_OcrDataAvailable = false;

                    for (int i = m_FirstFrame; i < m_FirstFrame + m_CountFrames; i++)
                    {
                        FrameStateData stateChannel = GetFrameStatusChannel(i);
                        if (stateChannel.CentralExposureTime.Ticks != MISSING_TIMESTAMP_TICKS)
                        {
                            m_OcrDataAvailable = true;
                            break;
                        }
                    }
                   
                }

			    return m_OcrDataAvailable.Value;
			}
        }

		public bool? m_NtpDataAvailable = null;

		public bool NtpDataAvailable
		{
			get
			{
				if (!m_NtpDataAvailable.HasValue)
				{
					m_NtpDataAvailable = false;

					for (int i = m_FirstFrame; i < m_FirstFrame + m_CountFrames; i++)
					{
						FrameStateData stateChannel = GetFrameStatusChannel(i);
						if (stateChannel.HasValidNtpTimeStamp)
						{
							m_NtpDataAvailable = true;
							break;
						}
					}

				}

				return m_NtpDataAvailable.Value;			
			}
		}

		private bool m_UseNtpTimeAsCentralExposureTime = false;
		private long m_CalibratedNtpTimeZeroPoint;
		private LinearRegression m_CalibratedNtpTimeSource;

		public void AstroAnalogueVideoNormaliseNtpDataIfNeeded(Action<int> progressCallback)
		{
			if (NtpDataAvailable && !OcrDataAvailable && !m_UseNtpTimeAsCentralExposureTime)
			{
				if (m_CountFrames > 1 /* No Timestamp for first frame */ + 1 /* No Timestamp for last frame*/ + 3 /* Minimum timestamped frames for a FIT */)
				{
					try
					{
						double frameDurationMilliseconds = 1000 / m_FrameRate;
						var lr = new LinearRegression();

						long zeroPointTicks = -1;

						int percentDone = 0;
						int percentDoneCalled = 0;						
						if (progressCallback != null) progressCallback(percentDone);

						for (int i = m_FirstFrame; i < m_FirstFrame + m_CountFrames; i++)
						{
							FrameStateData stateChannel = GetFrameStatusChannel(i);
							if (stateChannel.HasValidNtpTimeStamp)
							{
								long centralTicks = stateChannel.EndFrameNtpTime.AddMilliseconds(-0.5 * frameDurationMilliseconds).Ticks;
								if (zeroPointTicks == -1)
									zeroPointTicks = centralTicks;
								lr.AddDataPoint(i, new TimeSpan(centralTicks - zeroPointTicks).TotalMilliseconds);
							}

							percentDone = 100 * (i - m_FirstFrame) / m_CountFrames;
							if (progressCallback != null && percentDone - percentDoneCalled> 5)
							{
								progressCallback(percentDone);
								percentDoneCalled = percentDone;
							}
						}

						if (lr.NumberOfDataPoints > 3)
						{
							lr.Solve();

							m_CalibratedNtpTimeZeroPoint = zeroPointTicks;
							m_CalibratedNtpTimeSource = lr;
							m_UseNtpTimeAsCentralExposureTime = true;

							Trace.WriteLine(string.Format("NTP Timebase Established. 1-Sigma = {0} ms", lr.StdDev.ToString("0.00")));
						}

						progressCallback(100);
					}
					catch (Exception ex)
					{
						Trace.WriteLine(ex.GetFullStackTrace());
					}
				}
			}			
		}

		private DateTime ComputeCentralExposureTimeFromNtpTime(int frameNo)
		{
			return new DateTime(m_CalibratedNtpTimeZeroPoint).AddMilliseconds(m_CalibratedNtpTimeSource.ComputeY(frameNo));
		}

		private GeoLocationInfo geoLocation;

		public GeoLocationInfo GeoLocation
		{
			get { return geoLocation; }
		}

        private void CheckAdvFileFormat(string fileName, ref AdvEquipmentInfo equipmentInfo, ref GeoLocationInfo geoLocation)
		{
			AdvFile advFile = AdvFile.OpenFile(fileName);

			CheckAdvFileFormatInternal(advFile);

            equipmentInfo.Recorder = advFile.AdvFileTags["RECORDER"];
            equipmentInfo.Camera = advFile.AdvFileTags["CAMERA-MODEL"];
            equipmentInfo.Engine = advFile.AdvFileTags["FSTF-TYPE"];

            engine = equipmentInfo.Engine;

            if (engine == "ADV")
            {
                equipmentInfo.AdvrVersion = advFile.AdvFileTags["ADVR-SOFTWARE-VERSION"];
                equipmentInfo.HtccFirmareVersion = advFile.AdvFileTags["HTCC-FIRMWARE-VERSION"];
                equipmentInfo.SensorInfo = advFile.AdvFileTags["CAMERA-SENSOR-INFO"];

                advFile.AdvFileTags.TryGetValue("LONGITUDE-WGS84", out geoLocation.Longitude);
                advFile.AdvFileTags.TryGetValue("LATITUDE-WGS84", out geoLocation.Latitude);
                advFile.AdvFileTags.TryGetValue("ALTITUDE-MSL", out geoLocation.Altitude);
                advFile.AdvFileTags.TryGetValue("MSL-WGS84-OFFSET", out geoLocation.MslWgs84Offset);

				if (!string.IsNullOrEmpty(geoLocation.MslWgs84Offset) &&
				    !geoLocation.MslWgs84Offset.StartsWith("-"))
				{
					geoLocation.MslWgs84Offset = "+" + geoLocation.MslWgs84Offset;
				}

                advFile.AdvFileTags.TryGetValue("GPS-HDOP", out geoLocation.GpsHdop);
            }
            else if (engine == "AAV")
            {
                advFile.AdvFileTags.TryGetValue("OCR-ENGINE", out OcrEngine);
            }

            this.geoLocation = new GeoLocationInfo(geoLocation);
		}

		private void CheckAdvFileFormatInternal(AdvFile advFile)
		{
			bool fileIsCorrupted = true;
			bool isADVFormat = false;
			int advFormatVersion = -1;
			DoConsistencyCheck(advFile, ref fileIsCorrupted, ref isADVFormat, ref advFormatVersion);

			if (!isADVFormat)
				throw new ADVFormatException("The file is not in ADV/AAV format.");

			if (advFormatVersion > 1)
                throw new ADVFormatException("The file ADV/AAV version is not supported yet.");

			if (fileIsCorrupted)
                throw new ADVFormatException("The ADV/AAV file may be corrupted.\r\n\r\nTry to recover it from Tools -> ADV/AAV Tools -> Repair ADV/AAV File");
		}

		private void DoConsistencyCheck(AdvFile advFile, ref bool fileIsCorrupted, ref bool isADVFormat, ref int advFormatVersion)
		{
			fileIsCorrupted = advFile.IsCorrupted;

			string fstsTypeStr;
			if (!advFile.AdvFileTags.TryGetValue("FSTF-TYPE", out fstsTypeStr))
				isADVFormat = false;
			else
                isADVFormat = fstsTypeStr == "ADV" || fstsTypeStr == "AAV";

            if (fstsTypeStr == "ADV")
            {
                string advVersionStr;
                if (!advFile.AdvFileTags.TryGetValue("ADV-VERSION", out advVersionStr))
                    advFormatVersion = -1;
                else
                    if (!int.TryParse(advVersionStr, out advFormatVersion))
                        advFormatVersion = -1;                
            }
            else if (fstsTypeStr == "AAV")
            {
                string advVersionStr;
                if (!advFile.AdvFileTags.TryGetValue("AAV-VERSION", out advVersionStr))
                    advFormatVersion = -1;
                else
                    if (!int.TryParse(advVersionStr, out advFormatVersion))
                        advFormatVersion = -1;                
            }
		}
	}
}
