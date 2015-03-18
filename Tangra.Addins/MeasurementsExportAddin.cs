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
			get { return "Measurements CSV Export"; }
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
					currentMeasurements.Add(new MeasurementInfo()
					{
						Intensity = star.Intensity,
						PsfAmplitude = star.PSFAmplitude,
						ExcludedForHighResiduals = star.ExcludedForHighResidual,
						IsSaturated = star.IsSaturated
					});
				}
			}
		}

		internal void OnBeginMultiFrameAstrometry()
		{
			m_MeasurementsPerStar.Clear();
			m_CatalogStars.Clear();
		}

		internal void OnEndMultiFrameAstrometry()
		{
			ExportMagType type = Properties.Settings.Default.ExportMag;

			if (m_MeasurementsPerStar.Count > 0)
			{
				var output = new StringBuilder();
				output.Append(string.Format("Star No, RA Deg (J2000), DE Deg (J2000), Catalog Mag ({0}), Average Intencity, Error, Median Intencity, Error, Average PSF Amplitude, Error, Median PSF Amplitude, Error, All Frames, Saturated Frames, Excluded Frames\r\n", type.ToString()));

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

					if (type == ExportMagType.V) exportMag = catStar.MagV;
					else if (type == ExportMagType.R) exportMag = catStar.MagR;
					else if (type == ExportMagType.B) exportMag = catStar.MagB;
					else if (type == ExportMagType.J) exportMag = catStar.MagJ;
					else if (type == ExportMagType.K) exportMag = catStar.MagK;
					else
						exportMag = catStar.Mag;
		
					output.AppendFormat("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}\r\n", starNo, catStar.RAJ2000Deg, catStar.DEJ2000Deg, exportMag,
							average, averageError, median, medianError, averageAmp, averageAmpError, medianAmp, medianAmpError, allMea, allSaturated, allHighRes);
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
