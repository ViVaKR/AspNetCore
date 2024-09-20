using Microsoft.AspNetCore.Identity;

namespace Buddham.API.Data;

public class ApplicationUser : IdentityUser
{
    public string? Name { get; set; }
}
