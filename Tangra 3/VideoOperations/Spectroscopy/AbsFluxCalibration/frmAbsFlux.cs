using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Helpers;
using Tangra.Model.Helpers;
using nom.tam.fits;
using nom.tam.util;

namespace Tangra.VideoOperations.Spectroscopy.AbsFluxCalibration
{
	public partial class frmAbsFlux : Form
	{
		private bool m_NewlyOpened = false;
		private AbsFluxCalibrator m_AbsFluxCalibrator;

		public frmAbsFlux()
		{
			InitializeComponent();

			m_NewlyOpened = true;
			m_AbsFluxCalibrator = new AbsFluxCalibrator();

			UpdateUIState();
		}

		private void frmAbsFlux_Load(object sender, EventArgs e)
		{

		}

		private void UpdateUIState()
		{
			if (m_NewlyOpened)
				btnBrowseFiles.Text = "Browse for Spectra Files";
			else
				btnBrowseFiles.Text = "Change Spectra Files Location";

			if (lbAvailableFiles.Items.Count > 0)
				lblAvailableSpectraTitle.Text = "Available Files (doubleclick to include)";
			else
				lblAvailableSpectraTitle.Text = "Available Files";

			if (lbIncludedSpecta.Items.Count > 0)
				lblUsedSpectraTitle.Text = "Used Spectra (right-click to exclude)";
			else
				lblUsedSpectraTitle.Text = "Used Spectra";
		}

		private void btnBrowseFiles_Click(object sender, EventArgs e)
		{
			if (openFileDialog.ShowDialog(this) == DialogResult.OK)
			{
				string fileExt = Path.GetExtension(openFileDialog.FileName);
				if (!AbsFluxInputFile.IsFileTypeSupported(fileExt))
				{
					MessageBox.Show(
						string.Format("{0} files are not supported.", fileExt), 
						"Tangra", 
						MessageBoxButtons.OK, 
						MessageBoxIcon.Error);
					return;
				}

				string filePath = Path.GetDirectoryName(openFileDialog.FileName);
				if (filePath != null && Directory.Exists(filePath))
				{
					int addedFiles = 0;

					string[] files1 = Directory.GetFiles(filePath, "*.dat");

					lbAvailableFiles.Tag = filePath;
					lbAvailableFiles.Items.Clear();
					Cursor = Cursors.WaitCursor;
					try
					{
						foreach (string fileName in files1)
						{
							var inputFile = new AbsFluxInputFile(fileName);
							if (inputFile.ContainsWavelengthData)
							{
								lbAvailableFiles.Items.Add(inputFile);
								addedFiles++;
							}
						}
					}
					finally
					{
						Cursor = Cursors.Default;	
					}

					if (fileExt != null && !".dat".Equals(fileExt, StringComparison.InvariantCultureIgnoreCase))
					{
						files1 = Directory.GetFiles(filePath, string.Format("*.{0}", fileExt.TrimStart('.')));
						foreach (string fileName in files1)
						{
							var wrapper = new AbsFluxInputFile(fileName);
							if (wrapper.ContainsWavelengthData)
							{
								lbAvailableFiles.Items.Add(wrapper);
								addedFiles++;
							}
						}
					}

					if (addedFiles > 0)
					{
						m_NewlyOpened = false;
						UpdateUIState();						
					}
					else
						MessageBox.Show("None of the files in the selected location contain wavelength calibrated data.", "Tangra", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}

		private void lbAvailableFiles_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			var selectedFile = lbAvailableFiles.SelectedItem as AbsFluxInputFile;
			if (selectedFile != null)
			{
				if (lbIncludedSpecta.Items.Cast<AbsFluxSpectra>()
						.Any(x => x.FullFilePath.Equals(selectedFile.FullPath, StringComparison.InvariantCultureIgnoreCase)))
				{
					MessageBox.Show(string.Format("The file '{0}' has been already included.", selectedFile.FullPath), "Tangra", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
				else
					IncludeInputFile(selectedFile);
			}
		}

		private void IncludeInputFile(AbsFluxInputFile inputFile)
		{
			var spectra = new AbsFluxSpectra(inputFile);

			lbIncludedSpecta.Items.Add(spectra);
			lbAvailableFiles.Items.Remove(inputFile);

			if (spectra.IsComplete)
			{
				lbIncludedSpecta.ItemCheck -= lbIncludedSpecta_ItemCheck;
				try
				{
					lbIncludedSpecta.SetItemChecked(lbIncludedSpecta.Items.IndexOf(spectra), true);
				}
				finally
				{
					lbIncludedSpecta.ItemCheck += lbIncludedSpecta_ItemCheck;	
				}
				
				m_AbsFluxCalibrator.AddSpectra(spectra);
			}
		}

		private void miExcludeSpectra_Click(object sender, EventArgs e)
		{
			var selectedSpectra = lbIncludedSpecta.SelectedItem as AbsFluxSpectra;
			if (selectedSpectra != null)
			{
				lbIncludedSpecta.Items.Remove(selectedSpectra);
				lbAvailableFiles.Items.Add(selectedSpectra.InputFile);

				m_AbsFluxCalibrator.RemoveSpectra(selectedSpectra);
			}
		}

		private void lbIncludedSpecta_ItemCheck(object sender, ItemCheckEventArgs e)
		{
			var selectedSpectra = lbIncludedSpecta.Items[e.Index] as AbsFluxSpectra;
			if (selectedSpectra != null)
			{
				if (e.CurrentValue == CheckState.Checked && e.NewValue == CheckState.Unchecked)
				{
					m_AbsFluxCalibrator.RemoveSpectra(selectedSpectra);
				}
				else if (e.CurrentValue == CheckState.Unchecked && e.NewValue == CheckState.Checked)
				{
					TryAddSpectraToCalibrator(selectedSpectra, true);
				}
			}
		}

		private void ctxMenuIncludedSpectra_Opening(object sender, CancelEventArgs e)
		{
			e.Cancel = lbIncludedSpecta.SelectedItem as AbsFluxSpectra == null;
		}

		private void TryAddSpectraToCalibrator(AbsFluxSpectra spectra, bool forceComplete)
		{
			if (spectra.IsComplete)
			{
				m_AbsFluxCalibrator.AddSpectra(spectra);
			}
			else if (forceComplete)
			{
				// TODO: Ask user to provide missing data (object coordinates, identification, site location, exposure, etc)
				//       Then add the spectra to the calibration if all defined okay
			}
		}

        #region Reference Code for Building the CalSpec.db
        private void BuildCalSpecDb()
        {
            var db = new CalSpecDatabase();
            Fits fitsFile = new Fits();
            int totalBad = 0;
            string[] lines2 = File.ReadAllLines(@"Z:\CALSPEC\AbsFlux-TangraStars2.csv");
            for (int i = 1; i < lines2.Length; i++)
            {
                string[] tokens = lines2[i].Split(',');

                string calSpecId = tokens[0].Trim();
                string RA_FK5_Hours = tokens[1].Trim();
                string DEG_FK5_Deg = tokens[2].Trim();
                string pmRA = tokens[3].Trim();
                string pmDec = tokens[4].Trim();
                string specType = tokens[5].Trim();
                string magV = tokens[6].Trim();
                string magBV = tokens[7].Trim();
                string absFluxId = tokens[8].Trim();
                string stisFlag = tokens[9].Trim();
                string fitsFilePath = tokens[10].Trim();
                string tyc2 = tokens[11].Trim();
                string ucac4 = tokens[12].Trim();

                if (!string.IsNullOrEmpty(pmRA) && !string.IsNullOrEmpty(pmDec))
                {
                    var star = new CalSpecStar()
                    {
                        CalSpecStarId = calSpecId,
                        AbsFluxStarId = absFluxId,
                        STIS_Flag = stisFlag,
                        FITS_File = fitsFilePath,
                        TYC2 = tyc2,
                        U4 = ucac4,
                        pmRA = float.Parse(pmRA),
                        pmDE = float.Parse(pmDec),
                        MagV = float.Parse(magV),
                        MagBV = float.Parse(magBV),
                        SpecType = specType,
                        RA_J2000_Hours = (float)AstroConvert.ToRightAcsension(RA_FK5_Hours),
                        DE_J2000_Deg = (float)AstroConvert.ToDeclination(DEG_FK5_Deg)
                    };

                    string filePath = Path.GetFullPath(@"Z:\CALSPEC\current_calspec\" + fitsFilePath);

                    using (var bf = new BufferedFile(filePath, FileAccess.Read, FileShare.ReadWrite))
                    {
                        fitsFile.Read(bf);

                        BasicHDU imageHDU = fitsFile.GetHDU(1);

                        var table = (ColumnTable)imageHDU.Data.DataArray;
                        double[] wavelengths = (double[])table.Columns[0];
                        float[] fluxes = (float[])table.Columns[1];
                        short[] goodnessFlags = (short[])table.Columns[5];

                        for (int j = 0; j < fluxes.Length; j++)
                        {
                            if (wavelengths[j] < 3000) continue;
                            if (wavelengths[j] > 10120) break;

                            if (goodnessFlags[j] != 0)
                                star.DataPoints.Add((float)wavelengths[j], fluxes[j]);
                            else
                                totalBad++;
                        }
                    }

                    db.Stars.Add(star);
                }
            }

            using (var compressedStream = new FileStream(@"Z:\CALSPEC\current_calspec\tangra.db", FileMode.CreateNew, FileAccess.Write))
            using (var deflateStream = new DeflateStream(compressedStream, CompressionMode.Compress, true))
            {
                using (var writer = new BinaryWriter(deflateStream))
                {
                    db.Serialize(writer);
                }
            }

            using (var compressedStream = new FileStream(@"Z:\CALSPEC\current_calspec\tangra.db", FileMode.Open, FileAccess.Read))
            using (var deflateStream = new DeflateStream(compressedStream, CompressionMode.Decompress, true))
            {
                using (var reader = new BinaryReader(deflateStream))
                {
                    var db2 = new CalSpecDatabase(reader);
                    Trace.WriteLine(db2.Stars.Count);
                }
            }

            MessageBox.Show(totalBad.ToString() + " bad entries excluded.");

            string[] fitsFiles = Directory.GetFiles(@"Z:\CALSPEC\current_calspec", "*.fit");

            var dist = new Dictionary<int, int>();
            foreach (string filePath in fitsFiles)
            {
                using (var bf = new BufferedFile(filePath, FileAccess.Read, FileShare.ReadWrite))
                {
                    fitsFile.Read(bf);

                    var bld = new StringBuilder();

                    BasicHDU headerHDU = fitsFile.GetHDU(0);
                    BasicHDU imageHDU = fitsFile.GetHDU(1);

                    for (int i = 0; i < headerHDU.Header.NumberOfCards; i++)
                    {
                        string cardString = headerHDU.Header.GetCard(i);
                        bld.AppendFormat("# {0}\r\n", cardString);
                    }

                    for (int i = 0; i < imageHDU.Header.NumberOfCards; i++)
                    {
                        string cardString = imageHDU.Header.GetCard(i);
                        bld.AppendFormat("# {0}\r\n", cardString);
                    }

                    var table = (ColumnTable)imageHDU.Data.DataArray;
                    double[] wavelengths = (double[])table.Columns[0];
                    float[] fluxes = (float[])table.Columns[1];
                    float[] col2 = (float[])table.Columns[2];
                    float[] col3 = (float[])table.Columns[3];
                    float[] col4 = (float[])table.Columns[4];
                    short[] goodnessFlags = (short[])table.Columns[5];
                    float[] exposures = (float[])table.Columns[6];

                    for (int i = 0; i < fluxes.Length; i++)
                    {
                        if (wavelengths[i] < 2000) continue;
                        if (wavelengths[i] > 15000) break;

                        bld.Append(wavelengths[i].ToString().PadLeft(20));
                        bld.Append(fluxes[i].ToString().PadLeft(20));
                        bld.Append(col2[i].ToString().PadLeft(20));
                        bld.Append(col3[i].ToString().PadLeft(20));
                        bld.Append(col4[i].ToString().PadLeft(20));
                        bld.Append(goodnessFlags[i].ToString().PadLeft(15));
                        bld.Append(exposures[i].ToString().PadLeft(15));
                        bld.AppendLine();

                        int expMS = (int)Math.Round(exposures[i] * 1000);
                        if (!dist.ContainsKey(expMS)) dist.Add(expMS, 0);
                        dist[expMS]++;

                    }

                    string outFileName = Path.ChangeExtension(filePath, ".txt");
                    File.WriteAllText(outFileName, bld.ToString());
                }
            }

            var output = new StringBuilder();
            foreach (int key in dist.Keys)
            {
                output.AppendFormat("{0}s = {1}\r\n", (key / 1000.0).ToString("0.0"), dist[key]);
            }
            MessageBox.Show(output.ToString());
        }

        private void TestEmbeddedData()
        {
            foreach (CalSpecStar star in CalSpecDatabase.Instance.Stars)
            {
                Trace.WriteLine(star.AbsFluxStarId);
            }
        }
        #endregion
	}
}
