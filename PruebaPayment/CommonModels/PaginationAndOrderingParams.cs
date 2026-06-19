namespace PruebaPayment.CommonModels;

public class PaginationAndOrderingParams
{
    public int? PageSize { get; init; }
    public int? PageNumber { get; init; }
    public string? OrderBy { get; init; }
    public string? OrderDirection { get; init; }
    public string FormattedOrderDirection => 
        string.IsNullOrWhiteSpace(OrderDirection) 
        ? PaginationAndOrderingParamsDefaults.OrderDirection 
        : OrderDirection.Equals(PaginationAndOrderingParamsDefaults.OrderDirection, StringComparison.OrdinalIgnoreCase) 
            ? PaginationAndOrderingParamsDefaults.OrderDirection 
            : "DESC";

    public string ToCacheKey() => $"_{PageNumber}_{PageSize}_{OrderBy}_{OrderDirection}";
    public (int pageNumber, int pageSize) GetPageValues()
    {
        // Validación de parámetros de paginamiento
        int pageNumber = Math.Max(1, PageNumber ?? PaginationAndOrderingParamsDefaults.PageNumber);

        int pageSize = Math.Clamp(
            PageSize ?? PaginationAndOrderingParamsDefaults.PageSize,
            PaginationAndOrderingParamsDefaults.MinPageSize,
            PaginationAndOrderingParamsDefaults.MaxPageSize);

        return (pageNumber,  pageSize);
    }
}

static class PaginationAndOrderingParamsDefaults
{
    public const int PageNumber = 1;
    public const int PageSize = 100;
    public const int MinPageSize = 10;
    public const int MaxPageSize = 100;
    public const string OrderDirection = "ASC";
}