using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Controller;
using Tangra.Model.Config;
using Tangra.Model.Context;
using Tangra.Model.Helpers;
using Tangra.VideoOperations.Spectroscopy.Helpers;
using Tangra.VideoOperations.Spectroscopy.ViewSpectraStates;

namespace Tangra.VideoOperations.Spectroscopy
{
    public partial class frmViewSpectra : Form
    {
        private MasterSpectra m_Spectra;
	    private SpectraFileHeader m_Header;
	    private SpectroscopyController m_SpectroscopyController;
        private VideoController m_VideoController;
	    private SpectraViewerStateManager m_StateManager;
		private TangraConfig.SpectraViewDisplaySettings m_DisplaySettings = new TangraConfig.SpectraViewDisplaySettings();

	    public frmViewSpectra()
	    {
			InitializeComponent();
	    }

        public frmViewSpectra(SpectroscopyController controller, VideoController videoController)
			: this()
	    {
		    m_SpectroscopyController = controller;
		    
            picSpectraGraph.Image = new Bitmap(picSpectraGraph.Width, picSpectraGraph.Height, PixelFormat.Format24bppRgb);
            picSpectra.Image = new Bitmap(picSpectra.Width, picSpectra.Height, PixelFormat.Format24bppRgb);

			m_DisplaySettings.Load();
			m_DisplaySettings.Initialize();

            m_StateManager = new SpectraViewerStateManager(m_SpectroscopyController, picSpectraGraph, this);
	        m_VideoController = videoController;
	    }

		internal void SetMasterSpectra(MasterSpectra masterSpectra)
	    {
			m_Spectra = masterSpectra;
			m_StateManager.SetMasterSpectra(masterSpectra);
		    hsbSlidingWindow.Minimum = 0;
		    hsbSlidingWindow.Maximum = Math.Max(1, m_Spectra.RawMeasurements.Count - 50);
		    hsbSlidingWindow.Value = 0;

			if (masterSpectra.ProcessingInfo.GaussianBlur10Fwhm.HasValue)
				m_StateManager.ApplyGaussianBlur(masterSpectra.ProcessingInfo.GaussianBlur10Fwhm.Value / 10.0f);

            if (masterSpectra.IsCalibrated())
            {
                SpectraCalibrator calibrator = m_SpectroscopyController.GetSpectraCalibrator();    
                calibrator.Reset();

	            if (calibrator.LoadCalibration(masterSpectra.Calibration))
					m_StateManager.ChangeState<SpectraViewerStateCalibrated>();    
            }
            else if (m_SpectroscopyController.HasCalibratedConfiguration())
            {
                if (m_SpectroscopyController.ScaleSpectraByZeroOrderImagePosition())
                    m_StateManager.ChangeState<SpectraViewerStateCalibrated>();    
	        }
	    }

        private void frmViewSpectra_Load(object sender, EventArgs e)
        {
	        PlotSpectra();

            if (!m_SpectroscopyController.IsCalibrated())
                m_StateManager.ChangeState<SpectraViewerStateCalibrate>();
        }

	    internal void PlotSpectra()
	    {
			m_StateManager.DrawSpectra(picSpectra, picSpectraGraph, m_DisplaySettings);

	        gbxCalibration.Visible = m_SpectroscopyController.IsCalibrated();
		    var calibraror = m_SpectroscopyController.GetSpectraCalibrator();
	        lblDispersion.Text = string.Format("Dispersion: {0}", calibraror.GetCalibrationScale().ToString("0.00 A/pix"));
		    lblOrder.Text = string.Format("Order: {0}", calibraror.GetCalibrationOrder());
		    float rms = calibraror.GetCalibrationRMS();
		    if (float.IsNaN(rms))
			    lblRMS.Text = "";
			else
				lblRMS.Text = string.Format("RMS: {0}", rms.ToString("0.00 pix"));
	    }

		private void frmViewSpectra_Resize(object sender, EventArgs e)
		{
			picSpectraGraph.Image = new Bitmap(picSpectraGraph.Width, picSpectraGraph.Height, PixelFormat.Format24bppRgb);
			picSpectra.Image = new Bitmap(picSpectra.Width, picSpectra.Height, PixelFormat.Format24bppRgb);

			PlotSpectra();
		}

		private void miSaveSpectra_Click(object sender, EventArgs e)
		{
			SaveSpectraFile();
		}

		private void SaveSpectraFile()
		{
			m_SpectroscopyController.ConfigureSaveSpectraFileDialog(saveFileDialog);

			if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
			{
				Update();

				Cursor = Cursors.WaitCursor;
				try
				{
					m_Header = m_SpectroscopyController.GetSpectraFileHeader();

					SpectraFile.Save(saveFileDialog.FileName, m_Header, m_Spectra);

					m_SpectroscopyController.RegisterRecentSpectraFile(saveFileDialog.FileName);
				}
				finally
				{
					Cursor = Cursors.Default;
				}
			}
		}

		private void picSpectraGraph_MouseClick(object sender, MouseEventArgs e)
		{
			m_StateManager.MouseClick(sender, e);
		}

		private void picSpectraGraph_MouseDown(object sender, MouseEventArgs e)
		{
			m_StateManager.MouseDown(sender, e);
		}

		private void picSpectraGraph_MouseEnter(object sender, EventArgs e)
		{
			m_StateManager.MouseEnter(sender, e);
		}

		private void picSpectraGraph_MouseLeave(object sender, EventArgs e)
		{
			m_StateManager.MouseLeave(sender, e);
		}

		private void picSpectraGraph_MouseMove(object sender, MouseEventArgs e)
		{
			m_StateManager.MouseMove(sender, e);
		}

		private void picSpectraGraph_MouseUp(object sender, MouseEventArgs e)
		{
			m_StateManager.MouseUp(sender, e);
		}

        internal void DisplaySelectedDataPoint(int pixelNo, float wavelength)
        {
            lblPixelNo.Text = (pixelNo - m_Spectra.ZeroOrderPixelNo).ToString();

            if (!float.IsNaN(wavelength) && pixelNo >= m_Spectra.ZeroOrderPixelNo)
	        {
		        lblWavelength.Text = wavelength.ToString("0.0 A");
		        lblWavelengthCaption.Visible = true;
				lblWavelength.Visible = true;
	        }
	        else
	        {
				lblWavelengthCaption.Visible = false;
				lblWavelength.Visible = false;
			}

	        SpectraPoint point = m_Spectra.Points.SingleOrDefault(x => x.PixelNo == pixelNo);
	        if (point != null)
	        {
				lblPixelValue.Text = point.RawValue.ToString("0.0");
	        }

			gbSelection.Visible = true;
        }

	    internal void ClearSelectedDataPoint()
	    {
		    gbSelection.Visible = false;
	    }

        private void miShowCommonLines_CheckedChanged(object sender, EventArgs e)
        {
            m_StateManager.ShowCommonLines(miShowCommonLines.Checked);
        }

		private void miExport_Click(object sender, EventArgs e)
		{
            m_SpectroscopyController.ConfigureExportSpectraFileDialog(exportFileDialog, m_VideoController.CurrentVideoFileName);

            if (exportFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                SpectraCalibrator calibrator = m_SpectroscopyController.GetSpectraCalibrator();
                var bld = new StringBuilder();

                m_SpectroscopyController.AddDatExportHeader(bld, m_Spectra);

                foreach (SpectraPoint point in m_Spectra.Points)
                {
                    float wavelength = calibrator.ResolveWavelength(point.PixelNo);
                    if (wavelength < 0) continue;
                    bld.AppendFormat("{0}\t{1}\r\n", wavelength, point.RawValue);
                }

                File.WriteAllText(exportFileDialog.FileName, bld.ToString());
            }
		}

		private void frmViewSpectra_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			m_StateManager.PreviewKeyDown(sender, e);
		}

        private void ApplyLowPassFilterMenuItemClicked(object sender, EventArgs e)
        {
            var ctl = sender as ToolStripItem;
            if (ctl != null)
            {
                try
                {
                    float fwhm = Convert.ToSingle(ctl.Tag);

                    m_StateManager.ApplyGaussianBlur(fwhm);
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.GetFullStackTrace());
                }
            }
        }

        private void cbxSlidingWindow_CheckedChanged(object sender, EventArgs e)
        {
            hsbSlidingWindow.Enabled = cbxSlidingWindow.Checked;
            if (cbxSlidingWindow.Checked)
                DoNewMasterSpectraReduction();
        }

        private void DoNewMasterSpectraReduction()
        {
            MasterSpectra newSpectraReduction =
                m_SpectroscopyController.ComputeResult(m_Spectra.RawMeasurements,
                    m_Spectra.MeasurementInfo.FrameCombineMethod,
                    m_Spectra.MeasurementInfo.UseFineAdjustments,
                    m_Spectra.MeasurementInfo.AlignmentAbsorptionLinePos,
                    hsbSlidingWindow.Value + 1,
                    50);

            newSpectraReduction.RawMeasurements.Clear();
            newSpectraReduction.RawMeasurements.AddRange(m_Spectra.RawMeasurements);
            newSpectraReduction.MeasurementInfo = m_Spectra.MeasurementInfo;
            newSpectraReduction.Calibration = m_Spectra.Calibration;

            m_Spectra = newSpectraReduction;
            m_StateManager.SetMasterSpectra(newSpectraReduction);
            PlotSpectra();
        }

        private void hsbSlidingWindow_ValueChanged(object sender, EventArgs e)
        {
            DoNewMasterSpectraReduction();
        }

        private void miLowPass_DropDownOpening(object sender, EventArgs e)
        {
            miLP1_0.Checked = false;
            miLP1_5.Checked = false;
            miLP2_0.Checked = false;
            miLP2_5.Checked = false;
            miLP3_0.Checked = false;
            miLP3_5.Checked = false;
            miLP4_0.Checked = false;
            miLPNone.Checked = false;

            // 'Check' the correct FWHM menu item
            if (m_Spectra.ProcessingInfo.GaussianBlur10Fwhm.HasValue)
            {
                int fwhm10 = m_Spectra.ProcessingInfo.GaussianBlur10Fwhm.Value;
                if (fwhm10 == 10)
                    miLP1_0.Checked = true;
                else if (fwhm10 == 15)
                    miLP1_5.Checked = true;
                else if (fwhm10 == 20)
                    miLP2_0.Checked = true;
                else if (fwhm10 == 25)
                    miLP2_5.Checked = true;
                else if (fwhm10 == 30)
                    miLP3_0.Checked = true;
                else if (fwhm10 == 35)
                    miLP3_5.Checked = true;
                else if (fwhm10 == 40)
                    miLP4_0.Checked = true;
            }
            else
                miLPNone.Checked = true;
        }

        private void frmViewSpectra_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_SpectroscopyController.OnSpectraViewerClosed();
        }

		private void miDisplaySettings_Click(object sender, EventArgs e)
		{
			frmSpectraViewSettings frm = new frmSpectraViewSettings(m_DisplaySettings, this);
			frm.ShowDialog(this);
		}

	    internal void RedrawPlot()
	    {
			PlotSpectra();
			Application.DoEvents();
	    }

		private void miRecalibrateManually_Click(object sender, EventArgs e)
		{
			m_Spectra.Calibration = null;
			m_Spectra.Points.ForEach(x => x.Wavelength = float.NaN);
			m_SpectroscopyController.GetSpectraCalibrator().Reset();
			m_StateManager.ChangeState<SpectraViewerStateCalibrate>();
			RedrawPlot();
		}

		private void miApplySavedCalibration_Click(object sender, EventArgs e)
		{
			var frmCamera = new frmChooseConfiguration(TangraContext.Current.FrameWidth, TangraContext.Current.FrameHeight, m_VideoController.VideoBitPix);
			if (frmCamera.ShowDialog(this) == DialogResult.OK)
			{
				m_Spectra.Points.ForEach(x => x.Wavelength = float.NaN);
				m_SpectroscopyController.GetSpectraCalibrator().Reset();
				m_SpectroscopyController.GetSpectraCalibrator().LoadCalibration(frmCamera.SelectedConfiguration);
				m_StateManager.ChangeState<SpectraViewerStateCalibrated>();
				RedrawPlot();
			}
		}
    }
}
