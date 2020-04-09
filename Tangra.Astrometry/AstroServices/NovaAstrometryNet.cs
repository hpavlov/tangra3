using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Tangra.AstroServices
{
    public class NovaAstrometryNet
    {
        public void Submission()
        {
	        string APIKey = "iefygbxqgjvobdfs";
            string DATA = @"250,271
417,3
234,288
302,142
1,165
150,242
103,50
352,321
34,72
311,108
101,370
434,437
276,6
484,370
445,219
325,329
431,378
594,447
561,222
506,462
440,193
44,183
491,285
251,63
428,340
538,128
213,69
466,195
596,353
175,34
573,27
255,89
580,391
223,464
639,14
562,71
606,156
570,194
436,132
557,112";

	        var sessionId = Login(APIKey);
	        var subid = SubmitJob(sessionId, DATA);
            var jobId = GetSubmissionStatus(subid);
            var jobStatus = GetJobStatus(jobId);
        }

        private string GetJobStatus(string jobId)
        {
            var httpResponse = GetForm("http://nova.astrometry.net/api/jobs/" + jobId);

            try
            {
                if (httpResponse != null)
                {
                    //Newtonsoft.Json.Linq.JObject jObject = Newtonsoft.Json.Linq.JObject.Parse(httpResponse);

                    //return jObject["status"].ToString();
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }

            return null;

        }

        private string GetSubmissionStatus(string subid)
        {
            var httpResponse = GetForm("http://nova.astrometry.net/api/submissions/" + subid);

            try
            {
                if (httpResponse != null)
                {
                    //Newtonsoft.Json.Linq.JObject jObject = Newtonsoft.Json.Linq.JObject.Parse(httpResponse);

                    //// {"user": 18618, "processing_started": "2020-04-08 19:36:49.909422", "processing_finished": "2020-04-08 19:36:50.347303", "user_images": [], "jobs": [], "job_calibrations": [], "error_message": "...ist\n    w = int(math.ceil(fits.x.max()))\n  File \"/usr/local/lib/python3.6/dist-packages/numpy/core/_methods.py\", line 30, in _amax\n    return umr_maximum(a, axis, None, out, keepdims, initial, where)\nTypeError: cannot perform reduce with flexible type\n"}
                    //// {"user": 18618, "processing_started": "2020-04-08 19:44:49.199513", "processing_finished": "2020-04-08 19:44:49.452463", "user_images": [3536123], "jobs": [4098695], "job_calibrations": []}
                    //var jobs = jObject["jobs"].ToString();
                    //var error = jObject["error_message"].ToString();
                    //if (error != null)
                    //{
                    //    return error;
                    //}

                    // TODO: Read the results
                    // http://astrometry.net/doc/net/api.html#submitting-a-url
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }

            return null;
        }

        private static string SubmitJob(string sessionKey, string starCoordList)
        {
            var query = new NovaQuery();
            query.session = sessionKey;
            query.center_ra = (float)(15 * (14 + 44 / 60.0 + 51 / 3600.0));
            query.center_dec = (float)(-1 * (20 + 51 / 60.0 + 52 / 3600.0));
            query.radius = 0.81f;
            query.image_width = 640;
            query.image_height = 528;
            query.positional_error = 1.0f; // (Should be based on pixel size)
            query.scale_lower = 0.5f;
            query.scale_upper = 2.0f;

            var jsonQueryString = string.Empty;
            //jsonQueryString = Newtonsoft.Json.JsonConvert.SerializeObject(query);


            string boundary = string.Format("============{0:N}", Guid.NewGuid());
            var request = (HttpWebRequest)HttpWebRequest.Create("http://nova.astrometry.net/api/upload");
            request.Method = "POST";
            request.ContentType = "multipart/form-data; boundary=" + boundary;
            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";

            string dataTemplate = @"--{0}
Content-Type: text/plain
MIME-Version: 1.0
Content-disposition: form-data; name=""request-json""

{1}
--{0}
Content-Type: application/octet-stream
MIME-Version: 1.0
Content-disposition: form-data; name=""file""; filename=""myfile.txt""

{2}
--{0}--";

            string postdata = string.Format(dataTemplate, boundary, jsonQueryString, starCoordList);

            try
            {
                ASCIIEncoding ascii = new ASCIIEncoding();
                byte[] postBytes = ascii.GetBytes(postdata);

                request.ContentLength = postBytes.Length;
                using (Stream postStream = request.GetRequestStream())
                {
                    postStream.Write(postBytes, 0, postBytes.Length);
                    postStream.Flush();
                }

                try
                {
                    using (var response = (HttpWebResponse)request.GetResponse())
                    {
                        using (var respStr = response.GetResponseStream())
                        {
                            var httpResponse = new StreamReader(respStr).ReadToEnd();

                            try
                            {
                                if (httpResponse != null)
                                {
                                    //Newtonsoft.Json.Linq.JObject jObject = Newtonsoft.Json.Linq.JObject.Parse(httpResponse);

                                    //var status = jObject["status"].ToString();
                                    //if (status == "success")
                                    //{
                                    //    return jObject["subid"].ToString();
                                    //}
                                }
                            }
                            catch (Exception ex)
                            {
                                Trace.WriteLine(ex);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }

            return null;
        }

        private string Login(string apiKey)
        {
            NameValueCollection formData = HttpUtility.ParseQueryString(String.Empty);
            formData.Add("request-json", string.Format("{{\"apikey\": \"{0}\"}}", apiKey));

            var httpResponse = PostForm("http://nova.astrometry.net/api/login", formData);

            try
            {
                if (httpResponse != null)
                {
                    //Newtonsoft.Json.Linq.JObject jObject = Newtonsoft.Json.Linq.JObject.Parse(httpResponse);

                    //var status = jObject["status"].ToString();
                    //if (status == "success")
                    //{
                    //    return jObject["session"].ToString();
                    //}
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }

            return null;
        }

        private string PostForm(string url, NameValueCollection formData)
        {
            var request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";

            string postdata = formData.ToString();

            ASCIIEncoding ascii = new ASCIIEncoding();
            byte[] postBytes = ascii.GetBytes(postdata);

            request.ContentLength = postBytes.Length;
            using (Stream postStream = request.GetRequestStream())
            {
                postStream.Write(postBytes, 0, postBytes.Length);
                postStream.Flush();
            }

            try
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    using (var respStr = response.GetResponseStream())
                    {
                        return new StreamReader(respStr).ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }

            return null;
        }

        private string GetForm(string url)
        {
            var request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "GET";
            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";

            try
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    using (var respStr = response.GetResponseStream())
                    {
                        return new StreamReader(respStr).ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }

            return null;
        }

        public class NovaQuery
        {
            public NovaQuery()
            {
                publicly_visible = "n";
                allow_modifications = "d";
                allow_commercial_use = "d";
                positional_error = 1;

                downsample_factor = 1;
                scale_units = "degwidth";
                scale_type = "ul";
            }

            // string, requried. Your session key, required in all requests
            public string session { get; set; }

            // string: “y”, “n”
            public string publicly_visible { get; set; }

            // string: “d” (default), “y”, “n”, “sa” (share-alike): licensing terms
            public string allow_modifications { get; set; }

            // string: “d” (default), “y”, “n”: licensing terms
            public string allow_commercial_use { get; set; }

            // int, only necessary if you are submitting an “x,y list” of source positions.
            public int image_width { get; set; }

            // int, ditto.
            public int image_height { get; set; }

            // float, expected error on the positions of stars in your image. Default is 1
            public float positional_error { get; set; }

            // float, 0 to 360, in degrees. The position of the center of the image.
            public float center_ra { get; set; }

            // float, -90 to 90, in degrees. The position of the center of the image.
            public float center_dec { get; set; }

            // float, in degrees. Used with center_ra,``center_dec`` to specify that you know roughly where your image is on the sky.
            public float radius { get; set; }

            // float, >1. Downsample (bin) your image by this factor before performing source detection. This often helps with saturated images, noisy images, and large images. 2 and 4 are commonly-useful values.
            public float downsample_factor { get; set; }

            // string: “degwidth” (default), “arcminwidth”, “arcsecperpix”. The units for the “scale_lower” and “scale_upper” arguments; becomes the “–scale-units” argument to “solve-field” on the server side.
            public string scale_units { get; set; }

            // string, “ul” (default) or “ev”. Set “ul” if you are going to provide “scale_lower” and “scale_upper” arguments, or “ev” if you are going to provide “scale_est” (estimate) and “scale_err” (error percentage) arguments.
            public string scale_type { get; set; }

            // float. The lower-bound of the scale of the image.
            public float scale_lower { get; set; }

            // float. The upper-bound of the scale of the image.
            public float scale_upper { get; set; }
        }
    }
}
