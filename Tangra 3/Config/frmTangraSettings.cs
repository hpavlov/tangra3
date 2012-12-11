using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Config.SettingPannels;
using Tangra.Model.Config;

namespace Tangra.Config
{
	public partial class frmTangraSettings : Form
	{
		private int m_CurrentPropertyPageId = -1;
		private Dictionary<int, SettingsPannel> m_PropertyPages = new Dictionary<int, SettingsPannel>();

		private SettingsPannel m_CurrentPanel = null;

		public bool ShowCatalogRequiredHint = false;

		public frmTangraSettings(ILightCurveFormCustomizer lightCurveCustomizer, IAdvStatusPopupFormCustomizer advPopupCustomizer)
		{
			InitializeComponent();

			InitAllPropertyPages();

			TangraConfig.Load();

			ucCustomizeLightCurveViewer lightCurvesColoursPanel = m_PropertyPages.Select(kvp => kvp.Value).FirstOrDefault(x => x is ucCustomizeLightCurveViewer) as ucCustomizeLightCurveViewer;
			if (lightCurvesColoursPanel != null)
			    lightCurvesColoursPanel.SetLightCurveFormCustomizer(lightCurveCustomizer);

			ucADVSVideo12bit AdvsVideo12bitPanel = m_PropertyPages.Select(kvp => kvp.Value).FirstOrDefault(x => x is ucADVSVideo12bit) as ucADVSVideo12bit;
			if (AdvsVideo12bitPanel != null)
			    AdvsVideo12bitPanel.SetAdvStatusPopupFormCustomizer(advPopupCustomizer);

			foreach(SettingsPannel panel in m_PropertyPages.Values) 
			    panel.LoadSettings();
		}

		private void InitAllPropertyPages()
		{
			m_PropertyPages.Add(0, new ucGeneralTangra());

			m_PropertyPages.Add(2, new ucAnalogueVideo8bit());
			m_PropertyPages.Add(3, new ucADVSVideo12bit());

			m_PropertyPages.Add(4, new ucPhotometry());

			m_PropertyPages.Add(6, new ucTracking());

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
	}
}
