using System.Text.Json.Serialization;

namespace English;

[Serializable]
public class User
{
    [JsonPropertyName("name")]
    public string Name {get; set;}
    [JsonPropertyName("password")]
    public string Password {get; set;}

    //public List<Examples> Vocabulary { get; private set; } = new List<Examples>();
    public List<Examples> MissQuestion { get; private set; } = new List<Examples>();
    
    public void AddMissQustions(Examples v)
    {
        MissQuestion.Add(v);
    }
}