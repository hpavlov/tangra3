using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Tangra.Model.Controls
{
    public partial class ucColorPicker : UserControl
    {
        public Color SelectedColor
        {
            get { return pnlColor.BackColor; }
            set { pnlColor.BackColor = value; }
        }

        public event EventHandler SelectedColorChanged;

        public ucColorPicker()
        {
            InitializeComponent();

            pnlColor.BackColor = Color.White;
        }

        private void btnPick_Click(object sender, EventArgs e)
        {
            colorDialog.Color = SelectedColor;
            if (colorDialog.ShowDialog(this.ParentForm) == DialogResult.OK)
            {
                SelectedColor = colorDialog.Color;
                if (SelectedColorChanged != null)
                    SelectedColorChanged(this, EventArgs.Empty);
            }
        }
    }
}
