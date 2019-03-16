using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
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
    }

    public class AavTimeAnalyser
    {
        private AstroDigitalVideoStream m_Aav;
        private FileStream m_File;
        private BinaryReader m_Reader;

        public List<TimeAnalyserEntry> Entries = new List<TimeAnalyserEntry>();
        public List<TimeAnalyserEntry> DebugFrames = new List<TimeAnalyserEntry>();

        public float MinDeltaMs;

        public float MaxDeltaMs;

        public DateTime FromDateTime;

        public DateTime ToDateTime;

        public AavTimeAnalyser(AstroDigitalVideoStream aav)
        {
            m_Aav = aav;
        }

        public void Initialize(Action<int, int> progressCallback)
        {
            Task.Run(() =>
            {
                progressCallback(0, m_Aav.CountFrames);

                var aavFile = AdvFile.OpenFile(m_Aav.FileName);
                aavFile.Close();

                var SYSTEM_TIME_TAG = aavFile.StatusSection.TagDefinitions.SingleOrDefault(x => x.Name == "SystemTime");
                var SYSTEM_TIME_FT_TAG = aavFile.StatusSection.TagDefinitions.SingleOrDefault(x => x.Name == "SystemTimeFileTime");
                var NTP_TIME_TAG = aavFile.StatusSection.TagDefinitions.SingleOrDefault(x => x.Name == "NTPEndTimestamp");
                var NTP_ERROR_TAG = aavFile.StatusSection.TagDefinitions.SingleOrDefault(x => x.Name == "NTPTimestampError");
                var OCR_DEBUG_1_TAG = aavFile.StatusSection.TagDefinitions.SingleOrDefault(x => x.Name == "StartFrameTimestamp");
                var OCR_DEBUG_2_TAG = aavFile.StatusSection.TagDefinitions.SingleOrDefault(x => x.Name == "EndFrameTimestamp");

                float maxDeltaSys = 0;
                float minDeltaSys = 0;
                float maxDeltaSysF = 0;
                float minDeltaSysF = 0;
                float maxDeltaNtp = 0;
                float minDeltaNtp = 0;
                float maxNtpError = 0;
                float minNtpError = 0;

                using (FileStream file = new FileStream(m_Aav.FileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (BinaryReader reader = new BinaryReader(file))
                {
                    int i = 0;
                    string tagVal;

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

                        entry.ExposureMs = (float)(exposure / 10.0);
                        entry.ReferenceTime = AdvFile.ADV_ZERO_DATE_REF.AddMilliseconds(frameTimeMsSince2010).AddMilliseconds(-1 * entry.ExposureMs / 2);
                        long referenceTimeTicks = entry.ReferenceTime.Ticks;

                        if (SYSTEM_TIME_TAG != null && statusSection.TagValues.TryGetValue(SYSTEM_TIME_TAG, out tagVal))
                        {
                            entry.SystemTime = AdvFile.ADV_ZERO_DATE_REF.AddMilliseconds(long.Parse(tagVal));
                            // TODO: Detect Date Change and factor into Delta
                            entry.DeltaSystemTimeMs = (float)new TimeSpan(entry.SystemTime.Ticks - referenceTimeTicks).TotalMilliseconds;
                        }
                        if (SYSTEM_TIME_FT_TAG != null && statusSection.TagValues.TryGetValue(SYSTEM_TIME_FT_TAG, out tagVal))
                        {
                            entry.SystemTimeFileTime = DateTime.FromFileTime(long.Parse(tagVal));
                            // TODO: Detect Date Change and factor into Delta
                            entry.DeltaSystemFileTimeMs = (float)new TimeSpan(entry.SystemTimeFileTime.Ticks - referenceTimeTicks).TotalMilliseconds;
                        }
                        if (NTP_TIME_TAG != null && statusSection.TagValues.TryGetValue(NTP_TIME_TAG, out tagVal))
                        {
                            entry.NTPTime = AdvFile.ADV_ZERO_DATE_REF.AddMilliseconds(long.Parse(tagVal));
                            // TODO: Detect Date Change and factor into Delta
                            entry.DeltaNTPTimeMs = (float)new TimeSpan(entry.NTPTime.Ticks - referenceTimeTicks).TotalMilliseconds;
                        }
                        if (NTP_ERROR_TAG != null && statusSection.TagValues.TryGetValue(NTP_ERROR_TAG, out tagVal))
                        {
                            entry.NTPErrorMs = int.Parse(tagVal) / 2;
                        }

                        Entries.Add(entry);

                        if (imageData != null)
                        {
                            entry.DebugImage = BitmapFilter.ToVideoFields(Pixelmap.ConstructBitmapFromBitmapPixels(imageData.ImageData));

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
                                    Trace.WriteLine("Outlier: " + entry.DeltaSystemTimeMs);
                                }
                            }
                            if (entry.DeltaSystemTimeMs < minDeltaSys)
                            {
                                if (entry.DeltaSystemTimeMs > minOutlier)
                                    minDeltaSys = entry.DeltaSystemTimeMs;
                                else
                                {
                                    entry.IsOutlier = true;
                                    Trace.WriteLine("Outlier: " + entry.DeltaSystemTimeMs);
                                }
                            }
                            if (entry.DeltaSystemFileTimeMs > maxDeltaSysF)
                            {
                                if (entry.DeltaSystemFileTimeMs < maxOutlier)
                                    maxDeltaSysF = entry.DeltaSystemFileTimeMs;
                                else
                                {
                                    entry.IsOutlier = true;
                                    Trace.WriteLine("Outlier: " + entry.DeltaSystemFileTimeMs);
                                }
                            }
                            if (entry.DeltaSystemFileTimeMs < minDeltaSysF)
                            {
                                if (entry.DeltaSystemFileTimeMs > minOutlier)
                                    minDeltaSysF = entry.DeltaSystemFileTimeMs;
                                else
                                {
                                    entry.IsOutlier = true;
                                    Trace.WriteLine("Outlier: " + entry.DeltaSystemFileTimeMs);
                                }
                            }
                            if (entry.DeltaNTPTimeMs > maxDeltaNtp)
                            {
                                if (entry.DeltaNTPTimeMs < maxOutlier)
                                    maxDeltaNtp = entry.DeltaNTPTimeMs;
                                else
                                {
                                    entry.IsOutlier = true;
                                    Trace.WriteLine("Outlier: " + entry.DeltaNTPTimeMs);
                                }
                            }
                            if (entry.DeltaNTPTimeMs < minDeltaNtp)
                            {
                                if (entry.DeltaNTPTimeMs > minOutlier)
                                    minDeltaNtp = entry.DeltaNTPTimeMs;
                                else
                                {
                                    entry.IsOutlier = true;
                                    Trace.WriteLine("Outlier: " + entry.DeltaNTPTimeMs);
                                }
                            }
                            if (entry.NTPErrorMs > maxNtpError)
                            {
                                if (entry.NTPErrorMs < maxOutlier)
                                    maxNtpError = entry.NTPErrorMs;
                                else
                                {
                                    entry.IsOutlier = true;
                                    Trace.WriteLine("Outlier: " + entry.NTPErrorMs);
                                }
                            }
                            if (-entry.NTPErrorMs < minNtpError)
                            {
                                if (-entry.NTPErrorMs > minOutlier)
                                    minNtpError = -entry.NTPErrorMs;
                                else
                                {
                                    entry.IsOutlier = true;
                                    Trace.WriteLine("Outlier: " + -entry.NTPErrorMs);
                                }
                            }
                        }

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

                MinDeltaMs = Math.Min(Math.Min(Math.Min(minDeltaSys, minDeltaSysF), minDeltaNtp), minNtpError);
                MaxDeltaMs = Math.Max(Math.Max(Math.Max(maxDeltaSys, maxDeltaSysF), maxDeltaNtp), maxNtpError);

                if (Entries.Count > 0)
                {
                    FromDateTime = Entries.First().ReferenceTime;
                    ToDateTime = Entries.Last().ReferenceTime;
                }

                progressCallback(0, 0);
            });
        }
    }
}
