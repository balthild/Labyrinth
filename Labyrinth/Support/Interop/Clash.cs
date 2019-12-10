using System;
using System.Runtime.InteropServices;

namespace Labyrinth.Support.Interop {
    public static class Clash {
        [StructLayout(LayoutKind.Sequential)]
        struct StartResultStruct {
            public int code;
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
            StartResultStruct ret = ClashStart();

            var result = new StartResult {
                code = ret.code,
            };

            if (ret.code == 0) {
                result.addr = Marshal.PtrToStringAnsi(ret.addr)!;
                result.secret = Marshal.PtrToStringAnsi(ret.secret)!;
                Free(ret.addr);
                Free(ret.secret);
            }

            return result;
        }

        public static string? ValidateConfig(byte[] data) {
            IntPtr dataPtr = Marshal.AllocHGlobal(data.Length);
            Marshal.Copy(data, 0, dataPtr, data.Length);
            IntPtr resultPtr = ClashValidateConfig(dataPtr, data.Length);
            Marshal.FreeHGlobal(dataPtr);

            string? cause = Marshal.PtrToStringAnsi(resultPtr);

            if (resultPtr != IntPtr.Zero)
                Free(resultPtr);

            return cause;
        }

        [DllImport("clashffi", EntryPoint = "clash_mmdb_ok")]
        public static extern bool IsMaxmindDatabaseOk();

        [DllImport("clashffi", EntryPoint = "clash_config_dir")]
        private static extern IntPtr GetConfigDir();

        [DllImport("clashffi", EntryPoint = "clash_validate_config")]
        private static extern IntPtr ClashValidateConfig(IntPtr ptr, int len);

        [DllImport("clashffi", EntryPoint = "clash_start")]
        private static extern StartResultStruct ClashStart();

        [DllImport("clashffi", EntryPoint = "c_free")]
        private static extern void Free(IntPtr str);

        static Clash() {
            IntPtr configDir = GetConfigDir();
            ConfigDir = Marshal.PtrToStringAnsi(configDir)!;
            Free(configDir);
        }
    }
}
