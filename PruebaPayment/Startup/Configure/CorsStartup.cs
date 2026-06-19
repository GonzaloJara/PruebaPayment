namespace PruebaPayment.Startup.Configure;

internal static class CorsStartup
{
    internal static readonly string PolicyName = "EverythingEnabledCorsPolicy";
    internal static void AddCors(WebApplicationBuilder builder)
    {
        var allowedOrigins = builder.Configuration
            .GetSection("Cors:AllowedOrigins")
            .Get<string[]>() ?? [];

        builder.Services.AddCors(options =>
        {
            options.AddPolicy(
                name: PolicyName,
                policy =>
                {
                    policy
                        .WithOrigins(allowedOrigins)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
        });
    }
}
