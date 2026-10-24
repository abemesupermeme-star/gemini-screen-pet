using System.Collections.Generic;
using System.Windows.Media;

namespace GeminiScreenPet.Animations
{
    public static class SpriteFrames
    {
        public static readonly Dictionary<char, Brush> Palette = new();

        static SpriteFrames()
        {
            RegisterColor('B', Color.FromRgb(37, 99, 235));   // Deep Blue
            RegisterColor('L', Color.FromRgb(96, 165, 250));  // Light Blue
            RegisterColor('C', Color.FromRgb(165, 243, 252)); // Cyan Sparkle
            RegisterColor('P', Color.FromRgb(139, 92, 246));  // Purple
            RegisterColor('M', Color.FromRgb(217, 70, 239));  // Magenta
            RegisterColor('W', Color.FromRgb(255, 255, 255));  // White
            RegisterColor('K', Color.FromRgb(15, 23, 42));    // Dark Eyes / Outline
            RegisterColor('Y', Color.FromRgb(244, 114, 182));  // Pink Blush
            RegisterColor('G', Color.FromRgb(250, 204, 21));  // Golden Star
            RegisterColor('R', Color.FromRgb(248, 113, 113));  // Heart Red
        }

        private static void RegisterColor(char key, Color color)
        {
            var brush = new SolidColorBrush(color);
            brush.Freeze();
            Palette[key] = brush;
        }

        private static string[] Scale2x(string[] src16)
        {
            var res = new string[32];
            for (int r = 0; r < 16; r++)
            {
                var sb = new System.Text.StringBuilder(32);
                for (int c = 0; c < 16; c++)
                {
                    sb.Append(src16[r][c]);
                    sb.Append(src16[r][c]);
                }
                string row = sb.ToString();
                res[r * 2] = row;
                res[r * 2 + 1] = row;
            }
            return res;
        }

        // 16x16 Raw Templates
        private static readonly string[] _rawNormal = new string[16]
        {
            ".......WW.......",
            "......WCCW......",
            ".....WCCCCW.....",
            ".....WLLLLW.....",
            "....WLLBBLLW....",
            "...WLLBWWBLLW...",
            "..WLLBKKWWKLLW..",
            ".WLLBBKKWWKBLLW.",
            ".WLLBBKKWWKBLLW.",
            "..WLLBWWYYWLLW..",
            "...WLLBWWBLLW...",
            "....WLLBBLLW....",
            ".....WPPPPW.....",
            ".....WMMMMW.....",
            "......WMMW......",
            ".......WW......."
        };

        private static readonly string[] _rawWalk1 = new string[16]
        {
            "................",
            ".......WW.......",
            "......WCCW......",
            ".....WCCCCW.....",
            "....WLLLLLLW....",
            "...WLLBBBBLLW...",
            "..WLLBKKWWKBLLW.",
            ".WLLBBKKWWKBLLW.",
            "..WLLBKKWWKBLLW.",
            "...WLLWWYYWLLW..",
            "....WLLBBLLW....",
            ".....WPPPPW.....",
            "....WLLMMLLW....",
            ".....WMMMMW.....",
            "......WMMW......",
            ".......WW......."
        };

        private static readonly string[] _rawWalk2 = new string[16]
        {
            ".......WW.......",
            "......WCCW......",
            ".....WCCCCW.....",
            ".....WLLLLW.....",
            "....WLLBBLLW....",
            "...WLLBWWBLLW...",
            "..WLLBKKWWKBLLW.",
            ".WLLBBKKWWKBLLW.",
            ".WLLBBKKWWKBLLW.",
            "..WLLBWYYYYBLLW.",
            "...WLLBWWBLLW...",
            "....WLLBBLLW....",
            "....WPPLLPPW....",
            ".....WMMMMW.....",
            "......WMMW......",
            ".......WW......."
        };

        private static readonly string[] _rawHappy = new string[16]
        {
            ".......WW.......",
            "......WCCW......",
            ".....WCCCCW.....",
            ".....WLLLLW.....",
            "....WLLBBLLW....",
            "...WLLBWWBLLW...",
            "..WLLBKKWWKBLLW.",
            ".WLLBWKKWWKKBLLW",
            ".WLLBWKKWWKKBLLW",
            "..WLLBYYYYYBLLW.",
            "...WLLBWWBLLW...",
            "....WLLBBLLW....",
            ".....WPPPPW.....",
            ".....WMMMMW.....",
            "......WMMW......",
            ".......WW......."
        };

        private static readonly string[] _rawLove = new string[16]
        {
            ".......WW.......",
            "......WCCW......",
            ".....WCCCCW.....",
            ".....WLLLLW.....",
            "....WLLBBLLW....",
            "...WLLBWWBLLW...",
            "..WLLBRRWRRBLLW.",
            ".WLLBRRRWRRRBLLW",
            ".WLLBRRRWRRRBLLW",
            "..WLLBWRRRRWLLW.",
            "...WLLBWWBLLW...",
            "....WLLBBLLW....",
            ".....WPPPPW.....",
            ".....WMMMMW.....",
            "......WMMW......",
            ".......WW......."
        };

        private static readonly string[] _rawCool = new string[16]
        {
            ".......WW.......",
            "......WCCW......",
            ".....WCCCCW.....",
            ".....WLLLLW.....",
            "....WKKKKKKW....",
            "...WKKWKKWKKW...",
            "..WLLKKKKKKLLW..",
            ".WLLBBKKKKKBLLW.",
            ".WLLBBKKKKKBLLW.",
            "..WLLBWWYYWLLW..",
            "...WLLBWWBLLW...",
            "....WLLBBLLW....",
            ".....WPPPPW.....",
            ".....WMMMMW.....",
            "......WMMW......",
            ".......WW......."
        };

        private static readonly string[] _rawSleep = new string[16]
        {
            ".......WW.......",
            "......WCCW......",
            ".....WCCCCW.....",
            ".....WLLLLW.....",
            "....WLLBBLLW....",
            "...WLLBWWBLLW...",
            "..WLLBBBBBBLLW..",
            ".WLLBBKKKKKBLLW.",
            ".WLLBBBBBBBBLLW.",
            "..WLLBWWYYWLLW..",
            "...WLLBWWBLLW...",
            "....WLLBBLLW....",
            ".....WPPPPW.....",
            ".....WMMMMW.....",
            "......WMMW......",
            ".......WW......."
        };

        private static readonly string[] _rawSuperStar = new string[16]
        {
            ".......WW.......",
            "......WGGW......",
            ".....WGGGGW.....",
            ".....WGGGGW.....",
            "....WGGLLGGW....",
            "...WGGWWGGWWG...",
            "..WGGKKWWKKGGW..",
            ".WGGKKKKWKKKKGGW",
            ".WGGKKKKWKKKKGGW",
            "..WGGWYYYYWGGW..",
            "...WGGWGGWGGW...",
            "....WGGLLGGW....",
            ".....WGGGGW.....",
            ".....WGGGGW.....",
            "......WGGW......",
            ".......WW......."
        };

        // 32x32 Hi-Res Pixel Sprites (1024 Pixels!)
        public static readonly string[] Normal = Scale2x(_rawNormal);
        public static readonly string[] Walk1 = Scale2x(_rawWalk1);
        public static readonly string[] Walk2 = Scale2x(_rawWalk2);
        public static readonly string[] Happy = Scale2x(_rawHappy);
        public static readonly string[] Love = Scale2x(_rawLove);
        public static readonly string[] Cool = Scale2x(_rawCool);
        public static readonly string[] Sleep = Scale2x(_rawSleep);
        public static readonly string[] SuperStar = Scale2x(_rawSuperStar);
    }
}
