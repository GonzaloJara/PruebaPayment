using PruebaPayment.DB.Models;

namespace PruebaPayment.Features.Merchants.ViewModels;

public record MerchantViewModel(Guid Id, string Name, bool IsActive)
{
    public static MerchantViewModel From(Merchant entity)
        => new(entity.Id, entity.Name, entity.IsActive);
};

public record MerchantViewModelWithTotal(Guid Id, string Name, bool IsActive, long TotalRecords)
    : MerchantViewModel(Id, Name, IsActive);
