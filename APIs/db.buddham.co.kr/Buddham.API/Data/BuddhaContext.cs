using Buddham.API.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Buddham.API.Data;

public class BuddhaContext(DbContextOptions<BuddhaContext> options) : IdentityDbContext<AppUser>(options)
{
    public DbSet<Sutra> Sutras { get; set; }
    public DbSet<SutraCommet> SutraCommets { get; set; }
    public DbSet<PlayGround> PlayGrounds { get; set; }
    public DbSet<GuestComment> GuestComments { get; set; }
    public DbSet<TodaySutra> TodaySutras { get; set; }
    public DbSet<SutraImage> SutraImages { get; set; }
}
