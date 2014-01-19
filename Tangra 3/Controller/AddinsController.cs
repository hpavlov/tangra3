using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Tangra.Addins;
using Tangra.Model.Helpers;
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
        private MarshalByRefLightCurveDataProvider m_DelegatedLightCurveDataProvider;

		public AddinsController(frmMain mainFormView, VideoController videoController)
		{
			m_VideoController = videoController;
			m_MainForm = mainFormView;

            m_AddinManager = new AddinManager(mainFormView, videoController);
		}

		public void LoadAddins()
		{
			m_AddinManager.LoadAddins();
		}

        public void ReloadAddins()
        {
            UnloadAddins(); 
            m_AddinManager.LoadAddins();
        }

	    public bool CanUnloadAddins()
	    {
		    return
			    m_AddinManager != null &&
			    m_AddinManager.IsAppDomainIsoltion;
	    }

		public bool CanReloadAddins()
		{
			return
				m_AddinManager != null &&
				(m_AddinManager.Addins.Count == 0 || m_AddinManager.IsAppDomainIsoltion);
		}

	    public void UnloadAddins()
        {
            if (m_AddinManager != null)
                m_AddinManager.Dispose();
            m_AddinManager = null;

            m_AddinManager = new AddinManager(m_MainForm, m_VideoController); 
        }

        public void Dispose()
		{
			m_AddinManager.Dispose();
		}

		public void ShowLoadedAddins(ListBox listBox)
		{
            listBox.Items.Clear();

		    foreach (Addin addin in m_AddinManager.Addins)
		    {
		        listBox.Items.Add(addin);
		    }
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
						if (action.ActionType == AddinActionType.LightCurve || action.ActionType == AddinActionType.LightCurveEventTimeExtractor)
						{
							ToolStripMenuItem item = new ToolStripMenuItem(action.DisplayName);
							item.Click += OnLightCurveAddinItemClick;
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

		public bool OccultToolsAddinIsLoaded()
		{
			return m_AddinManager.Addins.FirstOrDefault(x => x.Instance.DisplayName == "Occult Tools for Tangra") != null;
		}

	    private void OnLightCurveAddinItemClick(object sender, EventArgs eventArgs)
	    {
			ToolStripDropDownItem item = sender as ToolStripDropDownItem;

			if (item != null &&
				item.Tag != null &&
				item.Tag is ITangraAddinAction)
			{
                m_DelegatedLightCurveDataProvider = new MarshalByRefLightCurveDataProvider(m_LocalLightCurveDataProvider);
                m_AddinManager.SetLightCurveDataProvider(m_DelegatedLightCurveDataProvider);
			    string addinActonName = string.Empty;

				item.GetCurrentParent().Cursor = Cursors.WaitCursor;

				try
				{
					addinActonName = ((ITangraAddinAction)item.Tag).DisplayName;
					bool isEventTimeExtrator = ((ITangraAddinAction)item.Tag).ActionType == AddinActionType.LightCurveEventTimeExtractor;
					bool canExecuteAction = false;

					if (isEventTimeExtrator)
						canExecuteAction = (m_LocalLightCurveDataProvider as frmLightCurve).PrepareForLightCurveEventTimeExtraction(((ITangraAddinAction)item.Tag).DisplayName);
					else
						canExecuteAction = true;

					if (canExecuteAction)
					{
						((ITangraAddinAction) item.Tag).Execute();

						if (isEventTimeExtrator)
							(m_LocalLightCurveDataProvider as frmLightCurve).FinishedLightCurveEventTimeExtraction();
					}
				}
				catch (AppDomainUnloadedException)
				{ }
				catch (ObjectDisposedException)
				{ }
				catch (Exception ex)
				{
                    Trace.WriteLine(ex.GetFullStackTrace());

					MessageBox.Show(
						string.Format("{0}:\r\n\r\n{1} ({2})",
							addinActonName,
							ex is TargetInvocationException ? ex.InnerException.Message : ex.Message,
							ex is TargetInvocationException ? ex.InnerException.GetType().Name : ex.GetType().Name),
						"Error running add-in",
						MessageBoxButtons.OK,
						MessageBoxIcon.Error);
				}
				finally
				{
					item.GetCurrentParent().Cursor = Cursors.WaitCursor; 
				}
			}
	    }

        public void OnLightCurveCurrentFrameChanged(int frameNo)
        {
            if (m_DelegatedLightCurveDataProvider != null)
            {
                m_DelegatedLightCurveDataProvider.CurrentlySelectedFrameNumberChanged(frameNo);

                if (m_AddinManager.Addins.Count > 0)
                {
                    foreach (Addin addin in m_AddinManager.Addins)
                    {
                        try
                        {
                            addin.OnEventNotification(AddinFiredEventType.LightCurveSelectedFrameChanged);
                        }
                        catch (Exception ex)
                        {
                            Trace.WriteLine(ex.GetFullStackTrace());
                        }
                    }
                }
            }
        }
            
		internal void SetLightCurveDataProvider(ILightCurveDataProvider provider)
		{
			m_LocalLightCurveDataProvider = provider;
		    m_DelegatedLightCurveDataProvider = null;
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
