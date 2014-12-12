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
			get { return "Extract Occulting Binary Minimum Time (Kwee-Van Woerden Method)"; }
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
				ShowErrorMessage("Extract Occulting Binary Minimum Time is already running.");
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
						"Occulting Binaries for Tangra", 
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

						KweeVanWoerdenResult result = Kwee_van_Woerden(frameIds.Length, jdAtUtcMidnight, secondsFromUTMidnight, dataWithBg, dataBg, compDataWithBg, compBg);

						PresentResults(result);
					}
				}
			}
		}

		private void PresentResults(KweeVanWoerdenResult result)
		{
			var frmResults = new frmResults();
			frmResults.Results = result;
			frmResults.StartPosition = FormStartPosition.CenterParent;
			frmResults.ShowDialog(m_TangraHost.ParentWindow);
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
			double[] times;
			double[] varStar;
			double[] varSky;
			double[] compStar;
			double[] compSky;

			
			int dataPoints = 0;
			
			byte[] bytes = LoadBytesFromEmbeddedResource("Input_Observations.bin");
			double jdAtUtcMidnight = 2456967.1234567;

			using (var memStr = new MemoryStream(bytes))
			using (var rdr = new BinaryReader(memStr))
			{
				dataPoints = rdr.ReadInt32();

				times = new double[dataPoints];
				varStar = new double[dataPoints];
				varSky = new double[dataPoints];
				compStar = new double[dataPoints];
				compSky = new double[dataPoints];

				for (int i = 0; i < dataPoints; i++)
				{
					times[i] = rdr.ReadDouble();
					varStar[i] = rdr.ReadDouble();
					varSky[i] = rdr.ReadDouble();
					compStar[i] = rdr.ReadDouble();
					compSky[i] = rdr.ReadDouble();
				}
			}

			KweeVanWoerdenResult result = Kwee_van_Woerden(dataPoints, jdAtUtcMidnight, times, varStar, varSky, compStar, compSky);

			PresentResults(result);
		}

		[Obsolete("The old implementation calling the Fortran library")]
		private string CallKweeVanWoerden(int numObs, double jdAtUtcMidnight, double[] timePoints, double[] dataWithBg, double[] dataBg, double[] compDataWithBg, double[] compBg)
		{			
			string nativeDllPath = Path.GetFullPath(FORTRAN_DLL_DIRECTORY_PATH + @"\kwee_van_woerden_subroutine.dll");
			Trace.WriteLine("DLL Path: " + nativeDllPath, "Occulting Binaries for Tangra");

			if (!File.Exists(nativeDllPath))
			{
				MessageBox.Show(m_TangraHost.ParentWindow, "Cannot find kwee_van_woerden_subroutine.dll", "Occulting Binaries for Tangra", MessageBoxButtons.OK, MessageBoxIcon.Stop);
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
							times[idx] = timePoints[i];
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
							MessageBox.Show(m_TangraHost.ParentWindow, "No solution was found with the current data", "Occulting Binaries for Tangra", MessageBoxButtons.OK, MessageBoxIcon.Stop);
					}
				}
				finally
				{
					NativeMethods.FreeLibrary(pDll);
				}
			}
			else 
			{
				MessageBox.Show(m_TangraHost.ParentWindow, "Cannot load kwee_van_woerden_subroutine.dll", "Occulting Binaries for Tangra", MessageBoxButtons.OK, MessageBoxIcon.Stop);
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
				"Occulting Binaries Addin for Tangra",
				MessageBoxButtons.OK,
				MessageBoxIcon.Error);
		}

		private KweeVanWoerdenResult Kwee_van_Woerden(long Number_Obs, double Time_First, double[] Time, double[] Variable_Star_DN, double[] Variable_Sky_DN, double[] Comparison_Star_DN, double[] Comparison_Sky_DN)
		{
			/* Constant parameter */
			long         Normal_Points = 101;                     /* Number of evenly spaced data pairs used in the analysis */
			long         Normal_Point_Middle;                     /* Index of the middle element of the normal point array */

			/* Luminosity values */
			double[]     Luminosity_Ratio;						  /* Variable luminosity divided by comparison luminosity */
			double       Luminosity_Normal_Sum;                   /* Summation of luminosities for normal point computation */
			long         Luminosity_Normal_Count;                 /* Counter for data points in one normal point */
			double[]     Luminosity_Normal = new double[102];     /* Normalized luminosities */

			/* Time values */
			double       Time_Interval;                           /* Time interval between observations */
			double       Time_Start, Time_Stop;                   /* Limits for normal points */
			double[]     Time_Normal = new double[102];           /* Times corresponding to normal points */

			/* Symmetry analysis */
			double       Luminosity_Faintest;                     /* Faintest normalized luminosity */
			long         Luminosity_Faintest_Index;               /* Index of the faintest normalized luminosity */
			long         Start_Light_Curve;                       /* Start element of normalized light curve to analyze */
			long         Middle_Light_Curve;                      /* Middle element of normalized light curve to analyze */
			long         Stop_Light_Curve;                        /* Stop element of normalized light curve to analyze */
			long         Start_Sum_Squares;                       /* Start element of the sum-of-squares array */
			long         Stop_Sum_Squares;                        /* Stop element of the sum-of-squares array */
			double       Sum_Of_Squares;                          /* Sum of squares across a time of symmetry */
			long[]       Sum_Of_Squares_Count = new long[102];    /* Number of squares accumulated */
			double[]     Sum_Of_Squares_Mean = new double[102];   /* Sum of squares divided by count */
			long         Sum_Of_Squares_Smallest_Index;           /* Index of the smallest sum of squares */
			double       Sum_Of_Squares_Smallest;                 /* Smallest sum of squares */

			/* Time of minimum and uncertainty */
			double[]     Quad = new double[4];                    /* Inputs to the quadratic equation taken from the sums of squares */
			double       A, B, C;                                 /* Intermediate values for quadratic equation */
			double       T0;                                      /* Time of symmetry */
			double       T0_Uncertainty;                          /* Time of symmetry uncertainty */
			double       Time_Of_Minimum;                         /* Time of minimum light */
			double       Time_Of_Minimum_Uncertainty;             /* Time of minimum light uncertainty */

			/* Loop indices */
			long         i, j;                                    /* Loop indices */

			var rv = new KweeVanWoerdenResult()
			{
				NumberObservations = Number_Obs
			};

			rv.Observations_File.Add("                Time     Luminosity_Ratio     Variable_Star_DN      Variable_Sky_DN   Comparison_Star_DN    Comparison_Sky_DN");
			rv.Normals_File.Add("                Time    Luminosity_Ratio");

			Luminosity_Ratio = new double[Number_Obs];
			/* Determine luminosity ratios */
			for (i = 0; i < Number_Obs; i++) 
			{
				Luminosity_Ratio[i] = ( Variable_Star_DN[i] - Variable_Sky_DN[i] ) / ( Comparison_Star_DN[i] - Comparison_Sky_DN[i] );

				rv.Observations_File.Add(
					Time[i].ToString("#########0.000000").PadLeft(20) + " " +
					Luminosity_Ratio[i].ToString("#########0.000000").PadLeft(20) + " " +
					Variable_Star_DN[i].ToString("#########0.000000").PadLeft(20) + " " +
					Variable_Sky_DN[i].ToString("#########0.000000").PadLeft(20) + " " +
					Comparison_Star_DN[i].ToString("#########0.000000").PadLeft(20) + " " +
					Comparison_Sky_DN[i].ToString("#########0.000000").PadLeft(20));
			}

			/* Compute normal point times and luminosities */
			Time_Interval = ( Time[Number_Obs - 1] - Time[0] ) / (float)Normal_Points;
			for (i = 0; i < Normal_Points; i++) 
			{
				Time_Start = Time[0] + i * Time_Interval;
				Time_Normal[i] = Time_Start + Time_Interval / 2.0;
				Time_Stop = Time_Start + Time_Interval;
				Luminosity_Normal_Sum = 0.0;
				Luminosity_Normal_Count = 0;
				for (j = 0; j < Number_Obs; j++ ) 
				{
					if ( ( Time[j] >= Time_Start ) && ( Time[j] < Time_Stop) ) 
					{
						Luminosity_Normal_Sum = Luminosity_Normal_Sum + Luminosity_Ratio[j];
						Luminosity_Normal_Count = Luminosity_Normal_Count + 1;
					}
				}

				if ( Luminosity_Normal_Count > 0 ) 
				{
					Luminosity_Normal[i] = Luminosity_Normal_Sum / (float)Luminosity_Normal_Count;
				}
				else 
				{
					rv.ErrorMessage = string.Format("Bad value for Luminosity_Normal_Count[{0}] = {1}", i, Luminosity_Normal_Count);
					return rv;
				}

				rv.Normals_File.Add(Time_Normal[i].ToString("#########0.000000").PadLeft(20) + " " + Luminosity_Normal[i].ToString("#########0.000000").PadLeft(20));
			}

			/* Locate the faintest luminosity */
			Luminosity_Faintest = 1000000000.0;
			Luminosity_Faintest_Index = 0;
			for (i = 0; i < Normal_Points; i++) 
			{
				if ( Luminosity_Normal[i] < Luminosity_Faintest ) 
				{
					Luminosity_Faintest_Index = i;
					Luminosity_Faintest = Luminosity_Normal[i];
				}
			}

			/* Set the limits of the light curve to be symmetrical around the faintest luminosity */
			Start_Light_Curve = 0;
			Stop_Light_Curve = Normal_Points - 1;
			Normal_Point_Middle = Normal_Points / 2;
			if ( Luminosity_Faintest_Index < Normal_Point_Middle ) { Stop_Light_Curve = 2 * Luminosity_Faintest_Index; }
			if ( Luminosity_Faintest_Index > Normal_Point_Middle ) { Start_Light_Curve = 2 * Luminosity_Faintest_Index - Normal_Points + 1; }
			rv.IncludedObservations = Stop_Light_Curve - Start_Light_Curve - 2;

			/* Compute the normalized sums of squares of luminosity differences across an array of times */
			Start_Sum_Squares = Start_Light_Curve + 1;
			Stop_Sum_Squares  = Stop_Light_Curve  - 1;
			for (i = Start_Sum_Squares; i <= Stop_Sum_Squares; i++) 
			{
				Sum_Of_Squares = 0.0;
				Sum_Of_Squares_Count[i] = 0;

				for (j = 0; j < Normal_Points; j++) 
				{
					if ( i - j >= 0 && i + j < Normal_Points ) 
					{
						Sum_Of_Squares = Sum_Of_Squares + Math.Pow(Luminosity_Normal[i-j] - Luminosity_Normal[i+j], 2);
						Sum_Of_Squares_Count[i] = Sum_Of_Squares_Count[i] + 1;
					}
				}

				Sum_Of_Squares_Mean[i] = Sum_Of_Squares / (float) Sum_Of_Squares_Count[i];
			}

			/* Find the smallest normalized sum of squares */
			Sum_Of_Squares_Smallest = 1000000000;
			Sum_Of_Squares_Smallest_Index = 0;
			for (i = Start_Sum_Squares; i <= Stop_Sum_Squares; i++) 
			{
				if ( Sum_Of_Squares_Mean[i] < Sum_Of_Squares_Smallest ) 
				{
					/* Must also have a reasonable sample of points */
					if ( Sum_Of_Squares_Count[i] > Normal_Points / 10 ) 
					{
						Sum_Of_Squares_Smallest_Index = i;
						Sum_Of_Squares_Smallest = Sum_Of_Squares_Mean[i];
					}
				}
			}

			/* Solve the quadratic equation */
			for (i = 0; i <= 3; i++) {
				Quad[i] = Sum_Of_Squares_Mean[Sum_Of_Squares_Smallest_Index+i-2];
			}

			A = -Quad[2] + ( Quad[3] + Quad[1] ) / 2.0;
			B = -3.0 * A + Quad[2] - Quad[1];
			C = Quad[1] - A - B;

			/* Calculate the time of minimum and uncertainty */			
			T0 = -B / ( 2.0 * A );
			T0_Uncertainty = Math.Sqrt((4.0 * A * C - B * B) / (4.0 * A * A) / ((float)Normal_Points / 4.0 - 1.0));

			rv.T0 = T0;
			rv.T0_Uncertainty = T0_Uncertainty;			
			rv.Time_Of_Minimum = Time_Normal[1] + ((Sum_Of_Squares_Smallest_Index - 1) + (T0 - 2.0)) * Time_Interval;
			rv.Time_Of_Minimum_JD = Time_First + rv.Time_Of_Minimum;
			rv.Time_Of_Minimum_Uncertainty = T0_Uncertainty * Time_Interval;
			rv.Success = true;

			rv.Summary_File.Add(string.Format("      Time_First               Number_Obs"));
			rv.Summary_File.Add(Time_First.ToString("#########0.000000").PadLeft(20) + " " + Number_Obs.ToString("#########0").PadLeft(20));
			rv.Summary_File.Add(string.Format("\n               Time of minimum: \n"));
			rv.Summary_File.Add(string.Format("      Bins-Quadratic          Uncertainty"));
			rv.Summary_File.Add(T0.ToString("#########0.000000").PadLeft(20) + " " + T0_Uncertainty.ToString("#########0.000000").PadLeft(20));
			rv.Summary_File.Add(string.Format("      From Time_First         Uncertainty"));
			rv.Summary_File.Add(rv.Time_Of_Minimum.ToString("#########0.000000").PadLeft(20) + " " + rv.Time_Of_Minimum_Uncertainty.ToString("#########0.000000").PadLeft(20));
			rv.Summary_File.Add(string.Format("      Absolute                Uncertainty"));
			rv.Summary_File.Add(rv.Time_Of_Minimum_JD.ToString("#########0.000000").PadLeft(20) + " " + rv.Time_Of_Minimum_Uncertainty.ToString("#########0.000000").PadLeft(20));

			/* Finish */
			return rv;
		}

		internal class KweeVanWoerdenResult
		{
			public long NumberObservations;
			public long IncludedObservations;
			public double T0;			
			public double T0_Uncertainty;
			public double Time_Of_Minimum;
			public double Time_Of_Minimum_JD;
			public double Time_Of_Minimum_Uncertainty;

			public bool Success;
			public string ErrorMessage;

			public List<string> Summary_File = new List<string>();
			public List<string> Observations_File = new List<string>();
			public List<string> Normals_File = new List<string>();
		}
	}
}
