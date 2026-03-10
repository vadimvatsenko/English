using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace English
{
    public class Program
    {
        private static readonly string DirPath = "A0";
        
        private static readonly string[] levels = new []{"A0", "A1", "A2", "B0",};
        
        private static readonly string FilePath = Path.GetFullPath(
            Path.Combine(AppContext.BaseDirectory, $"..\\..\\..\\{DirPath}\\A0_3.json")
        );
        
        // все папки
        private static readonly string PathAllFiles = Path.GetFullPath(
            Path.Combine(AppContext.BaseDirectory, $"..\\..\\..\\")
        );
        
        private static Dictionary<int, string> englishMenu = new Dictionary<int, string>()
        {
            [0] = "Vocabulary",
            [1] = "Sections",
        };
        
        /*private static readonly string FilePath 
            = Path.Combine(AppContext.BaseDirectory, DirPath, "A0_1.json");*/
        
        public static async Task Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;   // для виводу
            Console.InputEncoding  = Encoding.UTF8;
            
            string[] folders = Directory.GetDirectories(PathAllFiles);

            foreach (string folder in folders)
            {
                if (folder.Contains("A"))
                {
                    // получим последнюю папку
                    string floderName = Path.GetFileName(folder);
                    Console.WriteLine($"{floderName}");
                }
            }
            
            bool isFileExist = File.Exists(FilePath);
            
            Data dataList = await GetAsync();

            foreach (var m in englishMenu)
            {
                Console.WriteLine($"[{m.Key}]: [{m.Value}]");
            }
            
            Console.WriteLine("Enter your option: ");
            string option = Console.ReadLine();

            switch (option)
            {
                case "0":
                    
                    foreach (var d in dataList.Vocabulary)
                    {
                        bool isEqual = false;

                        while (!isEqual)
                        {
                            Console.Clear();

                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.WriteLine(d.Ru);
                            
                            Console.Write("Enter Word: ");
                            string words = Console.ReadLine().Trim();
                            
                            for (int i = 0; i < words.Length; i++)
                            {
                                if (d.En.Length != words.Length)
                                {
                                    isEqual = false;
                                    break;
                                }

                                if (words[i] != d.En[i])
                                {
                                    isEqual = false;
                                    break;
                                }

                                isEqual = true;
                            }

                            Console.ForegroundColor = isEqual ? ConsoleColor.Green : ConsoleColor.Red;
                            Console.WriteLine(isEqual ? "CORRECT" : "MISSTAKE");

                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            Console.WriteLine(d.Ipa);

                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine(d.En);

                            Console.WriteLine();
                            Console.ResetColor();

                            Console.WriteLine("press any key to continue...");
                            Console.WriteLine();
                            Console.ReadKey();
                        }
                    }
                    break;
                
                case "1":

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
                            
                            bool isEqual = false;

                            while (!isEqual)
                            {
                                Console.WriteLine();
                                Console.ForegroundColor = ConsoleColor.Blue;
                                Console.WriteLine(e.Ru);

                                Console.WriteLine("Enter Word: ");
                                
                                Console.WriteLine();
                                string words = Console.ReadLine();

                                for (int i = 0; i < words.Length; i++)
                                {
                                    if (e.En.Length != words.Length)
                                    {
                                        isEqual = false;
                                        break;
                                    }

                                    if (char.ToLower(words[i]) != char.ToLower(e.En[i]))
                                    {
                                        isEqual = false;
                                        break;
                                    }

                                    isEqual = true;
                                }

                                Console.ForegroundColor = isEqual ? ConsoleColor.Green : ConsoleColor.Red;
                                Console.WriteLine(isEqual ? "CORRECT" : "MISSTAKE");

                                Console.ReadKey();

                                Console.ForegroundColor = ConsoleColor.DarkRed;
                                Console.WriteLine(e.Ipa + " ");

                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine(e.En);

                                Console.WriteLine();

                                Console.ResetColor();

                                Console.WriteLine("press any key to continue...");
                                Console.WriteLine();

                                Console.ReadKey();
                            }
                            
                            count++;
                        }
                    }

                    break;
            }

            Console.ReadKey();
        }

        public static async Task<Data?> GetAsync()
        {

            string json = await File.ReadAllTextAsync(FilePath);
            
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

    [Serializable]
    public class User
    {
        [JsonPropertyName("name")]
        public string Name {get; private set;}
        [JsonPropertyName("name")]
        public string Password {get; private set;}

        public User(string name, string password)
        {
            Name = name;
            Password = password;
        }
    }

    [Serializable]
    public class Data
    {
        [JsonPropertyName("vocabulary")]
        public List<Vocabulary> Vocabulary { get; set; }
        
        [JsonPropertyName("sections")]
        public List<Sections> Sections { get; set; }
    }
    

    [Serializable]
    public class Vocabulary
    {
        [JsonPropertyName("en")]
        public string En { get; set; }
        [JsonPropertyName("ipa")]
        public string Ipa { get; set; }
        [JsonPropertyName("ru")]
        public string Ru { get; set; }
    }

    [Serializable]
    public class Sections
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }
        [JsonPropertyName("rule")]
        public string Rule {get; set;}
        [JsonPropertyName("examples")]
        public Examples[] Examples {get; set;}
    }

    [Serializable]
    public class Examples
    {
        [JsonPropertyName("en")]
        public string En { get; set; }
        [JsonPropertyName("ipa")]
        public string Ipa { get; set; }
        [JsonPropertyName("ru")]
        public string Ru { get; set; }
    }
}

