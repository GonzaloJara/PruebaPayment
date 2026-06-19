using Microsoft.EntityFrameworkCore;
using PruebaPayment.DB;
using PruebaPayment.DB.Models;

namespace PruebaPayment.Features.TransactionEvents.GetTransactionEvents;

public interface IGetTransactionEventsRepository
{
    Task<List<TransactionEvent>> GetByTransactionIdAsync(Guid transactionId, CancellationToken ct);
}

public class GetTransactionEventsRepository(PaymentsDbContext dbContext) : IGetTransactionEventsRepository
{
    public async Task<List<TransactionEvent>> GetByTransactionIdAsync(Guid transactionId, CancellationToken ct)
    {
        return await dbContext.TransactionEvents
            .Where(x => x.TransactionId == transactionId)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync(ct);
    }
}
