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
