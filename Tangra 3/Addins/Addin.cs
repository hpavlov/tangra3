using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Lifetime;
using System.Security;
using System.Security.Policy;
using System.Text;
using System.Xml.Serialization;
using Tangra.SDK;
using Tangra.Model.Helpers;

namespace Tangra.Addins
{
	[Serializable]
	public class Addin : IDisposable
	{
		private AppDomain m_HostDomain;
		private string m_DomainName;
		private AssemblyName m_AssemblyName;
		private ITangraAddin m_Instance;

		internal Addin(string fullTypeName, AddinManager addinManager)
		{

			string[] tokens = fullTypeName.Split(new char[] { ',' }, 2);

			var appSetup = new AppDomainSetup()
			{
				ApplicationName = "Tangra 3",
				ApplicationBase = AppDomain.CurrentDomain.BaseDirectory,
				ConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile,
				PrivateBinPath = @"Addins",
			};

			m_AssemblyName = new AssemblyName(tokens[1]);
			m_DomainName = string.Format("Tangra.Addins.{0}.v{1}", m_AssemblyName.Name, m_AssemblyName.Version.ToString());

			var e = new Evidence();
			e.AddHostEvidence(new Zone(SecurityZone.MyComputer));
			PermissionSet pset = SecurityManager.GetStandardSandbox(e);

			m_HostDomain = AppDomain.CreateDomain(m_DomainName, AppDomain.CurrentDomain.Evidence, appSetup, pset, null);
			m_HostDomain.AssemblyResolve += m_HostDomain_AssemblyResolve;
			m_HostDomain.ReflectionOnlyAssemblyResolve += m_HostDomain_AssemblyResolve;
			m_HostDomain.UnhandledException += m_HostDomain_UnhandledException;

			object obj = m_HostDomain.CreateInstanceAndUnwrap(tokens[1], tokens[0]);

			ILease lease = (ILease)(obj as MarshalByRefObject).GetLifetimeService();
			if (lease != null)
				lease.Register(addinManager.RemotingClientSponsor);

			m_Instance = (ITangraAddin)obj;
			m_Instance.Initialise(new TangraHostDelegate(fullTypeName, addinManager));
		}

		public XmlSerializer CreateXmlSettingsSerializer(Type settingsType)
		{
			return (XmlSerializer)m_HostDomain.CreateInstanceAndUnwrap(typeof(XmlSerializer).Assembly.FullName, "XmlSerializer", new object[] { settingsType });
		}

		void m_HostDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			if (e.ExceptionObject is Exception)
			{
				Trace.WriteLine(((Exception) e.ExceptionObject).FullExceptionInfo());
			}
		}

		Assembly m_HostDomain_AssemblyResolve(object sender, ResolveEventArgs args)
		{
			Assembly resolvedAssembly = null;

			string assemblyPath = Path.GetFullPath(string.Format(@"{0}\{1}.dll", Environment.CurrentDirectory, new AssemblyName(args.Name).Name));
			try
			{
				if (File.Exists(assemblyPath))
					resolvedAssembly = Assembly.LoadFile(assemblyPath);
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex.FullExceptionInfo());
			}


			return resolvedAssembly;
		}

		public void Dispose()
		{
			if (m_Instance != null)
				m_Instance.Finalise();

			if (m_HostDomain != null)
				AppDomain.Unload(m_HostDomain);

			m_HostDomain = null;
		}

		public ITangraAddin Instance
		{
			get { return m_Instance; }
		}
	}
}
