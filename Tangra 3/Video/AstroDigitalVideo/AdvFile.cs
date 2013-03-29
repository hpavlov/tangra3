using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Tangra.Helpers;

namespace Tangra.Video.AstroDigitalVideo
{
    public class AdvFile
    {
        public Dictionary<string, string> AdvFileTags = new Dictionary<string, string>();

        private List<IAdvDataSection> m_Sections = new List<IAdvDataSection>();

    	public static DateTime ADV_ZERO_DATE_REF = new DateTime(2010, 1, 1, 0, 0, 0, 0);

        private AdvFramesIndex m_Index;

        private FileStream m_InputFile;
        private BinaryReader m_FileReader;
    	private string m_FileName;

        private string m_Compression;
    	private string m_DataLayout;
        private uint m_NumberOfFrames = 0;
        private uint m_Magic;
    	private long m_RecoveryOffset = 0;

    	private bool m_IsCorrupted;


		private AdvFile()
		{ }

    	public bool IsCorrupted
    	{
    		get { return m_IsCorrupted; }
    	}

		private void InitialisePropertiesFromTags()
		{
			string propVal;
			if (AdvFileTags.TryGetValue(AdvKeywords.KEY_DATAFRAME_COMPRESSION, out propVal))
				m_Compression = propVal;

			if (AdvFileTags.TryGetValue(AdvKeywords.KEY_DATA_LAYOUT, out propVal))
				m_DataLayout = propVal;							
		}

	    public static AdvFile OpenFile(string fileName)
        {
        	FileStream inputFile = new FileStream(fileName, FileMode.Open, FileAccess.Read);
        	BinaryReader fileReader = new BinaryReader(inputFile);
        	
			uint magic = fileReader.ReadUInt32();

			if (magic != 0x46545346)
				throw new FormatException("Unknown or unsupported version of an FSTF file");

			byte dataFormatVer = fileReader.ReadByte();

			uint numFrames = fileReader.ReadUInt32();
			long indexTableOffset = fileReader.ReadInt64();
			long metadataSystemTableOffset = fileReader.ReadInt64();
			long metadataUserTableOffset = fileReader.ReadInt64();

			AdvFile rv = new AdvFile();

			rv.m_InputFile = inputFile;
			rv.m_FileReader = fileReader;
			rv.m_FileName = fileName;

			if (dataFormatVer >= 1)
			{
				byte sectionsCount = fileReader.ReadByte();

				var sectionDefs = new Dictionary<ulong, string>();

				for (int i = 0; i < sectionsCount; i++)
				{
					string sectionType = fileReader.ReadAsciiString256();
					ulong sectionHeaderOffset = fileReader.ReadUInt64();
					sectionDefs.Add(sectionHeaderOffset, sectionType);
				}

				rv.m_NumberOfFrames = numFrames;
				foreach (uint offset in sectionDefs.Keys)
				{
					inputFile.Seek(offset, SeekOrigin.Begin);
					IAdvDataSection section = AdvSectionFactory.ConstructSection(sectionDefs[offset], fileReader);
					rv.AddDataSection(section);
				}

				fileReader.BaseStream.Seek(metadataSystemTableOffset, SeekOrigin.Begin);
				#region read metadata

				uint propsCount = fileReader.ReadUInt32();
				for (int i = 0; i < propsCount; i++)
				{
					string propName = fileReader.ReadAsciiString256();
					string propValue = fileReader.ReadAsciiString256();
					rv.AdvFileTags[propName] = propValue;
				}
				#endregion

				rv.m_RecoveryOffset = fileReader.BaseStream.Position;				

				if (indexTableOffset > fileReader.BaseStream.Length || metadataUserTableOffset > fileReader.BaseStream.Length ||
					indexTableOffset == 0 || metadataUserTableOffset == 0)
				{
					rv.m_IsCorrupted = true;
					return rv;
				}

				inputFile.Seek(indexTableOffset, SeekOrigin.Begin);
			}

			rv.m_Magic = magic;

			try
			{
				rv.m_Index = new AdvFramesIndex(fileReader);
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex);
				rv.m_IsCorrupted = true;
				return rv;
			}
			
			rv.InitialisePropertiesFromTags();

			return rv;            	
        }

		public void GetFrameImageSectionHeader(int frameNo, out byte layoutId, out AdvImageLayout.GetByteMode byteMode)
		{
			AdvFramesIndexEntry idxEntry = m_Index.Index[frameNo];
			m_InputFile.Seek(idxEntry.Offset, SeekOrigin.Begin);

			uint frameDataMagic = m_FileReader.ReadUInt32();
			Trace.Assert(frameDataMagic == 0xEE0122FF);

			// 8 bytes timestamp + 4 bytes exposure + 4 bytes image section length
			m_InputFile.Seek(16, SeekOrigin.Current);

			layoutId = m_FileReader.ReadByte();
			byteMode = (AdvImageLayout.GetByteMode)m_FileReader.ReadByte();
		}

    	public object[] GetFrameSectionData(int frameNo, ushort[,] prevFrame)
        {
            AdvFramesIndexEntry idxEntry = m_Index.Index[frameNo];
            m_InputFile.Seek(idxEntry.Offset, SeekOrigin.Begin);

			uint frameDataMagic = m_FileReader.ReadUInt32();
			Trace.Assert(frameDataMagic == 0xEE0122FF);
            
            byte[] data = m_FileReader.ReadBytes((int)idxEntry.Length);

			// Read the timestamp and exposure 
			long frameTimeMsSince2010 =
				 (long)data[0] + (((long)data[1]) << 8) + (((long)data[2]) << 16) + (((long)data[3]) << 24) +
				(((long)data[4]) << 32) + (((long)data[5]) << 40) + (((long)data[6]) << 48) + (((long)data[7]) << 56);
    		int exposure = data[8] + (data[9] << 8) + (data[10] << 16) + (data[11] << 24);

            var sectionObjects = new List<object>();
            int dataOffset = 12;
            foreach(IAdvDataSection section in m_Sections)
            {
				int sectionDataLength = data[dataOffset] + (data[dataOffset + 1] << 8) + (data[dataOffset + 2] << 16) + (data[dataOffset + 3] << 24);
            	
				object sectionObject = section.GetDataFromDataBytes(data, prevFrame, sectionDataLength, dataOffset + 4);
                sectionObjects.Add(sectionObject);

				dataOffset += sectionDataLength + 4;
            }

			// ADV Format Specification v1.5, the time stamp is the MIDDLE of the exposure (this changed from spec v1.4 where it was the START) of the exposure
			try
			{
			    ((AdvImageData)sectionObjects[0]).MidExposureUtc = ADV_ZERO_DATE_REF.AddMilliseconds(frameTimeMsSince2010);
			}
            catch(ArgumentOutOfRangeException)
            {
                ((AdvImageData) sectionObjects[0]).MidExposureUtc = ADV_ZERO_DATE_REF; 
            }
            
			((AdvImageData)sectionObjects[0]).ExposureMilliseconds = (float)(exposure / 10.0);

            return sectionObjects.ToArray();
        }

        public void AddDataSection(IAdvDataSection section)
        {
            m_Sections.Add(section);
        }

    	public AdvFramesIndex Index
    	{
    		get { return m_Index; }
    	}

        public AdvImageSection ImageSection
        {
            get { return (AdvImageSection)m_Sections.FirstOrDefault(s => s.SectionType == AdvSectionTypes.SECTION_IMAGE); }
        }

		public AdvStatusSection StatusSection
		{
			get { return (AdvStatusSection)m_Sections.FirstOrDefault(s => s.SectionType == AdvSectionTypes.SECTION_SYSTEM_STATUS); }
		}

        public uint NumberOfFrames
        {
            get { return m_NumberOfFrames; }
        }

        public string Compression
        {
            get { return m_Compression; }
        }

    	public string DataLayout
    	{
			get { return m_DataLayout; }
    	}

        public string Signature
        {
            get
            {
                if (m_Magic == 0x31564441)
                    return "ADV"; // ADV1 is returned as 'ADV'
                else
                    return "ADV?";
            }
        }

        public void Close()
        {
            m_FileReader.Close();
            m_InputFile.Close();
        }

		private class RecoveredFrameHeader
		{
			public long Position;
			public DateTime StartExposureUT;
			public double ExposureMilliseconds;
		}

		private List<RecoveredFrameHeader> m_CandidateFrameOffsets = new List<RecoveredFrameHeader>(); 

    	internal delegate void OnSearchProgressDelegate(int percentDone, int argument);

		internal void SearchFramesByMagicPhase1(OnSearchProgressDelegate progressCallback)
		{
			long bytesToSearch = m_InputFile.Length - 5 - m_RecoveryOffset;

			m_CandidateFrameOffsets.Clear();

			int percentCompleted;
			int lastPercentCompleted = -1;

			int MIN_FRAME_SIZE = 12 + 4;
			
			for (long i = 0; i < bytesToSearch; i++)
			{
				m_InputFile.Seek(m_RecoveryOffset + i, SeekOrigin.Begin);

				uint frameDataMagic = m_FileReader.ReadUInt32();
				if(frameDataMagic == 0xEE0122FF)
				{					
					try
					{
						byte[] data = m_FileReader.ReadBytes(MIN_FRAME_SIZE);

						// Read the timestamp and exposure 
						long frameTimeMsSince2010 =
							 (long)data[0] + (((long)data[1]) << 8) + (((long)data[2]) << 16) + (((long)data[3]) << 24) +
							(((long)data[4]) << 32) + (((long)data[5]) << 40) + (((long)data[6]) << 48) + (((long)data[7]) << 56); 
						int exposure = data[8] + (data[9] << 8) + (data[10] << 16) + (data[11] << 24);

                        DateTime startExposure = ADV_ZERO_DATE_REF.AddMilliseconds(frameTimeMsSince2010);
						double exposureMS = exposure / 10.0;

						if (startExposure.Year < 2010 || startExposure.Year > 2100)
							continue;
						if (exposureMS < 1 || exposureMS > 60 * 1000)
							continue;

						m_CandidateFrameOffsets.Add(new RecoveredFrameHeader()
						{
							Position = m_RecoveryOffset + i,
							StartExposureUT = startExposure,
							ExposureMilliseconds = exposureMS
						});							

						int sectionDataLength = data[12] + (data[13] << 8) + (data[14] << 16) + (data[15] << 24);

						m_InputFile.Seek(m_RecoveryOffset + i + 4 + 12 + sectionDataLength + 4, SeekOrigin.Begin);

						byte tagValuesCount = m_FileReader.ReadByte();

						for (int j = 0; j < tagValuesCount; j++)
						{
							m_FileReader.ReadByte();
							byte len = m_FileReader.ReadByte();
							m_FileReader.ReadBytes(len);
						}

						// NOTE: The next frame should begin somewhere in the next 32 bytes
						long nextFrameSearchStartPos = m_FileReader.BaseStream.Position;

						for (int j = 0; j < 32; j++)
						{
							m_InputFile.Seek(nextFrameSearchStartPos + j, SeekOrigin.Begin);

							uint frameDataMagic2 = m_FileReader.ReadUInt32();
							if (frameDataMagic2 == 0xEE0122FF)
							{
								// We found it!
								i = nextFrameSearchStartPos + j - m_RecoveryOffset - 1;
								break;
							}
						}
					}
					catch { }
				}

				percentCompleted = (int)(100.0 * i / bytesToSearch);
				if (lastPercentCompleted < percentCompleted)
				{
					progressCallback(percentCompleted, m_CandidateFrameOffsets.Count);
					lastPercentCompleted = percentCompleted;
				}
			}
		}

		internal void SearchFramesByMagicPhase2(OnSearchProgressDelegate progressCallback)
		{
			m_Index = new AdvFramesIndex();

			int percentCompleted;
			int lastPercentCompleted = -1;
			int framesRecovered = 0;
			long startTimeTicks = -1;

			m_CandidateFrameOffsets.Add(new RecoveredFrameHeader()
			{
				Position = m_FileReader.BaseStream.Length
			});

			ushort[,] prevFramePixels = null;

			for(int i = 0; i < m_CandidateFrameOffsets.Count - 1; i ++)
			{
				RecoveredFrameHeader frameInfo1 = m_CandidateFrameOffsets[i];
				RecoveredFrameHeader frameInfo2 = m_CandidateFrameOffsets[i + 1];

				if (startTimeTicks == -1)
					startTimeTicks = frameInfo1.StartExposureUT.Ticks;

				m_Index.Index.Add(new AdvFramesIndexEntry()
				{
					ElapsedTime = new TimeSpan(frameInfo1.StartExposureUT.Ticks - startTimeTicks),
					Offset = frameInfo1.Position,
					Length = (uint)(frameInfo2.Position - frameInfo1.Position)
				});

				try
				{
					object[] data = GetFrameSectionData(framesRecovered, prevFramePixels);
					AdvImageData imageData = (AdvImageData)data[0];
					AdvStatusData statusData = (AdvStatusData)data[1];
					int frameWidth = imageData.ImageData.GetLength(0);
					int frameHeight = imageData.ImageData.GetLength(1);
					Trace.WriteLine(string.Format("Recovered frame #{0} with LayoutId {1} and {2} status tags. Dimentions: {3}x{4} pixels", framesRecovered, imageData.LayoutId, statusData.TagValues.Count, frameWidth, frameHeight));

					// NOTE: Doesn't have to be right, just need something so we read the pixels
					prevFramePixels = imageData.ImageData;

					framesRecovered++;
				}
				catch(Exception)
				{
					m_Index.Index.RemoveAt(m_Index.Index.Count - 1);
				}

				percentCompleted = (int)(100.0 * i / (m_CandidateFrameOffsets.Count - 1));
				if (lastPercentCompleted < percentCompleted)
				{
					progressCallback(percentCompleted, framesRecovered);
					lastPercentCompleted = percentCompleted;
				}
			}
		}

		internal void SaveRecoveredFileAs(string newFileName)
		{
			string oldFile = Path.GetFullPath(m_FileName);
			string newFile = Path.GetFullPath(newFileName);

			if (string.Compare(oldFile, newFile, StringComparison.InvariantCultureIgnoreCase) != 0)
			{
				File.Copy(oldFile, newFile, true);
			}

			using (FileStream fs = new FileStream(newFile, FileMode.Open, FileAccess.Write))
			using(BinaryWriter writer = new BinaryWriter(fs))
			{
				uint numFrames = m_Index.NumberOfFrames;
				long indexTablePosition = fs.Length;

				fs.Seek(0, SeekOrigin.End);
				
				writer.Write(numFrames);

				for (int i = 0; i < numFrames; i++)
				{
					AdvFramesIndexEntry entry = m_Index.Index[i];

					uint elapsedMiliseconds = (uint)entry.ElapsedTime.TotalMilliseconds;
					long offset = entry.Offset;
					uint length = entry.Length;

					writer.Write(elapsedMiliseconds);
					writer.Write(offset);
					writer.Write(length);
				}

				long userMetadataTablePosition = fs.Position;
				uint numUserEntries = 0;
				writer.Write(numUserEntries);

				fs.Seek(5, SeekOrigin.Begin);

				writer.Write(numFrames);
				writer.Write(indexTablePosition);

				fs.Seek(8, SeekOrigin.Current);

				writer.Write(userMetadataTablePosition);

				fs.Flush();
			}
		}

	    internal bool CropAdvFile(string fileName, int firstFrame, int lastFrame)
	    {
			
		    return false;
	    }
    }
}
