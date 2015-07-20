using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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

		public frmAbsFlux()
		{
			InitializeComponent();

			m_NewlyOpened = true;
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
				lblUsedSpectraTitle.Text = "Used Spectra (doubleclick to exclude)";
			else
				lblUsedSpectraTitle.Text = "Used Spectra";
		}

		private void btnBrowseFiles_Click(object sender, EventArgs e)
		{
			if (openFileDialog.ShowDialog(this) == DialogResult.OK)
			{
				string fileExt = Path.GetExtension(openFileDialog.FileName);
				if (!SpectraFileUIWrapper.IsFileTypeSupported(fileExt))
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

					string[] files1 = Directory.GetFiles(filePath, "*.spectra");

					lbAvailableFiles.Tag = filePath;
					lbAvailableFiles.Items.Clear();
					foreach (string fileName in files1)
					{
						var wrapper = new SpectraFileUIWrapper(fileName);
						if (wrapper.ContainsWavelengthData)
						{
							lbAvailableFiles.Items.Add(wrapper);
							addedFiles++;
						}
					}

					if (fileExt != null && !".spectra".Equals(fileExt, StringComparison.InvariantCultureIgnoreCase))
					{
						files1 = Directory.GetFiles(filePath, string.Format("*.{0}", fileExt.TrimStart('.')));
						foreach (string fileName in files1)
						{
							var wrapper = new SpectraFileUIWrapper(fileName);
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
	}
}
