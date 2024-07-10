using Buddham.API.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Buddham.API.Data;

public class DataContext(DbContextOptions<DataContext> options) : IdentityDbContext(options)
{
    public DbSet<Sutras> Sutras { get; set; }
}


/*

--> (1) dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore --version 9.0.0-preview.5.24306.11

--> (2) dotnet ef migrations add InitialCreate --output-dir Data/Migrations
 */
