using System;/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;
using Tangra.Controller;
using Tangra.Model.Config;
using Tangra.Model.Context;
using Tangra.Model.ImageTools;
using Tangra.Model.VideoOperations;
using Tangra.VideoOperations.Astrometry.Engine;

namespace Tangra.ImageTools
{
    internal class FrameSizer : ImageTool
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
	    private static string s_DisplayText = "OSD EXCLUSION AREA";
        
        protected Rectangle m_UserFrame;
        private bool m_Dragging = false;

        private SizerState m_State = SizerState.Normal;
        private SizerState m_StateTransition = SizerState.Normal;

		public FrameSizer(VideoController videoController)
		{
			m_VideoController = videoController;
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
				// Load from saved one or default 
				m_UserFrame = new Rectangle(AstrometryContext.Current.OSDRectToExclude.X, AstrometryContext.Current.OSDRectToExclude.Y,
											AstrometryContext.Current.OSDRectToExclude.Width, AstrometryContext.Current.OSDRectToExclude.Height);
        }

        public override void Deactivate()
        {
            if (m_UserFrame != Rectangle.Empty)
            {
                AstrometryContext.Current.OSDRectToExclude = new Rectangle(m_UserFrame.X, m_UserFrame.Y, m_UserFrame.Width, m_UserFrame.Height);
				TangraConfig.Settings.OSDSizes.AddOrUpdateOSDRectangleForFrameSize(TangraContext.Current.FrameWidth, TangraContext.Current.FrameWidth, AstrometryContext.Current.OSDRectToExclude);
	            TangraConfig.Settings.Save();
            }

            m_UserFrame = Rectangle.Empty;
        }

        public override void MouseLeave()
        {
            m_VideoController.SetPictureBoxCursor(Cursors.Arrow);
            m_State = SizerState.Normal;
            m_StateTransition = SizerState.Normal;
        }

		public override void MouseMove(Point location)
        {
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

			if (m_Dragging)
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
            if (m_StateTransition != SizerState.Normal)
            {
                m_Dragging = true;
                m_State = m_StateTransition;

	            TriggerRedraw();
            }
        }

		public override void MouseUp(Point location)
        {
            if (m_Dragging)
            {
                m_Dragging = false;

                m_State = SizerState.Normal;
                m_StateTransition = SizerState.Normal;

                m_VideoController.SetPictureBoxCursor(Cursors.Arrow);
				AstrometryContext.Current.OSDRectToExclude = new Rectangle(m_UserFrame.X, m_UserFrame.Y, m_UserFrame.Width, m_UserFrame.Height);

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
			if (m_UserFrame != Rectangle.Empty)
			{
				Pen osdAreaPen = m_Dragging ? Pens.Purple : Pens.Blue;
				Brush osdAreaBrush = m_Dragging ? Brushes.Purple : Brushes.Blue;

				Rectangle rect = m_UserFrame;
				rect.Inflate(-1, -1);
				g.DrawRectangle(osdAreaPen, rect);
				g.FillRectangle(new SolidBrush(Color.FromArgb(20, 0, 0, 255)), m_UserFrame);

				SizeF size = g.MeasureString(s_DisplayText, s_Font);
				g.DrawString(s_DisplayText, s_Font, osdAreaBrush, m_UserFrame.Left + 1, m_UserFrame.Top + 1 - size.Height);
			}
		}

		public override void MouseClick(ObjectClickEventArgs e)
		{ }
    }
}
