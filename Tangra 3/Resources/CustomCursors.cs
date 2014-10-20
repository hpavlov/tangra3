/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Tangra.Resources
{
    public static class CustomCursors
    {
        public static Cursor PanCursor;
        public static Cursor PanEnabledCursor;
        public static Cursor RotateCursor;
        public static Cursor RotateEnabledCursor;

        static CustomCursors()
        {
            using (var str = new MemoryStream(Tangra.Properties.Resources.PanCursor))
            {
                PanCursor = new Cursor(str);
            }

            using (var str = new MemoryStream(Tangra.Properties.Resources.PanEnabledCursor))
            {
                PanEnabledCursor = new Cursor(str);
            }

            using (var str = new MemoryStream(Tangra.Properties.Resources.RotateCursor))
            {
                RotateCursor = new Cursor(str);
            }

            using (var str = new MemoryStream(Tangra.Properties.Resources.RotateEnabledCursor))
            {
                RotateEnabledCursor = new Cursor(str);
            }
        }

    }
}
