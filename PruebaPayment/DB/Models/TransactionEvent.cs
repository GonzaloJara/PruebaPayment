using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PruebaPayment.DB.Models;

public enum TransactionEventType
{
    Created,
    Processing,
    AdqRequest,
    AdqResult,
    Result,
    InfoRequested
}

public enum TransactionEventLevel
{
    Info,
    Warning,
    Error
}

public class TransactionEvent
{
    public long Id { get; set; }
    public Guid TransactionId { get; set; }
    public TransactionEventType EventType { get; set; }
    public TransactionEventLevel Level { get; set; } = TransactionEventLevel.Info;
    public string Message { get; set; } = string.Empty;
    public string? Details { get; set; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}

public class TransactionEventConfiguration : IEntityTypeConfiguration<TransactionEvent>
{
    public void Configure(EntityTypeBuilder<TransactionEvent> builder)
    {
        builder.HasKey(x => x.Id);

        // No FK constraint to PaymentRequests: this keeps the event log usable for
        // requests targeting a transaction id that never resolved to a row (e.g.
        // info requests for an unknown id), which is itself useful signal.
        builder
            .HasIndex(x => new { x.TransactionId, x.CreatedAt });

        builder
            .HasIndex(x => x.Level);
    }
}
