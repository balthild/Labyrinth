using System;
using System.Runtime.InteropServices;

namespace Labyrinth.Support.Interop {
    public static class Clash {
        [StructLayout(LayoutKind.Sequential)]
        private readonly struct StartResultInterop {
            public readonly int code;
            public readonly IntPtr addr;
            public readonly IntPtr secret;
        }

        public struct StartResult {
            public int Code;
            public string? Addr;
            public string? Secret;
        }

        [StructLayout(LayoutKind.Sequential)]
        private readonly struct ConfigStatsInterop {
            public readonly int Code;
            public readonly int ProxyCount;
            public readonly int ProxyGroupCount;
            public readonly int RuleCount;
        }

        public struct ConfigStats {
            public int ProxyCount;
            public int ProxyGroupCount;
            public int RuleCount;
        }

        public static string ConfigDir { get; }

        public static StartResult Start() {
            StartResultInterop ret = ClashStart();

            var result = new StartResult {
                Code = ret.code,
            };

            if (ret.code == 0) {
                result.Addr = Marshal.PtrToStringAnsi(ret.addr)!;
                result.Secret = Marshal.PtrToStringAnsi(ret.secret)!;
                CFree(ret.addr);
                CFree(ret.secret);
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
                CFree(resultPtr);

            return cause;
        }

        public static ConfigStats? GetConfigStats(byte[] data) {
            IntPtr dataPtr = Marshal.AllocHGlobal(data.Length);
            Marshal.Copy(data, 0, dataPtr, data.Length);
            ConfigStatsInterop result = ClashGetConfigStats(dataPtr, data.Length);
            Marshal.FreeHGlobal(dataPtr);

            if (result.Code != 0) {
                return null;
            }

            return new ConfigStats {
                ProxyCount = result.ProxyCount,
                ProxyGroupCount = result.ProxyGroupCount,
                RuleCount = result.RuleCount,
            };
        }

        [DllImport("clashffi", EntryPoint = "clash_mmdb_ok")]
        public static extern bool IsMaxmindDatabaseOk();

        [DllImport("clashffi", EntryPoint = "clash_config_dir")]
        private static extern IntPtr GetConfigDir();

        [DllImport("clashffi", EntryPoint = "clash_validate_config")]
        private static extern IntPtr ClashValidateConfig(IntPtr ptr, int length);

        [DllImport("clashffi", EntryPoint = "clash_get_config_stats")]
        private static extern ConfigStatsInterop ClashGetConfigStats(IntPtr ptr, int length);

        [DllImport("clashffi", EntryPoint = "clash_start")]
        private static extern StartResultInterop ClashStart();

        [DllImport("clashffi", EntryPoint = "c_free")]
        private static extern void CFree(IntPtr str);

        static Clash() {
            IntPtr configDir = GetConfigDir();
            ConfigDir = Marshal.PtrToStringAnsi(configDir)!;
            CFree(configDir);
        }
    }
}
