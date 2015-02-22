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

namespace Tangra.Helpers
{
	public partial class frmLongRunningOperation : Form
	{
		public frmLongRunningOperation()
		{
			InitializeComponent();

			StartPosition = FormStartPosition.CenterParent;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}

			base.Dispose(disposing);
		}

		private IWin32Window m_ParentForm;

		public void Initialise(IWin32Window parentForm)
		{
			m_ParentForm = parentForm;
		}
		public void BeginOperation(string message)
		{
			lblStatus.Text = message;

			if (!this.Visible) this.Show(m_ParentForm);
			Refresh();
		}

		public void EndOperation()
		{
			if (this.Visible) this.Hide();
			Refresh();
		}
	}
}
