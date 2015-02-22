/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Tangra.StarCatalogues.UCAC4
{
	public class UCAC4FileIterator
	{
		public static IEnumerator<string> UCAC4Files(string basePath)
		{
			for (int i = 1; i <= 900; i++)
			{
				yield return Path.GetFullPath(string.Format("{0}\\z{1}", basePath, i.ToString("000")));
			}
		}

	}
}
