using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Controller;
using Tangra.VideoOperations.Spectroscopy.Helpers;
using Tangra.VideoOperations.Spectroscopy.ViewSpectraStates;

namespace Tangra.VideoOperations.Spectroscopy
{
    public partial class frmViewSpectra : Form
    {
        private MasterSpectra m_Spectra;
	    private SpectraFileHeader m_Header;
	    private SpectroscopyController m_SpectroscopyController;
	    private SpectraViewerStateManager m_StateManager;

	    public frmViewSpectra()
	    {
			InitializeComponent();
	    }

	    public frmViewSpectra(SpectroscopyController controller)
			: this()
	    {
		    m_SpectroscopyController = controller;
		    
            picSpectraGraph.Image = new Bitmap(picSpectraGraph.Width, picSpectraGraph.Height, PixelFormat.Format24bppRgb);
            picSpectra.Image = new Bitmap(picSpectra.Width, picSpectra.Height, PixelFormat.Format24bppRgb);

            m_StateManager = new SpectraViewerStateManager(m_SpectroscopyController, picSpectraGraph, this);
        }

		internal void SetMasterSpectra(MasterSpectra masterSpectra)
	    {
			m_Spectra = masterSpectra;
			m_StateManager.SetMasterSpectra(masterSpectra);
	    }

        private void frmViewSpectra_Load(object sender, EventArgs e)
        {
	        PlotSpectra();

            if (!m_SpectroscopyController.IsCalibrated())
                m_StateManager.ChangeState<SpectraViewerStateCalibrate>();
        }

	    internal void PlotSpectra()
	    {
			m_StateManager.DrawSpectra(picSpectra, picSpectraGraph);

	        gbxDispersion.Visible = m_SpectroscopyController.IsCalibrated();
	        lblDispersion.Text = m_SpectroscopyController.GetSpectraCalibrator().GetCalibrationScale().ToString("0.0 A/pixel");
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

        private void picSpectraGraph_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            m_StateManager.PreviewKeyDown(sender, e);
        }

		private void miSpectralCalibration_Click(object sender, EventArgs e)
		{
			m_StateManager.ChangeState<SpectraViewerStateCalibrate>();
		}

        internal void DisplaySelectedDataPoint(int pixelNo, float wavelength)
        {
            label1.Text = pixelNo.ToString();
            label2.Text = wavelength.ToString("0.0 A");
        }

        private void miShowCommonLines_CheckedChanged(object sender, EventArgs e)
        {
            m_StateManager.ShowCommonLines(miShowCommonLines.Checked);
        }


    }
}
