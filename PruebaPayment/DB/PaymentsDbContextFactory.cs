using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace PruebaPayment.DB;

public class PaymentsDbContextFactory : IDesignTimeDbContextFactory<PaymentsDbContext>
{
    public PaymentsDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__payment-db")
            ?? "Host=localhost;Port=5432;Database=payments;Username=postgres;Password=postgres";

        var optionsBuilder = new DbContextOptionsBuilder<PaymentsDbContext>()
            .UseNpgsql(connectionString);

        return new PaymentsDbContext(optionsBuilder.Options);
    }
}
