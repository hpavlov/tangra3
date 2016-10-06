using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Model.Astro;

namespace Tangra.Controller
{
    public class ConvertVideoToFitsController
    {
		private Form m_MainFormView;
		private VideoController m_VideoController;
        private Rectangle m_RegionOfInterest;

        public ConvertVideoToFitsController(Form mainFormView, VideoController videoController)
		{
			m_MainFormView = mainFormView;
			m_VideoController = videoController;			
		}

        internal void StartExport(string fileName, bool fitsCube, Rectangle roi)
        {
            m_RegionOfInterest = roi;
            // TODO: Start recording the file
           
        }

        internal void ProcessFrame(int frameNo, AstroImage astroImage)
        {
            // TODO
        }

        internal void FinishExport()
        {
            
        }
    }
}
