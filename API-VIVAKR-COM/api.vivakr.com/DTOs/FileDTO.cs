using System;
using System.Text.Json.Serialization;

namespace ViVaKR.API.DTOs;

public class FileDTO(string dbPath, string fullPath, long fileSize = 0)
{
    [JsonPropertyName("dbPath")]
    public string? DbPath { get; set; } = dbPath;

    [JsonPropertyName("fileName")]
    public string? FullPath { get; set; } = fullPath;

    [JsonPropertyName("fileSize")]
    public long FileSize { get; set; } = fileSize;
}
