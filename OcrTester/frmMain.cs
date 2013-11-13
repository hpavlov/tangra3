using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tangra.Model.Config;
using Tangra.Model.Image;

namespace OcrTester
{
	public partial class frmMain : Form
	{
		private List<string> m_InputFiles = new List<string>();
		private int m_CurrentIndex = -1;
		private Bitmap m_CurrentImage;
		private Pixelmap m_Pixelmap;
		private OsdProcessor m_Processor;
		private int m_Width = 0;
		private int m_Height = 0;

		public frmMain()
		{
			InitializeComponent();

			m_Processor = new OsdProcessor();
		}

		private void btnReload_Click(object sender, EventArgs e)
		{
			m_CurrentIndex = -1;
			m_InputFiles.Clear();
			m_InputFiles.AddRange(Directory.GetFiles(tbxInputFolder.Text, "*.bmp"));

			if (m_InputFiles.Count > 0) m_CurrentIndex = 0;
			ProcessCurrentImage();
		}

		private void ProcessCurrentImage()
		{
			if (m_CurrentIndex >= 0 && m_CurrentIndex < m_InputFiles.Count)
			{
				string fileName = m_InputFiles[m_CurrentIndex];
				m_CurrentImage = (Bitmap)Bitmap.FromFile(fileName);
				m_Pixelmap = Pixelmap.ConstructFromBitmap(m_CurrentImage, TangraConfig.ColourChannel.Red);

				if (m_Width != m_Pixelmap.Width || m_Height != m_Pixelmap.Height)
				{
					m_Width = m_Pixelmap.Width;
					m_Height = m_Pixelmap.Height;

					m_Processor.Initialise(m_Width, m_Height);					
				}

				using (Graphics g = Graphics.FromImage(m_CurrentImage))
				{
					m_Processor.Process(m_Pixelmap, g);
					g.Flush();
				}				

				picField.Image = m_CurrentImage;
				picField.Update();

			}
		}

		private void btnNext_Click(object sender, EventArgs e)
		{
			if (m_CurrentIndex < m_InputFiles.Count - 1)
			{
				m_CurrentIndex++;
				ProcessCurrentImage();
			}
		}

		private void btnPrev_Click(object sender, EventArgs e)
		{
			if (m_CurrentIndex > 0)
			{
				m_CurrentIndex--;
				ProcessCurrentImage();
			}
		}

		private void picField_MouseDown(object sender, MouseEventArgs e)
		{
			m_Processor.GetBlockAt(e.X, e.Y);

		}
	}
}
