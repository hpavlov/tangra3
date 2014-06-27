using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Model.Config;
using Tangra.Model.Image;

namespace Tangra.Video.SER
{
	public partial class frmSerStatusPopup : Form
	{
		private SerSettings m_Settings;
		private FrameStateData m_FrameState;

		public frmSerStatusPopup(SerSettings settings)
		{
			InitializeComponent();

			m_Settings = settings;
		}

		public void ShowStatus(FrameStateData frameState)
		{
			m_FrameState = frameState;

			var statusText = new StringBuilder();

			statusText.AppendLine(string.Format("System Time: {0}", m_FrameState.SystemTime.ToString("dd MMM yyyy HH:mm:ss.fff")));

			lblStatusCombined.Text = statusText.ToString();
		}

		private void btnCopy_Click(object sender, EventArgs e)
		{
			Clipboard.SetText(lblStatusCombined.Text);
		}
	}
}
