using Microsoft.AspNetCore.Identity;

namespace Buddham.API.Data;

public class ApplicationUser : IdentityUser // IdentityUser is a class that comes from the Identity framework
{
    public string? Name { get; set; }

}
