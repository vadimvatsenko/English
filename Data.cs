using System.Text.Json.Serialization;

namespace English;

[Serializable]
public class Data
{
    [JsonPropertyName("sections")]
    public List<Sections> Sections { get; set; }//
}