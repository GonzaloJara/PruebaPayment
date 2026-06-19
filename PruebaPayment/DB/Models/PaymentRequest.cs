using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PruebaPayment.DB.Models;

public enum PaymentRequestStatus
{
    Pending,
    Processsing,
    Approved,
    Declined,
    Failed
}
public class PaymentRequest
{
    public Guid TransactionId { get; set; } = Guid.CreateVersion7();
    public Guid IdempotencyKey { get; set; } = Guid.CreateVersion7();
    public PaymentRequestStatus Status { get; set; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string EncryptedCardData { get; set; } = string.Empty;

    public Merchant? Merchant { get; set; }
    public Guid MerchantId { get; set; }
}

public class PaymentRequestConfiguration : IEntityTypeConfiguration<PaymentRequest>
{
    public void Configure(EntityTypeBuilder<PaymentRequest> builder)
    {
        builder.HasKey(x => x.TransactionId);
    }
}
