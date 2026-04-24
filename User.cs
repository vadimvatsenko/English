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
    [JsonPropertyName("HardQuestion")]
    public List<Examples> HardQuestion { get; set; }
    
    public void AddMissQustions(Examples v)
    {
        if (!HardQuestion.Contains(v))
            if (HardQuestion != null)
                HardQuestion.Add(v);
    }
}