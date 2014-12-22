using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.SDK;

namespace Tangra.KweeVanWoerden
{
    public partial class frmConfigureRun : Form
    {
        internal int VariableStarIndex = 0;
        internal int ComparisonStarIndex = 0;
        internal int NumStars = 2;
        internal bool UseNormalisation = true;
	    internal int FromFrameNo = 0;
		internal int ToFrameNo = 0;
	    internal ISingleMeasurement[] TargetData;

	    private float[] m_DisplayData;

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

	        tbarFrom.Minimum = FromFrameNo;
			tbarFrom.Maximum = ToFrameNo;
			tbarFrom.Value = FromFrameNo;

			tbarTo.Minimum = FromFrameNo;
			tbarTo.Maximum = ToFrameNo;
			tbarTo.Value = ToFrameNo;

	        m_DisplayData = TargetData.Select(x => x.Measurement - x.Background).ToArray();
	        DrawHistogram();
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

		private void DynamicRangeChanged(object sender, EventArgs e)
		{
			timer1.Enabled = false;
			timer1.Enabled = true;

			pnlSelectedRange.Visible = tbarFrom.Value < tbarTo.Value;
			if (tbarFrom.Value < tbarTo.Value)
			{
				pnlSelectedRange.Left = 15 + (int)(504 * (1.0 * tbarFrom.Value / tbarFrom.Maximum));
				pnlSelectedRange.Width = (int)(504 * (1.0 * (tbarTo.Value - tbarFrom.Value) / tbarTo.Maximum)) - 1;
			}
		}

		private void DrawHistogram()
		{
			if (picHistogram.Image != null)
				picHistogram.Image.Dispose();

			picHistogram.Image = new Bitmap(picHistogram.Width, picHistogram.Height);

			float maxVal = m_DisplayData.Max();
			int XGAP = 10;
			int YGAP = 10;

			using (Graphics g = Graphics.FromImage(picHistogram.Image))
			{
				float xScale = (picHistogram.Image.Width - 2 * XGAP) * 1.0f / (m_DisplayData.Length + 1);
				float yScale = (picHistogram.Image.Height - 2 * YGAP) * 1.0f / maxVal;

				g.FillRectangle(SystemBrushes.ControlDark, new Rectangle(0, 0, picHistogram.Image.Width, picHistogram.Image.Height));
				g.DrawRectangle(Pens.Black, XGAP, YGAP, picHistogram.Image.Width - 2 * XGAP + 1, picHistogram.Image.Height - 2 * YGAP);

				for (int i = 0; i < m_DisplayData.Length; i++)
				{
					float xFrom = XGAP + i * xScale + 1;
					float xSize = Math.Max(0.5f, xScale);

					float ySize = 2; // m_DisplayData[i] * yScale;
					float yFrom = picHistogram.Image.Height - YGAP - m_DisplayData[i] * yScale;

					g.FillRectangle(Brushes.LimeGreen, xFrom, yFrom, xSize, ySize);
				}

				g.Save();
			}

			picHistogram.Refresh();
		}

		private void timer1_Tick(object sender, EventArgs e)
		{

		}
    }
}
