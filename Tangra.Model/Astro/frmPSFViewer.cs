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
using Tangra.Model.Astro;

namespace Tangra.Model.Astro
{
	public partial class frmPSFViewer : Form
	{
        private IPSFFit m_PSFFit;
		private int m_Bpp;
		private uint m_NormVal;

		public frmPSFViewer(IPSFFit psfFit, int bpp, uint normVal)
		{
			InitializeComponent();

			m_PSFFit = psfFit;
			m_Bpp = bpp;
			m_NormVal = normVal;
		}

		private void frmPSFViewer_Load(object sender, EventArgs e)
		{
			picFIT.Image = new Bitmap(picFIT.Width, picFIT.Height);
			using (Graphics g = Graphics.FromImage(picFIT.Image))
			{
				m_PSFFit.DrawGraph(g, new Rectangle(0, 0, picFIT.Image.Width, picFIT.Image.Height), m_Bpp, m_NormVal);
				g.Save();
			}

            int side = m_PSFFit.MatrixSize * 16;
		    picPixels.Width = side;
		    picPixels.Height = side;
            picPixels.Image = new Bitmap(side, side);
            using (Graphics g = Graphics.FromImage(picPixels.Image))
            {
				m_PSFFit.DrawDataPixels(g, new Rectangle(0, 0, picPixels.Width, picPixels.Height), m_Bpp, m_NormVal);
                g.Save();
            }

			picFIT.Refresh();
		    picPixels.Refresh();
		}
	}
}
