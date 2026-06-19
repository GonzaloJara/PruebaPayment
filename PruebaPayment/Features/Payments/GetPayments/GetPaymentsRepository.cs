using Dapper;
using Npgsql;
using PruebaPayment.CommonModels;
using PruebaPayment.Features.Payments.ViewModels;

namespace PruebaPayment.Features.Payments.GetPayments;
public interface IGetPaymentsRepository
{
    Task<PagedResult<PaymentViewModel>> GetAllAsync(PaymentSearchParams searchParams, CancellationToken ct);
}

public class GetPaymentsRepository(NpgsqlDataSource dataSource) : IGetPaymentsRepository
{
    public async Task<PagedResult<PaymentViewModel>> GetAllAsync(PaymentSearchParams searchParams, CancellationToken ct)
    {
        (int pageNumber, int pageSize) = searchParams.GetPageValues();

        string orderDirection = searchParams.FormattedOrderDirection;
        string orderBy = searchParams.OrderBy?.ToLower() switch
        {
            "merchantid" => "MerchantId",
            "createdat" => "CreatedAt",
            _ => "CreatedAt"
        };

        var sql =
        $"""
            SELECT
                "TransactionId",
                "MerchantId",
                "CreatedAt",
                "Status",
                COUNT(*) OVER() AS TotalRecords
            FROM
                public."PaymentRequests"
            WHERE
                    ("MerchantId" = @MerchantId OR @MerchantId IS NULL)
                AND ("Status"     = @Status     OR @Status IS NULL)
            ORDER BY "{orderBy}" {orderDirection}
            LIMIT @PageSize OFFSET @Offset
            """;

        await using var db = await dataSource.OpenConnectionAsync(ct);

        var results = (await db.QueryAsync<PaymentViewModelWithTotal>(sql, new
        {
            searchParams.MerchantId,
            searchParams.Status,
            Offset = (pageNumber - 1) * pageSize,
            PageSize = pageSize
        }))
        .ToList();

        long totalRecords = results.FirstOrDefault()?.TotalRecords ?? 0;

        return new PagedResult<PaymentViewModel>
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalRecords = totalRecords,
            TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize),
            Data = [.. results.Cast<PaymentViewModel>()]
        };
    }
}