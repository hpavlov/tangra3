/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Helpers;
using Tangra.Model.Image;
using Tangra.PInvoke;

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
        private long m_UserMetadataTableOffset = 0;

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

			if (AdvFileTags.TryGetValue(AdvKeywords.KEY_ADV16_NORMVAL, out propVal))
				ImageSection.Adv16NormalisationValue = int.Parse(propVal);			
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

			fileReader.BaseStream.Seek(metadataUserTableOffset, SeekOrigin.Begin);
	        rv.m_UserMetadataTableOffset = metadataUserTableOffset;
			#region read metadata

			uint count = fileReader.ReadUInt32();
			for (int i = 0; i < count; i++)
			{
				string propName = fileReader.ReadAsciiString256();
				string propValue = fileReader.ReadAsciiString256();
				rv.AdvFileTags[propName] = propValue;
			}
			#endregion

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

			// ADV Format Specification v1.5 (and later), the time stamp is the MIDDLE of the exposure (this changed from spec v1.4 where it was the START) of the exposure
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

        private ushort[,] prevFramePixels;
        private int prevFrameNo = -1;

        internal Pixelmap GetFrameData(int index, out AdvImageData imageData, out AdvStatusData statusData, out Bitmap displayBitmap)
        {
            imageData = null;

            if (index < NumberOfFrames)
            {
                byte layoutId;
                AdvImageLayout.GetByteMode byteMode;

                GetFrameImageSectionHeader(index, out layoutId, out byteMode);

                AdvImageLayout layout = ImageSection.GetImageLayoutFromLayoutId(layoutId);

                if (layout.IsDiffCorrLayout && byteMode == AdvImageLayout.GetByteMode.DiffCorrBytes && prevFrameNo != index - 1)
                {
                    // Move back and find the nearest previous key frame
                    int keyFrameIdx = index;
                    do
                    {
                        keyFrameIdx--;
                        GetFrameImageSectionHeader(keyFrameIdx, out layoutId, out byteMode);
                    }
                    while (keyFrameIdx > 0 && byteMode != AdvImageLayout.GetByteMode.KeyFrameBytes);

                    object[] keyFrameData = GetFrameSectionData(keyFrameIdx, null);
                    prevFramePixels = ((AdvImageData)keyFrameData[0]).ImageData;

                    if (layout.DiffCorrFrame == DiffCorrFrameMode.PrevFrame)
                    {
                        for (int i = keyFrameIdx + 1; i < index; i++)
                        {
                            object[] frameData = GetFrameSectionData(i, prevFramePixels);

                            prevFramePixels = ((AdvImageData)frameData[0]).ImageData;
                        }
                    }
                }

                object[] data;

                data = GetFrameSectionData(index, prevFramePixels);

                imageData = (AdvImageData)data[0];
                statusData = (AdvStatusData)data[1];

                if (prevFramePixels == null)
                    prevFramePixels = new ushort[ImageSection.Width, ImageSection.Height];

                if (layout.IsDiffCorrLayout)
                {
                    for (int x = 0; x < ImageSection.Width; x++)
                        for (int y = 0; y < ImageSection.Height; y++)
                        {
                            prevFramePixels[x, y] = imageData.ImageData[x, y];
                        }
                }

                prevFrameNo = index;

                Pixelmap rv = ImageSection.CreatePixelmap(imageData);

                //Pixelmap rv = new Pixelmap((int)m_AdvFile.ImageSection.Width, (int)m_AdvFile.ImageSection.Height, m_AdvFile.ImageSection.BitsPerPixel, null, null, null);
                //rv.Width = (int) m_AdvFile.ImageSection.Width;
                //rv.Height = (int) m_AdvFile.ImageSection.Height;
                //rv.BitPixCamera = m_AdvFile.ImageSection.BitsPerPixel;
                //rv.Array = new uint[Width * Height];
                //rv.CopyPixelsFrom(imageData.ImageData, imageData.Bpp);
                //displayBitmap = PixelmapFactory.ConstructBitmapFrom12BitPixelmap(rv);

                displayBitmap = rv.DisplayBitmap;

                return rv;
            }
            else
            {
                displayBitmap = null;
                statusData = null;
                return null;
            }
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
						if (exposureMS < 0 || exposureMS > 60 * 1000)
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

		private void CopyRawBytes(BinaryReader reader, long offset, long length, BinaryWriter writer)
		{
			reader.BaseStream.Seek(offset, SeekOrigin.Begin);

			for (int i = 0; i < length; i++)
			{
				byte bt = reader.ReadByte();
				writer.Write(bt);
			}			
		}
        
        internal bool SaveAsAviFile(string fileName, int firstFrame, int lastFrame, bool tryCodec, double msPerFrame, double addedGamma, OnSearchProgressDelegate progressCallback)
        {
            TangraVideo.CloseAviFile();
            TangraVideo.StartNewAviFile(fileName, (int)ImageSection.Width, (int)ImageSection.Height, 8, 25, tryCodec);
			try
			{
			    int aviFrameNo = 0;
                AdvFramesIndexEntry firstFrameIdx = m_Index.Index[firstFrame];
			    double startingTimeMilliseconds = firstFrameIdx.ElapsedTime.TotalMilliseconds;
				bool isAavFile = AdvFileTags["FSTF-TYPE"] == "AAV";

				double effFrameDuration = double.NaN;
				try
				{
					effFrameDuration = 1000 / double.Parse(AdvFileTags["EFFECTIVE-FRAME-RATE"]);
				}
				catch
				{ }

				if (isAavFile && m_Index.Index[lastFrame].ElapsedTime.Ticks == 0 && double.IsNaN(effFrameDuration))
				{
					MessageBox.Show(
						"This AAV video format is too old and cannot be converted to AVI", 
						"Tangra 3", 
						MessageBoxButtons.OK, 
						MessageBoxIcon.Error);

					return false;
				}

				if ((isAavFile && !double.IsNaN(effFrameDuration)) ||
				    !isAavFile && m_Index.Index[lastFrame].ElapsedTime.Ticks != 0)
				{
					// Sampling can be done as we have sufficient timing information
				}
				else
				{
					MessageBox.Show(
						"There is insufficient timing information in this file to convert it to AVI. This could be caused by an old file format.",
						"Tangra 3",
						MessageBoxButtons.OK,
						MessageBoxIcon.Error);

					return false;
				}

				progressCallback(5, 0);

				for (int i = firstFrame; i <= lastFrame; i++)
				{
					AdvFramesIndexEntry frame = m_Index.Index[i];

					AdvImageData currentImageData;
					AdvStatusData currentStatusData;
					Bitmap currentBitmap;

					using (Pixelmap pixmap = GetFrameData(i, out currentImageData, out currentStatusData, out currentBitmap))
					{
						int lastRepeatedAviFrameNo = 0;

						if (isAavFile && !double.IsNaN(effFrameDuration))
						{
							lastRepeatedAviFrameNo = (int)Math.Round(((i - firstFrame) * effFrameDuration) / msPerFrame);
						}
						else if (!isAavFile && frame.ElapsedTime.Ticks != 0)
							lastRepeatedAviFrameNo = (int)Math.Round((frame.ElapsedTime.TotalMilliseconds - startingTimeMilliseconds) / msPerFrame);

						while (aviFrameNo < lastRepeatedAviFrameNo)
						{
                            TangraVideo.AddAviVideoFrame(pixmap, addedGamma, ImageSection.Adv16NormalisationValue);
							aviFrameNo++;
						}

						if (currentBitmap != null)
							currentBitmap.Dispose();
					}

					int percDone = (int)Math.Min(90, 90 * (i - firstFrame) * 1.0 / (lastFrame - firstFrame + 1));
					progressCallback(5 + percDone, 0);
				}	
			}
            finally
            {   
                TangraVideo.CloseAviFile();
				progressCallback(100, 0);
            }

            return false;
        }

        internal bool CropAdvFile(string fileName, int firstFrame, int lastFrame, OnSearchProgressDelegate progressCallback)
        {
			using (var fsr = new FileStream(m_FileName, FileMode.Open, FileAccess.Read))
			using (var reader = new BinaryReader(fsr))
			using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
			using (var writer = new BinaryWriter(fs))
			{
				AdvFramesIndexEntry firstEntry = m_Index.Index[0];

				CopyRawBytes(reader, 0, firstEntry.Offset, writer);

				progressCallback(5, 0);

				AdvFramesIndexEntry firstEntryToCopy = m_Index.Index[firstFrame];
				long zeroTicks = firstEntryToCopy.ElapsedTime.Ticks;

				var newIndex = new List<AdvFramesIndexEntry>();

				for (int i = firstFrame; i <= lastFrame; i++)
				{
					AdvFramesIndexEntry frameToCopy = m_Index.Index[i];

					var copiedFrame = new AdvFramesIndexEntry()
					{
						Offset = writer.BaseStream.Position,
						Length = frameToCopy.Length
					};

					long nextFrameStartPos = i == m_Index.NumberOfFrames - 1 ? m_Index.TableOffset : m_Index.Index[i + 1].Offset;

					CopyRawBytes(reader, frameToCopy.Offset, nextFrameStartPos - frameToCopy.Offset, writer);

					copiedFrame.ElapsedTime = new TimeSpan(frameToCopy.ElapsedTime.Ticks - zeroTicks);
					newIndex.Add(copiedFrame);

					int percDone = (int)Math.Min(90, 90 * (i - firstFrame) * 1.0 / (lastFrame - firstFrame + 1));
					progressCallback(5 + percDone, 0);
				}

				long indexTableOffset = writer.BaseStream.Position;

				progressCallback(95, 0);

				writer.Write((uint)newIndex.Count);

				// Save the new INDEX
				foreach (AdvFramesIndexEntry newIndexEntry in newIndex)
				{
					writer.Write((uint)Math.Round(newIndexEntry.ElapsedTime.TotalMilliseconds));
					writer.Write((long)newIndexEntry.Offset);
					writer.Write((uint)newIndexEntry.Length);
				}

				long userMetadataTablePosition = writer.BaseStream.Position;

                if (fsr.Length > m_UserMetadataTableOffset)
			    {
                    CopyRawBytes(reader, m_UserMetadataTableOffset, fsr.Length - m_UserMetadataTableOffset, writer);
			    }

			    writer.BaseStream.Seek(5, SeekOrigin.Begin);

				writer.Write((uint)newIndex.Count);
				writer.Write(indexTableOffset);

				writer.BaseStream.Seek(8, SeekOrigin.Current);

				writer.Write(userMetadataTablePosition);

				writer.BaseStream.Flush();

				progressCallback(100, 0);
			}

		    return false;
	    }

		internal void ExportStatusSectionToCSV(string fileName, int firstFrame, int lastFrame, OnSearchProgressDelegate progressCallback)
		{
			progressCallback(5, 0);

			//string folder = Path.GetDirectoryName(fileName);

			bool headerAppended = false;

            using (FileStream fsOutput = new FileStream(fileName, FileMode.CreateNew, FileAccess.Write))
            using(TextWriter writer = new StreamWriter(fsOutput))
            {
                for (int i = firstFrame; i <= lastFrame; i++)
                {
                    object[] data = GetFrameSectionData(i, null);
                    AdvImageData imageData = (AdvImageData)data[0];
                    AdvStatusData statusData = (AdvStatusData)data[1];

					//using(FileStream fs = new FileStream(string.Format("{0}\\{1}.pix", folder, i), FileMode.CreateNew, FileAccess.Write))
					//using (BinaryWriter wrt = new BinaryWriter(fs))
					//{
					//	for (int y = 0; y < ImageSection.Height; y++)
					//	{
					//		for (int x = 0; x < ImageSection.Width; x++)
					//		{
					//			ushort val = imageData.ImageData[x, y];
					//			wrt.Write(val);
					//		}
					//	}
					//}
	                
                    string headerRow;
                    string nextRow = StatusDataToCsvRow(imageData, statusData, i, out headerRow);
                    if (!headerAppended)
                    {
                        writer.WriteLine(headerRow);
                        headerAppended = true;
                    }
                    writer.WriteLine(nextRow);

                    int percDone = (int)Math.Min(90, 90 * (i - firstFrame) * 1.0 / (lastFrame - firstFrame + 1));
                    progressCallback(5 + percDone, 0);
                }

                progressCallback(95, 0);

                writer.Flush();
            }

		}

		private string StatusDataToCsvRow(AdvImageData imageData, AdvStatusData statusData, int frameNo, out string headerRow)
		{
			var output = new StringBuilder();
			output.AppendFormat("\"{0}\"", frameNo);
            output.AppendFormat(",\"{0}\"", imageData.MidExposureUtc.AddMilliseconds( - 1 * imageData.ExposureMilliseconds / 2.0).ToString("dd-MMM-yyyy HH:mm:ss.fff"));
            output.AppendFormat(",\"{0}\"", imageData.MidExposureUtc.AddMilliseconds(imageData.ExposureMilliseconds / 2.0).ToString("dd-MMM-yyyy HH:mm:ss.fff"));

			var header = new StringBuilder();
            header.Append("FrameNo,OCRStartTimestamp,OCREndTimestamp");

			foreach (AdvTagDefinition statusTag in statusData.TagValues.Keys)
			{
				string tagValue = statusData.TagValues[statusTag];
                if ((statusTag.Name == "SystemTime" || statusTag.Name == "NTPStartTimestamp" || statusTag.Name == "NTPEndTimestamp" || statusTag.Name == "StartTimestampSecondary" || statusTag.Name == "EndTimestampSecondary") &&
					!string.IsNullOrEmpty(tagValue))
				{
					tagValue = AdvFile.ADV_ZERO_DATE_REF.AddMilliseconds(long.Parse(tagValue)).ToString("dd-MMM-yyyy HH:mm:ss.fff");
				}
				else if (statusTag.Name == "GPSTrackedSatellites" && !string.IsNullOrEmpty(tagValue))
				{
					tagValue = int.Parse(tagValue).ToString();
				}
				else if (statusTag.Name == "Gamma" && !string.IsNullOrEmpty(tagValue))
				{
					tagValue = string.Format("{0:0.000}", float.Parse(tagValue));
				}
				else if (statusTag.Name == "Gain" && !string.IsNullOrEmpty(tagValue))
				{
					tagValue = string.Format("{0:0} dB", float.Parse(tagValue));
				}
				else if (statusTag.Name == "Shutter" && !string.IsNullOrEmpty(tagValue))
				{
					tagValue = string.Format("{0:0.000} sec", float.Parse(tagValue));
				}
				else if (statusTag.Name == "Offset" && !string.IsNullOrEmpty(tagValue))
				{
					tagValue = string.Format("{0:0.00} %", float.Parse(tagValue));
				}
				else if ((statusTag.Name == "VideoCameraFrameId" || statusTag.Name == "HardwareTimerFrameId") &&
				         !string.IsNullOrEmpty(tagValue))
				{
					tagValue = int.Parse(tagValue).ToString("#,###,###,###,###");
				}
				else if (statusTag.Name == "GPSAlmanacStatus" && !string.IsNullOrEmpty(tagValue))
				{
					int almanacStatus = int.Parse(tagValue);
					tagValue = AdvStatusValuesHelper.TranslateGpsAlmanacStatus(almanacStatus);
				}
				else if (statusTag.Name == "GPSAlmanacOffset" && !string.IsNullOrEmpty(tagValue))
				{
					int almanacOffset = int.Parse(tagValue);
					if ((almanacOffset & 0x80) == 0x80)
						almanacOffset = (short) (almanacOffset + (0xFF << 8));

					tagValue = AdvStatusValuesHelper.TranslateGpsAlmanacOffset(1, almanacOffset, false);
				}
				else if (statusTag.Name == "GPSFixStatus" && !string.IsNullOrEmpty(tagValue))
				{
					int fixStatus = int.Parse(tagValue);
					tagValue = AdvStatusValuesHelper.TranslateGpsFixStatus(fixStatus);
				}

				if (!string.IsNullOrEmpty(tagValue) && (statusTag.Name == "UserCommand" || statusTag.Name == "SystemError" || statusTag.Name == "GPSFix"))
				{
					string[] tokens = tagValue.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
					tagValue = string.Empty;
					for (int i = 0; i < tokens.Length; i++)
					{
						tagValue += string.Format("{0}[{1}];", statusTag.Name, i + 1);
					}
				}

				if (tagValue == null) tagValue = string.Empty;

				output.AppendFormat(",\"{0}\"", tagValue.Replace("\"", "\"\""));
				header.AppendFormat(",{0}", statusTag.Name);
			}

			headerRow = header.ToString();
			return output.ToString();
		}


	    internal bool FixAavFile18Aug(string fileName, int firstFrame, int lastFrame)
	    {
		    fileName = m_FileName;
			string newFile = Path.ChangeExtension(fileName, ".new.aav");
			File.Copy(fileName, newFile, true);

			using (FileStream fsr = new FileStream(fileName, FileMode.Open, FileAccess.Read))
			using (BinaryReader reader = new BinaryReader(fsr))
			using (FileStream fs = new FileStream(newFile, FileMode.Open, FileAccess.Write))
			using (BinaryWriter writer = new BinaryWriter(fs))
			{
				uint numFrames = m_Index.NumberOfFrames;

				long ZEROTICKS = (new DateTime(2013, 8, 18).Ticks - ADV_ZERO_DATE_REF.Ticks) / 10000;

				for (int i = 0; i < numFrames; i++)
				{
					AdvFramesIndexEntry entry = m_Index.Index[i];

					uint elapsedMiliseconds = (uint)entry.ElapsedTime.TotalMilliseconds;
					long offset = entry.Offset;
					uint length = entry.Length;

					object[] data = GetFrameSectionData(i, null);
					AdvImageData imageData = (AdvImageData)data[0];
					AdvStatusData statusData = (AdvStatusData)data[1];

					AdvTagDefinition startTS = StatusSection.TagDefinitions[3];
					AdvTagDefinition endTS = StatusSection.TagDefinitions[4];
					AdvTagDefinition gpsFix = StatusSection.TagDefinitions[7];
					
					string startTSString = statusData.TagValues[startTS];
					string endTSString = statusData.TagValues[endTS];
					string gpsFixString = statusData.TagValues[gpsFix];

					long ticksStart = FixOSD(ref startTSString, true);
					long ticksEnd = FixOSD(ref endTSString, false);

					long midTicks = ZEROTICKS + (ticksStart - 20 * 10000 + ticksEnd) / 20000;
					long exposureMs10 = new TimeSpan(ticksEnd - ticksStart + 20 * 10000).Milliseconds * 10;

					fsr.Seek(12 + (4 + entry.Offset), SeekOrigin.Begin);
					int bytesToSkip = reader.ReadInt32();

					WriteInt16((short)exposureMs10, 8 + (4 + entry.Offset), writer, reader);
					WriteInt64(midTicks, (4 + entry.Offset), writer, reader);

					WriteString(startTSString, 16 + (4 + entry.Offset) + bytesToSkip + 0x22, writer, reader);
					WriteString(endTSString, 16 + (4 + entry.Offset) + bytesToSkip + 0x43, writer, reader);

					int gspFixByte = int.Parse(gpsFixString);
					if (gspFixByte >= 1) gspFixByte++;

					WriteInt8((byte)gspFixByte, 16 + (4 + entry.Offset) + bytesToSkip + 0x1f, writer, reader);
					//writer.Write(elapsedMiliseconds);
					//writer.Write(offset);
					//writer.Write(length);
				}
			
				fs.Flush();
			}

		    return false;
	    }

		private void WriteInt8(byte val, long offset, BinaryWriter wrt, BinaryReader rdr)
		{
			wrt.BaseStream.Seek(offset, SeekOrigin.Begin);
			rdr.BaseStream.Seek(offset, SeekOrigin.Begin);

			byte currVal = rdr.ReadByte();
			if (currVal != val)
				wrt.Write(val);
		}

		private void WriteInt16(short val, long offset, BinaryWriter wrt, BinaryReader rdr)
		{
			wrt.BaseStream.Seek(offset, SeekOrigin.Begin);
			rdr.BaseStream.Seek(offset, SeekOrigin.Begin);

			short currVal = rdr.ReadInt16();
			if (currVal != val)
				wrt.Write(val);
		}

		private void WriteInt64(long val, long offset, BinaryWriter wrt, BinaryReader rdr)
		{
			wrt.BaseStream.Seek(offset, SeekOrigin.Begin);
			rdr.BaseStream.Seek(offset, SeekOrigin.Begin);

			long currVal = rdr.ReadInt64();
			if (currVal != val)
				wrt.Write(val);
		}

		private void WriteString(string str, long offset, BinaryWriter wrt, BinaryReader rdr)
	    {
			wrt.BaseStream.Seek(offset, SeekOrigin.Begin);
			rdr.BaseStream.Seek(offset, SeekOrigin.Begin);

			foreach (char ch in str.ToCharArray())
			{
				//char currChr = rdr.ReadChar();
				wrt.Write(ch);
			}
	    }

	    private long FixOSD(ref string osd, bool start)
	    {
			//		    1         2         3
			//0123456789012345678901234567890123456789

			//P7 0 09:38:45 0000xxxx 00000000

		    long ticks = 0;

		    ReplaceChar(ref osd, 3, '0', ' ');
			ReplaceChar(ref osd, 29, '0', ' ');
			ReplaceChar(ref osd, 30, '0', ' ');

			int hh = int.Parse("" + osd[5]) * 10 + int.Parse("" + osd[6]);
			int mm = int.Parse("" + osd[8]) * 10 + int.Parse("" + osd[9]);
			int ss = int.Parse("" + osd[11]) * 10 + int.Parse("" + osd[12]);
		    int ms10 = 0;

			if (start)
			{
				ReplaceChar(ref osd, 14, '0', ' ');
				ReplaceChar(ref osd, 15, '0', ' ');
				ReplaceChar(ref osd, 16, '0', ' ');
				ReplaceChar(ref osd, 17, '0', ' ');

				ms10 = int.Parse("" + osd[18]) * 1000 + int.Parse("" + osd[19]) * 100 + int.Parse("" + osd[20]) * 10 + int.Parse("" + osd[21]);
			}
			else
			{
				ReplaceChar(ref osd, 18, '0', ' ');
				ReplaceChar(ref osd, 19, '0', ' ');
				ReplaceChar(ref osd, 20, '0', ' ');
				ReplaceChar(ref osd, 21, '0', ' ');

				ms10 = int.Parse("" + osd[14]) * 1000 + int.Parse("" + osd[15]) * 100 + int.Parse("" + osd[16]) * 10 + int.Parse("" + osd[17]);
			}

		    var ts = new TimeSpan(0, hh, mm, ss, ms10 / 10);
		    return ts.Ticks;
	    }

		private void ReplaceChar(ref string osd, int charId, char old, char newchar)
		{
			if (osd[charId] == old) 
				osd = osd.Substring(0, charId) + newchar + osd.Substring(charId + 1);
		}
    }
}
