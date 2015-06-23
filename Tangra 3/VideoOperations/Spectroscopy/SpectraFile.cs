using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Model.Helpers;
using Tangra.VideoOperations.Spectroscopy.Helpers;

namespace Tangra.VideoOperations.Spectroscopy
{
	public class SpectraFile
	{
		internal SpectraFileHeader Header;
		internal MasterSpectra Data;

		internal int SpectraFileFormatVersion;

        private static short SPECTRA_FILE_VERSION = 1;

		public static void Save(string fileName, SpectraFileHeader header, MasterSpectra data)
		{
			using (FileStream fileStr = new FileStream(fileName, FileMode.Create, FileAccess.Write))
			using (DeflateStream deflateStream = new DeflateStream(fileStr, CompressionMode.Compress, true))
			using (BinaryWriter writer = new BinaryWriter(deflateStream))
            {
				writer.Write(SPECTRA_FILE_VERSION);

                header.WriteTo(writer);

				if (data != null)
                {
                    ThreadingHelper.RunTaskWaitAndDoEvents(
                        delegate()
                            {
								data.WriteTo(writer);
                            },
                            150);
                }

                writer.Flush();
				deflateStream.Flush();
            }
		}

		public static SpectraFile Load(string fileName)
		{
			var fileInfo = new FileInfo(fileName);

            using (var inFile = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            using (var deflateStream = new DeflateStream(inFile, CompressionMode.Decompress, true))
            using (var reader = new BinaryReader(deflateStream))
            {
	            short version = reader.ReadInt16();
				if (version > SPECTRA_FILE_VERSION)
	            {
		            MessageBox.Show("This spectra file requires a newer version of Tangra.", "Tangra", MessageBoxButtons.OK, MessageBoxIcon.Error);
		            return null;
	            }

				var spectraFile = new SpectraFile
				{
					SpectraFileFormatVersion = version,
					Header = new SpectraFileHeader(reader)
				};

				spectraFile.Data = new MasterSpectra(reader);

				return spectraFile;
            }
		}
	}

	public struct SpectraFileHeader
	{
		public static SpectraFileHeader Empty = new SpectraFileHeader();

		public static bool IsEmpty(SpectraFileHeader compareTo)
		{
			return Empty.Equals(compareTo);
		}

		internal string PathToVideoFile;
        internal string ObjectName;
        internal string Telescope;
        internal string Instrument;
        internal string Recorder;
        internal string Observer;
        internal float RA;
        internal float DEC;
        internal float Longitude;
        internal float Latitude;
        internal int Width;
        internal int Height;
        internal int BitPix;
		internal uint DataAav16NormVal;
	    internal string SourceInfo;

		private static int SERIALIZATION_VERSION = 1;

		internal SpectraFileHeader(BinaryReader reader)
		{
			int version = reader.ReadInt32();

			PathToVideoFile = reader.ReadString();
            ObjectName = reader.ReadString();
            Telescope = reader.ReadString();
            Instrument = reader.ReadString();
            Recorder = reader.ReadString();
            Observer = reader.ReadString();
            RA = reader.ReadSingle();
            DEC = reader.ReadSingle();
            Longitude = reader.ReadSingle();
            Latitude = reader.ReadSingle();
            Width = reader.ReadInt32();
            Height = reader.ReadInt32();
            BitPix = reader.ReadInt32();
			DataAav16NormVal = reader.ReadUInt32();
		    SourceInfo = reader.ReadString();
		}

		internal void WriteTo(BinaryWriter writer)
		{
			writer.Write(SERIALIZATION_VERSION);

			writer.Write(PathToVideoFile);
		    writer.Write(ObjectName);
            writer.Write(Telescope);
            writer.Write(Instrument);
            writer.Write(Recorder);
            writer.Write(Observer);
            writer.Write(RA);
            writer.Write(DEC);
            writer.Write(Longitude);
            writer.Write(Latitude);
            writer.Write(Width);
            writer.Write(Height);
            writer.Write(BitPix);
			writer.Write(DataAav16NormVal);
            writer.Write(SourceInfo);
		}
	}
}
