using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;
using System.Text;
using Tangra.SDK;
using Tangra.Model.Helpers;

namespace Tangra.Addins
{
	[Serializable]
	public class Addin : IDisposable
	{
		private AppDomain m_HostDomain;
		private string m_DomainName;
		private ITangraAddin m_Instance;

		internal Addin(string fullTypeName, ITangraHost host)
		{
			string[] tokens = fullTypeName.Split(new char[] { ',' }, 2);

			var appSetup = new AppDomainSetup()
			{
				ApplicationName = "Tangra 3",
				ApplicationBase = AppDomain.CurrentDomain.BaseDirectory,
				ConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile,
				PrivateBinPath = @"Addins",
			};

			var assemblyName = new AssemblyName(tokens[1]);
			m_DomainName = string.Format("Tangra.Addins.{0}.v{1}", assemblyName.Name, assemblyName.Version.ToString());

			m_HostDomain = AppDomain.CreateDomain(m_DomainName, AppDomain.CurrentDomain.Evidence, appSetup);
			m_HostDomain.AssemblyResolve += m_HostDomain_AssemblyResolve;
			m_HostDomain.ReflectionOnlyAssemblyResolve += m_HostDomain_AssemblyResolve;
			m_HostDomain.UnhandledException += m_HostDomain_UnhandledException;

			object obj = m_HostDomain.CreateInstanceAndUnwrap(tokens[1], tokens[0]);

			m_Instance = (ITangraAddin)obj;
			m_Instance.Initialise(host);
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

			try
			{
				resolvedAssembly = Assembly.Load(args.Name);
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex.FullExceptionInfo());
			}

			if (resolvedAssembly == null)
			{
				string assemblyPath = Path.GetFullPath(string.Format(@"{0}\{1}.dll", Environment.CurrentDirectory, new AssemblyName(args.Name).Name));
				try
				{
					resolvedAssembly = Assembly.LoadFile(assemblyPath);
				}
				catch (Exception ex)
				{
					Trace.WriteLine(ex.FullExceptionInfo());
				}
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
