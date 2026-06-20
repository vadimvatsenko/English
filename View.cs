using ANSIConsole;
using English.Static;
using English.Utils;

namespace English;

public class View
{
    private readonly AuthService _authService;
    
    public View(AuthService authService)
    {
        _authService = authService;
    }

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

            string horizontalTop =
                $"  ╔════╦═══════════════════════════════════════════════╦═══════════╦═════════════════╦══════════════════╦═══════════════════════════╗";
            string horizontalBottom =
                $"  ╚════╩═══════════════════════════════════════════════╩═══════════╩═════════════════╩══════════════════╩═══════════════════════════╝";
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
                int tryes = rating != null ? rating.Tries : 0;
                int correctUnswers = rating != null ? rating.CorrectUnswers : 0;
                int allQuestions = rating != null ? rating.AllUnswers : 0;
                string data = rating != null ? rating.Date.ToString("dd.MM.yyyy HH:mm:ss") : "░░:░░:░░░░ ░░:░░:░░";
                // Выводим строку меню

                //float percentSuccess = ((correctUnswers == 0 ? 1 : correctUnswers) / (allQuestions == 0 ? 1 : allQuestions)) * 100;

                float percentSuccess = ((float)correctUnswers / (float)allQuestions) * 100f;

                // Если все вопросы = 0 и правильные = 0, результатом будет NaN
                // Если все вопросы = 0, а правильные > 0, результатом будет Infinity
                if (float.IsNaN(percentSuccess) || float.IsInfinity(percentSuccess))
                {
                    percentSuccess = 0;
                }

                string percentColorHex =
                    HexColorsLerp.LerpColorHex(StaticColors.Red, StaticColors.Green, percentSuccess);

                Console.Write(
                    $"{arrow} ║ {m.Key:00} ║ {centeredText}║ TRIES: {tryes.ToString("00")} ║ RATING: {correctUnswers:00} / {allQuestions:00} ║ "
                        .Background(backgroundColor)
                        .Color(foregroundColor).Bold());
                Console.Write($"Success: {percentSuccess.ToString("000.00")}% ".Background(backgroundColor)
                    .Color(percentColorHex).Bold());
                Console.WriteLine($"║ Date: {data} ║".Background(backgroundColor).Color(foregroundColor).Bold());


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

    public async Task QuestionsLogic(bool isEnToRu, string fileName, List<Sections> allQaList,
        Dictionary<int, string> levelsDict, User user)
    {
        int count = 1;
        int allQaCount = allQaList.Where(x => x.Examples.Length > 0).Sum(x => x.Examples.Length);

        bool IsExistRating = user.RatingText.Exists(r => r.NameTheme == fileName);
        Rating? currentRating = user.RatingText.FirstOrDefault(r => r.NameTheme == fileName);

        if (!IsExistRating)
        {
            currentRating = new Rating(fileName, 0, 0, allQaCount, DateTime.Now);
            user.RatingText.Add(currentRating);
        }

        currentRating.RatingClear();

        foreach (var d in allQaList)
        {
            foreach (var e in d.Examples)
            {
                int wrongAttemptsCount = 0;
                bool isEqual = false;

                while (!isEqual)
                {
                    Console.Clear();

                    Console.WriteLine(
                        $"Theme {fileName} Tryes {currentRating.Tries} [{currentRating.CorrectUnswers} / {currentRating.AllUnswers}]"
                            .GradientBackground(StaticColors.Gradient[0], StaticColors.Gradient[1],
                                StaticColors.Gradient[2], StaticColors.Gradient[3], StaticColors.Gradient[4]));

                    Console.WriteLine(
                        "╠════════════════════════════════════════════════════════════════════════════╣");

                    Console.WriteLine($"╠════ current QA number {count} / {allQaCount} ════╣"
                        .Gradient(StaticColors.Gradient[0], StaticColors.Gradient[1],
                            StaticColors.Gradient[2], StaticColors.Gradient[3], StaticColors.Gradient[4]));

                    Console.WriteLine(
                        "╠════════════════════════════════════════════════════════════════════════════╣");

                    Console.WriteLine($"CORRECT [{currentRating.CorrectUnswers}]".Color(StaticColors.Green) +
                                      " " +
                                      $"MISSTAKE [{currentRating.MissingUnswers}]".Color(StaticColors.Red));

                    Console.WriteLine(
                        "╠════════════════════════════════════════════════════════════════════════════╣");
                    Console.WriteLine(d.Title.Color(StaticColors.Magenta));
                    Console.WriteLine(d.Rule.Color(StaticColors.Magenta));

                    Console.WriteLine(
                        "╠════════════════════════════════════════════════════════════════════════════╣");

                    string correctText = isEnToRu ? e.Ru : e.En;
                    string questionText = isEnToRu ? e.En : e.Ru;


                    Console.WriteLine(questionText.Color(StaticColors.Blue));

                    Console.WriteLine("ENTER WORD:");
                    Console.WriteLine();

                    string words = Console.ReadLine()?.Trim() ?? string.Empty;

                    isEqual = string.Equals(
                        words,
                        correctText,
                        StringComparison.OrdinalIgnoreCase);

                    Console.WriteLine(
                        "╠═════════════════════════════════════════════════════════════════════════════╣");

                    Console.WriteLine(e.Ipa.Color(StaticColors.Magenta) + " ");
                    Console.WriteLine(words.Color(StaticColors.Yellow));
                    Console.WriteLine(correctText.Color(StaticColors.Green));

                    Console.WriteLine(
                        "╠═════════════════════════════════════════════════════════════════════════════╣");

                    Console.WriteLine(isEqual
                        ? "╠═════════════════════════ ● CORRECT ═════════════════════════╣"
                            .Color(StaticColors.White).Background(StaticColors.Green)
                        : "╠═════════════════════════ ❌ MISSTAKE ═════════════════════════╣"
                            .Color(StaticColors.White).Background(StaticColors.Red));

                    Console.ReadKey();

                    if (isEqual)
                    {
                        if (wrongAttemptsCount == 0)
                            currentRating.AddCorrectUnswers();
                    }
                    else
                    {
                        if (wrongAttemptsCount == 0)
                            currentRating.AddMissingUnswers();
                        wrongAttemptsCount++;
                    }
                }

                count++;
            }
        }

        currentRating.AddTries();
        currentRating.SetAllUnswers(allQaCount);
        currentRating.SetData();

        Console.WriteLine("SAVE PROGRESS...");
        await _authService.UpdateUsersAsync(user);

        Console.Clear();
        Console.WriteLine("=== ТЕМА ЗАВЕРШЕНА ===".Color(StaticColors.Green));
        Console.WriteLine($"Всего попыток: {currentRating.Tries}");
        Console.WriteLine($"Правильных ответов: {currentRating.CorrectUnswers} из {currentRating.AllUnswers}");
        Console.WriteLine("\nНажмите любую клавишу для возврата в меню...");
        Console.ReadKey();

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