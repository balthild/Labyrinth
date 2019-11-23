using System;
using System.Runtime.InteropServices;

namespace Labyrinth.Support.Interop {
    public static class Clash {
        [DllImport("clashffi", EntryPoint = "clash_start")]
        public static extern char Start();
    }
}
