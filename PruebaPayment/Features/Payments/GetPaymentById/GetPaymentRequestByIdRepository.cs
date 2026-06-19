using PruebaPayment.DB;
using PruebaPayment.DB.Models;

namespace PruebaPayment.Features.Payments.GetPaymentById;

public interface IGetPaymentRequestByIdRepository
{
    Task<PaymentRequest?> GetByIdAsync(Guid id, CancellationToken ct);
}

public class GetPaymentRequestByIdRepository(PaymentsDbContext dbContext)
    : IGetPaymentRequestByIdRepository
{
    public async Task<PaymentRequest?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await dbContext.PaymentRequests.FindAsync([id], ct);
    }
}