using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tangra.Controller;
using Tangra.Model.Video;
using Tangra.Model.VideoOperations;
using Tangra.Video;

namespace Tangra.View.CustomRenderers.AavTimeAnalyser
{
    public class AavStatusChannelOnlyRenderer : ICustomRenderer
    {
        private AstroDigitalVideoStream m_AAV;

        public AavStatusChannelOnlyRenderer(AstroDigitalVideoStream aav)
        {
            m_AAV = aav;
        }

        public void ShowModal(IVideoController videoController)
        {
            var frm = new frmAavStatusChannelOnlyView((VideoController)videoController, m_AAV);
            ((VideoController)videoController).ShowForm(frm);
        }
    }
}
