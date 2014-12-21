using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Tangra.KweeVanWoerden
{
    public partial class frmConfigureRun : Form
    {
        internal int VariableStarIndex = 0;
        internal int ComparisonStarIndex = 0;
        internal int NumStars = 2;
        internal bool UseNormalisation = true;

        public frmConfigureRun()
        {
            InitializeComponent();
        }

        private void frmConfigureRun_Load(object sender, EventArgs e)
        {
            if (NumStars == 2)
            {
                rbComp3.Visible = false;
                rbComp4.Visible = false;
                rbVar3.Visible = false;
                rbVar4.Visible = false;
            }
            else if (NumStars == 3)
            {
                rbComp4.Visible = false;
                rbVar4.Visible = false;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            RadioButton[] CompRadioButtons = new RadioButton[] { rbComp1, rbComp2, rbComp3, rbComp4 };
            RadioButton[] VarRadioButtons = new RadioButton[] { rbVar1, rbVar2, rbVar3, rbVar4 };

            UseNormalisation = cbxNormalisation.Checked;

            ComparisonStarIndex = int.Parse(CompRadioButtons.Single(x => x.Checked).Tag.ToString());
            VariableStarIndex = int.Parse(VarRadioButtons.Single(x => x.Checked).Tag.ToString());

            if (ComparisonStarIndex == VariableStarIndex)
            {
                MessageBox.Show("Variable and Comparison star must be different stars.");
                return;
            }

            Close();
        }
    }
}
