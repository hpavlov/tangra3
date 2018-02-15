/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Tangra.Config
{
    public struct DelayRequest
    {
        /// <summary>
        /// X position from 0 to 1 (frame width adjusted)
        /// </summary>
        public float XPosRatio;

        /// <summary>
        /// Y position from 0 to 1 (frame height adjusted)
        /// </summary>
        public float YPosRatio;
    }

	public static class InstrumentalDelayConfigManager
	{
        public const string RUNCAM = "RunCam";

		private static List<string> s_SupportedCameras;
        private static Dictionary<string, InstrumentalDelayConfiguration> s_SupportedCamerasConfig;
        private static List<string> s_SupportedCamerasForAstrometry;

		static InstrumentalDelayConfigManager()
		{
			s_SupportedCameras = new List<string>();
		    s_SupportedCamerasForAstrometry = new List<string>();
            s_SupportedCamerasConfig = new Dictionary<string, InstrumentalDelayConfiguration>();

			Add_WAT910HX_PAL_InstrumentalDelayConfig();
			Add_WAT910HX_NTSC_InstrumentalDelayConfig();
			Add_WAT120N_PAL_InstrumentalDelayConfig();
			Add_WAT120N_NTSC_InstrumentalDelayConfig();
			Add_Mintron_PAL_InstrumentalDelayConfig();
			Add_Mintron_NTSC_InstrumentalDelayConfig();
			Add_PC165DNR_NTSC_InstrumentalDelayConfig();
			Add_SCB2000N_NTSC_InstrumentalDelayConfig();
		    Add_RunCam_PAL_InstrumentalDelayConfig();
		    Add_RunCam_NTSC_InstrumentalDelayConfig();
		    Add_NonIntegratingCameras_InstrumentalDelayConfig();

			s_SupportedCameras.Sort();
		}

		private static void Add_WAT910HX_PAL_InstrumentalDelayConfig()
		{
            var delays = new Dictionary<int, float>();

			const string MODEL1 = "WAT-910HX (PAL)";
			const string MODEL2 = "WAT-910BD (PAL)";
			s_SupportedCameras.Add(MODEL1);
			s_SupportedCameras.Add(MODEL2);
            s_SupportedCamerasForAstrometry.Add(MODEL1);
            s_SupportedCamerasForAstrometry.Add(MODEL2);

			delays.Add(1, -0.040f);
            delays.Add(2, -0.060f);
            delays.Add(4, -0.100f);
            delays.Add(8, -0.180f);
            delays.Add(16, -0.340f);
            delays.Add(32, -0.660f);
            delays.Add(64, -1.300f);
            delays.Add(127, -2.560f);

            s_SupportedCamerasConfig.Add(MODEL1, new FixedDelayInstrumentalDelayConfiguration(delays));
            s_SupportedCamerasConfig.Add(MODEL2, new FixedDelayInstrumentalDelayConfiguration(delays));
		}

		private static void Add_WAT910HX_NTSC_InstrumentalDelayConfig()
		{
            var delays = new Dictionary<int, float>();

			const string MODEL1 = "WAT-910HX (NTSC)";
			const string MODEL2 = "WAT-910BD (NTSC)";
			s_SupportedCameras.Add(MODEL1);
			s_SupportedCameras.Add(MODEL2);
            s_SupportedCamerasForAstrometry.Add(MODEL1);
            s_SupportedCamerasForAstrometry.Add(MODEL2);

            delays.Add(1, -0.033f);
            delays.Add(2, -0.050f);
            delays.Add(4, -0.083f);
            delays.Add(8, -0.150f);
            delays.Add(16, -0.284f);
            delays.Add(32, -0.551f);
            delays.Add(64, -1.084f);
            delays.Add(127, -2.135f);

            s_SupportedCamerasConfig.Add(MODEL1, new FixedDelayInstrumentalDelayConfiguration(delays));
            s_SupportedCamerasConfig.Add(MODEL2, new FixedDelayInstrumentalDelayConfiguration(delays));
		}

		private static void Add_WAT120N_PAL_InstrumentalDelayConfig()
		{
            var delays = new Dictionary<int, float>();

			const string MODEL1 = "WAT-120N (PAL)";
			const string MODEL2 = "WAT-120N+ (PAL)";
			s_SupportedCameras.Add(MODEL1);
			s_SupportedCameras.Add(MODEL2);
            s_SupportedCamerasForAstrometry.Add(MODEL1);
            s_SupportedCamerasForAstrometry.Add(MODEL2);

            delays.Add(1, -0.050f);
            delays.Add(2, -0.070f);
            delays.Add(4, -0.110f);
            delays.Add(8, -0.190f);
            delays.Add(16, -0.350f);
            delays.Add(32, -0.670f);
            delays.Add(64, -1.310f);
            delays.Add(128, -2.590f);
            delays.Add(256, -5.150f);

            s_SupportedCamerasConfig.Add(MODEL1, new FixedDelayInstrumentalDelayConfiguration(delays));
            s_SupportedCamerasConfig.Add(MODEL2, new FixedDelayInstrumentalDelayConfiguration(delays));
		}

		private static void Add_WAT120N_NTSC_InstrumentalDelayConfig()
		{
            var delays = new Dictionary<int, float>();

			const string MODEL1 = "WAT-120N (NTSC)";
			const string MODEL2 = "WAT-120N+ (NTSC)";
			s_SupportedCameras.Add(MODEL1);
			s_SupportedCameras.Add(MODEL2);
            s_SupportedCamerasForAstrometry.Add(MODEL1);
            s_SupportedCamerasForAstrometry.Add(MODEL2);

            delays.Add(1, -0.042f);
            delays.Add(2, -0.058f);
            delays.Add(4, -0.092f);
            delays.Add(8, -0.159f);
            delays.Add(16, -0.292f);
            delays.Add(32, -0.559f);
            delays.Add(64, -1.093f);
            delays.Add(128, -2.161f);
            delays.Add(256, -4.296f);

            s_SupportedCamerasConfig.Add(MODEL1, new FixedDelayInstrumentalDelayConfiguration(delays));
            s_SupportedCamerasConfig.Add(MODEL2, new FixedDelayInstrumentalDelayConfiguration(delays));
		}

		private static void Add_Mintron_PAL_InstrumentalDelayConfig()
		{
			const string MODEL = "Mintron 12V1C-EX (PAL)";
			s_SupportedCameras.Add(MODEL);
            s_SupportedCamerasForAstrometry.Add(MODEL);
            var delays = new Dictionary<int, float>();

            delays.Add(1, -0.020f);
            delays.Add(2, -0.040f);
            delays.Add(3, -0.060f);
            delays.Add(4, -0.080f);
            delays.Add(6, -0.100f);
            delays.Add(8, -0.160f);
            delays.Add(12, -0.240f);
            delays.Add(16, -0.320f);
            delays.Add(24, -0.480f);
            delays.Add(32, -0.640f);
            delays.Add(48, -0.940f);
            delays.Add(64, -1.280f);

            s_SupportedCamerasConfig.Add(MODEL, new FixedDelayInstrumentalDelayConfiguration(delays));
		}

		private static void Add_Mintron_NTSC_InstrumentalDelayConfig()
		{
			const string MODEL = "Mintron 12V1C-EX (NTSC)";
			s_SupportedCameras.Add(MODEL);
            s_SupportedCamerasForAstrometry.Add(MODEL);
            var delays = new Dictionary<int, float>();            

            delays.Add(1, -0.017f);
            delays.Add(2, -0.033f);
            delays.Add(3, -0.050f);
            delays.Add(4, -0.067f);
            delays.Add(6, -0.083f);
            delays.Add(8, -0.134f);
            delays.Add(12, -0.200f);
            delays.Add(16, -0.267f);
            delays.Add(24, -0.400f);
            delays.Add(32, -0.534f);
            delays.Add(48, -0.784f);
            delays.Add(64, -1.068f);

            s_SupportedCamerasConfig.Add(MODEL, new FixedDelayInstrumentalDelayConfiguration(delays));
		}


		private static void Add_PC165DNR_NTSC_InstrumentalDelayConfig()
		{
			const string MODEL = "PC165DNR (NTSC)";
			s_SupportedCameras.Add(MODEL);
            s_SupportedCamerasForAstrometry.Add(MODEL);
            var delays = new Dictionary<int, float>();

            delays.Add(1, -0.033f);
            delays.Add(2, -0.050f);
            delays.Add(4, -0.083f);
            delays.Add(8, -0.150f);
            delays.Add(16, -0.284f);
            delays.Add(32, -0.551f);
            delays.Add(64, -1.084f);
            delays.Add(128, -2.152f);

            s_SupportedCamerasConfig.Add(MODEL, new FixedDelayInstrumentalDelayConfiguration(delays));
		}

		private static void Add_SCB2000N_NTSC_InstrumentalDelayConfig()
		{
			const string MODEL = "SCB-2000N (NTSC)";
			s_SupportedCameras.Add(MODEL);
            s_SupportedCamerasForAstrometry.Add(MODEL);
            var delays = new Dictionary<int, float>();            

            delays.Add(1, -0.050f);
            delays.Add(2, -0.067f);
            delays.Add(3, -0.083f);
            delays.Add(4, -0.100f);
            delays.Add(5, -0.117f);
            delays.Add(6, -0.134f);
            delays.Add(7, -0.150f);
            delays.Add(8, -0.167f);
            delays.Add(12, -0.234f);
            delays.Add(16, -0.300f);
            delays.Add(32, -0.567f);
            delays.Add(64, -1.101f);
            delays.Add(128, -2.169f);
            delays.Add(256, -4.304f);

            s_SupportedCamerasConfig.Add(MODEL, new FixedDelayInstrumentalDelayConfiguration(delays));
		}

        private static void Add_RunCam_NTSC_InstrumentalDelayConfig()
        {
            const string MODEL = RUNCAM + " Astro (NTSC)";
            s_SupportedCameras.Add(MODEL);
            s_SupportedCamerasForAstrometry.Add(MODEL);
            var delays = new Dictionary<int, Tuple<float, float>>();

            delays.Add(1, Tuple.Create(-0.066744f * 480, 69.03f));
            delays.Add(2, Tuple.Create(-0.067282f * 480, 102.68f));
            delays.Add(4, Tuple.Create(-0.066563f * 480, 169.24f));

            s_SupportedCamerasConfig.Add(MODEL, new ImageOffsetInstrumentalDelayConfiguration(delays));
        }

        private static void Add_RunCam_PAL_InstrumentalDelayConfig()
        {
            const string MODEL = RUNCAM + " Astro (PAL)";
            s_SupportedCameras.Add(MODEL);
            s_SupportedCamerasForAstrometry.Add(MODEL);
            var delays = new Dictionary<int, Tuple<float, float>>();

            delays.Add(1, Tuple.Create(-0.063143f * 576, 74.24f));
            delays.Add(2, Tuple.Create(-0.062652f * 576, 114.12f));
            delays.Add(4, Tuple.Create(-0.062463f * 576, 194.94f));

            s_SupportedCamerasConfig.Add(MODEL, new ImageOffsetInstrumentalDelayConfiguration(delays));
        }

	    private static void Add_NonIntegratingCameras_InstrumentalDelayConfig()
	    {
            string MODEL = "WAT-902H2 (PAL or NTSC)";
	        s_SupportedCameras.Add(MODEL);
            s_SupportedCamerasForAstrometry.Add(MODEL);
            var delays = new Dictionary<int, float>();

            delays.Add(1, 0.0f);

            s_SupportedCamerasConfig.Add(MODEL, new FixedDelayInstrumentalDelayConfiguration(delays));

            ////////////////////////////////
            MODEL = "PC164C-EX2 (EIA)";
	        s_SupportedCameras.Add(MODEL);
            s_SupportedCamerasForAstrometry.Add(MODEL);
            delays = new Dictionary<int, float>();

            delays.Add(1, 0.0f);

            s_SupportedCamerasConfig.Add(MODEL, new FixedDelayInstrumentalDelayConfiguration(delays));


            ////////////////////////////////
            MODEL = "SK-1004XC/SO CCIR";
	        s_SupportedCameras.Add(MODEL);
            s_SupportedCamerasForAstrometry.Add(MODEL);
            delays = new Dictionary<int, float>();

            delays.Add(1, 0.0f);

            s_SupportedCamerasConfig.Add(MODEL, new FixedDelayInstrumentalDelayConfiguration(delays));
	    }

	    public static List<string> GetAvailableCameras()
		{
			return s_SupportedCameras;
		}

        public static InstrumentalDelayConfiguration GetConfigurationForCamera(string cameraModel)
		{
            InstrumentalDelayConfiguration delaysConfig;
            if (s_SupportedCamerasConfig.TryGetValue(cameraModel, out delaysConfig))
            {
                return delaysConfig;
            }

            return new NotSupportedInstrumentalDelayConfiguration();
		}

        public static InstrumentalDelayConfiguration GetConfigurationForCameraForAstrometry(string cameraModel)
        {
            InstrumentalDelayConfiguration delaysConfig;
            if (s_SupportedCamerasForAstrometry.Contains(cameraModel) && 
                s_SupportedCamerasConfig.TryGetValue(cameraModel, out delaysConfig))
            {
                return delaysConfig;
            }

            return new NotSupportedInstrumentalDelayConfiguration();
        }
    }

}
