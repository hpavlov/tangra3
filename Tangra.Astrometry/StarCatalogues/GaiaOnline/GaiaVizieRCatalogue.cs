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

namespace Tangra.StarCatalogues.GaiaOnline
{
    public class GaiaVizieRCatalogue
    {
        public List<IStar> GetStarsInRegion(double raDeg, double deDeg, double radiusDeg, double limitMag, float epoch)
        {
            // TODO: Maintain local cache for all requests with the same coordinates (+/- 1 arcmin center) and the same FOV (+/- 1 arcmin)
            // TODO: Make multiple requests in magnitude ranges trying to get
            throw new NotSupportedException();
        }

        private void WebRequest()
        {
            var COORDS = "18 10 3.5 -32 57 10";
            var RADIUS_ARCMIN = "30";
            var GMAG_MIN = "-30";
            var GMAG_MAX = "12";

            var cc = new CookieContainer();

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("http://vizier.u-strasbg.fr/viz-bin/VizieR-3?-source=I/345");
            request.Method = "GET";
            request.CookieContainer = cc;
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9";
            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
            var response = (HttpWebResponse)request.GetResponse();
            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

            string REF = "VIZ5e4b086d0e7c";
            string TO = "4";
            string FROM = "-4";
            string THIS = "-4";

            var rexREF = new Regex("<INPUT TYPE=\"hidden\" NAME=\"-ref\" VALUE=\"([^\"]+)\">");
            var rexTO = new Regex("<INPUT TYPE=\"hidden\" NAME=\"-to\" VALUE=\"([^\"]+)\">");
            var rexFROM = new Regex("<INPUT TYPE=\"hidden\" NAME=\"-from\" VALUE=\"([^\"]+)\">");
            var rexTHIS = new Regex("<INPUT TYPE=\"hidden\" NAME=\"-this\" VALUE=\"([^\"]+)\">");

            var match = rexREF.Match(responseString);
            if (match.Success)
            {
                REF = match.Groups[1].Value;
            }

            match = rexTO.Match(responseString);
            if (match.Success)
            {
                TO = match.Groups[1].Value;
            }

            match = rexFROM.Match(responseString);
            if (match.Success)
            {
                FROM = match.Groups[1].Value;
            }

            match = rexTHIS.Match(responseString);
            if (match.Success)
            {
                THIS = match.Groups[1].Value;
            }
            request = (HttpWebRequest)HttpWebRequest.Create("http://vizier.u-strasbg.fr/viz-bin/asu-tsv");
            request.Method = "POST";
            request.CookieContainer = cc;
            request.ContentType = "application/x-www-form-urlencoded";
            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";

            NameValueCollection queryString = HttpUtility.ParseQueryString(String.Empty);
            queryString.Add("-ref", REF);
            queryString.Add("-to", TO);
            queryString.Add("-from", FROM);
            queryString.Add("-this", THIS);
            queryString.Add("//source", "I/345");
            queryString.Add("//tables", "I/345/gaia2");
            queryString.Add("-out.max", "unlimited");
            queryString.Add("//CDSportal", "http://cdsportal.u-strasbg.fr/StoreVizierData.html");
            queryString.Add("-out.form", "| -Separated-Values");
            queryString.Add("//outaddvalue", "default");
            queryString.Add("-order", "I");
            queryString.Add("-oc.form", "dec");
            queryString.Add("-c", COORDS);
            queryString.Add("-c.eq", "J2000");
            queryString.Add("-c.r", RADIUS_ARCMIN);
            queryString.Add("-c.u", "arcmin");
            queryString.Add("-c.geom", "b");
            queryString.Add("-source", "");
            queryString.Add("-source", "I/345/gaia2");
            queryString.Add("-out.orig", "standard");
            queryString.Add("-out", "RA_ICRS");
            queryString.Add("-out", "e_RA_ICRS");
            queryString.Add("-out", "DE_ICRS4");
            queryString.Add("-out", "e_DE_ICRS");
            queryString.Add("-out", "Source");
            queryString.Add("-out", "Epoch");
            queryString.Add("-out", "Plx");
            queryString.Add("-out", "e_Plx");
            queryString.Add("-out", "pmRA");
            queryString.Add("-out", "e_pmRA");
            queryString.Add("-out", "pmDE");
            queryString.Add("-out", "e_pmDE");
            queryString.Add("-out", "Dup");
            queryString.Add("-out", "Gmag");
            queryString.Add("-out", "e_Gmag");
            queryString.Add("-out", "BPmag");
            queryString.Add("-out", "e_BPmag");
            queryString.Add("-out", "RPmag");
            queryString.Add("-out", "e_RPmag");
            queryString.Add("Gmag", ">" + GMAG_MIN);
            queryString.Add("Gmag", "<" + GMAG_MAX);
            queryString.Add("-meta.ucd", "2");
            queryString.Add("-meta", "1");
            queryString.Add("-meta.foot", "1");
            queryString.Add("-usenav", "1");
            queryString.Add("-bmark", "POST");

            string postdata = queryString.ToString();

            ASCIIEncoding ascii = new ASCIIEncoding();
            byte[] postBytes = ascii.GetBytes(postdata);

            request.ContentLength = postBytes.Length;
            Stream postStream = request.GetRequestStream();
            postStream.Write(postBytes, 0, postBytes.Length);
            postStream.Flush();
            postStream.Close();

            response = (HttpWebResponse)request.GetResponse();

            responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
        }
    }
}
