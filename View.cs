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
        int maxVisible = 15; // Сколько элементов видно одновременно

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
            Console.Write($" {otherText}");
            Console.Write($"{menu[counter]}".Background(StaticColors.Blue).Color(StaticColors.White).Bold());

            Console.WriteLine(new string(' ', Console.WindowWidth)); // Очищаем пустую строку

            string horizontalTop =
                $"  ╔════╦══════════════════════════════════════════════════════╦═══════════╦═════════════════╦══════════════════╦═══════════════════════════╗";
            string horizontalBottom =
                $"  ╚════╩══════════════════════════════════════════════════════╩═══════════╩═════════════════╩══════════════════╩═══════════════════════════╝";
            Console.WriteLine($"{horizontalTop}".Background(StaticColors.White).Color(StaticColors.Blue).Bold());

            // Логика скроллинга
            int start = Math.Max(0, Math.Min(counter - maxVisible / 2, menu.Count - maxVisible));
            if (counter < maxVisible / 2) start = 0;
            int end = Math.Min(menu.Count, start + maxVisible);

            for (int i = start; i < end; i++)
            {
                var m = menu.ElementAt(i);
                bool isActive = m.Key == counter;
                string arrow = isActive ? ">" : " ";

                string backgroundColor = isActive ? StaticColors.Blue : StaticColors.White;
                string foregroundColor = isActive ? StaticColors.White : StaticColors.Blue;

                // Центрирование текста
                var centeredText = LeftText(m.Value, 53);

                var rating = user.RatingText.FirstOrDefault(r => r.NameTheme == m.Value);
                int tryes = rating != null ? rating.Tries : 0;
                int correctUnswers = rating != null ? rating.CorrectUnswers : 0;
                int allQuestions = rating != null ? rating.AllUnswers : 0;
                string data = rating != null ? rating.Date.ToString("dd.MM.yyyy HH:mm:ss") : "░░:░░:░░░░ ░░:░░:░░";

                float percentSuccess = allQuestions == 0 ? 0 : ((float)correctUnswers / (float)allQuestions) * 100f;

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

            // Если список меньше maxVisible, заполняем пустоту
            for (int i = menu.Count; i < maxVisible; i++)
            {
                Console.WriteLine(new string(' ', Console.WindowWidth));
            }

            Console.WriteLine($"{horizontalBottom}".Background(StaticColors.White).Color(StaticColors.Blue).Bold());

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

    private enum AnswerInputResult
    {
        Submitted,
        ExitTraining,
        RestartTraining
    }

    public async Task QuestionsLogic(bool isEnToRu, string fileName, List<Sections> allQaList,
        Dictionary<int, string> levelsDict, User user)
    {
        int allQaCount = allQaList.Where(x => x.Examples.Length > 0).Sum(x => x.Examples.Length);

        bool IsExistRating = user.RatingText.Exists(r => r.NameTheme == fileName);
        Rating? currentRating = user.RatingText.FirstOrDefault(r => r.NameTheme == fileName);

        if (!IsExistRating)
        {
            currentRating = new Rating(fileName, 0, 0, allQaCount, DateTime.Now);
            user.RatingText.Add(currentRating);
        }

        // история введённых ответов (стрелки вверх/вниз) - переживает рестарт темы по F5
        var answerHistory = new List<string>();

        // тренировку можно перезапустить (F5) - тогда весь цикл ниже стартует заново
        while (true)
        {
            // снимок состояния до попытки - нужен, чтобы откатить прогресс,
            // если пользователь на выходе/рестарте откажется его сохранять
            int snapshotTries = currentRating.Tries;
            int snapshotCorrect = currentRating.CorrectUnswers;
            int snapshotMissing = currentRating.MissingUnswers;
            int snapshotAll = currentRating.AllUnswers;
            DateTime snapshotDate = currentRating.Date;

            currentRating.RatingClear();

            int count = 1;
            bool exitRequested = false;
            bool restartRequested = false;

            foreach (var d in allQaList)
            {
                foreach (var e in d.Examples)
                {
                    int wrongAttemptsCount = 0;
                    bool isEqual = false;

                    while (!isEqual)
                    {
                    Console.Clear();

                    PrintQuestionHeader(fileName, d, currentRating, count, allQaCount);

                    string correctText = isEnToRu ? e.Ru : e.En;
                    string questionText = isEnToRu ? e.En : e.Ru;

                    Console.WriteLine();
                    foreach (string questionLine in WrapText(questionText, HeaderWidth))
                        Console.WriteLine(questionLine.Color(StaticColors.Blue).Bold());

                    Console.WriteLine();
                    Console.WriteLine("ENTER WORD:");
                    Console.WriteLine();

                    AnswerInputResult inputResult = ReadAnswerWithHotkeys(answerHistory, out string words);

                    if (inputResult == AnswerInputResult.ExitTraining)
                    {
                        exitRequested = true;
                        break;
                    }

                    if (inputResult == AnswerInputResult.RestartTraining)
                    {
                        restartRequested = true;
                        break;
                    }

                    isEqual = string.Equals(
                        words,
                        correctText,
                        StringComparison.OrdinalIgnoreCase);

                    Console.WriteLine();
                    PrintAnswerResult(words, correctText, e.Ipa, isEqual);

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

                    if (exitRequested || restartRequested) break;

                    count++;
                }

                if (exitRequested || restartRequested) break;
            }

            // не пройденные до конца вопросы (при ESC/F5) засчитываем как проваленные,
            // чтобы результат честно сохранялся в любом исходе тренировки
            int answeredCount = currentRating.CorrectUnswers + currentRating.MissingUnswers;
            int unfinishedCount = Math.Max(0, allQaCount - answeredCount);
            for (int i = 0; i < unfinishedCount; i++)
                currentRating.AddMissingUnswers();

            currentRating.AddTries();
            currentRating.SetAllUnswers(allQaCount);
            currentRating.SetData();

            // при досрочном выходе/рестарте (ESC/F5) спрашиваем, сохранять ли прогресс,
            // а не сохраняем его молча
            bool shouldSave = true;
            if (exitRequested || restartRequested)
            {
                shouldSave = AskSaveConfirmation(exitRequested
                    ? "Тренировка прервана"
                    : "Тема будет начата заново");
            }

            if (shouldSave)
            {
                Console.WriteLine("SAVE PROGRESS...");
                await _authService.UpdateUsersAsync(user);
            }
            else
            {
                // откатываем изменения этой попытки, чтобы они не осели в памяти без сохранения
                currentRating.Restore(snapshotTries, snapshotCorrect, snapshotMissing, snapshotAll, snapshotDate);
                Console.WriteLine("PROGRESS NOT SAVED...");
            }

            if (exitRequested)
            {
                PrintResultBox("ТРЕНИРОВКА ПРЕРВАНА", StaticColors.Red, currentRating);
                Console.WriteLine();
                Console.WriteLine("Нажмите любую клавишу для возврата к выбору уровня...".Color(StaticColors.White));
                Console.ReadKey();
                return;
            }

            if (restartRequested)
            {
                PrintResultBox("ТЕМА СБРОШЕНА - НАЧИНАЕМ ЗАНОВО", StaticColors.Yellow, currentRating);
                await Task.Delay(900);
                continue;
            }

            PrintResultBox("ТЕМА ЗАВЕРШЕНА", StaticColors.Green, currentRating);
            Console.WriteLine();

            if (AskRepeatConfirmation())
                continue;

            return;
        }
    }

    // запрос подтверждения сохранения прогресса при выходе (ESC) или рестарте (F5) тренировки
    private static bool AskSaveConfirmation(string reasonText)
    {
        const int width = 70;
        const string margin = "  ";
        string top = margin + "╔" + new string('═', width) + "╗";
        string bottom = margin + "╚" + new string('═', width) + "╝";

        Console.WriteLine();
        Console.WriteLine(top.Color(StaticColors.Yellow).Background(StaticColors.White).Bold());
        Console.WriteLine((margin + "║" + CenteredText(reasonText, width) + "║")
            .Color(StaticColors.Yellow).Background(StaticColors.White).Bold());
        Console.WriteLine((margin + "║" + CenteredText("Сохранить прогресс? (Y/N)", width) + "║")
            .Color(StaticColors.Blue).Background(StaticColors.White).Bold());
        Console.WriteLine(bottom.Color(StaticColors.Yellow).Background(StaticColors.White).Bold());

        while (true)
        {
            ConsoleKeyInfo keyInfo = Console.ReadKey(true);

            if (keyInfo.Key == ConsoleKey.Y || keyInfo.Key == ConsoleKey.Enter)
                return true;

            if (keyInfo.Key == ConsoleKey.N || keyInfo.Key == ConsoleKey.Escape)
                return false;
        }
    }

    // запрос после успешного завершения темы: повторить тему или выйти в меню выбора уровня
    private static bool AskRepeatConfirmation()
    {
        const int width = 70;
        const string margin = "  ";
        string top = margin + "╔" + new string('═', width) + "╗";
        string bottom = margin + "╚" + new string('═', width) + "╝";

        Console.WriteLine(top.Color(StaticColors.Green).Background(StaticColors.White).Bold());
        Console.WriteLine((margin + "║" + CenteredText("Повторить тему? (Y/N)", width) + "║")
            .Color(StaticColors.Blue).Background(StaticColors.White).Bold());
        Console.WriteLine((margin + "║" + CenteredText("[Y] - повторить    [N] / [Enter] - выйти в меню", width) + "║")
            .Color(StaticColors.Blue).Background(StaticColors.White).Bold());
        Console.WriteLine(bottom.Color(StaticColors.Green).Background(StaticColors.White).Bold());

        while (true)
        {
            ConsoleKeyInfo keyInfo = Console.ReadKey(true);

            if (keyInfo.Key == ConsoleKey.Y)
                return true;

            if (keyInfo.Key == ConsoleKey.N || keyInfo.Key == ConsoleKey.Enter || keyInfo.Key == ConsoleKey.Escape)
                return false;
        }
    }

    // красивая карточка результата в стиле таблицы выбора уровней
    private static void PrintResultBox(string title, string titleColor, Rating rating)
    {
        Console.Clear();

        const int width = 70;
        const string margin = "  ";
        string top = margin + "╔" + new string('═', width) + "╗";
        string sep = margin + "╠" + new string('═', width) + "╣";
        string bottom = margin + "╚" + new string('═', width) + "╝";

        float percentSuccess = rating.AllUnswers == 0
            ? 0
            : (float)rating.CorrectUnswers / rating.AllUnswers * 100f;
        string percentColor = HexColorsLerp.LerpColorHex(StaticColors.Red, StaticColors.Green, percentSuccess);

        void Row(string text, string color)
        {
            Console.WriteLine((margin + "║" + CenteredText(text, width) + "║")
                .Color(color).Background(StaticColors.White).Bold());
        }

        Console.WriteLine(top.Color(titleColor).Background(StaticColors.White).Bold());
        Row(title, titleColor);
        Console.WriteLine(sep.Color(titleColor).Background(StaticColors.White).Bold());
        Row($"Тема: {rating.NameTheme}", StaticColors.Blue);
        Row($"Попыток: {rating.Tries}", StaticColors.Blue);
        PrintMixedRow(margin, width,
            ($"Правильно: {rating.CorrectUnswers}", StaticColors.Green),
            ("  ", StaticColors.Blue),
            ($"Ошибок: {rating.MissingUnswers}", StaticColors.Red),
            ($"  Всего: {rating.AllUnswers}", StaticColors.Blue));
        Row($"Успех: {percentSuccess:000.00}%", percentColor);
        Row($"Дата: {rating.Date:dd.MM.yyyy HH:mm:ss}", StaticColors.Blue);
        Console.WriteLine(bottom.Color(titleColor).Background(StaticColors.White).Bold());
    }

    // строка таблицы с несколькими сегментами разного цвета, но общим центрированием
    private static void PrintMixedRow(string margin, int width, params (string text, string color)[] segments)
    {
        int totalLength = segments.Sum(s => s.text.Length);
        int spaces = Math.Max(0, width - totalLength);
        int padLeft = spaces / 2;
        int padRight = spaces - padLeft;

        Console.Write((margin + "║").Color(StaticColors.Blue).Background(StaticColors.White));
        Console.Write(new string(' ', padLeft).Background(StaticColors.White));
        foreach (var (text, color) in segments)
            Console.Write(text.Color(color).Background(StaticColors.White).Bold());
        Console.Write(new string(' ', padRight).Background(StaticColors.White));
        Console.WriteLine("║".Color(StaticColors.Blue).Background(StaticColors.White));
    }

    private const int HeaderWidth = 78;

    // шапка вопроса в виде единой таблицы: тема -> название/правило темы ->
    // попытки/дата -> текущий вопрос и счёт -> хоткеи
    private static void PrintQuestionHeader(string fileName, Sections section, Rating rating, int count,
        int allQaCount)
    {
        string top = "╔" + new string('═', HeaderWidth) + "╗";
        string sep = "╠" + new string('═', HeaderWidth) + "╣";
        string bottom = "╚" + new string('═', HeaderWidth) + "╝";

        void Row(string text, string color)
        {
            Console.WriteLine(("║" + CenteredText(text, HeaderWidth) + "║")
                .Color(color).Background(StaticColors.White).Bold());
        }

        void Sep() => Console.WriteLine(sep.Color(StaticColors.Blue).Background(StaticColors.White).Bold());

        Console.WriteLine(top.Color(StaticColors.Blue).Background(StaticColors.White).Bold());
        Row($"Тема: {fileName}", StaticColors.Green);
        Sep();

        foreach (string titleLine in WrapText(section.Title, HeaderWidth))
            Row(titleLine, StaticColors.Magenta);
        foreach (string ruleLine in WrapText(section.Rule, HeaderWidth))
            Row(ruleLine, StaticColors.Magenta);
        Sep();

        Row($"Попыток: {rating.Tries}   Последний запуск: {rating.Date:dd.MM.yyyy HH:mm:ss}", StaticColors.Blue);
        Sep();

        PrintMixedRow(string.Empty, HeaderWidth,
            ($"Вопрос {count} / {allQaCount}   ", StaticColors.Blue),
            ($"Правильно: {rating.CorrectUnswers}", StaticColors.Green),
            ("   ", StaticColors.Blue),
            ($"Неправильно: {rating.MissingUnswers}", StaticColors.Red));
        Sep();

        Row("[ESC] - выйти из тренировки    [F5] - начать тему заново", StaticColors.Yellow);
        Console.WriteLine(bottom.Color(StaticColors.Blue).Background(StaticColors.White).Bold());
    }

    // низ экрана вопроса: посимвольное сравнение ответа, правильный вариант, транскрипция и вердикт - тоже таблицей
    private static void PrintAnswerResult(string userAnswer, string correctAnswer, string ipa, bool isCorrect)
    {
        string top = "╔" + new string('═', HeaderWidth) + "╗";
        string sep = "╠" + new string('═', HeaderWidth) + "╣";
        string bottom = "╚" + new string('═', HeaderWidth) + "╝";

        void OpenRow() => Console.Write("║".Color(StaticColors.Blue).Background(StaticColors.White));

        void CloseRow(int written)
        {
            int pad = Math.Max(0, HeaderWidth - written);
            Console.Write(new string(' ', pad).Background(StaticColors.White));
            Console.WriteLine("║".Color(StaticColors.Blue).Background(StaticColors.White));
        }

        void LeftRow(string label, string value, string valueColor)
        {
            OpenRow();
            Console.Write(label.Color(StaticColors.Blue).Background(StaticColors.White).Bold());
            int available = Math.Max(0, HeaderWidth - label.Length);
            string clipped = value.Length > available ? value.Substring(0, available) : value;
            Console.Write(clipped.Color(valueColor).Background(StaticColors.White));
            CloseRow(label.Length + clipped.Length);
        }

        Console.WriteLine(top.Color(StaticColors.Blue).Background(StaticColors.White).Bold());

        // подписи выравниваем до одной ширины, чтобы сами значения (ответ / верный ответ / транскрипция)
        // начинались строго в одной колонке - так видно, в какой именно букве ошибка
        string labelYourAnswer = " Ваш ответ:";
        string labelCorrectAnswer = " Верный ответ:";
        string labelTranscription = " Транскрипция:";
        int labelWidth = new[] { labelYourAnswer.Length, labelCorrectAnswer.Length, labelTranscription.Length }
            .Max() + 1;
        labelYourAnswer = labelYourAnswer.PadRight(labelWidth);
        labelCorrectAnswer = labelCorrectAnswer.PadRight(labelWidth);
        labelTranscription = labelTranscription.PadRight(labelWidth);

        // посимвольный дифф введённого ответа и правильного
        OpenRow();
        Console.Write(labelYourAnswer.Color(StaticColors.Blue).Background(StaticColors.White).Bold());

        int maxLength = Math.Max(userAnswer.Length, correctAnswer.Length);
        string paddedUser = userAnswer.PadRight(maxLength);
        string paddedCorrect = correctAnswer.PadRight(maxLength);
        int written = labelYourAnswer.Length;

        for (int i = 0; i < maxLength && written < HeaderWidth; i++)
        {
            bool isMatch = char.ToLower(paddedCorrect[i]) == char.ToLower(paddedUser[i]);
            Console.Write(paddedUser[i].ToString()
                .Color(StaticColors.White).Background(isMatch ? StaticColors.Green : StaticColors.Red));
            written++;
        }
        CloseRow(written);

        LeftRow(labelCorrectAnswer, correctAnswer, StaticColors.Green);
        LeftRow(labelTranscription, ipa, StaticColors.Magenta);

        Console.WriteLine(sep.Color(StaticColors.Blue).Background(StaticColors.White).Bold());

        // ASCII-символ вместо эмодзи "❌" - у эмодзи двойная визуальная ширина в терминале,
        // из-за чего расчёт центрирования по .Length съезжал и рамка сдвигалась на 1 символ
        string status = isCorrect ? "* CORRECT" : "X MISSTAKE";
        Console.WriteLine(("║" + CenteredText(status, HeaderWidth) + "║")
            .Color(StaticColors.White).Background(isCorrect ? StaticColors.Green : StaticColors.Red).Bold());

        Console.WriteLine(bottom.Color(StaticColors.Blue).Background(StaticColors.White).Bold());
    }

    // разбивает длинную строку на строки, помещающиеся в рамку заданной ширины
    private static IEnumerable<string> WrapText(string text, int maxWidth)
    {
        if (string.IsNullOrEmpty(text))
        {
            yield return string.Empty;
            yield break;
        }

        var line = new System.Text.StringBuilder();

        foreach (string word in text.Split(' '))
        {
            string remaining = word;

            // само слово длиннее рамки - режем его по месту
            while (remaining.Length > maxWidth)
            {
                if (line.Length > 0)
                {
                    yield return line.ToString();
                    line.Clear();
                }

                yield return remaining.Substring(0, maxWidth);
                remaining = remaining.Substring(maxWidth);
            }

            if (line.Length > 0 && line.Length + 1 + remaining.Length > maxWidth)
            {
                yield return line.ToString();
                line.Clear();
            }

            if (line.Length > 0)
                line.Append(' ');

            line.Append(remaining);
        }

        if (line.Length > 0)
            yield return line.ToString();
    }

    // построчный ввод ответа с поддержкой хоткеев выхода/рестарта тренировки
    // и навигации по истории введённых ответов стрелками вверх/вниз
    private static AnswerInputResult ReadAnswerWithHotkeys(List<string> history, out string words)
    {
        var buffer = new System.Text.StringBuilder();
        int cursorPos = 0;
        int historyIndex = history.Count;

        int inputLeft = Console.CursorLeft;
        int inputTop = Console.CursorTop;

        // перерисовывает весь буфер и ставит реальный курсор консоли на cursorPos -
        // нужно, чтобы Left/Right/Home/End/Delete работали не только по хвосту строки
        void Redraw()
        {
            Console.SetCursorPosition(inputLeft, inputTop);
            Console.Write(new string(' ', Math.Max(0, Console.WindowWidth - inputLeft - 1)));
            Console.SetCursorPosition(inputLeft, inputTop);
            Console.Write(buffer.ToString());
            Console.SetCursorPosition(inputLeft + cursorPos, inputTop);
        }

        while (true)
        {
            ConsoleKeyInfo keyInfo = Console.ReadKey(true);

            if (keyInfo.Key == ConsoleKey.Escape)
            {
                words = string.Empty;
                return AnswerInputResult.ExitTraining;
            }

            if (keyInfo.Key == ConsoleKey.F5)
            {
                words = string.Empty;
                return AnswerInputResult.RestartTraining;
            }

            if (keyInfo.Key == ConsoleKey.Enter)
            {
                words = buffer.ToString().Trim();
                if (!string.IsNullOrEmpty(words))
                    history.Add(words);
                return AnswerInputResult.Submitted;
            }

            if (keyInfo.Key == ConsoleKey.UpArrow)
            {
                if (history.Count > 0 && historyIndex > 0)
                {
                    historyIndex--;
                    buffer.Clear();
                    buffer.Append(history[historyIndex]);
                    cursorPos = buffer.Length;
                    Redraw();
                }
                continue;
            }

            if (keyInfo.Key == ConsoleKey.DownArrow)
            {
                if (historyIndex < history.Count)
                {
                    historyIndex++;
                    buffer.Clear();
                    if (historyIndex < history.Count)
                        buffer.Append(history[historyIndex]);
                    cursorPos = buffer.Length;
                    Redraw();
                }
                continue;
            }

            if (keyInfo.Key == ConsoleKey.LeftArrow)
            {
                if (cursorPos > 0)
                {
                    cursorPos--;
                    Console.SetCursorPosition(inputLeft + cursorPos, inputTop);
                }
                continue;
            }

            if (keyInfo.Key == ConsoleKey.RightArrow)
            {
                if (cursorPos < buffer.Length)
                {
                    cursorPos++;
                    Console.SetCursorPosition(inputLeft + cursorPos, inputTop);
                }
                continue;
            }

            if (keyInfo.Key == ConsoleKey.Home)
            {
                cursorPos = 0;
                Console.SetCursorPosition(inputLeft + cursorPos, inputTop);
                continue;
            }

            if (keyInfo.Key == ConsoleKey.End)
            {
                cursorPos = buffer.Length;
                Console.SetCursorPosition(inputLeft + cursorPos, inputTop);
                continue;
            }

            if (keyInfo.Key == ConsoleKey.Backspace)
            {
                if (cursorPos > 0)
                {
                    buffer.Remove(cursorPos - 1, 1);
                    cursorPos--;
                    Redraw();
                }
                continue;
            }

            if (keyInfo.Key == ConsoleKey.Delete)
            {
                if (cursorPos < buffer.Length)
                {
                    buffer.Remove(cursorPos, 1);
                    Redraw();
                }
                continue;
            }

            if (!char.IsControl(keyInfo.KeyChar))
            {
                buffer.Insert(cursorPos, keyInfo.KeyChar);
                cursorPos++;
                Redraw();
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