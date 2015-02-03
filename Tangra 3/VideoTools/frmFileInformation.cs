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
using Tangra.Model.Video;
using Tangra.Video;
using Tangra.Video.SER;

namespace Tangra.VideoTools
{
	public partial class frmFileInformation : Form
	{
		private IFrameStream m_FrameStream;

		public frmFileInformation()
		{
			InitializeComponent();
		}

		internal frmFileInformation(IFrameStream frameStream)
		{
			InitializeComponent();

			m_FrameStream = frameStream;
			PopulateInfo();
		}

		private void PopulateInfo()
		{
			var ds = new List<TagValuePair>();

			ds.Add(new TagValuePair("File Type", m_FrameStream.VideoFileType));
			ds.Add(new TagValuePair("Engine", m_FrameStream.Engine));

			var serStream = m_FrameStream as SERVideoStream;
			if (serStream != null)
			{
				ds.Add(new TagValuePair("Observer", serStream.Observer));
				ds.Add(new TagValuePair("Instrument", serStream.Instrument));
				ds.Add(new TagValuePair("Telescope", serStream.Telescope));
			}

			ds.Add(new TagValuePair("Width", m_FrameStream.Width.ToString()));
			ds.Add(new TagValuePair("Height", m_FrameStream.Height.ToString()));
			ds.Add(new TagValuePair("Total Frames", m_FrameStream.CountFrames.ToString()));
			ds.Add(new TagValuePair("First Frame", m_FrameStream.FirstFrame.ToString()));
			ds.Add(new TagValuePair("Last Frame", m_FrameStream.LastFrame.ToString()));
			ds.Add(new TagValuePair("BitPix", m_FrameStream.BitPix.ToString()));
			ds.Add(new TagValuePair("Frame Rate", m_FrameStream.FrameRate.ToString()));
			ds.Add(new TagValuePair("NormVal", m_FrameStream.GetAav16NormVal().ToString()));

			dgvFileInfo.DataSource = ds;
		}
	}

	public class TagValuePair
	{
		public TagValuePair()
		{ }

		public TagValuePair(string tag, string value)
		{
			Tag = tag;
			Value = value;
		}

		public string Tag { get; set; }
		public string Value { get; set; }
	}
}
