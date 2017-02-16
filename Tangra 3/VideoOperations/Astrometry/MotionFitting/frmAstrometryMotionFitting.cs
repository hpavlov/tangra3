using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Model.Config;
using Tangra.Model.Helpers;
using Tangra.Model.Numerical;
using Tangra.MotionFitting;

namespace Tangra.VideoOperations.Astrometry.MotionFitting
{
    public partial class frmAstrometryMotionFitting : Form
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
                        if (m_DataProvider.ObservationDate.HasValue && m_DataProvider.ObservationDate.Value != DateTime.MinValue)
                            dtpDate.Value = m_DataProvider.ObservationDate.Value;
                        nudInstDelaySec.Value = m_DataProvider.InstrumentalDelaySec;
                        nudPixelsPerArcSec.Value = m_DataProvider.ArsSecsInPixel > 0 ? (decimal)m_DataProvider.ArsSecsInPixel : DEFAULT_ARCSEC_PER_PIXEL;

                        int optimalChunks = m_DataProvider.NumberOfMeasurements / 50;
                        if (optimalChunks > 6) optimalChunks = 6;
                        if (optimalChunks < 1) optimalChunks = 1;
                        nudMeaIntervals.Value = optimalChunks;
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

                    Recalculate();
                }
            }
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
                    FactorInPositionalUncertainty = cbxFactorInPositionalUncertainty.Checked,
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

        private void nudSinglePosMea_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13 && m_PositionExtractor != null)
            {
                // Calculate single position
                double raHours, deDeg, errRACosDEArcSec, errDEArcSec;
                m_PositionExtractor.CalculateSingleMeasurement((double)nudSinglePosMea.Value, out raHours, out deDeg, out errRACosDEArcSec, out errDEArcSec);

                if (!double.IsNaN(raHours))
                {
                    lblRA.Text = AstroConvert.ToStringValue(raHours, "HH MM SS.TT") + " +/-" + errRACosDEArcSec.ToString("0.00") + "\"";
                    lblDE.Text = AstroConvert.ToStringValue(deDeg, "+HH MM SS.T") + " +/-" + errDEArcSec.ToString("0.00") + "\"";
                }
                else
                {
                    lblRA.Text = "";
                    lblDE.Text = "";
                }
            }
        }

    }
}
