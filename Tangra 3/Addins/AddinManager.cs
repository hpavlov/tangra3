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

		internal RemotingClientSponsor RemotingClientSponsor = new RemotingClientSponsor();

        public AddinManager(frmMain mainForm, VideoController videoController)
		{
			m_MainForm = mainForm;
            m_VideoController = videoController;

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
							.Where(t => t.IsClass && t.GetInterface(typeof(ITangraAddin).FullName) != null)
							.Select(t => string.Concat(t.FullName, ",", loadedAssembly.FullName))
							.ToList();

						addins.AddRange(addinTypesFound);
					}
					catch (Exception ex)
					{
						Trace.WriteLine(ex.GetFullStackTrace());
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

		IWin32Window IAddinManager.ParentWindow
		{
			get { return m_MainForm; }
		}

		ILightCurveDataProvider IAddinManager.LightCurveDataProvider
		{
			get { return m_lcDataProvider; }
		}

        void IAddinManager.PositionToFrame(int frameNo)
        {
            m_VideoController.MoveToFrame(frameNo);
        }
	}
}
