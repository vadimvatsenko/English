using System.Text.Json.Serialization;

namespace English;

[Serializable]
public class User
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    
    [JsonPropertyName("name")]
    public string Name {get; set;}
    
    [JsonPropertyName("password")]
    public string Password {get; set;}
   
    // личный словарь трудных выражений: уровень -> тема -> выражения
    [JsonPropertyName("hardDictionary")]
    public List<HardLevel> HardDictionary { get; set; } = new List<HardLevel>();

    [JsonPropertyName("ratingText")] 
    public List<Rating> RatingText { get; set; } =  new List<Rating>();
    
}