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
   
    [JsonPropertyName("hardQuestion")]
    public List<Examples> HardQuestion { get; set; } = new List<Examples>();

    [JsonPropertyName("ratingText")] 
    public List<Rating> RatingText { get; set; } =  new List<Rating>();
    
}