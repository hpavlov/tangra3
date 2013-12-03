using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Tangra.Helpers;
using Tangra.Model.Astro;
using Tangra.Model.Config;
using Tangra.Model.Helpers;
using Tangra.Model.Image;
using Tangra.Model.VideoOperations;
using Tangra.OCR;
using Tangra.VideoOperations.LightCurves.Tracking;

namespace Tangra.VideoOperations.LightCurves
{
    internal class LCFile
    {
        internal LCMeasurementHeader Header;
        internal LCMeasurementFooter Footer;
        internal List<List<LCMeasurement>> Data;
    	internal List<LCFrameTiming> FrameTiming;

    	internal int LcFileFormatVersion;

        private static short LC_LAST_SUPPORTED_FILE_VERSION = 4;
        private static short LC_FILE_VERSION = 4;

        public static void Save(string fileName, LCMeasurementHeader header, List<List<LCMeasurement>> data, List<LCFrameTiming> frameTiming, LCMeasurementFooter footer)
        {
            using (MemoryStream memStr = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(memStr))
            {
                writer.Write(LC_FILE_VERSION);

                header.WriteTo(writer);

                writer.Write(header.ObjectCount);

                bool showProgress = false;

                if (data.Count > 0)
                {
                    int itemsInGroup = data[0].Count;
                    writer.Write(itemsInGroup);

                    int total = header.ObjectCount*itemsInGroup;
                    showProgress = (total) / 100 > 100;

                    ThreadingHelper.RunTaskWaitAndDoEvents(
                        delegate()
                            {
                                if (showProgress)
                                    FileProgressManager.BeginFileOperation(total);

                                try
                                {
                                    int idx = 0;

                                    for (int i = 0; i < itemsInGroup; i++)
                                    {
										if (frameTiming != null && 
											header.TimingType != MeasurementTimingType.UserEnteredFrameReferences)
										{
											LCFrameTiming timingEntry = frameTiming[i];
											timingEntry.WriteTo(writer);
										}

                                        for (int j = 0; j < header.ObjectCount; j++)
                                        {
                                            LCMeasurement measurement = data[j][i];

                                            measurement.WriteTo(writer);

                                            if (showProgress)
                                            {
                                                idx++;
                                                if (idx % 100 == 0)
                                                    FileProgressManager.FileOperationProgress(idx);
                                            }
                                        }
                                    }
                                }
                                finally
                                {
                                    if (showProgress)
                                        FileProgressManager.EndFileOperation();
                                }
                            },
                            150); ;
                }

                footer.WriteTo(writer);

                writer.Flush();


                using (FileStream outFile = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                {
                    memStr.Seek(0, SeekOrigin.Begin);

                    byte[] inputReadBuffer = memStr.ToArray();

                    using (DeflateStream deflateStream = new DeflateStream(outFile, CompressionMode.Compress, true))
                    {
                        memStr.Read(inputReadBuffer, 0, inputReadBuffer.Length);

                        deflateStream.Write(inputReadBuffer, 0, inputReadBuffer.Length);
                        deflateStream.Flush();
                    }
                }
            }
        }

        public static LCFile Load(string fileName)
        {
            using (var inFile = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            using (var deflateStream = new DeflateStream(inFile, CompressionMode.Decompress, true))
            using (var reader = new BinaryReader(deflateStream))
            {
                short version = reader.ReadInt16();
                if (version > LC_FILE_VERSION)
                {
                    MessageBox.Show("This light curve file requires a newer version of Tangra.", "Tangra", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }

				//if (version < LC_LAST_SUPPORTED_FILE_VERSION)
				//{
				//	string requiredVersion = "1.0";
				//	if (version >= 2 && version <= 3)
				//		requiredVersion = "1.4";

				//	MessageBox.Show(
				//		string.Format("This light curve is old and is not supported by this version of Tangra.\r\n\r\n You will need Tangra v{0} to open this file.", requiredVersion), 
				//		"Tangra", 
				//		MessageBoxButtons.OK, 
				//		MessageBoxIcon.Error);

				//	return null;
				//}

                var lcFile = new LCFile
	            {
		            LcFileFormatVersion = version, 
					Header = new LCMeasurementHeader(reader)
	            };

	            byte totalObjects = reader.ReadByte();

                lcFile.Data = new List<List<LCMeasurement>>(new List<LCMeasurement>[] { new List<LCMeasurement>(), new List<LCMeasurement>(), new List<LCMeasurement>(), new List<LCMeasurement>() });
            	lcFile.FrameTiming = new List<LCFrameTiming>();

                if (totalObjects > 0)
                {
                    int itemsInGroup = reader.ReadInt32();

	                bool showProgress = true; //(totalObjects * itemsInGroup) / 100 > 100;
                    bool notSupported = false;

                    ThreadingHelper.RunTaskWaitAndDoEvents(
                        delegate()
                        {
                            if (showProgress)
                                FileProgressManager.BeginFileOperation(totalObjects * itemsInGroup);

                            try
                            {
                                int idx = 0;
                                for (int i = 0; i < 4; i++)
                                    lcFile.Data[i] = new List<LCMeasurement>();

                                LCMeasurement prevMeasurement = LCMeasurement.Empty;

                                int lastFrameNo = -1;

                                for (int j = 0; j < itemsInGroup; j++)
                                {
									if (version > 2 &&
										lcFile.Header.TimingType != MeasurementTimingType.UserEnteredFrameReferences)
									{
										var frameTiming = new LCFrameTiming(reader);
										lcFile.FrameTiming.Add(frameTiming);
									}

                                    LCMeasurement measurement = LCMeasurement.Empty;

                                    for (int i = 0; i < totalObjects; i++)
                                    {
                                        measurement = new LCMeasurement(reader, prevMeasurement);
                                        prevMeasurement = measurement;

                                        if (lastFrameNo != (int)measurement.CurrFrameNo)
                                            lcFile.Data[i].Add(measurement);
                                        else
                                            // This shouldn't happen, but this is a fix if it happens. Overwrite the last frame measurement if the measurement is repeated
                                            lcFile.Data[i][lcFile.Data[i].Count - 1] = measurement;

                                        if (showProgress)
                                        {
                                            idx++;
                                            if (idx % 100 == 0)
                                                FileProgressManager.FileOperationProgress(idx);
                                        }
                                    }

                                    lastFrameNo =(int)measurement.CurrFrameNo;
                                }
                            }
                            catch (NotSupportedException)
                            {
                                notSupported = true;
                            }
                            finally
                            {
                                if (showProgress)
                                    FileProgressManager.EndFileOperation();
                            }
                        },
                            150);

                    if (notSupported)
                        MessageBox.Show(
                            "This file is incompatible with your current version of Tangra.",
                            "Cannot load light curve",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                }

                lcFile.Footer = new LCMeasurementFooter(reader);

                return lcFile;
            }
        }


        private static string s_OnTheFlyFileName = Path.GetFullPath(Path.GetTempPath() + @"\tangra.lc.tmp");
        private static string s_OnTheFlyZippedFileName = Path.GetFullPath(Path.GetTempPath() + @"\tangra.lc");

        private static FileStream s_OnTheFlyFile;
        private static BinaryWriter s_OnTheFlyWriter;
        private static int[] s_NumMeasurements = new int[4];

        public static void NewOnTheFlyOutputFile(string pathToVideoFile, string sourceInfo, byte numberOfTargets, float positionTolerance)
        {
            if (s_OnTheFlyFile != null)
                s_OnTheFlyFile.Close();

            if (File.Exists(s_OnTheFlyFileName))
            {
                try
                {
                    File.Delete(s_OnTheFlyFileName);
                }
                catch(IOException ioe)
                {
                    Trace.WriteLine(ioe);
                    MessageBox.Show("There is another instance of Tangra already running.");
                    Application.Exit();
                }
            }

            s_OnTheFlyFile = new FileStream(s_OnTheFlyFileName, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
            s_OnTheFlyWriter = new BinaryWriter(s_OnTheFlyFile);

            LCMeasurementHeader header = new LCMeasurementHeader(
                pathToVideoFile, sourceInfo , - 1, 0, 0, 0, 0, 0, 0, numberOfTargets, LightCurveReductionType.UntrackedMeasurement, MeasurementTimingType.UserEnteredFrameReferences,   0, 0,
                EmptyArray<int>(numberOfTargets), EmptyArray<float>(numberOfTargets), EmptyArray<bool>(numberOfTargets), positionTolerance);

            s_NumMeasurements[0] = 0; s_NumMeasurements[1] = 0; s_NumMeasurements[2] = 0; s_NumMeasurements[3] = 0;

            s_OnTheFlyWriter.Write(LC_FILE_VERSION);

            header.WriteTo(s_OnTheFlyWriter);

            s_OnTheFlyWriter.Write(header.ObjectCount);
            s_OnTheFlyWriter.Write(s_NumMeasurements[0]);
        }

        private static T[] EmptyArray<T>(int count)
        {
            List<T> list = new List<T>();
            for (int i = 0; i < count; i++) list.Add(default(T));
            return list.ToArray();
        }

        public static void SaveOnTheFlyMeasurement(LCMeasurement measurement)
        {
            measurement.WriteTo(s_OnTheFlyWriter);
            s_NumMeasurements[measurement.TargetNo]++;
        }

        public static LCFile FlushOnTheFlyOutputFile(LCMeasurementHeader header, LCMeasurementFooter footer)
        {
			if (s_OnTheFlyFile != null)
			{
				NotificationManager.Instance.NotifyBeginLongOperation();

				try
				{
					s_OnTheFlyWriter.Flush();
					s_OnTheFlyFile.Seek(0, SeekOrigin.Begin);

					s_OnTheFlyWriter.Write(LC_FILE_VERSION);
					header.WriteTo(s_OnTheFlyWriter);
					s_OnTheFlyWriter.Write(header.ObjectCount);
					s_OnTheFlyWriter.Write(s_NumMeasurements[0]);

					s_OnTheFlyFile.Seek(0, SeekOrigin.End);
					footer.WriteTo(s_OnTheFlyWriter);

					s_OnTheFlyWriter.Flush();

					s_OnTheFlyFile.Close();
					s_OnTheFlyFile = null;

					using (FileStream inFile = new FileStream(s_OnTheFlyFileName, FileMode.Open, FileAccess.Read))
					using (FileStream outFile = new FileStream(s_OnTheFlyZippedFileName, FileMode.Create, FileAccess.Write))
					{
						inFile.Seek(0, SeekOrigin.Begin);

						int BUFFER_SIZE = 1024;
						byte[] inputReadBuffer = new byte[BUFFER_SIZE];

						using (DeflateStream deflateStream = new DeflateStream(outFile, CompressionMode.Compress, true))
						{
							int bytesRead = 0;
							do
							{
								bytesRead = inFile.Read(inputReadBuffer, 0, inputReadBuffer.Length);
								deflateStream.Write(inputReadBuffer, 0, inputReadBuffer.Length);
							}
							while (bytesRead == BUFFER_SIZE);

							deflateStream.Flush();
						}
					}

					File.Delete(s_OnTheFlyFileName);
				}
				finally
				{
					NotificationManager.Instance.NotifyEndLongOperation();
				}
			}

            return LCFile.Load(s_OnTheFlyZippedFileName);
        }

		public static void SaveOnTheFlyFrameTiming(LCFrameTiming frameTiming)
		{
			frameTiming.WriteTo(s_OnTheFlyWriter);
		}

		internal DateTime GetTimeForFrame(double frameNo, out string correctedForInstrumentalDelayMessage)
		{
			correctedForInstrumentalDelayMessage = null;

			if (FrameTiming != null && FrameTiming.Count > 0)
			{
				if (Footer.InstrumentalDelayConfig != null)
				{
					return GetTimeForFrameWithInstrumentalDelay(frameNo, out correctedForInstrumentalDelayMessage);
				}
				else
				{
					LCFrameTiming timingEntry = FrameTiming[(int) ((uint) frameNo - Header.MinFrame)];
					return timingEntry.FrameMidTime;
				}
			}
			else
			{
				if (Footer.InstrumentalDelayConfig != null)
				{
					return GetTimeForFrameFromManuallyEnteredTimesWithInstrumentalDelay(frameNo, out correctedForInstrumentalDelayMessage);
				}
				else
				{
					return Header.GetTimeForFrameFromManuallyEnteredTimes(frameNo);
				}
			}
		}

		private DateTime GetTimeForFrameFromManuallyEnteredTimesWithInstrumentalDelay(double frameNo, out string correctedForInstrumentalDelayMessage)
		{
			DateTime headerComputedTime = Header.GetTimeForFrameFromManuallyEnteredTimes(frameNo);
			DateTime rv = headerComputedTime;

			correctedForInstrumentalDelayMessage = null;

			int integratedFields = 0;
			double ntscCountedFrames = 29.97 / Header.ComputedFramesPerSecond;
			double palCountedFrames = 25.00 / Header.ComputedFramesPerSecond;
			for (int i = 0; i < 256; i++)
			{
				if (Math.Abs(i - ntscCountedFrames) < 0.01)
				{
					integratedFields = 2 * i;
					break;
				}

				if (Math.Abs(i - palCountedFrames) < 0.01)
				{
					integratedFields = 2 * i;
					break;
				}
			}

			if (integratedFields > 0 && Footer.InstrumentalDelayConfig.ContainsKey(integratedFields / 2))
			{
				double corrInstrumentalDelay = Footer.InstrumentalDelayConfig[integratedFields / 2]; // This is already negative

				rv = rv.AddSeconds(corrInstrumentalDelay);

				correctedForInstrumentalDelayMessage = string.Format(
					"Instrumental delay has been applied to the times\r\n\r\nEnd of first field OSD timestamp: {0}", headerComputedTime.ToString("HH:mm:ss.fff"));
			}

			return rv;
		}

		private DateTime GetTimeForFrameWithInstrumentalDelay(double frameNo, out string correctedForInstrumentalDelayMessage)
		{
			int frameTimingIndex = (int) ((uint) frameNo - Header.MinFrame);

			LCFrameTiming timingEntry = FrameTiming[frameTimingIndex];
			LCFrameTiming othertimingEntry = timingEntry;
			
			DateTime rv = timingEntry.FrameMidTime;
			correctedForInstrumentalDelayMessage = null;

			if (frameTimingIndex > 0)
			{
				// Use the previous frame
				othertimingEntry = FrameTiming[frameTimingIndex - 1];
			}
			else if (frameTimingIndex == 0 && FrameTiming.Count > 1)
			{
				// For first frame, use the second frame
				othertimingEntry = FrameTiming[1];
			}

			double intervalDuration = Math.Abs(new TimeSpan(othertimingEntry.FrameMidTime.Ticks - timingEntry.FrameMidTime.Ticks).TotalSeconds);
			if (intervalDuration > 0)
			{
				double videoStandardFieldDurationSec = 0;
				int integratedFields = 0;

				double ntscCountedFrames = intervalDuration * 29.97;
				double palCountedFrames = intervalDuration * 25.00;
				for (int i = 0; i < 256; i++)
				{
					if (Math.Abs(i - ntscCountedFrames) < 0.01)
					{
						integratedFields = 2 * i;
						videoStandardFieldDurationSec = 0.01668335;
						break;
					}

					if (Math.Abs(i - palCountedFrames) < 0.01)
					{
						integratedFields = 2 * i;
						videoStandardFieldDurationSec = 0.02;
						break;
					}
				}

				if (integratedFields > 0 && Footer.InstrumentalDelayConfig.ContainsKey(integratedFields / 2))
				{
					// Apply correction: [FRAMETIME - ((1/2 * INTEGRATION PERIOD) - 1 FIELD) * (PAL or NTSC FrameRate) - GERHARD_CORRECTION(INTEGRATION PERIOD)]
					double corrEndFirstField = -1 * (((integratedFields / 2) - 1) * videoStandardFieldDurationSec); // Half frame back sets us at the begining of the first field, then add 1 field to get to the end of the field
					double corrInstrumentalDelay = Footer.InstrumentalDelayConfig[integratedFields/2]; // This is already negative
					
					DateTime endOfFirstField = rv.AddSeconds(corrEndFirstField);
					rv = rv.AddSeconds(corrEndFirstField + corrInstrumentalDelay);

					correctedForInstrumentalDelayMessage = string.Format(
						"Instrumental delay has been applied to the times\r\n\r\nEnd of first field OSD timestamp: {0}", endOfFirstField.ToString("HH:mm:ss.fff"));
				}
			}

			return rv;
		}

		internal bool CanDetermineFrameTimes
		{
			get
			{
				return 
					Header.FirstTimedFrameTime != DateTime.MaxValue || 
					Footer.ReductionContext.HasEmbeddedTimeStamps;
			}			
		}
    }

	internal struct LCFrameTiming
	{
		public DateTime FrameMidTime;
		public int FrameDurationInMilliseconds;
		public byte[,] OCRedPixels;

		// TODO: Add ORC area dimentions to header
		
		public LCFrameTiming(DateTime frameMidTime, int frameDurationInMilliseconds)
		{
			FrameMidTime = frameMidTime;
			FrameDurationInMilliseconds = frameDurationInMilliseconds;
			// NOTE: Not saving OSD pixels at this stage
			OCRedPixels = new byte[0,0];
		}

		private static int SERIALIZATION_VERSION = 1;

		internal LCFrameTiming(BinaryReader reader)
		{
		    int version = reader.ReadInt32();

			FrameMidTime = new DateTime(reader.ReadInt64());
			FrameDurationInMilliseconds = reader.ReadInt32();
			int width = reader.ReadInt32();
			int height = reader.ReadInt32();

			OCRedPixels = new byte[width, height];

			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					OCRedPixels[x, y] = reader.ReadByte();
				}
			}
		}


		internal void WriteTo(BinaryWriter writer)
		{
			writer.Write(SERIALIZATION_VERSION);

			writer.Write(FrameMidTime.Ticks);
			writer.Write(FrameDurationInMilliseconds);

			int width = OCRedPixels.GetLength(0);
			int height = OCRedPixels.GetLength(1);
			writer.Write(width);
			writer.Write(height);

			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					writer.Write(OCRedPixels[x, y]);
				}
			}
		}
	}

    internal enum ProcessingType
    {
        SignalOnly,
        SignalMinusBackground,
        BackgroundOnly,
		SignalDividedByBackground,
		SignalDividedByNoise
    }

    internal struct LCMeasurement : IMeasurableObject
    {
        public static LCMeasurement Empty = new LCMeasurement();

        private static int MATRIX_SIZE = 35;

        public static bool IsEmpty(LCMeasurement compareTo)
        {
            return Empty.Equals(compareTo);
        }

        internal uint CurrFrameNo;
        internal byte TargetNo;

        internal uint TotalReading;
        internal uint TotalBackground;

        internal uint[,] PixelData;
        internal byte Flags;
		internal uint FlagsDWORD;
        internal float X0;
        internal float Y0;
        internal int PixelDataX0;
        internal int PixelDataY0;

        internal PSFFit PsfFit;

        internal string GetFlagsExplained()
        {

			if (!HasDWORDFlags)
				return TrackedObject.GetByteFlagsExplained(Flags);
			else
				return TrackedObject.GetDWORDFlagsExplained(FlagsDWORD);
        }

		private bool HasDWORDFlags
		{
			get
			{
				return FlagsDWORD != 0 && Flags == 0;
			}
		}

        public LCMeasurement Clone()
        {
            LCMeasurement clone = new LCMeasurement();
            clone.CurrFrameNo = this.CurrFrameNo;
            clone.TargetNo = this.TargetNo;
            clone.TotalBackground = this.TotalBackground;
            clone.Flags = this.Flags;
        	clone.FlagsDWORD = this.FlagsDWORD;
            clone.X0 = this.X0;
            clone.Y0 = this.Y0;
            clone.PixelDataX0 = this.PixelDataX0;
            clone.PixelDataY0 = this.PixelDataY0;

            clone.PixelData = new uint[MATRIX_SIZE, MATRIX_SIZE];
            for (int i = 0; i < MATRIX_SIZE; i++)
                for (int j = 0; j < MATRIX_SIZE; j++)
                {
                    clone.PixelData[i, j] = this.PixelData[i, j];
                }

            return clone;
        }

        // Not serialized. Used to store computed value for displaying
        internal int AdjustedReading;
        internal int AdjustedBackground;

    	private static List<int> GOOD_FLAGS = new List<int>(new int[]
      	{
			0, // No Flags
      		1, // TrackedSuccessfully
			2, // FixedObject
			6, // FullyDisappearingStarMarkedTrackedWithoutBeingFound
			9, // TrackedSuccessfullyAfterDistanceCheck + TrackedSuccessfully
		    14,// TrackedSuccessfullyAfterDistanceCheck + FullyDisappearingStarMarkedTrackedWithoutBeingFound  
			17,// TrackedSuccessfullyAfterWiderAreaSearch + TrackedSuccessfully 
			22,// TrackedSuccessfullyAfterWiderAreaSearch + FullyDisappearingStarMarkedTrackedWithoutBeingFound   
			25,// TrackedSuccessfullyAfterStarRecognition + TrackedSuccessfully 
			30 // TrackedSuccessfullyAfterStarRecognition + FullyDisappearingStarMarkedTrackedWithoutBeingFound  
      	});

        internal bool IsSuccessfulReading
        {
            get
            {
				if (HasDWORDFlags)
					return GOOD_FLAGS.IndexOf((byte)(this.FlagsDWORD & 0xFF)) != -1;
				else
					return GOOD_FLAGS.IndexOf(Flags) != -1;
            }
        }

		public void SetIsMeasured(NotMeasuredReasons reasons)
		{
			if (reasons != NotMeasuredReasons.MeasuredSuccessfully)
				FlagsDWORD += (uint)reasons;
		}

		internal bool IsOffScreen
		{
			get
			{
				if (HasDWORDFlags)
					return (FlagsDWORD & 0x00000100) != 0;
				else
					return false;
			}
		}

        public DateTime OSDTimeStamp;

        private static int SERIALIZATION_VERSION = 7;
        private static int MINAMAL_SUPPORTED_VERSION = 3;
    	private static int FIRST_UINT_MATRIX_VERSION = 7;

        internal LCMeasurement(
            uint currFrameNo,
            byte targetNo,
            uint totalReading,
            uint totalBackground,
            uint flagsDWORD,
            float x0, float y0,
            PSFFit psfFit,
            uint[,] pixelData,
            int pixelDataX0, int pixelDataY0,
            DateTime osdTimeStamp)
        {
            AdjustedReading = 0;
            AdjustedBackground = 0;

            CurrFrameNo = currFrameNo;
            TargetNo = targetNo;
            TotalReading = totalReading;
            TotalBackground = totalBackground;
            PixelData = pixelData;
            Flags = 0;
        	FlagsDWORD = flagsDWORD;

            X0 = x0;
            Y0 = y0;

            PixelDataX0 = pixelDataX0;
            PixelDataY0 = pixelDataY0;

            PsfFit = psfFit;

            OSDTimeStamp = osdTimeStamp;

            ReProcessingPsfFitMatrixSize = 11;
        }

        internal LCMeasurement(BinaryReader reader, LCMeasurement prevMeasurement)
        {
            AdjustedReading = 0;
            AdjustedBackground = 0;
            PsfFit = null;

            int version = reader.ReadInt32();
            if (version < MINAMAL_SUPPORTED_VERSION)
                throw new NotSupportedException("This is an old light curve file and it is not supported any more.");

            CurrFrameNo = reader.ReadUInt32();
            TargetNo = reader.ReadByte();
            TotalReading = reader.ReadUInt32();
            TotalBackground = reader.ReadUInt32();

        	bool matrixAsBytes = version < FIRST_UINT_MATRIX_VERSION;

            PixelData = new uint[MATRIX_SIZE, MATRIX_SIZE];
            for (int i = 0; i < MATRIX_SIZE; i++)
                for (int j = 0; j < MATRIX_SIZE; j++)
                {
					PixelData[i, j] = matrixAsBytes ? reader.ReadByte() : reader.ReadUInt32();
                }

            Flags = reader.ReadByte();

            X0 = reader.ReadSingle();
            Y0 = reader.ReadSingle();

            PixelDataX0 = reader.ReadInt32();
            PixelDataY0 = reader.ReadInt32();

            if (version == 3)
            {
                // This was only saved in version 3 and is for backward compatibility only
                reader.ReadSingle(); /* XMatrix */
                reader.ReadSingle(); /* YMatrix */
            }

            OSDTimeStamp = DateTime.MinValue;
        	FlagsDWORD = 0;
			if (version > 4)
            {
                OSDTimeStamp = new DateTime(reader.ReadInt64());

				if (version > 5)
				{
					FlagsDWORD = reader.ReadUInt32();
				}
            }

            ReProcessingPsfFitMatrixSize = 11;
        }

        internal void WriteTo(BinaryWriter writer)
        {
            writer.Write(SERIALIZATION_VERSION);

            writer.Write(CurrFrameNo);
            writer.Write(TargetNo);
            writer.Write(TotalReading);
            writer.Write(TotalBackground);
            for (int i = 0; i < MATRIX_SIZE; i++)
                for (int j = 0; j < MATRIX_SIZE; j++)
                {
                    writer.Write(PixelData[i, j]);
                }
            writer.Write(Flags);
            writer.Write(X0);
            writer.Write(Y0);
            writer.Write(PixelDataX0);
            writer.Write(PixelDataY0);

            // Version 5 Data
            writer.Write(OSDTimeStamp.Ticks);

			// Version 6 Data
			writer.Write(FlagsDWORD);
        }

		internal int ReProcessingPsfFitMatrixSize;

		bool IMeasurableObject.IsOccultedStar
		{
			get
			{
				// We don't know this (don't have a guess) when re-processing data
				return false;
			}
		}

		bool IMeasurableObject.MayHaveDisappeared
		{
			get
			{
				// We don't know this (don't have a guess) when re-processing data
				return false;
			}
		}

		int IMeasurableObject.PsfFittingMatrixSize
		{
			get { return ReProcessingPsfFitMatrixSize; }
		}
	}

	public enum MeasurementTimingType
	{
		UserEnteredFrameReferences = 0,
		OCRedTimeForEachFrame = 1,
		EmbeddedTimeForEachFrame = 2
	}

    internal struct LCMeasurementHeader
    {
        public static LCMeasurementHeader Empty = new LCMeasurementHeader();

        public static bool IsEmpty(LCMeasurementHeader compareTo)
        {
            return Empty.Equals(compareTo);
        }

	    private static long MISSING_TIMESTAMP_TICKS = 633979008000000000;

    	public LCFile LcFile;

        internal uint MinFrame;
        internal uint MaxFrame;
        internal uint MeasuredFrames;
        internal byte ObjectCount;

        internal readonly int TimedFrames;

        internal string PathToVideoFile;
        internal int FirstFrameInVideoFile;
        internal int CountFrames;

        internal uint MeasurementInterval;

        internal DateTime FirstTimedFrameTime;
        internal DateTime SecondTimedFrameTime;

        internal int FirstTimedFrameNo;
        internal int LastTimedFrameNo;

        private TimeSpan TotalTimeStapmedTime;

    	internal MeasurementTimingType TimingType;

        internal double GetAbsoluteTimeDeltaInMilliseconds(out string videoSystem)
        {
            TimeSpan totalTimeStapmedTime = new TimeSpan(SecondTimedFrameTime.Ticks - FirstTimedFrameTime.Ticks);

            double assumedFrameRate = FramesPerSecond;
            videoSystem = null;

			if (SourceInfo.Contains("(AAV.8)"))
			{
				assumedFrameRate  = ComputedFramesPerSecond;
				videoSystem = "AAV";
			}
            else if (FramesPerSecond > 24.0 && FramesPerSecond < 26.0)
            {
                assumedFrameRate = 25.0;
                videoSystem = "PAL";
            }
            else if (FramesPerSecond > 29.0 && FramesPerSecond < 31.0)
            { 
                assumedFrameRate = 29.97;
                videoSystem = "NTSC";
            }

            double videoTimeInSec = (LastTimedFrameNo - FirstTimedFrameNo) / assumedFrameRate;
            return Math.Abs(videoTimeInSec - totalTimeStapmedTime.TotalSeconds) * 1000;
        }

        internal bool DoesTimeStampDerivedTimeMatchStandardFrameRate(out string videoSystem, out double derivedFrameRate)
        {
            TimeSpan totalTimeStapmedTime = new TimeSpan(SecondTimedFrameTime.Ticks - FirstTimedFrameTime.Ticks);
            derivedFrameRate = (LastTimedFrameNo - FirstTimedFrameNo) / totalTimeStapmedTime.TotalSeconds;

            if (Math.Round(Math.Abs(derivedFrameRate - 25), 4) <= TangraConfig.Settings.Special.PalNtscFrameRateDifference)
            {
                videoSystem = "PAL";
                return true; /* PAL frame rate*/
            }

            if (Math.Round(Math.Abs(derivedFrameRate - 29.97), 4) <= TangraConfig.Settings.Special.PalNtscFrameRateDifference)
            {
                videoSystem = "NTSC";
                return true; /* NTSC frame rate*/
            }

            videoSystem = "Unknown";
            return false;
        }

		internal bool HasNonEqualySpacedDataPoints()
		{
			List<LCFrameTiming> timings = LcFile != null ? LcFile.FrameTiming : null;

			if (timings != null && timings.Count > 1)
			{
				var allFrameIntervals = new List<double>();

				for (int i = 0; i < timings.Count - 1; i++)
				{
					double interval = new TimeSpan(timings[i + 1].FrameMidTime.Ticks - timings[i].FrameMidTime.Ticks).TotalMilliseconds;
					allFrameIntervals.Add(interval);
				}

				double maxDifference = (allFrameIntervals.Max() - allFrameIntervals.Min()) / 2.0;
				double average = allFrameIntervals.Average();

				double maxDifferencePerc = maxDifference / average;

				if (maxDifferencePerc > 0.15 || // More than 15% difference
					maxDifference > 20) // ... or more than 10ms (1/2 PAL frame) difference in total
				{
					return true;
				}
			}

			return false;
		}

        internal double GetTimeDeltaInMillisecondsFor1000Frames()
        {
            if (TotalTimeStapmedTime != TimeSpan.Zero)
            {
                double videoTimeInSec = (TimedFrames - 1) / FramesPerSecond;
                return Math.Abs(videoTimeInSec - TotalTimeStapmedTime.TotalSeconds) * 1000 * 1000 / TimedFrames;
            }

            return double.NaN;
        }

        internal DateTime GetTimeForFrameFromManuallyEnteredTimes(double frameNo)
        {
            if (TimedFrames > 1)
                return FirstTimedFrameTime.AddMilliseconds(
                    (frameNo - FirstTimedFrameNo) * TotalTimeStapmedTime.TotalMilliseconds / (TimedFrames - 1));
            else
                return FirstTimedFrameTime;
        }

        internal DateTime GetTimeForFrameFromFrameTiming(double frameNo, bool interpolateMissingTimestamps)
        {
            if (Math.Abs((int) frameNo - frameNo) < 0.01)
                return GetTimeForFrameFromFrameTiming((int)frameNo, interpolateMissingTimestamps);
            else
            {
                int prev = (int)Math.Floor(frameNo);
                int next = (int)Math.Ceiling(frameNo);

                long prevTicks = GetTimeForFrameFromFrameTiming(prev, interpolateMissingTimestamps).Ticks;
                long nextTicks = GetTimeForFrameFromFrameTiming(next, interpolateMissingTimestamps).Ticks;

                return new DateTime(prevTicks + (long)Math.Round((frameNo - prev) * (nextTicks - prevTicks)));
            }
        }

        internal DateTime GetTimeForFrameFromFrameTiming(int frameNo, bool interpolateMissingTimestamps)
		{
			DateTime rv;
			int frameTimingIndex = (int) (frameNo - MinFrame);

			if (LcFile != null && LcFile.FrameTiming != null && LcFile.FrameTiming.Count > 0)
			{
				if (frameTimingIndex >= 0 && frameTimingIndex < LcFile.FrameTiming.Count)
					rv = LcFile.FrameTiming[frameTimingIndex].FrameMidTime;
				else if (frameTimingIndex < 0)
					rv = LcFile.FrameTiming[0].FrameMidTime;
				else if (frameTimingIndex < LcFile.FrameTiming.Count)
					rv = LcFile.FrameTiming[LcFile.FrameTiming.Count - 1].FrameMidTime;
				else
					rv = LcFile.FrameTiming[0].FrameMidTime;

				if (interpolateMissingTimestamps && rv.Ticks == MISSING_TIMESTAMP_TICKS)
					rv = GetInterpolatedTimeForFrameFromFrameTiming(frameNo);
			}
			else
				rv = FirstTimedFrameTime;

			return rv;
		}

	    private DateTime GetInterpolatedTimeForFrameFromFrameTiming(double frameNo)
	    {
			int frameTimingIndex = (int)(frameNo - MinFrame);
		    long referenceFrameTicks = 0;
			int referenceFrameNo = 0;
			long referenceFrameDurationInTicks = 0;
			long interpolatedTicks = MISSING_TIMESTAMP_TICKS;

			// Look for a frame with a timestamp going forward
		    int nextFrameWithTimeStamp = frameTimingIndex + 1;
			while (nextFrameWithTimeStamp < LcFile.FrameTiming.Count)
			{
				long ticks = LcFile.FrameTiming[nextFrameWithTimeStamp].FrameMidTime.Ticks;
				if (ticks != MISSING_TIMESTAMP_TICKS)
				{
					if (referenceFrameTicks == 0)
					{
						referenceFrameTicks = ticks;
						referenceFrameNo = nextFrameWithTimeStamp;
						referenceFrameDurationInTicks = LcFile.FrameTiming[nextFrameWithTimeStamp].FrameDurationInMilliseconds * 10000;
						break;
					}
				}

				nextFrameWithTimeStamp++;
			}

			if (referenceFrameTicks != 0)
			{
				interpolatedTicks = referenceFrameTicks - (referenceFrameNo - frameTimingIndex) * referenceFrameDurationInTicks;
			}
			else
			{
				// Try to find a time reference backward
				nextFrameWithTimeStamp = frameTimingIndex - 1;
				while (nextFrameWithTimeStamp >= 0)
				{
					long ticks = LcFile.FrameTiming[nextFrameWithTimeStamp].FrameMidTime.Ticks;
					if (ticks != MISSING_TIMESTAMP_TICKS)
					{
						if (referenceFrameTicks == 0)
						{
							referenceFrameTicks = ticks;
							referenceFrameNo = nextFrameWithTimeStamp;
							referenceFrameDurationInTicks = LcFile.FrameTiming[nextFrameWithTimeStamp].FrameDurationInMilliseconds * 10000;
							break;
						}
					}

					nextFrameWithTimeStamp--;
				}

				if (referenceFrameTicks != 0)
				{
					interpolatedTicks = referenceFrameTicks + (frameTimingIndex - referenceFrameNo) * referenceFrameDurationInTicks;	
				}
			}

		    return new DateTime(interpolatedTicks);
	    }

	    internal uint GetFrameNumberForFrameTicksFromFrameTiming(long ticks)
        {
            for (int i = 0; i < LcFile.FrameTiming.Count; i++)
            {
                long startTicks1 = LcFile.FrameTiming[i].FrameMidTime.AddMilliseconds(LcFile.FrameTiming[i].FrameDurationInMilliseconds / -2.0).Ticks;
                long startTicks2 = i + 1 != LcFile.FrameTiming.Count 
					? LcFile.FrameTiming[i + 1].FrameMidTime.AddMilliseconds(LcFile.FrameTiming[i + 1].FrameDurationInMilliseconds / -2.0).Ticks
					: LcFile.FrameTiming[i].FrameMidTime.AddMilliseconds(LcFile.FrameTiming[i].FrameDurationInMilliseconds / 2.0).Ticks;

	            if (startTicks1 <= ticks && ticks < startTicks2)
	            {
					if (startTicks1 == MISSING_TIMESTAMP_TICKS && startTicks2 != MISSING_TIMESTAMP_TICKS && i + 1 != LcFile.FrameTiming.Count)
					{
						// If this frame has a missing time, then we may have got the wrong frame
						int prevTimedFrameId = i;
						while (prevTimedFrameId > 0 && 
								(
									LcFile.FrameTiming[prevTimedFrameId].FrameMidTime.Ticks == MISSING_TIMESTAMP_TICKS || 
									LcFile.FrameTiming[prevTimedFrameId].FrameMidTime.Ticks > ticks
								)
							)
						{
							prevTimedFrameId--;
						}

						int minFrameId = i;
						long minDiff = long.MaxValue;
						 
						int durStart = LcFile.FrameTiming[prevTimedFrameId].FrameDurationInMilliseconds * 10000;
						long ticksStart = LcFile.FrameTiming[prevTimedFrameId].FrameMidTime.Ticks;

						for (int j = prevTimedFrameId; j <= i + 1; j++)
						{
							if (j >= 0 && j + 1 < LcFile.FrameTiming.Count)
							{
								long diff = Math.Abs(ticksStart + (j - prevTimedFrameId)*durStart - ticks);
								if (diff < minDiff)
								{
									minDiff = diff;
									minFrameId = j;
								}
							}
						}

						return (uint)minFrameId + MinFrame;
					} 
					
					return (uint) i + MinFrame;
	            }
            }

			if (ticks > LcFile.FrameTiming[LcFile.FrameTiming.Count - 1].FrameMidTime.AddMilliseconds(LcFile.FrameTiming[LcFile.FrameTiming.Count - 1].FrameDurationInMilliseconds / -2.0).Ticks)
                return (uint)LcFile.FrameTiming.Count + MinFrame;
            else
				return MinFrame;
        }

        // Not serialized. Used to track re-processing properties on the series only
        internal ProcessingType ProcessingType;
        internal int MinAdjustedReading;
        internal int MaxAdjustedReading;

        internal int BackgroundType;
        internal int FilterType;

        internal float PositionTolerance;

        internal string SourceInfo;

        private double m_FramesPerSecond;
        internal double FramesPerSecond
        {
            get
            {
                return m_FramesPerSecond;
            }
            set
            {
                m_FramesPerSecond = value;

                m_FrameExposureInMS = 1000.0 / m_FramesPerSecond;
            }
        }

		internal double SecondsPerFrameComputed()
		{
			if (TotalTimeStapmedTime != TimeSpan.Zero)
				return TotalTimeStapmedTime.TotalSeconds / TimedFrames;	
			else
				return double.NaN;
		}

        internal double ComputedFramesPerSecond
        {
            get
            {
                return (TimedFrames - 1) / TotalTimeStapmedTime.TotalSeconds;
            }
        }

        private double m_FrameExposureInMS;

        internal double FrameExposureInMS
        {
            get
            {
                return m_FrameExposureInMS;
            }
        }

        internal int[] PsfFitMatrixSizes;
        internal float[] MeasurementApertures;
        internal bool[] FixedApertureFlags;

        internal LightCurveReductionType ReductionType;

        private static int SERIALIZATION_VERSION = 3;

        internal LCMeasurementHeader(
            string pathToVideoFile, string sourceInfo,
            int firstFrameNo, int framesCount, double framesPerSecond,
            uint minFrame, uint maxFrame,
            uint measuredFrames, uint measurementInterval, byte objectCount,
			LightCurveReductionType reductionType, MeasurementTimingType timingType, int backgroundType, int filterType,
            int[] psfFitMatrixSizes, float[] measurementApertures, bool[] fixedApertureFlags, float positionTolerance)
        {
            m_FramesPerSecond = 25;
            m_FrameExposureInMS = 40;
            ProcessingType = ProcessingType.SignalMinusBackground;
            MinAdjustedReading = 0;
            MaxAdjustedReading = 0;

            FirstTimedFrameTime = DateTime.MaxValue;
            SecondTimedFrameTime = DateTime.MinValue;

            MinFrame = minFrame;
            MaxFrame = maxFrame;
            MeasuredFrames = measuredFrames;
            ObjectCount = objectCount;
            ReductionType = reductionType;

            PathToVideoFile = pathToVideoFile;
            SourceInfo = sourceInfo;
            FirstFrameInVideoFile = firstFrameNo;
            CountFrames = framesCount;

            MeasurementInterval = measurementInterval;
            BackgroundType = backgroundType;
            FilterType = filterType;

            PsfFitMatrixSizes = psfFitMatrixSizes;
            MeasurementApertures = measurementApertures;
            FixedApertureFlags = fixedApertureFlags;

            PositionTolerance = positionTolerance;

            FirstTimedFrameNo = (int)MinFrame;
            LastTimedFrameNo = (int)MaxFrame;

            if (FirstTimedFrameTime != DateTime.MaxValue &&
                SecondTimedFrameTime != DateTime.MinValue)
            {
                TotalTimeStapmedTime = new TimeSpan(SecondTimedFrameTime.Ticks - FirstTimedFrameTime.Ticks);
                TimedFrames = LastTimedFrameNo - FirstTimedFrameNo + 1;
            }
            else
            {
                TotalTimeStapmedTime = TimeSpan.Zero;
                TimedFrames = 1;
            }

			TimingType = timingType;
			LcFile = null;

            FramesPerSecond = framesPerSecond;        	
        }

        internal LCMeasurementHeader(BinaryReader reader)
        {
            m_FrameExposureInMS = 40;
            ProcessingType = ProcessingType.SignalMinusBackground;
            MinAdjustedReading = 0;
            MaxAdjustedReading = 0;

            int version = reader.ReadInt32();

            MinFrame = reader.ReadUInt32();
            MaxFrame = reader.ReadUInt32();
            MeasuredFrames = reader.ReadUInt32();
            ObjectCount = reader.ReadByte();
            ReductionType = (LightCurveReductionType)reader.ReadInt32();

            PathToVideoFile = reader.ReadString();
            SourceInfo = reader.ReadString();
            FirstFrameInVideoFile = reader.ReadInt32();
            CountFrames = reader.ReadInt32();

            FirstTimedFrameTime = new DateTime(reader.ReadInt64());
            SecondTimedFrameTime = new DateTime(reader.ReadInt64());
            m_FramesPerSecond = reader.ReadDouble();
            MeasurementInterval = reader.ReadUInt32();

            BackgroundType = reader.ReadInt32();
            FilterType = reader.ReadInt32();

            PsfFitMatrixSizes = new int[ObjectCount];
            MeasurementApertures = new float[ObjectCount];
            FixedApertureFlags = new bool[ObjectCount];

            for (int i = 0; i < ObjectCount; i++)
            {
                PsfFitMatrixSizes[i] = reader.ReadInt32();
                MeasurementApertures[i] = reader.ReadSingle();
                FixedApertureFlags[i] = reader.ReadBoolean();
            }

            PositionTolerance = reader.ReadSingle();

            FirstTimedFrameNo = (int)MinFrame;
            LastTimedFrameNo = (int)MaxFrame;
            if (version > 1)
            {
                FirstTimedFrameNo = reader.ReadInt32();
                LastTimedFrameNo = reader.ReadInt32();
                TimedFrames = LastTimedFrameNo - FirstTimedFrameNo + 1;

				if (version > 2)
				{
					TimingType = (MeasurementTimingType)reader.ReadByte();

				}
				else
				{
					TimingType = MeasurementTimingType.UserEnteredFrameReferences;
				}
            }
            else
            {
            	TimedFrames = (int) MaxFrame - (int) MinFrame + 1;
				TimingType = MeasurementTimingType.UserEnteredFrameReferences;
            }

            if (FirstTimedFrameTime != DateTime.MaxValue &&
                SecondTimedFrameTime != DateTime.MinValue)
            {
                TotalTimeStapmedTime = new TimeSpan(SecondTimedFrameTime.Ticks - FirstTimedFrameTime.Ticks);
            }
            else
                TotalTimeStapmedTime = TimeSpan.Zero;

			LcFile = null;					
        }

        internal void WriteTo(BinaryWriter writer)
        {
            writer.Write(SERIALIZATION_VERSION);

            writer.Write(MinFrame);
            writer.Write(MaxFrame);
            writer.Write(MeasuredFrames);
            writer.Write(ObjectCount);
            writer.Write((int)ReductionType);

            writer.Write(PathToVideoFile);
            writer.Write(SourceInfo);
            writer.Write(FirstFrameInVideoFile);
            writer.Write(CountFrames);

            writer.Write(FirstTimedFrameTime.Ticks);
            writer.Write(SecondTimedFrameTime.Ticks);
            writer.Write(FramesPerSecond);

            writer.Write(MeasurementInterval);
            writer.Write(BackgroundType);
            writer.Write(FilterType);

            for (int i = 0; i < ObjectCount; i++)
            {
                writer.Write(PsfFitMatrixSizes[i]);
                writer.Write(MeasurementApertures[i]);
                writer.Write(FixedApertureFlags[i]);
            }

            writer.Write(PositionTolerance);

            // VERSION 2 Data
            writer.Write(FirstTimedFrameNo);
            writer.Write(LastTimedFrameNo);

			// VERSION 3 Data
			writer.Write(((byte)TimingType));
        }
    }

    internal struct LCMeasurementFooter
    {
        public static LCMeasurementFooter Empty = new LCMeasurementFooter();

        private static int SERIALIZATION_VERSION = 6;

        internal byte[] AveragedFrameBytes;
    	internal int AveragedFrameWidth;
		internal int AveragedFrameHeight;
		internal int AveragedFrameBpp;

        internal TangraConfig ProcessedWithTangraConfig;
        internal LightCurveReductionContext ReductionContext;
        internal List<TrackedObjectConfig> TrackedObjects;
        internal float RefinedAverageFWHM;
        internal string TimestampOCR;
        internal Dictionary<int, long> ThumbPrintDict;
	    internal Dictionary<int, float> InstrumentalDelayConfig;
		internal string InstrumentalDelayConfigName;

        internal string CameraName;
        internal int AAVFrameIntegration; 

        internal LCMeasurementFooter(
			Pixelmap averagedFrame, 
            TangraConfig config, 
            LightCurveReductionContext reductionContext, 
            List<TrackedObjectConfig> trackedObjectConfigs,
            ITracker tracker,
            ITimestampOcr timestampOCR,
            Dictionary<int, long> thumbPrintDict,
			Dictionary<int, float> instrumentalDelayConfig,
			string instrumentalDelayConfigName,
            string cameraName,
            int aavFrameIntegration)
        {
			AveragedFrameBytes = averagedFrame.DisplayBitmapPixels;
        	AveragedFrameWidth = averagedFrame.Width;
			AveragedFrameHeight = averagedFrame.Height;
			AveragedFrameBpp = averagedFrame.BitPixCamera;

            ProcessedWithTangraConfig = config;
            ReductionContext = reductionContext;
            TrackedObjects = trackedObjectConfigs;

            RefinedAverageFWHM = tracker.RefinedAverageFWHM;
            TimestampOCR = timestampOCR != null ? timestampOCR.NameAndVersion() : string.Empty;
            ThumbPrintDict = thumbPrintDict;
			InstrumentalDelayConfig = instrumentalDelayConfig;
	        InstrumentalDelayConfigName = instrumentalDelayConfigName ?? string.Empty;
            CameraName = cameraName ?? string.Empty;
            AAVFrameIntegration = aavFrameIntegration;
        }

        internal LCMeasurementFooter(BinaryReader reader)
        {
            int version = reader.ReadInt32();

			// Old Bitmap format
			AveragedFrameWidth = reader.ReadInt32();
			AveragedFrameHeight = reader.ReadInt32();
			AveragedFrameBpp = reader.ReadInt32();
			int bytesCount = reader.ReadInt32();
			AveragedFrameBytes = reader.ReadBytes(bytesCount);

			string configString = reader.ReadString();
			try
			{
				ProcessedWithTangraConfig = configString.FromBase64String<TangraConfig>();
			}
			catch
			{
				ProcessedWithTangraConfig = new TangraConfig();
			}
			

			ReductionContext = LightCurveReductionContext.Load(reader);

			int numObjects = reader.ReadInt32();
			TrackedObjects = new List<TrackedObjectConfig>();
			for (int i = 0; i < numObjects; i++)
			{
				TrackedObjects.Add(TrackedObjectConfig.Load(reader));
			}

			RefinedAverageFWHM = float.NaN;
			TimestampOCR = string.Empty;
			ThumbPrintDict = new Dictionary<int, long>();
			InstrumentalDelayConfig = new Dictionary<int, float>();
	        InstrumentalDelayConfigName = string.Empty;
            CameraName = string.Empty;
            AAVFrameIntegration = -1;

			if (version > 1)
			{
				RefinedAverageFWHM = reader.ReadSingle();

				if (version > 2)
				{
					TimestampOCR = reader.ReadString();

					if (version > 3)
					{
						int numRecs = reader.ReadInt32();
						for (int i = 0; i < numRecs; i++)
						{
							int frameNo = reader.ReadInt32();
							long thumbprint = reader.ReadInt64();
							ThumbPrintDict.Add(frameNo, thumbprint);
						}

						if (version > 4)
						{
							numRecs = reader.ReadInt32();
							for (int i = 0; i < numRecs; i++)
							{
								int frameIntegration = reader.ReadInt32();
								float delayInMilliseconds = reader.ReadSingle();
								InstrumentalDelayConfig.Add(frameIntegration, delayInMilliseconds);
							}

							InstrumentalDelayConfigName = reader.ReadString();

                            if (version > 5)
                            {
                                CameraName = reader.ReadString();
                                AAVFrameIntegration = reader.ReadInt32();
                            }
						}
					}
				}
			}
        }

        internal void WriteTo(BinaryWriter writer)
        {
            writer.Write(SERIALIZATION_VERSION);

            writer.Write(AveragedFrameWidth);
            writer.Write(AveragedFrameHeight);
            writer.Write(AveragedFrameBpp);

			// Saving the display bitmap of the frame
			writer.Write(AveragedFrameBytes.Length);
			writer.Write(AveragedFrameBytes);

            string configString = ProcessedWithTangraConfig.AsBase64String();
            writer.Write(configString);

            ReductionContext.Save(writer);

            writer.Write(TrackedObjects.Count);
            foreach(TrackedObjectConfig obj in TrackedObjects)
            {
                obj.Save(writer);
            }

            writer.Write(RefinedAverageFWHM);

            writer.Write(TimestampOCR);

			if (ThumbPrintDict != null)
			{
				writer.Write(ThumbPrintDict.Count);
				foreach (int key in ThumbPrintDict.Keys)
				{
					writer.Write(key);
					writer.Write(ThumbPrintDict[key]);
				}
			}
			else
				writer.Write((int)0);

			if (InstrumentalDelayConfig != null)
			{
				writer.Write(InstrumentalDelayConfig.Count);
				foreach (int key in InstrumentalDelayConfig.Keys)
				{
					writer.Write(key);
					writer.Write(InstrumentalDelayConfig[key]);
				}
			}
			else
				writer.Write((int)0);

	        writer.Write(InstrumentalDelayConfigName);

            writer.Write(CameraName);
            writer.Write(AAVFrameIntegration);
        }
    }

    public class FileProgressManager
    {
        public static void BeginFileOperation(int maxSteps)
        {
            NotificationManager.Instance.NotifyFileProgressManagerBeginFileOperation(maxSteps);
        }

        public static void FileOperationProgress(int currentProgress)
        {
            NotificationManager.Instance.NotifyFileProgressManagerFileOperationProgress(currentProgress);
        }

        public static void EndFileOperation()
        {
            NotificationManager.Instance.NotifyFileProgressManagerEndFileOperation();
        }
    }
}
