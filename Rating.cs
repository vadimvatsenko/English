namespace English;

public class Rating
{
    public string NameTheme { get; private set; }
    public int Tries { get; private set; }
    public string RatingTheme { get; private set; }
    
    public Rating(string name, string theme)
    {
        NameTheme = name;
        RatingTheme = theme;
        Tries = 0;
    }

    public void SetNameTheme(string name) => NameTheme = name;
    public void SetRatingTheme(string theme) => RatingTheme = theme;
    public void AddTries() => Tries++;
    
}