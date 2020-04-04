/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Tangra.Controls
{
    public enum ImageDisplayMode
    {
        Scroll,
        Shrink
    }

    public partial class ImagePanel : UserControl
    {
        public ImagePanel()
        {
            InitializeComponent();

            // Set the value of the double-buffering style bits to true.
            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
              ControlStyles.UserPaint | ControlStyles.ResizeRedraw |
              ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);

            this.DisplayMode = ImageDisplayMode.Shrink;
            this.MouseWheel += OnMouseWheel;
        }

        private int m_ViewRectWidth;
        private int m_ViewRectHeight;
        private float m_Scale;

        private Size m_CanvasSize = new Size(60, 40);

        public Size CanvasSize
        {
            get { return m_CanvasSize; }
            set
            {
                m_CanvasSize = value;
                RecalculateScaleAndScrollbars();
                Invalidate();
            }
        }

        private readonly object m_SyncLock = new object();

        private Bitmap m_Image;

        public Bitmap Image
        {
            get { return m_Image; }
            set 
            {
                lock (m_SyncLock)
                {
                    m_Image = value;
                }

                RecalculateScaleAndScrollbars();
                Invalidate();
            }
        }

        InterpolationMode interMode = InterpolationMode.HighQualityBilinear;
        public InterpolationMode InterpolationMode
        {
            get{return interMode;}
            set{interMode=value;}
        }

        private ImageDisplayMode m_DisplayMode;

        public ImageDisplayMode DisplayMode
        {
            get { return m_DisplayMode; }
            set
            {
                if (m_DisplayMode == value)
                {
                    return;
                }

                m_DisplayMode = value;

                RecalculateScaleAndScrollbars();
                Invalidate();
            }
        }

        private void OnMouseWheel(object sender, MouseEventArgs e)
        {
            if (m_DisplayMode == ImageDisplayMode.Scroll)
            {
                // TODO: Implement mouse wheel zoom in/out
                // m_ZoomScale += (e.Delta / 5000.0f);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            RecalculateScaleAndScrollbars();
            base.OnLoad(e);
        }

        protected override void OnResize(EventArgs e)
        {
            RecalculateScaleAndScrollbars();
            base.OnResize(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if(m_Image != null)
            {
                lock (m_SyncLock)
                {
                    if (m_Image != null)
                    {
                        Rectangle srcRect, distRect;

                        Point pt = new Point((int)(hScrollBar1.Value), (int)(vScrollBar1.Value));
                        if (m_CanvasSize.Width < m_ViewRectWidth && m_CanvasSize.Height < m_ViewRectHeight)
                        {
                            srcRect = new Rectangle(0, 0, m_CanvasSize.Width, m_CanvasSize.Height); // view all image
                            if (m_ViewRectWidth == m_CanvasSize.Width + 1 && m_ViewRectHeight == m_CanvasSize.Height + 1)
                            {
                                try
                                {
                                    e.Graphics.DrawImage(m_Image, 0, 0);
                                }
                                catch (ArgumentException)
                                {
                                }

                                return;
                            }
                        }
                        else
                        {
                            srcRect = new Rectangle(pt, new Size((int)(m_ViewRectWidth), (int)(m_ViewRectHeight))); // view a portion of image
                        }

                        if (m_Scale < 1 && this.DisplayMode == ImageDisplayMode.Shrink)
                        {
                            Matrix mxs = new Matrix();
                            mxs.Scale(m_Scale, m_Scale);

                            Graphics gs = e.Graphics;
                            gs.InterpolationMode = interMode;
                            gs.Transform = mxs;
                            try
                            {
                                gs.DrawImage(m_Image, new PointF(0, 0));
                            }
                            catch (ArgumentException)
                            { }

                            return;
                        }

                        distRect = new Rectangle((int)(-srcRect.Width / 2), -srcRect.Height / 2, srcRect.Width, srcRect.Height); // the center of apparent image is on origin

                        Matrix mx = new Matrix(); // create an identity matrix
                        mx.Translate(m_ViewRectWidth / 2.0f, m_ViewRectHeight / 2.0f, MatrixOrder.Append); // move image to view window center
                        Graphics g = e.Graphics;
                        g.InterpolationMode = interMode;
                        g.Transform = mx;
                        try
                        {
                            g.DrawImage(m_Image, distRect, srcRect, GraphicsUnit.Pixel);
                        }
                        catch (ArgumentException)
                        { }
                    }
                }
            }

        }

        private void RecalculateScaleAndScrollbars()
        {
            DisplayScrollbar();
            SetScrollbarValues();

            var vertScale = m_ViewRectHeight * 1.0f / m_CanvasSize.Height;
            var horScale = m_ViewRectWidth * 1.0f / m_CanvasSize.Width;
            m_Scale = Math.Min(vertScale, horScale);
        }

        private void DisplayScrollbar()
        {
            m_ViewRectWidth = this.Width;
            m_ViewRectHeight = this.Height;

            if (m_Image != null)
            {
                lock (m_SyncLock)
                {
                    if (m_Image != null)
                    {
                        try
                        {
                            m_CanvasSize = m_Image.Size;
                        }
                        catch (ArgumentException)
                        {
                            return;
                        }
                    }
                }
            }

            // If the zoomed image is wider than view window, show the HScrollBar and adjust the view window
            if (m_DisplayMode != ImageDisplayMode.Scroll || m_ViewRectWidth > m_CanvasSize.Width)
            {
                hScrollBar1.Visible = false;
	            m_ViewRectHeight = Height;
            }
            else
            {
                hScrollBar1.Visible = true;
                m_ViewRectHeight = Height - hScrollBar1.Height;
            }

            // If the zoomed image is taller than view window, show the VScrollBar and adjust the view window
            if (m_DisplayMode != ImageDisplayMode.Scroll || m_ViewRectHeight > m_CanvasSize.Height)
            {
                vScrollBar1.Visible = false;
				m_ViewRectWidth = Width;
            }
            else
            {
                vScrollBar1.Visible = true;
                m_ViewRectWidth = Width - vScrollBar1.Width;
            }

			// Check for horizontal scrollbar one more time as the width may have changed due to showing a vertical scroll bar
            if (m_DisplayMode != ImageDisplayMode.Scroll || m_ViewRectWidth > m_CanvasSize.Width)
			{
				hScrollBar1.Visible = false;
				m_ViewRectHeight = Height;
			}
			else
			{
				hScrollBar1.Visible = true;
				m_ViewRectHeight = Height - hScrollBar1.Height;
			}

            // Set up scrollbars
            hScrollBar1.Location = new Point(0, Height - hScrollBar1.Height);
            hScrollBar1.Width = m_ViewRectWidth;
            vScrollBar1.Location = new Point(Width - vScrollBar1.Width, 0);
            vScrollBar1.Height = m_ViewRectHeight;
        }

        private void SetScrollbarValues()
        {
            // Set the Maximum, Minimum, LargeChange and SmallChange properties.
            this.vScrollBar1.Minimum = 0;
            this.hScrollBar1.Minimum = 0;

            // If the offset does not make the Maximum less than zero, set its value. 
            if ((m_CanvasSize.Width - m_ViewRectWidth) > 0)
            {
                this.hScrollBar1.Maximum =(int)( m_CanvasSize.Width) - m_ViewRectWidth;
            }
            // If the VScrollBar is visible, adjust the Maximum of the 
            // HSCrollBar to account for the width of the VScrollBar.  
            if (this.vScrollBar1.Visible)
            {
                this.hScrollBar1.Maximum += this.vScrollBar1.Width;
            }
            this.hScrollBar1.LargeChange = this.hScrollBar1.Maximum / 10;
            this.hScrollBar1.SmallChange = this.hScrollBar1.Maximum / 20;

            // Adjust the Maximum value to make the raw Maximum value 
            // attainable by user interaction.
            this.hScrollBar1.Maximum += this.hScrollBar1.LargeChange;

            // If the offset does not make the Maximum less than zero, set its value.    
            if ((m_CanvasSize.Height - m_ViewRectHeight) > 0)
            {
                this.vScrollBar1.Maximum = (int)(m_CanvasSize.Height) - m_ViewRectHeight;
            }

            // If the HScrollBar is visible, adjust the Maximum of the 
            // VSCrollBar to account for the width of the HScrollBar.
            if (this.hScrollBar1.Visible)
            {
                this.vScrollBar1.Maximum += this.hScrollBar1.Height;
            }
            this.vScrollBar1.LargeChange = this.vScrollBar1.Maximum / 10;
            this.vScrollBar1.SmallChange = this.vScrollBar1.Maximum / 20;

            // Adjust the Maximum value to make the raw Maximum value 
            // attainable by user interaction.
            this.vScrollBar1.Maximum += this.vScrollBar1.LargeChange;
        }

		public Point GetImageLocation(Point mousePoint)
		{
		    if (m_Image != null)
		    {
		        lock (m_SyncLock)
		        {
		            if (m_Image != null)
		            {
                        try
                        {
                            if (m_DisplayMode == ImageDisplayMode.Scroll || m_Scale >= 1.0)
                            {
                                int x = mousePoint.X + (hScrollBar1.Visible ? hScrollBar1.Value : (this.Image != null && !vScrollBar1.Visible ? (this.Image.Width - m_ViewRectWidth) / 2 : 0));
                                int y = mousePoint.Y + (vScrollBar1.Visible ? vScrollBar1.Value : (this.Image != null && !hScrollBar1.Visible ? (this.Image.Height - m_ViewRectHeight) / 2 : 0));

                                return new Point(x, y);
                            }
                            else if (m_DisplayMode == ImageDisplayMode.Shrink)
                            {
                                int x = (int)Math.Round(mousePoint.X / m_Scale);
                                int y = (int)Math.Round(mousePoint.Y / m_Scale);

                                return new Point(x, y);
                            }
                        }
                        catch (ArgumentException)
                        { }
		            }
		        }
		    }

            return new Point(mousePoint.X, mousePoint.Y);
		}

        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            this.Invalidate();
        }
    }
}
