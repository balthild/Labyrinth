using System;
using System.Runtime.InteropServices;

namespace Labyrinth.Support.Interop {
    public static class WinApi {
        // ReSharper disable InconsistentNaming
        public const uint WM_NCCALCSIZE = 0x0083;
        public const uint WM_LBUTTONDBLCLK = 0x0203;
        public const uint WM_LBUTTONDOWN = 0x0201;
        public const uint WM_LBUTTONUP = 0x0202;
        public const uint WM_RBUTTONDBLCLK = 0x0206;
        public const uint WM_RBUTTONDOWN = 0x0204;
        public const uint WM_RBUTTONUP = 0x0205;

        public const uint WM_APP = 0x8000;
        public const uint WM_NOTIFYICON_CB = WM_APP + 1;

        public const int GWL_STYLE = -16;

        public const uint WS_MAXIMIZEBOX = 0x00010000;

        public const int NIM_ADD = 0x00000000;
        public const int NIM_MODIFY = 0x00000001;
        public const int NIM_DELETE = 0x00000002;
        public const int NIM_SETVERSION = 0x00000004;

        public const int NIF_MESSAGE = 0x00000001;
        public const int NIF_ICON = 0x00000002;
        public const int NIF_TIP = 0x00000004;
        public const int NIF_STATE = 0x00000008;

        public const int NIS_HIDDEN = 0x00000001;
        public const int NIS_SHAREDICON = 0x00000002;

        public const uint LR_DEFAULTCOLOR = 0x00000000;
        public const uint LR_DEFAULTSIZE= 0x00000040;
        public const uint LR_MONOCHROME = 0x00000001;
        public const uint LR_SHARED = 0x00008000;
        // ReSharper restore InconsistentNaming

        [StructLayout(LayoutKind.Sequential)]
        public struct Margins {
            public int Left;
            public int Right;
            public int Top;
            public int Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Guid {
            public uint Data1;
            public ushort Data2;
            public ushort Data3;
            public IntPtr Data4;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NotifyIconData {
            public uint CbSize;
            public IntPtr HWnd;
            public uint UID;
            public uint UFlags;
            public uint UCallbackMessage;
            public IntPtr HIcon;
            public IntPtr SzTip;
            public uint DwState;
            public uint DwStateMask;
            public IntPtr SzInfo;
            public uint UVersion;
            public IntPtr SzInfoTitle;
            public uint DwInfoFlags;
            public Guid GuidItem;
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

        [DllImport("shell32.dll", EntryPoint = "Shell_NotifyIconW")]
        public static extern bool ShellNotifyIconW(int message, IntPtr data);
    }
}
