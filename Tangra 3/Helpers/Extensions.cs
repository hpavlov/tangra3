using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Tangra.Helpers
{
	public static class Extensions
	{
		public static void SetNUDValue(this NumericUpDown nud, double value)
		{
			if (!double.IsNaN(value))
				SetNUDValue(nud, (decimal)value);
		}

		public static void SetNUDValue(this NumericUpDown nud, int value)
		{
			SetNUDValue(nud, (decimal)value);
		}

		public static void SetNUDValue(this NumericUpDown nud, decimal value)
		{
			if (value < nud.Minimum)
				nud.Value = nud.Minimum;
			else if (value > nud.Maximum)
				nud.Value = nud.Maximum;
			else
				nud.Value = value;
		}

		public static void SetCBXIndex(this ComboBox cbx, int index)
		{
			if (cbx.Items.Count > 0)
				cbx.SelectedIndex = Math.Max(0, Math.Min(cbx.Items.Count - 1, index));
			else
				cbx.SelectedIndex = -1;
		}
	}
}
