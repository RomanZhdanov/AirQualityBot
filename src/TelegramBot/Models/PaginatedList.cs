namespace AirBro.TelegramBot.Models;

public class PaginatedList<T>
{
    public int PageNumber { get; }
    public int TotalPages { get; }
    public int TotalCount { get; }
    public IReadOnlyCollection<T> Items { get; }
    
    public PaginatedList(IReadOnlyCollection<T> items, int totalCount, int pageNumber, int pageSize)
    {
        PageNumber = pageNumber;
        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        TotalCount = totalCount;
        Items = items;
    }

    public bool HasPreviousPage => PageNumber > 1;

    public bool HasNextPage => PageNumber < TotalPages;
}