/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Tangra.StarCatalogues.PPMXL
{
    public class PPMXLFileIterator
    {
        public static IEnumerator<string> PPMXLFiles(string basePath)
        {
            for (int i = 89; i >= 0; i--)
            {
                yield return Path.GetFullPath(string.Format("{0}\\s{1}d.dat", basePath, i.ToString("00")));
                yield return Path.GetFullPath(string.Format("{0}\\s{1}c.dat", basePath, i.ToString("00")));
                yield return Path.GetFullPath(string.Format("{0}\\s{1}b.dat", basePath, i.ToString("00")));
                yield return Path.GetFullPath(string.Format("{0}\\s{1}a.dat", basePath, i.ToString("00")));
            }

            for (int i = 0; i <= 89; i++)
            {
                yield return Path.GetFullPath(string.Format("{0}\\n{1}a.dat", basePath, i.ToString("00")));
                yield return Path.GetFullPath(string.Format("{0}\\n{1}b.dat", basePath, i.ToString("00")));
                yield return Path.GetFullPath(string.Format("{0}\\n{1}c.dat", basePath, i.ToString("00")));
                yield return Path.GetFullPath(string.Format("{0}\\n{1}d.dat", basePath, i.ToString("00")));
            }
        }
    }
}
