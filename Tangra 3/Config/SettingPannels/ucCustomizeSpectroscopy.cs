using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Model.Config;

namespace Tangra.Config.SettingPannels
{
	public partial class ucCustomizeSpectroscopy : SettingsPannel
	{
		private ISpectraViewFormCustomizer m_SpectraViewCustomizer;
		private TangraConfig.SpectraViewDisplaySettings m_DisplaySettings;
		private bool m_DontApplySettingsBack;

		public ucCustomizeSpectroscopy()
		{
			InitializeComponent();
		}

		public void SetSpectraViewFormCustomizer(ISpectraViewFormCustomizer spectraViewCustomizer)
		{
			m_SpectraViewCustomizer = spectraViewCustomizer;
			if (spectraViewCustomizer != null)
			{
				m_DisplaySettings = spectraViewCustomizer.FormDisplaySettings;
			}
			else
			{
				m_DisplaySettings = new TangraConfig.SpectraViewDisplaySettings();
				m_DisplaySettings.Load();
				m_DisplaySettings.Initialize();
			}
		}

		public override void LoadSettings()
		{
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

		public override void SaveSettings()
		{
			base.SaveSettings();

			ApplyValuesNoSave();

			m_DisplaySettings.Save();
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

		public override void Reset()
		{
			Preview();
		}

		private void Preview()
		{
			ApplyValuesNoSave();

			if (m_SpectraViewCustomizer != null)
				m_SpectraViewCustomizer.RedrawPlot();
		}

		private void SelectedColorChanged(object sender, EventArgs e)
		{
			m_DisplaySettings.SpectraLineColor = ucColorPickerReferenceStar.SelectedColor;
			m_DisplaySettings.KnownLineColor = ucColorPickerKnownLine.SelectedColor;
			m_DisplaySettings.GridLinesColor = ucColorPickerGridLines.SelectedColor;
			m_DisplaySettings.LegendColor = ucColorPickerGridLegend.SelectedColor;

			if (!m_DontApplySettingsBack)
				Preview();
		}
	}
}
