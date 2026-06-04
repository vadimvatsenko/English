using System.Text.Json;
using ANSIConsole;
using English.Static;
using English.Utils;

namespace English;

public class AuthService
{
    private const string DirPath = "save data";
    private readonly string FilePath;
    public bool IsAutologin { get; private set; } = false;
    
    // Шлях до файлу сесії (для автологіну)
    private readonly string SessionFilePath;

    public AuthService()
    {
        FilePath = Path.Combine(DirPath, "save.json");
        SessionFilePath = Path.Combine(DirPath, "session.json");
    }
    
    public async Task InitializeAsync()
    {
        if (File.Exists(SessionFilePath)) IsAutologin = true;
        
        // Створюємо папку DirPath, якщо її ще немає
        Directory.CreateDirectory(DirPath);

        // Якщо файлу збереження ще немає — створимо його
        if (!File.Exists(FilePath))
        {
            // Записуємо "[]" — порожній JSON-масив (тобто порожній список користувачів)
            await File.WriteAllTextAsync(FilePath, "[]");
        }
    }
    
    public async Task<User?> TryAutoLoginAsync()
    {
        // Якщо файлу сесії немає — автоматичний вхід неможливий
        if (!File.Exists(SessionFilePath)) return null;
        
        try
        {
            IsAutologin = true;
            // Читаємо ID користувача, який зберігся з минулого разу
            string savedId = await File.ReadAllTextAsync(SessionFilePath);
            
            if (string.IsNullOrWhiteSpace(savedId)) return null;
            
            // Завантажуємо всіх користувачів і шукаємо того, чий ID збігається
            List<User> allUsers = await LoadUsersAsync();
            User? user = allUsers.FirstOrDefault(u => u.Id == savedId);
            
            if (user != null)
            {
                Console.WriteLine($"[Автологін] З поверненням, {user.Name}!");
                return user;
            }
        }
        catch
        {
            // Якщо файл сесії пошкоджений — ігноруємо його і просимо увійти вручную
            return null;
        }

        return null;
    }
    
    // --- ДОПОМІЖНИЙ МЕТОД: Збереження сесії ---
    public async Task SaveSessionAsync(string userId)
    {
        // Записуємо ID користувача у файл сесії
        // Для навчального проєкту цього достатньо. У реальних додатках тут зазвичай зберігають зашифрований токен.
        await File.WriteAllTextAsync(SessionFilePath, userId);
        IsAutologin = true;
    }

    // --- НОВИЙ МЕТОД: Вихід з акаунту (розлогін) ---
    public async Task LogoutAsync()
    {
        // Якщо файл сесії існує — видаляємо його, щоб скинути автологін
        if (File.Exists(SessionFilePath))
        {
            File.Delete(SessionFilePath);
        }
        IsAutologin = false;
    }
    
    public async Task<List<User>> LoadUsersAsync()
    {
        // Якщо файлу немає — повертаємо порожній список, щоб програма не падала
        if (!File.Exists(FilePath)) return new List<User>();
        
        // Читаємо весь текст JSON з файлу
        string json = await File.ReadAllTextAsync(FilePath);
        
        // Якщо текст порожній/з пробілами — теж повертаємо порожній список
        if (string.IsNullOrWhiteSpace(json)) return new List<User>();

        try
        {
            // Десеріалізуємо JSON у List<User>
            // Якщо Deserialize повернув null — повертаємо порожній список
            return JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
        }
        catch (Exception e)
        {
            // Якщо JSON зламався або формат неправильний — не падаємо, просто повертаємо порожній список
            // Console.WriteLine(e.Message); // можна включити для дебагу
            Console.ReadKey();
            return new List<User>();
        }
    }

    // Метод для збереження всіх користувачів у JSON-файл
    public async Task SaveUsersAsync(List<User> users)
    {
        // Перетворюємо список users у JSON-рядок
        // WriteIndented = true робить JSON “красивим” з відступами
        string json = JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true });

        // Записуємо JSON у файл (перезаписуємо повністю)
        await File.WriteAllTextAsync(FilePath, json);
    }

    // Метод для оновлення конкретного користувача у файлі (наприклад, після зміни HP/Level/Weapons)
    public async Task UpdateUsersAsync(User users)
    {
        // Завантажуємо всіх користувачів з файлу
        List<User> allUser = await LoadUsersAsync();

        // Шукаємо індекс користувача в списку за ім'ям (без врахування регістру)
        int index = allUser.FindIndex(u => u.Id.Equals(users.Id, StringComparison.OrdinalIgnoreCase));

        // Якщо користувач знайдений
        if (index != -1)
        {
            // Замінюємо старий запис користувача на оновлений
            allUser[index] = users;

            // Зберігаємо список назад у файл
            await SaveUsersAsync(allUser);
        }
    }

    // Реєстрація нового користувача
    public async Task<User?> RegisterUserAsync()
    {
        // Завантажуємо всіх користувачів, щоб додати нового
        List<User> allUser = await LoadUsersAsync();

        // Очищаємо екран
        Console.Clear();

        // Заголовок
        Console.WriteLine($"-- Register User --");

        // Запитуємо ім'я

        string name = " ";
        bool isName = false;
        do
        {
            Console.Write("Enter your Name: ".Color(StaticColors.White).Background(StaticColors.Blue));
            name = Console.ReadLine().Trim();
            isName = allUser.Any(u => u.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (isName)
            {
                Console.WriteLine($"{name} is already registered.".Color(StaticColors.White).Background(StaticColors.Red));
            }
            
        } while (isName);
        

        // Запитуємо пароль
        Console.Write("Enter your Password: ");
        string password = UserValidation.ReadPassword();

        // Перевірка: якщо ім'я або пароль порожні — реєстрацію не робимо
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(password))
        {
            Console.WriteLine("Enter valid Name and Password");
            Console.ReadKey(); // чекаємо натискання клавіші, щоб учень встиг прочитати
            return null;        // повертаємо null — означає “неуспішно”
        }

        // Створюємо нового користувача
        User user = new User
        {
            Id = IDGenerator.GenerateStringID(10),
            Name = name,               // записуємо ім'я
            Password = password,       // записуємо пароль
        };

        // Додаємо користувача у список
        allUser.Add(user);

        // Зберігаємо список у файл
        await SaveUsersAsync(allUser);

        // Повідомлення про успіх
        Console.WriteLine($"User {user.Name} created.");

        // Повертаємо створеного користувача
        return user;
    }

    // Логін користувача
    public async Task<User?> LoginUserAsync()
    {
        // Завантажуємо всіх користувачів
        List<User> allUser = await LoadUsersAsync();
        
        // Очищаємо екран
        Console.Clear();

        // Заголовок
        Console.BackgroundColor = ConsoleColor.DarkMagenta;
        Console.WriteLine(" --- Login ---");
        Console.ResetColor();
        Console.WriteLine();
        
        // Вводимо ім'я
        Console.Write("Enter your Name: ");
        string name = Console.ReadLine().Trim();

        // Вводимо пароль
        Console.Write("Enter your Password: ");
        string password = UserValidation.ReadPassword();

        // Шукаємо користувача по імені (перший, який підходить)
        User user = allUser.FirstOrDefault(u => u.Name.Equals(name));
        
        Console.WriteLine($"{user.Name}: {user.Password}");
        Console.ReadKey();

        // Якщо користувача не знайдено або пароль не співпав — логін невдалий
        if (user == null || user.Password != password)
        {
            Console.WriteLine("Wrong password or name");
            Console.ReadKey();
            return null;
        }

        // Якщо все добре — повідомляємо про успіх
        Console.BackgroundColor = ConsoleColor.DarkGreen;
        Console.WriteLine($"User {user.Name} successfully logged in.");
        Console.ResetColor();
        Console.ReadKey();

        // Повертаємо знайденого користувача
        return user;
    }
}