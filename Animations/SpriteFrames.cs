using System;
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
            RegisterColor('K', Color.FromRgb(15, 23, 42));    // Black / Dark Slate
            RegisterColor('Y', Color.FromRgb(244, 114, 182));  // Pink Blush
            RegisterColor('G', Color.FromRgb(250, 204, 21));  // Golden Yellow
            RegisterColor('R', Color.FromRgb(248, 113, 113));  // Heart / Love Red
        }

        private static void RegisterColor(char key, Color color)
        {
            var brush = new SolidColorBrush(color);
            brush.Freeze();
            Palette[key] = brush;
        }

        // 1. Idle (Floating Star)
        public static readonly string[] Normal = new string[16]
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

        // 2. Walking Step 1
        public static readonly string[] Walk1 = new string[16]
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

        // 3. Walking Step 2 (Foot forward)
        public static readonly string[] Walk2 = new string[16]
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

        // 4. Happy / Joy (^ _ ^)
        public static readonly string[] Happy = new string[16]
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

        // 5. Love / Hearts in Eyes (♥ ♥)
        public static readonly string[] Love = new string[16]
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

        // 6. Cyber Sunglasses (🕶️ Deal With It)
        public static readonly string[] Cool = new string[16]
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

        // 7. Sleeping (Zzz...)
        public static readonly string[] Sleep = new string[16]
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

        // 8. Shocked / Derp (O _ O)
        public static readonly string[] Shocked = new string[16]
        {
            ".......WW.......",
            "......WCCW......",
            ".....WCCCCW.....",
            ".....WLLLLW.....",
            "....WLLBBLLW....",
            "...WLLKKKKLLW...",
            "..WLLKWKKWKLLW..",
            ".WLLBBKWKKWKBLLW",
            ".WLLBBKWKKWKBLLW",
            "..WLLBWWYYWLLW..",
            "...WLLKKKKLLW...",
            "....WLLBBLLW....",
            ".....WPPPPW.....",
            ".....WMMMMW.....",
            "......WMMW......",
            ".......WW......."
        };

        // 9. Party / Golden Super Star
        public static readonly string[] SuperStar = new string[16]
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
    }
}
