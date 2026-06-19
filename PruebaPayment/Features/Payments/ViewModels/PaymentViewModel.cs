using PruebaPayment.DB.Models;

namespace PruebaPayment.Features.Payments.ViewModels;

public record PaymentViewModel(Guid TransactionId, Guid MerchantId, DateTime CreatedAt, string Status)
{
    public static PaymentViewModel From(PaymentRequest entity)
        => new(entity.TransactionId, entity.MerchantId, entity.CreatedAt, entity.Status.ToString());
};

public record PaymentViewModelWithTotal(Guid TransactionId, Guid MerchantId, DateTime CreatedAt, string Status, long TotalRecords)
    : PaymentViewModel(TransactionId, MerchantId, CreatedAt, Status);