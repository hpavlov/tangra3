using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tangra.VideoOperations.ConvertVideoToAav
{
    public partial class frmAdditionalHeaders : Form
    {
        public List<HeaderValuePair> Headers = new List<HeaderValuePair>();

        public frmAdditionalHeaders()
        {
            InitializeComponent();
            
            Headers.Add(new HeaderValuePair("LATITUDE", ""));
            Headers.Add(new HeaderValuePair("LONGITUDE", ""));
            Headers.Add(new HeaderValuePair("OBSERVER", ""));
            Headers.Add(new HeaderValuePair("TELESCOP", ""));
            Headers.Add(new HeaderValuePair("OBJECT", ""));
            Headers.Add(new HeaderValuePair("RA_OBJ", ""));
            Headers.Add(new HeaderValuePair("DEC_OBJ", ""));
            Headers.Add(new HeaderValuePair("", ""));
            Headers.Add(new HeaderValuePair("", ""));
            Headers.Add(new HeaderValuePair("", ""));

            dgvHeaders.DataSource = Headers;
        }
    }
}
