using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Lifetime;
using System.Text;
using System.Windows.Forms;
using Tangra.SDK;
using Tangra.Model.Helpers;

namespace Tangra.Addins
{
	public interface IAddinManager
	{
		IWin32Window ParentWindow { get; }
		ILightCurveDataProvider LightCurveDataProvider { get; }
	}

	internal class RemotingClientSponsor : MarshalByRefObject, ISponsor
	{
		public TimeSpan Renewal(ILease lease)
		{
			TimeSpan tsLease = TimeSpan.FromMinutes(5);
			lease.Renew(tsLease);
			return tsLease;
		}
	}

	[Serializable]
	public class AddinManager : IDisposable, IAddinManager
	{
		public readonly List<Addin> Addins = new List<Addin>();

		private frmMain m_MainForm;
		private ILightCurveDataProvider m_lcDataProvider;

		internal RemotingClientSponsor RemotingClientSponsor = new RemotingClientSponsor();

		public AddinManager(frmMain mainForm)
		{
			m_MainForm = mainForm;
		}

		public void LoadAddins()
		{
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
					Trace.WriteLine(ex.FullExceptionInfo());
					instance = null;
				}

				if (instance != null)
					Addins.Add(instance);
			}
		}

		private List<string> SearchAddins()
		{
			var addins = new List<string>();

			string addinsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Addins");

			if (Directory.Exists(addinsDir))
			{
				string[] names = Directory.GetFiles(addinsDir, "*.dll");

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
						Trace.WriteLine(ex.FullExceptionInfo());
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
					Trace.WriteLine(ex.FullExceptionInfo());
				}
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
	}
}
