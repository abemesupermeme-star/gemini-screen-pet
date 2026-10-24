using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using GeminiScreenPet.Animations;
using GeminiScreenPet.Models;
using GeminiScreenPet.Services;

namespace GeminiScreenPet
{
    public partial class MainWindow : Window
    {
        private static readonly List<MainWindow> _activePets = new();
        private readonly PixelRenderer _renderer;
        private readonly DispatcherTimer _timer = new();
        private readonly DispatcherTimer _clickTimer = new();
        private readonly Random _rand = new();

        private double _targetX;
        private double _targetY;
        private PetState _state = PetState.Idle;
        private int _ticksInState = 0;
        private int _bubbleTicks = 0;
        private bool _isDragging = false;
        private double _facing = 1.0;
        private double _spinAngle = 0.0;
        private double _customScale = 1.0;

        private int _clickCount = 0;
        private bool _isCool = false;

        public MainWindow() : this(1.0, null, null) { }

        public MainWindow(double initialScale, double? startX, double? startY)
        {
            InitializeComponent();
            _customScale = initialScale;
            _renderer = new PixelRenderer(PixelGrid);

            _activePets.Add(this);
            Closed += (s, e) => _activePets.Remove(this);

            Loaded += MainWindow_Loaded;
            MouseLeftButtonDown += MainWindow_MouseLeftButtonDown;

            _timer.Interval = TimeSpan.FromMilliseconds(40); // 25 FPS
            _timer.Tick += Timer_Tick;

            _clickTimer.Interval = TimeSpan.FromMilliseconds(350);
            _clickTimer.Tick += (s, e) =>
            {
                _clickTimer.Stop();
                HandleClicks(_clickCount);
                _clickCount = 0;
            };

            if (startX.HasValue && startY.HasValue)
            {
                Left = startX.Value;
                Top = startY.Value;
                _targetX = Left;
                _targetY = Top;
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            IntPtr hwnd = new WindowInteropHelper(this).Handle;
            Win32Helper.MakeToolWindowAndTopmost(hwnd);

            if (Left <= 0 && Top <= 0)
            {
                Rect workArea = SystemParameters.WorkArea;
                Left = Math.Max(50, workArea.Right - Width - 100);
                Top = Math.Max(50, workArea.Bottom - Height - 50);
                _targetX = Left;
                _targetY = Top;
            }

            ApplyScale(_customScale);
            _renderer.Render(SpriteFrames.Normal, false);
            _timer.Start();

            if (_customScale < 0.9)
            {
                Say("✨ Я мини-клон! 👥", 70);
            }
            else
            {
                Say("✨ Привет! Я Gemini! ✨", 90);
            }
        }

        private void ApplyScale(double s)
        {
            _customScale = s;
            PetScale.ScaleX = _customScale;
            PetScale.ScaleY = _customScale;
        }

        private void SetAuraColors(Color c1, Color c2)
        {
            GlowColor1.Color = c1;
            GlowColor2.Color = c2;
        }

        private void Say(string text, int ticks = 80)
        {
            SpeechText.Text = text;
            SpeechBorder.Visibility = Visibility.Visible;
            _bubbleTicks = ticks;
        }

        private void MainWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _clickCount++;
            _clickTimer.Stop();
            _clickTimer.Start();

            if (e.ClickCount >= 1 && _clickCount <= 1)
            {
                _isDragging = true;
                DragMove();
                _targetX = Left;
                _targetY = Top;
                _isDragging = false;
            }
        }

        private void HandleClicks(int count)
        {
            if (count == 1)
            {
                if (_state == PetState.Sleep)
                {
                    _state = PetState.Idle;
                    Say("Я проснулся! ☀️", 60);
                }
            }
            else if (count == 2)
            {
                _state = PetState.Bounce;
                _ticksInState = 0;
                Say(SpeechLibrary.GetRandom(SpeechLibrary.IdlePhrases), 90);
            }
            else if (count >= 3)
            {
                DoMitosis();
            }
        }

        private void DoMitosis()
        {
            double newScale = Math.Max(0.55, _customScale * 0.72);
            ApplyScale(newScale);

            Say("💥 МИТОЗ! Нас теперь двое! 👥", 85);
            _renderer.Render(SpriteFrames.Happy, IsFacingLeft);

            var clone = new MainWindow(newScale, Left + 65, Top);
            clone.Show();
        }

        private void MergeAllIntoOne()
        {
            var others = new List<MainWindow>(_activePets);
            others.Remove(this);

            if (others.Count == 0)
            {
                ApplyScale(1.0);
                Say("✨ Я и так один единственный!", 75);
                _renderer.Render(SpriteFrames.Happy, IsFacingLeft);
                return;
            }

            foreach (var pet in others)
            {
                try { pet.Close(); } catch { }
            }

            ApplyScale(1.0);
            _state = PetState.Bounce;
            _ticksInState = 0;
            SetAuraColors(Color.FromArgb(0x90, 0x38, 0xBD, 0xF8), Color.FromArgb(0x50, 0x81, 0x8C, 0xF8));
            Say("🧲 ВЖУХ! Собрались обратно в одного! ✨", 95);
            _renderer.Render(SpriteFrames.Happy, IsFacingLeft);
        }

        private bool IsFacingLeft => _facing < 0;

        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (_isDragging) return;

            _ticksInState++;

            if (_bubbleTicks > 0)
            {
                _bubbleTicks--;
                if (_bubbleTicks <= 0)
                {
                    SpeechBorder.Visibility = Visibility.Collapsed;
                }
            }

            Rect workArea = SystemParameters.WorkArea;
            double minX = workArea.Left + 20;
            double maxX = workArea.Right - Width - 20;
            double minY = workArea.Top + 20;
            double maxY = workArea.Bottom - Height - 10;

            string[] baseIdleFrame = _isCool ? SpriteFrames.Cool : SpriteFrames.Normal;
            string[] baseWalk1 = _isCool ? SpriteFrames.Cool : SpriteFrames.Walk1;
            string[] baseWalk2 = _isCool ? SpriteFrames.Cool : SpriteFrames.Walk2;

            switch (_state)
            {
                case PetState.Idle:
                    double floatOffset = Math.Sin(_ticksInState * 0.15) * 1.6;
                    Top = Math.Max(minY, Math.Min(maxY, Top + floatOffset));

                    if (_ticksInState % 45 >= 41 && !_isCool)
                    {
                        _renderer.Render(SpriteFrames.Happy, IsFacingLeft);
                    }
                    else
                    {
                        _renderer.Render(baseIdleFrame, IsFacingLeft);
                    }

                    if (_ticksInState > 55)
                    {
                        int roll = _rand.Next(100);
                        if (roll < 45)
                        {
                            _state = PetState.Walk;
                            _ticksInState = 0;
                            _targetX = _rand.Next((int)minX, (int)maxX);
                            if (_rand.Next(100) < 65)
                                _targetY = _rand.Next((int)Math.Max(minY, maxY - 160), (int)maxY);
                            else
                                _targetY = _rand.Next((int)minY, (int)maxY);

                            _facing = (_targetX >= Left) ? 1.0 : -1.0;
                        }
                        else if (roll < 55)
                        {
                            if (Win32Helper.GetCursorPos(out Win32Helper.POINT pt))
                            {
                                _state = PetState.FollowMouse;
                                _ticksInState = 0;
                                Say("👀 Иду за курсором!", 60);
                            }
                        }
                        else if (roll < 65)
                        {
                            Say(SpeechLibrary.GetRandom(SpeechLibrary.IdlePhrases), 80);
                            _ticksInState = 0;
                        }
                        else
                        {
                            _ticksInState = 15;
                        }
                    }
                    break;

                case PetState.Walk:
                    double dx = _targetX - Left;
                    double dy = _targetY - Top;
                    double dist = Math.Sqrt(dx * dx + dy * dy);

                    if (dist < 6.0 || _ticksInState > 250)
                    {
                        _state = PetState.Idle;
                        _ticksInState = 0;
                        _renderer.Render(baseIdleFrame, IsFacingLeft);
                    }
                    else
                    {
                        double speed = 3.6;
                        Left += (dx / dist) * Math.Min(speed, dist);
                        Top += (dy / dist) * Math.Min(speed, dist);

                        int stepPhase = (_ticksInState % 12);
                        if (stepPhase < 6)
                            _renderer.Render(baseWalk1, IsFacingLeft);
                        else
                            _renderer.Render(baseWalk2, IsFacingLeft);
                    }
                    break;

                case PetState.FollowMouse:
                    if (Win32Helper.GetCursorPos(out Win32Helper.POINT curPt))
                    {
                        _targetX = curPt.X - (Width / 2);
                        _targetY = curPt.Y - Height + 20;

                        double fDx = _targetX - Left;
                        double fDy = _targetY - Top;
                        double fDist = Math.Sqrt(fDx * fDx + fDy * fDy);

                        if (fDist > 80.0)
                        {
                            _facing = (fDx >= 0) ? 1.0 : -1.0;
                            double speed = 4.2;
                            Left += (fDx / fDist) * Math.Min(speed, fDist);
                            Top += (fDy / fDist) * Math.Min(speed, fDist);

                            if ((_ticksInState % 8) < 4)
                                _renderer.Render(baseWalk1, IsFacingLeft);
                            else
                                _renderer.Render(baseWalk2, IsFacingLeft);
                        }
                        else
                        {
                            _renderer.Render(SpriteFrames.Happy, IsFacingLeft);
                        }

                        if (_ticksInState > 160)
                        {
                            _state = PetState.Idle;
                            _ticksInState = 0;
                            _renderer.Render(baseIdleFrame, IsFacingLeft);
                        }
                    }
                    else
                    {
                        _state = PetState.Idle;
                    }
                    break;

                case PetState.Bounce:
                    double bounceOffset = Math.Sin(_ticksInState * 0.45) * 7.0;
                    Top = _targetY - Math.Abs(bounceOffset);
                    _renderer.Render(SpriteFrames.Happy, IsFacingLeft);
                    if (_ticksInState > 22)
                    {
                        _state = PetState.Idle;
                        _ticksInState = 0;
                        Top = _targetY;
                        _renderer.Render(baseIdleFrame, IsFacingLeft);
                    }
                    break;

                case PetState.Spin:
                    _spinAngle = (_spinAngle + 30) % 360;
                    PetRotate.Angle = _spinAngle;
                    if (_ticksInState > 24)
                    {
                        _state = PetState.Idle;
                        _ticksInState = 0;
                        _spinAngle = 0;
                        PetRotate.Angle = 0;
                        _renderer.Render(SpriteFrames.Happy, IsFacingLeft);
                    }
                    break;

                case PetState.Sleep:
                    _renderer.Render(SpriteFrames.Sleep, IsFacingLeft);
                    if (_ticksInState % 60 == 0)
                    {
                        Say("Zzz... 💤", 45);
                    }
                    break;

                case PetState.Party:
                    double danceX = Math.Sin(_ticksInState * 0.6) * 4.0;
                    double danceY = Math.Cos(_ticksInState * 0.6) * 4.0;
                    Left += danceX;
                    Top += danceY;
                    _renderer.Render(SpriteFrames.SuperStar, IsFacingLeft);
                    if (_ticksInState > 80)
                    {
                        _state = PetState.Idle;
                        _ticksInState = 0;
                        SetAuraColors(Color.FromArgb(0x90, 0x38, 0xBD, 0xF8), Color.FromArgb(0x50, 0x81, 0x8C, 0xF8));
                        _renderer.Render(baseIdleFrame, IsFacingLeft);
                    }
                    break;

                case PetState.Love:
                    _renderer.Render(SpriteFrames.Love, IsFacingLeft);
                    if (_ticksInState > 65)
                    {
                        _state = PetState.Idle;
                        _ticksInState = 0;
                        SetAuraColors(Color.FromArgb(0x90, 0x38, 0xBD, 0xF8), Color.FromArgb(0x50, 0x81, 0x8C, 0xF8));
                        _renderer.Render(baseIdleFrame, IsFacingLeft);
                    }
                    break;
            }
        }

        #region Context Menu Handlers
        private void MenuSayHi_Click(object sender, RoutedEventArgs e)
        {
            Say("✨ Рад тебя видеть! Хорошего дня!", 80);
            _renderer.Render(SpriteFrames.Happy, IsFacingLeft);
        }

        private void MenuLove_Click(object sender, RoutedEventArgs e)
        {
            _state = PetState.Love;
            _ticksInState = 0;
            SetAuraColors(Color.FromArgb(0x90, 0xF4, 0x72, 0xB6), Color.FromArgb(0x50, 0xF8, 0x71, 0x71));
            Say(SpeechLibrary.GetRandom(SpeechLibrary.LovePhrases), 80);
        }

        private void MenuParty_Click(object sender, RoutedEventArgs e)
        {
            _state = PetState.Party;
            _ticksInState = 0;
            SetAuraColors(Color.FromArgb(0xA0, 0xFA, 0xCC, 0x15), Color.FromArgb(0x60, 0xFB, 0x92, 0x3C));
            Say(SpeechLibrary.GetRandom(SpeechLibrary.PartyPhrases), 80);
        }

        private void MenuSpin_Click(object sender, RoutedEventArgs e)
        {
            _state = PetState.Spin;
            _ticksInState = 0;
            _spinAngle = 0;
            Say("Виууу! 💫", 60);
        }

        private void MenuFeed_Click(object sender, RoutedEventArgs e)
        {
            Say("Ням! Вкусная звёздочка! ⭐😋", 80);
            _state = PetState.Bounce;
            _ticksInState = 0;
        }

        private void MenuClone_Click(object sender, RoutedEventArgs e)
        {
            DoMitosis();
        }

        private void MenuMerge_Click(object sender, RoutedEventArgs e)
        {
            MergeAllIntoOne();
        }

        private void MenuGlasses_Click(object sender, RoutedEventArgs e)
        {
            _isCool = !_isCool;
            if (_isCool)
            {
                Say("🕶️ Deal with it!", 70);
                _renderer.Render(SpriteFrames.Cool, IsFacingLeft);
            }
            else
            {
                Say("Очки снял ✨", 50);
                _renderer.Render(SpriteFrames.Normal, IsFacingLeft);
            }
        }

        private void MenuSleep_Click(object sender, RoutedEventArgs e)
        {
            _state = PetState.Sleep;
            _ticksInState = 0;
            Say("Спокойной ночи... 💤", 60);
        }

        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        #endregion
    }
}
