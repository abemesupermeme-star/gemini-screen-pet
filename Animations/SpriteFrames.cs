using System.Collections.Generic;
using Brush = System.Windows.Media.Brush;
using Color = System.Windows.Media.Color;
using SolidColorBrush = System.Windows.Media.SolidColorBrush;

namespace GeminiScreenPet.Animations
{
    public static class SpriteFrames
    {
        public enum Theme
        {
            OfficialGemini, // Deep Blue to Purple gradient + White core
            Monochrome,     // Crisp Black & White minimal
            GoldSparkle,    // Golden Star
            PinkLove        // Pink & Crimson Love
        }

        public static Theme CurrentTheme = Theme.OfficialGemini;

        public static readonly Dictionary<char, Brush> PaletteOfficial = new();
        public static readonly Dictionary<char, Brush> PaletteMono = new();

        static SpriteFrames()
        {
            // Official Google Gemini Colors
            RegisterColor(PaletteOfficial, 'W', Color.FromRgb(255, 255, 255)); // Pure White Core
            RegisterColor(PaletteOfficial, 'C', Color.FromRgb(192, 232, 255)); // Soft Sparkle Cyan
            RegisterColor(PaletteOfficial, 'L', Color.FromRgb(91, 158, 255));  // Vibrant Gemini Sky Blue
            RegisterColor(PaletteOfficial, 'B', Color.FromRgb(26, 115, 232));  // Google Blue
            RegisterColor(PaletteOfficial, 'P', Color.FromRgb(147, 51, 234));  // Gemini Deep Purple
            RegisterColor(PaletteOfficial, 'M', Color.FromRgb(192, 38, 211));  // Deep Magenta
            RegisterColor(PaletteOfficial, 'K', Color.FromRgb(15, 23, 42));    // Midnight Slate (Eyes)
            RegisterColor(PaletteOfficial, 'Y', Color.FromRgb(244, 114, 182));  // Blush Pink
            RegisterColor(PaletteOfficial, 'G', Color.FromRgb(250, 204, 21));  // Star Gold
            RegisterColor(PaletteOfficial, 'R', Color.FromRgb(244, 63, 94));   // Rose Red

            // Monochrome Minimalist Theme (B&W)
            RegisterColor(PaletteMono, 'W', Color.FromRgb(255, 255, 255));
            RegisterColor(PaletteMono, 'C', Color.FromRgb(241, 245, 249));
            RegisterColor(PaletteMono, 'L', Color.FromRgb(203, 213, 225));
            RegisterColor(PaletteMono, 'B', Color.FromRgb(148, 163, 184));
            RegisterColor(PaletteMono, 'P', Color.FromRgb(71, 85, 105));
            RegisterColor(PaletteMono, 'M', Color.FromRgb(30, 41, 59));
            RegisterColor(PaletteMono, 'K', Color.FromRgb(0, 0, 0));
            RegisterColor(PaletteMono, 'Y', Color.FromRgb(203, 213, 225));
            RegisterColor(PaletteMono, 'G', Color.FromRgb(226, 232, 240));
            RegisterColor(PaletteMono, 'R', Color.FromRgb(15, 23, 42));
        }

        private static void RegisterColor(Dictionary<char, Brush> dict, char key, Color color)
        {
            var brush = new SolidColorBrush(color);
            brush.Freeze();
            dict[key] = brush;
        }

        public static Dictionary<char, Brush> ActivePalette => 
            CurrentTheme == Theme.Monochrome ? PaletteMono : PaletteOfficial;

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

        // --- 16x16 BASE SHAPES WITH REFINED ANATOMY & GRADIENT ---
        // 1. Idle Normal (Official Gemini 4-Point Star)
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

        // 2. Walk Step 1 (Left Leg forward + Squash)
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

        // 3. Walk Step 2 (Right Leg forward + Stretch)
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

        // 4. Happy Blink (^ _ ^)
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

        // 5. Love Hearts in Eyes (♥ ♥)
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

        // 6. Cool Cyber Glasses 🕶️
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

        // 7. Sleep (Zzz)
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

        // 8. Super Star (Golden Glow)
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

        // 32x32 Hi-Res Sprites
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
