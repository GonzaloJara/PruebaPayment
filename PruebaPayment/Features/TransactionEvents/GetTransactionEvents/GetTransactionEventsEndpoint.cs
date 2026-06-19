using Microsoft.AspNetCore.Mvc;

namespace PruebaPayment.Features.TransactionEvents.GetTransactionEvents;

public static class GetTransactionEventsEndpoint
{
    public static async Task<IResult> GetByTransactionId(
        [FromRoute] Guid id,
        IGetTransactionEventsService service,
        CancellationToken ct)
    {
        var result = await service.GetByTransactionIdAsync(id, ct);

        return Results.Ok(result);
    }
}
