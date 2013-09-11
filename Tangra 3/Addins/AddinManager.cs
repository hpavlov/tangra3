using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Tangra.SDK;
using Tangra.Model.Helpers;

namespace Tangra.Addins
{
	[Serializable]
	public class AddinManager : IDisposable
	{
		private List<Addin> m_Addins = new List<Addin>();

		public void LoadAddins(ITangraHost host)
		{
			List<string> candiates = SearchAddins();
			foreach (string candiate in candiates)
			{
				m_Addins.Add(new Addin(candiate, host));
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
			foreach (Addin addin in m_Addins)
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

	}
}
