using Spectre.Console;

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
        [1] = "Personal Cabinet",
        [2] = "<== Exit"
    };

    // меню личного кабинета - смена имени/пароля, словарь трудных выражений, автологин и логаут
    public static readonly Dictionary<int, string> PersonalCabinetMenu = new Dictionary<int, string>()
    {
        [0] = "Изменить имя",
        [1] = "Изменить пароль",
        [2] = "Словарь трудных выражений",
        [3] = "AutoLogin [ ]",
        [4] = "< LogOut",
        [5] = "<== Назад",
    };

    // меню выбора, что делать
    public static readonly Dictionary<int, string> EnglishMenu = new Dictionary<int, string>()
    {
        [0] = "From Russian to English",
        [1] = "From English to Russian",
        [2] = "<== BACK TO LEVEL MENU",
    };

    public static readonly Dictionary<int, string> YesOrNoMenu = new Dictionary<int, string>()
    {
        [0] = "YES",
        [1] = "NO",
    };

    public static void ChangeAutoLoginEnabled(bool enabled)
    {
        PersonalCabinetMenu[3] = enabled ? "AutoLogin [X]" : "AutoLogin [ ]";
    }

}