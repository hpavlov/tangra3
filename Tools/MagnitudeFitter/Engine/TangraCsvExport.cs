using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagnitudeFitter.Engine
{
	internal class TangraCsvExport
	{
		private static char[] DELIMITERS = ",\t".ToCharArray();

		public string[] Headers;
		public List<TangraExportEntry> Entries = new List<TangraExportEntry>();

		public TangraCsvExport(string fileName)
		{
			if (File.Exists(fileName))
			{
				string[] lines = File.ReadAllLines(fileName);

				if (lines.Length > 1)
				{
					Headers = lines[0].Split(DELIMITERS);

					for (int i = 0; i < lines.Length; i++)
					{
						var entry = new TangraExportEntry(lines[i].Split(DELIMITERS));
						if (entry.IsValid)
							Entries.Add(entry);
					}
				}
			}
		}
	}
}
