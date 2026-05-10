using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;

namespace English
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            EncodingSetup();
            // вернёт назад authService и user
            var (authService, user) = await StartMenu();

            if (user != null)
            {
                // Словарь с названиями уровней, цифра номер string название json
                Dictionary<int, string> levelsDict = FillLevelDictionaryNames();
                await UserChoiceMenu(user, levelsDict, authService);
            }
            else
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Red;
                string points = "..........";
                Console.Write($"Exit");
                foreach (var p in points)
                {
                    Console.Write(p);
                    await Task.Delay(100);
                }
            }
        }
        
        // стартовое меню с регистрацией и логинизацией
        private static async Task<(AuthService authService, User? user)> StartMenu()
        {
            AuthService authService = new AuthService();
            await authService.InitializeAsync();

            string messageHeader = "Enter you option";
            int inputInt = ColorizeMenuInput(StaticFields.MainMenu, messageHeader);
            
            User? user = null;

            switch (inputInt)
            {
                case 0:
                    user = await authService.LoginUserAsync();
                    break;
                case 1:
                    user = await authService.RegisterUserAsync();
                    break;
                case 2:
                    return (authService, user);
                default:
                    Console.WriteLine($"[{inputInt}]: Invalid option.");
                    break;
            }

            return (authService, user);
        }
        
        // покраска и управление меню
        private static int ColorizeMenuInput(Dictionary<int, string> menu, string header)
        {
            int counter = 0;

            while (true)
            {
                Console.WriteLine(header);
                Console.WriteLine();
                
                foreach (var m in menu)
                {
                    string arrow = m.Key == counter ? "=>" : "  "; 
                    Console.BackgroundColor = m.Key == counter ? ConsoleColor.DarkGreen :  ConsoleColor.DarkBlue;
                    
                    // центрирование текста
                    var centeredText = CenteredText(m.Value, 22);

                    Console.WriteLine($"{arrow} [{m.Key:00}]: [{centeredText}]");
                    
                    Console.ResetColor();
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
                    counter %= menu.Count;
                    
                    if (counter < 0)
                    {
                        counter = menu.Count - 1;
                    }
                }
                else if (keyInfo.Key == ConsoleKey.Enter)
                {
                    return counter;
                }
                
                Console.Clear();
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

        // заполняем словарь с уровнями, просто название и номер
        private static Dictionary<int, string> FillLevelDictionaryNames()
        {
            Dictionary<int, string> levelsDict = new Dictionary<int, string>();
            int folderCount = 0;
            // тут берем все папки из корневой директории
            string[] folders = Directory.GetDirectories(StaticFields.PathAllFiles);
                
            foreach (string folder in folders)
            {
                if ((folder.Contains("A") || folder.Contains("B") || folder.Contains("C") ||
                     folder.Contains("Numbers") )&& !folder.Contains("save data"))
                {
                    // получим последнюю папку
                    string floderName = Path.GetFileName(folder);
                    levelsDict.Add(folderCount, floderName);
                    folderCount++;
                }
            }
            return levelsDict;
        }
        
        // меню выбора
        private static async Task UserChoiceMenu(User user, Dictionary<int, string> levelsDict, AuthService authService)
        {
            Console.Clear();
            
            string messageHeader = $"{user.Name} please enter option";
            int userMenuInt = ColorizeMenuInput(StaticFields.UserMenu, messageHeader);
            
            switch (userMenuInt)
            {
                case 0 :
                    return;
                case 1:
                    await StartPractice(levelsDict, user, authService);
                    break;
                case 2:
                    await ShowHardQuestions(user.HardQuestion, false, user, authService);
                    break;
                default:
                    Console.WriteLine("Wrong menu option!");
                    break;
            }
        }

        // практика, тут нужно выбрать уровень
        private static async Task StartPractice(Dictionary<int, string> levelsDict, User user, AuthService authService)
        {
            Console.Clear();
            int numberLevel = ColorizeMenuInput(levelsDict, "Enter Level: ");
            string levelName = levelsDict[numberLevel];
            
            bool isValidLevel = false;
            
            Console.WriteLine($"Current Level: {levelsDict[numberLevel]}");

            // логика выбора урока в теме
            // 1. получаем все уроки в папке
            string[] filesOnTheme = Directory.GetFiles( StaticFields.PathAllFiles + levelName);
            Dictionary<int, string> filesOnThemeDict = new Dictionary<int, string>();

            int coutFiles = 0;
            
            Console.WriteLine();
            
            // запоняем словарь данными
            foreach (string file in filesOnTheme)
            {
                string name = Path.GetFileNameWithoutExtension(file);
                
                filesOnThemeDict.Add(coutFiles, name);
                coutFiles++;
            }
            
            Console.Clear();
            int themeNumber = ColorizeMenuInput(filesOnThemeDict, "Enter Theme: ");

            string fileName = ChooseDir(filesOnThemeDict, themeNumber);
            
            // полный путь к теме

            string FilePath = String.Empty;

            // путь в зависимости от платформы
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                FilePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, $"{levelName}\\{fileName}.json"));
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                FilePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, $"{levelName}/{fileName}.json"));
            //
            
            Data dataList = await GetAsync(FilePath);
            
            Console.Clear();
            string message = "Enter your option:";
            int option = ColorizeMenuInput(StaticFields.EnglishMenu, message);
            
            switch (option)
            {
                case 0:
                    Extensions(dataList, false, fileName, user, authService);
                    break;
                case 1:
                    Extensions(dataList, true, fileName, user, authService);
                    break;
            }

            Console.ReadKey();
        }
        
        private static void Extensions(Data? dataList, bool isEnToRu, string fileName, User user, AuthService authService)
        {
            if (dataList == null || dataList.Sections == null)
            {
                Console.WriteLine($"The Questions Is Over");
                return;
            }
            
            int count = 1;
            int correctAnswer = 0;
            int misstakeAnswer = 0;
            int allQaCount = 0;

            var allQaList = dataList.Sections;

            foreach (var d in allQaList)
            {
                allQaCount += d.Examples.Length;
            }
            
            QuestionsLogic(isEnToRu, fileName, allQaList, count, allQaCount, correctAnswer, misstakeAnswer, user, authService);
        }

        private static void QuestionsLogic(bool isEnToRu, string fileName, List<Sections> allQaList, int count,
            int allQaCount,
            int correctAnswer, int misstakeAnswer, User user, AuthService authService)
        {
            foreach (var d in allQaList)
            {
                foreach (var e in d.Examples)
                {
                    bool isEqual = false;

                    while (!isEqual)
                    {
                        Console.Clear();

                        Console.WriteLine($"Theme {fileName}");
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine($" === current QA {count} / {allQaCount} ===");

                        Console.WriteLine();
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"CORRECT {correctAnswer}");

                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine($"MISSTAKE {misstakeAnswer}");

                        Console.WriteLine();
                        Console.ForegroundColor = ConsoleColor.DarkMagenta;
                        Console.WriteLine(d.Title);
                        Console.WriteLine(d.Rule);

                        Console.WriteLine();

                        string correctText = isEnToRu ? e.Ru : e.En;
                        string questionText = isEnToRu ? e.En : e.Ru;

                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine(questionText);

                        Console.WriteLine("Enter Word:");
                        Console.WriteLine();

                        EncodingSetup();

                        string words = Console.ReadLine()?.Trim() ?? string.Empty;

                        isEqual = string.Equals(
                            words,
                            correctText,
                            StringComparison.OrdinalIgnoreCase);

                        if (isEqual)
                            correctAnswer++;
                        else
                        {
                            Console.WriteLine($"Enter " + " Add Question in User Vocabluary");
                            user.AddMissQustions(e);
                            authService.SaveUsersAsync(new List<User> { user });
                            Console.WriteLine($"add miss answer {d}");
                            misstakeAnswer++;
                        }

                        Console.ForegroundColor = isEqual ? ConsoleColor.Green : ConsoleColor.Red;
                        Console.WriteLine(isEqual ? "CORRECT" : "MISSTAKE");

                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine(e.Ipa + " ");

                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"{words}");
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine(correctText);

                        Console.WriteLine();

                        Console.ResetColor();
                        Console.WriteLine("press any key to continue...");
                        Console.WriteLine();

                        Console.ReadKey();
                    }

                    count++;
                }
            }
        }

        private static async Task ShowHardQuestions(List<Examples> examples, bool isEnToRu, User user, AuthService authService)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"Welcome User {user.Name}");
            Console.ResetColor();
            
            int count = 1;
            int correctAnswer = 0;
            int misstakeAnswer = 0;
            
            Console.WriteLine(examples.Count);
            Console.ReadKey();
            
            foreach (var e in examples)
            {
                bool isEqual = false;

                while (!isEqual)
                {
                    Console.Clear();
                        
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine($" === current QA {count} / {examples.Count} ===");

                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"CORRECT {correctAnswer}");

                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine($"MISSTAKE {misstakeAnswer}");
                        
                    Console.WriteLine();

                    string correctText = isEnToRu ? e.Ru : e.En;
                    string questionText = isEnToRu ? e.En : e.Ru;

                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine(questionText);

                    Console.WriteLine("Enter Word:");
                    Console.WriteLine();

                    EncodingSetup();
                    string words = Console.ReadLine()?.Trim() ?? string.Empty;

                    isEqual = string.Equals(
                        words,
                        correctText,
                        StringComparison.OrdinalIgnoreCase);

                    if (isEqual)
                        correctAnswer++;
                    else
                    {
                        misstakeAnswer++;
                    }

                    Console.ForegroundColor = isEqual ? ConsoleColor.Green : ConsoleColor.Red;
                    Console.WriteLine(isEqual ? "CORRECT" : "MISSTAKE");

                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine(e.Ipa + " ");

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"{words}");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(correctText);

                    Console.WriteLine();

                    Console.ResetColor();
                    Console.WriteLine("press any key to continue...");
                    Console.WriteLine();

                    Console.ReadKey();
                }

                count++;
            }
        }
        
        // находит имя темы
        private static string ChooseDir(Dictionary<int, string> dict, int numbTheme)
        {
            bool isValidLevel;
            
            do
            {
                isValidLevel =  dict.ContainsKey(numbTheme);

                if (isValidLevel)
                {
                    Console.WriteLine("Success");
                    return dict[numbTheme];
                }
                else
                {
                    Console.WriteLine("Invalid Level");
                    continue;
                }
                
            } while (!isValidLevel);

            return String.Empty;
        }

        private static void EncodingSetup()
        {
            Console.OutputEncoding = Encoding.UTF8;   // для виводу
            
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                Console.InputEncoding  = Encoding.Unicode;
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                Console.InputEncoding  = Encoding.UTF8;
        }

        public static async Task<Data?> GetAsync(string filePath )
        {
            string json = await File.ReadAllTextAsync(filePath);
            
            try
            {
                return JsonSerializer.Deserialize<Data>(json) ?? new Data();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new Data();
            }
        }
    }
}

