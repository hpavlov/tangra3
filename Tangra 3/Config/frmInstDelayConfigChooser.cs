using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Tangra.Config
{
	public partial class frmInstDelayConfigChooser : Form
	{
		public frmInstDelayConfigChooser()
		{
			InitializeComponent();
		}

		public string SelectedCamera
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public void SetCameraModels(List<string> cameras)
		{
			throw new NotImplementedException();
		}
	}
}
