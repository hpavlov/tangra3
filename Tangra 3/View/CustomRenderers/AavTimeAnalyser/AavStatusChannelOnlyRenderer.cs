using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
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
            frm.StartPosition = FormStartPosition.CenterParent;

            ((VideoController)videoController).ShowForm(frm);
        }
    }
}
