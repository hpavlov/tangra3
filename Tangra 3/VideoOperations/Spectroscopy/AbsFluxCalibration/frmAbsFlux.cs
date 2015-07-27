using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

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
	}
}
