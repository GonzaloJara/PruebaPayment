using PruebaPayment.ExceptionHandlers;
using PruebaPayment.Startup.Configure;
using PruebaPayment.Startup.DI;
using Serilog;

namespace PruebaPayment.Startup;

public static class StartupHandler
{
    public static void ConfigureStartup(WebApplicationBuilder builder)
    {
        // Add Serilog
        SerilogStartup.AddAndConfigureSerilog(builder);

        // Add Cors
        CorsStartup.AddCors(builder);

        // Add rate limiter on x-merchant-id header
        RateLimitingStartup.AddRateLimiter(builder);

        // Add jwt configuration
        JwtStartup.AddJwt(builder);

        // Add EFCore
        DbContextStartup.AddEntityFramework(builder);

        // Add Dapper SqlConnection
        DapperStartup.AddDapperSqlConnection(builder);

        // Add Redis as the distributed (L2) cache store
        RedisCacheStartup.AddRedisCache(builder);

        // Add cache
        builder.Services.AddHybridCache();

        // Add RabbitMq
        RabbitMqStartup.AddRabbitMq(builder);

        // Add Http clients
        AdqHttpClientStartup.AddAdqHttpClient(builder);

        // Add swagger
        SwaggerStartup.AddSwagger(builder);

        // Add exception handler
        builder.Services.AddProblemDetails();
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

        // Add services
        builder.RegisterServices(typeof(Program).Assembly);

        // Add endpoints to be mapped
        builder.Services.AddEndpoints(typeof(Program).Assembly);
    }

    public static async Task UseStartup(WebApplication app)
    {
        // Apply pending migrations and seed data
        DbContextStartup.UseEntityFramework(app);

        // Declare RabbitMQ topology owned by this publisher (the payment.events exchange)
        await RabbitMqStartup.UseRabbitMqAsync(app);

        // Add Serilog request logging
        app.UseSerilogRequestLogging();

        // Use Cors
        app.UseCors(CorsStartup.PolicyName);

        // Use rate limiter on x-merchant-id header
        app.UseRateLimiter();

        // Use Swagger
        SwaggerStartup.UseSwagger(app);

        // Use auth
        app.UseAuthentication();
        app.UseAuthorization();

        // Use exception handler
        app.UseExceptionHandler();

        // Use HTTPS Redirect
        app.UseHttpsRedirection();

        // Map endpoints added
        app.MapEndpoints();
    }
}
