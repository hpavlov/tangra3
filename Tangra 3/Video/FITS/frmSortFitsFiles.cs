using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using nom.tam.fits;
using nom.tam.util;
using Tangra.Helpers;

namespace Tangra.Video.FITS
{
    public partial class frmSortFitsFiles : Form
    {
        private string[] m_FitsFiles;
        private Header[] m_FitsHeaders;
        private DateTime?[] m_FitsTimestamps;

        private int m_FilesWithoutExposure = 0;
        private int m_FilesWithExposure = 0;

        public frmSortFitsFiles()
        {
            InitializeComponent();
        }

        internal void SetFiles(string[] fitsFiles)
        {
            m_FitsFiles = fitsFiles;
        }

        internal string[] GetSortedFiles()
        {
            Array.Sort(m_FitsTimestamps, m_FitsFiles);
            return m_FitsFiles;
        }

        private void frmSortFitsFiles_Shown(object sender, EventArgs e)
        {
            m_FitsHeaders = new Header[m_FitsFiles.Length];
            m_FitsTimestamps = new DateTime?[m_FitsFiles.Length];

            pbar.Minimum = 0;
            pbar.Maximum = m_FitsFiles.Length;
            pbar.Value = 0;

            for (int i = 0; i < m_FitsFiles.Length; i++)
            {
                using (BufferedFile bf = new BufferedFile(m_FitsFiles[i], FileAccess.Read, FileShare.ReadWrite))
                {
                    Header hdr = Header.ReadHeader(bf);
                    m_FitsHeaders[i] = hdr;

                    bool isMidPoint;
                    double? fitsExposure;
                    DateTime? timestamp = FITSHelper.ParseExposure(hdr, out isMidPoint, out fitsExposure);
                    m_FitsTimestamps[i] = timestamp;

                    if (fitsExposure.HasValue)
                        m_FilesWithExposure++;
                    else
                        m_FilesWithoutExposure++;
                }

                pbar.Value = i;
                Application.DoEvents();
            }

            pbar.Value = pbar.Minimum;
            Application.DoEvents();

            timer1.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            if (m_FilesWithExposure == 0 && m_FilesWithoutExposure > 0)
                MessageBox.Show("None of the FITS files have a saved exposure (checked headers: EXPOSURE, EXPTIME and RAWTIME). Please treat the reported timestamps with suspicion.", "Tangra", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else if (m_FilesWithExposure > 0 && m_FilesWithoutExposure > 0)
                MessageBox.Show(string.Format("{0} out of {1} of the FITS files have a missing exposure (checked headers: EXPOSURE, EXPTIME and RAWTIME). Please treat the reported timestamps with suspicion and expect inconsistencies.", m_FilesWithoutExposure, m_FilesWithoutExposure + m_FilesWithExposure), "Tangra", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            Close();
        }
    }
}
