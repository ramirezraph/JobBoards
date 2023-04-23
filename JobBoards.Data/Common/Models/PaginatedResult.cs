using Microsoft.EntityFrameworkCore;

namespace JobBoards.Data.Common.Models;

public class PaginatedResult<T>
{
    public List<T> Items { get; private set; } = null!;
    public int CurrentPage { get; private set; }
    public int ItemsPerPage { get; private set; }
    public int TotalPages { get; private set; }
    public bool HasPreviousPage { get; private set; }
    public bool HasNextPage { get; private set; }

    public PaginatedResult(
        List<T> items,
        int currentPage,
        int itemsPerPage,
        int totalPages,
        bool hasPreviousPage,
        bool hasNextPage)
    {
        Items = items;
        CurrentPage = currentPage;
        ItemsPerPage = itemsPerPage;
        TotalPages = totalPages;
        HasPreviousPage = hasPreviousPage;
        HasNextPage = hasNextPage;
    }

    public PaginatedResult()
    {
    }

    public static async Task<PaginatedResult<T>> CreateAsync(
        IQueryable<T> source,
        int pageIndex,
        int pageSize)
    {
        var count = await source.CountAsync();
        var items = await source
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize).ToListAsync();

        var totalPages = (int)Math.Ceiling(count / (double)pageSize);
        var hasPreviousPage = pageIndex > 1;
        var hasNextPage = pageIndex < totalPages;

        return new PaginatedResult<T>
        {
            Items = items,
            CurrentPage = pageIndex,
            ItemsPerPage = pageSize,
            TotalPages = totalPages,
            HasPreviousPage = hasPreviousPage,
            HasNextPage = hasNextPage
        };
    }
}

