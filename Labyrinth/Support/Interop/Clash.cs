using System;
using System.Runtime.InteropServices;

namespace Labyrinth.Support.Interop {
    public static class Clash {
        public static string ConfigDir { get; }

        [DllImport("clashffi", EntryPoint = "clash_start")]
        public static extern char Start();

        [DllImport("clashffi", EntryPoint = "clash_mmdb_ok")]
        public static extern bool IsMaxmindDatabaseOk();

        [DllImport("clashffi", EntryPoint = "clash_config_dir", CharSet = CharSet.Ansi)]
        private static extern IntPtr GetConfigDir();

        static Clash() {
            ConfigDir = Marshal.PtrToStringAnsi(GetConfigDir())!;
        }
    }
}
