using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using Tangra.Model.Helpers;

namespace Tangra.VideoOperations.Spectroscopy.FilterResponses
{
	public enum PhotometricBand
	{
		Unknown,
		U,
		B,
		V,
		I,
		R,
		Sloan_u,
		Sloan_g,
		Sloan_r,
		Sloan_i,
		Sloan_z
	}

	public class FilterResponse
	{
		public string Designation;
		public Dictionary<int, double> Response = new Dictionary<int, double>();

		public PhotometricBand Band
		{
			get
			{
				if (Designation == "U") return PhotometricBand.U;
				if (Designation == "B") return PhotometricBand.B;
				if (Designation == "V") return PhotometricBand.V;
				if (Designation == "R") return PhotometricBand.R;
				if (Designation == "I") return PhotometricBand.I;
				if (Designation == "u'") return PhotometricBand.Sloan_u;
				if (Designation == "g'") return PhotometricBand.Sloan_g;
				if (Designation == "r'") return PhotometricBand.Sloan_r;
				if (Designation == "i'") return PhotometricBand.Sloan_i;
				if (Designation == "z'") return PhotometricBand.Sloan_z;
				return PhotometricBand.Unknown;
			}
		}
	}

	public class FilterResponseDatabase
	{
		public FilterResponseDatabase()
		{ }

		private static FilterResponseDatabase s_Instance;
		private static object s_SyncRoot = new object();

		public static FilterResponseDatabase Instance
		{
			get
			{
				if (s_Instance == null)
				{
					lock (s_SyncRoot)
					{
						if (s_Instance == null)
						{
							using (Stream compressedStream = AssemblyHelper.GetEmbededResourceStreamThatClientMustDispose("Tangra.VideoOperations.Spectroscopy.FilterResponses", "FilterResponseDb.dat"))
							using (var deflateStream = new DeflateStream(compressedStream, CompressionMode.Decompress, true))
							{
								using (var reader = new BinaryReader(deflateStream))
								{
									var db = new FilterResponseDatabase(reader);
									Thread.MemoryBarrier();
									s_Instance = db;
								}
							}
						}
					}
				}

				return s_Instance;
			}
		}

		public FilterResponse Johnson_V;
		public FilterResponse Johnson_B;
		public FilterResponse Johnson_R;
		public FilterResponse Johnson_U;
		public FilterResponse Johnson_I;

		public FilterResponse SLOAN_u;
		public FilterResponse SLOAN_g;
		public FilterResponse SLOAN_r;
		public FilterResponse SLOAN_i;
		public FilterResponse SLOAN_z;

		private FilterResponseDatabase(BinaryReader reader)
		{
			Johnson_V = LoadFilterResponse(reader);
			Johnson_B = LoadFilterResponse(reader);
			Johnson_R = LoadFilterResponse(reader);
			Johnson_U = LoadFilterResponse(reader);
			Johnson_I = LoadFilterResponse(reader);

			SLOAN_u = LoadFilterResponse(reader);
			SLOAN_g = LoadFilterResponse(reader);
			SLOAN_r = LoadFilterResponse(reader);
			SLOAN_i = LoadFilterResponse(reader);
			SLOAN_z = LoadFilterResponse(reader);
		}

		internal void Save(string filePath)
		{
			using(FileStream fs = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write))
			using (DeflateStream deflateStream = new DeflateStream(fs, CompressionMode.Compress, true))
			using (BinaryWriter wrt = new BinaryWriter(deflateStream))
			{
				SaveFilterResponse(Johnson_V, wrt);
				SaveFilterResponse(Johnson_B, wrt);
				SaveFilterResponse(Johnson_R, wrt);
				SaveFilterResponse(Johnson_U, wrt);
				SaveFilterResponse(Johnson_I, wrt);

				SaveFilterResponse(SLOAN_u, wrt);
				SaveFilterResponse(SLOAN_g, wrt);
				SaveFilterResponse(SLOAN_r, wrt);
				SaveFilterResponse(SLOAN_i, wrt);
				SaveFilterResponse(SLOAN_z, wrt);

				deflateStream.Flush();
			}
		}

		private void SaveFilterResponse(FilterResponse data, BinaryWriter wrt)
		{
			wrt.Write(data.Designation);
			wrt.Write(data.Response.Count);
			foreach (int key in data.Response.Keys)
			{
				wrt.Write(key);
				wrt.Write(data.Response[key]);
			}
		}

		private FilterResponse LoadFilterResponse(BinaryReader rdr)
		{
			var rv = new FilterResponse();
			rv.Designation = rdr.ReadString();
			int count = rdr.ReadInt32();
			for (int i = 0; i < count; i++)
			{
				int wavelength = rdr.ReadInt32();
				double response = rdr.ReadDouble();

				rv.Response.Add(wavelength, response);
			}

			return rv;
		}
	}
}
