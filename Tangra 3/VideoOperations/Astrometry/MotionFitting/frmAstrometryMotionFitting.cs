using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Tangra.VideoOperations.Astrometry.MotionFitting
{
    public partial class frmAstrometryMotionFitting : Form
    {
        internal class AvailableFileEntry
        {
            public string FilePath { get; private set; }
            public string FileNameOnly { get; private set; }

            public AvailableFileEntry(string fileName)
            {
                FilePath = fileName;
                FileNameOnly = Path.GetFileName(fileName);
            }

            public override string ToString()
            {
                return FileNameOnly;
            }
        }

        public frmAstrometryMotionFitting()
        {
            InitializeComponent();
        }

        private void tsbtnOpenFile_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                lbAvailableFiles.Items.Add(new AvailableFileEntry(openFileDialog1.FileName));
            }
        }

        private void tsbtnOpenDirectory_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog(this) == DialogResult.OK)
            {
                string[] files = Directory.GetFiles(folderBrowserDialog1.SelectedPath, "*.csv");
                foreach(string file in files)
                    lbAvailableFiles.Items.Add(new AvailableFileEntry(file));
            }
        }

        private IMeasurementPositionProvider m_DataProvider;
        private void lbAvailableFiles_SelectedIndexChanged(object sender, EventArgs e)
        {           
            if (lbAvailableFiles.SelectedIndex != -1)
            {
                var entry = (AvailableFileEntry) lbAvailableFiles.SelectedItem;
                m_DataProvider = new MeasurementPositionCSVProvider(entry.FilePath);
                Recalculate();
            }
        }

        private void Recalculate()
        {
            
        }
    }
}
