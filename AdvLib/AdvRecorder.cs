using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class AdvRecorder
{
	private uint m_TAGID_SystemTime;
	private uint m_TAGID_TrackedGPSSatellites;
	private uint m_TAGID_GPSAlmanacStatus;
	private uint m_TAGID_GPSAlmanacOffset;
	private uint m_TAGID_GPSFixStatus;
	private uint m_TAGID_Gain;
	private uint m_TAGID_Shutter;
	private uint m_TAGID_Offset;
	private uint m_TAGID_VideoCameraFrameId;
	private uint m_TAGID_Gamma;
	private uint m_TAGID_UserCommand;
	private uint m_TAGID_SystemError;

	private int m_NumberRecordedFrames = 0;
	private int m_NumberDroppedFrames = 0;
	private long m_FirstRecordedFrameTimestamp = 0;

	private const byte CFG_ADV_LAYOUT_1_UNCOMPRESSED = 1;
	private const byte CFG_ADV_LAYOUT_2_COMPRESSED = 2;
	private const byte CFG_ADV_LAYOUT_3_COMPRESSED = 3;


	public AdvStatusSectionConfig StatusSectionConfig = new AdvStatusSectionConfig();
 
	public AdvRecorder()
	{
			
	}

	public string RecorderName { get; set; }
	public string RecorderVersion { get; set; }
	public string RecorderTimerFirmwareVersion { get; set; }

	public string CameraModel { get; set; }
	public string CameraSerialNumber { get; set; }
	public string CameraVendorNumber { get; set; }
	public string CameraSensorInfo { get; set; }
	public string CameraSensorResolution { get; set; }
	public string CameraFirmwareVersion { get; set; }
	public string CameraFirmwareBuildTime { get; set; }
	public string CameraDriverVersion { get; set; }

	public ushort ImageWidth { get; private set; }
	public ushort ImageHeight { get; private set; }
	public byte ImageBitsPerPixel { get; private set; }
	public bool ImageBigEndian { get; set; }

	public void SetImageParameters(ushort width, ushort height, byte bpp)
	{
		ImageWidth = width;
		ImageHeight = height;
		ImageBitsPerPixel = bpp;
	}

	public void StartRecordingNewFile(string fileName)
	{
		AdvLib.AdvNewFile(fileName);

		AdvLib.AdvAddFileTag("RECORDER-SOFTWARE-VERSION", RecorderVersion);
		AdvLib.AdvAddFileTag("TIMER-FIRMWARE-VERSION", RecorderTimerFirmwareVersion);
		AdvLib.AdvAddFileTag("ADVLIB-VERSION", "1.0");

		AdvLib.AdvAddFileTag("RECORDER", RecorderName);
		AdvLib.AdvAddFileTag("FSTF-TYPE", "ADV");
		AdvLib.AdvAddFileTag("ADV-VERSION", "1");

		//AdvLib.AdvAddFileTag("LONGITUDE-WGS84", &SYS_GPS_LONGITUDE[0]);
		//AdvLib.AdvAddFileTag("LATITUDE-WGS84", &SYS_GPS_LATITUDE[0]);
		//AdvLib.AdvAddFileTag("ALTITUDE-MSL", &SYS_GPS_ALTITUDE[0]);
		//AdvLib.AdvAddFileTag("MSL-WGS84-OFFSET", &SYS_GPS_WSG84[0]);
		//AdvLib.AdvAddFileTag("GPS-HDOP", &SYS_GPS_HDOP[0]);

		AdvLib.AdvAddFileTag("CAMERA-MODEL", CameraModel);
		AdvLib.AdvAddFileTag("CAMERA-SERIAL-NO", CameraSerialNumber);
		AdvLib.AdvAddFileTag("CAMERA-VENDOR-NAME", CameraVendorNumber);
		AdvLib.AdvAddFileTag("CAMERA-SENSOR-INFO", CameraSensorInfo);
		AdvLib.AdvAddFileTag("CAMERA-SENSOR-RESOLUTION", CameraSensorResolution);
		AdvLib.AdvAddFileTag("CAMERA-FIRMWARE-VERSION", CameraFirmwareVersion);
		AdvLib.AdvAddFileTag("CAMERA-FIRMWARE-BUILD-TIME", CameraFirmwareBuildTime);
		AdvLib.AdvAddFileTag("CAMERA-DRIVER-VERSION", CameraDriverVersion);

		AdvLib.AdvDefineImageSection(ImageWidth, ImageHeight, ImageBitsPerPixel);
		AdvLib.AdvAddOrUpdateImageSectionTag("IMAGE-BYTE-ORDER", ImageBigEndian ? "BIG-ENDIAN" : "LITTLE-ENDIAN");

		AdvLib.AdvDefineImageLayout(CFG_ADV_LAYOUT_1_UNCOMPRESSED, "FULL-IMAGE-RAW", "UNCOMPRESSED", 16, 0, null);
		AdvLib.AdvDefineImageLayout(CFG_ADV_LAYOUT_2_COMPRESSED, "FULL-IMAGE-DIFFERENTIAL-CODING", "QUICKLZ", 12, 32, "PREV-FRAME");
		AdvLib.AdvDefineImageLayout(CFG_ADV_LAYOUT_3_COMPRESSED, "FULL-IMAGE-RAW", "QUICKLZ", 16, 0, null);

		if (StatusSectionConfig.RecordSystemTime) m_TAGID_SystemTime = AdvLib.AdvDefineStatusSectionTag("SystemTime", AdvTagType.ULong64);

		if (StatusSectionConfig.RecordSystemTime) m_TAGID_TrackedGPSSatellites = AdvLib.AdvDefineStatusSectionTag("GPSTrackedSatellites", AdvTagType.UInt8);
		if (StatusSectionConfig.RecordSystemTime) m_TAGID_GPSAlmanacStatus = AdvLib.AdvDefineStatusSectionTag("GPSAlmanacStatus", AdvTagType.UInt8);
		if (StatusSectionConfig.RecordSystemTime) m_TAGID_GPSAlmanacOffset = AdvLib.AdvDefineStatusSectionTag("GPSAlmanacOffset", AdvTagType.UInt8);
		if (StatusSectionConfig.RecordSystemTime) m_TAGID_GPSFixStatus = AdvLib.AdvDefineStatusSectionTag("GPSFixStatus", AdvTagType.UInt8);
		if (StatusSectionConfig.RecordSystemTime) m_TAGID_Gain = AdvLib.AdvDefineStatusSectionTag("Gain", AdvTagType.Real);
		if (StatusSectionConfig.RecordSystemTime) m_TAGID_Shutter = AdvLib.AdvDefineStatusSectionTag("Shutter", AdvTagType.Real);
		if (StatusSectionConfig.RecordSystemTime) m_TAGID_Offset = AdvLib.AdvDefineStatusSectionTag("Offset", AdvTagType.Real);
		if (StatusSectionConfig.RecordSystemTime) m_TAGID_VideoCameraFrameId = AdvLib.AdvDefineStatusSectionTag("VideoCameraFrameId", AdvTagType.ULong64);
		if (StatusSectionConfig.RecordSystemTime) m_TAGID_Gamma = AdvLib.AdvDefineStatusSectionTag("Gamma", AdvTagType.Real);
		if (StatusSectionConfig.RecordSystemTime) m_TAGID_UserCommand = AdvLib.AdvDefineStatusSectionTag("UserCommand", AdvTagType.List16OfAnsiString255);
		if (StatusSectionConfig.RecordSystemTime) m_TAGID_SystemError = AdvLib.AdvDefineStatusSectionTag("SystemError", AdvTagType.List16OfAnsiString255);

		m_NumberRecordedFrames = 0;
		m_NumberDroppedFrames = 0;
		m_FirstRecordedFrameTimestamp = 0;
	}

	public void StopRecording()
	{
		AdvLib.AdvEndFile();
	}

	public void AddVideoFrame(ushort[] pixels, bool compress, AdvTimeStamp timeStamp, uint exposureIn10thMilliseconds, AdvStatusEntry metadata)
	{
		byte layoutIdForCurrentFramerate = compress ? CFG_ADV_LAYOUT_3_COMPRESSED : CFG_ADV_LAYOUT_1_UNCOMPRESSED;
		
		long elapsedTimeMilliseconds = 0; // since the first recorded frame was taken
		if (m_NumberRecordedFrames > 0 && m_FirstRecordedFrameTimestamp != 0)
		{
			elapsedTimeMilliseconds = timeStamp.MillisecondsAfterAdvZeroEpoch - m_FirstRecordedFrameTimestamp;
		}
		else if (m_NumberRecordedFrames == 0)
		{
			m_FirstRecordedFrameTimestamp = timeStamp.MillisecondsAfterAdvZeroEpoch;
		}

		bool frameStartedOk = AdvLib.AdvBeginFrame(timeStamp.MillisecondsAfterAdvZeroEpoch, elapsedTimeMilliseconds > 0 ? (uint)elapsedTimeMilliseconds : 0, exposureIn10thMilliseconds);
		if (!frameStartedOk)
		{
			// If we can't add the first frame, this may be a file creation issue; otherwise increase the dropped frames counter
			if (m_NumberRecordedFrames > 0)
				m_NumberDroppedFrames++;
			return;
		}
	
		if (StatusSectionConfig.RecordSystemTime) AdvLib.AdvFrameAddStatusTag64(m_TAGID_SystemTime, metadata.SystemTime.MillisecondsAfterAdvZeroEpoch);
	
		if (StatusSectionConfig.RecordGPSTrackedSatellites) AdvLib.AdvFrameAddStatusTagUInt8(m_TAGID_TrackedGPSSatellites, metadata.GPSTrackedSatellites);
		if (StatusSectionConfig.RecordGPSAlmanacStatus) AdvLib.AdvFrameAddStatusTagUInt8(m_TAGID_GPSAlmanacStatus, (byte)metadata.GPSAlmanacStatus);
		if (StatusSectionConfig.RecordGPSAlmanacOffset) AdvLib.AdvFrameAddStatusTagUInt8(m_TAGID_GPSAlmanacOffset, metadata.GPSAlmanacOffset);	
		if (StatusSectionConfig.RecordGPSFixStatus) AdvLib.AdvFrameAddStatusTagUInt8(m_TAGID_GPSFixStatus, (byte)metadata.GPSFixStatus);
		if (StatusSectionConfig.RecordGain) AdvLib.AdvFrameAddStatusTagReal(m_TAGID_Gain, metadata.Gain);
		if (StatusSectionConfig.RecordGamma) AdvLib.AdvFrameAddStatusTagReal(m_TAGID_Gamma, metadata.Gamma);
		if (StatusSectionConfig.RecordShutter) AdvLib.AdvFrameAddStatusTagReal(m_TAGID_Shutter, metadata.Shutter);
		if (StatusSectionConfig.RecordCameraOffset) AdvLib.AdvFrameAddStatusTagReal(m_TAGID_Offset, metadata.CameraOffset);
		if (StatusSectionConfig.RecordVideoCameraFrameId) AdvLib.AdvFrameAddStatusTag64(m_TAGID_VideoCameraFrameId, metadata.VideoCameraFrameId);

		if (StatusSectionConfig.RecordUserCommands && metadata.UserCommands != null)
		{
			for (int i = 0; i < Math.Min(16, metadata.UserCommands.Count()); i++)
			{
				if (metadata.UserCommands[i] != null)
				{
					if (metadata.UserCommands[i].Length > 255)
						AdvLib.AdvFrameAddStatusTagMessage(m_TAGID_UserCommand, metadata.UserCommands[i].Substring(0, 255));
					else
						AdvLib.AdvFrameAddStatusTagMessage(m_TAGID_UserCommand, metadata.UserCommands[i]);
				}
			}
		}

		if (StatusSectionConfig.RecordSystemErrors && metadata.SystemErrors != null)
		{
			for (int i = 0; i < Math.Min(16, metadata.SystemErrors.Count()); i++)
			{
				if (metadata.SystemErrors[i] != null)
				{
					if (metadata.SystemErrors[i].Length > 255)
						AdvLib.AdvFrameAddStatusTagMessage(m_TAGID_SystemError, metadata.SystemErrors[i].Substring(0, 255));
					else
						AdvLib.AdvFrameAddStatusTagMessage(m_TAGID_SystemError, metadata.SystemErrors[i]);
				}
			} 
		}

		AdvLib.AdvFrameAddImage(layoutIdForCurrentFramerate, pixels, ImageBitsPerPixel);	

		AdvLib.AdvEndFrame();			
	}
}
