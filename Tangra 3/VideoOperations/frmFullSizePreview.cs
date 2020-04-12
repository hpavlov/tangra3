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
using Tangra.Controller;
using Tangra.Model.Astro;
using Tangra.Model.Image;

namespace Tangra.VideoOperations
{
	public partial class frmFullSizePreview : Form
	{
	    private VideoController m_VideoController;

        public frmFullSizePreview()
        {
            InitializeComponent();
        }

        public frmFullSizePreview(VideoController videoController)
            : this()
		{
			m_VideoController = videoController;
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
        internal static AstroImage CurrFrame;

	    public static event Action<Graphics> OnDrawOverlays;
        public static event Action<MouseEventArgs> OnMouseClicked;
	    public static event Action<MouseEventArgs> OnMouseMoved;
        public static event Action<MouseEventArgs> OnMouseDowned;
	    public static event Action<MouseEventArgs> OnMouseUped;
        public static event Action<KeyEventArgs> OnPreviewKeyDowned;

		public static void EnsureFullPreviewVisible(Pixelmap currFrame, Form parentForm, VideoController videoController)
		{
			lock(s_SyncRoot)
			{
				if (s_FullPreviewForm != null)
				{
					try
					{
						// This will test if the form has been disposed
						if (!s_FullPreviewForm.Visible && s_FullPreviewForm.Handle == IntPtr.Zero)
							s_FullPreviewForm = null;
					}
					catch(ObjectDisposedException)
					{
						s_FullPreviewForm = null;
					}
				}

                if (s_FullPreviewForm == null)
                {
                    s_FullPreviewForm = new frmFullSizePreview();
                    s_FullPreviewForm.Width = currFrame.Width + (s_FullPreviewForm.Width - s_FullPreviewForm.pictureBox.Width);
                    s_FullPreviewForm.Height = currFrame.Height + (s_FullPreviewForm.Height - s_FullPreviewForm.pictureBox.Height);
                    s_FullPreviewForm.Top = parentForm.Top;
                    s_FullPreviewForm.Left = parentForm.Right;
                    s_FullPreviewForm.StartPosition = FormStartPosition.Manual;
                }

                s_FullPreviewForm.pictureBox.Image = GetPreviewImage(currFrame, videoController);

                if (!s_FullPreviewForm.Visible)
                    s_FullPreviewForm.Show(parentForm);
                s_FullPreviewForm.Refresh();
			}
		}

        private static Bitmap GetPreviewImage(Pixelmap currFrame, VideoController videoController)
	    {
	        CurrFrame = new AstroImage(currFrame);
            Bitmap image = currFrame.CreateDisplayBitmapDoNotDispose();

            videoController.ApplyDynamicRangeAdjustments(image, currFrame);

	        if (OnDrawOverlays != null)
	        {
	            using (Graphics g = Graphics.FromImage(image))
	            {
	                OnDrawOverlays.Invoke(g);
	                g.Save();
	            }
	        }

	        return image;
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

        public static void Update(Pixelmap currFrame, VideoController videoController)
		{
			if (s_FullPreviewForm != null)
			{
				lock(s_SyncRoot)
				{
					if (s_FullPreviewForm != null)
					{

                        s_FullPreviewForm.pictureBox.Image = GetPreviewImage(currFrame, videoController);
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

	    public static void SetCursor(Cursor cursor)
	    {
            lock (s_SyncRoot)
            {
                if (s_FullPreviewForm != null)
                {
                    s_FullPreviewForm.pictureBox.Cursor = cursor;
                }
            }
	    }

        private void pictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (OnMouseClicked != null)
                OnMouseClicked.Invoke(e);
        }

        private void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (OnMouseMoved != null)
                OnMouseMoved.Invoke(e);
        }

        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (OnMouseDowned != null)
                OnMouseDowned.Invoke(e);
        }

        private void pictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (OnMouseUped != null)
                OnMouseUped.Invoke(e);
        }

        private void pictureBox_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

        }

        private void frmFullSizePreview_KeyDown(object sender, KeyEventArgs e)
        {
            if (OnPreviewKeyDowned != null)
                OnPreviewKeyDowned.Invoke(e);
        }

        private void frmFullSizePreview_KeyUp(object sender, KeyEventArgs e)
        {
            if (OnPreviewKeyDowned != null)
                OnPreviewKeyDowned.Invoke(e);
        }
	}
}
