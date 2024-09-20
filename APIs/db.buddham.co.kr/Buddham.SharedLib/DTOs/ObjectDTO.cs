using System.Text.Json.Serialization;

namespace Buddham.SharedLib.DTOs;

public class ObjectDTO
{
    [JsonPropertyName("obj")]
    public object? Obj { get; set; }
}
