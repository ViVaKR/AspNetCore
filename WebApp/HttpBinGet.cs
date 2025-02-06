using System.Text.Json.Serialization;

namespace WebApp;

public class HttpBinGet
{
    [JsonPropertyName("authenticated")]
    public bool Authenticated { get; set; }

    [JsonPropertyName("token")]
    public string Token { get; set; } = string.Empty;
}
