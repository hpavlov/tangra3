/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Controller;
using Tangra.Model.Config;
using Tangra.Model.Context;
using Tangra.Model.ImageTools;
using Tangra.VideoOperations.Astrometry.Engine;

namespace Tangra.ImageTools
{
    internal class RoiSelector : ImageTool
    {
        protected enum SizerState
        {
            Normal,
            ResizingUp,
            ResizingDown,
            ResizingLeft,
            ResizingRight,
            ResizingTopLeft,
            ResizingBottomRight,
            ResizingTopRight,
            ResizingBottomLeft
        }

	    protected VideoController m_VideoController;

        private static int s_FrameWidth = 3;
        private bool m_DraggingCorner;

		private static string s_DisplayText = "ROI";
        
        private Rectangle m_UserFrame;

        private SizerState m_State = SizerState.Normal;
        private SizerState m_StateTransition = SizerState.Normal;

		public event OnAreaChanged AreaChanged;

        public Rectangle SelectedROI
        {
            get { return m_UserFrame; }
        }

        private bool m_Enabled;

        public bool Enabled
        {
            get { return m_Enabled; }
            set
            {
                m_Enabled = value;
                TriggerRedraw();
            }
        }

        public RoiSelector(VideoController videoController)
		{
			m_VideoController = videoController;
		}

		protected void SetUserFrame(Rectangle userFrame)
		{
			m_UserFrame.X = userFrame.X;
			m_UserFrame.Y = userFrame.Y;
			m_UserFrame.Width = userFrame.Width;
			m_UserFrame.Height = userFrame.Height;
		}

        public override ZoomImageBehaviour ZoomBehaviour
        {
            get
            {
                return ZoomImageBehaviour.None;
            }
        }

        public override void Activate()
        {
			m_VideoController.SetPictureBoxCursor(Cursors.Arrow);
            m_State = SizerState.Normal;
            m_StateTransition = SizerState.Normal;

            if (m_UserFrame == Rectangle.Empty)
            {
                int left = TangraContext.Current.FrameWidth / 4;
                int top = TangraContext.Current.FrameHeight / 4;
                m_UserFrame = new Rectangle(left, top, 2 * left, 2 * top);
            }
        }

        public override void Deactivate()
        {
            m_UserFrame = Rectangle.Empty;
        }

        public override void MouseLeave()
        {
            if (!m_Enabled) return;

            m_VideoController.SetPictureBoxCursor(Cursors.Arrow);
            m_State = SizerState.Normal;
            m_StateTransition = SizerState.Normal;
        }

		public override void MouseMove(Point location)
        {
            if (!m_Enabled) return;

			int x1 = location.X - m_UserFrame.X;
			int y1 = location.Y - m_UserFrame.Y;

            if (m_State == SizerState.Normal)
            {
                if (x1 >= 0 && x1 < s_FrameWidth && y1 >= 0 && y1 < s_FrameWidth)
                {
                    if (m_StateTransition != SizerState.ResizingBottomRight)
                    {
                        m_VideoController.SetPictureBoxCursor(Cursors.SizeNWSE);
                        m_StateTransition = SizerState.ResizingBottomRight;
                    }
                }
                else if (x1 >= s_FrameWidth && x1 <= m_UserFrame.Width - s_FrameWidth && y1 >= 0 && y1 < s_FrameWidth)
                {
                    if (m_StateTransition != SizerState.ResizingDown)
                    {
						m_VideoController.SetPictureBoxCursor(Cursors.SizeNS);
                        m_StateTransition = SizerState.ResizingDown;
                    }
                }
                else if (x1 >= 0 && x1 < s_FrameWidth && y1 >= m_UserFrame.Height - s_FrameWidth && y1 < m_UserFrame.Height)
                {
                    if (m_StateTransition != SizerState.ResizingBottomLeft)
                    {
						m_VideoController.SetPictureBoxCursor(Cursors.SizeNESW);
                        m_StateTransition = SizerState.ResizingBottomLeft;
                    }
                }
                else if (x1 >= 0 && x1 < s_FrameWidth && y1 > 0 && y1 <= m_UserFrame.Height)
                {
                    if (m_StateTransition != SizerState.ResizingLeft)
                    {
						m_VideoController.SetPictureBoxCursor(Cursors.SizeWE);
                        m_StateTransition = SizerState.ResizingLeft;
                    }
                }
                else if (
                    x1 > m_UserFrame.Width - s_FrameWidth && x1 <= m_UserFrame.Width &&
                    y1 > m_UserFrame.Height - s_FrameWidth && y1 < m_UserFrame.Height)
                {
                    if (m_StateTransition != SizerState.ResizingTopLeft)
                    {
						m_VideoController.SetPictureBoxCursor(Cursors.SizeNWSE);
                        m_StateTransition = SizerState.ResizingTopLeft;
                    }
                }
               else if (
                    x1 >= s_FrameWidth && x1 <= m_UserFrame.Width - s_FrameWidth &&
                    y1 > m_UserFrame.Height - s_FrameWidth && y1 < m_UserFrame.Height)
                {
                    if (m_StateTransition != SizerState.ResizingUp)
                    {
						m_VideoController.SetPictureBoxCursor(Cursors.SizeNS);
                        m_StateTransition = SizerState.ResizingUp;
                    }
                }
                else if (x1 >= 0 && x1 < s_FrameWidth && y1 > m_UserFrame.Height - s_FrameWidth && y1 < m_UserFrame.Height)
                {
                    if (m_StateTransition != SizerState.ResizingTopRight)
                    {
						m_VideoController.SetPictureBoxCursor(Cursors.SizeNESW);
                        m_StateTransition = SizerState.ResizingTopRight;
                    }
                }
                else if (x1 >= m_UserFrame.Width - s_FrameWidth && x1 <= m_UserFrame.Width && y1 >= 0 && y1 < m_UserFrame.Height)
                {
                    if (m_StateTransition != SizerState.ResizingRight)
                    {
						m_VideoController.SetPictureBoxCursor(Cursors.SizeWE);
                        m_StateTransition = SizerState.ResizingRight;
                    }
                }
                else
                {
                    if (m_StateTransition != SizerState.Normal)
                    {
						m_VideoController.SetPictureBoxCursor(Cursors.Arrow);
                        m_StateTransition = SizerState.Normal;
                    }
                }

                if (x1 < 0 || x1 > m_UserFrame.Width || y1 < 0 || y1 > m_UserFrame.Height)
                    return;
            }

			int posX = location.X > TangraContext.Current.FrameWidth ? TangraContext.Current.FrameWidth : location.X;
			int posY = location.Y > TangraContext.Current.FrameHeight ? TangraContext.Current.FrameHeight : location.Y;
            if (posX < 0) posX = 0;
            if (posY < 0) posY = 0;

			if (m_DraggingCorner)
			{
				if (m_State == SizerState.ResizingDown)
				{
					m_UserFrame.Height = m_UserFrame.Bottom - posY;
					m_UserFrame.Y = posY;
				}
				else if (m_State == SizerState.ResizingBottomRight)
				{
					m_UserFrame.Height = m_UserFrame.Bottom - posY;
					m_UserFrame.Width = m_UserFrame.Right - posX;
					m_UserFrame.X = posX;
					m_UserFrame.Y = posY;
				}
				else if (m_State == SizerState.ResizingBottomLeft)
				{
                    m_UserFrame.Width = m_UserFrame.Right - posX;
                    m_UserFrame.X = posX;
                    m_UserFrame.Height = posY - m_UserFrame.Y;
				}
				else if (m_State == SizerState.ResizingUp)
				{
					m_UserFrame.Height = posY - m_UserFrame.Top;
				}
				else if (m_State == SizerState.ResizingTopRight)
				{
					m_UserFrame.Height = posY - m_UserFrame.Top;
					m_UserFrame.Width = m_UserFrame.Right - posX;
					m_UserFrame.X = posX;
				}
				else if (m_State == SizerState.ResizingTopLeft)
				{
					m_UserFrame.Height = posY - m_UserFrame.Top;
					m_UserFrame.Width = posX - m_UserFrame.Left;
				}
				else if (m_State == SizerState.ResizingLeft)
				{
                    m_UserFrame.Width = m_UserFrame.Right - posX;
                    m_UserFrame.X = posX;
				}
				else if (m_State == SizerState.ResizingRight)
				{
                    m_UserFrame.Width = posX - m_UserFrame.Left;
				}

				TriggerRedraw();
			}
        }

        public override void MouseDown(Point location)
        {
            if (!m_Enabled) return;

            if (m_StateTransition != SizerState.Normal)
            {
                m_DraggingCorner = true;
                m_State = m_StateTransition;

	            TriggerRedraw();
            }
        }

		public override void MouseUp(Point location)
        {
            if (!m_Enabled) return;

            if (m_DraggingCorner)
            {
                m_DraggingCorner = false;

                m_State = SizerState.Normal;
                m_StateTransition = SizerState.Normal;

                m_VideoController.SetPictureBoxCursor(Cursors.Arrow);

				if (AreaChanged != null)
					AreaChanged();
	            
				TriggerRedraw();
            }
        }

		private void TriggerRedraw()
		{
			m_VideoController.RedrawCurrentFrame(false, true);
		}

	    private static Font s_Font = new Font(FontFamily.GenericSerif, 8, FontStyle.Bold);

		public override void PostDraw(Graphics g)
		{
			if (m_Enabled && m_UserFrame != Rectangle.Empty)
			{
                Pen osdAreaPen = m_DraggingCorner ? Pens.DeepPink : Pens.Lime;
                Brush osdAreaBrush = m_DraggingCorner ? Brushes.DeepPink : Brushes.Lime;

                Rectangle rect = m_UserFrame;
                rect.Inflate(-1, -1);
                g.DrawRectangle(osdAreaPen, rect);
                g.FillRectangle(new SolidBrush(Color.FromArgb(20, 0, 255, 0)), m_UserFrame);

                g.DrawString(s_DisplayText, s_Font, osdAreaBrush, m_UserFrame.Left + 1, m_UserFrame.Top + 1);
			}
		}

		public override void MouseClick(ObjectClickEventArgs e)
		{ }
    }
}
