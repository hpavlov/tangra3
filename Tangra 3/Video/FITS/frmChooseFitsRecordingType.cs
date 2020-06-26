using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tangra.Video.FITS
{
    public partial class frmChooseFitsRecordingType : Form
    {
        public bool IsFitsVideo;

        public frmChooseFitsRecordingType()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            IsFitsVideo = rbVideo.Checked;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
