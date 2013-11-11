using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Config.SettingPannels;
using Tangra.Controller;
using Tangra.Helpers;
using Tangra.Model.Config;

namespace Tangra.Config
{
	public partial class frmTangraSettings : Form
	{
		private int m_CurrentPropertyPageId = -1;
		private Dictionary<int, SettingsPannel> m_PropertyPages = new Dictionary<int, SettingsPannel>();

		private SettingsPannel m_CurrentPanel = null;
		private IAdvStatusPopupFormCustomizer m_AdvPopupCustomizer;
		private IAavStatusPopupFormCustomizer m_AavPopupCustomizer;
		private IAddinContainer[] m_AddinContainers;
	    private AddinsController m_AddinsController;

		public bool ShowCatalogRequiredHint = false;

		public frmTangraSettings(
            ILightCurveFormCustomizer lightCurveCustomizer, 
            IAdvStatusPopupFormCustomizer advPopupCustomizer, 
            IAavStatusPopupFormCustomizer aavPopupCustomizer,
            AddinsController addinsController,
			IAddinContainer[] addinContainers)
		{
			InitializeComponent();

		    m_AddinsController = addinsController;
			m_AdvPopupCustomizer = advPopupCustomizer;
			m_AavPopupCustomizer = aavPopupCustomizer;
			m_AddinContainers = addinContainers;

			InitAllPropertyPages();

            TangraConfig.Load(ApplicationSettingsSerializer.Instance);

			ucCustomizeLightCurveViewer lightCurvesColoursPanel = m_PropertyPages.Select(kvp => kvp.Value).FirstOrDefault(x => x is ucCustomizeLightCurveViewer) as ucCustomizeLightCurveViewer;
			if (lightCurvesColoursPanel != null)
			    lightCurvesColoursPanel.SetLightCurveFormCustomizer(lightCurveCustomizer);

			ucADVSVideo12bit AdvsVideo12bitPanel = m_PropertyPages.Select(kvp => kvp.Value).FirstOrDefault(x => x is ucADVSVideo12bit) as ucADVSVideo12bit;
			if (AdvsVideo12bitPanel != null)
			    AdvsVideo12bitPanel.SetAdvStatusPopupFormCustomizer(advPopupCustomizer);

			ucAAV8bit Aav8bitPanel = m_PropertyPages.Select(kvp => kvp.Value).FirstOrDefault(x => x is ucAAV8bit) as ucAAV8bit;
			if (Aav8bitPanel != null)
				Aav8bitPanel.SetAdvStatusPopupFormCustomizer(aavPopupCustomizer);

			foreach(SettingsPannel panel in m_PropertyPages.Values) 
			    panel.LoadSettings();
		}

		private void InitAllPropertyPages()
		{
			m_PropertyPages.Add(0, new ucGeneralTangra());

			m_PropertyPages.Add(1, new ucGeneralVideo());
			m_PropertyPages.Add(2, new ucAnalogueVideo8bit());
			m_PropertyPages.Add(3, new ucADVSVideo12bit());
			m_PropertyPages.Add(10, new ucAAV8bit());

			m_PropertyPages.Add(11, new ucPreProcessingSettings());

			m_PropertyPages.Add(4, new ucPhotometry());

			m_PropertyPages.Add(6, new ucTracking());
            m_PropertyPages.Add(8, new ucCompatibility());

            m_PropertyPages.Add(12, new ucAddins(m_AddinsController, m_AddinContainers));

			m_PropertyPages.Add(7, new ucCustomizeLightCurves());
			m_PropertyPages.Add(9, new ucCustomizeLightCurveViewer());
		}

		private void SetFormTitle(TreeNode currentNode)
		{
			if (currentNode != null)
			{
				string newTitle = "Tangra Settings";

				if (currentNode.Parent == null)
				{
					// Select the first sibling
					if (currentNode.Nodes.Count > 0)
						newTitle = string.Format("Tangra Settings - {0} - {1}", currentNode.Text, currentNode.Nodes[0].Text);
					else
						newTitle = string.Format("Tangra Settings - {0}", currentNode.Text);
				}
				else
					newTitle = string.Format("Tangra Settings - {0} - {1}", currentNode.Parent.Text, currentNode.Text);

				this.Text = newTitle;
			}
		}

		void tvSettings_BeforeSelect(object sender, System.Windows.Forms.TreeViewCancelEventArgs e)
		{
			e.Cancel = m_CurrentPanel != null && !m_CurrentPanel.ValidateSettings();
		}

		private void tvSettings_AfterSelect(object sender, TreeViewEventArgs e)
		{
			if (e.Node != null)
			{
				int propPageId = int.Parse((string)e.Node.Tag);

				if (m_CurrentPropertyPageId != propPageId)
				{
					SettingsPannel propPage = null;
					if (m_PropertyPages.TryGetValue(propPageId, out propPage))
					{
						LoadPropertyPage(propPage);
						m_CurrentPanel = propPage;
						m_CurrentPropertyPageId = propPageId;
						SetFormTitle(e.Node);
					}
				}

				if (e.Node.Nodes.Count > 0)
					e.Node.Expand();
			}			
		}

		private void LoadPropertyPage(Control propPage)
		{
			if (pnlPropertyPage.Controls.Count == 1)
				pnlPropertyPage.Controls.Remove(pnlPropertyPage.Controls[0]);

			if (propPage != null)
			{
				pnlPropertyPage.Controls.Add(propPage);
				propPage.Dock = DockStyle.Fill;				
			}
		}

		private void frmTangraSettings2_Load(object sender, EventArgs e)
		{
			tvSettings.SelectedNode = tvSettings.Nodes["ndGeneral"];
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			if (m_CurrentPanel != null)
			{
				if (m_CurrentPanel.ValidateSettings())
				{
					foreach (SettingsPannel panel in m_PropertyPages.Values)
					{
						if (panel.ValidateSettings())
							panel.SaveSettings();
						else
						{
							m_CurrentPanel = panel;
							LoadPropertyPage(m_CurrentPanel);
							return;
						}
					}
				}
				else
					return;
			}

            TangraConfig.Settings.Save();

			foreach (SettingsPannel panel in m_PropertyPages.Values)
			{
				panel.OnPostSaveSettings();
			}

			DialogResult = DialogResult.OK;
			Close();

		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			// Restore the displayed ADV/AAV status settings and they may have been changed by the user during the use of the dialog
			if (m_AdvPopupCustomizer != null)
			{				
				m_AdvPopupCustomizer.UpdateSettings(TangraConfig.Settings.ADVS);
				m_AdvPopupCustomizer.RefreshState();				
			}

			if (m_AavPopupCustomizer != null)
			{
				m_AavPopupCustomizer.UpdateSettings(TangraConfig.Settings.AAV);
				m_AavPopupCustomizer.RefreshState();
			}

			DialogResult = DialogResult.Cancel;
			Close();
		}

		private void btnResetDefaults_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show(
				this,
				"You are about to reset all Tangra settings to their default values. All customisations will be lost.\r\n\r\nDo you want to continue?",
				"Tangra",
				MessageBoxButtons.YesNo,
				MessageBoxIcon.Warning,
				MessageBoxDefaultButton.Button2) == DialogResult.Yes)
			{
				TangraConfig.Reset(ApplicationSettingsSerializer.Instance);

				foreach (SettingsPannel panel in m_PropertyPages.Values)
				{
					panel.LoadSettings();
					panel.Reset();
				}

				MessageBox.Show(
					this,
					"Tangra settings have been reset.",
					"Tangra",
					MessageBoxButtons.YesNo,
					MessageBoxIcon.Information);
			}
		}
	}
}
