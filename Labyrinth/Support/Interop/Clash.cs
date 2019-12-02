using System;
using System.Runtime.InteropServices;

namespace Labyrinth.Support.Interop {
    public static class Clash {
        [StructLayout(LayoutKind.Sequential)]
        struct StartResultStruct {
            public char code;
            public IntPtr addr;
            public IntPtr secret;
        }

        public struct StartResult {
            public int code;
            public string addr;
            public string secret;
        }

        public static string ConfigDir { get; }

        public static StartResult Start() {
            IntPtr ptr = ClashStart();
            var structure = (StartResultStruct) Marshal.PtrToStructure(ptr, typeof(StartResultStruct))!;

            var result = new StartResult {
                code = structure.code,
            };
            if (structure.code == 0) {
                result.addr = Marshal.PtrToStringAnsi(structure.addr)!;
                result.secret = Marshal.PtrToStringAnsi(structure.secret)!;
                Free(structure.addr);
                Free(structure.secret);
            }

            Free(ptr);

            return result;
        }

        [DllImport("clashffi", EntryPoint = "clash_mmdb_ok")]
        public static extern bool IsMaxmindDatabaseOk();

        [DllImport("clashffi", EntryPoint = "clash_config_dir")]
        private static extern IntPtr GetConfigDir();

        [DllImport("clashffi", EntryPoint = "clash_start")]
        private static extern IntPtr ClashStart();

        [DllImport("clashffi", EntryPoint = "c_free")]
        private static extern void Free(IntPtr str);

        static Clash() {
            IntPtr configDir = GetConfigDir();
            ConfigDir = Marshal.PtrToStringAnsi(configDir)!;
            Free(configDir);
        }
    }
}
