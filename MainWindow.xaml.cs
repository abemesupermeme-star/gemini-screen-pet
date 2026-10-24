using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Threading;
using GeminiScreenPet.Animations;
using GeminiScreenPet.Models;
using GeminiScreenPet.Services;
using Forms = System.Windows.Forms;
using Color = System.Windows.Media.Color;

namespace GeminiScreenPet
{
    public partial class MainWindow : Window
    {
        private static readonly List<MainWindow> _activePets = new();
        private static Forms.NotifyIcon? _trayIcon;

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
        private bool _alwaysFollowMouse = false;

        // Hunger & Speed mechanics
        private double _hunger = 100.0; // 0% to 100%
        private double _moveSpeed = 4.0;

        // Trail Ribbon / Particle System
        public enum TrailType { None, Cyan, Rainbow, Fire, Pink, Sparkle }
        private TrailType _currentTrail = TrailType.Cyan;

        public MainWindow() : this(1.0, null, null) { }

        public MainWindow(double initialScale, double? startX, double? startY)
        {
            InitializeComponent();
            _customScale = initialScale;
            _renderer = new PixelRenderer(PixelGrid);

            _activePets.Add(this);
            Closed += (s, e) =>
            {
                _timer.Stop();
                _clickTimer.Stop();
                _activePets.Remove(this);
            };

            Loaded += MainWindow_Loaded;
            MouseLeftButtonDown += MainWindow_MouseLeftButtonDown;

            _timer.Interval = TimeSpan.FromMilliseconds(30); // ~33 FPS smooth motion
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
            UpdateAutoStartMenuState();
            InitTrayIcon();
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

        private void InitTrayIcon()
        {
            if (_trayIcon != null) return;

            _trayIcon = new Forms.NotifyIcon
            {
                Icon = SystemIcons.Application,
                Text = "Gemini Screen Pet",
                Visible = true
            };

            var trayMenu = new Forms.ContextMenuStrip();
            trayMenu.Items.Add("✨ Показать Gemini", null, (s, e) => ShowFromTray());
            trayMenu.Items.Add(new Forms.ToolStripSeparator());
            trayMenu.Items.Add("❌ Закрыть приложение", null, (s, e) =>
            {
                _trayIcon.Visible = false;
                _trayIcon.Dispose();
                _trayIcon = null;
                System.Windows.Application.Current.Shutdown();
            });

            _trayIcon.ContextMenuStrip = trayMenu;
            _trayIcon.DoubleClick += (s, e) => ShowFromTray();
        }

        private void ShowFromTray()
        {
            foreach (var pet in _activePets)
            {
                pet.Show();
                pet.WindowState = WindowState.Normal;
            }
            Say("✨ Я вернулся из трея!", 70);
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
                try 
                { 
                    pet._timer.Stop();
                    pet.Close(); 
                } 
                catch { }
            }

            TrailOverlay.Instance?.Clear();

            ApplyScale(1.0);
            _state = PetState.Bounce;
            _ticksInState = 0;
            SetAuraColors(Color.FromArgb(0x90, 0x38, 0xBD, 0xF8), Color.FromArgb(0x50, 0x81, 0x8C, 0xF8));
            Say("🧲 ВЖУХ! Собрались обратно в одного! ✨", 95);
            _renderer.Render(SpriteFrames.Happy, IsFacingLeft);
        }

        private bool IsFacingLeft => _facing < 0;

        private void EmitGlobalTrail()
        {
            if (TrailOverlay.Instance != null && _currentTrail != TrailType.None)
            {
                double emitScreenX = Left + (Width / 2);
                double emitScreenY = Top + Height - 50;
                TrailOverlay.Instance.Emit(emitScreenX, emitScreenY, _currentTrail);
            }
        }

        private DateTime _lastHungerDrainTime = DateTime.UtcNow;

        private void UpdateHunger()
        {
            // Drain 1% per 30 minutes (1800 seconds)
            var now = DateTime.UtcNow;
            double elapsedSeconds = (now - _lastHungerDrainTime).TotalSeconds;
            if (elapsedSeconds >= 1.0)
            {
                double drain = elapsedSeconds / 1800.0; // 1.0 full percent per 1800s
                _hunger = Math.Max(0.0, _hunger - drain);
                _lastHungerDrainTime = now;

                HungerProgressBar.Value = _hunger;
                HungerPercentText.Text = $"{(int)Math.Round(_hunger)}%";
            }

            if (_hunger < 20.0 && _ticksInState % 180 == 0 && _state != PetState.Sleep)
            {
                Say("🍗 Я проголодался... Покорми звёздочкой!", 85);
            }
        }

        private void SpeedSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _moveSpeed = e.NewValue;
            if (SpeedValueText != null)
            {
                SpeedValueText.Text = $"{_moveSpeed:F1}x";
            }
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (_isDragging) return;

            _ticksInState++;
            UpdateHunger();

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

            if (_alwaysFollowMouse && _state != PetState.Spin && _state != PetState.Bounce)
            {
                _state = PetState.FollowMouse;
            }

            switch (_state)
            {
                case PetState.Idle:
                    double floatOffset = Math.Sin(_ticksInState * 0.12) * 2.2;
                    Top = Math.Max(minY, Math.Min(maxY, Top + floatOffset));

                    double breathScaleY = 1.0 + Math.Sin(_ticksInState * 0.12) * 0.04;
                    double breathScaleX = 1.0 - Math.Sin(_ticksInState * 0.12) * 0.02;
                    PetScale.ScaleX = _customScale * breathScaleX;
                    PetScale.ScaleY = _customScale * breathScaleY;

                    if (_ticksInState % 50 >= 46 && !_isCool)
                    {
                        _renderer.Render(SpriteFrames.Happy, IsFacingLeft);
                    }
                    else
                    {
                        _renderer.Render(baseIdleFrame, IsFacingLeft);
                    }

                    if (_ticksInState > 65)
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
                            _ticksInState = 20;
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
                        ApplyScale(_customScale);
                        _renderer.Render(baseIdleFrame, IsFacingLeft);
                    }
                    else
                    {
                        double effectiveSpeed = (_hunger < 15.0) ? (_moveSpeed * 0.6) : _moveSpeed;
                        Left += (dx / dist) * Math.Min(effectiveSpeed, dist);
                        Top += (dy / dist) * Math.Min(effectiveSpeed, dist);

                        double walkSquash = 1.0 + Math.Sin(_ticksInState * 0.4) * 0.06;
                        PetScale.ScaleY = _customScale * walkSquash;
                        PetScale.ScaleX = _customScale * (2.0 - walkSquash);

                        EmitGlobalTrail();

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
                        _targetY = curPt.Y - Height + 30;

                        double fDx = _targetX - Left;
                        double fDy = _targetY - Top;
                        double fDist = Math.Sqrt(fDx * fDx + fDy * fDy);

                        if (fDist > 65.0)
                        {
                            _facing = (fDx >= 0) ? 1.0 : -1.0;
                            double followSpeed = _moveSpeed * 1.25;
                            Left += (fDx / fDist) * Math.Min(followSpeed, fDist);
                            Top += (fDy / fDist) * Math.Min(followSpeed, fDist);

                            double followSquash = 1.0 + Math.Sin(_ticksInState * 0.45) * 0.08;
                            PetScale.ScaleY = _customScale * followSquash;

                            EmitGlobalTrail();

                            if ((_ticksInState % 8) < 4)
                                _renderer.Render(baseWalk1, IsFacingLeft);
                            else
                                _renderer.Render(baseWalk2, IsFacingLeft);
                        }
                        else
                        {
                            ApplyScale(_customScale);
                            _renderer.Render(SpriteFrames.Happy, IsFacingLeft);
                        }

                        if (!_alwaysFollowMouse && _ticksInState > 180)
                        {
                            _state = PetState.Idle;
                            _ticksInState = 0;
                            ApplyScale(_customScale);
                            _renderer.Render(baseIdleFrame, IsFacingLeft);
                        }
                    }
                    else
                    {
                        _state = PetState.Idle;
                    }
                    break;

                case PetState.Bounce:
                    double bounceOffset = Math.Sin(_ticksInState * 0.35) * 8.0;
                    Top = _targetY - Math.Abs(bounceOffset);
                    PetScale.ScaleY = _customScale * (1.0 + Math.Abs(bounceOffset) * 0.02);
                    _renderer.Render(SpriteFrames.Happy, IsFacingLeft);
                    if (_ticksInState > 24)
                    {
                        _state = PetState.Idle;
                        _ticksInState = 0;
                        Top = _targetY;
                        ApplyScale(_customScale);
                        _renderer.Render(baseIdleFrame, IsFacingLeft);
                    }
                    break;

                case PetState.Spin:
                    _spinAngle = (_spinAngle + 30) % 360;
                    PetRotate.Angle = _spinAngle;
                    EmitGlobalTrail();
                    if (_ticksInState > 24)
                    {
                        _state = PetState.Idle;
                        _ticksInState = 0;
                        _spinAngle = 0;
                        PetRotate.Angle = 0;
                        ApplyScale(_customScale);
                        _renderer.Render(SpriteFrames.Happy, IsFacingLeft);
                    }
                    break;

                case PetState.Sleep:
                    _renderer.Render(SpriteFrames.Sleep, IsFacingLeft);
                    double sleepBreathe = 1.0 + Math.Sin(_ticksInState * 0.06) * 0.03;
                    PetScale.ScaleY = _customScale * sleepBreathe;
                    if (_ticksInState % 60 == 0)
                    {
                        Say("Zzz... 💤", 45);
                    }
                    break;

                case PetState.Party:
                    double danceX = Math.Sin(_ticksInState * 0.6) * 4.5;
                    double danceY = Math.Cos(_ticksInState * 0.6) * 4.5;
                    Left += danceX;
                    Top += danceY;
                    EmitGlobalTrail();
                    _renderer.Render(SpriteFrames.SuperStar, IsFacingLeft);
                    if (_ticksInState > 80)
                    {
                        _state = PetState.Idle;
                        _ticksInState = 0;
                        ApplyScale(_customScale);
                        SetAuraColors(Color.FromArgb(0x90, 0x38, 0xBD, 0xF8), Color.FromArgb(0x50, 0x81, 0x8C, 0xF8));
                        _renderer.Render(baseIdleFrame, IsFacingLeft);
                    }
                    break;

                case PetState.Love:
                    EmitGlobalTrail();
                    _renderer.Render(SpriteFrames.Love, IsFacingLeft);
                    if (_ticksInState > 65)
                    {
                        _state = PetState.Idle;
                        _ticksInState = 0;
                        ApplyScale(_customScale);
                        SetAuraColors(Color.FromArgb(0x90, 0x38, 0xBD, 0xF8), Color.FromArgb(0x50, 0x81, 0x8C, 0xF8));
                        _renderer.Render(baseIdleFrame, IsFacingLeft);
                    }
                    break;
            }
        }

        #region Context Menu Handlers
        private void MenuHideToTray_Click(object sender, RoutedEventArgs e)
        {
            foreach (var pet in _activePets)
            {
                pet.Hide();
            }
            if (_trayIcon != null)
            {
                _trayIcon.ShowBalloonTip(3000, "Gemini спрятался!", "Кликни дважды по иконке в трее, чтобы вернуть меня обратно.", Forms.ToolTipIcon.Info);
            }
        }

        private void UpdateAutoStartMenuState()
        {
            bool isAuto = AutoStartHelper.IsAutoStartEnabled();
            MenuAutoStartItem.Header = isAuto ? "✅ Автозапуск с Windows (ВКЛ)" : "🚀 Запуск вместе с Windows";
        }

        private void MenuAutoStart_Click(object sender, RoutedEventArgs e)
        {
            bool current = AutoStartHelper.IsAutoStartEnabled();
            AutoStartHelper.SetAutoStart(!current);
            UpdateAutoStartMenuState();

            if (!current)
            {
                Say("🚀 Добавлен в автозагрузку!", 75);
            }
            else
            {
                Say("Удалён из автозагрузки ✨", 65);
            }
        }

        private void MenuFollowToggle_Click(object sender, RoutedEventArgs e)
        {
            _alwaysFollowMouse = !_alwaysFollowMouse;
            if (_alwaysFollowMouse)
            {
                MenuFollowItem.Header = "✅ Режим: Следовать за курсором (ВКЛ)";
                Say("🎯 Я следую за твоим курсором!", 75);
                _state = PetState.FollowMouse;
            }
            else
            {
                MenuFollowItem.Header = "🎯 Режим: Следовать за курсором (ВЫКЛ)";
                Say("Погуляю сам! ✨", 60);
                _state = PetState.Idle;
            }
        }

        private void MenuThemeOfficial_Click(object sender, RoutedEventArgs e)
        {
            SpriteFrames.CurrentTheme = SpriteFrames.Theme.OfficialGemini;
            SetAuraColors(Color.FromArgb(0x90, 0x38, 0xBD, 0xF8), Color.FromArgb(0x50, 0x81, 0x8C, 0xF8));
            _renderer.Render(SpriteFrames.Normal, IsFacingLeft);
            Say("✨ Официальный стиль Gemini!", 60);
        }

        private void MenuThemeMono_Click(object sender, RoutedEventArgs e)
        {
            SpriteFrames.CurrentTheme = SpriteFrames.Theme.Monochrome;
            SetAuraColors(Color.FromArgb(0x80, 0x94, 0xA3, 0xB8), Color.FromArgb(0x40, 0x33, 0x41, 0x55));
            _renderer.Render(SpriteFrames.Normal, IsFacingLeft);
            Say("⚫⚪ Чёрно-белый минимализм!", 60);
        }

        private void MenuTrailNone_Click(object sender, RoutedEventArgs e) => _currentTrail = TrailType.None;
        private void MenuTrailCyan_Click(object sender, RoutedEventArgs e) => _currentTrail = TrailType.Cyan;
        private void MenuTrailRainbow_Click(object sender, RoutedEventArgs e) => _currentTrail = TrailType.Rainbow;
        private void MenuTrailFire_Click(object sender, RoutedEventArgs e) => _currentTrail = TrailType.Fire;
        private void MenuTrailPink_Click(object sender, RoutedEventArgs e) => _currentTrail = TrailType.Pink;
        private void MenuTrailSparkle_Click(object sender, RoutedEventArgs e) => _currentTrail = TrailType.Sparkle;

        private void MenuFeed_Click(object sender, RoutedEventArgs e)
        {
            _hunger = Math.Min(100.0, _hunger + 25.0);
            HungerProgressBar.Value = _hunger;
            HungerPercentText.Text = $"{(int)Math.Round(_hunger)}%";

            Say("Ням! Вкусная звёздочка! +25% ⭐😋", 85);
            _state = PetState.Bounce;
            _ticksInState = 0;
        }

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
            if (_trayIcon != null)
            {
                _trayIcon.Visible = false;
                _trayIcon.Dispose();
                _trayIcon = null;
            }
            Close();
        }
        #endregion
    }
}
