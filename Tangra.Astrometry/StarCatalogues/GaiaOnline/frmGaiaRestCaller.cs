using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using System.Xml;
using Tangra.Model.Helpers;

namespace Tangra.StarCatalogues.GaiaOnline
{
    public partial class frmGaiaRestCaller : Form
    {
        public enum JobStaus
        {
            New,
            Queued,
            Completed,
            Retreived,
            Ready,
            Errored
        }

        public frmGaiaRestCaller()
        {
            InitializeComponent();
        }

        private string authToken;
        private string tapUrl;
        private double raDeg;
        private double deDeg;
        private double radiusDeg;
        private double limitMag;
        private int limitStars;

        private JobStaus currentState;

        public frmGaiaRestCaller(string tapUrl, string authToken, double raDeg, double deDeg, double radiusDeg, double limitMag, int limitStars = 2000)
            : this()
        {
            this.tapUrl = tapUrl;
            this.authToken = authToken;
            this.raDeg = raDeg;
            this.deDeg = deDeg;
            this.radiusDeg = radiusDeg;
            this.limitMag = limitMag;
            this.limitStars = limitStars;
        }

        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        public List<IStar> DownloadedStars;

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;

            if (this.IsDisposed)
            {
                return;
            }

            if (this.currentState == JobStaus.New)
            {
                Task.Run(() =>
                {
                    SubmitGaiaTapJob();
                }, 
                cancellationTokenSource.Token).IgnoreFaults();
            }
            else if (this.currentState == JobStaus.Queued)
            {
                Task.Run(() =>
                {
                    CheckGaiaTapJobStatus();
                },
                cancellationTokenSource.Token).IgnoreFaults();
            }
            else if (this.currentState == JobStaus.Completed)
            {
                Task.Run(() =>
                {
                    GetGaiaTapJobData();
                },
                cancellationTokenSource.Token).IgnoreFaults();
            }
            else if (this.currentState == JobStaus.Retreived)
            {
                Task.Run(() =>
                {
                    CleanUpGaiaTapResult();
                },
                cancellationTokenSource.Token).IgnoreFaults();
            }
            else if (this.currentState == JobStaus.Retreived)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                DialogResult = DialogResult.Cancel;
                Close();
            }
        }

        private void frmGaiaRestCaller_Load(object sender, EventArgs e)
        {
            lblJobInfo.Text = string.Format("({0}, {1}) {2:0.00} deg, {3:0.0} mag", AstroConvert.ToStringValue(raDeg / 15, "REC"), AstroConvert.ToStringValue(deDeg, "DEC"), radiusDeg, limitMag);

            this.currentState = JobStaus.New;
            UpdateStatus("Submitting new request ...");

            timer1.Enabled = true;
        }

        private string jobUrl;
        private string resultUrl;

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

        private void SubmitGaiaTapJob()
        {
            NameValueCollection outgoingQueryString = HttpUtility.ParseQueryString(String.Empty);

            var QUERY = @"SELECT solution_id, designation, source_id, ref_epoch, ra, ra_error, dec, dec_error, parallax, parallax_error, pmra, pmra_error, pmdec, pmdec_error, phot_g_mean_mag, bp_rp, phot_variable_flag
            FROM gdr2.gaia_source
            WHERE pos @ scircle(spoint(RADIANS({0}), RADIANS({1})), RADIANS({2})) AND(phot_g_mean_mag <= {3}) ORDER BY phot_g_mean_mag LIMIT {4}";
            outgoingQueryString.Add("QUERY", string.Format(QUERY, raDeg, deDeg, radiusDeg, limitMag, limitStars, CultureInfo.InvariantCulture));
            outgoingQueryString.Add("LANG", "postgresql-9.6");
            outgoingQueryString.Add("QUEUE", "5m");
            outgoingQueryString.Add("PHASE", "RUN");

            string postdata = outgoingQueryString.ToString();

            ASCIIEncoding ascii = new ASCIIEncoding();
            byte[] postBytes = ascii.GetBytes(postdata);

            var request = (HttpWebRequest)HttpWebRequest.Create(tapUrl + "/async");
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Headers.Add(HttpRequestHeader.Authorization, "Token " + authToken);
            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
            request.ContentLength = postBytes.Length;
            request.AllowAutoRedirect = false;

            using (Stream postStream = request.GetRequestStream())
            {
                postStream.Write(postBytes, 0, postBytes.Length);
                postStream.Flush();
                postStream.Close();
            }

            Trace.WriteLine(string.Format("Sending GaiaOnline request to {0}", tapUrl));

            WebRequestWithAbortOnError(request, () =>
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode == HttpStatusCode.SeeOther)
                    {
                        jobUrl = (string)response.Headers.GetType().GetProperty("Location", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(response.Headers);

                        Trace.WriteLine(string.Format("GaiaOnline job queued: {0}", jobUrl));

                        UpdateStatus("Request queued ...", JobStaus.Queued);
                    }
                }
            });
        }

        private void CheckGaiaTapJobStatus()
        {
            var request = (HttpWebRequest)HttpWebRequest.Create(jobUrl);
            request.Method = "GET";
            request.Headers.Add(HttpRequestHeader.Authorization, "Token " + authToken);
            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
            request.AllowAutoRedirect = false;

            WebRequestWithAbortOnError(request, () =>
            {
                using (var response = (HttpWebResponse)request.GetResponse())
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

                                string phase = "UNKNOWN";
                                TimeSpan queued = TimeSpan.Zero;
                                TimeSpan running = TimeSpan.Zero;

                                DateTime? created = null;
                                DateTime? started = null;
                                var node = xmlDoc.SelectSingleNode("//*[name() = 'uws:phase']");
                                if (node != null) phase = node.InnerText;

                                var nodeCreated = xmlDoc.SelectSingleNode("//*[name() = 'uws:creationTime']");
                                var nodeStarted = xmlDoc.SelectSingleNode("//*[name() = 'uws:startTime']");
                                try
                                {
                                    if (phase != "COMPLETED")
                                    {
                                        if (nodeCreated != null) created = DateTime.Parse(nodeCreated.InnerText).ToLocalTime();
                                        if (nodeStarted != null && !string.IsNullOrEmpty(nodeStarted.InnerText))
                                        {
                                            started = DateTime.Parse(nodeStarted.InnerText).ToLocalTime();
                                        }

                                        if (created.HasValue && started.HasValue)
                                        {
                                            queued = started.Value - created.Value;
                                            running = DateTime.Now - created.Value;
                                        }

                                        if (created.HasValue && !started.HasValue)
                                        {
                                            queued = DateTime.Now - created.Value;
                                        }
                                    }
                                }
                                catch { }

                                node = xmlDoc.SelectSingleNode("//*[name() = 'uws:result' and @id = 'votable']");
                                try
                                {
                                    if (node != null) resultUrl = node.Attributes["xlink:href"].Value;
                                }
                                catch { }

                                if (phase == "COMPLETED" && resultUrl != null)
                                {
                                    Trace.WriteLine(string.Format("GaiaOnline got result: {0}, job status: {1}", resultUrl, phase));

                                    UpdateStatus("Retreiving result...", JobStaus.Completed);
                                }
                                else
                                {
                                    Trace.WriteLine(string.Format("GaiaOnline job status: {0}", phase));

                                    Thread.Sleep(2000);

                                    if (running == TimeSpan.Zero && queued == TimeSpan.Zero)
                                    {
                                        UpdateStatus(String.Format("{0}", phase), JobStaus.Queued);
                                    }
                                    else if (running == TimeSpan.Zero && queued != TimeSpan.Zero)
                                    {
                                        UpdateStatus(String.Format("{0}, queued for {1:0} sec", phase, queued.TotalSeconds), JobStaus.Queued);
                                    }
                                    else if (running != TimeSpan.Zero && queued != TimeSpan.Zero)
                                    {
                                        UpdateStatus(String.Format("{0}, running for {1:0} sec", phase, running.TotalSeconds), JobStaus.Queued);
                                    }
                                }
                            }
                        }
                    }
                }
            });
        }

        private void GetGaiaTapJobData()
        {
            var request = (HttpWebRequest)HttpWebRequest.Create(resultUrl);
            request.Method = "GET";
            request.Headers.Add(HttpRequestHeader.Authorization, "Token " + authToken);
            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";

            Trace.WriteLine(string.Format("Retrieving GaiaOnline job result: {0}", resultUrl));

            WebRequestWithAbortOnError(request, () =>
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        using (var responseStr = response.GetResponseStream())
                        {
                            if (responseStr != null)
                            {
                                var responseString = new StreamReader(responseStr).ReadToEnd();
                                var xmlDoc = new XmlDocument();
                                if (!string.IsNullOrWhiteSpace(responseString))
                                {
                                    xmlDoc.LoadXml(responseString);

                                    var nodes = xmlDoc.SelectNodes("//*[name() ='TABLEDATA']/*[name() = 'TR']");
                                    if (nodes != null)
                                    {
                                        var rv = new List<IStar>();

                                        foreach (XmlNode node in nodes)
                                        {
                                            rv.Add(new GaiaDR2Entry(node));
                                        }

                                        DownloadedStars = rv;

                                        Trace.WriteLine(string.Format("GaiaOnline got {0} stars", rv.Count));

                                        UpdateStatus("Cleaning up...", JobStaus.Retreived);
                                    }
                                }
                            }
                        }
                    }
                }
            }, 
            3);
        }

        private void CleanUpGaiaTapResult()
        {
            var request = (HttpWebRequest)HttpWebRequest.Create(jobUrl);
            request.Method = "DELETE";
            request.Headers.Add(HttpRequestHeader.Authorization, "Token " + authToken);
            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";

            WebRequestWithAbortOnError(request, () =>
            {
                try
                {
                    using (var response = (HttpWebResponse)request.GetResponse())
                    {
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            // All good
                            Trace.WriteLine(string.Format("GaiaOnline job {0} deleted", jobUrl));
                        }
                    }
                }
                catch (WebException wex)
                {
                    var webResp = wex.Response as HttpWebResponse;
                    if (webResp != null &&
                        webResp.StatusCode == HttpStatusCode.NotFound)
                    {
                        Trace.WriteLine(string.Format("GaiaOnline job {0} already deleted", jobUrl));
                    }
                    else
                    {
                        throw;
                    }
                }
            });

            UpdateStatus("Done.", JobStaus.Ready);
            currentState = JobStaus.Ready;
        }

        private void WebRequestWithAbortOnError(HttpWebRequest request, Action action, int retryCount = 1)
        {
            for (int i = 1; i <= retryCount; i++)
            {
                try
                {
                    action();

                    break;
                }
                catch (WebException wex)
                {
                    if (i < retryCount)
                    {
                        Thread.Sleep(5000);
                        continue;
                    }

                    var webResp = wex.Response as HttpWebResponse;
                    if (webResp != null)
                    {
                        AbortWithError(string.Format("{0} returned {1} {2} {3}", request.RequestUri, wex.Status, webResp.StatusCode, wex.Message));
                    }
                    else
                    {
                        AbortWithError(string.Format("{0} returned {1} {2}", request.RequestUri, wex.Status, wex.Message));
                    }
                }
                catch (Exception ex)
                {
                    AbortWithError(string.Format("{0} resulted in {1} {2}", request.RequestUri, ex.GetType().Name, ex.Message));
                }
            }
        }

        internal delegate void StringMessageCallback(string errorMessage);
        internal delegate void StatusMessageCallback(string errorMessage, JobStaus? newJobStatus);

        private void UpdateStatusCallback(string message, JobStaus? newJobStatus)
        {
            if (currentState != JobStaus.Queued)
                lblStatusMessage.Text = string.Format("{0}: {1}", currentState, message);
            else
                lblStatusMessage.Text = message;

            if (newJobStatus != null)
            {
                currentState = newJobStatus.Value;
                timer1.Interval = 1000;
                timer1.Enabled = true;
            }
        }

        private void AbortWithErrorCallback(string errorMessage)
        {
            MessageBox.Show(errorMessage, "Tangra", MessageBoxButtons.OK, MessageBoxIcon.Error);
            this.DialogResult = DialogResult.Abort;
            Close();
        }

        private void UpdateStatus(string message, JobStaus? newJobStatus = null)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new StatusMessageCallback(UpdateStatusCallback), message, newJobStatus);
            }
            else
            {
                UpdateStatusCallback(message, newJobStatus);
            }
        }

        private void AbortWithError(string errorMessage)
        {
            if (this.IsDisposed)
            {
                return;
            }

            if (this.InvokeRequired)
            {
                this.Invoke(new StringMessageCallback(AbortWithErrorCallback), errorMessage);
            }
            else
            {
                AbortWithErrorCallback(errorMessage);
            }
        }

        private void frmGaiaRestCaller_FormClosing(object sender, FormClosingEventArgs e)
        {
            cancellationTokenSource.Cancel();
        }
    }
}
