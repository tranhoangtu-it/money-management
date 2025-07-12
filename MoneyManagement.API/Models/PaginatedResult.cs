namespace MoneyManagement.API.Models;

public class PaginatedResult<T>
{
    public IEnumerable<T> Data { get; set; } = [];
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasPrevious => CurrentPage > 1;
    public bool HasNext => CurrentPage < TotalPages;

    public PaginatedResult(IEnumerable<T> data, int currentPage, int pageSize, int totalCount)
    {
        Data = data;
        CurrentPage = currentPage;
        PageSize = pageSize;
        TotalCount = totalCount;
    }
}

public class PaginationParameters
{
    private const int MaxPageSize = 100;
    private int _pageSize = 10;
    
    public int Page { get; set; } = 1;
    
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
    }
}