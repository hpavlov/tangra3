using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Model.Config;
using Tangra.Model.Numerical;
using Tangra.MotionFitting;

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
        private FastMotionPositionExtractor m_PositionExtractor = new FastMotionPositionExtractor();
        private AvailableFileEntry m_SelectedEntry;

        private void lbAvailableFiles_SelectedIndexChanged(object sender, EventArgs e)
        {           
            if (lbAvailableFiles.SelectedIndex != -1)
            {
                var entry = (AvailableFileEntry) lbAvailableFiles.SelectedItem;
                if (ReferenceEquals(m_SelectedEntry, entry))
                {
                    m_DataProvider = new MeasurementPositionCSVProvider(entry.FilePath);
                    m_SelectedEntry = entry;


                    nudMeaIntervals.ValueChanged -= OnChunkSizeChanged;
                    try
                    {
                        int optimalChunks = m_DataProvider.NumberOfMeasurements / 50;
                        if (optimalChunks > 6) optimalChunks = 6;
                        nudMeaIntervals.Value = optimalChunks;
                    }
                    finally
                    {
                        nudMeaIntervals.ValueChanged += OnChunkSizeChanged;
                    }
                    Recalculate();
                }
            }
        }

        private void Recalculate()
        {
            m_PositionExtractor.Calculate(
                m_DataProvider,
                rbWeightingPosAstr.Checked ? WeightingMode.SNR : WeightingMode.None,
                (int)nudMeaIntervals.Value);
        }

        private void OnWeightingChanged(object sender, EventArgs e)
        {
            Recalculate();
        }

        private void OnChunkSizeChanged(object sender, EventArgs e)
        {
            Recalculate();
        }
    }
}
