using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Tangra.Model.Astro;
using Tangra.Model.Image;
using Tangra.Model.VideoOperations;

namespace Tangra.VideoOperations.LightCurves.Tracking
{
    internal class LocationVector
    {
        public double DeltaXToAdd;
        public double DeltaYToAdd;
    }

    internal class TrackedObject : IMeasuredObject
    {
        public readonly TrackedObjectConfig OriginalObject;

        private List<ImagePixel> RefiningPositions = new List<ImagePixel>();
        private List<float> RefiningSignalLevels = new List<float>();
        private List<double> RefiningFWHMs = new List<double>();

        public PSFFit ThisFrameFit = null;
        public float ThisFrameX = float.NaN;
        public float ThisFrameY = float.NaN;
        public float ThisSignalLevel = 0;
        public float LastFrameX;
        public float LastFrameY;
        public float LastSignalLevel = 1;

        public PSFFit PSFFit
        {
            get { return ThisFrameFit; }
        }

    	public NotMeasuredReasons NotMeasuredReasons;

    	private bool m_IsLocated = false;
    	public bool IsLocated
    	{
			get { return m_IsLocated; }
    	}

		public bool IsOffScreen
		{
			get
			{
				return (NotMeasuredReasons & NotMeasuredReasons.ObjectExpectedPositionIsOffScreen) != 0;
			}
			
		}

		public void SetIsLocated(bool isLocated, NotMeasuredReasons reason)
		{
			m_IsLocated = isLocated;

			// Overwrite only the same level reason and keep the other level so we can have 2 reasons
			if (reason <=  NotMeasuredReasons.LAST_FIRST_LEVEL_REASON)
				NotMeasuredReasons = (NotMeasuredReasons)((int)NotMeasuredReasons & 0xFF00) | reason;
			else
				NotMeasuredReasons = (NotMeasuredReasons)((int)NotMeasuredReasons & 0xFF00FF) | reason;
		}

		public void SetIsMeasured(bool isMeasured, NotMeasuredReasons reason)
		{
			m_IsLocated = isMeasured;
			NotMeasuredReasons = (NotMeasuredReasons)((int)NotMeasuredReasons & 0x00FFFF) | reason;
		}

		public byte GetLCMeasurementByteFlags()
		{
            // 11111111

            // NOT RECORDED REASONS:
            // NoPixelsToMeasure = 128 + 0,
            // ObjectExpectedPositionIsOffScreen = 8

            // xxx [0][1][2] MASK = 0x7
            // UnknownReason = 0
		    // TrackedSuccessfully = 1
		    // FixedObject = 2
		    // GuidingStarBrightnessFluctoationTooHigh = 3
		    // PSFFittingFailed = 4
		    // FoundObjectNotWithInExpectedPositionTolerance = 5
		    // FullyDisappearingStarMarkedTrackedWithoutBeingFound = 6
		    // FitSuspectAsNoGuidingStarsAreLocated = 7

            // xxx [3][4][5] MASK = 0x7
            // No Second Level Reason = 0
		    // TrackedSuccessfullyAfterDistanceCheck = 1     /* Flag = 9  | 8 + 1 | 8 + 6 = 14*/
			// TrackedSuccessfullyAfterWiderAreaSearch = 2   /* Flag = 17 | 16 + 1 | 16 + 6 = 22*/
			// TrackedSuccessfullyAfterStarRecognition = 3   /* Flag = 25 | 24 + 1 | 24 + 6 = 30*/
            // FailedToLocateAfterDistanceCheck = 4
            // FailedToLocateAfterWiderAreaSearch = 5
            // FailedToLocateAfterStarRecognition = 6

            // xx [6][7] MASK = 0x3
            // No Third Level Reason = 0
            // MeasurementPSFFittingFailed = 1
		    // DistanceToleranceTooHighForNonFullDisappearingOccultedStar = 2
		    // FWHMOutOfRange = 3

		    int firstLevel = (int) NotMeasuredReasons & 0x7;
            int secondLevel = (((int)NotMeasuredReasons & 0xFF00) >> 8) & 0x7;
			int thirdLevel = ((((int)NotMeasuredReasons & 0xFF0000) >> 16) & 0x7);
			if (thirdLevel > 0) thirdLevel -= 1/* We don't save NoPixelsToMeasure, this is why we subtrackt 1 */ ;

            int flags = firstLevel + (secondLevel << 3) + (thirdLevel << 6);

#if !PRODUCTION
			//Trace.Assert(flags >= 0 && flags <= 256);
			
			//int firstLevel2 = (int)flags & 0x7;
			//int secondLevel2 = ((int)flags >> 3) & 0x7;
			//int thirdLevel2 = ((int)flags >> 6) & 0x3;
			
			//Trace.Assert(firstLevel == firstLevel2 && secondLevel == secondLevel2 && thirdLevel == thirdLevel2);
			
			//NotMeasuredReasons firstFlags = (NotMeasuredReasons)firstLevel2;
			//NotMeasuredReasons secondFlags = (NotMeasuredReasons)(secondLevel2 << 8);
			//NotMeasuredReasons thirdFlags = (NotMeasuredReasons)((thirdLevel2 != 0 ? thirdLevel2 + 1 : thirdLevel2) /* We don't save NoPixelsToMeasure, this is why we add 1 */ << 16);

			//Trace.Assert((firstFlags | secondFlags | thirdFlags) == NotMeasuredReasons);
#endif
			
		    return (byte) flags;
		}

		private static void DecodeByteFlags(
			byte flags, 
			out NotMeasuredReasons firstFlags,
			out NotMeasuredReasons secondFlags,
			out NotMeasuredReasons thirdFlags)
		{
			int firstLevel = (int)flags & 0x7;
			int secondLevel = ((int)flags >> 3) & 0x7;
			int thirdLevel = ((int)flags >> 6) & 0x3;

			firstFlags = (NotMeasuredReasons)firstLevel;
			secondFlags = (NotMeasuredReasons)(secondLevel << 8);
			thirdFlags = (NotMeasuredReasons)((thirdLevel != 0 ? thirdLevel + 1 : thirdLevel) /* We don't save NoPixelsToMeasure, this is why we add 1 */ << 16);
		}

        public static string GetByteFlagsExplained(byte flags)
        {
			NotMeasuredReasons firstFlags, secondFlags, thirdFlags;
        	DecodeByteFlags(flags, out firstFlags, out secondFlags, out thirdFlags);
        	return TranslateFlags(firstFlags, secondFlags, thirdFlags);
        }

		private static string TranslateFlags(NotMeasuredReasons firstFlags, NotMeasuredReasons secondFlags, NotMeasuredReasons thirdFlags)
		{
			StringBuilder output = new StringBuilder();

			output.AppendLine(GetNotMeasuredReasonsDisplayValue(firstFlags));

			if (secondFlags != 0)
				output.AppendLine(GetNotMeasuredReasonsDisplayValue(secondFlags));

			if (thirdFlags != 0)
				output.AppendLine(GetNotMeasuredReasonsDisplayValue(thirdFlags));

			return output.ToString();			
		}

    	private static uint FLAG_OFFSCREEN = 0x00000100;
		public uint GetLCMeasurementFlags()
		{
			uint flags = 0;

			// NOTE: Get the byte flag as we know it is working
			byte byteFlag = GetLCMeasurementByteFlags();

			flags = byteFlag;

			// NOTE: Now add the extra bits that are not saved in the byte flag
			if ((NotMeasuredReasons & NotMeasuredReasons.ObjectExpectedPositionIsOffScreen) != 0)
			{
				flags = flags | FLAG_OFFSCREEN;
			}

			return flags;
		}

		public static string GetDWORDFlagsExplained(uint flags)
		{
			NotMeasuredReasons firstFlags, secondFlags, thirdFlags;
			DecodeByteFlags((byte)(flags & 0xFF), out firstFlags, out secondFlags, out thirdFlags);

			if ((flags & FLAG_OFFSCREEN) != 0)
				firstFlags = NotMeasuredReasons.ObjectExpectedPositionIsOffScreen;

			return TranslateFlags(firstFlags, secondFlags, thirdFlags);
		}

        private static string GetNotMeasuredReasonsDisplayValue(NotMeasuredReasons reason)
        {
            switch(reason)
            {
                case NotMeasuredReasons.UnknownReason:
            		return null; //"Tracking has failed for unknown reason";

                case NotMeasuredReasons.TrackedSuccessfully:
            		return null;

                case NotMeasuredReasons.FixedObject:
					return "W:Tracking:Object with fixed manually positioned aperture";

                case NotMeasuredReasons.GuidingStarBrightnessFluctoationTooHigh:
                    return "W:Tracking:Tracking was unsuccessful because the brightness fluctoation was out of the expected range";

                case NotMeasuredReasons.PSFFittingFailed:
                    return "W:Tracking:Tracking was unsuccessful because the a PSF could not be fitted";

                case NotMeasuredReasons.FoundObjectNotWithInExpectedPositionTolerance:
                    return "W:Tracking:Tracking was unsuccessful because the center of the PSF fit was too far from the expected location";

                case NotMeasuredReasons.FullyDisappearingStarMarkedTrackedWithoutBeingFound:
                    return "I:Tracking:A fully disappearing object was marked tracked without being detected (assuming it has disappeared)";

                case NotMeasuredReasons.FitSuspectAsNoGuidingStarsAreLocated:
                    return "W:Tracking:No guiding stars have been reliably located, all objects are marked as suspect";

                case NotMeasuredReasons.ObjectExpectedPositionIsOffScreen:
                    return "W:Tracking:The expected object position is outside the FOV";

                case NotMeasuredReasons.TrackedSuccessfullyAfterDistanceCheck:
                    return "I:Recovering:The object was recovered after a distance alignment";

                case NotMeasuredReasons.TrackedSuccessfullyAfterWiderAreaSearch:
                    return "I:Recovering:The object was recovered after searching in a wider area";

                case NotMeasuredReasons.TrackedSuccessfullyAfterStarRecognition:
                    return "I:Recovering:The object was recovered after using pattern recognition";

                case NotMeasuredReasons.FailedToLocateAfterDistanceCheck:
                    return "W:Recovering:Failed to recover the object after a distance alignment";

                case NotMeasuredReasons.FailedToLocateAfterWiderAreaSearch:
                    return "W:Recovering:Failed to recover the object after searching in a wider area";

                case NotMeasuredReasons.FailedToLocateAfterStarRecognition:
                    return "W:Recovering:Failed to recover the object after using pattern recognition";

                case NotMeasuredReasons.NoPixelsToMeasure:
                    return "W:Measuring:Aperture contains no pixels";

                case NotMeasuredReasons.MeasurementPSFFittingFailed:
                    return "W:Measuring:The PSF fitting failed";

                case NotMeasuredReasons.DistanceToleranceTooHighForNonFullDisappearingOccultedStar:
                    return "W:Measuring:The object apeared too far from the expected position";

                case NotMeasuredReasons.FWHMOutOfRange:
                    return "W:Measuring:The FWHM of the PSF fit was out of the expected range";
            }

            return null;
        }

        //public bool IsOffScreen = false;
        private byte targetNo;

        public int LastKnownGoodFrameId;
        public ImagePixel LastKnownGoodPosition;

        // Used for stripes display only
        private double m_appMeaAveragePixel;
        [XmlIgnore]
        public double AppMeaAveragePixel
        {
            get { return m_appMeaAveragePixel; }
            set { m_appMeaAveragePixel = value; }
        }

        private double m_ApertureArea;
        public double ApertureArea
        {
            get { return m_ApertureArea; }
            set { m_ApertureArea = value; }
        }

        #region Refined Position and Flickering
        public bool HasRefinedPositions
        {
            get { return RefiningPositions.Count > 0; }
        }

        public ImagePixel LastRefinedPosition
        {
            get { return RefiningPositions[RefiningPositions.Count - 1]; }
        }

        public void RegisterRefinedPosition(ImagePixel position, float signalLevel, double fwhm)
        {
            RefiningPositions.Add(position);

            if (!float.IsNaN(signalLevel)) RefiningSignalLevels.Add(signalLevel);
            if (!double.IsNaN(fwhm)) RefiningFWHMs.Add(fwhm);
        }

        private float RefinedSignalLevel = float.NaN;
        public float RefinedOrLastSignalLevel
        {
            get
            {
                if (float.IsNaN(RefinedSignalLevel))
                    return LastSignalLevel;
                else
                    return RefinedSignalLevel;
            }
        }

        public void GetPositionShift(out double deltaX, out double deltaY)
        {
            int refinedPositions = RefiningPositions.Count;
            ImagePixel lastCenter = RefiningPositions[refinedPositions - 1];
            ImagePixel prevCenter;
            if (refinedPositions > 1)
                prevCenter = RefiningPositions[refinedPositions - 2];
            else
                prevCenter = OriginalObject.AsImagePixel;

            deltaX = lastCenter.XDouble - prevCenter.XDouble;
            deltaY = lastCenter.YDouble - prevCenter.YDouble;
        }

        public void ComputeRefinedFlickering(out float flickering, out float maxResidual)
        {
            if (RefiningSignalLevels.Count == 0)
            {
                flickering = float.NaN;
                maxResidual = float.NaN;
                return;
            }

            float averageSignal = 0;
            RefiningSignalLevels.Sort();

            if (RefiningSignalLevels.Count > 0)
            {
                if (RefiningSignalLevels.Count % 2 == 1)
                    averageSignal = RefiningSignalLevels[RefiningSignalLevels.Count / 2];
                else
                    averageSignal = 0.5f * (RefiningSignalLevels[(RefiningSignalLevels.Count / 2) - 1] + RefiningSignalLevels[RefiningSignalLevels.Count / 2]);
            }

            float residuals = 0;
            float maxResVal = -1;
            foreach (float signalLevel in RefiningSignalLevels)
            {
                float res = Math.Abs(averageSignal - signalLevel);
                if (maxResVal < res) maxResVal = res;
                residuals += res * res;
            }
            residuals = (float)(Math.Sqrt(residuals / (RefiningSignalLevels.Count - 1)));

            flickering = residuals / averageSignal;
            maxResidual = maxResVal / averageSignal;

            RefinedSignalLevel = averageSignal;
        }

        public double ComputeRefinedDistances(TrackedObject obj2, out double vectorX, out double vectorY)
        {
            double refinedDistance = double.NaN;

            if (this.OriginalObject.IsFixedAperture || 
                obj2.OriginalObject.IsFixedAperture)
            {
                // For manually placed apertures, return the initial distances

                vectorX = obj2.OriginalObject.ApertureStartingX - this.OriginalObject.ApertureStartingX;
                vectorY = obj2.OriginalObject.ApertureStartingY - this.OriginalObject.ApertureStartingY;

                return Math.Sqrt(vectorX * vectorX + vectorY * vectorY);
            }

            List<double> distances = new List<double>();

            double xVector = 0;
            double yVector = 0;

            for (int k = 0; k < RefiningPositions.Count; k++)
            {
                ImagePixel pos1 = RefiningPositions[k];
                ImagePixel pos2 = obj2.RefiningPositions[k];

                if (pos1.IsSpecified &&
                    pos2.IsSpecified)
                {
                    // Compute and average the distance
                    double dist = Math.Sqrt((pos1.XDouble - pos2.XDouble) * (pos1.XDouble - pos2.XDouble) + (pos1.YDouble - pos2.YDouble) * (pos1.YDouble - pos2.YDouble));
                    distances.Add(dist);

                    xVector += (pos2.XDouble - pos1.XDouble);
                    yVector += (pos2.YDouble - pos1.YDouble);
                }
            }

            // Get the median and add in the final dict
            distances.Sort();
            if (distances.Count % 2 == 1)
                refinedDistance = distances[distances.Count / 2];
            else if (distances.Count > 0)
                refinedDistance = 0.5 * (distances[(distances.Count / 2) - 1] + distances[distances.Count / 2]);

            vectorX = xVector / distances.Count;
            vectorY = yVector / distances.Count;
            return refinedDistance;
        }

        public double ComputeRefinedFWHM()
        {
            RefiningFWHMs.Sort();

            if (RefiningFWHMs.Count == 0)
                return double.NaN;

            if (RefiningFWHMs.Count % 2 == 1)
                return RefiningFWHMs[RefiningFWHMs.Count / 2];
            else
                return (RefiningFWHMs[RefiningFWHMs.Count / 2] + RefiningFWHMs[(RefiningFWHMs.Count / 2) - 1]) / 2;
        }
        #endregion

        public Dictionary<int, LocationVector> OtherGuidingStarsLocationVectors;

        public TrackedObject(byte targetNo, TrackedObjectConfig originalObject)
        {
            this.targetNo = targetNo;
            OriginalObject = originalObject;

            ThisFrameX = originalObject.ApertureStartingX;
            ThisFrameY = originalObject.ApertureStartingY;

			ApertureArea = Math.PI * originalObject.ApertureInPixels * originalObject.ApertureInPixels;
        }

        public bool IsGuidingStar
        {
            get { return OriginalObject.TrackingType == TrackingType.GuidingStar; }
        }

        public bool IsOcultedStar
        {
            get { return OriginalObject.TrackingType == TrackingType.OccultedStar; }
        }

        public ImagePixel Center
        {
            get
            {
                if (ThisFrameX + ThisFrameY == 0)
                    return OriginalObject.AsImagePixel;
                else
                    return new ImagePixel(ThisFrameX, ThisFrameY);
            }
        }

        public float Aperture
        {
            get
            {
                return OriginalObject.ApertureInPixels;
            }
        }

        public byte TargetNo
        {
            get { return targetNo; }
        }

        public int PsfFitMatrixSize
        {
            get
            {
                return OriginalObject.PsfFitMatrixSize;
            }
        }

        public void NewFrame()
        {
            LastFrameX = ThisFrameX;
            LastFrameY = ThisFrameY;
            LastSignalLevel = ThisSignalLevel;

            ThisFrameX = 0;
            ThisFrameY = 0;

            ThisSignalLevel = 1;
            ThisFrameFit = null;
        	m_IsLocated = false;
        	NotMeasuredReasons = NotMeasuredReasons.UnknownReason;
            //IsOffScreen = false;
        }
    }
}
