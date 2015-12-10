using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using Tangra.Model.Helpers;

namespace Tangra.VideoOperations.Spectroscopy.AbsFluxCalibration
{
    public class CalSpecDatabase
    {
        public List<CalSpecStar> Stars = new List<CalSpecStar>();

        internal CalSpecDatabase()
        { }

        internal CalSpecDatabase(BinaryReader reader)
        {
            int count = reader.ReadInt32();
            Stars.Clear();

            for (int i = 0; i < count; i++)
            {
                var star = new CalSpecStar(reader);
                Stars.Add(star);
            }
        }

        internal void Serialize(BinaryWriter writer)
        {
            writer.Write(Stars.Count);

            foreach (CalSpecStar star in Stars)
            {
                star.Serialize(writer);
            }
        }

        private static CalSpecDatabase s_Instance;
        private static object s_SyncRoot = new object();

        public static CalSpecDatabase Instance
        {
			get
			{
                if (s_Instance == null)
                {
                    lock (s_SyncRoot)
                    {
                        if (s_Instance == null)
                        {
                           using (Stream compressedStream = AssemblyHelper.GetEmbededResourceStreamThatClientMustDispose("Tangra.VideoOperations.Spectroscopy.AbsFluxCalibration.Standards", "CalSpec.db"))
                           using (var deflateStream = new DeflateStream(compressedStream, CompressionMode.Decompress, true))
                           {
                               using (var reader = new BinaryReader(deflateStream))
                               {
                                   var db = new CalSpecDatabase(reader);
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
    }

    public class CalSpecStar
    {
        public string CalSpecStarId;
        public string AbsFluxStarId;
        public string TYC2;
        public string U4;
        public double RA_J2000_Hours;
        public double DE_J2000_Deg;
        public double pmRA;
        public double pmDE;
        public double MagB;
		public double MagV;
		public double MagR;
		public double Mag_g;
		public double Mag_r;
		public double Mag_i;
        public double MagBV;
        public string SpecType;
        public string STIS_Flag;
        public string FITS_File;

		public Dictionary<double, double> DataPoints = new Dictionary<double, double>();

        internal  CalSpecStar()
        { }

        internal CalSpecStar(BinaryReader reader)
        {
            CalSpecStarId = reader.ReadString();
            AbsFluxStarId = reader.ReadString();
            TYC2 = reader.ReadString();
            U4 = reader.ReadString();
            RA_J2000_Hours = reader.ReadDouble();
            DE_J2000_Deg = reader.ReadDouble();
            pmRA = reader.ReadDouble();
            pmDE = reader.ReadDouble();
            MagV = reader.ReadDouble();
            MagBV = reader.ReadDouble();
			MagB = reader.ReadDouble();
			MagR = reader.ReadDouble();
			Mag_g = reader.ReadDouble();
			Mag_r = reader.ReadDouble();
			Mag_i = reader.ReadDouble();
            SpecType = reader.ReadString();
            STIS_Flag = reader.ReadString();
            FITS_File = reader.ReadString();

            int cnt = reader.ReadInt32();
            DataPoints.Clear();
            for (int i = 0; i < cnt; i++)
            {
                double wavelength = reader.ReadDouble();
                double flux = reader.ReadDouble();
                DataPoints.Add(wavelength, flux);
            }
        }

        internal void Serialize(BinaryWriter writer)
        {
            writer.Write(CalSpecStarId);
            writer.Write(AbsFluxStarId);
            writer.Write(TYC2);
            writer.Write(U4);
            writer.Write(RA_J2000_Hours);
            writer.Write(DE_J2000_Deg);
            writer.Write(pmRA);
            writer.Write(pmDE);
            writer.Write(MagV);
            writer.Write(MagBV);
			writer.Write(MagB);
			writer.Write(MagR);
			writer.Write(Mag_g);
			writer.Write(Mag_r);
			writer.Write(Mag_i);
            writer.Write(SpecType);
            writer.Write(STIS_Flag);
            writer.Write(FITS_File);

            writer.Write(DataPoints.Count);
            foreach (double key in DataPoints.Keys)
            {
				writer.Write(key);
                writer.Write(DataPoints[key]);
            }
        }
    }
}
