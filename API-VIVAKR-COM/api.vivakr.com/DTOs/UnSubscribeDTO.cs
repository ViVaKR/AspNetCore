namespace ViVaKR.API.DTOs;

public class UnSubscribeDTO
{
    public string Email { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public int SubscriberId { get; set; }
}
