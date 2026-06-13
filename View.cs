using ANSIConsole;
using English.Static;

namespace English;

public class View
{
        // покраска и управление меню
        public int ColorizeMenuInput(Dictionary<int, string> menu, string header)
        {
            int counter = 0;

            // Оптимизация: парсим заголовок ОДИН раз до начала цикла
            string[] headerParts = header.Split(new[] { ' ' }, 2);
            string name = headerParts[0];
            string otherText = headerParts.Length > 1 ? headerParts[1] : string.Empty;
            
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
                
                string horizontalTop = $"  ╔════╦═════════════════════════════════════════════════════════════╗";
                string horizontalBottom = $"  ╚════╩═════════════════════════════════════════════════════════════╝";
                Console.WriteLine($"{horizontalTop}".Background(StaticColors.White).Color(StaticColors.Blue).Bold());

                foreach (var m in menu)
                {
                    bool isActive = m.Key == counter;
                    string arrow = isActive ? ">" : " ";

                    string backgroundColor = isActive ? StaticColors.Blue : StaticColors.White;
                    string foregroundColor = isActive ? StaticColors.White : StaticColors.Blue;

                    // Центрирование текста
                    var centeredText = CenteredText(m.Value, 60);

                    // Выводим строку меню
                    Console.WriteLine($"{arrow} ║ {m.Key:00} ║ {centeredText}║".Background(backgroundColor)
                        .Color(foregroundColor).Bold());
                }
                
                Console.WriteLine($"{horizontalBottom}".Background(StaticColors.White).Color(StaticColors.Blue).Bold());

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
        
        public int ColorizeMenuInput(Dictionary<int, string> menu, User user, string header)
        {
            int counter = 0;

            // Оптимизация: парсим заголовок ОДИН раз до начала цикла
            string[] headerParts = header.Split(new[] { ' ' }, 2);
            string name = headerParts[0];
            string otherText = headerParts.Length > 1 ? headerParts[1] : string.Empty;
            
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
                
                string horizontalTop = $"  ╔════╦═══════════════════════════════════════════════╦═══════════╦═════════════════╦═══════════════════════════╗";
                string horizontalBottom = $"  ╚════╩═══════════════════════════════════════════════╩═══════════╩═════════════════╩═══════════════════════════╝";
                Console.WriteLine($"{horizontalTop}".Background(StaticColors.White).Color(StaticColors.Blue).Bold());
                
                foreach (var m in menu)
                {
                    bool isActive = m.Key == counter;
                    string arrow = isActive ? ">" : " ";

                    string backgroundColor = isActive ? StaticColors.Blue : StaticColors.White;
                    string foregroundColor = isActive ? StaticColors.White : StaticColors.Blue;

                    // Центрирование текста
                    var centeredText = LeftText(m.Value, 46);

                    var rating = user.RatingText.FirstOrDefault(r => r.NameTheme == m.Value);
                    int tryes = rating != null? rating.Tries : 0;
                    int correctUnswers = rating != null ? rating.CorrectUnswers : 0;
                    int allQuestions = rating != null ? rating.AllUnswers : 0;
                    string data = rating != null ? rating.Date.ToString("dd.MM.yyyy HH:mm:ss") : "░░:░░:░░░░ ░░:░░:░░" ;
                    // Выводим строку меню
                    
                    
                    Console.WriteLine($"{arrow} ║ {m.Key:00} ║ {centeredText}║ TRIES: {tryes.ToString("00")} " +
                                      $"║ RATING: {correctUnswers:00} / {allQuestions:00} ║ Date: {data} " +
                                      $"║".Background(backgroundColor)
                                        .Color(foregroundColor).Bold());
                }
                
                
                Console.WriteLine($"{horizontalBottom}".Background(StaticColors.White).Color(StaticColors.Blue).Bold());

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
        
        private static string RightText(string text, int width)
        {
            // Отнимаем 1 из общей ширины, чтобы оставить пустой символ справа
            int targetWidth = width - 1;

            // Если текст длиннее, чем доступное место, возвращаем его как есть (или с обрезанным краем)
            if (text.Length >= targetWidth)
            {
                return text.PadRight(width); 
            }

            // Дополняем текст пробелами СЛЕВА до нужной ширины, 
            // а затем добавляем 1 пробел СПРАВА
            return text.PadLeft(targetWidth) + " ";
        }
        
        private static string LeftText(string text, int width)
        {
            // Если текст пустой или слишком длинный, просто возвращаем его с отступами,
            // но проверяем, чтобы не выйти за рамки width
            if (text.Length + 1 >= width)
            {
                return " " + text;
            }

            // Добавляем 1 пробел слева, а затем дополняем строку пробелами справа до общей ширины
            return (" " + text).PadRight(width);
        }
}