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
    public Rating(string nameTheme, int tries, int correctUnswers, int allUnswers)
    {
        NameTheme = nameTheme;
        Tries = tries;
        CorrectUnswers = correctUnswers;
        AllUnswers = allUnswers;
    }

    public void SetNameTheme(string name) => NameTheme = name;
    public void AddTries() => Tries++;
    public void AddCorrectUnswers() => CorrectUnswers++;
    public void SetAllUnswers(int all) => AllUnswers = all;
    public void AddMissingUnswers() => MissingUnswers++;
    
    public void RatingClear()
    {
        MissingUnswers = 0;
        CorrectUnswers = 0;
    }
    
}