namespace SmartBudget.Domain.Primitives.Pagination;

public class PagedResponse<T>
{
    public IReadOnlyList<T> Data { get; init; } = [];
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public int TotalPages { get; init; }
    public int TotalRecords { get; init; }
    public bool HasNextPage => PageNumber < TotalPages;
    public bool HasPreviousPage => PageNumber > 1;

    public PagedResponse<TOut> Map<TOut>(Func<T, TOut> mapper) => new()
    {
        Data = [.. Data.Select(mapper)],
        PageNumber = PageNumber,
        PageSize = PageSize,
        TotalRecords = TotalRecords,
        TotalPages = TotalPages
    };
}
