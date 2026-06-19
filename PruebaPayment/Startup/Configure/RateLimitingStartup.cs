using System.Globalization;
using System.Threading.RateLimiting;

namespace PruebaPayment.Startup.Configure;

internal static class RateLimitingStartup
{
    internal static void AddRateLimiter(WebApplicationBuilder builder)
    {
        builder.Services.AddRateLimiter(options =>
        {
            options.AddPolicy("fixed-per-merchant_id", httpContext =>
            {
                var phone = httpContext.Request.Headers["x-merchant-id"].ToString();

                return RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: phone ?? "no-merchant",
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 1,
                        Window = TimeSpan.FromSeconds(5),
                        QueueLimit = 0,
                        AutoReplenishment = true
                    });
            });
            options.OnRejected = RespondTooManyAttempts();
        });
    }

    private static Func<Microsoft.AspNetCore.RateLimiting.OnRejectedContext, CancellationToken, ValueTask> RespondTooManyAttempts()
    {
        return static async (context, cancellationToken) =>
        {
            if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
            {
                context.HttpContext.Response.Headers.RetryAfter =
                    ((int)retryAfter.TotalSeconds).ToString(NumberFormatInfo.InvariantInfo);
            }

            context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            await context.HttpContext.Response.WriteAsync("Too many requests. Please try again later.", cancellationToken);
        };
    }
}
