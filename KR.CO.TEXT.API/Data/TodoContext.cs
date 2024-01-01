using Microsoft.EntityFrameworkCore;

namespace KR.CO.TEXT.API;

public class TodoContext(DbContextOptions<TodoContext> options) : DbContext(options)
{
    public DbSet<TodoItem> TodoItems {get;set;} = null!;
}
