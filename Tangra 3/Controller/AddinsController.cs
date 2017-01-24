/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Tangra.Addins;
using Tangra.Helpers;
using Tangra.Model.Config;
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
        private IAstrometryProvider m_LocalAstrometryProvider;
        private IFileInfoProvider m_LocalFileInfoProvider;
        private MarshalByRefLightCurveDataProvider m_DelegatedLightCurveDataProvider;
        private MarshalByRefAstrometryProvider m_DelegatedAstrometryProvider;
        private MarshalByRefFileInfoProvider m_DelegatedFileInfoProvider;

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
						if (action.ActionType == AddinActionType.LightCurve || action.ActionType == AddinActionType.LightCurveEventTimeExtractor || action.ActionType == AddinActionType.LightCurveEventTimeExtractorSupportsBinning)
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
				item.Tag != null)
			{
				ITangraAddinAction addinAction = item.Tag as ITangraAddinAction;

				if (addinAction != null && m_AddinManager != null)
				{
					Control waitCursorCtl = item.GetCurrentParent();

					m_DelegatedLightCurveDataProvider = new MarshalByRefLightCurveDataProvider(m_LocalLightCurveDataProvider);
					m_AddinManager.SetLightCurveDataProvider(m_DelegatedLightCurveDataProvider);

					string addinActonName = string.Empty;

					if (waitCursorCtl != null) waitCursorCtl.Cursor = Cursors.WaitCursor;

					try
					{
						addinActonName = addinAction.DisplayName;
						bool isEventTimeExtrator = addinAction.ActionType == AddinActionType.LightCurveEventTimeExtractor;
						bool isEventTimeExtratorSupportsBinning = addinAction.ActionType == AddinActionType.LightCurveEventTimeExtractorSupportsBinning;
						bool canExecuteAction = false;

						if (isEventTimeExtrator || isEventTimeExtratorSupportsBinning)
							canExecuteAction = (m_LocalLightCurveDataProvider as frmLightCurve).PrepareForLightCurveEventTimeExtraction(addinActonName, isEventTimeExtratorSupportsBinning);
						else
							canExecuteAction = true;

						if (canExecuteAction)
						{
							UsageStats.Instance.AddinActionsInvoked++;
							UsageStats.Instance.Save();

							addinAction.Execute();
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
						if (waitCursorCtl != null) waitCursorCtl.Cursor = Cursors.Default;
					}
					
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

        internal void SetAstrometryProvider(IAstrometryProvider provider)
		{
            m_LocalAstrometryProvider = provider;
			m_DelegatedAstrometryProvider = new MarshalByRefAstrometryProvider(m_LocalAstrometryProvider);
			m_AddinManager.SetAstrometryProvider(m_DelegatedAstrometryProvider);
		}

        internal void SetFileInfoProvider(IFileInfoProvider fileInfoProvider)
        {
            m_LocalFileInfoProvider = fileInfoProvider;
            m_DelegatedFileInfoProvider = new MarshalByRefFileInfoProvider(m_LocalFileInfoProvider);
            m_AddinManager.SetFileInfoProvider(m_DelegatedFileInfoProvider);
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

        public List<ITangraAddinAction> GetTimestampOcrActions(out List<ITangraAddin> timestampOcrAddins)
        {
            var rv = new List<ITangraAddinAction>();
            timestampOcrAddins = new List<ITangraAddin>();

            if (m_AddinManager.Addins.Count > 0)
            {
                foreach (Addin addin in m_AddinManager.Addins)
                {
                    foreach (ITangraAddinAction action in addin.Instance.GetAddinActions())
                    {
                        if (action.ActionType == AddinActionType.OcrEngine)
                        {
                            rv.Add(action);
                            if (!timestampOcrAddins.Contains(addin.Instance)) timestampOcrAddins.Add(addin.Instance);
                        }
                    }
                }
            }

            return rv;
        }

        public List<ITangraAddinAction> GetAstrometryActions(out List<ITangraAddin> astrometryAddins)
		{
			var rv = new List<ITangraAddinAction>();
			astrometryAddins = new List<ITangraAddin>();

			if (m_AddinManager.Addins.Count > 0)
			{
				foreach (Addin addin in m_AddinManager.Addins)
				{
					foreach (ITangraAddinAction action in addin.Instance.GetAddinActions())
					{
						if (action.ActionType == AddinActionType.Astrometry)
						{
							rv.Add(action);
							if (!astrometryAddins.Contains(addin.Instance)) astrometryAddins.Add(addin.Instance);
						}
					}
				}
			}

			return rv;
		}
    }
}
