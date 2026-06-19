namespace PruebaPayment.CommonModels;

public class PagedResult<T>
{
    public required int PageNumber { get; init; }
    public required int PageSize { get; init; }
    public required long TotalRecords { get; init; }
    public required int TotalPages { get; init; }

    public required List<T> Data { get; init; } = [];

    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}