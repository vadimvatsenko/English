namespace English;

public class StaticFields
{
    public static readonly string PathAllFiles = Path.GetFullPath(AppContext.BaseDirectory);
        
    public static readonly Dictionary<int, string> MainMenu = new Dictionary<int, string>()
    {
        [0] = "Exit",      // 0 — вихід
        [1] = "Register",  // 1 — реєстрація
        [2] = "Login"      // 2 — логін
    };
        
    public static readonly Dictionary<int, string> UserMenu = new Dictionary<int, string>()
    {
        [0] = "Exit",
        [1] = "Start Practice",
        [2] = "Repeat Hard Questions",
    };
        
    // меню выбора, что делать
    public static readonly Dictionary<int, string> EnglishMenu = new Dictionary<int, string>()
    {
        [1] = "Vocabulary Russian to English",
        [2] = "Vocabulary English to Russian",
        [3] = "From Russian to English",
        [4] = "From English to Russian",
    };
}