using System.Text.Json.Serialization;

namespace English;

// личный словарь трудных выражений пользователя, сгруппированный так же,
// как и сами материалы для практики: уровень -> тема -> список выражений
[Serializable]
public class HardLevel
{
    [JsonPropertyName("levelName")]
    public string LevelName { get; set; }

    [JsonPropertyName("themes")]
    public List<HardTheme> Themes { get; set; } = new List<HardTheme>();
}

[Serializable]
public class HardTheme
{
    [JsonPropertyName("themeName")]
    public string ThemeName { get; set; }

    [JsonPropertyName("expressions")]
    public List<Examples> Expressions { get; set; } = new List<Examples>();
}
