using GameStore.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Data;

public class GameStoreContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Genre> Genres => Set<Genre>(); //{ get; set; }

    public DbSet<Game> Games => Set<Game>(); // { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Seed
        modelBuilder.Entity<Genre>().HasData(
            new { Id = 1, Name = "Fighting" },
            new { Id = 2, Name = "Roleplaying" },
            new { Id = 3, Name = "Sports" },
            new { Id = 4, Name = "Racing" },
            new { Id = 5, Name = "Kids and Family" }
        );
    }
}
