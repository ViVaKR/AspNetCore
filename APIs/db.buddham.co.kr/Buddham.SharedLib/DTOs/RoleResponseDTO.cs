namespace Buddham.SharedLib.DTOs;

public class RoleResponseDTO
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public int TotalUsers { get; set; }
    public string? NormalizedName { get; set; }
    public string? ConcurrencyStamp { get; set; }
}
