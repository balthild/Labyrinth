using System;
using System.Runtime.InteropServices;

namespace Labyrinth.Support.Interop {
    public static class WinApi {
        // ReSharper disable UnusedMember.Global
        // ReSharper disable InconsistentNaming
        // ReSharper disable IdentifierTypo
        public enum WindowMessage: uint {
            WM_NCCALCSIZE = 0x0083,
            WM_LBUTTONDBLCLK = 0x0203,
            WM_LBUTTONDOWN = 0x0201,
            WM_LBUTTONUP = 0x0202,
            WM_RBUTTONDBLCLK = 0x0206,
            WM_RBUTTONDOWN = 0x0204,
            WM_RBUTTONUP = 0x0205,

            WM_APP = 0x8000,
            WM_NOTIFYICON_CB = WM_APP + 1,
        }

        public enum WindowLongIndex {
            GWL_EXSTYLE = -20,
            GWL_STYLE = -16,
        }

        [Flags]
        public enum WindowStyle : uint {
            WS_MAXIMIZEBOX = 0x00010000,
        }

        [Flags]
        public enum WindowStyleEx : uint {
            WS_EX_APPWINDOW = 0x00040000,
            WS_EX_TOOLWINDOW = 0x00000080,
        }

        public enum NotifyIconMessage {
            NIM_ADD = 0x00000000,
            NIM_MODIFY = 0x00000001,
            NIM_DELETE = 0x00000002,
            NIM_SETVERSION = 0x00000004,
        }

        [Flags]
        public enum NotifyIconFlag {
            NIF_MESSAGE = 0x00000001,
            NIF_ICON = 0x00000002,
            NIF_TIP = 0x00000004,
            NIF_STATE = 0x00000008,
        }

        public enum NotifyIconState {
            NIS_HIDDEN = 0x00000001,
            NIS_SHAREDICON = 0x00000002,
        }
        // ReSharper restore InconsistentNaming
        // ReSharper restore UnusedMember.Global
        // ReSharper restore IdentifierTypo

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
            public NotifyIconFlag UFlags;
            public WindowMessage UCallbackMessage;
            public IntPtr HIcon;
            public IntPtr SzTip;
            public NotifyIconState DwState;
            public NotifyIconState DwStateMask;
            public IntPtr SzInfo;
            public uint UVersion;
            public IntPtr SzInfoTitle;
            public uint DwInfoFlags;
            public Guid GuidItem;
        }

        public delegate IntPtr WndProc(IntPtr hWnd, WindowMessage uMsg, IntPtr wParam, IntPtr lParam);

        [DllImport("dwmapi.dll", EntryPoint = "DwmExtendFrameIntoClientArea")]
        public static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref Margins pMarInset);

        [DllImport("dwmapi.dll", EntryPoint = "DwmSetWindowAttribute")]
        public static extern int DwmSetWindowAttribute(IntPtr hWnd, int dw, ref int pv, int cb);

        [DllImport("dwmapi.dll", EntryPoint = "DwmIsCompositionEnabled")]
        public static extern int DwmIsCompositionEnabled(ref int pfEnabled);

        [DllImport("ComCtl32.dll", EntryPoint = "SetWindowSubclass")]
        public static extern bool SetWindowSubclass(IntPtr hWnd, WndProc fn, IntPtr id, IntPtr data);

        [DllImport("ComCtl32.dll", EntryPoint = "DefSubclassProc")]
        public static extern IntPtr DefSubclassProc(IntPtr hWnd, WindowMessage uMsg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
        public static extern uint GetWindowLongPtr(IntPtr hWnd, WindowLongIndex nIndex);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
        public static extern IntPtr SetWindowLongPtr(IntPtr hWnd, WindowLongIndex nIndex, uint value);

        [DllImport("user32.dll", EntryPoint = "SetForegroundWindow")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("shell32.dll", EntryPoint = "Shell_NotifyIconW")]
        public static extern bool ShellNotifyIconW(NotifyIconMessage message, IntPtr data);
    }
}
