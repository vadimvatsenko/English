using ANSIConsole;
using English.Utils;

namespace English
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.Title = "English";
            // размер окна и буфера можно менять только в Windows
            if (OperatingSystem.IsWindows())
            {
                Console.SetWindowSize(140,40);
                Console.BufferHeight = Console.WindowHeight;
                Console.BufferWidth = Console.WindowWidth;
            }
            
            
            EncodingUtil encodingUtil = new EncodingUtil();
            
            encodingUtil.EncodingSetup();

            // инициализация колорайзера (в Linux/macOS терминалы понимают ANSI без неё)
            if (OperatingSystem.IsWindows() && !ANSIInitializer.Init(false)) ANSIInitializer.Enabled = false;

            AuthService authService = new AuthService();
            await authService.InitializeAsync();

            View view = new View(authService);
            
            StaticFields.ChangeAutoLoginEnabled(authService.IsAutologin);

            Logic logic = new Logic(authService, view);
            
            await logic.StartProgram();
        }
    }
}

