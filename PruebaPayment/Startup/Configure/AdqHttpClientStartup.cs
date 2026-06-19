using Microsoft.Extensions.Http.Resilience;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;

namespace PruebaPayment.Startup.Configure;

internal static class AdqHttpClientStartup
{
    internal static void AddAdqHttpClient(WebApplicationBuilder builder)
    {
        builder.Services.AddHttpClient("adq", (client) =>
        {
            client.BaseAddress = new Uri("http+https://payment-adq"); // Aspire service discovery resolves this url to the correct one
        })
        // Se puede usar el standart resilliency handler que es más o menos lo mismo que lo de abajo, pero ya qué.
        //.AddStandardResilienceHandler()
        .AddResilienceHandler("adq-pipeline", pipelineBuilder =>
        {
            // Same defaults as AddStandardResilienceHandler: 1000 concurrent permits, no queueing (extra calls fail fast).
            pipelineBuilder.AddConcurrencyLimiter(permitLimit: 1000, queueLimit: 0);

            // Overall timeout. Includes retry attempts
            pipelineBuilder.AddTimeout(TimeSpan.FromSeconds(20));

            // Only transient failures (5xx, 408, network/timeout errors) are retried.
            // Critical/client errors (4xx) are not transient and fail fast instead of being retried.
            pipelineBuilder.AddRetry(new RetryStrategyOptions<HttpResponseMessage>
            {
                ShouldHandle = static args => ValueTask.FromResult(HttpClientResiliencePredicates.IsTransient(args.Outcome)),
                MaxRetryAttempts = 3,
                BackoffType = DelayBackoffType.Exponential,
                Delay = TimeSpan.FromSeconds(1),
                UseJitter = true,
            });

            // If a lot of transient failures happens (if ≥50% of calls fail within a rolling 30s window, once at least 10 calls have occurred)
            // it opens the circuit to protect the adq api.
            pipelineBuilder.AddCircuitBreaker(new CircuitBreakerStrategyOptions<HttpResponseMessage>
            {
                ShouldHandle = static args => ValueTask.FromResult(HttpClientResiliencePredicates.IsTransient(args.Outcome)),
                FailureRatio = 0.5,
                MinimumThroughput = 10,
                SamplingDuration = TimeSpan.FromSeconds(30),
                BreakDuration = TimeSpan.FromSeconds(15),
            });
        });
    }
}
