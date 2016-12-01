using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Controller;

namespace Tangra.VideoOperations.ConvertVideoToAav
{
    public partial class ucConvertVideoToAav : UserControl
    {
        private ConvertVideoToAavOperation m_Operation;
        private VideoController m_VideoController;

        public ucConvertVideoToAav()
        {
            InitializeComponent();
        }

        public ucConvertVideoToAav(ConvertVideoToAavOperation operation, VideoController videoController)
            : this()
        {
            m_Operation = operation;
            m_VideoController = videoController;
        }


    }
}
