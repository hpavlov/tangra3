/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tangra.Model.Config;
using Tangra.Model.VideoOperations;
using Tangra.VideoOperations.LightCurves;
using Tangra.VideoOperations.LightCurves.InfoForms;

namespace Tangra.Controller
{
	internal class LightCurveContext
	{
		internal List<List<LCMeasurement>> AllReadings = new List<List<LCMeasurement>>(new List<LCMeasurement>[] { new List<LCMeasurement>(), new List<LCMeasurement>(), new List<LCMeasurement>(), new List<LCMeasurement>() });

		internal MagnitudeConverter MagnitudeConverter;

		internal LightCurveContext(LCFile lcFile)
		{
			if (lcFile.Header.MeasuredFrames > 0)
			{
				for (int i = 0; i < lcFile.Header.ObjectCount; i++)
				{
					m_ReProcessApertures[i] = lcFile.Header.MeasurementApertures[i];
					m_ReProcessFitAreas[i] = 2 * (lcFile.Header.PsfFitMatrixSizes[i] / 2) + 1;
					AllReadings[i] = lcFile.Data[i];
				}
			}

			MagnitudeConverter = new MagnitudeConverter(lcFile.Header.ReferenceMagnitudes, lcFile.Header.ReferenceIntensity);

			for (int i = 0; i < 4; i++)
				m_ObjectTitles[i] = string.Format("Object {0}", i);

			NormMethod = NormalisationMethod.Average4Frame;
		}		

		internal enum FilterType
		{
			NoFilter,
			LowPass,
			LowPassDifference
		}

		internal enum NormalisationMethod
		{
			Average4Frame,
			Average8Frame,
			Average16Frame,
			LinearFit,
			FrameByFrame
		}

		internal enum BackgroundComputationMethod
		{
			FromPSFFit,
			FromBackgroundDistribution
		}

		internal enum XAxisMode
		{
			FrameNo,
			Time
		}

        internal enum YAxisMode
        {
            Flux,
            Magnitudes
        }

        internal enum LightCurveMode
        {
            Line,
            Scatter
        }

		private bool m_Dirty = false;
		private bool m_FirstZoomedFrameChanged = false;
		private bool m_RequiresFullReprocessing = false;
		private ProcessingType m_ProcessingType = ProcessingType.SignalMinusBackground;
		private int m_Binning = 0;
	    private int m_BinningFirstFrame = 0;
		private int m_Normalisation = -1;
		private FilterType m_Filter = FilterType.NoFilter;
		private NormalisationMethod m_NormMethod = NormalisationMethod.Average4Frame;
		private float[] m_ReProcessApertures = new float[4];
		private int[] m_ReProcessFitAreas = new int[4];

		public string InstrumentalDelayConfigName;
	    public bool AcquisitionDelayApplied;
		public MeasurementTimingType TimingType;
		public string CameraName;
		public int AAVFrameIntegration;
	    public bool IsAstroAnalogueVideo;
	    public bool IsDigitalVideo;
		public bool InstrumentalDelayCorrectionsNotRequired;
		public uint MinFrame;
		public uint MaxFrame;
		public int ObjectCount;

		public uint m_SelectedFrameNo = 0;

		private XAxisMode m_XAxisMode;
        private YAxisMode m_YAxisMode;
        private LightCurveMode m_LightCurveMode;
		private string m_ChartTitle;
        

		public uint SelectedFrameNo
		{
			get { return m_SelectedFrameNo; }
			set
			{
				if (m_SelectedFrameNo != value)
				{
					m_SelectedFrameNo = value;
				}
			}
		}

		public int BitPix;
		public uint MaxPixelValue;
		public IDisplayBitmapConverter DisplayBitmapConverter;

		public bool CustomBinning { get; set; }

		private bool m_OutlierRemoval = false;
		public string[] m_ObjectTitles = new string[4];
		public string[] m_ChartTitleLines;

		#region Backed-up settings so they can be restored when CancelChanges() is requested

		private bool m_bu_Dirty = false;
		private bool m_bu_RequiresFullReprocessing = false;
		private ProcessingType m_bu_ProcessingType = ProcessingType.SignalMinusBackground;
		private int m_bu_Binning = 0;
		private int m_bu_Normalisation = -1;
		private FilterType m_bu_Filter = FilterType.NoFilter;
		private NormalisationMethod mbu__NormMethod = NormalisationMethod.Average4Frame;
		private float[] m_bu_ReProcessApertures = new float[4];
		private int[] m_bu_ReProcessFitAreas = new int[4];
		public uint bu_SelectedFrameNo = 0;
		public string[] m_bu_ObjectTitles = new string[4];
		public string[] m_bu_ChartTitleLines;

		internal void PrepareForCancelling()
		{
			m_bu_Dirty = m_Dirty;
			m_bu_RequiresFullReprocessing = m_RequiresFullReprocessing;
			m_bu_ProcessingType = m_ProcessingType;
			m_bu_Binning = m_Binning;
			m_bu_Normalisation = m_Normalisation;
			m_bu_Filter = m_Filter;
			mbu__NormMethod = m_NormMethod;
			bu_SelectedFrameNo = SelectedFrameNo;

			for (int i = 0; i < 4; i++)
				m_bu_ReProcessApertures[i] = m_ReProcessApertures[i];

			for (int i = 0; i < 4; i++)
				m_bu_ReProcessFitAreas[i] = m_ReProcessFitAreas[i];

			for (int i = 0; i < 4; i++)
				m_bu_ObjectTitles[i] = m_ObjectTitles[i];

			m_bu_ChartTitleLines = m_ChartTitleLines;
		}

		internal void CancelChanges()
		{
			m_Dirty = m_bu_Dirty;
			m_RequiresFullReprocessing = m_bu_RequiresFullReprocessing;
			m_ProcessingType = m_bu_ProcessingType;
			m_Binning = m_bu_Binning;
			m_Normalisation = m_bu_Normalisation;
			m_Filter = m_bu_Filter;
			m_NormMethod = mbu__NormMethod;
			SelectedFrameNo = bu_SelectedFrameNo;

			for (int i = 0; i < 4; i++)
				m_ReProcessApertures[i] = m_bu_ReProcessApertures[i];

			for (int i = 0; i < 4; i++)
				m_ReProcessFitAreas[i] = m_bu_ReProcessFitAreas[i];

			for (int i = 0; i < 4; i++)
				m_ObjectTitles[i] = m_bu_ObjectTitles[i];

			m_ChartTitleLines = m_bu_ChartTitleLines;
		}

		#endregion

		public ProcessingType ProcessingType
		{
			get { return m_ProcessingType; }
			set
			{
				if (m_ProcessingType != value)
				{
					m_ProcessingType = value;
					m_Dirty = true;
				}
			}
		}

		public XAxisMode XAxisLabels
		{
			get { return m_XAxisMode; }
			set
			{
				if (m_XAxisMode != value)
				{
					m_XAxisMode = value;
					m_Dirty = true;
				}
			}
		}

        public YAxisMode YAxisLabels
        {
            get { return m_YAxisMode; }
            set
            {
                if (m_YAxisMode != value)
                {
                    m_YAxisMode = value;
                    m_Dirty = true;
                }
            }
        }

        public LightCurveMode ChartType
        {
            get { return m_LightCurveMode; }
            set
            {
                if (m_LightCurveMode != value)
                {
                    m_LightCurveMode = value;
                    m_Dirty = true;
                }
            }
        }

		public string ChartTitle
		{
			get { return m_ChartTitle; }
			set
			{
				if (m_ChartTitle != value)
				{
					m_ChartTitle = value;
					m_Dirty = true;
				}
			}
		}

		public bool OutlierRemoval
		{
			get { return m_OutlierRemoval; }
			set
			{
				if (m_OutlierRemoval != value)
				{
					m_OutlierRemoval = value;
					m_Dirty = true;
				}
			}
		}

	    public int BinningFirstFrame
	    {
            get { return m_BinningFirstFrame; }
			set
			{
                if (m_BinningFirstFrame != value)
				{
                    m_BinningFirstFrame = value;
					m_Dirty = true;
				}
			}
	    }

		public int Binning
		{
			get { return m_Binning; }
			set
			{
				if (m_Binning != value)
				{
					m_Binning = value;
					m_Dirty = true;
				}
			}
		}

		public int Normalisation
		{
			get { return m_Normalisation; }
			set
			{
				if (m_Normalisation != value)
				{
					m_Normalisation = value;
					m_Dirty = true;
				}
			}
		}

		public FilterType Filter
		{
			get { return m_Filter; }
			set
			{
				if (m_Filter != value)
				{
					m_Filter = value;
					m_Dirty = true;
					m_RequiresFullReprocessing = true;
				}
			}
		}

		public NormalisationMethod NormMethod
		{
			get { return m_NormMethod; }
			set
			{
				if (m_NormMethod != value)
				{
					m_NormMethod = value;
					// If the normalization is specified, changing the method will make the data durty
					m_Dirty = m_Normalisation > -1;
				}
			}
		}

		private TangraConfig.BackgroundMethod m_BackgroundMethod = TangraConfig.BackgroundMethod.AverageBackground;

		public TangraConfig.BackgroundMethod BackgroundMethod
		{
			get { return m_BackgroundMethod; }
			set
			{
				if (m_BackgroundMethod != value)
				{
					m_BackgroundMethod = value;
					m_Dirty = true;

					m_RequiresFullReprocessing = true;
				}
			}
		}

		private TangraConfig.PhotometryReductionMethod m_SignalMethod = TangraConfig.PhotometryReductionMethod.AperturePhotometry;

		public TangraConfig.PhotometryReductionMethod SignalMethod
		{
			get { return m_SignalMethod; }
			set
			{
				if (m_SignalMethod != value)
				{
					m_SignalMethod = value;
					m_Dirty = true;

					m_RequiresFullReprocessing = true;
				}
			}
		}

		private TangraConfig.PsfQuadrature m_PsfQuadratureMethod = TangraConfig.PsfQuadrature.NumericalInAperture;

		public TangraConfig.PsfQuadrature PsfQuadratureMethod
		{
			get { return m_PsfQuadratureMethod; }
			set
			{
				if (m_PsfQuadratureMethod != value)
				{
					m_PsfQuadratureMethod = value;
					m_Dirty = true;

					m_RequiresFullReprocessing = true;
				}
			}
		}

		private byte[] m_DecodingGammaMatrix = new byte[256];
		public byte[] DecodingGammaMatrix
		{
			get { return m_DecodingGammaMatrix; }
		}

		private double m_EncodingGamma = 1.0;
		public double EncodingGamma
		{
			get { return m_EncodingGamma; }
			set
			{
				if (m_EncodingGamma != value)
				{
					m_EncodingGamma = value;

					double decodingGamma = 1 / m_EncodingGamma;

					for (int i = 0; i < 256; i++)
						m_DecodingGammaMatrix[i] = (byte)Math.Max(0, Math.Min(255, Math.Round(256 * Math.Pow(i / 256.0, decodingGamma))));

					m_Dirty = true;

					m_RequiresFullReprocessing = true;
				}
			}
		}

		private TangraConfig.KnownCameraResponse m_ReverseCameraResponse;
		public TangraConfig.KnownCameraResponse ReverseCameraResponse
		{
			get { return m_ReverseCameraResponse; }
			set
			{
				if (m_ReverseCameraResponse != value)
				{
					m_ReverseCameraResponse = value;

					// TODO: Build a decoding matrix, in conjuction with Gamma??
					m_Dirty = true;

					m_RequiresFullReprocessing = true;

					//throw new NotImplementedException();

				}
			}
		}

		public bool UseClipping;
		public bool UseStretching;
		public bool UseBrightnessContrast;
		public byte FromByte;
		public byte ToByte;
		public int Brightness;
		public int Contrast;

		private CompositeFramePreProcessor m_FramePreProcessor = null;

		internal void InitFrameBytePreProcessors()
		{
			m_FramePreProcessor = new CompositeFramePreProcessor();

			if (UseBrightnessContrast)
			{
				IFramePreProcessor bytePreProcessor = new FrameByteBrightnessContrast(Brightness, Contrast, true, BitPix);
				m_FramePreProcessor.AddPreProcessor(bytePreProcessor);
			}
			else if (UseStretching)
			{
				IFramePreProcessor bytePreProcessor = new FrameByteStretcher(FromByte, ToByte, true, BitPix);
				m_FramePreProcessor.AddPreProcessor(bytePreProcessor);
			}
			else if (UseClipping)
			{
				IFramePreProcessor bytePreProcessor = new FrameByteClipper(FromByte, ToByte, true, BitPix);
				m_FramePreProcessor.AddPreProcessor(bytePreProcessor);
			}
		}

		private TangraConfig.PsfFittingMethod m_PsfFittingMethod = TangraConfig.PsfFittingMethod.DirectNonLinearFit;

		public TangraConfig.PsfFittingMethod PsfFittingMethod
		{
			get { return m_PsfFittingMethod; }
			set
			{
				if (m_PsfFittingMethod != value)
				{
					m_PsfFittingMethod = value;
					m_Dirty = true;

					m_RequiresFullReprocessing = true;
				}
			}
		}

		private float m_ManualAverageFWHM = float.NaN;

		public float ManualAverageFWHM
		{
			get { return m_ManualAverageFWHM; }
			set
			{
				if (m_ManualAverageFWHM != value)
				{
					m_ManualAverageFWHM = value;
					m_Dirty = true;

					m_RequiresFullReprocessing = true;
				}
			}
		}

		public float[] ReProcessApertures
		{
			get { return m_ReProcessApertures; }
		}

		public int[] ReProcessFitAreas
		{
			get { return m_ReProcessFitAreas; }
		}


		public string[] ObjectTitles
		{
			get { return m_ObjectTitles; }
		}

		public string[] ChartTitleLines
		{
			get { return m_ChartTitleLines; }
			set { m_ChartTitleLines = value; }
		}

		public bool Dirty
		{
			get { return m_Dirty; }
		}

		public bool RequiresFullReprocessing
		{
			get { return m_RequiresFullReprocessing; }
		}

		public bool FirstZoomedFrameChanged
		{
			get { return m_FirstZoomedFrameChanged; }
		}

		public void MarkClean()
		{
			m_Dirty = false;
			m_RequiresFullReprocessing = false;
			m_FirstZoomedFrameChanged = false;
		}

		public void MarkDirtyNoFullReprocessing()
		{
			m_Dirty = true;
		}

		public void MarkFirstZoomedFrameChanged()
		{
			m_FirstZoomedFrameChanged = true;
		}

		public void MarkDirtyWithFullReprocessing()
		{
			m_Dirty = true;
			m_RequiresFullReprocessing = true;
		}
	}
}
