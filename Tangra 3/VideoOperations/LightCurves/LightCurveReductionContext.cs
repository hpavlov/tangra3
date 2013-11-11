using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

using Tangra.Config;
using Tangra.Model.Config;
using Tangra.Model.Image;
using Tangra.Model.Video;
using Tangra.VideoOperations.LightCurves.Tracking;


namespace Tangra.VideoOperations.LightCurves
{
    public enum LightCurveReductionType
    {
        UntrackedMeasurement,
        MutualEvent,
        Asteroidal,
        TotalLunarDisappearance,
		TotalLunarReppearance
    }

    public class LightCurveReductionContext
    {
        private static byte VERSION = 7;

        public static LightCurveReductionContext Instance = new LightCurveReductionContext();

        public TangraConfig.PreProcessingFilter DigitalFilter;
        public TangraConfig.BackgroundMethod  NoiseMethod;
        public LightCurveReductionType LightCurveReductionType;
        public TangraConfig.PhotometryReductionMethod ReductionMethod;
        public bool FullDisappearance;
        public bool HighFlickering;
        public bool WindOrShaking;
        public bool AllFaintStars;
        public bool StopOnLostTracking;
        public bool FieldRotation;

        private TangraConfig.ColourChannel m_ColourChannel;

        public TangraConfig.ColourChannel ColourChannel
        {
            get { return m_ColourChannel; }
            set
            {
                m_ColourChannel = value;
            }
        }

        public bool IsColourVideo;
    	public bool IsDriftThrough;

        public bool UseStretching;
        public bool UseClipping;
        public bool UseBrightnessContrast;
        public byte FromByte;
        public byte ToByte;
        public int Brightness;
        public int Contrast;

    	private int m_BitPix;
    	public int BitPix    	
		{
    		get { return m_BitPix; }
    		set
    		{
    			m_BitPix = value;
				DisplayBitmapConverterImpl = DisplayBitmapConverter.ConstructConverter(m_BitPix, null);
			}
    	}

    	public bool HasEmbeddedTimeStamps { get; set; }

    	public uint MaxPixelValue = 256;
    	public IDisplayBitmapConverter DisplayBitmapConverterImpl = new DisplayBitmapConverter.DefaultDisplayBitmapConverter();

        public Rectangle OSDFrame;

        private FrameIntegratingMode m_FrameIntegratingMode;

        public FrameIntegratingMode FrameIntegratingMode
        {
            get { return m_FrameIntegratingMode; }
            set
            {
                m_FrameIntegratingMode = value;
            }
        }

        public PixelIntegrationType PixelIntegrationType;
        public int NumberFramesToIntegrate;

        public bool DebugTracking = false;

        public bool SaveTrackingSession = false;        

        public bool AllowsManuallyPlacedAperture
        {
            get { return ReductionMethod == TangraConfig.PhotometryReductionMethod.AperturePhotometry; }
        }

	    public string UsedTracker = null;

	    internal string ValidateObjects(List<TrackedObjectConfig> selectedObjects)
        {
            if (LightCurveReductionType == LightCurveReductionType.MutualEvent)
            {
                if (selectedObjects.Count > 1)
                {
                    bool guidingStarFound = false;
                    foreach (TrackedObjectConfig obj in selectedObjects)
                    {
                        if (obj.TrackingType == TrackingType.GuidingStar)
                        {
                            guidingStarFound = true;
                            break;
                        }
                    }

                    if (!guidingStarFound)
                        return "When more than one object is selected, at least one of them must be a guiding star.";
                }
            }
            else if (LightCurveReductionType == LightCurveReductionType.Asteroidal)
            {
                int occultedStarsFound = 0;
                foreach (TrackedObjectConfig obj in selectedObjects)
                {
                    if (obj.TrackingType == TrackingType.OccultedStar)
                    {
                        occultedStarsFound++;
                    }
                }

                if (occultedStarsFound == 0)
                    return "Plese specify which object will be occulted.";
                else if (occultedStarsFound > 1)
                    return "Only one of the objects can be marked as 'Occulted Star'.";

            }

            return null;
        }

        internal int GetColourByteOffset24bbp()
        {
            return ImageFilters.GetColourByteOffset24bbp(ColourChannel);
        }

        internal int GetColourByteOffset32bbp()
        {
            return ImageFilters.GetColourByteOffset32bbp(ColourChannel);
        }

        internal void Save(BinaryWriter fileWriter)
        {
            fileWriter.Write(VERSION);

            fileWriter.Write((int)DigitalFilter);
            fileWriter.Write((int)NoiseMethod);
            fileWriter.Write((int)LightCurveReductionType);
            fileWriter.Write((int)ReductionMethod);
            fileWriter.Write(FullDisappearance);
            fileWriter.Write(HighFlickering);
            fileWriter.Write(WindOrShaking);
            fileWriter.Write(AllFaintStars);
            fileWriter.Write((int)FrameIntegratingMode);
            fileWriter.Write((int)PixelIntegrationType);
            fileWriter.Write(NumberFramesToIntegrate);

            // Version 2 Data
            fileWriter.Write(IsColourVideo);
            fileWriter.Write((int)ColourChannel);
            fileWriter.Write(OSDFrame.Left);
            fileWriter.Write(OSDFrame.Top);
            fileWriter.Write(OSDFrame.Width);
            fileWriter.Write(OSDFrame.Height);

            // Version 3 Data
            fileWriter.Write(UseStretching);
            fileWriter.Write(UseClipping);
            fileWriter.Write(UseBrightnessContrast);
            fileWriter.Write(FromByte);
            fileWriter.Write(ToByte);
            fileWriter.Write(Brightness);
            fileWriter.Write(Contrast);

			// Version 4 Data
			fileWriter.Write(IsDriftThrough);

			// Version 5 Data
			fileWriter.Write(BitPix);
			fileWriter.Write(MaxPixelValue);
            string config = DisplayBitmapConverterImpl.GetConfig();
            fileWriter.Write(config);

			// Version 6 Data
			fileWriter.Write(HasEmbeddedTimeStamps);

			// Version 7 Data
			fileWriter.Write(UsedTracker ?? string.Empty);			
        }

        internal static LightCurveReductionContext Load(BinaryReader reader)
        {
            LightCurveReductionContext instance = new LightCurveReductionContext();
            byte version = reader.ReadByte();
            if (version > 0)
            {
                instance.DigitalFilter = (TangraConfig.PreProcessingFilter)reader.ReadInt32();
                instance.NoiseMethod = (TangraConfig.BackgroundMethod)reader.ReadInt32();
                instance.LightCurveReductionType = (LightCurveReductionType)reader.ReadInt32();
                instance.ReductionMethod = (TangraConfig.PhotometryReductionMethod)reader.ReadInt32();

                instance.FullDisappearance = reader.ReadBoolean();
                instance.HighFlickering = reader.ReadBoolean();
                instance.WindOrShaking = reader.ReadBoolean();
                instance.AllFaintStars = reader.ReadBoolean();

                instance.FrameIntegratingMode = (FrameIntegratingMode)reader.ReadInt32();
                instance.PixelIntegrationType = (PixelIntegrationType)reader.ReadInt32();
                instance.NumberFramesToIntegrate = reader.ReadInt32();

				#region Default values of properties added after V1
				instance.BitPix = 8;
				instance.MaxPixelValue = 255;
				instance.DisplayBitmapConverterImpl = DisplayBitmapConverter.Default;
				instance.HasEmbeddedTimeStamps = false;
	            instance.UsedTracker = null;
				#endregion 

                if (version > 1)
                {
                    instance.IsColourVideo = reader.ReadBoolean();
                    instance.ColourChannel = (TangraConfig.ColourChannel)reader.ReadInt32();
                    int left = reader.ReadInt32();
                    int top = reader.ReadInt32();
                    int width = reader.ReadInt32();
                    int height = reader.ReadInt32();
                    instance.OSDFrame = new Rectangle(left, top, width, height);

                    if (version > 2)
                    {
                        instance.UseStretching = reader.ReadBoolean();
                        instance.UseClipping = reader.ReadBoolean();
                        instance.UseBrightnessContrast = reader.ReadBoolean();
                        instance.FromByte = reader.ReadByte();
                        instance.ToByte = reader.ReadByte();
                        instance.Brightness = reader.ReadInt32();
                        instance.Contrast = reader.ReadInt32();

						if (version > 3)
						{
							instance.IsDriftThrough = reader.ReadBoolean();

							if (version > 4)
							{
								instance.BitPix = reader.ReadInt32();
								instance.MaxPixelValue = reader.ReadUInt32();
								string config = reader.ReadString();
								instance.DisplayBitmapConverterImpl = DisplayBitmapConverter.ConstructConverter(instance.BitPix, config);

								if (version > 5)
								{
									instance.HasEmbeddedTimeStamps = reader.ReadBoolean();

									if (version > 6)
									{
										instance.UsedTracker = reader.ReadString();
									}
								}
							}
						}
                    }
                }
            }

            return instance;
        }

		internal int GetMaxApertureSize()
		{
			return LightCurveReductionType == LightCurveReductionType.UntrackedMeasurement
				? 9 /* 17 would be the desired size, but it requires a fait bit of work */ :
						ReductionMethod == TangraConfig.PhotometryReductionMethod.AperturePhotometry
					   ? 9
					   : 7;
		}
    }
}
