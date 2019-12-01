using System;
using System.Runtime.InteropServices;

namespace Labyrinth.Support.Interop {
    public static class Clash {
        public static string ConfigDir { get; }

        [DllImport("clashffi", EntryPoint = "clash_start")]
        public static extern char Start();

        [DllImport("clashffi", EntryPoint = "clash_mmdb_ok")]
        public static extern bool IsMaxmindDatabaseOk();

        [DllImport("clashffi", EntryPoint = "clash_config_dir")]
        private static extern IntPtr GetConfigDir();

        [DllImport("clashffi", EntryPoint = "free_cstr")]
        private static extern void FreeCStr(IntPtr str);

        static Clash() {
            IntPtr configDir = GetConfigDir();
            ConfigDir = Marshal.PtrToStringAnsi(configDir)!;
            FreeCStr(configDir);
        }
    }
}
