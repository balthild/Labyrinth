using System;
using System.Reflection;
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
                HasSystemDecorations = false;
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
                // WindowEffectsX11();
                // TODO: The implementation above is incorrect
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

        private void WindowEffectsX11() {
            var functions = X11.MotifFunctions.Move | X11.MotifFunctions.Close | X11.MotifFunctions.Minimize;
            var decorations = X11.MotifDecorations.Border;

            var hints = new X11.MotifWmHints {
                flags = new IntPtr((int) (X11.MotifFlags.Decorations | X11.MotifFlags.Functions)),
                decorations = new IntPtr((int) decorations),
                functions = new IntPtr((int) functions)
            };

            Type type = PlatformImpl.GetType();
            FieldInfo field = type.GetField("_x11", BindingFlags.NonPublic | BindingFlags.Instance);
            dynamic info = field?.GetValue(PlatformImpl);

            if (info == null)
                return;

            X11.XChangeProperty(info.Display, PlatformImpl.Handle.Handle,
                info.Atoms._MOTIF_WM_HINTS, info.Atoms._MOTIF_WM_HINTS, 32,
                X11.PropertyMode.Replace, ref hints, 5);
        }

        private static IntPtr Win32SubclassWndProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam) {
            return uMsg switch {
                WinApi.WM_NCCALCSIZE => IntPtr.Zero,
                _ => WinApi.DefSubclassProc(hWnd, uMsg, wParam, lParam),
            };
        }
    }
}
