using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Model.Image;

namespace Tangra.VideoOperations
{
	public partial class frmFullSizePreview : Form
	{
		public frmFullSizePreview()
		{
			InitializeComponent();
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

		private static frmFullSizePreview s_FullPreviewForm = null;
		private static object s_SyncRoot = new object();

		public static void EnsureFullPreviewVisible(Pixelmap currFrame, Form parentForm)
		{
			lock(s_SyncRoot)
			{
				if (s_FullPreviewForm == null)
				{
					s_FullPreviewForm = new frmFullSizePreview();
					s_FullPreviewForm.Width = currFrame.Width + (s_FullPreviewForm.Width - s_FullPreviewForm.pictureBox.Width);
					s_FullPreviewForm.Height = currFrame.Height + (s_FullPreviewForm.Height - s_FullPreviewForm.pictureBox.Height);
					s_FullPreviewForm.Top = parentForm.Top;
					s_FullPreviewForm.Left = parentForm.Right;
					s_FullPreviewForm.StartPosition = FormStartPosition.Manual;
				}

				s_FullPreviewForm.pictureBox.Image = currFrame.CreateDisplayBitmapDoNotDispose();

				if (!s_FullPreviewForm.Visible) 
					s_FullPreviewForm.Show(parentForm);
				s_FullPreviewForm.Refresh();
			}
		}

		public static void EnsureFullPreviewHidden()
		{
			lock (s_SyncRoot)
			{
				if (s_FullPreviewForm != null)
				{
					s_FullPreviewForm.Close();
					s_FullPreviewForm.Dispose();
					s_FullPreviewForm = null;
				}
			}
		}

		public static void Update(Pixelmap currFrame)
		{
			if (s_FullPreviewForm != null)
			{
				lock(s_SyncRoot)
				{
					if (s_FullPreviewForm != null)
					{

						s_FullPreviewForm.pictureBox.Image = currFrame.CreateDisplayBitmapDoNotDispose();
						s_FullPreviewForm.Refresh();
					}
				}
			}
		}

		public static void MoveForm(int left, int top)
		{
			lock (s_SyncRoot)
			{
				if (s_FullPreviewForm != null)
				{
					s_FullPreviewForm.Left = left;
					s_FullPreviewForm.Top = top;
				}
			}
		}
	}
}
