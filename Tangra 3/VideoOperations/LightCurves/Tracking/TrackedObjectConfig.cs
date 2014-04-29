using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Tangra.Model.Astro;
using Tangra.Model.Image;


namespace Tangra.VideoOperations.LightCurves.Tracking
{
    public enum TrackingType
    {
        GuidingStar,
        ComparisonStar,
        OccultedStar
    }

	internal class TrackedObjectConfig : ITrackedObjectConfig
    {
		public TrackingType TrackingType { get; set; }
		public bool MeasureThisObject;
		public float ApertureInPixels { get; set; }
		public int PsfFitMatrixSize { get; set; }

		public float ApertureStartingX { get; set; }
		public float ApertureStartingY { get; set; }

        public int OriginalFieldCenterX;
        public int OriginalFieldCenterY;

		public PSFFit Gaussian;

		public float PositionTolerance { get; set; }
		public bool IsWeakSignalObject { get; set; }
		public bool IsFixedAperture { get; set; }
		public bool ProcessInGroup { get; set; }

        public float ApertureMatrixX0;
        public float ApertureMatrixY0;
        public float ApertureDX;
        public float ApertureDY;

        public float RefinedFWHM  { get; set; }

        public List<PSFFit> AutoStarsInArea = new List<PSFFit>();
 
		public bool IsCloseToOtherStars
		{
			get { return AutoStarsInArea.Count > 1; }
		}

        public ImagePixel AsImagePixel
        {
            get
            {
                if (Gaussian != null)
                    return new ImagePixel(Gaussian.XCenter, Gaussian.YCenter);
                else
                    return new ImagePixel((int)ApertureStartingX, (int)ApertureStartingY);
            }
        }

        public byte GaussianPeak
        {
            get
            {
                if (Gaussian != null)
                    return (byte)Math.Round(Gaussian.IMax - Gaussian.I0);
                else
                    return 0;
            }
        }

		public TrackedObjectConfig()
		{
			RefinedFWHM = float.NaN;
		}

        private static byte VERSION = 4;

        internal void Save(BinaryWriter fileWriter)
        {
            fileWriter.Write(VERSION);

            fileWriter.Write((int)TrackingType);
            fileWriter.Write(MeasureThisObject);
            fileWriter.Write(ApertureInPixels);
            fileWriter.Write(PsfFitMatrixSize);
            fileWriter.Write(ApertureStartingX);
            fileWriter.Write(ApertureStartingY);
            fileWriter.Write(OriginalFieldCenterX);
            fileWriter.Write(OriginalFieldCenterY);

            fileWriter.Write(Gaussian != null);
            if (Gaussian != null) Gaussian.Save(fileWriter);

            fileWriter.Write(PositionTolerance);
            fileWriter.Write(IsWeakSignalObject);
            fileWriter.Write(ApertureMatrixX0);
            fileWriter.Write(ApertureMatrixY0);
            fileWriter.Write(ApertureDX);
            fileWriter.Write(ApertureDY);

            fileWriter.Write(AutoStarsInArea.Count);
            foreach (PSFFit fit in AutoStarsInArea) fit.Save(fileWriter);

            // Save Version 2 data
            fileWriter.Write(RefinedFWHM);

			// Save Version 3 data
			fileWriter.Write(IsFixedAperture);

			// Save Version 4 data
			fileWriter.Write(ProcessInGroup);			
        }

        internal static TrackedObjectConfig Load(BinaryReader reader)
        {
            TrackedObjectConfig instance = new TrackedObjectConfig();

            byte version = reader.ReadByte();
            if (version > 0)
            {
                instance.TrackingType = (TrackingType)reader.ReadInt32();

                instance.MeasureThisObject = reader.ReadBoolean();
                instance.ApertureInPixels = reader.ReadSingle();
                instance.PsfFitMatrixSize = reader.ReadInt32();
                instance.ApertureStartingX = reader.ReadSingle();
                instance.ApertureStartingY = reader.ReadSingle();
                instance.OriginalFieldCenterX = reader.ReadInt32();
                instance.OriginalFieldCenterY = reader.ReadInt32();

                bool hasGaussian = reader.ReadBoolean();
                if (hasGaussian)
                    instance.Gaussian = PSFFit.Load(reader);

                instance.PositionTolerance = reader.ReadSingle();
                instance.IsWeakSignalObject = reader.ReadBoolean();
                instance.ApertureMatrixX0 = reader.ReadSingle();
                instance.ApertureMatrixY0 = reader.ReadSingle();
                instance.ApertureDX = reader.ReadSingle();
                instance.ApertureDY = reader.ReadSingle();

                instance.AutoStarsInArea = new List<PSFFit>();
                int autoStarsInArea = reader.ReadInt32();
                for (int i = 0; i < autoStarsInArea; i++)
                {
                    instance.AutoStarsInArea.Add(PSFFit.Load(reader));
                }

                if (version > 1)
                {
                    instance.RefinedFWHM = reader.ReadSingle();

					if (version > 2)
					{
						instance.IsFixedAperture = reader.ReadBoolean();

						if (version > 3)
						{
							instance.ProcessInGroup = reader.ReadBoolean();
						}						
					}
                }
            }

            return instance;
        }
    }
}
