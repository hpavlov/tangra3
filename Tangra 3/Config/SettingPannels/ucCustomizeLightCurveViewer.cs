using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Config;
using Tangra.Model.Config;

namespace Tangra.Config.SettingPannels
{
	public partial class ucCustomizeLightCurveViewer : SettingsPannel
	{

		private ILightCurveFormCustomizer m_LightCurveCustomizer;
		private TangraConfig.LightCurvesDisplaySettings m_DisplaySettings;
		private bool m_DontApplySettingsBack = true;

		public ucCustomizeLightCurveViewer()
		{
			InitializeComponent();
		}

		public void SetLightCurveFormCustomizer(ILightCurveFormCustomizer lightCurveCustomizer)
		{
			m_LightCurveCustomizer = lightCurveCustomizer;
			if (lightCurveCustomizer != null)
			{
				m_DisplaySettings = lightCurveCustomizer.FormDisplaySettings;
			}
			else
			{
				m_DisplaySettings = new TangraConfig.LightCurvesDisplaySettings();
				m_DisplaySettings.Load();
				m_DisplaySettings.Initialize();
			}
		}

        public override void LoadSettings()
        {
			m_DontApplySettingsBack = true;
			try
			{
				cpBackground.SelectedColor = m_DisplaySettings.BackgroundColor;
				cpLabels.SelectedColor = m_DisplaySettings.LabelsColor;
				cpGrid.SelectedColor = m_DisplaySettings.GridLinesColor;

				cbxColorScheme.SelectedIndex = (int)m_DisplaySettings.ColorScheme;
				cbxTangraTargetColors.Checked = m_DisplaySettings.UseTangraTargetColors;
				nudPointSize.Value = (decimal)m_DisplaySettings.DatapointSize;
				cbxDrawGrid.Checked = m_DisplaySettings.DrawGrid;

				cpFocusArea.SelectedColor = m_DisplaySettings.SmallGraphFocusBackgroundBrushColor;
				cpSelectionCursor.SelectedColor = m_DisplaySettings.SelectionCursorColor;

				SetColorScheme(m_DisplaySettings.ColorScheme);
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
            m_DisplaySettings.BackgroundColor = cpBackground.SelectedColor;
            m_DisplaySettings.LabelsColor = cpLabels.SelectedColor;
            m_DisplaySettings.GridLinesColor = cpGrid.SelectedColor;

            m_DisplaySettings.ColorScheme = (TangraConfig.LightCurvesColorScheme)cbxColorScheme.SelectedIndex;
            m_DisplaySettings.UseTangraTargetColors = cbxTangraTargetColors.Checked;
            m_DisplaySettings.DatapointSize = (float)nudPointSize.Value;
        	m_DisplaySettings.DrawGrid = cbxDrawGrid.Checked;

            m_DisplaySettings.SmallGraphFocusBackgroundBrushColor = cpFocusArea.SelectedColor;
        	m_DisplaySettings.SelectionCursorColor = cpSelectionCursor.SelectedColor;

            m_DisplaySettings.Initialize();                
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
        	Preview();
        }

        private void cbxTangraTargetColors_CheckedChanged(object sender, EventArgs e)
        {
            bool dontApplySettingsBack = m_DontApplySettingsBack;
            m_DontApplySettingsBack = true;
            try
            {
                if (cbxTangraTargetColors.Checked)
                {
                    cpTarget1.SelectedColor = TangraConfig.Settings.Color.Target1;
                    cpTarget2.SelectedColor = TangraConfig.Settings.Color.Target2;
                    cpTarget3.SelectedColor = TangraConfig.Settings.Color.Target3;
                    cpTarget4.SelectedColor = TangraConfig.Settings.Color.Target4;
                }
                else
                {
                    m_DisplaySettings.UseTangraTargetColors = false;

                    cpTarget1.SelectedColor = m_DisplaySettings.Target1Color;
                    cpTarget2.SelectedColor = m_DisplaySettings.Target2Color;
                    cpTarget3.SelectedColor = m_DisplaySettings.Target3Color;
                    cpTarget4.SelectedColor = m_DisplaySettings.Target4Color;
                }
            }
            finally
            {
                m_DontApplySettingsBack = dontApplySettingsBack; 
            }

            if (!m_DontApplySettingsBack)
                Preview();
        }

        private void SetColorScheme(TangraConfig.LightCurvesColorScheme scheme)
        {
			gbColors.Enabled = scheme == TangraConfig.LightCurvesColorScheme.Custom;

            bool dontApplySettingsBack = m_DontApplySettingsBack;
            m_DontApplySettingsBack = true;
            try
            {
				if (scheme == TangraConfig.LightCurvesColorScheme.Clasic)
                {
                    cpBackground.SelectedColor = Color.FromArgb(128, 128, 128);
                    cpGrid.SelectedColor = Color.FromArgb(180, 180, 180);
                    cpLabels.SelectedColor = Color.White;
                    cbxTangraTargetColors.Checked = true;
					m_DisplaySettings.ColorScheme = TangraConfig.LightCurvesColorScheme.Clasic;
                }
				else if (scheme == TangraConfig.LightCurvesColorScheme.Pastel)
                {
					cpBackground.SelectedColor = Color.FromArgb(237, 240, 241);
                    cpGrid.SelectedColor = Color.FromArgb(180, 180, 180);
                    cpLabels.SelectedColor = Color.Navy;
                    cbxTangraTargetColors.Checked = false;
					cpTarget1.SelectedColor = Color.FromArgb(124, 194, 240);
					cpTarget2.SelectedColor = Color.FromArgb(255, 102, 89);
					cpTarget3.SelectedColor = Color.FromArgb(133, 221, 161);
					cpTarget4.SelectedColor = Color.FromArgb(255, 187, 99);
					m_DisplaySettings.ColorScheme = TangraConfig.LightCurvesColorScheme.Pastel;
                }
				else if (scheme == TangraConfig.LightCurvesColorScheme.Contrast)
                {
                    cpBackground.SelectedColor = Color.WhiteSmoke;
                    cpGrid.SelectedColor = Color.DarkGray;
                    cpLabels.SelectedColor = Color.Black;
                    cbxTangraTargetColors.Checked = false;
                    cpTarget1.SelectedColor = Color.Blue;
                    cpTarget2.SelectedColor = Color.Green;
                    cpTarget3.SelectedColor = Color.Magenta;
                    cpTarget4.SelectedColor = Color.FromArgb(0, 0, 64);
					m_DisplaySettings.ColorScheme = TangraConfig.LightCurvesColorScheme.Contrast;
                }
				else if (scheme == TangraConfig.LightCurvesColorScheme.Custom)
                {
					m_DisplaySettings.ColorScheme = TangraConfig.LightCurvesColorScheme.Custom;
                    cbxTangraTargetColors.Checked = m_DisplaySettings.UseTangraTargetColors;

                    if (!cbxTangraTargetColors.Checked)
                    {
                        cpTarget1.SelectedColor = m_DisplaySettings.Target1Color;
                        cpTarget2.SelectedColor = m_DisplaySettings.Target2Color;
                        cpTarget3.SelectedColor = m_DisplaySettings.Target3Color;
                        cpTarget4.SelectedColor = m_DisplaySettings.Target4Color;
                    }
                }

            	m_DisplaySettings.UseTangraTargetColors = cbxTangraTargetColors.Checked;
                m_DisplaySettings.Target1Color = cpTarget1.SelectedColor;
                m_DisplaySettings.Target2Color = cpTarget2.SelectedColor;
                m_DisplaySettings.Target3Color = cpTarget3.SelectedColor;
                m_DisplaySettings.Target4Color = cpTarget4.SelectedColor;
            }
            finally
            {
                m_DontApplySettingsBack = dontApplySettingsBack;
            }
        }

        private void cbxColorScheme_SelectedIndexChanged(object sender, EventArgs e)
        {
			TangraConfig.LightCurvesColorScheme scheme = (TangraConfig.LightCurvesColorScheme)cbxColorScheme.SelectedIndex;
            
            SetColorScheme(scheme);

            if (!m_DontApplySettingsBack)
                Preview();
        }

        private void Preview()
        {
            ApplyValuesNoSave();

			if (m_LightCurveCustomizer != null)
				m_LightCurveCustomizer.RedrawPlot();
        }

        private void SelectedColorChanged(object sender, EventArgs e)
        {
            m_DisplaySettings.BackgroundColor = cpBackground.SelectedColor;
            m_DisplaySettings.GridLinesColor = cpGrid.SelectedColor;
            m_DisplaySettings.LabelsColor = cpLabels.SelectedColor;
        	m_DisplaySettings.SmallGraphFocusBackgroundBrushColor = cpFocusArea.SelectedColor;
			m_DisplaySettings.SelectionCursorColor = cpSelectionCursor.SelectedColor;

            if (!m_DisplaySettings.UseTangraTargetColors)
            {
                m_DisplaySettings.Target1Color = cpTarget1.SelectedColor;
                m_DisplaySettings.Target2Color = cpTarget2.SelectedColor;
                m_DisplaySettings.Target3Color = cpTarget3.SelectedColor;
                m_DisplaySettings.Target4Color = cpTarget4.SelectedColor;
            }

            if (!m_DontApplySettingsBack)
                Preview();
        }

        private void nudPointSize_ValueChanged(object sender, EventArgs e)
        {
            if (!m_DontApplySettingsBack)
                Preview();
        }

		private void cbxDrawGrid_CheckedChanged(object sender, EventArgs e)
		{
            if (!m_DontApplySettingsBack)
                Preview();
		}
	}
}
