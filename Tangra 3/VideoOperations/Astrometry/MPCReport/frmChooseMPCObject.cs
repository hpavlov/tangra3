/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Model.Config;

namespace Tangra.VideoOperations.Astrometry.MPCReport
{
	public partial class frmChooseMPCObject : Form
	{
		public frmChooseMPCObject()
		{
			InitializeComponent();
		}

		private void tbxObject_TextChanged(object sender, EventArgs e)
		{
			string designation = MPCObsLine.GetObjectCode(tbxObject.Text);
			if (designation != null)
			{
				tbxLine.Text = designation;
				tbxLine.Enabled = false;
			}
			else
			{
				tbxLine.Text = string.Empty;
				tbxLine.Enabled = false;
			}

			tbxObject.Focus();
		}

		private void tbxLine_TextChanged(object sender, EventArgs e)
		{
			btnOK.Enabled = !string.IsNullOrEmpty(tbxLine.Text);

		}

		private void tbxLine_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				string parsedCode;
				string designation = MPCObsLine.GetObjectCode(tbxLine.Text, out parsedCode);
				tbxLine.Text = designation;
				tbxObject.TextChanged -= tbxObject_TextChanged;
				try
				{
					tbxObject.Text = parsedCode;	
				}
				finally
				{
					tbxObject.TextChanged += tbxObject_TextChanged;
				}				
			}
		}

	}
}
