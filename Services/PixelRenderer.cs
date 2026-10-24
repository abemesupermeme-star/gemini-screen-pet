using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows;

namespace GeminiScreenPet.Services
{
    public class PixelRenderer
    {
        private readonly Rectangle[] _pixelRects = new Rectangle[256];
        private readonly Canvas _canvas;
        private const double PixelSize = 5.0;

        public PixelRenderer(Canvas canvas)
        {
            _canvas = canvas;
            InitializeGrid();
        }

        private void InitializeGrid()
        {
            _canvas.Children.Clear();
            for (int r = 0; r < 16; r++)
            {
                for (int c = 0; c < 16; c++)
                {
                    var rect = new Rectangle
                    {
                        Width = PixelSize,
                        Height = PixelSize,
                        Visibility = Visibility.Hidden
                    };
                    Canvas.SetLeft(rect, c * PixelSize);
                    Canvas.SetTop(rect, r * PixelSize);
                    _canvas.Children.Add(rect);
                    _pixelRects[r * 16 + c] = rect;
                }
            }
        }

        public void Render(string[] frame, bool flipX = false)
        {
            for (int r = 0; r < 16; r++)
            {
                string row = frame[r];
                for (int c = 0; c < 16; c++)
                {
                    int col = flipX ? (15 - c) : c;
                    char ch = row[col];
                    var rect = _pixelRects[r * 16 + c];

                    if (ch != '.' && Animations.SpriteFrames.Palette.TryGetValue(ch, out var brush))
                    {
                        rect.Fill = brush;
                        rect.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        rect.Visibility = Visibility.Hidden;
                    }
                }
            }
        }
    }
}
