using System.Text.Json.Serialization;

namespace English;

[Serializable]
public class Data
{
    [JsonPropertyName("vocabulary")]
    public List<Vocabulary> Vocabulary { get; set; }
        
    [JsonPropertyName("sections")]
    public List<Sections> Sections { get; set; }
}