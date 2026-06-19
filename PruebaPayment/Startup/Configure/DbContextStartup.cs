using Microsoft.EntityFrameworkCore;
using PruebaPayment.DB;
using PruebaPayment.DB.Models;

namespace PruebaPayment.Startup.Configure;

internal static class DbContextStartup
{
    internal static void AddEntityFramework(WebApplicationBuilder builder)
    {
        builder.AddNpgsqlDbContext<PaymentsDbContext>(connectionName: "payment-db",
            configureDbContextOptions: options =>
            {
                options.UseSeeding(DBSeeder.Seed);
            });
    }

    internal static void UseEntityFramework(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<PaymentsDbContext>();
        context.Database.Migrate();
    }
}
