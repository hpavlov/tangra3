using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Tangra.SDK;

namespace Tangra.OccultTools.OccultWrappers
{
    public class OccultReflectionWrapper : IOccultWrapper
    {
        public bool HasSupportedVersionOfOccult(string occultLocation)
        {
            return OccultUtilitiesWrapper.HasSupportedVersionOfOccult(occultLocation);
        }

        public AotaReturnValue RunAOTA(SDK.ILightCurveDataProvider dataProvider, System.Windows.Forms.IWin32Window parentWindow)
        {
            return OccultUtilitiesWrapper.RunAOTA(dataProvider, parentWindow);
        }

        public void EnsureAOTAClosed()
        {
            OccultUtilitiesWrapper.EnsureAOTAClosed();
        }

        private static class OccultUtilitiesWrapper
        {
            public static Assembly AssemblyOccultUtilities;
            private static Type TYPE_AOTA_ExternalAccess;

            private static MethodInfo AOTA_Set_TargetData;
            private static MethodInfo AOTA_Set_TargetData_InclBg;
            private static MethodInfo AOTA_Set_TargetData_BackgroundAlreadySubtracted;
            private static MethodInfo AOTA_Set_FrameID;
            private static MethodInfo AOTA_Set_Comp1Data;
            private static MethodInfo AOTA_Set_Comp1Data_InclBg;
            private static MethodInfo AOTA_Set_Comp2Data;
            private static MethodInfo AOTA_Set_Comp2Data_InclBg;
            private static MethodInfo AOTA_Set_Comp3Data;
            private static MethodInfo AOTA_Set_Comp3Data_InclBg;
            private static MethodInfo AOTA_Set_TimeBase;
            private static MethodInfo AOTA_Set_TimeBaseEx;
            private static MethodInfo AOTA_Set_VideoCamera;
            private static MethodInfo AOTA_RunAOTA;
            private static MethodInfo AOTA_RunAOTAEx;
            private static MethodInfo AOTA_InitialiseAOTA;
            private static MethodInfo AOTA_CloseAOTA;
            private static PropertyInfo AOTA_ResultsCamera;
            private static PropertyInfo AOTA_ResultsForEvent1;
            private static PropertyInfo AOTA_ResultsForEvent2;
            private static PropertyInfo AOTA_ResultsForEvent3;
            private static PropertyInfo AOTA_ResultsForEvent4;
            private static PropertyInfo AOTA_ResultsForEvent5;
            private static PropertyInfo AOTA_ResultsReport;
            private static PropertyInfo AOTA_IsMiss;
            private static PropertyInfo AOTA_Version;
            private static PropertyInfo AOTA_ResultsAreAvailable;

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
                    //public void Set_TargetData_BackgroundAlreadySubtracted(float[] foreground, float[] background)
                    AOTA_Set_TargetData_BackgroundAlreadySubtracted = TYPE_AOTA_ExternalAccess.GetMethod("Set_TargetData_BackgroundAlreadySubtracted", new Type[] { typeof(float[]), typeof(float[]) });
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
                    //public void Set_VideoCamera(string VideoCamera)
                    AOTA_Set_VideoCamera = TYPE_AOTA_ExternalAccess.GetMethod("Set_VideoCamera", new Type[] { typeof(string) });
                    //public bool RunAOTA(IWin32Window parentWindow)
                    AOTA_RunAOTA = TYPE_AOTA_ExternalAccess.GetMethod("RunAOTA", new Type[] { typeof(IWin32Window) });
                    //public bool RunAOTA(IWin32Window parentWindow, int FirstFrame, int FramesInIntegration)    
                    AOTA_RunAOTAEx = TYPE_AOTA_ExternalAccess.GetMethod("RunAOTA", new Type[] { typeof(IWin32Window), typeof(int), typeof(int) });
                    //public void InitialiseAOTA()
                    AOTA_InitialiseAOTA = TYPE_AOTA_ExternalAccess.GetMethod("InitialiseAOTA", new Type[] { typeof(string) });
                    //public void CloseAOTA()
                    AOTA_CloseAOTA = TYPE_AOTA_ExternalAccess.GetMethod("CloseAOTA");

                    AOTA_ResultsCamera = TYPE_AOTA_ExternalAccess.GetProperty("Results_Camera");
                    AOTA_ResultsForEvent1 = TYPE_AOTA_ExternalAccess.GetProperty("ResultsForEvent1");
                    AOTA_ResultsForEvent2 = TYPE_AOTA_ExternalAccess.GetProperty("ResultsForEvent2");
                    AOTA_ResultsForEvent3 = TYPE_AOTA_ExternalAccess.GetProperty("ResultsForEvent3");
                    AOTA_ResultsForEvent4 = TYPE_AOTA_ExternalAccess.GetProperty("ResultsForEvent4");
                    AOTA_ResultsForEvent5 = TYPE_AOTA_ExternalAccess.GetProperty("ResultsForEvent5");
                    AOTA_ResultsReport = TYPE_AOTA_ExternalAccess.GetProperty("ResultsReport");
                    AOTA_IsMiss = TYPE_AOTA_ExternalAccess.GetProperty("IsMiss");
                    AOTA_Version = TYPE_AOTA_ExternalAccess.GetProperty("AOTA_Version");
                    AOTA_ResultsAreAvailable = TYPE_AOTA_ExternalAccess.GetProperty("ResultsAreAvailable");
                }
            }

            internal static AotaReturnValue RunAOTA(ILightCurveDataProvider dataProvider, IWin32Window parentWindow)
            {
                try
                {
                    if (m_AotaInstance == null)
                        m_AotaInstance = Activator.CreateInstance(TYPE_AOTA_ExternalAccess);

                    AOTA_InitialiseAOTA.Invoke(m_AotaInstance, new object[] { dataProvider.FileName });

                    ISingleMeasurement[] measurements = dataProvider.GetTargetMeasurements();

                    bool hasReliableTimeBase = dataProvider.HasReliableTimeBase;

                    float[] data = measurements.Select(x => x.Measurement).ToArray();
                    float[] frameIds = measurements.Select(x => (float)x.CurrFrameNo).ToArray();

                    if (AOTA_Set_TargetData_BackgroundAlreadySubtracted != null)
                    {
                        float[] dataBg = measurements.Select(x => x.Background).ToArray();
                        AOTA_Set_TargetData_BackgroundAlreadySubtracted.Invoke(m_AotaInstance, new object[] { data, dataBg });
                    }
                    else
                        AOTA_Set_TargetData.Invoke(m_AotaInstance, new object[] { data });

                    AOTA_Set_FrameID.Invoke(m_AotaInstance, new object[] { frameIds });

                    DateTime[] timestamps = measurements.Select(x => x.Timestamp).ToArray();

                    hasReliableTimeBase = hasReliableTimeBase &&
                        timestamps[0].Date != DateTime.MinValue &&
                        timestamps[measurements.Length - 1].Date != DateTime.MinValue &&
                        timestamps[0].Date.Ticks < timestamps[measurements.Length - 1].Ticks;

                    double[] secondsFromUTMidnight;
                    long startFrameStartDayTicks;

                    if (hasReliableTimeBase)
                    {
                        startFrameStartDayTicks = timestamps[0].Date.Ticks;
                        secondsFromUTMidnight = timestamps.Select(x => (Math.Truncate(new TimeSpan(x.Ticks - startFrameStartDayTicks).TotalSeconds * 10000) / 10000.0)).ToArray();
                    }
                    else
                    {
                        startFrameStartDayTicks = timestamps.FirstOrDefault(x => x != DateTime.MinValue).Date.Ticks;
                        secondsFromUTMidnight = new double[timestamps.Length];
                        for (int i = 0; i < timestamps.Length; i++)
                        {
                            if (timestamps[i] != DateTime.MinValue)
                                secondsFromUTMidnight[i] = Math.Truncate(new TimeSpan(timestamps[i].Ticks - startFrameStartDayTicks).TotalSeconds * 10000) / 10000.0;
                            else
                                secondsFromUTMidnight[i] = 0;
                        }
                    }

                    bool cameraCorrectionsHaveBeenApplied = dataProvider.CameraCorrectionsHaveBeenApplied;

                    if (AOTA_Set_TimeBaseEx != null)
                        AOTA_Set_TimeBaseEx.Invoke(m_AotaInstance, new object[] { secondsFromUTMidnight, cameraCorrectionsHaveBeenApplied });
                    else if (AOTA_Set_TimeBase != null)
                        AOTA_Set_TimeBase.Invoke(m_AotaInstance, new object[] { secondsFromUTMidnight });

                    if (cameraCorrectionsHaveBeenApplied)
                    {
                        string cameraName = dataProvider.VideoCameraName;
                        if (!string.IsNullOrEmpty(cameraName))
                            AOTA_Set_VideoCamera.Invoke(m_AotaInstance, new object[] { cameraName });
                    }

                    // Now go and set any comparison stars
                    for (int i = 0; i < dataProvider.NumberOfMeasuredComparisonObjects; i++)
                    {
                        ISingleMeasurement[] compMeasurements = dataProvider.GetComparisonObjectMeasurements(i);

                        if (compMeasurements != null)
                        {
                            float[] compData = compMeasurements.Select(x => x.Measurement).ToArray();

                            if (i == 0)
                                AOTA_Set_Comp1Data.Invoke(m_AotaInstance, new object[] { compData });
                            else if (i == 1)
                                AOTA_Set_Comp2Data.Invoke(m_AotaInstance, new object[] { compData });
                            else if (i == 2)
                                AOTA_Set_Comp3Data.Invoke(m_AotaInstance, new object[] { compData });
                        }
                    }

                    int firstFrameIndex = 0;// (int)frameIds[0];
                    int framesInIntegration = 1;

                    AOTA_RunAOTAEx.Invoke(m_AotaInstance, new object[] { null /*parentWindow*/, firstFrameIndex, framesInIntegration });

                    AotaReturnValue result = ReadAOTAResult();
                    result.IsMiss = (bool)AOTA_IsMiss.GetValue(m_AotaInstance, new object[] { });
                    result.AreResultsAvailable = (bool)AOTA_ResultsAreAvailable.GetValue(m_AotaInstance, new object[] { });
                    result.AOTAVersion = (string)AOTA_Version.GetValue(m_AotaInstance, new object[] { });

                    return result;
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.ToString());
                    MessageBox.Show(ex is TargetInvocationException ? ex.InnerException.Message : ex.Message, "AOTA Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                return null;
            }

            internal static void EnsureAOTAClosed()
            {
                if (m_AotaInstance != null)
                {
                    AOTA_CloseAOTA.Invoke(m_AotaInstance, new object[] { });
                    m_AotaInstance = null;
                }
            }

            internal static AotaReturnValue ReadAOTAResult()
            {
                var rv = new AotaReturnValue();

                if (m_AotaInstance != null)
                {
                    object cameraObj = AOTA_ResultsCamera.GetValue(m_AotaInstance, new object[] { });
                    object event1Obj = AOTA_ResultsForEvent1.GetValue(m_AotaInstance, new object[] { });
                    object event2Obj = AOTA_ResultsForEvent2.GetValue(m_AotaInstance, new object[] { });
                    object event3Obj = AOTA_ResultsForEvent3.GetValue(m_AotaInstance, new object[] { });
                    object event4Obj = AOTA_ResultsForEvent4.GetValue(m_AotaInstance, new object[] { });
                    object event5Obj = AOTA_ResultsForEvent5.GetValue(m_AotaInstance, new object[] { });

                    if (cameraObj != null)
                        ReflectionCopy(cameraObj, ref rv.CameraResult);

                    rv.EventResults = new EventResults[5];

                    if (event1Obj != null) ReflectionCopy(event1Obj, ref rv.EventResults[0]);
                    if (event2Obj != null) ReflectionCopy(event2Obj, ref rv.EventResults[1]);
                    if (event3Obj != null) ReflectionCopy(event3Obj, ref rv.EventResults[2]);
                    if (event4Obj != null) ReflectionCopy(event4Obj, ref rv.EventResults[3]);
                    if (event5Obj != null) ReflectionCopy(event5Obj, ref rv.EventResults[4]);
                }

                return rv;
            }

            private static void ReflectionCopy(object source, ref EventResults destination)
            {
                Type sourceType = source.GetType();

                destination.IsNonEvent = (bool)GetReflectedFieldValue(sourceType, source, "IsNonEvent", false);
                destination.D_Frame = (float)GetReflectedFieldValue(sourceType, source, "D_Frame", float.NaN);
                destination.R_Frame = (float)GetReflectedFieldValue(sourceType, source, "R_Frame", float.NaN);
                destination.D_FrameUncertPlus = (float)GetReflectedFieldValue(sourceType, source, "D_FrameUncertPlus", float.NaN);
                destination.D_FrameUncertMinus = (float)GetReflectedFieldValue(sourceType, source, "D_FrameUncertMinus", float.NaN);
                destination.R_FrameUncertPlus = (float)GetReflectedFieldValue(sourceType, source, "R_FrameUncertPlus", float.NaN);
                destination.R_FrameUncertMinus = (float)GetReflectedFieldValue(sourceType, source, "R_FrameUncertMinus", float.NaN);
                destination.D_UTC = (string)GetReflectedFieldValue(sourceType, source, "D_UTC", null);
                destination.R_UTC = (string)GetReflectedFieldValue(sourceType, source, "R_UTC", null);
                destination.D_DurationFrames = (float)GetReflectedFieldValue(sourceType, source, "D_DurationFrames", float.NaN);
                destination.R_DurationFrames = (float)GetReflectedFieldValue(sourceType, source, "R_DurationFrames", float.NaN);
            }

            private static void ReflectionCopy(object source, ref Camera destination)
            {
                Type sourceType = source.GetType();

                destination.CameraType = (string)GetReflectedFieldValue(sourceType, source, "CameraType", null);
                destination.MeasuringTool = (string)GetReflectedFieldValue(sourceType, source, "MeasuringTool", null);
                destination.VideoSystem = (string)GetReflectedFieldValue(sourceType, source, "VideoSystem", null);
                destination.FramesIntegrated = (int)GetReflectedFieldValue(sourceType, source, "FramesIntegrated", 0);
                destination.MeasurementsBinned = (int)GetReflectedFieldValue(sourceType, source, "MeasurementsBinned", 0);
                destination.MeasuredAtFieldLevel = (bool)GetReflectedFieldValue(sourceType, source, "MeasuredAtFieldLevel", false);
                destination.TimeScaleFromMeasuringTool = (bool)GetReflectedFieldValue(sourceType, source, "TimeScaleFromMeasuringTool", false);
            }

            private static object GetReflectedFieldValue(Type sourceType, object source, string fieldName, object defaultValue)
            {
                FieldInfo srcField = sourceType.GetField(fieldName);
                if (srcField != null)
                    return srcField.GetValue(source);
                else
                    return defaultValue;
            }
        }
    }
}
