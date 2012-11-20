using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Tangra.SDK
{
	public interface ITangraLightCurve
	{
		ITangraLightCurveInfo GenericInfo { get; }
		ITangraLightCurveProcessingInfo ProcessingInfo { get; }
        ITangraLightCurveContext State { get; }

		byte ObjectCount { get; }
		ITangraMeasuredObjectInfo[] MeasuredObjectsInfo { get; }

		List<ITangraLightCurveObjectMeasurement> GetRawMeasurementsForObject(int objectId);

		List<ITangraLightCurveDataPoint> GetDataPointsForObject(int objectId);

        DateTime GetTimeForFrame(uint frameNo);
        void SelectFrame(uint frameNo);
	}

	public interface ITangraLightCurveDataPoint
	{
		uint FrameNo { get; }		
		int BinNo { get; }
		float Value { get; }
	}

	public interface ITangraLightCurveObjectMeasurement
	{
		uint FrameNo { get; }
		byte TargetNo { get; }

		uint RawReading { get; }
		uint RawBackground { get; }

		byte Flags1{ get; }
		uint Flags2 { get; }

		string ExplainFlags();

		bool IsSuccessfulReading { get; }
		bool IsOffScreen { get; }
	}

	public enum LightCurveReductionType
	{
		UntrackedMeasurement,
		MutualEvent,
		Asteroidal,
		TotalLunarDisappearance
	}

	public enum DisplayedValueType
	{
		SignalOnly,
		SignalMinusBackground,
		BackgroundOnly,
		SignalDividedByBackground,
		SignalDividedByNoise
	}

	public enum ColourChannel
	{
		Red,
		Green,
		Blue,
		GrayScale
	}

	public enum BackgroundMethod
	{
		AverageBackground,
		BackgroundMode,
		BackgroundGradientFit,
		PSFBackground
	}

	public enum PreProcessingFilter
	{
		NoFilter,
		LowPassFilter,
		LowPassDifferenceFilter
	}

	public enum PhotometryReductionMethod
	{
		AperturePhotometry,
		PsfPhotometryNumerical,
		PsfPhotometryAnalytical,
		OptimalExtraction
	}

    public interface ITangraLightCurveContext
    {
        DisplayedValueType DisplayedValueType { get; }
        int MinAdjustedReading { get; }
        int MaxAdjustedReading { get; }

        int NormalizationObject { get; }
        byte BinSize { get; }

        int SelectedFrameNo { get; }
    }

	public interface ITangraLightCurveProcessingInfo
	{
		float RefinedAverageFWHM { get; }

		PreProcessingFilter DigitalFilter { get; }
		BackgroundMethod BackgroundMethod { get; }

		PhotometryReductionMethod SignalMethod { get; }

		ColourChannel ColourChannel { get; }

		bool IsColourVideo { get; }
		
		bool UseStretching { get; }
		bool UseClipping { get; }
		bool UseBrightnessContrast { get; }
		byte StretchingOrClippingFromByte { get; }
		byte StretchingOrClippingToByte { get; }
		int Brightness { get; }
		int Contrast { get; }
	}

	public interface ITangraLightCurveInfo
	{
		uint FirstVideoFrame { get; }
		uint LastVideoFrame { get; }		

		int FirstFrameInVideoFile { get; }

		DateTime FirstTimedFrameTime { get; }
		DateTime SecondTimedFrameTime { get; }

		int FirstTimedFrameNo { get; }
		int LastTimedFrameNo { get; }

		float OccultedStarPositionTolerance { get; }

		string SourceInfo { get; }

		double FramesPerSecondFromVideo { get; }

		double SecondsPerFrameComputed { get; }

		double FramesPerSecondComputed { get; }

		double FrameExposureInMS { get; }

		LightCurveReductionType ReductionType { get; }

		Bitmap AveragedFrame { get; }

		bool FullDisappearance { get; }
		bool HighFlickering { get; }
		bool WindOrShaking { get; }
		bool AllFaintStars { get; }
		bool StopOnLostTracking { get; }
		bool FieldRotation { get; }
		bool IsDriftThrough { get; }
	}

	public enum ObjectType
	{
		GuidingStar,
		ComparisonStar,
		OccultedStar
	}

	public interface ITangraMeasuredObjectInfo
	{
		int PsfFitMatrixSize { get; }
		bool IsFixedAperture { get; }

		ObjectType ObjectType { get; }
        bool MeasureThisObject{ get; }
        float ApertureInPixels{ get; }
        
        float ApertureStartingX { get; }
        float ApertureStartingY { get; }

        float PositionTolerance{ get; }
        bool IsWeakSignalObject{ get; }
        
        float RefinedFWHM { get; }
	}
}
