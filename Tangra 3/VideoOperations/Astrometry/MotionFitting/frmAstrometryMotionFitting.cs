using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Config;
using Tangra.Model.Config;
using Tangra.Model.Helpers;
using Tangra.Model.Numerical;
using Tangra.MotionFitting;
using Tangra.VideoOperations.Astrometry.Engine;
using Tangra.VideoOperations.Astrometry.MPCReport;

namespace Tangra.VideoOperations.Astrometry.MotionFitting
{
    public partial class frmAstrometryMotionFitting : Form, IMPCReportFileManager
    {
        public const decimal DEFAULT_ARCSEC_PER_PIXEL = 2.5M;

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

            dtpDate.Value = DateTime.Today;

            cbxErrorMethod.Items.Clear();
            foreach(var method in typeof(ErrorMethod).GetValuesDescription())
                cbxErrorMethod.Items.Add(method);
            cbxErrorMethod.SelectedIndex = 0;
        }


        internal protected frmAstrometryMotionFitting(string fileName)
            : this()
        {
            lbAvailableFiles.Items.Add(new AvailableFileEntry(fileName));
        }

        internal protected void CalcFirstFile()
        {
            if (lbAvailableFiles.Items.Count > 0)
                lbAvailableFiles.SelectedIndex = 0;
        }

        private void tsbtnOpenFile_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                if (openFileDialog1.FileNames != null && openFileDialog1.FileNames.Length > 1)
                {
                    foreach (var fileName in openFileDialog1.FileNames)
                        lbAvailableFiles.Items.Add(new AvailableFileEntry(fileName));
                }
                else
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
                if (!ReferenceEquals(m_SelectedEntry, entry))
                {
                    m_DataProvider = new MeasurementPositionCSVProvider(entry.FilePath);
                    m_SelectedEntry = entry;

                    if (m_DataProvider.Unmeasurable)
                    {
                        MessageBox.Show(this, "This file cannot be processed by the Fast Motion Astrometry module. Please use AAV files or AVI files for no-integration videos.", "Tangra", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        m_DataProvider = MeasurementPositionCSVProvider.Empty;
                        lbAvailableFiles.SelectedIndex = -1;
                        Recalculate();
                        return;
                    }

                    if (m_DataProvider.NumberOfMeasurements < 3)
                    {
                        MessageBox.Show(this,
                            "At least 3 data points are required for the Fast Motion Astrometry module. Please use a different file.",
                            "Tangra",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);

                        m_DataProvider = MeasurementPositionCSVProvider.Empty;
                        lbAvailableFiles.SelectedIndex = -1;
                        Recalculate();
                        return;
                    }

                    nudPixelsPerArcSec.ValueChanged -= nudPixelsPerArcSec_ValueChanged;
                    nudInstDelaySec.ValueChanged -= nudInstDelaySec_ValueChanged;
                    dtpDate.ValueChanged -= dtpDate_ValueChanged;
                    tbxObsCode.TextChanged -= tbxObsCode_TextChanged;
                    tbxObjectDesign.TextChanged -= tbxObjectDesign_TextChanged;
                    nudMeaIntervals.ValueChanged -= OnChunkSizeChanged;
                    try
                    {
                        tbxObjectDesign.Text = m_DataProvider.ObjectDesignation;
                        tbxObsCode.Text = m_DataProvider.ObservatoryCode;
                        if (!string.IsNullOrWhiteSpace(m_DataProvider.CatalogueCode)) tbxNetCode.Text = m_DataProvider.CatalogueCode;
                        if (m_DataProvider.ObservationDate.HasValue && m_DataProvider.ObservationDate.Value != DateTime.MinValue)
                            dtpDate.Value = m_DataProvider.ObservationDate.Value;
                        nudInstDelaySec.Value = m_DataProvider.InstrumentalDelaySec;
                        nudPixelsPerArcSec.Value = m_DataProvider.ArsSecsInPixel > 0 ? (decimal)m_DataProvider.ArsSecsInPixel : DEFAULT_ARCSEC_PER_PIXEL;

                        nudMeaIntervals.Value = CalculateOptimalChunks(m_DataProvider.NumberOfMeasurements);

                        if (cbxErrorMethod.SelectedIndex < 1 && m_DataProvider.Measurements.Count() < 10)
                        {
                            // For smaller number of measurements by default use a solution error of StdDev / 2
                            // This appears to be working better
                            cbxErrorMethod.SelectedIndex = (int) ErrorMethod.HalfStdDev;
                        }
                    }
                    finally
                    {
                        nudMeaIntervals.ValueChanged += OnChunkSizeChanged;
                        nudPixelsPerArcSec.ValueChanged += nudPixelsPerArcSec_ValueChanged;
                        nudInstDelaySec.ValueChanged += nudInstDelaySec_ValueChanged;
                        dtpDate.ValueChanged += dtpDate_ValueChanged;
                        tbxObsCode.TextChanged += tbxObsCode_TextChanged;
                        tbxObjectDesign.TextChanged += tbxObjectDesign_TextChanged;
                    }

                    cbxContraintPattern.SelectedIndex = 0;
                    if (m_DataProvider.NumberOfMeasurements < 12)
                    {
                        cbxContraintPattern.Enabled = false;

                        MessageBox.Show(this, 
                            "At least 12 data points are required to use constraint patterns. This file doesn't have enough data points and constraint patters have been disabled.", 
                            "Tangra", 
                            MessageBoxButtons.OK, 
                            MessageBoxIcon.Information);
                    }
                    else
                        cbxContraintPattern.Enabled = true;

                    

                    Recalculate();
                }
            }
        }

        private int CalculateOptimalChunks(int numberOfMeasurements)
        {
            if (numberOfMeasurements < 40)
                return 1;
            else if (numberOfMeasurements < 60)
                return 2;
            else if (numberOfMeasurements < 120)
                return 3;
            else if (numberOfMeasurements < 1000)
                return 4;
            else if (numberOfMeasurements < 1500)
                return 5;
            else 
                return 6;
        }

        private void Recalculate()
        {
            if (m_DataProvider != null)
            {
                WeightingMode mode = WeightingMode.None;
                if (rbWeightingPosAstr.Checked) mode = WeightingMode.SNR;
                else if (rbWeightingAstr.Checked) mode = WeightingMode.SolutionUncertainty;

                var settings = new ReductionSettings()
                {
                    InstrumentalDelaySec = nudInstDelaySec.Value,
                    Weighting = mode,
                    NumberOfChunks = (int)nudMeaIntervals.Value,
                    RemoveOutliers = cbxOutlierRemoval.Checked,
                    ConstraintPattern = cbxContraintPattern.SelectedIndex,
                    BestPositionUncertaintyArcSec = TangraConfig.Settings.Astrometry.AssumedPositionUncertaintyPixels * (double)nudPixelsPerArcSec.Value,
                    SmallestReportedUncertaintyArcSec = TangraConfig.Settings.Astrometry.SmallestReportedUncertaintyArcSec,
                    FactorInPositionalUncertainty = cbxFactorInPositionalUncertainty.Checked,
                    ErrorMethod = (ErrorMethod)cbxErrorMethod.SelectedIndex,
                    OutliersSigmaThreashold = (double)nudSigmaExclusion.Value
                };

                m_PositionExtractor.Calculate(
                    m_DataProvider,
                    settings);

                Replot();

                var lines = m_PositionExtractor.ExtractPositions(tbxObsCode.Text, tbxObjectDesign.Text, dtpDate.Value.Date);
                tbxMeasurements.Text = string.Join("\r\n", lines);
            }
        }

        private void Replot()
        {
            if (m_DataProvider != null)
            {
                if (pboxRAPlot.Image == null || 
                    pboxRAPlot.Image.Width != pboxRAPlot.Width ||
                    pboxRAPlot.Image.Height != pboxRAPlot.Height)
                {
                    if (pboxRAPlot.Width != 0 && pboxRAPlot.Height != 0)
                        pboxRAPlot.Image = new Bitmap(pboxRAPlot.Width, pboxRAPlot.Height, PixelFormat.Format24bppRgb);
                }

                if (pboxDECPlot.Image == null ||
                    pboxDECPlot.Image.Width != pboxDECPlot.Width ||
                    pboxDECPlot.Image.Height != pboxDECPlot.Height)
                {
                    if (pboxDECPlot.Width != 0 && pboxDECPlot.Height != 0)
                        pboxDECPlot.Image = new Bitmap(pboxDECPlot.Width, pboxDECPlot.Height, PixelFormat.Format24bppRgb);
                }

                if (pboxRAPlot.Image != null)
                {
                    using (Graphics gra = Graphics.FromImage(pboxRAPlot.Image))
                    {
                        m_PositionExtractor.PlotRAFit(gra, pboxRAPlot.Image.Width, pboxRAPlot.Image.Height);
                        gra.Save();
                    }                    
                }

                if (pboxDECPlot.Image != null)
                {
                    using (Graphics gde = Graphics.FromImage(pboxDECPlot.Image))
                    {
                        m_PositionExtractor.PlotDECFit(gde, pboxDECPlot.Image.Width, pboxDECPlot.Image.Height);
                        gde.Save();
                    }
                }

                pboxRAPlot.Refresh();
                pboxDECPlot.Refresh();
            }
        }

        private void OnWeightingChanged(object sender, EventArgs e)
        {
            Recalculate();
        }

        private void OnChunkSizeChanged(object sender, EventArgs e)
        {
            Recalculate();
        }

        private void frmAstrometryMotionFitting_Resize(object sender, EventArgs e)
        {
            var halfWidth = pnlPlot.Width / 2;
            pboxRAPlot.Width = halfWidth;

            Replot();
        }

        private void cbxOutlierRemoval_CheckedChanged(object sender, EventArgs e)
        {
            nudSigmaExclusion.Enabled = cbxOutlierRemoval.Checked;
            Recalculate();
        }

        private void nudSigmaExclusion_ValueChanged(object sender, EventArgs e)
        {
            Recalculate();
        }

        private void cbxContraintPattern_SelectedIndexChanged(object sender, EventArgs e)
        {
            Recalculate();
        }

        private void cbxFactorInPositionalUncertainty_CheckedChanged(object sender, EventArgs e)
        {
            Recalculate();
        }

        private void nudPixelsPerArcSec_ValueChanged(object sender, EventArgs e)
        {
            Recalculate();
        }

        private void nudInstDelaySec_ValueChanged(object sender, EventArgs e)
        {
            Recalculate();
        }

        private void dtpDate_ValueChanged(object sender, EventArgs e)
        {
            Recalculate();
        }

        private void tbxObsCode_TextChanged(object sender, EventArgs e)
        {
            Recalculate();
        }

        private void tbxObjectDesign_TextChanged(object sender, EventArgs e)
        {
            Recalculate();
        }

        private void cbxErrorMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            Recalculate();
        }

        private void btnCalcPos_Click(object sender, EventArgs e)
        {
            if (m_PositionExtractor != null)
            {
                var frmToD = new frmCalculatePositionForTimeOfDay();
                if (frmToD.ShowDialog(this) == DialogResult.OK)
                {
                    // Calculate single position
                    string reportLine = m_PositionExtractor.CalculateSingleMeasurement(tbxObsCode.Text, tbxObjectDesign.Text, dtpDate.Value.Date, (double)frmToD.nudSinglePosMea.Value);
                    if (!string.IsNullOrWhiteSpace(reportLine))
                        tbxMeasurements.Text = tbxMeasurements.Text.TrimEnd("\r\n".ToCharArray()) + "\r\n" + reportLine;
                }
            }
        }

        private void btnAddToMCPReport_Click(object sender, EventArgs e)
        {
            if (tbxNetCode.Text.Trim().Length == 0)
            {
                MessageBox.Show(this, "Catalogue code must be specified.", "Tangra", MessageBoxButtons.OK, MessageBoxIcon.Error);
                tbxNetCode.Focus();
                return;
            }

            SaveToReportFile();
        }

        private MPCReportFile m_CurrentReportFile;

        private void SaveToReportFile()
        {
            if (m_CurrentReportFile == null)
            {
                // Is there a report form currently opened? 
                // If no then ask the user to append to an existing report or create a new one
                frmChooseReportFile reportFileForm = new frmChooseReportFile();
                if (reportFileForm.ShowDialog(this.ParentForm) != DialogResult.OK)
                    return;

                if (reportFileForm.IsNewReport)
                {
                    frmMPCObserver frmObserver = new frmMPCObserver(frmMPCObserver.MPCHeaderSettingsMode.NewMPCReport);
                    if (frmObserver.ShowDialog(ParentForm) == DialogResult.Cancel)
                        return;

                    if (saveFileDialog.ShowDialog(ParentForm) != DialogResult.OK)
                        return;

                    MPCObsHeader header = frmObserver.Header;
                    header.NET = tbxNetCode.Text;
                    m_CurrentReportFile = new MPCReportFile(saveFileDialog.FileName, header);

                    TangraConfig.Settings.RecentFiles.NewRecentFile(RecentFileType.MPCReport, saveFileDialog.FileName);
                    TangraConfig.Settings.Save();
                }
                else
                {
                    m_CurrentReportFile = new MPCReportFile(reportFileForm.ReportFileName);

                    if (m_CurrentReportFile.Header.NET != tbxNetCode.Text)
                    {
                        MessageBox.Show(
                            string.Format("The selected observation file uses {0} rather than {1}. Pelase select a different observation file or change the used catalog.",
                            m_CurrentReportFile.Header.NET, tbxNetCode.Text), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        m_CurrentReportFile = null;
                        return;
                    }
                }
            }

            if (m_CurrentReportFile != null)
            {
                foreach (string line in tbxMeasurements.Lines)
                {
                    string lineToAdd = line;
                    if (TangraConfig.Settings.Astrometry.ExportHigherPositionAccuracy && lineToAdd.Length > 80)
                    {
                        // Remove the extra signifficant digit from the RA/DE in high precision mode as those
                        // cannot be reported to the MPC with the current OBS format

                        //          1         2         3         4         5         6         7         8
                        //0123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789
                        //     K15FB7W  n2015 04 01.51040713 03 21.038 -41 43 17.18          13.5 R      E28 0.12 0.12
                        //        3200  n2017 12 06.78359306 15 58.559 +41 46 52.33          11.9 R      619 0.10 0.10

                        lineToAdd = 
                            line.Substring(0, 38) +
                            double.Parse(line.Substring(38, 6), CultureInfo.InvariantCulture).ToString("00.00") + 
                            line.Substring(44, 8) +
                            double.Parse(line.Substring(52, 5), CultureInfo.InvariantCulture).ToString("00.0") + 
                            line.Substring(57);
                    }

                    m_CurrentReportFile.AddObservation(lineToAdd);
                }

                m_CurrentReportFile.Save();

                m_CurrentReportFile.Present(this);

                AstrometryContext.Current.CurrentReportFile = m_CurrentReportFile;
            }
        }

        public void CloseReportFile()
        {
            m_CurrentReportFile = null;
            AstrometryContext.Current.CurrentReportFile = null;
        }
    }
}
