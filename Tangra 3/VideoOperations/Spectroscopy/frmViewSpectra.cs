﻿using System;
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

namespace Tangra.VideoOperations.Spectroscopy
{
    public partial class frmViewSpectra : Form
    {
        private MasterSpectra m_Spectra;
	    private SpectraFileHeader m_Header;
	    private SpectroscopyController m_SpectroscopyController;

        private static Brush[] m_GreyBrushes = new Brush[256];

        static frmViewSpectra()
	    {
		    for (int i = 0; i < 256; i++)
		    {
                m_GreyBrushes[i] = new SolidBrush(Color.FromArgb(i, i, i));
		    }
	    }

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
        }

		internal void SetMasterSpectra(MasterSpectra masterSpectra)
	    {
			m_Spectra = masterSpectra;
	    }

        private void frmViewSpectra_Load(object sender, EventArgs e)
        {
	        PlotSpectra();
        }

	    private void PlotSpectra()
	    {
			DisplaySpectra(m_Spectra);
	    }

        public void DisplaySpectra(Spectra spectra)
        {
            int xOffset = spectra.ZeroOrderPixelNo - 10;
            float xCoeff = picSpectra.Width * 1.0f / spectra.Points.Count;
            float colorCoeff = 1.5f * 256.0f/spectra.MaxPixelValue;

            float yCoeff = (picSpectraGraph.Width - 20) * 1.0f / spectra.MaxPixelValue;
            PointF prevPoint = PointF.Empty;

            using (Graphics g = Graphics.FromImage(picSpectra.Image))
            using (Graphics g2 = Graphics.FromImage(picSpectraGraph.Image))
            {
                g2.Clear(Color.WhiteSmoke);

                foreach (SpectraPoint point in spectra.Points)
                {
                    byte clr = (byte) (Math.Round(point.RawValue*colorCoeff));
                    float x = xCoeff*(point.PixelNo - xOffset);
                    if (x >= 0)
                    {
                        g.FillRectangle(m_GreyBrushes[clr], x, 0, xCoeff, picSpectra.Width);

                        PointF graphPoint = new PointF(x, picSpectraGraph.Image.Height - 10 - (float)Math.Round(point.RawValue * yCoeff));
                        if (prevPoint != PointF.Empty)
                        {
                            if (graphPoint.X > 0 && graphPoint.X < picSpectraGraph.Image.Width && graphPoint.Y > 0 &&
                                graphPoint.Y < picSpectraGraph.Image.Height &&
                                prevPoint.X > 0 && prevPoint.X < picSpectraGraph.Image.Width && prevPoint.Y > 0 &&
                                prevPoint.Y < picSpectraGraph.Image.Height)
                            {
                                g2.DrawLine(Pens.Red, prevPoint, graphPoint);
                            }
                        }
                        prevPoint = graphPoint;
                    }
                }

                g.Save();
                g2.Save();
            }

            picSpectra.Invalidate();
            picSpectraGraph.Invalidate();
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

					//m_LCFilePath = saveFileDialog.FileName;
					//m_LightCurveController.RegisterRecentFile(RecentFileType.LightCurve, saveFileDialog.FileName);
				}
				finally
				{
					Cursor = Cursors.Default;
				}
			}
		}
    }
}
