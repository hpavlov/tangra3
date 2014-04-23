using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Tangra.Video
{
	public partial class frmJumpToFrame : Form
	{
		public frmJumpToFrame()
		{
			InitializeComponent();
		}

		private void frmJumpToFrame_Load(object sender, EventArgs e)
		{
			nudFrameToJumpTo.Focus();
			nudFrameToJumpTo.Select(0, nudFrameToJumpTo.Text.Length);
		}

		private void nudFrameToJumpTo_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == '\r')
			{
				DialogResult = DialogResult.OK;
				Close();
			}
		}
	}
}
