using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Model.Image;
using Tangra.VideoTools;

namespace Tangra.Video.FITS
{
    public partial class frmFitsHeaderPopup : Form
    {
        public frmFitsHeaderPopup()
        {
            InitializeComponent();
        }

        public void ShowStatus(FrameStateData frameState)
        {
            var dataList = frameState.AdditionalProperties.Select(kvp => new TagValuePair(kvp.Key, Convert.ToString(kvp.Value))).ToList();
            dataGridView.DataSource = dataList;
        }
    }
}
