using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Tangra.NativeWrapper
{
    public static class Core
    {
        private const string TANGRA_CORE_DLL_NAME = "Tangra.Core.dll";

        [DllImport(TANGRA_CORE_DLL_NAME)]
        private static extern void AdvNewFile(byte[] fileName, byte[] compression, int keyFrames);

        public static void NewFile(string fileName, string compression, int keyFrames)
        {
            AdvNewFile(Encoding.ASCII.GetBytes(fileName), Encoding.ASCII.GetBytes(compression), keyFrames);
        }
    }
}
