/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Tangra.VideoOperations.Astrometry
{
	public partial class ucFrameInterval : UserControl
	{
		private int m_Value;

		private static int[] INDEX_TO_VAL_MAP = new int[] { 1, 2, 4, 8, 16, 32, 64 };

		public event EventHandler<FrameIntervalChangedEventArgs> FrameIntervalChanged;

		public int Value
		{
			get { return m_Value; }
			set
			{
				if (value == 1) m_Value = 1;
				else if (value == 2) m_Value = 2;
				else if (value == 4) m_Value = 4;
				else if (value == 8) m_Value = 8;
				else if (value == 16) m_Value = 16;
				else if (value == 32) m_Value = 32;
				else if (value == 64) m_Value = 64;
			}
		}
		public ucFrameInterval()
		{
			InitializeComponent();

			cbxEveryFrame.SelectedIndex = 0;
			m_Value = 1;
		}

		private void cbxEveryFrame_SelectedIndexChanged(object sender, EventArgs e)
		{
			m_Value = INDEX_TO_VAL_MAP[cbxEveryFrame.SelectedIndex];
			if (FrameIntervalChanged != null)
				FrameIntervalChanged(this, new FrameIntervalChangedEventArgs(m_Value));
		}
	}

	public class FrameIntervalChangedEventArgs : EventArgs
	{
		public readonly int FrameInterval;

		internal FrameIntervalChangedEventArgs(int frameInterval)
		{
			FrameInterval = frameInterval;
		}
	}
}
