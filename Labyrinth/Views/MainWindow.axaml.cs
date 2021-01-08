using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Markup.Xaml;
using Avalonia.Platform;
using Labyrinth.Controls;
using static Labyrinth.Support.Interop.WinApi;

namespace Labyrinth.Views {
    public class MainWindow : BorderlessWindow {
        // Prevent the delegate being garbage collected
        private WndProc? subclassWndProc;

        private NotifyIconData notifyIconData;

        private bool keepRunning = true;

        public MainWindow() {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif

            AddStatusIcon();
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }

        private void AddStatusIcon() {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                AddNotifyIconWin32();
            }
        }

        private void AddNotifyIconWin32() {
            IntPtr hWnd = PlatformImpl.Handle.Handle;

            var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
            // TODO: Design my own icon
            using Stream iconStream = assets.Open(new Uri("avares://Labyrinth/Assets/avalonia-logo.ico"));
            var icon = new Icon(iconStream);

            notifyIconData = new NotifyIconData {
                HWnd = hWnd,
                UFlags = NotifyIconFlag.NIF_MESSAGE | NotifyIconFlag.NIF_ICON | NotifyIconFlag.NIF_STATE,
                DwState = NotifyIconState.NIS_HIDDEN,
                DwStateMask = NotifyIconState.NIS_HIDDEN,
                UCallbackMessage = WindowMessage.WM_NOTIFYICON_CB,
                HIcon = icon.Handle,
            };

            int size = Marshal.SizeOf(notifyIconData);
            notifyIconData.CbSize = (uint) size;

            IntPtr dataPtr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(notifyIconData, dataPtr, true);

            // Create the notify icon
            ShellNotifyIconW(NotifyIconMessage.NIM_ADD, dataPtr);

            // Process mouse events on the icon
            subclassWndProc = Win32SubclassWndProc;
            SetWindowSubclass(hWnd, subclassWndProc, IntPtr.Zero, IntPtr.Zero);
        }

        private void RemoveStatusIcon() {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                RemoveNotifyIconWin32();
            }
        }

        private void RemoveNotifyIconWin32() {
            int size = Marshal.SizeOf(notifyIconData);
            IntPtr dataPtr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(notifyIconData, dataPtr, true);

            ShellNotifyIconW(NotifyIconMessage.NIM_DELETE, dataPtr);
        }

        private IntPtr Win32SubclassWndProc(IntPtr hWnd, WindowMessage uMsg, IntPtr wParam, IntPtr lParam) {
            if (uMsg != WindowMessage.WM_NOTIFYICON_CB)
                return DefSubclassProc(hWnd, uMsg, wParam, lParam);

            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
            switch ((WindowMessage) lParam) {
                case WindowMessage.WM_RBUTTONUP:
                case WindowMessage.WM_LBUTTONUP:
                    Show();
                    Activate();
                    break;
            }

            return IntPtr.Zero;
        }

        protected override bool HandleClosing() {
            bool cancelled = keepRunning;
            if (cancelled) Hide();
            return cancelled;
        }

        public void Exit() {
            keepRunning = false;
            RemoveStatusIcon();
            Close();
        }
    }
}
