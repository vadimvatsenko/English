namespace English;

public class StaticFields
{
    public static readonly string PathAllFiles = Path.GetFullPath(AppContext.BaseDirectory);
        
    public static readonly Dictionary<int, string> MainMenu = new Dictionary<int, string>()
    {
        [0] = "Login",     // 0 — логін
        [1] = "Register",  // 1 — реєстрація
        [2] = "Exit",      // 2 — вихід
    };
        
    public static readonly Dictionary<int, string> UserMenu = new Dictionary<int, string>()
    {
        [0] = "Start Practice",
        [1] = "Repeat Hard Questions",
        [2] = "< BACK TO MAIN MENU",
        [3] = "AutoLogin",
        [4] = "LogOut"
    };
        
    // меню выбора, что делать
    public static readonly Dictionary<int, string> EnglishMenu = new Dictionary<int, string>()
    {
        [0] = "From Russian to English",
        [1] = "From English to Russian",
        [2] = "< BACK TO LEVEL MENU",
    };
    
    public static readonly Dictionary<int, string> YesOrNoMenu = new Dictionary<int, string>()
    {
        [0] = "YES",
        [1] = "NO",
    };
}