using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Tangra.VideoOperations.LightCurves
{
    public partial class frmConfirmDay : Form
    {
        public frmConfirmDay()
        {
            InitializeComponent();
        }

        internal DateTime? SelectedDay;

        private void btnOK_Click(object sender, EventArgs e)
        {
            SelectedDay = dateTimePicker.Value.Date;

            DialogResult = DialogResult.OK;

            Close();
        }

        private void frmConfirmDay_Load(object sender, EventArgs e)
        {
            if (SelectedDay.HasValue && SelectedDay.Value.Year > 1900)
                dateTimePicker.Value = SelectedDay.Value;
            else
                dateTimePicker.Value = DateTime.Today;
        }
    }
}
