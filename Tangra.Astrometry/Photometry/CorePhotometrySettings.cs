/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tangra.Photometry
{
    public class CorePhotometrySettings
    {
        public static CorePhotometrySettings Default = new CorePhotometrySettings();

        public float RejectionBackgroundPixelsStdDev = 6.0f;
    	public int MatrixSizeForCalibratedPhotometry = 17;
    }
}
