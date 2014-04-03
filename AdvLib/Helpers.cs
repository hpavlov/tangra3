/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public enum AlmanacStatus
{
	Uncertain = 0,
	Good = 1,
	Certain = 2
}

public enum FixStatus
{
	NoFix = 0,
	InternalTimeKeeping = 1,
	GFix = 2,
	PFix = 3
}

public enum AdvImageData
{
	PixelDepth16Bit,
	PixelDepth8Bit
}

/// <summary>
/// Represents the ADV system time which is the number of milliseconds elapsed since: 1 Jan 2010, 00:00:00 GMT
/// </summary>
public struct AdvTimeStamp
{
	public const long ADV_EPOCH_ZERO_TICKS = 633979008000000000;

	public long MillisecondsAfterAdvZeroEpoch;

	public static AdvTimeStamp FromWindowsTicks(long windowsTicks)
	{
		return new AdvTimeStamp()
		{
			MillisecondsAfterAdvZeroEpoch = (windowsTicks - ADV_EPOCH_ZERO_TICKS) / 10000
		};
	}

	public static AdvTimeStamp FromDateTime(DateTime dateTime)
	{
		return AdvTimeStamp.FromWindowsTicks(dateTime.Ticks);
	}

	public static AdvTimeStamp FromDateTime(int year, int month, int day, int hours, int minutes, int seconds, int milliseconds)
	{
		return AdvTimeStamp.FromWindowsTicks(new DateTime(year, month, day, hours, minutes, seconds, milliseconds).Ticks);
	}
}

public class AdvStatusSectionConfig
{
	public bool RecordSystemTime { get; set; }
	public bool RecordGPSTrackedSatellites { get; set; }
	public bool RecordGPSAlmanacStatus { get; set; }
	public bool RecordGPSAlmanacOffset { get; set; }
	public bool RecordGPSFixStatus { get; set; }
	public bool RecordGain { get; set; }
	public bool RecordShutter { get; set; }
	public bool RecordCameraOffset { get; set; }
	public bool RecordGamma { get; set; }
	public bool RecordVideoCameraFrameId { get; set; }
	public bool RecordUserCommands { get; set; }
	public bool RecordSystemErrors { get; set; }

	internal Dictionary<string, AdvTagType> AdditionalStatusTags = new Dictionary<string, AdvTagType>(); 

	public int AddDefineTag(string tagName, AdvTagType tagType)
	{
		if (AdditionalStatusTags.ContainsKey(tagName))
			throw new ArgumentException("This tag name as been already added.");

		AdditionalStatusTags.Add(tagName, tagType);

		return AdditionalStatusTags.Count - 1;
	}
}

public class AdvStatusEntry
{
	/// <summary>
	/// Lower accuracy system timestamp for the frame. Could be used as a backup time reference in case of a problem with the main timing hardware.
	/// </summary>
	public AdvTimeStamp SystemTime { get; set; }

	/// <summary>
	/// Number of tracked GPS satellites
	/// </summary>
	public byte GPSTrackedSatellites { get; set; }

	/// <summary>
	/// The status of the GPS almanac update 
	/// </summary>
	public AlmanacStatus GPSAlmanacStatus { get; set; }

	/// <summary>
	/// The almanac offset in seconds that was added to the uncorrected time reported by the GPS in order to compute the UTC time
	/// </summary>
	public byte GPSAlmanacOffset { get; set; }

	/// <summary>
	/// The status of the GPS fix
	/// </summary>
	public FixStatus GPSFixStatus { get; set; }

	/// <summary>
	/// The gain of the camera in dB
	/// </summary>
	public float Gain { get; set; }

	/// <summary>
	/// Camera shutter speed in seconds
	/// </summary>
	public float Shutter { get; set; }

	/// <summary>
	/// The gamma correction applied to the produced image
	/// </summary>
	public float Gamma { get; set; }

	/// <summary>
	/// The offset in percentage applied to the produced image
	/// </summary>
	public float CameraOffset { get; set; }

	/// <summary>
	/// The id of the frame as labeled by the camera frame counter
	/// </summary>
	public ulong VideoCameraFrameId { get; set; }

	/// <summary>
	/// The user commands executed since the last recorded frame. Up to 16 lines, each line up to 255 characters.
	/// </summary>
	public string[] UserCommands { get; set; }

	/// <summary>
	/// System errors detected since the last recorded frame. Up to 16 lines, each line up to 255 characters.
	/// </summary>
	public string[] SystemErrors { get; set; }

	/// <summary>
	/// The values of the additional tags. The value types must correspond to the defined tag type. Only the following
	/// .NET types are supported: byte, ushort, uint, ulong, float, string and string[]
	/// </summary>
	public object[] AdditionalStatusTags;
}
