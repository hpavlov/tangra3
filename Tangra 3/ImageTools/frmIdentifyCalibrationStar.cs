/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Tangra.Model.Astro;
using Tangra.StarCatalogues;

namespace Tangra.ImageTools
{
	public partial class frmIdentifyCalibrationStar : Form
	{
		private List<IStar> m_Stars;
		private Dictionary<PSFFit, IStar> m_SelectedStars;

		internal IStar SelectedStar = null;

		public frmIdentifyCalibrationStar()
		{
			InitializeComponent();
		}

		public frmIdentifyCalibrationStar(List<IStar> stars, Dictionary<PSFFit, IStar> selectedStars)
			: this()
		{
			m_Stars = new List<IStar>();
			m_Stars.AddRange(stars);

			m_SelectedStars = selectedStars;
			lvStars.Items.Clear();

			// Remove the already identified stars from the list
			List<ulong> identifiedStarNo = selectedStars.Values.Select(s => s.StarNo).ToList();
			m_Stars.RemoveAll(s => identifiedStarNo.IndexOf(s.StarNo) > -1);

			lblIdentifiedStarNo.Text = selectedStars.Count.ToString();

			PopulateFilteredStars(null);			
		}

		private string NormStarNo(string starNo)
		{
			if (starNo == null)
				return null;
			else
				return starNo.Replace("-", "");
		}

		private void PopulateFilteredStars(Regex srchRegex)
		{
			lvStars.BeginUpdate();
			try
			{
				lvStars.Items.Clear();

				foreach (IStar star in m_Stars)
				{
					string starNo = star.GetStarDesignation(0);
					if (srchRegex == null || srchRegex.IsMatch(starNo))
					{
						ListViewItem item = lvStars.Items.Add(starNo);
						item.SubItems.Add(star.Mag.ToString("0.0"));
						item.Tag = star;
					}
				}

				btnOK.Enabled = false;
			}
			finally
			{
				lvStars.EndUpdate();
			}
		}

		private void lvStars_SelectedIndexChanged(object sender, EventArgs e)
		{
			btnOK.Enabled = false;
			SelectedStar = null;

			if (lvStars.SelectedItems != null &&
				lvStars.SelectedItems.Count == 1)
			{
				SelectedStar = (IStar)lvStars.SelectedItems[0].Tag;
				btnOK.Enabled = true;
			}
		}

		private void tbxStarNo_TextChanged(object sender, EventArgs e)
		{
			string searchText = tbxStarNo.Text.Trim();
			string[] tokens = searchText.Split('-');
			StringBuilder regExStr = new StringBuilder("^.*" + tokens[0]);
			for (int i = 1; i < tokens.Count(); i++)
			{
				regExStr.Append("-[0\\s]*");
				regExStr.Append(tokens[i]);
			}
			regExStr.Append(".*$");

            try
            {
                Regex srchRegex = new Regex(regExStr.ToString());
                tbxStarNo.Enabled = false;
                try
                {
                    PopulateFilteredStars(srchRegex);
                }
                finally
                {
                    tbxStarNo.Enabled = true;
                    tbxStarNo.Focus();
                }
            }
            catch(ArgumentException)
            { }
		}
	}
}
