using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Occult.SDK;
using Tangra.SDK;

namespace Tangra.OccultTools.OccultWrappers
{
    public class OccultSDKWrapper : IOccultWrapper
    {
        private const string MIN_VERSION_OCCULT_REQUIRED = "4.1.0.25";
        private const string MIN_VERSION_TANGRA_REQUIRED = "3.0.84";

        private IAOTAClientCallbacks m_ClientCallbacks;

        public OccultSDKWrapper(IAOTAClientCallbacks clientCallbacks)
        {
            m_ClientCallbacks = clientCallbacks;
        }

        public string HasSupportedVersionOfOccult(string occultLocation)
        {
            return OccultUtilitiesSDKWrapper.HasSupportedVersionOfOccult(occultLocation);
        }

        public string GetOccultCurrentOccultVersion(string occultLocation)
        {
            return OccultUtilitiesSDKWrapper.GetOccultCurrentOccultVersion(occultLocation);
        }

        public bool RunAOTA(SDK.ILightCurveDataProvider dataProvider, System.Windows.Forms.IWin32Window parentWindow)
        {
            return OccultUtilitiesSDKWrapper.RunAOTA(dataProvider, parentWindow, m_ClientCallbacks);
        }

        public AotaReturnValue GetAOTAResult()
        {
            return OccultUtilitiesSDKWrapper.GetAOTAResult();
        }

        public void EnsureAOTAClosed()
        {
            OccultUtilitiesSDKWrapper.EnsureAOTAClosed();
        }

        public void NotifyAOTAOfCurrentFrameChanged(int currFrameId)
        {
            OccultUtilitiesSDKWrapper.NotifyAOTAOfCurrentFrameChanged(currFrameId);
        }

        private static class OccultUtilitiesSDKWrapper
        {
            public static Assembly AssemblyOccultUtilities;
            private static Type TYPE_AOTA_ExternalAccess;

            private static bool? s_IsOccultSupported = null;

            private static Occult.SDK.IAOTAExternalAccess m_AotaInstance = null;

            private static string s_IncompatibleVersionOfOccultErrorMessage = null;

            public static string HasSupportedVersionOfOccult(string occultLocation)
            {
                try
                {
                    if (s_IsOccultSupported == null)
                    {
                        LoadOccultUtilitiesAssemblyAndSetIncompatibleVersionOfOccultErrorMessage(occultLocation);

                        if (TYPE_AOTA_ExternalAccess != null)
                        {
                            s_IsOccultSupported = true;
                            s_IncompatibleVersionOfOccultErrorMessage = null;
                            return null;
                        }

                        s_IsOccultSupported = false;
                        return s_IncompatibleVersionOfOccultErrorMessage;
                    }
                    else
                        return s_IncompatibleVersionOfOccultErrorMessage;
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex);
                }

                return "There was an error determining the required version of Occult. Probably your version of Occult and Tangra3 are not compatible.";
            }

            public static string GetOccultCurrentOccultVersion(string occultLocation)
            {
                string error = HasSupportedVersionOfOccult(occultLocation);
                if (error == null && AssemblyOccultUtilities != null)
                    return "Using Occult v" + AssemblyOccultUtilities.GetName().Version.ToString() + ".";

                return error;
            }

            private static void LoadOccultUtilitiesAssemblyAndSetIncompatibleVersionOfOccultErrorMessage(string occultLocation)
            {
                if (AssemblyOccultUtilities == null)
                {
                    string path = Path.Combine(occultLocation, "OccultUtilities.dll");
                    if (!File.Exists(path))
                    {
                        s_IncompatibleVersionOfOccultErrorMessage = "Cannot find Occult at the specified location.";
                        return;
                    }

                    AssemblyOccultUtilities = Assembly.UnsafeLoadFrom(path);

                    try
                    {
                        AssemblyOccultUtilities.GetTypes();
                    }
                    catch (ReflectionTypeLoadException tlex)
                    {
                        var fileNotFoundExceptions = tlex.LoaderExceptions.Where(x => x is FileNotFoundException).ToList();
                        if (fileNotFoundExceptions.Count > 0)
                        {
                            string notFoundFileNames = fileNotFoundExceptions.Select(x => ((FileNotFoundException) x).FileName)
                                                  .Where(x => x.Contains("Occult.SDK, Version="))
                                                  .FirstOrDefault();

                            if (!string.IsNullOrEmpty(notFoundFileNames))
                            {
                                // The version of Occult uses a different version of Occult.SDK
                                Regex verRegex = new Regex("^Occult\\.SDK, Version=(?<Mj>[0-9]+)\\.(?<MjR>[0-9]+)\\.(?<Mn>[0-9]+)\\.(?<MnR>[0-9]+), Culture=neutral, PublicKeyToken=7216253074b08f24$");
                                Match match = verRegex.Match(notFoundFileNames);
                                int requiredMajor = int.Parse(match.Groups["Mj"].Value);
                                int requiredMajorRevision = int.Parse(match.Groups["MjR"].Value);
                                int requiredMinor = int.Parse(match.Groups["Mn"].Value);
                                int requiredMinorRevision = int.Parse(match.Groups["MnR"].Value);
                                long requiredVersion = requiredMajor*0x1000000000000 +
                                                       requiredMajorRevision * 0x100000000 +
                                                       requiredMinor * 0x10000 + 
                                                       requiredMinorRevision;

                                int currentMajor = typeof (IAOTAExternalAccess).Assembly.GetName().Version.Major;
                                int currentMajorRevision = typeof(IAOTAExternalAccess).Assembly.GetName().Version.MajorRevision;
                                int currentMinor = typeof(IAOTAExternalAccess).Assembly.GetName().Version.Minor;
                                int currentMinorRevision = typeof(IAOTAExternalAccess).Assembly.GetName().Version.MinorRevision;

                                long currentVersion = currentMajor * 0x1000000000000 +
                                                       currentMajorRevision * 0x100000000 +
                                                       currentMinor * 0x10000 +
                                                       currentMinorRevision;

                                if (requiredVersion > currentVersion)
                                {
                                    s_IncompatibleVersionOfOccultErrorMessage = "Your version of Occult is newer than the version supported by Tangra3. Please update Tangra3 to use AOTA.";
                                    return;
                                }
                                else if (requiredVersion < currentVersion)
                                {
                                    s_IncompatibleVersionOfOccultErrorMessage = "You need Occult version " + MIN_VERSION_OCCULT_REQUIRED + " or newer to use AOTA from Tangra. Please update Occult.";
                                    return;
                                } 
                            }
                        }

                        Trace.WriteLine(tlex.Message);
                    }

                    Type probledType = AssemblyOccultUtilities.GetType("AOTA.AOTA_ExternalAccess");

                    if (probledType != null && probledType.GetInterfaces().Contains(typeof (IAOTAExternalAccess)))
                    {
                        TYPE_AOTA_ExternalAccess = probledType;
                        s_IncompatibleVersionOfOccultErrorMessage = null;
                    }
                    else
                        s_IncompatibleVersionOfOccultErrorMessage = "You need Occult version " + MIN_VERSION_OCCULT_REQUIRED + " or newer to use AOTA from Tangra. Please update Occult.";

                }
            }

            internal static AotaReturnValue GetAOTAResult()
            {
                AotaReturnValue result = null;
                ShieldedCall(() => result = ReadAOTAResult());
                ShieldedCall(() => result.IsMiss = m_AotaInstance.IsMiss);
                ShieldedCall(() => result.AreResultsAvailable = m_AotaInstance.ResultsAreAvailable);
                ShieldedCall(() => result.AOTAVersion = m_AotaInstance.AOTA_Version);

                return result;
            }


            internal static bool RunAOTA(ILightCurveDataProvider dataProvider, IWin32Window parentWindow, IAOTAClientCallbacks clientCallbacks)
            {
                try
                {
                    if (m_AotaInstance == null)
                        m_AotaInstance = Activator.CreateInstance(TYPE_AOTA_ExternalAccess) as Occult.SDK.IAOTAExternalAccess;

                    ShieldedCall(() => m_AotaInstance.InitialiseAOTA(dataProvider.FileName, clientCallbacks));

                    ISingleMeasurement[] measurements = dataProvider.GetTargetMeasurements();

                    bool hasReliableTimeBase = dataProvider.HasReliableTimeBase;

                    float[] data = measurements.Select(x => x.Measurement).ToArray();
                    float[] frameIds = measurements.Select(x => (float)x.CurrFrameNo).ToArray();

                    float[] dataBg = measurements.Select(x => x.Background).ToArray();
                    ShieldedCall(() => m_AotaInstance.Set_TargetData_BackgroundAlreadySubtracted(data, dataBg));

                    ShieldedCall(() => m_AotaInstance.Set_FrameID(frameIds));

                    DateTime[] timestamps = measurements.Select(x => x.Timestamp).ToArray();

                    hasReliableTimeBase = hasReliableTimeBase &&
                        timestamps[0].Date != DateTime.MinValue &&
                        timestamps[measurements.Length - 1].Date != DateTime.MinValue &&
                        timestamps[0].Date.Ticks < timestamps[measurements.Length - 1].Ticks;

                    double[] secondsFromUTMidnight;
                    long startFrameStartDayTicks;

                    startFrameStartDayTicks = hasReliableTimeBase ? timestamps[0].Date.Ticks : timestamps.FirstOrDefault(x => x != DateTime.MinValue).Date.Ticks;
                    secondsFromUTMidnight = new double[timestamps.Length];
                    for (int i = 0; i < timestamps.Length; i++)
                    {
                        if (timestamps[i] != DateTime.MinValue)
                            secondsFromUTMidnight[i] = Math.Truncate(new TimeSpan(timestamps[i].Ticks - startFrameStartDayTicks).TotalSeconds * 10000) / 10000.0;
                        else
                            secondsFromUTMidnight[i] = 0;
                    }

                    bool cameraCorrectionsHaveBeenApplied = dataProvider.CameraCorrectionsHaveBeenApplied;

                    ShieldedCall(() => m_AotaInstance.Set_TimeBase(secondsFromUTMidnight, cameraCorrectionsHaveBeenApplied));

                    if (cameraCorrectionsHaveBeenApplied)
                    {
                        string cameraName = dataProvider.VideoCameraName;
                        if (!string.IsNullOrEmpty(cameraName))
                            ShieldedCall(() => m_AotaInstance.Set_VideoCamera(cameraName));
                    }

                    // Now go and set any comparison stars
                    for (int i = 0; i < dataProvider.NumberOfMeasuredComparisonObjects; i++)
                    {
                        ISingleMeasurement[] compMeasurements = dataProvider.GetComparisonObjectMeasurements(i);

                        if (compMeasurements != null)
                        {
                            float[] compData = compMeasurements.Select(x => x.Measurement).ToArray();

                            if (i == 0)
                                ShieldedCall(() => m_AotaInstance.Set_Comp1Data(compData));
                            else if (i == 1)
                                ShieldedCall(() => m_AotaInstance.Set_Comp2Data(compData));
                            else if (i == 2)
                                ShieldedCall(() => m_AotaInstance.Set_Comp3Data(compData));
                        }
                    }

                     ShieldedCall(() => m_AotaInstance.Camera = new Occult.SDK.Camera()
                        {
                            CameraType = dataProvider.VideoCameraName,
                            VideoSystem = dataProvider.VideoSystem,
                            FramesIntegrated = dataProvider.NumberIntegratedFrames
                        });

                    int firstFrameIndex = 0;// (int)frameIds[0];
                    int framesInIntegration = 1;

                    ShieldedCall(() => m_AotaInstance.RunAOTA(null, firstFrameIndex, framesInIntegration, false));

                    return true;
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.ToString());
                    MessageBox.Show(ex is TargetInvocationException ? ex.InnerException.Message : ex.Message, "AOTA Add-in Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                return false;
            }

            private static void ShieldedCall(Action methodcall)
            {
                try
                {
                    methodcall();
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
                    m_AotaInstance.CloseAOTA();
                    m_AotaInstance = null;
                }
            }

            internal static void NotifyAOTAOfCurrentFrameChanged(int currFrameId)
            {
                if (m_AotaInstance != null)
                {
                    m_AotaInstance.Set_AOTA_DisplayLocation(currFrameId);
                }
            }

            internal static AotaReturnValue ReadAOTAResult()
            {
                var rv = new AotaReturnValue();

                if (m_AotaInstance != null)
                {
                    object cameraObj = m_AotaInstance.Camera;
                    object event1Obj = m_AotaInstance.ResultsForEvent1;
                    object event2Obj = m_AotaInstance.ResultsForEvent2;
                    object event3Obj = m_AotaInstance.ResultsForEvent3;
                    object event4Obj = m_AotaInstance.ResultsForEvent4;
                    object event5Obj = m_AotaInstance.ResultsForEvent5;

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
