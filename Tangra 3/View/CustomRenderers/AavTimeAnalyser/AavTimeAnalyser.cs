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
    }

    public class AavTimeAnalyser
    {
        private AstroDigitalVideoStream m_Aav;
        private FileStream m_File;
        private BinaryReader m_Reader;

        private List<TimeAnalyserEntry> m_Entries = new List<TimeAnalyserEntry>();
        private List<TimeAnalyserEntry> m_DebugFrames = new List<TimeAnalyserEntry>();

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


                        if (SYSTEM_TIME_TAG != null && statusSection.TagValues.TryGetValue(SYSTEM_TIME_TAG, out tagVal))
                        {
                            entry.SystemTime = AdvFile.ADV_ZERO_DATE_REF.AddMilliseconds(long.Parse(tagVal));
                        }
                        if (SYSTEM_TIME_FT_TAG != null && statusSection.TagValues.TryGetValue(SYSTEM_TIME_FT_TAG, out tagVal))
                        {
                            entry.SystemTimeFileTime = DateTime.FromFileTime(long.Parse(tagVal));
                        }
                        if (NTP_TIME_TAG != null && statusSection.TagValues.TryGetValue(NTP_TIME_TAG, out tagVal))
                        {
                            entry.NTPTime = AdvFile.ADV_ZERO_DATE_REF.AddMilliseconds(long.Parse(tagVal));
                        }
                        if (NTP_ERROR_TAG != null && statusSection.TagValues.TryGetValue(NTP_ERROR_TAG, out tagVal))
                        {
                            entry.NTPErrorMs = int.Parse(tagVal) / 2;
                        }

                        m_Entries.Add(entry);

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

                            m_DebugFrames.Add(entry);
                        }

                        i++;
                        if (i % 100 == 0)
                        {
                            progressCallback(i - m_Aav.FirstFrame, m_Aav.CountFrames);

                        }
                    }
                }

                progressCallback(0, 0);
            });
        }
    }
}
