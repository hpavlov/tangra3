using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.SDK;

namespace Tangra.KweeVanWoerden
{
    public partial class frmConfigureRun : Form
    {
        internal ILightCurveDataProvider DataProvider;

        internal int VariableStarIndex = 0;
        internal int ComparisonStarIndex = 0;
        internal int NumStars = 2;
        internal bool UseNormalisation = true;
	    internal int FromFrameNo = 0;
		internal int ToFrameNo = 0;
	    internal ISingleMeasurement[] TargetData;

        internal int IncludeDataFrom = 0;
        internal int IncludeDataTo = 0;

	    private float[] m_DisplayData;

        private Color m_Target1Color;
        private Color m_Target2Color;
        private Color m_Target3Color;
        private Color m_Target4Color;
        private Brush m_Target1Brush;
        private Brush m_Target2Brush;
        private Brush m_Target3Brush;
        private Brush m_Target4Brush;

        public frmConfigureRun()
        {
            InitializeComponent();
        }

        public override object InitializeLifetimeService()
        {
            // The lifetime of the object is managed by the add-in
            return null;
        }

        private void frmConfigureRun_Load(object sender, EventArgs e)
        {
            ITangraDrawingSettings settings = DataProvider.GetTangraDrawingSettings();
            m_Target1Color = settings.Target1Color;
            m_Target2Color = settings.Target2Color;
            m_Target3Color = settings.Target3Color;
            m_Target4Color = settings.Target4Color;
            m_Target1Brush = new SolidBrush(m_Target1Color);
            m_Target2Brush = new SolidBrush(m_Target2Color);
            m_Target3Brush = new SolidBrush(m_Target3Color);
            m_Target4Brush = new SolidBrush(m_Target4Color);

            pb1.BackColor = m_Target1Color;
            pb2.BackColor = m_Target2Color;
            pb3.BackColor = m_Target3Color;
            pb4.BackColor = m_Target4Color;
            pb1c.BackColor = m_Target1Color;
            pb2c.BackColor = m_Target2Color;
            pb3c.BackColor = m_Target3Color;
            pb4c.BackColor = m_Target4Color;

            if (NumStars == 2)
            {
                rbComp3.Visible = false;
                rbComp4.Visible = false;
                rbVar3.Visible = false;
                rbVar4.Visible = false;
                pb3.Visible = false;
                pb3c.Visible = false;
                pb4.Visible = false;
                pb4c.Visible = false;
            }
            else if (NumStars == 3)
            {
                rbComp4.Visible = false;
                rbVar4.Visible = false;
                pb4.Visible = false;
                pb4c.Visible = false;
            }

	        tbarFrom.Minimum = FromFrameNo;
			tbarFrom.Maximum = ToFrameNo;
			tbarFrom.Value = FromFrameNo;

			tbarTo.Minimum = FromFrameNo;
			tbarTo.Maximum = ToFrameNo;
			tbarTo.Value = ToFrameNo;

	        m_DisplayData = TargetData.Where(x => x.IsSuccessful).Select(x => x.Measurement).ToArray();

            IncludeDataFrom = 0;
            IncludeDataTo = m_DisplayData.Length - 1;

	        DrawLightCurve();
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

		private void DrawLightCurve()
		{
			if (picHistogram.Image != null)
				picHistogram.Image.Dispose();

			picHistogram.Image = new Bitmap(picHistogram.Width, picHistogram.Height);

			float maxVal = m_DisplayData.Max();
            float minVal = m_DisplayData.Min();
			int XGAP = 10;
			int YGAP = 10;

			using (Graphics g = Graphics.FromImage(picHistogram.Image))
			{
				float xScale = (picHistogram.Image.Width - 2 * XGAP) * 1.0f / (m_DisplayData.Length + 1);
				float yScale = (picHistogram.Image.Height - 2 * YGAP) * 1.0f / (maxVal - minVal);

				g.FillRectangle(SystemBrushes.ControlDark, new Rectangle(0, 0, picHistogram.Image.Width, picHistogram.Image.Height));
				g.DrawRectangle(Pens.Black, XGAP, YGAP, picHistogram.Image.Width - 2 * XGAP + 1, picHistogram.Image.Height - 2 * YGAP);

			    Brush targetBrush = Brushes.LimeGreen;
			    if (rbVar1.Checked) targetBrush = m_Target1Brush;
                else if (rbVar2.Checked) targetBrush = m_Target2Brush;
                else if (rbVar3.Checked) targetBrush = m_Target3Brush;
                else if (rbVar4.Checked) targetBrush = m_Target4Brush;

				for (int i = 0; i < m_DisplayData.Length; i++)
				{
					float xFrom = XGAP + i * xScale + 1;
					float xSize = Math.Max(0.5f, xScale);

					float ySize = 2;
                    float yFrom = picHistogram.Image.Height - YGAP - (m_DisplayData[i] - minVal) * yScale;

                    g.FillRectangle(i >= IncludeDataFrom && i <= IncludeDataTo ? targetBrush : Brushes.DarkGray, xFrom, yFrom, xSize, ySize);
				}

				g.Save();
			}

			picHistogram.Refresh();
		}

		private void timer1_Tick(object sender, EventArgs e)
		{
		    timer1.Enabled = false;

		    IncludeDataFrom = tbarFrom.Value - FromFrameNo;
            IncludeDataTo = tbarTo.Value - FromFrameNo;

            DrawLightCurve();
		}
    }
}
