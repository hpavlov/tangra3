using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using nom.tam.fits;
using nom.tam.util;
using Tangra.Controller;
using Tangra.Helpers;
using Tangra.Model.Helpers;

namespace Tangra.Video.FITS
{
    public partial class frmSortFitsFiles : Form
    {
        private string[] m_FitsFiles;
        private Header[] m_FitsHeaders;
        private DateTime?[] m_FitsTimestamps;
        private bool m_SortedByTimeStamp = false;

        private int m_FilesWithoutExposure = 0;
        private int m_FilesWithExposure = 0;

        private string m_OrderedFitsFileHash;

        private VideoController m_VideoController;

        private string GetOrderedFitsFileHash()
        {
            if (m_OrderedFitsFileHash == null)
            {
                var hasher = new SHA1CryptoServiceProvider();
                hasher.Initialize();
                var orderedFilesByName = new List<string>(m_FitsFiles);
                orderedFilesByName.Sort();
                byte[] combinedFileNamesBytes = Encoding.UTF8.GetBytes(string.Join("|", orderedFilesByName));
                var hash = hasher.ComputeHash(combinedFileNamesBytes, 0, combinedFileNamesBytes.Length);
                m_OrderedFitsFileHash = Convert.ToBase64String(hash);
            }

            return m_OrderedFitsFileHash;
        }

        private frmSortFitsFiles()
        {
            InitializeComponent();
        }

        public frmSortFitsFiles(VideoController videoController)
            : this()
        {
            m_VideoController = videoController;
        }

        internal void SetFiles(string[] fitsFiles)
        {
            m_FitsFiles = fitsFiles;
        }

        internal string[] GetSortedFiles()
        {
            if (m_SortedByTimeStamp)
            {
                Array.Sort(m_FitsTimestamps, m_FitsFiles);
                return m_FitsFiles;                
            }
            else
            {
                List<string> byFileNameList = new List<string>(m_FitsFiles);
                byFileNameList.Sort();
                return byFileNameList.ToArray();
            }
        }

        public string ErrorMessage { get; private set; }

        public FITSTimeStampReader TimeStampReader { get; private set; }

        public bool FlipVertically { get; private set; }
        public bool FlipHorizontally { get; private set; }

        public int MinPixelValue { get; private set; }
        public uint MaxPixelValue { get; private set; }
        public int BitPix { get; private set; }
        public int NegPixCorrection { get; private set; }

        internal class FitsFileFormatInfoRecord
        {
            public string FirstFile;
            public int NumFiles;
        }

        private void frmSortFitsFiles_Shown(object sender, EventArgs e)
        {
            m_FitsHeaders = new Header[m_FitsFiles.Length];
            m_FitsTimestamps = new DateTime?[m_FitsFiles.Length];

            pbar.Minimum = 0;
            pbar.Maximum = m_FitsFiles.Length;
            pbar.Value = 0;

            var fileSizeInfo = new Dictionary<string, FitsFileFormatInfoRecord>();
            TimeStampReader = null;

            for (int i = 0; i < m_FitsFiles.Length; i++)
            {
                try
                {
                    using (BufferedFile bf = new BufferedFile(m_FitsFiles[i], FileAccess.Read, FileShare.ReadWrite))
                    {
                        Header hdr = Header.ReadHeader(bf);
                        m_FitsHeaders[i] = hdr;

                        int numAxis = -1;
                        int width = -1;
                        int height = -1;
                        int.TryParse(hdr.FindCard("NAXIS") != null ? hdr.FindCard("NAXIS").Value : "0", out numAxis);
                        int.TryParse(hdr.FindCard("NAXIS1") != null ? hdr.FindCard("NAXIS1").Value : "0", out width);
                        int.TryParse(hdr.FindCard("NAXIS2") != null ? hdr.FindCard("NAXIS2").Value : "0", out height);
                        string format = String.Format("{0} x {1}", width, height);
                        if (fileSizeInfo.ContainsKey(format))
                            fileSizeInfo[format].NumFiles++;
                        else
                            fileSizeInfo.Add(format, new FitsFileFormatInfoRecord { FirstFile = Path.GetFileName(m_FitsFiles[i]), NumFiles = 1 });

                        bool isMidPoint;
                        double? fitsExposure = null;
                        DateTime? timestamp = null;

                        if (i == 0)
                        {
                            var frm = new frmChooseTimeHeaders(m_FitsFiles[i], GetOrderedFitsFileHash(), m_VideoController);
                            if (frm.ShowDialog(this) == DialogResult.OK)
                            {
                                TimeStampReader = frm.TimeStampReader;
                                FlipVertically = frm.FlipVertically;
                                FlipHorizontally = frm.FlipHorizontally;
                            }

                            MinPixelValue = frm.MinPixelValue;
                            MaxPixelValue = frm.MaxPixelValue;
                            BitPix = frm.BitPix;
                            NegPixCorrection = frm.NegPixCorrection;
                        }

                        try
                        {
                            timestamp = FITSHelper2.ParseExposure(m_FitsFiles[i], hdr, TimeStampReader, out isMidPoint, out fitsExposure);
                        }
                        catch (Exception ex)
                        {
                            Trace.WriteLine(ex.ToString());
                        }

                        m_FitsTimestamps[i] = timestamp;

                        if (timestamp != null && fitsExposure.HasValue)
                            m_FilesWithExposure++;
                        else
                            m_FilesWithoutExposure++;
                    }
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(string.Format("Error processing FITS files: {0}", m_FitsFiles[i]), ex);
                }

                pbar.Value = i;
                Application.DoEvents();
            }

            pbar.Value = pbar.Minimum;
            Application.DoEvents();

            if (fileSizeInfo.Count > 1)
            {
                var errorInfo = new StringBuilder();
                foreach (string key in fileSizeInfo.Keys)
                {
                    if (fileSizeInfo[key].NumFiles > 1)
                        errorInfo.AppendFormat("'{1}' and {2} other files: {0}\r\n", key, fileSizeInfo[key].FirstFile, fileSizeInfo[key].NumFiles - 1);
                    else
                        errorInfo.AppendFormat("'{1}' (single file): {0}\r\n", key, fileSizeInfo[key].FirstFile);
                }
                ErrorMessage = string.Format("Cannot load FITS file sequence because there are files with different image dimentions:\r\n\r\n{0}\r\n\r\nPlease ensure that all files in the directory have the same dimention (number of axis).", errorInfo.ToString());

            }
            timer1.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            m_SortedByTimeStamp = true;
            if (m_FilesWithExposure == 0 && m_FilesWithoutExposure > 0)
            {
                MessageBox.Show("None of the FITS files have a saved exposure and they have been ordered by file name!", "Tangra", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                m_SortedByTimeStamp = false;
            }
            else if (m_FilesWithExposure > 0 && m_FilesWithoutExposure > 0)
                MessageBox.Show(string.Format("{0} out of {1} of the FITS files have a missing exposure (checked headers: EXPOSURE, EXPTIME and RAWTIME). Please treat the reported timestamps with suspicion and expect inconsistencies.", m_FilesWithoutExposure, m_FilesWithoutExposure + m_FilesWithExposure), "Tangra", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            Close();
        }
    }
}
