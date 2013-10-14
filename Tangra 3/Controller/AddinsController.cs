using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Addins;
using Tangra.SDK;
using Tangra.Video;
using Tangra.VideoOperations.LightCurves;

namespace Tangra.Controller
{
    public class AddinsController : IDisposable
    {
		private VideoController m_VideoController;
		private frmMain m_MainForm;
		private AddinManager m_AddinManager;
	    private ILightCurveDataProvider m_LocalLightCurveDataProvider;

		public AddinsController(frmMain mainFormView, VideoController videoController)
		{
			m_VideoController = videoController;
			m_MainForm = mainFormView;

			m_AddinManager = new AddinManager(mainFormView);
		}

		public void LoadAddins()
		{
			m_AddinManager.LoadAddins();

			BuildMainFormAddinsMenu();			
		}

		public void Dispose()
		{
			m_MainForm.miConfigureAddin.DropDownItems.Clear();
			m_AddinManager.Dispose();
		}

		private void BuildMainFormAddinsMenu()
		{
			if (m_AddinManager.Addins.Count > 0)
			{
				m_MainForm.miConfigureAddin.DropDownItems.Clear();

				foreach (Addin addin in m_AddinManager.Addins)
				{
					ToolStripMenuItem item = new ToolStripMenuItem(addin.Instance.DisplayName);
					item.Click += ConfigureAddinItem_Click;
					item.Tag = addin;

					m_MainForm.miConfigureAddin.DropDownItems.Add(item);
				}

				m_MainForm.miAddins.Visible = true;
			}
			else
				m_MainForm.miAddins.Visible = false;
		}

		void ConfigureAddinItem_Click(object sender, EventArgs e)
		{
			ToolStripDropDownItem item = sender as ToolStripDropDownItem;

			if (item != null &&
				item.Tag != null &&
				item.Tag is Addin)
			{
				((Addin)item.Tag).Instance.Configure();
			}
		}

		public void ShowLoadedAddins()
		{
			
		}

		public void BuildLightCurveMenuAddins(ToolStripMenuItem topMenuItem)
		{
			bool lightCurveAddinsAdded = false;

			if (m_AddinManager.Addins.Count > 0)
			{
				topMenuItem.DropDownItems.Clear();

				foreach (Addin addin in m_AddinManager.Addins)
				{
					foreach (ITangraAddinAction action in addin.Instance.GetAddinActions())
					{
						if (action.ActionType == AddinActionType.LightCurve)
						{
							ToolStripMenuItem item = new ToolStripMenuItem(action.DisplayName);
							item.Click += ItemOnClick;
							if (action.Icon != null)
							{
								item.Image = (Bitmap)Bitmap.FromHbitmap(action.Icon).Clone();
								item.ImageTransparentColor = Color.FromArgb(action.IconTransparentColorARGB);
							}
							item.Tag = action;

							topMenuItem.DropDownItems.Add(item);
							lightCurveAddinsAdded = true;
						}
					}
				}
			}


			topMenuItem.Visible = lightCurveAddinsAdded;
			
		}

	    private void ItemOnClick(object sender, EventArgs eventArgs)
	    {
			ToolStripDropDownItem item = sender as ToolStripDropDownItem;

			if (item != null &&
				item.Tag != null &&
				item.Tag is ITangraAddinAction)
			{
				m_AddinManager.SetLightCurveDataProvider(new MarshalByRefLightCurveDataProvider(m_LocalLightCurveDataProvider));
				((ITangraAddinAction)item.Tag).Execute();
			}
	    }

		internal void SetLightCurveDataProvider(ILightCurveDataProvider provider)
		{
			m_LocalLightCurveDataProvider = provider;
		}

        public void FinaliseAddins()
        {
            if (m_AddinManager.Addins.Count > 0)
            {
                foreach (Addin addin in m_AddinManager.Addins)
                {
                    foreach (ITangraAddinAction action in addin.Instance.GetAddinActions())
                    {
                        action.Finalise();
                    }

                    addin.Instance.Finalise();
                }

                m_AddinManager.Addins.Clear();
            }
        }
    }
}
