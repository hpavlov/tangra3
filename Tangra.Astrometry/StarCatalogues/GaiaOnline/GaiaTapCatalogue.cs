using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Xml;
using Tangra.Model.Config;
using Tangra.StarCatalogues.UCAC4;

namespace Tangra.StarCatalogues.GaiaOnline
{
    public class GaiaTapCatalogue
    {
        private static CatalogMagnitudeBand JohnsonVFromMagnitudeBand = new CatalogMagnitudeBand(GaiaDR2Entry.BAND_ID_V, "Johnson V - Computed from G and BP-RP");
        private static CatalogMagnitudeBand CousinsRFromMagnitudeBand = new CatalogMagnitudeBand(GaiaDR2Entry.BAND_ID_R, "Cousins R - Computed from G and BP-RP");

        public static CatalogMagnitudeBand[] CatalogMagnitudeBands = new CatalogMagnitudeBand[] { CousinsRFromMagnitudeBand, JohnsonVFromMagnitudeBand };

        public static double ConvertMagnitude(double measuredMag, double vrColorIndex, Guid catalogMagBand, TangraConfig.MagOutputBand magOutputBand)
        {
            // https://gea.esac.esa.int/archive/documentation/GDR2/Data_processing/chap_cu5pho/sec_cu5pho_calibr/ssec_cu5pho_PhotTransf.html

            if (magOutputBand == TangraConfig.MagOutputBand.JohnsonV)
            {
                // G − V  = -0.01760 - 0.006860(BP-RP) - 0.1732(BP-RP)*(BP-RP);
            }

            if (magOutputBand == TangraConfig.MagOutputBand.CousinsR)
            {
                // G − R  = -0.003226 + 0.3833(BP-RP) - 0.1345(BP-RP)*(BP-RP);
            }

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

        public GaiaTapCatalogue(string authToken, string tapUrl = "https://gaia.aip.de/tap")
        {
            this.authToken = authToken;
            this.tapUrl = tapUrl.TrimEnd('/');
        }

        public List<IStar> GetStarsInRegion(double raDeg, double deDeg, double radiusDeg, double limitMag, float epoch)
        {
            GaiaDR2Entry.TargetEpoch = epoch;
            return GetGaiaTapData(raDeg, deDeg, radiusDeg, limitMag);
        }

        // REST 
        //  How to use TAP on the command line
        //  TAP can also be used with a HTTP command line client. Here we use HTTPie, but there are a lot of other similar clients (e.g. curl).

        //  # retrieve the job list
        //  http https://gaia.aip.de/tap/async

        //  # submit an asyncronous job (using PostgreSQL and the 5 minutes queue)
        //  http -f --follow POST https://gaia.aip.de/tap/async \
        //      QUERY="SELECT ra, dec FROM gdr2.gaia_source WHERE random_index < 100" \
        //      LANG="postgresql-9.6" QUEUE="5m" PHASE="RUN"

        //  # get all the information about a job
        //  http https://gaia.aip.de/tap/async/78d9c528-8cf0-46e3-8a5b-ec151229a30b

        //  # check the status of a job
        //  http https://gaia.aip.de/tap/async/78d9c528-8cf0-46e3-8a5b-ec151229a30b/phase

        //  # get the results of a job as csv or votable
        //  http https://gaia.aip.de/tap/async/78d9c528-8cf0-46e3-8a5b-ec151229a30b/results/csv
        //  http https://gaia.aip.de/tap/async/78d9c528-8cf0-46e3-8a5b-ec151229a30b/results/votable

        //  # archive the job (this deletes the database table and frees up space)
        //  http --follow DELETE https://gaia.aip.de/tap/async/78d9c528-8cf0-46e3-8a5b-ec151229a30b
        //  As with the Python interface, you can also use your personal token to authenticate with the system to use your personal account (and your personal joblist, quota etc.). To do so, you need to send the token as part of the Authorization header with every HTTP request.

        //  http https://gaia.aip.de/tap/async Authorization:"Token c1a67d3db8b1c93e55573aa1a8a2133a5e65c301"

        private List<IStar> GetGaiaTapData(double raDeg, double deDeg, double radiusDeg, double limitMag, int limitStars = 2000)
        {
            var rv = new List<IStar>();

            var request = (HttpWebRequest)HttpWebRequest.Create(tapUrl + "/sync");
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Headers.Add(HttpRequestHeader.Authorization, "Token " + authToken);
            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";

            NameValueCollection outgoingQueryString = HttpUtility.ParseQueryString(String.Empty);

            var QUERY = @"SELECT solution_id, designation, source_id, ref_epoch, ra, ra_error, dec, dec_error, parallax, parallax_error, pmra, pmra_error, pmdec, pmdec_error, phot_g_mean_mag, bp_rp, phot_variable_flag
            FROM gdr2.gaia_source
            WHERE pos @ scircle(spoint(RADIANS({0}), RADIANS({1})), RADIANS({2})) AND(phot_g_mean_mag <= {3}) LIMIT {4}";
            outgoingQueryString.Add("QUERY", string.Format(QUERY, raDeg, deDeg, radiusDeg, limitMag, limitStars));
            outgoingQueryString.Add("LANG", "postgresql-9.6");

            string postdata = outgoingQueryString.ToString();

            ASCIIEncoding ascii = new ASCIIEncoding();
            byte[] postBytes = ascii.GetBytes(postdata);

            request.ContentLength = postBytes.Length;

            using (Stream postStream = request.GetRequestStream())
            {
                postStream.Write(postBytes, 0, postBytes.Length);
                postStream.Flush();
                postStream.Close();
            }

            using (var response = (HttpWebResponse) request.GetResponse())
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (var responseStr = response.GetResponseStream())
                    {
                        if (responseStr != null)
                        {
                            var responseString = new StreamReader(responseStr).ReadToEnd();
                            var xmlDoc = new XmlDocument();
                            xmlDoc.LoadXml(responseString);

                            var nodes = xmlDoc.SelectNodes("//*[name() ='TABLEDATA']/*[name() = 'TR']");
                            if (nodes != null)
                            {
                                foreach (XmlNode node in nodes)
                                {
                                    rv.Add(new GaiaDR2Entry(node));
                                }
                            }
                        }
                    }
                }
            }

            return rv;
        }
    }
}
