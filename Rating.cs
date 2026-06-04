using System.Text.Json.Serialization;

namespace English;

[Serializable]
public class Rating
{
    public string NameTheme { get; private set; }
    public int Tries { get; private set; }
    public int CorrectUnswers {get; private set; }
    public int MissingUnswers {get; private set; }
    public int AllUnswers {get; private set; }

    [JsonConstructor]
    public Rating(string nameTheme)
    {
        NameTheme = nameTheme;
        Tries = 0;
        CorrectUnswers = 0;
        AllUnswers = 0;
    }

    public void SetNameTheme(string name) => NameTheme = name;
    public void AddTries() => Tries++;
    public void AddCorrectUnswers() => CorrectUnswers++;
    public void SetAllUnswers(int all) => AllUnswers = all;
    public void AddMissingUnswers() => MissingUnswers++;
    
}