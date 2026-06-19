using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PruebaPayment.DB.Models;

public class Merchant
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public required string Name { get; init; }
    public bool IsActive { get; init; } = true;
}


public class MerchantConfiguration : IEntityTypeConfiguration<Merchant>
{
    public void Configure(EntityTypeBuilder<Merchant> builder)
    {
        builder.HasKey(x => x.Id);

        builder
            .HasIndex(m => m.Name)
            .IsUnique();

        builder.Property(x => x.IsActive)
            .HasDefaultValue(true);
    }
}