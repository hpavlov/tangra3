/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Tangra.Controls
{
    public class SmoothPanel2 : Panel
    {
       private const BufferedGraphics NO_MANAGED_BACK_BUFFER = null;

        BufferedGraphicsContext GraphicManager;
        BufferedGraphics ManagedBackBuffer;

        public delegate void PaintCallback(Graphics g);

        public PaintCallback m_OnPaint = null;

        public SmoothPanel2()
        {
            GraphicManager = BufferedGraphicsManager.Current;
            GraphicManager.MaximumBuffer = new Size(this.Width + 1, this.Height + 1);
            ManagedBackBuffer = GraphicManager.Allocate(this.CreateGraphics(), ClientRectangle);

            //SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
        }

        protected override void Dispose(bool disposing)
        {
            MemoryCleanup();

            base.Dispose(disposing);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (m_OnPaint != null)
                m_OnPaint(ManagedBackBuffer.Graphics);

            ManagedBackBuffer.Render(e.Graphics);
        }

        protected override void OnResize(EventArgs eventargs)
        {
            if (ManagedBackBuffer != NO_MANAGED_BACK_BUFFER) ManagedBackBuffer.Dispose();

            GraphicManager.MaximumBuffer = new Size(this.Width + 1, this.Height + 1);

            ManagedBackBuffer = GraphicManager.Allocate(this.CreateGraphics(), ClientRectangle);

            this.Refresh();
        }

        private void MemoryCleanup()
        {
            if (ManagedBackBuffer != NO_MANAGED_BACK_BUFFER)
            {
                ManagedBackBuffer.Dispose();
                ManagedBackBuffer = NO_MANAGED_BACK_BUFFER;
            }
        }
    }

    public class SmoothPanel : Panel
    {
        public SmoothPanel()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
        }
    }
}
