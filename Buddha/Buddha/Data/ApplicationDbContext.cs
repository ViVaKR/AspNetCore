using Buddha.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Buddha.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Demo> Demos => Set<Demo>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Demo>().HasKey(x => x.Id);

        builder.Entity<Demo>().Property(x => x.Id).ValueGeneratedOnAdd();

        // Seed Demo
        builder.Entity<Demo>().HasData(
            new Demo { Id = 1, Name = "김범준" },
            new Demo { Id = 2, Name = "장길산" },
            new Demo { Id = 3, Name = "이순신" }
        );
    }
}
