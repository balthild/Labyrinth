using System;
using System.Runtime.InteropServices;

namespace Labyrinth.Support {
    public static class WindowsInterop {
        public enum WindowMsg : uint {
            WM_NCCALCSIZE = 0x0083
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Margins {
            public int Left;
            public int Right;
            public int Top;
            public int Bottom;
        }

        public delegate IntPtr WndProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);

        [DllImport("dwmapi.dll", EntryPoint = "DwmExtendFrameIntoClientArea")]
        public static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref Margins pMarInset);

        [DllImport("dwmapi.dll", EntryPoint = "DwmSetWindowAttribute")]
        public static extern int DwmSetWindowAttribute(IntPtr hWnd, int dwAttribute, ref int pvAttribute, int cbAttribute);

        [DllImport("dwmapi.dll", EntryPoint = "DwmIsCompositionEnabled")]
        public static extern int DwmIsCompositionEnabled(ref int pfEnabled);

        [DllImport("ComCtl32.dll", EntryPoint = "SetWindowSubclass")]
        public static extern bool SetWindowSubclass(IntPtr hWnd, WndProc pfnSubclass, IntPtr uIdSubclass, IntPtr dwRefData);

        [DllImport("ComCtl32.dll", EntryPoint = "DefSubclassProc")]
        public static extern IntPtr DefSubclassProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);
    }
}
