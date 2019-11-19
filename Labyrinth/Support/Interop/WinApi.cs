using System;
using System.Runtime.InteropServices;

namespace Labyrinth.Support.Interop {
    public static class WinApi {
        // ReSharper disable InconsistentNaming
        public const uint WM_NCCALCSIZE = 0x0083;
        public const int GWL_STYLE = -16;
        public const uint WS_MAXIMIZEBOX = 0x00010000;
        // ReSharper restore InconsistentNaming

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
        public static extern int DwmSetWindowAttribute(IntPtr hWnd, int dw, ref int pv, int cb);

        [DllImport("dwmapi.dll", EntryPoint = "DwmIsCompositionEnabled")]
        public static extern int DwmIsCompositionEnabled(ref int pfEnabled);

        [DllImport("ComCtl32.dll", EntryPoint = "SetWindowSubclass")]
        public static extern bool SetWindowSubclass(IntPtr hWnd, WndProc fn, IntPtr id, IntPtr data);

        [DllImport("ComCtl32.dll", EntryPoint = "DefSubclassProc")]
        public static extern IntPtr DefSubclassProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
        public static extern uint GetWindowLongPtr(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
        public static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, uint value);
    }
}
