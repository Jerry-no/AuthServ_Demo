namespace AuthService.Common.Dtos;


public class PageRequest
{
    private const int MaxPageSize = 200;

    private int _pageNumber = 1;
    private int _pageSize = 20;

    public int PageNumber
    {
        get => _pageNumber;
        init => _pageNumber = value < 1 ? 1 : value;
    }

    public int PageSize
    {
        get => _pageSize;
        init => _pageSize = value switch
        {
            < 1 => 20,
            > MaxPageSize => MaxPageSize,
            _ => value
        };
    }

    public string? Keyword { get; init; }

    public string? SortBy { get; init; }

    public string? SortDirection { get; init; } = "desc";

    public int Skip => (PageNumber - 1) * PageSize;
}