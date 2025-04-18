namespace ViVaKR.API.DTOs;

public class UnSubscribeTokenDTO
{
    public string Email { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public DateTime? ExpiresAt { get; set; }
}
