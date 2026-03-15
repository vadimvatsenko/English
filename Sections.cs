using System.Text.Json.Serialization;

namespace English;

[Serializable]
public class Sections
{
    [JsonPropertyName("title")]
    public string Title { get; set; }
    [JsonPropertyName("rule")]
    public string Rule {get; set;}
    [JsonPropertyName("examples")]
    public Examples[] Examples {get; set;}
}