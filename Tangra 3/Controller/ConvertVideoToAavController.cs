using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Tangra.Controller
{
    public class ConvertVideoToAavController
    {
        private Form m_MainFormView;
        private VideoController m_VideoController;

        public ConvertVideoToAavController(Form mainFormView, VideoController videoController)
		{
			m_MainFormView = mainFormView;
			m_VideoController = videoController;
		}
    }
}
