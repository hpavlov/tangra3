 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tangra.Model.Helpers
{
    public class CommandLineParser
    {
        public static Dictionary<string, string> Parse(string[] args)
        {
            var rv = new Dictionary<string, string>();

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == null) continue;

                string key = string.Empty;
                string val = string.Empty;

                if (args[i].StartsWith("-"))
                {
                    key = args[i].Substring(1).ToLower();

                    if (i + 1 < args.Length && !args[i + 1].StartsWith("-"))
                    {
                        val = args[i + 1];
                        i++;
                    }
                }
                else
                    val = args[i];

                rv[key] = val;
            }

            return rv;
        }

    }
}
