namespace PruebaPayment.DB.Conventions;

using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

public class LowercaseEnumConvention : IModelFinalizingConvention
{
    public void ProcessModelFinalizing(
        IConventionModelBuilder builder,
        IConventionContext<IConventionModelBuilder> context)
    {
        foreach (var entityType in builder.Metadata.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                var clrType = property.ClrType;
                var enumType = Nullable.GetUnderlyingType(clrType) ?? clrType;

                if (!enumType.IsEnum) continue;

                var converterType = typeof(LowercaseEnumConverter<>).MakeGenericType(enumType);
                var converter = (ValueConverter)Activator.CreateInstance(converterType)!;
                property.Builder.HasConversion(converter);
            }
        }
    }
}

public class LowercaseEnumConverter<TEnum> : ValueConverter<TEnum, string>
    where TEnum : struct, Enum
{
    public LowercaseEnumConverter() : base(
        v => v.ToString().ToLowerInvariant(),
        v => Enum.Parse<TEnum>(v, ignoreCase: true))
    { }
}