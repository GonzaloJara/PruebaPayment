using Dapper;
using Npgsql;
using PruebaPayment.CommonModels;
using PruebaPayment.Features.Merchants.ViewModels;

namespace PruebaPayment.Features.Merchants.GetMerchants;
public interface IGetMerchantsRepository
{
    Task<PagedResult<MerchantViewModel>> GetAllAsync(MerchantSearchParams searchParams, CancellationToken ct);
}

public class GetMerchantsRepository(NpgsqlDataSource dataSource) : IGetMerchantsRepository
{
    public async Task<PagedResult<MerchantViewModel>> GetAllAsync(MerchantSearchParams searchParams, CancellationToken ct)
    {
        (int pageNumber, int pageSize) = searchParams.GetPageValues();

        string orderDirection = searchParams.FormattedOrderDirection;
        string orderBy = searchParams.OrderBy?.ToLower() switch
        {
            "name" => "Name",
            "isactive" => "IsActive",
            _ => "Name"
        };

        var sql =
        $"""
            SELECT
                "Id",
                "Name",
                "IsActive",
                COUNT(*) OVER() AS TotalRecords
            FROM
                public."Merchants"
            WHERE
                    ("Name"     = @Name     OR @Name IS NULL)
                AND ("IsActive" = @IsActive OR @IsActive IS NULL)
            ORDER BY "{orderBy}" {orderDirection}
            LIMIT @PageSize OFFSET @Offset
            """;

        await using var db = await dataSource.OpenConnectionAsync(ct);

        var results = (await db.QueryAsync<MerchantViewModelWithTotal>(sql, new
        {
            searchParams.Name,
            searchParams.IsActive,
            Offset = (pageNumber - 1) * pageSize,
            PageSize = pageSize
        }))
        .ToList();

        long totalRecords = results.FirstOrDefault()?.TotalRecords ?? 0;

        return new PagedResult<MerchantViewModel>
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalRecords = totalRecords,
            TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize),
            Data = [.. results.Cast<MerchantViewModel>()]
        };
    }
}
