using System.Windows.Controls;
using Rectangle = System.Windows.Shapes.Rectangle;
using System.Windows;

namespace GeminiScreenPet.Services
{
    public class PixelRenderer
    {
        private const int GridSize = 32;
        private readonly Rectangle[] _pixelRects = new Rectangle[GridSize * GridSize];
        private readonly Canvas _canvas;
        private const double PixelSize = 2.6;

        public PixelRenderer(Canvas canvas)
        {
            _canvas = canvas;
            InitializeGrid();
        }

        private void InitializeGrid()
        {
            _canvas.Children.Clear();
            for (int r = 0; r < GridSize; r++)
            {
                for (int c = 0; c < GridSize; c++)
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
                    _pixelRects[r * GridSize + c] = rect;
                }
            }
        }

        public void Render(string[] frame, bool flipX = false)
        {
            var palette = Animations.SpriteFrames.ActivePalette;

            for (int r = 0; r < GridSize; r++)
            {
                string row = frame[r];
                for (int c = 0; c < GridSize; c++)
                {
                    int col = flipX ? (GridSize - 1 - c) : c;
                    char ch = row[col];
                    var rect = _pixelRects[r * GridSize + c];

                    if (ch != '.' && palette.TryGetValue(ch, out var brush))
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
