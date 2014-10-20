/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Video;

namespace Tangra.Controller
{
    public class VideoOperationController
    {
        private FramePlayer m_FramePlayer;
        private Form m_MainFormView;

        public VideoOperationController(Form mainFormView, FramePlayer framePlayer)
		{
            m_FramePlayer = framePlayer;
			m_MainFormView = mainFormView;
		}
    }
}
