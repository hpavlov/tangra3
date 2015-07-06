using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Tangra.VideoOperations.Spectroscopy.Helpers
{
	public partial class frmStackedPixels : Form
	{
		public frmStackedPixels()
		{
			InitializeComponent();
		}

		public void DisplayBitmap(Bitmap bmp)
		{
			Width = Width - picPixels.Width + bmp.Width;
			Height = Height - picPixels.Height + bmp.Height;
			picPixels.Image = bmp;
		}
	}
}
