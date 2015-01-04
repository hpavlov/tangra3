using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace Tangra.KweeVanWoerden
{
	public partial class frmAddinConfig : Form
	{
		private KweeVanWoerdenAddinSettings m_Settings;

		public frmAddinConfig()
		{
			InitializeComponent();
		}

        public override object InitializeLifetimeService()
        {
            // The lifetime of the object is managed by the add-in
            return null;
        }

		internal void SetSettings(KweeVanWoerdenAddinSettings settings)
		{
			m_Settings = settings;

			cbxUseSimulatedData.Checked = m_Settings.UseSimulatedDataSet; 
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			m_Settings.UseSimulatedDataSet = cbxUseSimulatedData.Checked;
			m_Settings.Save();

			DialogResult = DialogResult.OK;
			Close();
		}
	}
}
