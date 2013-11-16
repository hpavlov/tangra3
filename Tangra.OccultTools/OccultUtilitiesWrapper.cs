using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tangra.SDK;

namespace Tangra.OccultTools
{
	public static class OccultUtilitiesWrapper
	{
		public static Assembly AssemblyOccultUtilities;
		private static Type TYPE_AOTA_ExternalAccess;

		private static MethodInfo AOTA_Set_TargetData;
		private static MethodInfo AOTA_Set_TargetData_InclBg;
		private static MethodInfo AOTA_Set_FrameID;
		private static MethodInfo AOTA_Set_Comp1Data;
		private static MethodInfo AOTA_Set_Comp1Data_InclBg;
		private static MethodInfo AOTA_Set_Comp2Data;
		private static MethodInfo AOTA_Set_Comp2Data_InclBg;
		private static MethodInfo AOTA_Set_Comp3Data;
		private static MethodInfo AOTA_Set_Comp3Data_InclBg;
		private static MethodInfo AOTA_Set_TimeBase;
        private static MethodInfo AOTA_Set_TimeBaseEx;
		private static MethodInfo AOTA_RunAOTA;
	    private static MethodInfo AOTA_RunAOTAEx;
		private static MethodInfo AOTA_InitialiseAOTA;
	    private static MethodInfo AOTA_CloseAOTA;

		private static BindingFlags OccultBindingFlags;

		private static bool? s_IsOccultSupported = null;

	    private static object m_AotaInstance = null;

		public static bool HasSupportedVersionOfOccult(string occultLocation)
		{
			try
			{
				if (s_IsOccultSupported == null)
				{
					LoadOccultUtilitiesAssembly(occultLocation);

					if (TYPE_AOTA_ExternalAccess != null)
					{
						s_IsOccultSupported = true;
						return true;
					}

					s_IsOccultSupported = false;
				}
				else
					return s_IsOccultSupported.Value;
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex);
			}

			return false;
		}

		private static void LoadOccultUtilitiesAssembly(string occultLocation)
		{
			if (AssemblyOccultUtilities == null)
			{
				string path = Path.Combine(occultLocation, "OccultUtilities.dll");
				AssemblyOccultUtilities = Assembly.UnsafeLoadFrom(path);

				TYPE_AOTA_ExternalAccess = AssemblyOccultUtilities.GetType("AOTA.AOTA_ExternalAccess");

				//public void Set_TargetData(float[] data)				
				AOTA_Set_TargetData = TYPE_AOTA_ExternalAccess.GetMethod("Set_TargetData", new Type[] { typeof(float[]) });
				//public void Set_TargetData(float[] foreground, float[] background)
				AOTA_Set_TargetData_InclBg = TYPE_AOTA_ExternalAccess.GetMethod("Set_TargetData", new Type[] { typeof(float[]), typeof(float[]) });
				//public void Set_FrameID(float[] data)
				AOTA_Set_FrameID = TYPE_AOTA_ExternalAccess.GetMethod("Set_FrameID", new Type[] { typeof(float[]) });
				//public void Set_Comp1Data(float[] data)
				AOTA_Set_Comp1Data = TYPE_AOTA_ExternalAccess.GetMethod("Set_Comp1Data", new Type[] { typeof(float[]) });
				//public void Set_Comp1Data(float[] foreground, float[] background)
				AOTA_Set_Comp1Data_InclBg = TYPE_AOTA_ExternalAccess.GetMethod("Set_Comp1Data", new Type[] { typeof(float[]), typeof(float[]) });
				//public void Set_Comp2Data(float[] data)
				AOTA_Set_Comp2Data = TYPE_AOTA_ExternalAccess.GetMethod("Set_Comp2Data", new Type[] { typeof(float[]) });
				//public void Set_Comp2Data(float[] foreground, float[] background)
				AOTA_Set_Comp2Data_InclBg = TYPE_AOTA_ExternalAccess.GetMethod("Set_Comp2Data", new Type[] { typeof(float[]), typeof(float[]) });
				//public void Set_Comp3Data(float[] data)
				AOTA_Set_Comp3Data = TYPE_AOTA_ExternalAccess.GetMethod("Set_Comp3Data", new Type[] { typeof(float[]) });
				//public void Set_Comp3Data(float[] foreground, float[] background)
				AOTA_Set_Comp3Data_InclBg = TYPE_AOTA_ExternalAccess.GetMethod("Set_Comp2Data", new Type[] { typeof(float[]), typeof(float[]) });
				//public void Set_TimeBase(double[] data)
				AOTA_Set_TimeBase = TYPE_AOTA_ExternalAccess.GetMethod("Set_TimeBase", new Type[] { typeof(double[]) });
                //public void Set_TimeBase(double[] data, bool CameraCorrectionsHaveBeenApplied)
                AOTA_Set_TimeBaseEx = TYPE_AOTA_ExternalAccess.GetMethod("Set_TimeBase", new Type[] { typeof(double[]), typeof(bool) });
				//public bool RunAOTA(IWin32Window parentWindow)
                AOTA_RunAOTA = TYPE_AOTA_ExternalAccess.GetMethod("RunAOTA", new Type[] { typeof(IWin32Window) });
                //public bool RunAOTA(IWin32Window parentWindow, int FirstFrame, int FramesInIntegration)    
                AOTA_RunAOTAEx = TYPE_AOTA_ExternalAccess.GetMethod("RunAOTA", new Type[] { typeof(IWin32Window), typeof(int), typeof(int) });
				//public void InitialiseAOTA()
				AOTA_InitialiseAOTA = TYPE_AOTA_ExternalAccess.GetMethod("InitialiseAOTA");
                //public void CloseAOTA()
                AOTA_CloseAOTA = TYPE_AOTA_ExternalAccess.GetMethod("CloseAOTA");
			}
		}

		internal static void RunAOTA(ILightCurveDataProvider dataProvider, IWin32Window parentWindow)
		{
            try
            {
                if (m_AotaInstance == null)
                    m_AotaInstance = Activator.CreateInstance(TYPE_AOTA_ExternalAccess);

                AOTA_InitialiseAOTA.Invoke(m_AotaInstance, new object[] { });

                ISingleMeasurement[] measurements = dataProvider.GetTargetMeasurements();

                bool hasReliableTimeBase = dataProvider.HasReliableTimeBase;

                float[] data = measurements.Select(x => x.Measurement).ToArray();
                float[] frameIds = measurements.Select(x => (float)x.CurrFrameNo).ToArray();

                AOTA_Set_TargetData.Invoke(m_AotaInstance, new object[] { data });
                AOTA_Set_FrameID.Invoke(m_AotaInstance, new object[] { frameIds });

                DateTime[] timestamps = measurements.Select(x => x.Timestamp).ToArray();

                hasReliableTimeBase = hasReliableTimeBase &&
                    timestamps[0].Date != DateTime.MinValue &&
                    timestamps[measurements.Length - 1].Date != DateTime.MinValue &&
                    timestamps[0].Date.Ticks < timestamps[measurements.Length - 1].Ticks;

                if (hasReliableTimeBase)
                {
                    long startFrameStartDayTicks = timestamps[0].Date.Ticks;
                    double[] secondsFromUTMidnight = timestamps.Select(x => (Math.Truncate(new TimeSpan(x.Ticks - startFrameStartDayTicks).TotalSeconds * 10000) / 10000.0)).ToArray();

                    bool cameraCorrectionsHaveBeenApplied = dataProvider.CameraCorrectionsHaveBeenApplied;

                    if (AOTA_Set_TimeBaseEx != null)
                        AOTA_Set_TimeBaseEx.Invoke(m_AotaInstance, new object[] { secondsFromUTMidnight, cameraCorrectionsHaveBeenApplied });
                    else if (AOTA_Set_TimeBase != null)
                        AOTA_Set_TimeBase.Invoke(m_AotaInstance, new object[] { secondsFromUTMidnight });
                }

                AOTA_RunAOTAEx.Invoke(m_AotaInstance, new object[] { null /*parentWindow*/, 0, 1 });
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
                MessageBox.Show(ex is TargetInvocationException ? ex.InnerException.Message : ex.Message, "AOTA Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
		}

        internal static void EnsureAOTAClosed()
        {
            if (m_AotaInstance != null)
            {
                AOTA_CloseAOTA.Invoke(m_AotaInstance, new object[] { });
                m_AotaInstance = null;
            }
            
        }
	}
}
