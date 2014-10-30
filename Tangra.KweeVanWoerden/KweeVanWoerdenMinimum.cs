using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Tangra.SDK;

namespace Tangra.KweeVanWoerden
{
	[Serializable]
	public class KweeVanWoerdenMinimum : MarshalByRefObject, ITangraAddinAction
	{
		[DllImport("kwee_van_woerden_subroutine.dll", CallingConvention = CallingConvention.Cdecl)]
		private static extern void Kwee_van_Woerden([In] int number_obs, [In] double[] time, [In] double[] var_star, [In] double[] var_sky, [In] double[] comp_star, [In] double[] comp_sky);

		private ITangraHost m_TangraHost;
		private bool m_Running;

		internal KweeVanWoerdenMinimum(ITangraHost tangraHost)
		{
			m_TangraHost = tangraHost;
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

					double[] secondsFromUTMidnight= new double[timestamps.Length];
					long startFrameStartDayTicks = timestamps[0].Date.Ticks;

					for (int i = 0; i < timestamps.Length; i++)
					{
						if (timestamps[i] != DateTime.MinValue)
							secondsFromUTMidnight[i] = Math.Truncate(new TimeSpan(timestamps[i].Ticks - startFrameStartDayTicks).TotalSeconds * 10000) / 10000.0;
						else
							secondsFromUTMidnight[i] = 0;
					}

					// Now go and get the comparison star data
					if(dataProvider.NumberOfMeasuredComparisonObjects > 0)
					{
						ISingleMeasurement[] compMeasurements = dataProvider.GetComparisonObjectMeasurements(0);

						if (compMeasurements != null)
						{
							double[] compDataWithBg = compMeasurements.Select(x => (double)(x.Measurement + x.Background)).ToArray();
							double[] compBg = compMeasurements.Select(x => (double)x.Background).ToArray();

							Kwee_van_Woerden(frameIds.Length, secondsFromUTMidnight, dataWithBg, dataBg, compDataWithBg, compBg);

							// TODO: Load the 'Time_Of_Minimum_Summary.Txt' file and parse it:
							
							//              T0              Uncertainty     Time_Of_Minimum        Uncertainty
							//            2.341183            0.304325       596737.227733         3311.417467
							
							// TODO: Convert the Time_Of_Minium into a valid Epoch and the Uncertainty into valid time units. Should we use JD for this?

							double secFromMidnightOfMinimum = 0.1;
							double errorInSeconds = 0.2;

							long ticksOfMinimum = startFrameStartDayTicks + TimeSpan.FromSeconds(secFromMidnightOfMinimum).Ticks;
							DateTime epochOfMinimum = new DateTime(ticksOfMinimum);

							// TODO: Write the Time_Of_Minimum and T0 back into the file in UT time format down to 0.1 second 
							// TODO: Open up the Time_Of_Minimum_Summary.Txt in Notepad or default .txt file software
						}
					}
				}
			}
			finally
			{
				m_Running = false;
			}
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
