using System.Globalization;

namespace English.Utils;

public static class HexColorsLerp
{
    /// <summary>
    /// Универсальный Lerp между двумя HEX-цветами по проценту (0 - 100)
    /// </summary>
    public static string LerpColorHex(string startHex, string endHex, double percent)
    {
        // 1. Парсим HEX-строки в RGB компоненты
        var (r1, g1, b1) = ParseHex(startHex);
        var (r2, g2, b2) = ParseHex(endHex);

        // 2. Переводим процент в коэффициент от 0.0 до 1.0 с ограничением границ
        double t = Math.Clamp(percent / 100.0, 0.0, 1.0);

        // 3. Вычисляем интерполяцию для каждого канала: A + (B - A) * t
        int r = (int)(r1 + (r2 - r1) * t);
        int g = (int)(g1 + (g2 - g1) * t);
        int b = (int)(b1 + (b2 - b1) * t);

        // 4. Собираем обратно в HEX-строку
        return $"#{r:X2}{g:X2}{b:X2}";
    }

    /// <summary>
    /// Вспомогательный метод для конвертации HEX в RGB кортеж
    /// </summary>
    static (int r, int g, int b) ParseHex(string hex)
    {
        // Убираем решетку, если она есть
        hex = hex.TrimStart('#');

        // Если передали короткий хак вроде "F00" вместо "FF0000"
        if (hex.Length == 3)
        {
            hex = $"{hex[0]}{hex[0]}{hex[1]}{hex[1]}{hex[2]}{hex[2]}";
        }

        // Вырезаем по 2 символа на канал и парсим из 16-ричной системы
        int r = int.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
        int g = int.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
        int b = int.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);

        return (r, g, b);
    }
}

