using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace GeminiScreenPet
{
    public partial class TrailOverlay : Window
    {
        #region Win32 Pass-Through
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_TRANSPARENT = 0x00000020;
        private const int WS_EX_TOOLWINDOW = 0x00000080;
        private const int WS_EX_LAYERED = 0x00080000;
        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        #endregion

        public static TrailOverlay? Instance { get; private set; }

        private readonly List<Particle> _particles = new();
        private readonly DispatcherTimer _timer = new();
        private readonly Random _rand = new();

        private class Particle
        {
            public FrameworkElement Element { get; set; } = null!;
            public double X { get; set; }
            public double Y { get; set; }
            public double Vx { get; set; }
            public double Vy { get; set; }
            public double Size { get; set; }
            public double Opacity { get; set; } = 1.0;
            public double Decay { get; set; } = 0.05;
        }

        public TrailOverlay()
        {
            InitializeComponent();
            Instance = this;

            Width = SystemParameters.VirtualScreenWidth;
            Height = SystemParameters.VirtualScreenHeight;
            Left = SystemParameters.VirtualScreenLeft;
            Top = SystemParameters.VirtualScreenTop;

            Loaded += (s, e) =>
            {
                IntPtr hwnd = new WindowInteropHelper(this).Handle;
                int ex = GetWindowLong(hwnd, GWL_EXSTYLE);
                SetWindowLong(hwnd, GWL_EXSTYLE, ex | WS_EX_TRANSPARENT | WS_EX_TOOLWINDOW | WS_EX_LAYERED);
                SetWindowPos(hwnd, HWND_TOPMOST, 0, 0, 0, 0, 0x0001 | 0x0002 | 0x0010 | 0x0040);
            };

            _timer.Interval = TimeSpan.FromMilliseconds(25); // 40 FPS smooth particles
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        public void Clear()
        {
            _particles.Clear();
            OverlayCanvas.Children.Clear();
        }

        public void Emit(double screenX, double screenY, MainWindow.TrailType type)
        {
            if (type == MainWindow.TrailType.None) return;

            Brush fill;
            switch (type)
            {
                case MainWindow.TrailType.Cyan:
                    fill = new SolidColorBrush(Color.FromArgb(220, 56, 189, 248));
                    break;
                case MainWindow.TrailType.Rainbow:
                    byte r = (byte)_rand.Next(100, 255);
                    byte g = (byte)_rand.Next(100, 255);
                    byte b = (byte)_rand.Next(100, 255);
                    fill = new SolidColorBrush(Color.FromArgb(230, r, g, b));
                    break;
                case MainWindow.TrailType.Fire:
                    fill = new SolidColorBrush(_rand.Next(2) == 0 ? Color.FromArgb(230, 249, 115, 22) : Color.FromArgb(230, 250, 204, 21));
                    break;
                case MainWindow.TrailType.Pink:
                    fill = new SolidColorBrush(Color.FromArgb(230, 244, 114, 182));
                    break;
                case MainWindow.TrailType.Sparkle:
                default:
                    fill = new SolidColorBrush(Color.FromArgb(240, 255, 255, 255));
                    break;
            }

            double size = _rand.Next(6, 12);
            Shape shape;
            if (type == MainWindow.TrailType.Sparkle || _rand.Next(3) == 0)
            {
                shape = new Rectangle 
                { 
                    Width = size, 
                    Height = size, 
                    Fill = fill,
                    RenderTransform = new RotateTransform(_rand.Next(0, 90))
                };
            }
            else
            {
                shape = new Ellipse { Width = size, Height = size, Fill = fill };
            }

            Canvas.SetLeft(shape, screenX - Left);
            Canvas.SetTop(shape, screenY - Top);
            OverlayCanvas.Children.Add(shape);

            _particles.Add(new Particle
            {
                Element = shape,
                X = screenX - Left,
                Y = screenY - Top,
                Vx = (_rand.NextDouble() - 0.5) * 1.5,
                Vy = (_rand.NextDouble() - 0.5) * 1.5 + 0.5,
                Size = size,
                Opacity = 1.0,
                Decay = _rand.NextDouble() * 0.03 + 0.035
            });
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            for (int i = _particles.Count - 1; i >= 0; i--)
            {
                var p = _particles[i];
                p.X += p.Vx;
                p.Y += p.Vy;
                p.Opacity -= p.Decay;
                p.Size = Math.Max(0.5, p.Size - 0.25);

                Canvas.SetLeft(p.Element, p.X);
                Canvas.SetTop(p.Element, p.Y);
                p.Element.Opacity = Math.Max(0, p.Opacity);
                p.Element.Width = p.Size;
                p.Element.Height = p.Size;

                if (p.Opacity <= 0)
                {
                    OverlayCanvas.Children.Remove(p.Element);
                    _particles.RemoveAt(i);
                }
            }
        }
    }
}
