using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ViVaBM.API.Models;

namespace ViVaBM.API.Data;

public class VivabmDbContext(DbContextOptions<VivabmDbContext> options) : IdentityDbContext(options)
{
    public DbSet<Category> Categories => Set<Category>();

    public DbSet<Code> Codes => Set<Code>();

    public DbSet<CodeQuestion> CodeQuestions => Set<CodeQuestion>();

    public DbSet<Demo> Demos => Set<Demo>();

}
