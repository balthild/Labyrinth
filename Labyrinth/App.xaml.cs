﻿using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Labyrinth.Support.Interop;
using Labyrinth.ViewModels;
using Labyrinth.Views;

namespace Labyrinth {
    public class App : Application {
        public override void Initialize() {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted() {
            base.OnFrameworkInitializationCompleted();
            CreateWindow();
            RunClash();
        }

        private void CreateWindow() {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                desktop.MainWindow = new MainWindow {
                    DataContext = new MainWindowViewModel()
                };
        }

        private void RunClash() {
            Clash.Start();
        }
    }
}
