using PruebaPayment.CommonModels;

namespace PruebaPayment.Features.TransactionEvents.GetTransactionEvents;

public interface IGetTransactionEventsService
{
    Task<Result<List<TransactionEventViewModel>>> GetByTransactionIdAsync(Guid transactionId, CancellationToken ct);
}

public class GetTransactionEventsService(IGetTransactionEventsRepository repository) : IGetTransactionEventsService
{
    public async Task<Result<List<TransactionEventViewModel>>> GetByTransactionIdAsync(Guid transactionId, CancellationToken ct)
    {
        var entities = await repository.GetByTransactionIdAsync(transactionId, ct);
        return Result<List<TransactionEventViewModel>>.Success([.. entities.Select(TransactionEventViewModel.From)]);
    }
}
