using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ViVaKR.API.Models;

namespace ViVaKR.API.Data;

public class VivaKRDbContext(DbContextOptions<VivaKRDbContext> options) : IdentityDbContext<AppUser>(options)
{
    public DbSet<Category> Categories => Set<Category>();

    public DbSet<Code> Codes => Set<Code>();

    public DbSet<FileManager> FileManagers => Set<FileManager>();

    public DbSet<TodayCode> TodayCodes => Set<TodayCode>();

    public DbSet<CodeComment> CodeComments => Set<CodeComment>();

    public DbSet<UserInfo> UserInfos => Set<UserInfo>();

    public DbSet<QnA> QnAs => Set<QnA>();

    public DbSet<BackupManager> BackupManagers => Set<BackupManager>();

    public DbSet<Subscribe> Subscribes => Set<Subscribe>();
    public DbSet<UnSubscribeToken> UnSubscribeTokens => Set<UnSubscribeToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Fluent API
        modelBuilder.Entity<Subscribe>()
            .HasKey(s => s.Id);
        modelBuilder.Entity<Subscribe>()
            .Property(s => s.Id)
            .ValueGeneratedOnAdd(); // ID 자동 증가 명시
        modelBuilder.Entity<Subscribe>()
            .Property(s => s.Email)
            .IsRequired()
            .HasMaxLength(100);

        modelBuilder.Entity<UnSubscribeToken>()
            .HasKey(s => s.Id);
        modelBuilder.Entity<UnSubscribeToken>()
            .Property(s => s.Id)
            .ValueGeneratedOnAdd(); // ID 자동 증가 명시
        modelBuilder.Entity<UnSubscribeToken>()
            .Property(s => s.Email)
            .IsRequired()
            .HasMaxLength(100);

        // Seed Roles (Admin, User, Guest)
        modelBuilder.Entity<IdentityRole>().HasData(
            new IdentityRole
            {
                Id = "admin-001",
                Name = "Admin",
                NormalizedName = "ADMIN",
                ConcurrencyStamp = "a1b2c3d4-e5f6-7890-abcd-1234567890ab"
            },
            new IdentityRole
            {
                Id = "user-001",
                Name = "User",
                NormalizedName = "USER",
                ConcurrencyStamp = "b2c3d4e5-f6a7-8901-bcde-2345678901bc"
            },
            new IdentityRole
            {
                Id = "writer-001",
                Name = "Writer",
                NormalizedName = "WRITER",
                ConcurrencyStamp = "265008DF-C88D-4C60-A177-B24A8E8B1FD6"
            },
            new IdentityRole
            {
                Id = "guest-001",
                Name = "Guest",
                NormalizedName = "GUEST",
                ConcurrencyStamp = "c3d4e5f6-a7b8-9012-cdef-3456789012cd"
            }
        );
    }
}