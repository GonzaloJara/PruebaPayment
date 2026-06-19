using PruebaPayment.Adq.Models;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Mock de adquirente externo ISO 8583: aprueba, rechaza o falla de distintas formas,
// pensado para poder ejercitar el resilience pipeline del cliente "adq":
// - 500/503 (~20%): error transitorio del adquirente -> se reintenta y cuenta para el circuit breaker.
// - delay de 6s (~10%): supera el timeout por intento del cliente (5s) -> se reintenta como timeout.
// - 400 (~10%): error crítico/no transitorio -> el cliente falla rápido, sin reintentos.
app.MapPost("/authorize", async (AuthorizationRequest request, ILogger<Program> logger, CancellationToken cancellationToken) =>
{
    logger.LogInformation("Received authorize request, Stan {Stan}, MerchantId {MerchantId}", request.Stan, request.MerchantId);

    var outcome = Random.Shared.Next(10);

    switch (outcome)
    {
        case 6:
        case 7:
            var transientStatus = Random.Shared.Next(2) == 0
                ? StatusCodes.Status500InternalServerError
                : StatusCodes.Status503ServiceUnavailable;
            logger.LogWarning("Simulating acquirer processing error {StatusCode} for Stan {Stan}", transientStatus, request.Stan);
            return Results.Problem(
                title: "Acquirer processing error",
                statusCode: transientStatus);

        case 8:
            logger.LogWarning("Simulating acquirer timeout for Stan {Stan}", request.Stan);
            await Task.Delay(TimeSpan.FromSeconds(6), cancellationToken);
            return Results.Problem(
                title: "Acquirer timeout",
                statusCode: StatusCodes.Status504GatewayTimeout);

        case 9:
            logger.LogWarning("Simulating invalid authorization request for Stan {Stan}", request.Stan);
            return Results.Problem(
                title: "Invalid authorization request",
                statusCode: StatusCodes.Status400BadRequest);

        default:
            var approved = outcome < 4; // 0-3 = Approved (40%), 4-5 = Declined (20%)
            var response = approved
                ? new AuthorizationResponse("0110", request.Stan, "00", "Approved", Random.Shared.Next(100000, 999999).ToString())
                : new AuthorizationResponse("0110", request.Stan, "05", "Declined", null);
            logger.LogInformation("Authorize response code {ResponseCode} for Stan {Stan}", response.ResponseCode, request.Stan);
            return Results.Ok(response);
    }
})
.WithName("Authorize");

app.Run();
