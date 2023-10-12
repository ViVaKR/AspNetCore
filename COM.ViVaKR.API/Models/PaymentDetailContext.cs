using Microsoft.EntityFrameworkCore;

namespace COM.ViVaKR.API;

public class PaymentDetailContext : DbContext
{
    public PaymentDetailContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<PaymentDetail> PaymentDetails { get; set; }
    public DbSet<TodoItem> TodoItems { get; set; } = null!;
}
