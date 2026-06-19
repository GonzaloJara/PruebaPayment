namespace PruebaPayment.Startup.Configure;

internal static class RedisCacheStartup
{
    internal static readonly string ConnectionStringName = "payment-cache";

    internal static void AddRedisCache(WebApplicationBuilder builder)
    {
        builder.AddRedisDistributedCache(connectionName: ConnectionStringName);
    }
}
