using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform;
using Avalonia.Xaml.Interactivity;
using JetBrains.Annotations;
using Labyrinth.Controls;
using Labyrinth.Support.Interop;

namespace Labyrinth.Views {
    public class MainWindow : BorderlessWindow {
        // Prevent the delegate being garbage collected
        private WinApi.WndProc subclassWndProc;

        private WinApi.NotifyIconData notifyIconData;

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

            this.FindControl<Control>("Title").PointerPressed += (i, e) => {
                if (((Control) e.Source).Classes.Contains("drag")) {
                    BeginMoveDrag(e);
                }
            };
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

            notifyIconData = new WinApi.NotifyIconData {
                HWnd = hWnd,
                UFlags = WinApi.NIF_MESSAGE | WinApi.NIF_ICON | WinApi.NIF_STATE,
                DwState = WinApi.NIS_HIDDEN,
                DwStateMask = WinApi.NIS_HIDDEN,
                UCallbackMessage = WinApi.WM_NOTIFYICON_CB,
                HIcon = icon.Handle
            };

            int size = Marshal.SizeOf(notifyIconData);
            notifyIconData.CbSize = (uint) size;

            IntPtr dataPtr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(notifyIconData, dataPtr, true);

            // Create the notify icon
            WinApi.ShellNotifyIconW(WinApi.NIM_ADD, dataPtr);

            // Process mouse events on the icon
            subclassWndProc = Win32SubclassWndProc;
            WinApi.SetWindowSubclass(hWnd, subclassWndProc, IntPtr.Zero, IntPtr.Zero);
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

            WinApi.ShellNotifyIconW(WinApi.NIM_DELETE, dataPtr);
        }

        private IntPtr Win32SubclassWndProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam) {
            if (uMsg != WinApi.WM_NOTIFYICON_CB)
                return WinApi.DefSubclassProc(hWnd, uMsg, wParam, lParam);

            switch ((uint) lParam) {
                case WinApi.WM_LBUTTONDBLCLK:
                    Show();
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

        [UsedImplicitly]
        public void AddCurrentTabClass(object target, AvaloniaPropertyChangedEventArgs args) {
            var behavior = (Behavior) args.Sender;
            var tab = (Control) behavior.AssociatedObject!;
            tab?.Classes.Add("current");
        }

        [UsedImplicitly]
        public void RemoveCurrentTabClass(object target, AvaloniaPropertyChangedEventArgs args) {
            var behavior = (Behavior) args.Sender;
            var tab = (Control) behavior.AssociatedObject!;
            tab?.Classes.Remove("current");
        }

        [UsedImplicitly]
        public void CloseWindow(object sender, RoutedEventArgs args) {
            Close();
        }
    }
}
