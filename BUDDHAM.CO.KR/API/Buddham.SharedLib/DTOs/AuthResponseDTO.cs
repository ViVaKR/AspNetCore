namespace Buddham.SharedLib.DTOs;

public class AuthResponseDTO
{
    public string? Token { get; set; }
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
    public string? RefreshToken { get; set; } = string.Empty;

}
