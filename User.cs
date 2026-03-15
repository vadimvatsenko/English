using System.Text.Json.Serialization;

namespace English;

[Serializable]
public class User
{
    [JsonPropertyName("name")]
    public string Name {get; private set;}
    [JsonPropertyName("name")]
    public string Password {get; private set;}

    public User(string name, string password)
    {
        Name = name;
        Password = password;
    }
}