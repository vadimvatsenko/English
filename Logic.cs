using System.Runtime.InteropServices;
using System.Text.Json;
using ANSIConsole;
using English.Static;

namespace English;

public class Logic
{
    private User? _user = null;
    private readonly AuthService _authService;
    private readonly View _view;

    public Logic(AuthService authService, View view)
    {
        _authService = authService;
        _view = view;
    }
    
    public async Task StartProgram()
    {
        _user = await _authService.TryAutoLoginAsync();

        if (_user == null)
        {
            _user = await StartMenu();
        }

        if (_user != null)
        {
            // Словарь с названиями уровней, цифра номер string название json
            Dictionary<int, string> levelsDict = FillLevelDictionaryNames();
            await UserChoiceMenu(levelsDict);
        }
        else
        {
            await Exit();
        }
    }

    private async Task Exit(float delay = 100)
    {
        Console.Clear();
        string points = "..........";
        Console.Write($"Exit".Color(StaticColors.White).Background(StaticColors.Red));
        foreach (var p in points)
        {
            Console.Write(p.ToString().Color(StaticColors.White).Background(StaticColors.Red));
            await Task.Delay(100);
        }

        Environment.Exit(0);
    }

    // стартовое меню с регистрацией и логинизацией
    private async Task<User?> StartMenu()
    {
        Console.Clear();

        string messageHeader = "Enter you option";
        int inputInt = _view.ColorizeMenuInput(StaticFields.MainMenu, messageHeader);

        switch (inputInt)
        {
            case 0:
                _user = await _authService.LoginUserAsync();
                break;
            case 1:
                _user = await _authService.RegisterUserAsync();
                break;
            case 2:
                await Exit();
                break;
            default:
                Console.WriteLine($"[{inputInt}]: Invalid option.");
                break;
        }

        return _user;
    }

    // заполняем словарь с уровнями, просто название и номер
    private Dictionary<int, string> FillLevelDictionaryNames()
    {
        Dictionary<int, string> levelsDict = new Dictionary<int, string>();
        int folderCount = 0;
        // тут берем все папки из корневой директории
        string[] folders = Directory.GetDirectories(StaticFields.PathAllFiles);

        foreach (string folder in folders)
        {
            if ((folder.Contains("A") || folder.Contains("B") || folder.Contains("C") ||
                 folder.Contains("Numbers")) && !folder.Contains("save data"))
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
    private async Task UserChoiceMenu(Dictionary<int, string> levelsDict)
    {
        Console.Clear();

        string messageHeader = $"{_user.Name} please enter option";
        int userMenuInt = _view.ColorizeMenuInput(StaticFields.UserMenu, messageHeader);

        switch (userMenuInt)
        {
            case 0:
                await StartPractice(levelsDict);
                break;
            case 1:
                // автологин
                StaticFields.ChangeAutoLoginEnabled(true);
                await _authService.SaveSessionAsync(_user.Id);
                await UserChoiceMenu(levelsDict);
                break;
            case 2:
                StaticFields.ChangeAutoLoginEnabled(false);
                await _authService.LogoutAsync();
                await StartProgram();
                break;
            case 3:
                await Exit();
                break;
            default:
                Console.WriteLine("Wrong menu option!");
                break;
        }
    }

    // практика, тут нужно выбрать уровень
    private async Task StartPractice(Dictionary<int, string> levelsDict)
    {
        Console.Clear();
        
        string levelName = await EnterLevel(levelsDict);
        Dictionary<int, string> filesOnThemeDict = FillLevelTopics(levelName);
        Console.Clear();
        
        int themeNumber = _view.ColorizeMenuInput(filesOnThemeDict, _user, "ENTER THEME NUMBER: ");
        
        // чтобы вернутся на один пункт назад
        if (filesOnThemeDict.Keys.Max() == themeNumber)
        {
            await StartPractice(levelsDict);
            return;
        }

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
        int option = _view.ColorizeMenuInput(StaticFields.EnglishMenu, message);

        switch (option)
        {
            case 0:
                Extensions(dataList, levelsDict, false, fileName);
                break;
            case 1:
                Extensions(dataList, levelsDict, true, fileName);
                break;
            case 2:
                break;
        }

        Console.ReadKey();
    }
    
    // выбор уровня
    private async Task<string> EnterLevel(Dictionary<int, string> levelsDict)
    {
        bool isContainceBackMenu = levelsDict.ContainsValue(StaticFields.BACK_TO_USER_OPTION);
        
        if (!isContainceBackMenu)
        {
            int lastKey = levelsDict.Keys.Max();
            levelsDict.Add(lastKey + 1, StaticFields.BACK_TO_USER_OPTION);
        }
        
        int numberLevel = _view.ColorizeMenuInput(levelsDict, $"{_user.Name}  ENTER LEVEL:");

        if (levelsDict.Keys.Max()  == numberLevel)
        {
            await UserChoiceMenu(levelsDict);
            return null;
        }
        
        string levelName = levelsDict[numberLevel];
        bool isValidLevel = false;

        Console.WriteLine($"Current Level: {levelsDict[numberLevel]}");
        return levelName;
    }
    
    // получаем словарь с темами по уровню
    private Dictionary<int, string> FillLevelTopics(string levelName)
    {
        string[] filesOnTheme = Directory.GetFiles(StaticFields.PathAllFiles + levelName);
        Dictionary<int, string> filesOnThemeDict = new Dictionary<int, string>();

        int numberTheme = 0;

        Console.WriteLine();
        
        // запоняем словарь данными
        foreach (string file in filesOnTheme)
        {
            string nameTheme = Path.GetFileNameWithoutExtension(file);

            filesOnThemeDict.Add(numberTheme, nameTheme);
            
            numberTheme++;
        }
        
        bool isContainceBackToChooseLevel 
            = filesOnThemeDict.ContainsValue(StaticFields.BACK_TO_CHOOSE_LEVEL);
        
        if (!isContainceBackToChooseLevel)
        {
            int lastKey = filesOnThemeDict.Keys.Max();
            filesOnThemeDict.Add(lastKey + 1, StaticFields.BACK_TO_CHOOSE_LEVEL);
        }
        
        return filesOnThemeDict;
    }

    private async void Extensions(Data? dataList, Dictionary<int, string> levelDict, bool isEnToRu,
        string fileName)
    {
        if (dataList == null || dataList.Sections == null)
        {
            Console.WriteLine($"The Questions Is Over");
            await StartPractice(levelDict);
            return;
        }
        
        var allQaList = dataList.Sections;
        
        await _view.QuestionsLogic(isEnToRu, fileName, allQaList, levelDict, _user);
    }

    
    // находит имя темы
    private static string ChooseDir(Dictionary<int, string> dict, int numbTheme)
    {
        bool isValidLevel;

        do
        {
            isValidLevel = dict.ContainsKey(numbTheme);

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


    //
    public static async Task<Data?> GetAsync(string filePath)
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
