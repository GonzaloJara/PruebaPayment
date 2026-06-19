using Microsoft.EntityFrameworkCore;
using PruebaPayment.DB.Conventions;
using PruebaPayment.DB.Models;

namespace PruebaPayment.DB;

public class PaymentsDbContext(DbContextOptions<PaymentsDbContext> options) : DbContext(options)
{
    public DbSet<Merchant> Merchants { get; set; }
    public DbSet<PaymentRequest> PaymentRequests { get; set; }
    public DbSet<TransactionEvent> TransactionEvents { get; set; }

//#if DEBUG
//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//        => optionsBuilder
//            .EnableSensitiveDataLogging(true);
//#endif

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Conventions.Add(_ => new LowercaseEnumConvention());
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PaymentsDbContext).Assembly);
    }
}