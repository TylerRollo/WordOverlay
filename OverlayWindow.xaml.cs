using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Threading;
using WordOverlay.Models;
using WordOverlay.Services;

namespace WordOverlay
{
    public partial class OverlayWindow : Window
    {
        private readonly WordService _wordService;
        private readonly ConfigService _configService;
        private readonly DispatcherTimer _rotationTimer;
        private readonly DispatcherTimer _pinToBottomTimer;
        private OverlayConfig _config;
        private IntPtr _hwnd;

        // --- Win32 interop for pinning the window behind all others (desktop level) ---
        private static readonly IntPtr HWND_BOTTOM = new IntPtr(1);
        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_NOMOVE = 0x0002;
        private const uint SWP_NOACTIVATE = 0x0010;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter,
            int x, int y, int cx, int cy, uint uFlags);

        public OverlayWindow()
        {
            InitializeComponent();

            _configService = new ConfigService();
            _config = _configService.Load();

            string wordsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "words.json");
            _wordService = new WordService(wordsPath);

            Width = _config.Width;
            Height = _config.Height;

            Loaded += OverlayWindow_Loaded;

            _rotationTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(Math.Max(15, _config.RotationSeconds))
            };
            _rotationTimer.Tick += (s, e) => NextWord();
            _rotationTimer.Start();

            // Windows periodically restacks windows (app launches, alt-tab, etc.)
            // Re-assert bottom-of-z-order on an interval so it stays pinned to the desktop.
            _pinToBottomTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(2)
            };
            _pinToBottomTimer.Tick += (s, e) => SendToBottom();
            _pinToBottomTimer.Start();
        }

        private void OverlayWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _hwnd = new WindowInteropHelper(this).Handle;

            PositionWindow();
            NextWord();
            SendToBottom();
        }

        private void SendToBottom()
        {
            if (_hwnd == IntPtr.Zero) return;

            SetWindowPos(_hwnd, HWND_BOTTOM, 0, 0, 0, 0,
                SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
        }

        private void PositionWindow()
        {
            var workArea = SystemParameters.WorkArea;

            if (_config.Left >= 0 && _config.Top >= 0)
            {
                // Restore saved position, clamped to the current work area.
                Left = Math.Min(Math.Max(_config.Left, workArea.Left), workArea.Right - Width);
                Top = Math.Min(Math.Max(_config.Top, workArea.Top), workArea.Bottom - Height);
            }
            else
            {
                // Default: top-right corner.
                Left = workArea.Right - Width - 16;
                Top = workArea.Top + 16;
            }
        }

        public void NextWord()
        {
            WordEntry entry = _wordService.GetNext();
            WordText.Text = entry.Word;
            DefinitionText.Text = entry.Definition;
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                DragMove();
                SendToBottom();
            }
        }

        private void ResizeThumbBottomRight_DragDelta(object sender, DragDeltaEventArgs e)
        {
            ResizeWidth(e.HorizontalChange);
            ResizeHeight(e.VerticalChange);
        }

        private void ResizeThumbRight_DragDelta(object sender, DragDeltaEventArgs e)
        {
            ResizeWidth(e.HorizontalChange);
        }

        private void ResizeThumbBottom_DragDelta(object sender, DragDeltaEventArgs e)
        {
            ResizeHeight(e.VerticalChange);
        }

        private void ResizeWidth(double horizontalChange)
        {
            double newWidth = Width + horizontalChange;
            if (newWidth >= MinWidth)
                Width = newWidth;
        }

        private void ResizeHeight(double verticalChange)
        {
            double newHeight = Height + verticalChange;
            if (newHeight >= MinHeight)
                Height = newHeight;
        }

        public void SaveConfigAndClose()
        {
            _rotationTimer.Stop();
            _pinToBottomTimer.Stop();

            _config.Left = Left;
            _config.Top = Top;
            _config.Width = Width;
            _config.Height = Height;

            _configService.Save(_config);
        }
    }
}
