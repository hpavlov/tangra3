using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Tangra.Helpers
{
	internal class WindowsHelpers
	{
		[DllImport("user32", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int SendMessage(IntPtr hWnd, UInt32 Msg, int wParam, IntPtr lParam);

		public const UInt32 BCM_SETSHIELD = 0x160C;
	}
}
