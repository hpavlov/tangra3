using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Tangra.VideoOperations.Spectroscopy
{
    public partial class ucSpectroscopy : UserControl
    {
        private VideoSpectroscopyOperation m_VideoOperation;

        public ucSpectroscopy()
        {
            InitializeComponent();
        }

        public ucSpectroscopy(VideoSpectroscopyOperation videoOperation)
            : this()
        {
            m_VideoOperation = videoOperation;
        }
    }
}
