using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Model.ImageTools;

namespace Tangra.ImageTools
{
    public class ArrowTool : ImageTool
    {
        public override void Activate()
        {
            base.Activate();

            //m_Host.SetPictureBoxCursor(Cursors.Arrow);
        }
    }
}
