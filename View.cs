using ANSIConsole;
using English.Static;

namespace English;

public class View
{
    // покраска и управление меню
        public int ColorizeMenuInput(Dictionary<int, string> menu, string header, bool clear = true)
        {
            int counter = 0;

            // Оптимизация: парсим заголовок ОДИН раз до начала цикла
            string[] headerParts = header.Split(new[] { ' ' }, 2);
            string name = headerParts[0];
            string otherText = headerParts.Length > 1 ? headerParts[1] : string.Empty;
            
            // Если clear = true, один раз очищаем экран перед запуском меню
            if (clear) Console.Clear();

            while (true)
            {
                // Вместо очистки возвращаем курсор в левый верхний угол
                Console.SetCursorPosition(0, 0);

                // Отрисовка заголовка
                Console.Write($"{name}".Background(StaticColors.Blue).Color(StaticColors.White).Bold());
                // Добавляем PadRight, чтобы затереть старый хвост, если текст изменится
                Console.Write($" {otherText}: ");
                Console.Write($"{menu[counter]}".Background(StaticColors.Blue).Color(StaticColors.White).Bold());

                Console.WriteLine(new string(' ', Console.WindowWidth)); // Очищаем пустую строку

                foreach (var m in menu)
                {
                    bool isActive = m.Key == counter;
                    string arrow = isActive ? "=>" : "  ";

                    string backgroundColor = isActive ? StaticColors.Blue : StaticColors.White;
                    string foregroundColor = isActive ? StaticColors.White : StaticColors.Blue;

                    // Центрирование текста
                    var centeredText = CenteredText(m.Value, 26);

                    // Выводим строку меню
                    Console.WriteLine($"{arrow} [{m.Key:00}]: [{centeredText}]".Background(backgroundColor)
                        .Color(foregroundColor).Bold());
                }

                // Очищаем оставшуюся нижнюю часть экрана на случай, если меню уменьшилось
                // (актуально, если этот метод вызывается для меню с разным количеством элементов)
                for (int i = 0; i < 2; i++)
                {
                    Console.WriteLine(new string(' ', Console.WindowWidth));
                }

                ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                if (keyInfo.Key == ConsoleKey.DownArrow)
                {
                    counter++;
                    counter %= menu.Count;
                }
                else if (keyInfo.Key == ConsoleKey.UpArrow)
                {
                    counter--;
                    if (counter < 0)
                    {
                        counter = menu.Count - 1;
                    }
                }
                else if (keyInfo.Key == ConsoleKey.Enter)
                {
                    // Перед выходом возвращаем видимость курсора назад
                    Console.CursorVisible = true;
                    return counter;
                }
            }
        }
        
        // центрирование текста в меню
        private static string CenteredText(string text, int width)
        {
            int spaces = width - text.Length;
            int padLeft = spaces / 2 + text.Length;
            string centeredText = text.PadLeft(padLeft).PadRight(width);
            return centeredText;
        }
}