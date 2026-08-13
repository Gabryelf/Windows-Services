using System;
using System.Runtime.InteropServices;

namespace FolderIconService
{
    public static class NativeMethods
    {
        [DllImport("shell32.dll")]
        public static extern void SHChangeNotify(
            uint wEventId,
            uint uFlags,
            IntPtr dwItem1,
            IntPtr dwItem2
        );
    }
}