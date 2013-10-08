using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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
		private static MethodInfo AOTA_RunAOTA;

		private static BindingFlags OccultBindingFlags;

		private static bool? s_IsOccultSupported = null;

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
				AssemblyOccultUtilities = Assembly.LoadFile(path);

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
				//public void Set_TimeBase(float[] data)
				AOTA_Set_TimeBase = TYPE_AOTA_ExternalAccess.GetMethod("Set_TimeBase", new Type[] { typeof(float[]) });
				//public bool RunAOTA(IWin32Window parentWindow)
				AOTA_RunAOTA = TYPE_AOTA_ExternalAccess.GetMethod("RunAOTA");
			}
		}

		internal static void RunAOTA(ILightCurveDataProvider dataProvider)
		{
			object aotaInstance = Activator.CreateInstance(TYPE_AOTA_ExternalAccess);

			ISingleMeasurement[] measurements = dataProvider.GetTargetMeasurements();

			float[] data = measurements.Select(x => x.Measurement).ToArray();
			float[] frameIds = measurements.Select(x => (float)x.CurrFrameNo).ToArray();

			AOTA_Set_TargetData.Invoke(aotaInstance, new object[] { data });
			AOTA_Set_FrameID.Invoke(aotaInstance, new object[] { frameIds });
			AOTA_RunAOTA.Invoke(aotaInstance, new object[] { null });
		}
	}
}
