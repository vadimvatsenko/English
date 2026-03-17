using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace English
{
    public class Program
    {
        
        // все папки в корневом каталоге
        /*private static readonly string PathAllFiles = Path.GetFullPath(
            Path.Combine(AppContext.BaseDirectory, $"..\\..\\..\\")
        );*/

        private static readonly string PathAllFiles = Path.GetFullPath(AppContext.BaseDirectory);
        
        // менб выбора, что делать
        private static Dictionary<int, string> englishMenu = new Dictionary<int, string>()
        {
            [0] = "Vocabulary Russian to English",
            [1] = "Vocabulary English to Russian",
            [2] = "From Russian to English",
            [3] = "From English to Russian",
        };
        
        public static async Task Main(string[] args)
        {
            EncodingSetup();
            
            string[] folders = Directory.GetDirectories(PathAllFiles);
            
            int folderCount = 0;

            Dictionary<int, string> levelsDict = new Dictionary<int, string>();
            
            foreach (string folder in folders)
            {
                if (folder.Contains("A") || folder.Contains("B") || folder.Contains("C") ||  folder.Contains("Numbers"))
                {
                    // получим последнюю папку
                    string floderName = Path.GetFileName(folder);
                    levelsDict.Add(folderCount, floderName);
                    
                    folderCount++;
                }
            }
            
            foreach (KeyValuePair<int, string> level in levelsDict)
            {
                Console.WriteLine($"[{level.Key}]: [{level.Value}]");
            }

            bool isValidLevel = false;
            
            string dirPath = ChooseDir(levelsDict, "Enter Level");
            Console.WriteLine($"Current Level: {dirPath}");
            
            // логика выбора урока в теме
            // 1. получаем все уроки в папке
            string[] filesOnTheme = Directory.GetFiles(PathAllFiles + dirPath);
            Dictionary<int, string> filesOnThemeDict = new Dictionary<int, string>();
            
            int coutFiles = 0;
            foreach (string file in filesOnTheme)
            {
                string name = Path.GetFileNameWithoutExtension(file);
                Console.WriteLine($"[{coutFiles}] : [{name}]");
                filesOnThemeDict.Add(coutFiles, name);
                coutFiles++;
            }
            
            string fileName = ChooseDir(filesOnThemeDict, "Enter Lesson: ");

            Console.WriteLine(fileName);
            
            // полный путь к теме
            
            string FilePath = String.Empty;
            
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                FilePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, $"{dirPath}\\{fileName}.json"));
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                FilePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, $"{dirPath}/{fileName}.json"));
            
            bool isFileExist = File.Exists(FilePath);
            Data dataList = await GetAsync(FilePath);

            Console.WriteLine("Enter your option: ");
            
            foreach (var m in englishMenu)
            {
                Console.WriteLine($"[{m.Key}]: [{m.Value}]");
            }
            
            string option = Console.ReadLine();

            switch (option)
            {
                case "0":
                    Vocabulary(dataList, false);
                    break;

                case "1":
                    Vocabulary(dataList, true);
                    break;
                case "2":
                    Extensions(dataList, false);
                    break;
                case "3":
                    Extensions(dataList, true);
                    break;
            }

            Console.ReadKey();
        }



        private static void Extensions(Data? dataList, bool isEnToRu)
        {
            if (dataList == null || dataList.Sections == null)
                return;

            int count = 1;
            int correctAnswer = 0;
            int misstakeAnswer = 0;
            int allQaCount = 0;

            var allQaList = dataList.Sections;

            foreach (var d in allQaList)
            {
                allQaCount += d.Examples.Length;
            }

            foreach (var d in dataList.Sections)
            {
                foreach (var e in d.Examples)
                {
                    bool isEqual = false;

                    while (!isEqual)
                    {
                        Console.Clear();

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
                        
                        Console.WriteLine($"[{words}]");

                        isEqual = string.Equals(
                            words,
                            correctText,
                            StringComparison.OrdinalIgnoreCase);

                        if (isEqual)
                            correctAnswer++;
                        else
                            misstakeAnswer++;

                        Console.ForegroundColor = isEqual ? ConsoleColor.Green : ConsoleColor.Red;
                        Console.WriteLine(isEqual ? "CORRECT" : "MISSTAKE");

                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine(e.Ipa + " ");

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

        private static void Vocabulary(Data? dataList, bool showEnglishWord)
        {
            if (dataList?.Vocabulary == null || dataList.Vocabulary.Count == 0)
                return;

            foreach (var d in dataList.Vocabulary)
            {
                bool isEqual = false;

                while (!isEqual)
                {
                    Console.Clear();

                    string question = showEnglishWord ? d.En : d.Ru;
                    string correctAnswer = showEnglishWord ? d.Ru : d.En;

                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine(question);

                    Console.Write("Enter word: ");
                    string userAnswer = Console.ReadLine()?.Trim() ?? "";

                    isEqual = string.Equals(userAnswer, correctAnswer, StringComparison.OrdinalIgnoreCase);

                    Console.ForegroundColor = isEqual ? ConsoleColor.Green : ConsoleColor.Red;
                    Console.WriteLine(isEqual ? "CORRECT" : "MISTAKE");
                    
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine(d.Ipa);

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(showEnglishWord ?  d.Ru : d.En);

                    Console.WriteLine();
                    Console.ResetColor();

                    Console.WriteLine("Press any key to continue...");
                    Console.WriteLine();
                    Console.ReadKey();
                }
            }
        }

        private static string ChooseDir(Dictionary<int, string> dict, string message)
        {
            bool isValidLevel;
            
            do
            {
                Console.Write($"{message}: ");
            
                string inputLevel = Console.ReadLine();
            
                isValidLevel = int.TryParse(inputLevel, out int inputLevelInt) && dict.ContainsKey(inputLevelInt);

                if (isValidLevel)
                {
                    Console.WriteLine("Success");
                    return dict[inputLevelInt];
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

