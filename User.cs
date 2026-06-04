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
   
    [JsonPropertyName("HardQuestion")]
    public List<Examples> HardQuestion { get; set; }

    [JsonPropertyName("RatingText")] 
    public List<Rating> RatingText { get; set; }
    
    
    
    public void AddMissQustions(Examples v)
    {
        if (HardQuestion == null)
        {
            HardQuestion = new List<Examples>();
        }
        if (!HardQuestion.Contains(v))
            if (HardQuestion != null)
                HardQuestion.Add(v);
    }
}