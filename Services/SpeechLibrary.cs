using System;

namespace GeminiScreenPet.Services
{
    public static class SpeechLibrary
    {
        private static readonly Random _rand = new();

        public static readonly string[] IdlePhrases = new[]
        {
            "✨ Привет! Я твой Gemini!",
            "⭐ Чистый код — залог успеха!",
            "🚀 Лови звёздный импульс!",
            "💙 Я прямо тут, над всеми окнами!",
            "👾 Блип-блоп! Исследую экран...",
            "☕ Не забудь выпить водички!",
            "🌟 Ноль багов сегодня в проде!",
            "🧠 Нейросети думают о прекрасном!",
            "💫 Вращаемся на орбите экрана!"
        };

        public static readonly string[] LovePhrases = new[]
        {
            "❤️ Ты замечательный разработчик!",
            "💖 Спасибо, что заботишься обо мне!",
            "🥰 Люблю помогать тебе с кодом!",
            "✨ Сердечки в пикселях светятся!"
        };

        public static readonly string[] PartyPhrases = new[]
        {
            "🎉 ТУСОВКА! Режим Super Star!",
            "🌟 Золотой режим активирован!",
            "🪩 Дэнсим под биты кулера процессора!",
            "✨ Ультра-звезда вошла в чат!"
        };

        public static string GetRandom(string[] array) => array[_rand.Next(array.Length)];
    }
}
