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
using System.Windows.Forms;
using Tangra.Model.Config;

namespace Tangra.VideoOperations.Spectroscopy
{
    public partial class frmSpectraViewSettings : Form
    {
		private TangraConfig.SpectraViewDisplaySettings m_DisplaySettings;
        private frmViewSpectra m_frmSpectraView;
        private bool m_DontApplySettingsBack = true;

		public frmSpectraViewSettings(TangraConfig.SpectraViewDisplaySettings displaySettings, frmViewSpectra frmSpectraView)
        {
            InitializeComponent();

            m_frmSpectraView = frmSpectraView;
            m_DisplaySettings = displaySettings;

			m_DontApplySettingsBack = true;
			try
			{
				ucColorPickerReferenceStar.SelectedColor = m_DisplaySettings.SpectraLineColor;
				ucColorPickerKnownLine.SelectedColor = m_DisplaySettings.KnownLineColor;
				ucColorPickerGridLines.SelectedColor = m_DisplaySettings.GridLinesColor;
				ucColorPickerGridLegend.SelectedColor = m_DisplaySettings.LegendColor;
				ucColorPickerAperture.SelectedColor = m_DisplaySettings.SpectraApertureColor;
				ucColorPickerBackground.SelectedColor = m_DisplaySettings.PlotBackgroundColor;
			}
			finally
			{
				m_DontApplySettingsBack = false;
			}
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            ApplyValuesNoSave();

			m_DisplaySettings.Save();

            DialogResult = DialogResult.OK;
            Close();
        }

		private void ApplyValuesNoSave()
		{
			m_DisplaySettings.SpectraLineColor = ucColorPickerReferenceStar.SelectedColor;
			m_DisplaySettings.KnownLineColor = ucColorPickerKnownLine.SelectedColor;
			m_DisplaySettings.GridLinesColor = ucColorPickerGridLines.SelectedColor;
			m_DisplaySettings.LegendColor = ucColorPickerGridLegend.SelectedColor;
			m_DisplaySettings.SpectraApertureColor = ucColorPickerAperture.SelectedColor;
			m_DisplaySettings.PlotBackgroundColor = ucColorPickerBackground.SelectedColor;

			m_DisplaySettings.Initialize(); 
		}


		private void Preview()
		{
			ApplyValuesNoSave();

			m_frmSpectraView.RedrawPlot();
		}

		private void SelectedColorChanged(object sender, EventArgs e)
		{
			m_DisplaySettings.SpectraLineColor = ucColorPickerReferenceStar.SelectedColor;
			m_DisplaySettings.KnownLineColor = ucColorPickerKnownLine.SelectedColor;
			m_DisplaySettings.GridLinesColor = ucColorPickerGridLines.SelectedColor;
			m_DisplaySettings.LegendColor = ucColorPickerGridLegend.SelectedColor;
			m_DisplaySettings.SpectraApertureColor = ucColorPickerAperture.SelectedColor;
			m_DisplaySettings.PlotBackgroundColor = ucColorPickerBackground.SelectedColor;

			if (!m_DontApplySettingsBack)
				Preview();
		}
    }
}
