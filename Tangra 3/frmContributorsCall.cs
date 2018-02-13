using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Helpers;
using Tangra.Model.Helpers;

namespace Tangra
{
	public partial class frmContributorsCall : Form
	{
		public frmContributorsCall()
		{
			InitializeComponent();
		}

		private void linkLabel1_Click(object sender, EventArgs e)
		{
			ShellHelper.OpenUrl("https://github.com/hpavlov/tangra3");
		}

		private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			ShellHelper.OpenUrl("mailto://hristo_dpavlov@yahoo.com");
		}
	}
}
