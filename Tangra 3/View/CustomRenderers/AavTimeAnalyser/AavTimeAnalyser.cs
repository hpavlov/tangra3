using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tangra.Model.Image;
using Tangra.PInvoke;
using Tangra.Video;
using Tangra.Video.AstroDigitalVideo;

namespace Tangra.View.CustomRenderers.AavTimeAnalyser
{
    public class TimeAnalyserEntry
    {
        public DateTime ReferenceTime;
        public DateTime SystemTime;
        public float ExposureMs;
        public DateTime SystemTimeFileTime;
        public DateTime NTPTime;
        public float NTPErrorMs;
        public string OrcField1;
        public string OrcField2;
        public Bitmap DebugImage;

        public float DeltaSystemTimeMs;
        public float DeltaSystemFileTimeMs;
        public float DeltaNTPTimeMs;
        public bool IsOutlier;

        public int FrameNo;
        public SystemUtilisationEntry UtilisationEntry;
    }

    public class SystemUtilisationEntry
    {
        public float CpuUtilisation;
        public float DiskUtilisation;
        public float FreeMemory;
    }

    public class NtpUpdateEntry
    {
        public bool Updated;
        public float Delta;
        public float Latency;
    }

    public class MeinbergAdvLogEntry
    {
        public DateTime Time;
        public float? Delay;
        public string PeerIP;
    }

    public class AavTimeAnalyser
    {
        private AstroDigitalVideoStream m_Aav;

        private int m_AppliedNtpTimeCorr = 0;
        private float m_FrameDurationMs = 40;

        // The NTP time has already an applied fixed correction before it is saved in the AAV file. The time we would like to plot is based on the NTP timestamp at the end of the frame
        // We subtract half a frame duration to get the expected mid-frame time. We do not correct for:
        //    A) The the unknown delay between the moment the frame has been received by the frame grabber and the moment the frame was made available to OccuRec for recording
        //    B) The elapsed time from the actual optical exposure to the moment the frame has been timestamped by IOTA-VTI
        private float m_CorrNtpTimeMs = 60 - 20; // By default assume PAL and 60ms default config in OccuRec for CAPHNTP-TIMING-CORRECTION

        // The system time recorded by OccuRec is the time when we received the frame. We correct it for half frame duration to make it the expected mid-frame time
        // This is something that any recording software could (and should) do. We do not correct for the unknown acqusition delay - (A) above
        private float m_CorrSystemTimeMs = -20; // By default assume PAL

        public List<TimeAnalyserEntry> Entries = new List<TimeAnalyserEntry>();
        public List<TimeAnalyserEntry> DebugFrames = new List<TimeAnalyserEntry>();
        public List<SystemUtilisationEntry>  SystemUtilisation = new List<SystemUtilisationEntry>();
        public List<NtpUpdateEntry> NtpUpdates = new List<NtpUpdateEntry>(); 
        public List<MeinbergAdvLogEntry> MeinbergAdvLog = new List<MeinbergAdvLogEntry>();

        public float MinDeltaNTPMs;
        public float MaxDeltaNTPMs;
        public float MinDeltaNTPErrorMs;
        public float MaxDeltaNTPErrorMs;
        public float MinDeltaSystemTimeMs;
        public float MaxDeltaSystemTimeMs;
        public float MinDeltaSystemFileTimeMs;
        public float MaxDeltaSystemFileTimeMs;

        public float MaxFreeMemoryMb;
        public float MinFreeMemoryMb;
        public float MaxDiskUsage;

        public DateTime FromDateTime;
        public DateTime ToDateTime;

        public AavTimeAnalyser(AstroDigitalVideoStream aav)
        {
            m_Aav = aav;

            float frameRate = 40;
            if (float.TryParse(m_Aav.GetFileTag("NATIVE-FRAME-RATE"), out frameRate))
            {
                m_FrameDurationMs = 1000.0f / frameRate;
            }

            int ntpAppliedCorr = 60;
            if (int.TryParse(m_Aav.GetFileTag("CAPHNTP-TIMING-CORRECTION"), out ntpAppliedCorr))
            {
                m_AppliedNtpTimeCorr = ntpAppliedCorr;
            }

            m_CorrNtpTimeMs = ntpAppliedCorr - frameRate / 2.0f;
            m_CorrSystemTimeMs = -frameRate / 2.0f;
        }

        public void Initialize(Action<int, int> progressCallback)
        {
            Task.Run(() =>
            {
                progressCallback(0, m_Aav.CountFrames);

                var aavFile = AdvFile.OpenFile(m_Aav.FileName);
                aavFile.Close();

                // This is the system timestamp retrieved by the system when the frame was saved to disk.
                var SYSTEM_TIME_TAG = aavFile.StatusSection.TagDefinitions.SingleOrDefault(x => x.Name == "SystemTime");

                // This is the system timestamp retrieved by the system when the frame was received.
                var SYSTEM_TIME_FT_TAG = aavFile.StatusSection.TagDefinitions.SingleOrDefault(x => x.Name == "SystemTimeFileTime");

                // This is the OCR-ed time with the full precision (0.1 ms for IOTA-VTI) rather than only the 1 ms from the AAVv1 timestamp
                var OCR_TIME_TAG = aavFile.StatusSection.TagDefinitions.SingleOrDefault(x => x.Name == "OcrTime");

                // This is the NTP timestamp retrieved by the system when the frame was received. It has been corrected for the configured 'Calibration Correction' in OccuRec
                var NTP_TIME_TAG = aavFile.StatusSection.TagDefinitions.SingleOrDefault(x => x.Name == "NTPEndTimestamp");

                var NTP_ERROR_TAG = aavFile.StatusSection.TagDefinitions.SingleOrDefault(x => x.Name == "NTPTimestampError");
                var OCR_DEBUG_1_TAG = aavFile.StatusSection.TagDefinitions.SingleOrDefault(x => x.Name == "StartFrameTimestamp");
                var OCR_DEBUG_2_TAG = aavFile.StatusSection.TagDefinitions.SingleOrDefault(x => x.Name == "EndFrameTimestamp");

                 var CPU_USAGE_TAG = aavFile.StatusSection.TagDefinitions.SingleOrDefault(x => x.Name == "CpuUtilisation");
                 var DISK_USAGE_TAG = aavFile.StatusSection.TagDefinitions.SingleOrDefault(x => x.Name == "DisksUtilisation");
                 var FREE_MEMORY_TAG = aavFile.StatusSection.TagDefinitions.SingleOrDefault(x => x.Name == "FreeMemoryMb");

                float maxDeltaSys = 0;
                float minDeltaSys = 0;
                float maxDeltaSysF = 0;
                float minDeltaSysF = 0;
                float maxDeltaNtp = 0;
                float minDeltaNtp = 0;
                float maxNtpError = 0;
                float minNtpError = 0;
                float maxFreeMemory = 0;
                float minFreeMemory = float.MaxValue;
                float maxDiskUsage = 0;

                using (FileStream file = new FileStream(m_Aav.FileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (BinaryReader reader = new BinaryReader(file))
                {
                    int i = 0;
                    string tagVal;
                    List<TimeAnalyserEntry> entriesWithNoUtil = new List<TimeAnalyserEntry>();

                    foreach (var idx in aavFile.Index.Index)
                    {
                        file.Seek(idx.Offset, SeekOrigin.Begin);

                        uint frameDataMagic = reader.ReadUInt32();
                        Trace.Assert(frameDataMagic == 0xEE0122FF);

                        byte[] data = reader.ReadBytes((int)idx.Length);

                        // Read the timestamp and exposure 
                        long frameTimeMsSince2010 =
                             (long)data[0] + (((long)data[1]) << 8) + (((long)data[2]) << 16) + (((long)data[3]) << 24) +
                            (((long)data[4]) << 32) + (((long)data[5]) << 40) + (((long)data[6]) << 48) + (((long)data[7]) << 56);
                        int exposure = data[8] + (data[9] << 8) + (data[10] << 16) + (data[11] << 24);

                        if (frameTimeMsSince2010 == 0)
                        {
                            // First or last AAV frame
                            continue;
                        }

                        var entry = new TimeAnalyserEntry();
                        int dataOffset = 12;
                        AdvImageData imageData = null;
                        int sectionDataLength = data[dataOffset] + (data[dataOffset + 1] << 8) + (data[dataOffset + 2] << 16) + (data[dataOffset + 3] << 24);
                        if (sectionDataLength > 2)
                        {
                            imageData = (AdvImageData)aavFile.ImageSection.GetDataFromDataBytes(data, null, sectionDataLength, dataOffset + 4);
                        }
                        dataOffset += sectionDataLength + 4;

                        sectionDataLength = data[dataOffset] + (data[dataOffset + 1] << 8) + (data[dataOffset + 2] << 16) + (data[dataOffset + 3] << 24);
                        AdvStatusData statusSection = (AdvStatusData)aavFile.StatusSection.GetDataFromDataBytes(data, null, sectionDataLength, dataOffset + 4);

                        entry.FrameNo = i;
                        entry.ExposureMs = (float)(exposure / 10.0);

                        if (OCR_TIME_TAG != null && statusSection.TagValues.TryGetValue(OCR_TIME_TAG, out tagVal))
                        {
                            entry.ReferenceTime = new DateTime(long.Parse(tagVal));
                        }
                        else
                        {
                            entry.ReferenceTime = AdvFile.ADV_ZERO_DATE_REF.AddMilliseconds(frameTimeMsSince2010).AddMilliseconds(-1 * entry.ExposureMs / 2);
                        }

                        long referenceTimeTicks = entry.ReferenceTime.Ticks;

                        if (SYSTEM_TIME_TAG != null && statusSection.TagValues.TryGetValue(SYSTEM_TIME_TAG, out tagVal))
                        {
                            entry.SystemTime = AdvFile.ADV_ZERO_DATE_REF.AddMilliseconds(long.Parse(tagVal)).AddMilliseconds(m_CorrSystemTimeMs);
                            if (entry.SystemTime.Day + 1 == entry.ReferenceTime.Day)
                            {
                                // Fixing a date-change bug in OccuRec (var 1)
                                entry.ReferenceTime = entry.ReferenceTime.AddDays(-1);
                                referenceTimeTicks = entry.ReferenceTime.Ticks;
                            }
                            if (entry.SystemTime.Day == entry.ReferenceTime.Day && entry.ReferenceTime.Hour - entry.SystemTime.Hour == 23)
                            {
                                // Fixing a date-change bug in OccuRec (var 2)
                                entry.ReferenceTime = entry.ReferenceTime.AddDays(-1);
                                referenceTimeTicks = entry.ReferenceTime.Ticks;
                            }
                            entry.DeltaSystemTimeMs = (float)new TimeSpan(entry.SystemTime.Ticks - referenceTimeTicks).TotalMilliseconds;
                        }
                        if (SYSTEM_TIME_FT_TAG != null && statusSection.TagValues.TryGetValue(SYSTEM_TIME_FT_TAG, out tagVal))
                        {
                            // SystemTimeAsFileTime has a microsecond precision. However the IOTA-VTI reference time precision is only 0.1 ms
                            // so here we are rounding the SystemTimeAsFileTime value to the nearest 0.1 ms for correctness
                            entry.SystemTimeFileTime = new DateTime(1000 * (long.Parse(tagVal) / 1000)).AddMilliseconds(m_CorrSystemTimeMs);
                            entry.DeltaSystemFileTimeMs = (float)new TimeSpan(entry.SystemTimeFileTime.Ticks - referenceTimeTicks).TotalMilliseconds;
                        }
                        if (NTP_TIME_TAG != null && statusSection.TagValues.TryGetValue(NTP_TIME_TAG, out tagVal))
                        {
                            entry.NTPTime = AdvFile.ADV_ZERO_DATE_REF.AddMilliseconds(long.Parse(tagVal)).AddMilliseconds(m_CorrNtpTimeMs);
                            entry.DeltaNTPTimeMs = (float)new TimeSpan(entry.NTPTime.Ticks - referenceTimeTicks).TotalMilliseconds;
                        }
                        if (NTP_ERROR_TAG != null && statusSection.TagValues.TryGetValue(NTP_ERROR_TAG, out tagVal))
                        {
                            entry.NTPErrorMs = 3 * int.Parse(tagVal) / 10.0f; // Value recorded in 0.1MS, converted to MS and then taken as 3-Sigma
                        }

                        Entries.Add(entry);

                        if (imageData != null)
                        {
                            entry.DebugImage = BitmapFilter.ToVideoFields(Pixelmap.ConstructBitmapFromBitmapPixels(imageData.ImageData));
                            entry.IsOutlier = true;

                            if (OCR_DEBUG_1_TAG != null && statusSection.TagValues.TryGetValue(OCR_DEBUG_1_TAG, out tagVal))
                            {
                                entry.OrcField1 = tagVal;
                            }
                            if (OCR_DEBUG_2_TAG != null && statusSection.TagValues.TryGetValue(OCR_DEBUG_2_TAG, out tagVal))
                            {
                                entry.OrcField2 = tagVal;
                            }

                            DebugFrames.Add(entry);
                        }
                        else
                        {
                            int maxOutlier = 2000;
                            int minOutlier = -2000;

                            if (entry.DeltaSystemTimeMs > maxDeltaSys)
                            {
                                if (entry.DeltaSystemTimeMs < maxOutlier)
                                    maxDeltaSys = entry.DeltaSystemTimeMs;
                                else
                                {
                                    entry.IsOutlier = true;
                                    Trace.WriteLine("SystemTime Outlier: " + entry.DeltaSystemTimeMs);
                                }
                            }
                            if (entry.DeltaSystemTimeMs < minDeltaSys)
                            {
                                if (entry.DeltaSystemTimeMs > minOutlier)
                                    minDeltaSys = entry.DeltaSystemTimeMs;
                                else
                                {
                                    entry.IsOutlier = true;
                                    Trace.WriteLine("SystemTime Outlier: " + entry.DeltaSystemTimeMs);
                                }
                            }
                            if (entry.DeltaSystemFileTimeMs > maxDeltaSysF)
                            {
                                if (entry.DeltaSystemFileTimeMs < maxOutlier)
                                    maxDeltaSysF = entry.DeltaSystemFileTimeMs;
                                else
                                {
                                    entry.IsOutlier = true;
                                    Trace.WriteLine("SystemFileTime Outlier: " + entry.DeltaSystemFileTimeMs);
                                }
                            }
                            if (entry.DeltaSystemFileTimeMs < minDeltaSysF)
                            {
                                if (entry.DeltaSystemFileTimeMs > minOutlier)
                                    minDeltaSysF = entry.DeltaSystemFileTimeMs;
                                else
                                {
                                    entry.IsOutlier = true;
                                    Trace.WriteLine("SystemFileTime Outlier: " + entry.DeltaSystemFileTimeMs);
                                }
                            }
                            if (entry.DeltaNTPTimeMs > maxDeltaNtp)
                            {
                                if (entry.DeltaNTPTimeMs < maxOutlier)
                                    maxDeltaNtp = entry.DeltaNTPTimeMs;
                                else
                                {
                                    entry.IsOutlier = true;
                                    //Trace.WriteLine("NTPTime Outlier: " + entry.DeltaNTPTimeMs);
                                }
                            }
                            if (entry.DeltaNTPTimeMs < minDeltaNtp)
                            {
                                if (entry.DeltaNTPTimeMs > minOutlier)
                                    minDeltaNtp = entry.DeltaNTPTimeMs;
                                else
                                {
                                    entry.IsOutlier = true;
                                    //Trace.WriteLine("NTPTime Outlier: " + entry.DeltaNTPTimeMs);
                                }
                            }
                            if (entry.NTPErrorMs > maxNtpError)
                            {
                                if (entry.NTPErrorMs < maxOutlier)
                                    maxNtpError = entry.NTPErrorMs;
                                else
                                {
                                    entry.IsOutlier = true;
                                    //Trace.WriteLine("NtpError Outlier: " + entry.NTPErrorMs);
                                }
                            }
                            if (-entry.NTPErrorMs < minNtpError)
                            {
                                if (-entry.NTPErrorMs > minOutlier)
                                    minNtpError = -entry.NTPErrorMs;
                                else
                                {
                                    entry.IsOutlier = true;
                                    //Trace.WriteLine("NtpError Outlier: " + -entry.NTPErrorMs);
                                }
                            }
                        }

                        if (CPU_USAGE_TAG != null && statusSection.TagValues.TryGetValue(CPU_USAGE_TAG, out tagVal))
                        {
                            float cpuUsage = float.Parse(tagVal, CultureInfo.InvariantCulture);
                            float diskUsage = 0;
                            float freeMemory = 0;
                            if (DISK_USAGE_TAG != null && statusSection.TagValues.TryGetValue(DISK_USAGE_TAG, out tagVal))
                            {
                                diskUsage = float.Parse(tagVal, CultureInfo.InvariantCulture);
                                if (maxDiskUsage < diskUsage) maxDiskUsage = diskUsage;
                            }

                            if (FREE_MEMORY_TAG != null && statusSection.TagValues.TryGetValue(FREE_MEMORY_TAG, out tagVal))
                            {
                                freeMemory = float.Parse(tagVal, CultureInfo.InvariantCulture);
                                if (maxFreeMemory < freeMemory) maxFreeMemory = freeMemory;
                                if (minFreeMemory > freeMemory) minFreeMemory = freeMemory;
                            }
                            var currUtilisationEntry = new SystemUtilisationEntry() { CpuUtilisation = cpuUsage, DiskUtilisation = diskUsage, FreeMemory = freeMemory };
                            SystemUtilisation.Add(currUtilisationEntry);
                            entriesWithNoUtil.ForEach(x => x.UtilisationEntry = currUtilisationEntry);
                            entriesWithNoUtil.Clear();
                        }
                        entriesWithNoUtil.Add(entry);

                        i++;
                        if (i % 100 == 0)
                        {
                            progressCallback(i - m_Aav.FirstFrame, m_Aav.CountFrames);
                        }
                    }
                }

                Trace.WriteLine(string.Format("MinDeltaSys: {0:0.0}, MaxDeltaSys: {1:0.0}", minDeltaSys, maxDeltaSys));
                Trace.WriteLine(string.Format("MinDeltaSysF: {0:0.0}, MaxDeltaSysF: {1:0.0}", minDeltaSysF, maxDeltaSysF));
                Trace.WriteLine(string.Format("MinDeltaNtp: {0:0.0}, MaxDeltaNtp: {1:0.0}", minDeltaNtp, maxDeltaNtp));

                MinDeltaNTPMs = minDeltaNtp;
                MaxDeltaNTPMs = maxDeltaNtp;
                MinDeltaNTPErrorMs = minNtpError;
                MaxDeltaNTPErrorMs = maxNtpError;
                MinDeltaSystemTimeMs = minDeltaSys;
                MaxDeltaSystemTimeMs = maxDeltaSys;
                MinDeltaSystemFileTimeMs = minDeltaSysF;
                MaxDeltaSystemFileTimeMs = maxDeltaSysF;
                MinFreeMemoryMb = minFreeMemory;
                MaxFreeMemoryMb = maxFreeMemory;
                MaxDiskUsage = maxDiskUsage;

                if (Entries.Count > 0)
                {
                    FromDateTime = Entries.First().ReferenceTime;
                    ToDateTime = Entries.Last().ReferenceTime;
                }

                var logFileName = Path.ChangeExtension(m_Aav.FileName, ".log");
                if (File.Exists(logFileName))
                {
                    ExtractNTPLogData(logFileName);
                }

                var meinbergFileName = Path.GetFullPath(Path.GetDirectoryName(m_Aav.FileName) + @"\ntsmadvlog.txt");
                if (File.Exists(meinbergFileName))
                {
                    ExtractMeinbergLogData(meinbergFileName);
                }

                progressCallback(0, 0);
            });
        }

        Regex NtpRegex = new Regex("Time (\\*NOT\\* )?Updated:\\s*Delta = ([0-9\\.\\-]+).*Latency = ([0-9\\.\\-]+)", RegexOptions.Compiled);

        private void ExtractNTPLogData(string fileName)
        {
            var lines = File.ReadAllLines(fileName);
            foreach (var line in lines)
            {
                var match = NtpRegex.Match(line);
                if (match.Success)
                {
                    var updated = match.Groups[1].Value != "*NOT* ";
                    float delta = float.Parse(match.Groups[2].Value);
                    float latency = float.Parse(match.Groups[3].Value);
                    NtpUpdates.Add(new NtpUpdateEntry() { Delta = delta, Latency = latency, Updated = updated });
                }
            }
        }

        private void ExtractMeinbergLogData(string fileName)
        {
            MeinbergAdvLogEntry entry = null;

            var lines = File.ReadAllLines(fileName);
            foreach (var line in lines)
            {
                var tokens = line.Split(' ');
                if (tokens.Length == 4)
                {
                    DateTime dt;
                    if (DateTime.TryParseExact(tokens[0] + " " + tokens[1], "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out dt))
                    {
                        if (dt > FromDateTime && dt < ToDateTime)
                        {
                            if (entry == null || entry.Time != dt)
                            {
                                if (entry != null) MeinbergAdvLog.Add(entry);
                                entry = new MeinbergAdvLogEntry() { Time = dt };
                            }

                            int type;
                            if (int.TryParse(tokens[2], out type))
                            {
                                if (type == 1)
                                {
                                    // IP Address of Peer
                                    entry.PeerIP = tokens[3];
                                }
                                else if (type == 2)
                                {
                                    // Delay (ms)
                                    float delay = float.NaN;
                                    if (float.TryParse(tokens[3], NumberStyles.Float, CultureInfo.InvariantCulture, out delay))
                                    {
                                        entry.Delay = delay;
                                    }
                                }
                                else if (type == 3)
                                {
                                    // Polling interval
                                }
                                else if (type == 0)
                                {
                                    // local Stratum
                                }
                            }
                        }
                    }
                }
            }

            if (entry != null) MeinbergAdvLog.Add(entry);
        }

        public void ExportData(string fileName, Action<int, int> progressCallback)
        {
            Task.Run(() =>
            {
                progressCallback(0, Entries.Count);

                var output = new StringBuilder();
                output.AppendLine("Delta SystemTimePreciseAdFileTime (ms),Delta SystemTime(ms),Delta OccuRecTime (ms),3-Sigma NTP Error(ms)");

                int i = 0;
                foreach (var entry in Entries)
                {
                    output.AppendFormat(CultureInfo.InvariantCulture, "{0:0.000},{1:0.0},{2:0.000},{3:0.0}\r\n", entry.DeltaSystemFileTimeMs, entry.DeltaSystemTimeMs, entry.DeltaNTPTimeMs, entry.NTPErrorMs);

                    i++;
                    if (i % 100 == 0)
                    {
                        progressCallback(i, Entries.Count);
                    }
                }

                File.WriteAllText(fileName, output.ToString());

                progressCallback(0, 0);
                progressCallback(0, SystemUtilisation.Count);

                output = new StringBuilder();
                output.AppendLine("Cpu Utilisation (%), Disk Utilisation (%), Free Memory(Mb)");

                i = 0;
                foreach (var entry in SystemUtilisation)
                {
                    output.AppendFormat(CultureInfo.InvariantCulture, "{0:0.000},{1:0.0},{2:0.000}\r\n", entry.CpuUtilisation, entry.DiskUtilisation, entry.FreeMemory);

                    i++;
                    if (i % 100 == 0)
                    {
                        progressCallback(i, SystemUtilisation.Count);
                    }
                }

                File.WriteAllText("Utilisation_" + fileName, output.ToString());

                progressCallback(0, 0);
            });
        }
    }
}
