using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Tangra.SDK;

namespace Tangra.KweeVanWoerden
{
	static class NativeMethods
	{
		[DllImport("kernel32.dll")]
		public static extern IntPtr LoadLibrary(string dllToLoad);

		[DllImport("kernel32.dll")]
		public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

		[DllImport("kernel32.dll")]
		public static extern bool FreeLibrary(IntPtr hModule);
	}

	[Serializable]
	public class KweeVanWoerdenMinimum : MarshalByRefObject, ITangraAddinAction
	{
		// Fortran Interoperability Links:
		// http://fortranwiki.org/fortran/show/c%23+interoperability
		// http://stackoverflow.com/questions/10317691/making-fortran-dll-and-calling-it-from-c-sharp
		// https://www.uni-muenster.de/imperia/md/content/physik_ct/pdf/05_intel_optimierung.pdf

		// Subroutine Kwee_van_Woerden ( Number_Obs, Time_First, Time, Variable_Star_DN, Variable_Sky_DN, Comparison_Star_DN, Comparison_Sky_DN, Directory_Name )
		// ! Input values
		// integer                    Number_Obs                              ! Number of observed data points
		// double precision           Time_First                              ! Absolute time for the first observation
		// double precision           Time(10000)                             ! Mid-times of each observation
		// double precision           Variable_Star_DN(10000)                 ! Pixels values for the variable star brightness (including sky background)
		// double precision           Variable_Sky_DN(10000)                  ! Pixels values for the variable star sky background to be subtracted
		// double precision           Comparison_Star_DN(10000)               ! Pixels values for the comparison star brightness (including sky background)
		// double precision           Comparison_Sky_DN(10000)                ! Pixels values for the comparison star sky background to be subtracted
		// character*100              Directory_Name                          ! Location for the output files
		public delegate void KweeVanWoerdenDelegate(
			ref int number_obs,
			ref double first_time,
			[In, Out] double[] time,
			[In, Out] double[] var_star,
			[In, Out] double[] var_sky,
			[In, Out] double[] comp_star,
			[In, Out] double[] comp_sky,
			[In, Out] char[] directory_name);

		private string FORTRAN_DLL_DIRECTORY_PATH = AppDomain.CurrentDomain.BaseDirectory + @"Addins";

		private ITangraHost m_TangraHost;
		private KweeVanWoerdenAddinSettings m_Settings;
		private bool m_Running;

		internal KweeVanWoerdenMinimum(ITangraHost tangraHost, KweeVanWoerdenAddinSettings settings)
		{
			m_TangraHost = tangraHost;
			m_Settings = settings;
			m_Running = false;
		}

		public string DisplayName
		{
			get { return "Extract Eclipsing Binary Minimum Time (Kwee-Van Woerden Method)"; }
		}

		public AddinActionType ActionType
		{
			get { return AddinActionType.LightCurveEventTimeExtractor; }
		}

		public IntPtr Icon
		{
			get { return Properties.Resource.EclipsingBinaries.ToBitmap().GetHbitmap(); }
		}

		public int IconTransparentColorARGB
		{
			get { return System.Drawing.Color.Transparent.ToArgb(); }
		}

		//----------------------------------------------------------------------------------------
		// UTC DateTime to UTC Julian date
		//----------------------------------------------------------------------------------------
		private double DateUtcToJulian(DateTime dt)
		{
			double tNow = (double)dt.Ticks - 6.30822816E+17;	// .NET ticks at 01-Jan-2000T00:00:00
			double j = 2451544.5 + (tNow / 8.64E+11);		// Tick difference to days difference
			return j;
		}

		//----------------------------------------------------------------------------------------
		// UTC Julian date to UTC DateTime
		//----------------------------------------------------------------------------------------
		private DateTime JulianToDateUtc(double j)
		{
			long tix = (long)(6.30822816E+17 + (8.64E+11 * (j - 2451544.5)));
			DateTime dt = new DateTime(tix);
			return dt;
		}

		public void Execute()
		{
			if (m_Running)
			{
				ShowErrorMessage("Extract Eclipsing Binary Minimum Time is already running.");
				return;
			}

			m_Running = true;
			try
			{
				if (m_Settings.UseSimulatedDataSet)
				{
					MessageBox.Show(
						m_TangraHost.ParentWindow,
					    "A simulated dataset will be used rather than real data.\r\n\r\nTo use actual light curve data from Tangra please reconfigure the add-in from the Settings.",
					    "Eclipsing Binaries for Tangra", 
						MessageBoxButtons.OK, 
						MessageBoxIcon.Warning);

					ExecuteWithSimulatedData();
				}
				else
					ExecuteWithRealData();
			}
			finally
			{
				m_Running = false;
			}
		}

		private void ExecuteWithRealData()
		{
			ILightCurveDataProvider dataProvider = m_TangraHost.GetLightCurveDataProvider();

			if (dataProvider != null)
			{
				if (dataProvider.NumberOfMeasuredComparisonObjects == 0)
				{
					ShowErrorMessage("At least one comparison object is required to determine the target eclipsing binary minimum.");
					return;
				}

				ISingleMeasurement[] measurements = dataProvider.GetTargetMeasurements();

				bool hasReliableTimeBase = dataProvider.HasReliableTimeBase;

				double[] dataWithBg = measurements.Select(x => (double)(x.Measurement + x.Background)).ToArray();
				double[] frameIds = measurements.Select(x => (double)x.CurrFrameNo).ToArray();

				double[] dataBg = measurements.Select(x => (double)x.Background).ToArray();

				DateTime[] timestamps = measurements.Select(x => x.Timestamp).ToArray();

				hasReliableTimeBase = hasReliableTimeBase &&
					timestamps[0].Date != DateTime.MinValue &&
					timestamps[measurements.Length - 1].Date != DateTime.MinValue &&
					timestamps[0].Date.Ticks < timestamps[measurements.Length - 1].Ticks;

				if (!hasReliableTimeBase)
				{
					ShowErrorMessage("This light curve measurement does not have a reliable time base.");
					return;
				}

				double[] secondsFromUTMidnight = new double[timestamps.Length];
				long startFrameStartDayTicks = timestamps[0].Date.Ticks;

				for (int i = 0; i < timestamps.Length; i++)
				{
					if (timestamps[i] != DateTime.MinValue)
						secondsFromUTMidnight[i] = Math.Truncate(new TimeSpan(timestamps[i].Ticks - startFrameStartDayTicks).TotalSeconds * 10000) / 10000.0;
					else
						secondsFromUTMidnight[i] = 0;
				}

				// Now go and get the comparison star data
				if (dataProvider.NumberOfMeasuredComparisonObjects > 0)
				{
					ISingleMeasurement[] compMeasurements = dataProvider.GetComparisonObjectMeasurements(0);

					if (compMeasurements != null)
					{
						double[] compDataWithBg = compMeasurements.Select(x => (double)(x.Measurement + x.Background)).ToArray();
						double[] compBg = compMeasurements.Select(x => (double)x.Background).ToArray();

						double jdAtUtcMidnight = DateUtcToJulian(new DateTime(startFrameStartDayTicks));

						string outputFileName = CallKweeVanWoerden(frameIds.Length, jdAtUtcMidnight, secondsFromUTMidnight, dataWithBg, dataBg, compDataWithBg, compBg);

						if (outputFileName != null &&
							File.Exists(outputFileName))
						{
							Process.Start(outputFileName);
						}
					}
				}
			}
		}

		public static byte[] LoadBytesFromEmbeddedResource(string fileNameOnly)
		{
			Assembly assembly = Assembly.GetExecutingAssembly();
			using (Stream str = assembly.GetManifestResourceStream("Tangra.KweeVanWoerden.Resources." + fileNameOnly))
			using (BinaryReader rdr = new BinaryReader(str))
			{
				return rdr.ReadBytes((int)str.Length);
			}
		}

		private void ExecuteWithSimulatedData()
		{
			double jdAtUtcMidnight = DateUtcToJulian(DateTime.UtcNow.Date);

			double[] times = new double[10000];
			double[] varStar = new double[10000];
			double[] varSky = new double[10000];
			double[] compStar = new double[10000];
			double[] compSky = new double[10000];

			int dataPoints = 0;
			byte[] bytes = LoadBytesFromEmbeddedResource("Input_Observations.bin");
			using (var memStr = new MemoryStream(bytes))
			using (var rdr = new BinaryReader(memStr))
			{
				dataPoints = rdr.ReadInt32();
				for (int i = 0; i < dataPoints; i++)
				{
					times[i] = rdr.ReadDouble() * 86400; // Will be converted back to days inside CallKweeVanWoerden()
					varStar[i] = rdr.ReadDouble();
					varSky[i] = rdr.ReadDouble();
					compStar[i] = rdr.ReadDouble();
					compSky[i] = rdr.ReadDouble();
				}
			}

			string outputFileName = CallKweeVanWoerden(dataPoints, jdAtUtcMidnight, times, varStar, varSky, compStar, compSky);

			if (outputFileName != null &&
				File.Exists(outputFileName))
			{
				Process.Start(outputFileName);
			}
		}

		private string CallKweeVanWoerden(int numObs, double jdAtUtcMidnight, double[] timePoints, double[] dataWithBg, double[] dataBg, double[] compDataWithBg, double[] compBg)
		{			
			string nativeDllPath = Path.GetFullPath(FORTRAN_DLL_DIRECTORY_PATH + @"\kwee_van_woerden_subroutine.dll");
			Trace.WriteLine("DLL Path: " + nativeDllPath, "Eclipsing Binaries for Tangra");

			if (!File.Exists(nativeDllPath))
			{
				MessageBox.Show(m_TangraHost.ParentWindow, "Cannot find kwee_van_woerden_subroutine.dll", "Eclipsing Binaries for Tangra", MessageBoxButtons.OK, MessageBoxIcon.Stop);
				return null;
			}

			IntPtr pDll = NativeMethods.LoadLibrary(nativeDllPath);

			if (pDll != IntPtr.Zero)
			{
				try
				{
					IntPtr pAddressOfFunctionToCall = NativeMethods.GetProcAddress(pDll, "Kwee_van_Woerden");

					if (pAddressOfFunctionToCall != IntPtr.Zero)
					{
						KweeVanWoerdenDelegate kweeVanWoerden = (KweeVanWoerdenDelegate)Marshal.GetDelegateForFunctionPointer(pAddressOfFunctionToCall, typeof(KweeVanWoerdenDelegate));

						double[] times = new double[10000];
						double[] varStar = new double[10000];
						double[] varSky = new double[10000];
						double[] compStar = new double[10000];
						double[] compSky = new double[10000];

						int idx = 0;
						for (int i = 0; i < numObs; i++)
						{
							times[idx] = timePoints[i] / 86400.0; // Convert seconds to days
							varStar[idx] = dataWithBg[i];
							varSky[idx] = dataBg[i];
							compStar[idx] = compDataWithBg[i];
							compSky[idx] = compBg[i];
							idx++;
						}

						string outputDir = Path.GetFullPath(Path.GetTempPath() + "\\Tangra3Temp");
						if (!Directory.Exists(outputDir)) Directory.CreateDirectory(outputDir);
						string outputFile = Path.GetFullPath(outputDir + "\\Time_Of_Minimum_Summary.Txt");
						if (File.Exists(outputFile)) File.Delete(outputFile);

						char[] outputDirChars = new char[256];
						Array.Copy(outputDir.ToCharArray(), outputDirChars, outputDir.Length);
						outputDirChars[outputDir.Length] = ' '; // Must be space terminated

						double lastUTDateInJD = jdAtUtcMidnight;

						kweeVanWoerden(ref idx, ref lastUTDateInJD, times.ToArray(), varStar.ToArray(), varSky.ToArray(), compStar.ToArray(), compSky.ToArray(), outputDirChars);

						if (File.Exists(outputFile))
							return outputFile;
						else
							MessageBox.Show(m_TangraHost.ParentWindow, "No solution was found with the current data", "Eclipsing Binaries for Tangra", MessageBoxButtons.OK, MessageBoxIcon.Stop);
					}
				}
				finally
				{
					NativeMethods.FreeLibrary(pDll);
				}
			}
			else 
			{
				MessageBox.Show(m_TangraHost.ParentWindow, "Cannot load kwee_van_woerden_subroutine.dll", "Eclipsing Binaries for Tangra", MessageBoxButtons.OK, MessageBoxIcon.Stop);
			}

			return null;
		}

		public void Finalise()
		{
			
		}

		private void ShowErrorMessage(string errorMessage)
		{
			MessageBox.Show(
				m_TangraHost.ParentWindow,
				errorMessage,
				"Eclipsing Binaries Addin for Tangra",
				MessageBoxButtons.OK,
				MessageBoxIcon.Error);
		}
	}
}
