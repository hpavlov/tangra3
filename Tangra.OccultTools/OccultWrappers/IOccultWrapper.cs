/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.SDK;

namespace Tangra.OccultTools.OccultWrappers
{
    public struct EventResults
    {
        public bool IsNonEvent;
        public float D_Frame;
        public float R_Frame;
        public float D_FrameUncertPlus;
        public float D_FrameUncertMinus;
        public float R_FrameUncertPlus;
        public float R_FrameUncertMinus;
        public string D_UTC;
        public string R_UTC;
        public float D_DurationFrames;
        public float R_DurationFrames;
    }

    public struct Camera
    {
        public string CameraType;
        public string MeasuringTool;
        public string VideoSystem;
        public int FramesIntegrated;
        public int MeasurementsBinned;
        public bool MeasuredAtFieldLevel;
        public bool TimeScaleFromMeasuringTool;
    }

    public class AotaReturnValue
    {
        public bool IsMiss;
        public bool AreResultsAvailable;
        public string AOTAVersion;
        public Camera CameraResult;
        public EventResults[] EventResults;
    }

    interface IOccultWrapper
    {
        string HasSupportedVersionOfOccult(string occultLocation);
        string GetOccultCurrentOccultVersion(string occultLocation);
        bool RunAOTA(ILightCurveDataProvider dataProvider, IWin32Window parentWindow);
        AotaReturnValue GetAOTAResult();
        void EnsureAOTAClosed();
        void NotifyAOTAOfCurrentFrameChanged(int currFrameId);
    }
}
