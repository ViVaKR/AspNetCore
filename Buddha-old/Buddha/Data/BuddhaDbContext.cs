using Buddha.Models;
using Microsoft.EntityFrameworkCore;

namespace Buddha.Data;

public class BuddhaDbContext(DbContextOptions<BuddhaDbContext> options) : DbContext(options)
{
    public DbSet<Subscribe> Subscribes => Set<Subscribe>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Subscribe>()
            .HasKey(x => x.Id);
        
        modelBuilder.Entity<Subscribe>()
            .Property(x => x.Id)
            .ValueGeneratedOnAdd(); // ID 자동 증가 명시
    }
}
