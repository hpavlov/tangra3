/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Lifetime;
using System.Runtime.Remoting.Services;
using System.Text;
using System.Windows.Forms;
using Tangra.Controller;
using Tangra.Model.Config;
using Tangra.SDK;
using Tangra.Model.Helpers;

namespace Tangra.Addins
{
	public interface IAddinManager
	{
		IWin32Window ParentWindow { get; }
		ILightCurveDataProvider LightCurveDataProvider { get; }
        IAstrometryProvider AstrometryProvider { get; }
        IFileInfoProvider FileInfoProvider { get; }
	    void PositionToFrame(int frameNo);
	}

	internal class RemotingClientSponsor : MarshalByRefObject, ISponsor
	{
		public TimeSpan Renewal(ILease lease)
		{
			TimeSpan tsLease = TimeSpan.FromMinutes(5);
			lease.Renew(tsLease);
			return tsLease;
		}

		public override object InitializeLifetimeService()
		{
			return null;
		}
	}

	[Serializable]
	public class AddinManager : IDisposable, IAddinManager
	{
		private bool m_IsAppDomainIsoltion;

		public readonly List<Addin> Addins = new List<Addin>();

        public readonly static string ADDINS_DIRECTORY = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Addins");

		private frmMain m_MainForm;
        private VideoController m_VideoController;
		private ILightCurveDataProvider m_lcDataProvider;
	    private IAstrometryProvider m_astrometryProvider;
	    private IFileInfoProvider m_fileInfoProvider;

		internal RemotingClientSponsor RemotingClientSponsor = new RemotingClientSponsor();

        public AddinManager(frmMain mainForm, VideoController videoController)
		{
			m_MainForm = mainForm;
            m_VideoController = videoController;

            if (TangraConfig.Settings.Generic.AddinDebugTraceEnabled)
			    TrackingServices.RegisterTrackingHandler(new AddinTrackingHandler());
		}

		public void LoadAddins()
		{
			m_IsAppDomainIsoltion = TangraConfig.Settings.Generic.AddinIsolationLevel == TangraConfig.IsolationLevel.AppDomain;

			List<string> candiates = SearchAddins();
			foreach (string candiate in candiates)
			{
				Addin instance;
				try
				{
					instance = new Addin(candiate, this);
				}
				catch (Exception ex)
				{
					Trace.WriteLine(ex.GetFullStackTrace());
					instance = null;
				}

				if (instance != null)
					Addins.Add(instance);
			}
		}

		private List<string> SearchAddins()
		{
			var addins = new List<string>();

            if (Directory.Exists(ADDINS_DIRECTORY))
			{
                string[] names = Directory.GetFiles(ADDINS_DIRECTORY, "*.dll");

				foreach (string asm in names)
				{
				    try
				    {
				        Assembly loadedAssembly = Assembly.LoadFile(asm);

				        List<string> addinTypesFound = loadedAssembly
				            .GetTypes()
				            .Where(t => t.IsClass && t.GetInterface(typeof (ITangraAddin).FullName) != null)
				            .Select(t => string.Concat(t.FullName, ",", loadedAssembly.FullName))
				            .ToList();

				        addins.AddRange(addinTypesFound);
				    }
				    catch (ReflectionTypeLoadException rtlex)
				    {
                        Trace.WriteLine(string.Format("'{0}' is not identified as an add-in: {1}\r\n{2}\r\n{3}", 
                            asm, 
                            rtlex.GetType(), 
                            rtlex,
                            rtlex.LoaderExceptions != null ? string.Join("\r\n", rtlex.LoaderExceptions.Select(x => x.ToString())) : null));
				    }
					catch (Exception ex)
					{
                        Trace.WriteLine(string.Format("'{0}' is not identified as an add-in: {1}", asm, ex.GetType()));
					}
				}
			}

			return addins;
		}

		public void Dispose()
		{
			foreach (Addin addin in Addins)
			{
				try
				{
					addin.Dispose();
				}
				catch (Exception ex)
				{
					Trace.WriteLine(ex.GetFullStackTrace());
				}
			}
		}

		internal bool IsAppDomainIsoltion
		{
			get
			{
				return Addins.Count > 0 && m_IsAppDomainIsoltion;
			}
		}

		internal void SetLightCurveDataProvider(ILightCurveDataProvider provider)
		{
			m_lcDataProvider = provider;
		}

		internal void SetAstrometryProvider(IAstrometryProvider provider)
		{
            m_astrometryProvider = provider;
		}

        internal void SetFileInfoProvider(IFileInfoProvider fileInfoProvider)
	    {
	        m_fileInfoProvider = fileInfoProvider;
	    }

		IWin32Window IAddinManager.ParentWindow
		{
			get { return m_MainForm; }
		}

		ILightCurveDataProvider IAddinManager.LightCurveDataProvider
		{
			get { return m_lcDataProvider; }
		}

	    IAstrometryProvider IAddinManager.AstrometryProvider
	    {
            get { return m_astrometryProvider; }
	    }

	    IFileInfoProvider IAddinManager.FileInfoProvider
	    {
            get { return m_fileInfoProvider; }
	    }

        void IAddinManager.PositionToFrame(int frameNo)
        {
            if (frameNo >= m_VideoController.VideoFirstFrame && frameNo <= m_VideoController.VideoLastFrame)
            {
                m_VideoController.MoveToFrame(frameNo);
            }
            else
                m_VideoController.ShowMessageBox(
                    string.Format("Tangra received a command from an Add-in to position to frame {0} however this is outside the allowed range of [{1}, {2}] for this video. Please report this problem to the Add-in author.", 
                        frameNo, m_VideoController.VideoFirstFrame, m_VideoController.VideoLastFrame),
                    "Tangra", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Error);
        }
	}
}
