/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Tangra.StarCatalogues.UCAC2
{
    public class UCAC2FileIterator
    {
        public static IEnumerator<string> UCAC2Files(string basePath)
        {
            for (int i = 1; i <= 288; i++)
            {
                yield return Path.GetFullPath(string.Format("{0}\\z{1}", basePath, i.ToString("000")));
            }

            yield return Path.GetFullPath(string.Format("{0}\\ucac2bss", basePath));
        }
    }
}
