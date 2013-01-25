using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Tangra.VideoOperations.LightCurves
{
	public partial class frmEditLcDataSeries : Form
	{
		internal List<LCFileSeriesEntry> DataList;

		public frmEditLcDataSeries()
		{
			InitializeComponent();
		}

		public void SetDataSource(List<LCFileSeriesEntry> dataList)
		{
			DataList = dataList;
			dgvData.DataSource = dataList;
		}

		private void btnSave_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;
			Close();
		}
	}
}
