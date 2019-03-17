using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tangra.View.CustomRenderers.AavTimeAnalyser
{
    public partial class frmChooseSubset : Form
    {
        public frmChooseSubset()
        {
            InitializeComponent();
        }

        public int From { get; private set; }
        public int To { get; private set; }

        public frmChooseSubset(AavTimeAnalyser timeAnalyser)
            : this()
        {
            nudFrom.Minimum = 0;
            nudFrom.Maximum = timeAnalyser.Entries.Count - 1;
            nudFrom.Value = 0;

            nudTo.Minimum = 0;
            nudTo.Maximum = timeAnalyser.Entries.Count - 1;
            nudTo.Value = timeAnalyser.Entries.Count - 1;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (nudTo.Value < nudFrom.Value)
            {
                MessageBox.Show("From index must be smaller than To index");
                return;
            }

            From = (int)nudFrom.Value;
            To = (int)nudTo.Value;
            DialogResult = DialogResult.OK;
        }
    }
}
