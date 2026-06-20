using ANSIConsole;
using English.Utils;

namespace English
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.Title = "English";
            Console.SetWindowSize(135,35);
            
            EncodingUtil encodingUtil = new EncodingUtil();
            
            encodingUtil.EncodingSetup();

            // инициализация колорайзера
            if (!ANSIInitializer.Init(false)) ANSIInitializer.Enabled = false;

            AuthService authService = new AuthService();
            await authService.InitializeAsync();

            View view = new View(authService);
            
            StaticFields.ChangeAutoLoginEnabled(authService.IsAutologin);

            Logic logic = new Logic(authService, view);
            
            await logic.StartProgram();
        }
    }
}

