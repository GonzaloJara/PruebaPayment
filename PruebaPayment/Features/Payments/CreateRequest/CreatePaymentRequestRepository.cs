using Microsoft.EntityFrameworkCore;
using PruebaPayment.DB;
using PruebaPayment.DB.Models;

namespace PruebaPayment.Features.Payments.CreateRequest;

public interface ICreatePaymentRequestRepository
{
    Task<PaymentRequest> CreateAsync(PaymentRequest entity, CancellationToken ct);
    Task<bool> ExistsByIdempotencyKeyAsync(Guid idempotencyKey, CancellationToken ct);
    Task<bool> IsMerchantActiveAsync(Guid merchantId, CancellationToken ct);
}


public class CreatePaymentRequestRepository(PaymentsDbContext dbContext)
    : ICreatePaymentRequestRepository
{
    public async Task<PaymentRequest> CreateAsync(PaymentRequest entity, CancellationToken ct)
    {
        await dbContext.PaymentRequests.AddAsync(entity, ct);
        await dbContext.SaveChangesAsync(ct);
        return entity;
    }

    public async Task<bool> ExistsByIdempotencyKeyAsync(Guid idempotencyKey, CancellationToken ct)
    {
        return await dbContext.PaymentRequests
            .Where(x => x.IdempotencyKey == idempotencyKey)
            .AnyAsync(ct);
    }

    public async Task<bool> IsMerchantActiveAsync(Guid merchantId, CancellationToken ct)
    {
        return await dbContext.Merchants
            .Where(x => x.Id == merchantId)
            .Select(x => x.IsActive)
            .FirstOrDefaultAsync(ct);
    }
}