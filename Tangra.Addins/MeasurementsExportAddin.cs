using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Model.Helpers;
using Tangra.SDK;

namespace Tangra.Addins
{
	public enum ExportMagType
	{
		Clear,
		V,
		R,
		B,
		J,
		K
	}

	[Serializable]
	public class MeasurementsExportAddin : MarshalByRefObject, ITangraAddinAction
	{
		internal class MeasurementInfo
		{
			public float Intensity;
			public float PsfAmplitude;
			public bool ExcludedForHighResiduals;
			public bool IsSaturated;
		    public PhotometryReductionMethod MeaSignalMethod;
		    public BackgroundMethod MeaBackgroundMethod;
		    public float? MeaSingleApertureSize;
		    public int MeaBackgroundPixelCount;
		    public uint MeaSaturationLevel;
		}

		private ITangraHost m_Host;

		Dictionary<ulong, List<MeasurementInfo>> m_MeasurementsPerStar = new Dictionary<ulong, List<MeasurementInfo>>();
		private List<ITangraCatalogStar> m_CatalogStars = new List<ITangraCatalogStar>();

		public void Initialise(ITangraHost host)
		{
			m_Host = host;
		}

		public void Finalise()
		{ }

		public string DisplayName
		{
			get { return "Star Photometry CSV Export"; }
		}

		public AddinActionType ActionType
		{
			get { return AddinActionType.Astrometry; }
		}

		public IntPtr Icon
		{
			get { return IntPtr.Zero; }
		}

		public int IconTransparentColorARGB
		{
			get { return Color.Transparent.ToArgb(); }
		}

		public void Execute()
		{
			ITangraAstrometricSolution solution = m_Host.GetAstrometryProvider().GetCurrentFrameAstrometricSolution();
			List<ITangraMatchedStar> matchedStars = solution.GetAllMatchedStars();
			foreach (ITangraMatchedStar star in matchedStars)
			{
				if (star.CatalogStar != null)
				{
					List<MeasurementInfo> currentMeasurements;
					if (!m_MeasurementsPerStar.TryGetValue(star.CatalogStar.StarNo, out currentMeasurements))
					{
						currentMeasurements = new List<MeasurementInfo>();
						m_MeasurementsPerStar.Add(star.CatalogStar.StarNo, currentMeasurements);
						m_CatalogStars.Add(star.CatalogStar);
					}

                    var mea = new MeasurementInfo()
					{
						Intensity = star.Intensity,
						PsfAmplitude = star.PSFAmplitude,
						ExcludedForHighResiduals = star.ExcludedForHighResidual,
						IsSaturated = star.IsSaturated
                    };

				    var meaInfo = star as ITangraStarMeasurementInfo;
				    if (meaInfo != null)
				    {
				        mea.MeaSignalMethod = meaInfo.MeaSignalMethod;
                        mea.MeaBackgroundMethod = meaInfo.MeaBackgroundMethod;
                        mea.MeaSingleApertureSize = meaInfo.MeaSingleApertureSize;
                        mea.MeaBackgroundPixelCount = meaInfo.MeaBackgroundPixelCount;
                        mea.MeaSaturationLevel = meaInfo.MeaSaturationLevel;
				    }
				    else
				    {
                        mea.MeaSignalMethod = PhotometryReductionMethod.Unknown;
                        mea.MeaBackgroundMethod = BackgroundMethod.Unknown;
                        mea.MeaSingleApertureSize = null;
                        mea.MeaBackgroundPixelCount = 0;
                        mea.MeaSaturationLevel = 0;
                    }

				    currentMeasurements.Add(mea);
				}
			}
		}

		internal void OnBeginMultiFrameAstrometry()
		{
			m_MeasurementsPerStar.Clear();
			m_CatalogStars.Clear();
		}

	    private float? AbsNullFloat(float? f1, float? f2)
	    {
	        float? diff = f1 - f2;
	        if (diff.HasValue)
	            return Math.Abs(diff.Value);
	        else
	            return null;
	    }

		internal void OnEndMultiFrameAstrometry()
		{
			ExportMagType type = Properties.Settings.Default.ExportMag;

			if (m_MeasurementsPerStar.Count > 0)
			{
				var output = new StringBuilder();
                output.Append(string.Format("Star No, RA Deg (J2000), DE Deg (J2000), Catalog Mag ({0}), B-V, Sloan r', Average Intencity, StdDev, Median Intencity, StdDev, Average PSF Amplitude, StdDev, Median PSF Amplitude, StdDev, All Frames, Saturated Frames, Excluded Frames, Aperture, SignalMethod, BackgroundMethod, BgPixelCount, SaturationLevel\r\n", type.ToString()));

				foreach (ulong starNo in m_MeasurementsPerStar.Keys)
				{
					ITangraCatalogStar catStar = m_CatalogStars.SingleOrDefault(x => x.StarNo == starNo);

					List<MeasurementInfo> mea = m_MeasurementsPerStar[starNo];
					List<float> intensities = mea.Select(x => x.Intensity).Where(i => i > 0).ToList();
					float average = intensities.Count > 0 ? intensities.Average() : float.NaN;
					float averageError = intensities.Count > 1
						? (float)Math.Sqrt(intensities.Select(x => Math.Pow(x - average, 2)).Sum() / (intensities.Count - 1))
						: float.NaN;

					float median = intensities.Count > 0 ? intensities.SortAndGetMedian() : float.NaN;
					float medianError = intensities.Count > 1
						? (float)Math.Sqrt(intensities.Select(x => Math.Pow(x - median, 2)).Sum() / (intensities.Count - 1))
						: float.NaN;

					List<float> amplitudes = mea.Select(x => x.PsfAmplitude).Where(i => i > 0).ToList();
					float averageAmp = amplitudes.Count > 0 ? amplitudes.Average() : float.NaN;
					float averageAmpError = amplitudes.Count > 1
						? (float)Math.Sqrt(amplitudes.Select(x => Math.Pow(x - averageAmp, 2)).Sum() / (amplitudes.Count - 1))
						: float.NaN;

					float medianAmp = amplitudes.Count > 0 ? amplitudes.SortAndGetMedian() : float.NaN;
					float medianAmpError = amplitudes.Count > 1
						? (float)Math.Sqrt(amplitudes.Select(x => Math.Pow(x - medianAmp, 2)).Sum() / (amplitudes.Count - 1))
						: float.NaN;

					int allMea = mea.Count;
					int allSaturated = mea.Count(x => x.IsSaturated);
					int allHighRes = mea.Count(x => x.ExcludedForHighResiduals || Math.Abs(x.Intensity) < 0.001 || Math.Abs(x.PsfAmplitude-1) < 0.001);

					float exportMag;
				    var apassData = catStar as ITangraAPASSStarMagnitudes;
                    if (type == ExportMagType.V) exportMag = apassData != null ? apassData.V : catStar.MagV;
                    else if (type == ExportMagType.R) exportMag = apassData != null ? apassData.r : catStar.MagR;
                    else if (type == ExportMagType.B) exportMag = apassData != null ? apassData.B : catStar.MagB;
					else if (type == ExportMagType.J) exportMag = catStar.MagJ;
					else if (type == ExportMagType.K) exportMag = catStar.MagK;
					else
						exportMag = catStar.Mag;

				    string bvColour = null;
                    string sloanR = null;
				    if (apassData != null)
				    {
				        if (!float.IsNaN(apassData.r)) sloanR = apassData.r.ToString("0.000");
                        if (!float.IsNaN(apassData.B) && !float.IsNaN(apassData.V)) bvColour = (apassData.B - apassData.V).ToString("0.000");
				    }

				    string meaAperture = null;
				    string meaSignalMethod = null;
                    string meaBackgroundMethod = null;
                    string meaBgPixelCount = null;
                    string meaSaturationLevel = null;
                    if (mea.Count > 2)
                    {
                        var allApertures = mea.Select(x => x.MeaSingleApertureSize).ToList();
                        var medianAperture = allApertures.SortAndGetMedian();
                        int sameCnt = allApertures.Count(x => AbsNullFloat(x, medianAperture) < 0.01);
                        if (sameCnt + 2 < allApertures.Count)
                            meaAperture = string.Format("{0} ({1} of {2})", Convert.ToString(medianAperture), sameCnt, allApertures.Count);
                        else
                            meaAperture = Convert.ToString(medianAperture);

                        var allSig = mea.Select(x => x.MeaSignalMethod).ToList();
                        var medianSig = allSig.SortAndGetMedian();
                        sameCnt = allSig.Count(x => x == medianSig);
                        if (sameCnt + 2 < allSig.Count)
                            meaSignalMethod = string.Format("{0} ({1} of {2})", Convert.ToString(medianSig), sameCnt, allSig.Count);
                        else
                            meaSignalMethod = Convert.ToString(medianSig);

                        var allBg = mea.Select(x => x.MeaBackgroundMethod).ToList();
                        var medianBg = allBg.SortAndGetMedian();
                        sameCnt = allBg.Count(x => x == medianBg);
                        if (sameCnt + 2 < allBg.Count)
                            meaBackgroundMethod = string.Format("{0} ({1} of {2})", Convert.ToString(medianBg), sameCnt, allBg.Count);
                        else
                            meaBackgroundMethod = Convert.ToString(medianBg);

                        var allBgPix = mea.Select(x => x.MeaBackgroundPixelCount).ToList();
                        var bgPix = allBgPix.SortAndGetMedian();
                        sameCnt = allBgPix.Count(x => x == bgPix);
                        if (sameCnt + 2 < allBgPix.Count)
                            meaBgPixelCount = string.Format("{0} ({1} of {2})", Convert.ToString(bgPix), sameCnt, allBgPix.Count);
                        else
                            meaBgPixelCount = Convert.ToString(bgPix);

                        var allSat = mea.Select(x => x.MeaSaturationLevel).ToList();
                        var sat = allSat.SortAndGetMedian();
                        sameCnt = allSat.Count(x => x == sat);
                        if (sameCnt + 2 < allSat.Count)
                            meaSaturationLevel = string.Format("{0} ({1} of {2})", Convert.ToString(sat), sameCnt, allSat.Count);
                        else
                            meaSaturationLevel = Convert.ToString(sat);
                    }

                    output.AppendFormat("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}, {15}, {16}, {17}, {18}, {19}, {20}, {21}\r\n", starNo, catStar.RAJ2000Deg, catStar.DEJ2000Deg, exportMag, bvColour, sloanR,
							average, averageError, median, medianError, averageAmp, averageAmpError, medianAmp, medianAmpError, allMea, allSaturated, allHighRes,
                            meaAperture, meaSignalMethod, meaBackgroundMethod, meaBgPixelCount, meaSaturationLevel);
				}

				var dialog = new SaveFileDialog();
				dialog.Filter = "Comma Separated Values (*.csv)|*.csv|All Files (*.*)|*.*";
				dialog.DefaultExt = "csv";
				dialog.Title = "Export Tangra Measurements";

				if (dialog.ShowDialog(m_Host.ParentWindow) == DialogResult.OK)
				{
					File.WriteAllText(dialog.FileName, output.ToString());
					Process.Start(dialog.FileName);
				}
			}
		}
	}
}
