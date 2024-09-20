namespace Buddham.SharedLib.DTOs;

public class UserDetailDTO
{
    public string? Id { get; set; }

    public string? FullName { get; set; }

    public string? Email { get; set; }

    public bool EmailConfirmed { get; set; }

    public string[]? Roles { get; set; }

    public string? PhoneNumber { get; set; }

    public bool TwoFactorEnabled { get; set; }

    public bool PhoneNumberConformed { get; set; }

    public int AccessFailedCount { get; set; }

}

/*
--> Id	FullName	RefreshToken	RefreshTokenExpiryTime	UserName	NormalizedUserName	Email	NormalizedEmail	EmailConfirmed	PasswordHash	SecurityStamp	ConcurrencyStamp	PhoneNumber	PhoneNumberConfirmed	TwoFactorEnabled	LockoutEnd	LockoutEnabled	AccessFailedCount

 */
