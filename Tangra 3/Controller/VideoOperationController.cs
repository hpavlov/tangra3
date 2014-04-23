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
