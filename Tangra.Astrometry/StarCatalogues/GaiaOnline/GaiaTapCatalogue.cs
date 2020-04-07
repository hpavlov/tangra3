using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Windows.Forms;
using System.Xml;
using Tangra.Model.Config;
using Tangra.Model.Helpers;
using Tangra.StarCatalogues.UCAC4;

namespace Tangra.StarCatalogues.GaiaOnline
{
    public class GaiaTapCatalogue
    {
        public class WindowWrapper : IWin32Window
        {
            public WindowWrapper(IntPtr handle)
            {
                m_Hwnd = handle;
            }

            public IntPtr Handle
            {
                get { return m_Hwnd; }
            }

            private IntPtr m_Hwnd;
        }

        private static CatalogMagnitudeBand JohnsonVFromMagnitudeBand = new CatalogMagnitudeBand(GaiaDR2Entry.BAND_ID_V, "Johnson V - Computed from G and BP-RP");
        private static CatalogMagnitudeBand CousinsRFromMagnitudeBand = new CatalogMagnitudeBand(GaiaDR2Entry.BAND_ID_R, "Cousins R - Computed from G and BP-RP");

        public static CatalogMagnitudeBand[] CatalogMagnitudeBands = new CatalogMagnitudeBand[] { CousinsRFromMagnitudeBand, JohnsonVFromMagnitudeBand };

        public static double ConvertMagnitude(double measuredMag, double vrColorIndex, Guid catalogMagBand, TangraConfig.MagOutputBand magOutputBand)
        {
            // https://gea.esac.esa.int/archive/documentation/GDR2/Data_processing/chap_cu5pho/sec_cu5pho_calibr/ssec_cu5pho_PhotTransf.html

            if (catalogMagBand == GaiaDR2Entry.BAND_ID_R && magOutputBand == TangraConfig.MagOutputBand.CousinsR) return measuredMag;
            if (catalogMagBand == GaiaDR2Entry.BAND_ID_R && magOutputBand == TangraConfig.MagOutputBand.JohnsonV) return ColourIndexTables.GetVFromRAndVR(measuredMag, vrColorIndex);

            if (catalogMagBand == GaiaDR2Entry.BAND_ID_V && magOutputBand == TangraConfig.MagOutputBand.CousinsR) return ColourIndexTables.GetRFromVAndVR(measuredMag, vrColorIndex);
            if (catalogMagBand == GaiaDR2Entry.BAND_ID_V && magOutputBand == TangraConfig.MagOutputBand.JohnsonV) return measuredMag;

            return double.NaN;
        }

        private static Regex s_ApiTokenRegex = new Regex(@"^[\da-f]{40}$", RegexOptions.Compiled);

        public static bool IsValidApiToken(ref string apiToken)
        {
            if (apiToken == null)
            {
                return false;
            }

            apiToken = apiToken.Trim();
            return s_ApiTokenRegex.IsMatch(apiToken);
        }

        private string authToken;
        private string tapUrl;

        private string dataDirectory = Path.GetFullPath(string.Format("{0}\\Tangra3\\GaiaDR2", Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)));

        public GaiaTapCatalogue(string authToken, string tapUrl = "https://gaia.aip.de/tap")
        {
            this.authToken = authToken;
            this.tapUrl = tapUrl.TrimEnd('/');

            if (!Directory.Exists(dataDirectory))
            {
                try
                {
                    Directory.CreateDirectory(dataDirectory);
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.GetFullStackTrace());
                }
            }
        }

        public List<IStar> GetStarsInRegion(double raDeg, double deDeg, double radiusDeg, double limitMag, float epoch, IWin32Window parentWindow)
        {
            GaiaDR2Entry.TargetEpoch = epoch;

            var requestKey = GetRequestKey(raDeg, deDeg, radiusDeg, limitMag);
            List<IStar> rv = LoadFromCache(requestKey);

            if (rv == null || rv.Count == 0)
            {
                var frm = new frmGaiaRestCaller(tapUrl, authToken, raDeg, deDeg, radiusDeg, limitMag);
                var res = frm.ShowDialog(parentWindow ?? new WindowWrapper(Process.GetCurrentProcess().MainWindowHandle));

                if (res != DialogResult.OK)
                {
                    rv = frm.DownloadedStars;
                    if (rv != null && rv.Any())
                    {
                        SaveToCache(requestKey, rv);
                    }
                }
            }

            return rv;
        }

        private static string INVALID_FILENAME_CHARS = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());

        private static Regex INVALID_FILENAME_CHARS_REGEX = new Regex(string.Format("[{0}]", Regex.Escape(INVALID_FILENAME_CHARS)));

        private string GetRequestKey(double raDeg, double deDeg, double radiusDeg, double limitMag)
        {
            var unescapedString = string.Format("{0} {1} {2:0.00} {3:0.0}", 
                AstroConvert.ToStringValue(raDeg / 15, "REC"),
                AstroConvert.ToStringValue(deDeg, "DEC"),
                radiusDeg, limitMag);

            return INVALID_FILENAME_CHARS_REGEX.Replace(unescapedString, "");
        }

        private static string CACHE_MAGIC = "GaiaDR2Cache";
        private static int CACHE_VERSION = 1;


        private List<IStar> LoadFromCache(string requestKey)
        {
            List<IStar> rv = null;

            try
            {
                string dataFileName = Path.GetFullPath(dataDirectory + "\\" + requestKey);
                if (File.Exists(dataFileName))
                {
                    rv = new List<IStar>();

                    using (var fs = new FileStream(dataFileName, FileMode.Open, FileAccess.Read))
                    using (var br = new BinaryReader(fs, Encoding.ASCII))
                    {
                        try
                        {
                            var magic = br.ReadString();
                            var version = br.ReadInt32();
                            var noStars = br.ReadInt32();

                            if (magic == CACHE_MAGIC)
                            {
                                if (version >= 1)
                                {
                                    for (int i = 0; i < noStars; i++)
                                    {
                                        var star = new GaiaDR2Entry(br);
                                        rv.Add(star);
                                    }
                                }
                            }
                        }
                        catch { }
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }

            return rv;
        }

        private void SaveToCache(string requestKey, List<IStar> stars)
        {
            try
            {
                string dataFileName = Path.GetFullPath(dataDirectory + "\\" + requestKey);

                using (var fs = new FileStream(dataFileName, FileMode.Create, FileAccess.Write))
                using (var bw = new BinaryWriter(fs, Encoding.ASCII))
                {
                    bw.Write(CACHE_MAGIC);
                    bw.Write(CACHE_VERSION);
                    bw.Write(stars.Count);

                    foreach (var star in stars)
                    {
                        try
                        {
                            var gdr2Star = (GaiaDR2Entry)star;
                            gdr2Star.Serialize(bw);
                        }
                        catch { }
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }
        }
    }
}
