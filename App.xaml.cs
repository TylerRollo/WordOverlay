using System;
using System.Windows;
using System.Windows.Forms;
using Microsoft.Win32;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace WordOverlay
{
    public partial class App : Application
    {
        private NotifyIcon? _trayIcon;
        private OverlayWindow? _overlay;

        private const string RunKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
        private const string RunValueName = "WordOverlay";

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            ShutdownMode = ShutdownMode.OnExplicitShutdown;

            RegisterAutostart();

            _overlay = new OverlayWindow();
            _overlay.Show();

            CreateTrayIcon();
        }

        private void CreateTrayIcon()
        {
            _trayIcon = new NotifyIcon
            {
                Icon = System.Drawing.SystemIcons.Application,
                Visible = true,
                Text = "Word Overlay"
            };

            var contextMenu = new ContextMenuStrip();

            var toggleItem = new ToolStripMenuItem("Show/Hide");
            toggleItem.Click += (s, e) => ToggleOverlay();
            contextMenu.Items.Add(toggleItem);

            var nextItem = new ToolStripMenuItem("Next Word");
            nextItem.Click += (s, e) => _overlay?.NextWord();
            contextMenu.Items.Add(nextItem);

            contextMenu.Items.Add(new ToolStripSeparator());

            var exitItem = new ToolStripMenuItem("Exit");
            exitItem.Click += (s, e) => ExitApplication();
            contextMenu.Items.Add(exitItem);

            _trayIcon.ContextMenuStrip = contextMenu;
            _trayIcon.DoubleClick += (s, e) => ToggleOverlay();
        }

        private void ToggleOverlay()
        {
            if (_overlay == null) return;

            if (_overlay.IsVisible)
                _overlay.Hide();
            else
                _overlay.Show();
        }

        private void ExitApplication()
        {
            _overlay?.SaveConfigAndClose();

            if (_trayIcon != null)
            {
                _trayIcon.Visible = false;
                _trayIcon.Dispose();
            }

            Shutdown();
        }

        private static void RegisterAutostart()
        {
            try
            {
                string exePath = Environment.ProcessPath
                    ?? System.Diagnostics.Process.GetCurrentProcess().MainModule?.FileName
                    ?? string.Empty;

                if (string.IsNullOrEmpty(exePath))
                    return;

                using RegistryKey? key = Registry.CurrentUser.OpenSubKey(RunKeyPath, writable: true);
                key?.SetValue(RunValueName, $"\"{exePath}\"");
            }
            catch (Exception ex)
            {
                // Non-fatal: autostart registration failing shouldn't block the app.
                System.Diagnostics.Debug.WriteLine($"Failed to register autostart: {ex.Message}");
            }
        }
    }
}
