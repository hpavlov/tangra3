using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Model.Video;
using Tangra.VideoOperations.Spectroscopy.Helpers;

namespace Tangra.VideoOperations.Spectroscopy
{
	public partial class frmRunMultiFrameSpectroscopy : Form
	{
		public int NumberOfMeasurements { get; private set; }

		public SpectraCombineMethod CombineMethod { get; private set; }

		public frmRunMultiFrameSpectroscopy()
		{
			InitializeComponent();
		}

		public frmRunMultiFrameSpectroscopy(IFramePlayer framePlayer)
			: this()
		{
			nudNumberMeasurements.Maximum = framePlayer.Video.LastFrame - framePlayer.CurrentFrameIndex;
			nudNumberMeasurements.Value = Math.Min(200, nudNumberMeasurements.Maximum);
		}

		private void btnNext_Click(object sender, EventArgs e)
		{
			NumberOfMeasurements = (int)nudNumberMeasurements.Value;
			CombineMethod = SpectraCombineMethod.Average;

			DialogResult = DialogResult.OK;
			Close();
		}
	}
}
