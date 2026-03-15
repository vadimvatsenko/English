using System.Text.Json.Serialization;

namespace English;

[Serializable]
public class Vocabulary
{
    [JsonPropertyName("en")]
    public string En { get; set; }
    [JsonPropertyName("ipa")]
    public string Ipa { get; set; }
    [JsonPropertyName("ru")]
    public string Ru { get; set; }
}