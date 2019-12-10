using System;
using System.Runtime.InteropServices;
using Avalonia.Controls;
using Labyrinth.Support.Interop;

namespace Labyrinth.Controls {
    public class BorderlessWindow : Window {
        // Prevent the delegate being garbage collected
        private static readonly WinApi.WndProc SubclassWndProc = Win32SubclassWndProc;

        internal BorderlessWindow() {
            WindowEffects();
        }

        private void WindowEffects() {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                WindowEffectsWin32();
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
                // TODO
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
                // TODO
            }
        }

        private void WindowEffectsWin32() {
            int dwmEnabled = 0;
            WinApi.DwmIsCompositionEnabled(ref dwmEnabled);
            if (dwmEnabled == 0)
                return;

            IntPtr hWnd = PlatformImpl.Handle.Handle;

            // Disable window maximizing
            uint style = WinApi.GetWindowLongPtr(hWnd, WinApi.GWL_STYLE);
            style &= ~WinApi.WS_MAXIMIZEBOX;
            WinApi.SetWindowLongPtr(hWnd, WinApi.GWL_STYLE, style);

            // Make window looks "border-less" but keep effects such as aero peek and minimizing animations
            WinApi.SetWindowSubclass(hWnd, SubclassWndProc, IntPtr.Zero, IntPtr.Zero);

            // Allow drawing in non-client area
            int pv = 2;
            WinApi.DwmSetWindowAttribute(hWnd, 2, ref pv, 4);

            // Enable window shadows
            var m = new WinApi.Margins { Left = 1, Right = 1, Top = 1, Bottom = 1 };
            WinApi.DwmExtendFrameIntoClientArea(hWnd, ref m);
        }

        private static IntPtr Win32SubclassWndProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam) {
            return uMsg switch {
                WinApi.WM_NCCALCSIZE => IntPtr.Zero,
                _ => WinApi.DefSubclassProc(hWnd, uMsg, wParam, lParam),
            };
        }
    }
}
