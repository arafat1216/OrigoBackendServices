using Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Common.Extensions;

/// <summary>
/// Extension method for returning paginated results. From https://vmsdurano.com/asp-net-core-5-implement-web-api-pagination-with-hateoas-links/
/// </summary>
public static class DataPaginationExtension
{
    public static async Task<PagedModel<TModel>> PaginateAsync<TModel>(
        this IQueryable<TModel> query,
        int page,
        int limit,
        CancellationToken cancellationToken)
        where TModel : class
    {

        var paged = new PagedModel<TModel>();

        page = (page < 0) ? 1 : page;

        paged.CurrentPage = page;
        paged.PageSize = limit;

        var startRow = (page - 1) * limit;
        paged.Items = await query
            .Skip(startRow)
            .Take(limit)
            .ToListAsync(cancellationToken);

        paged.TotalItems = await query.CountAsync(cancellationToken);
        paged.TotalPages = (int)Math.Ceiling(paged.TotalItems / (double)limit);

        return paged;
    }
    
    public static async Task<PagedModel<TModel>> PaginateIndexedBasedAsync<TModel>(
        this IQueryable<TModel> query,
        int startIndex,
        int limit,
        CancellationToken cancellationToken)
        where TModel : class
    {

        var paged = new PagedModel<TModel>();

        startIndex = (startIndex < 0) ? 0 : startIndex;

        paged.PageSize = limit;

        paged.Items = await query
            .Skip(startIndex)
            .Take(limit)
            .ToListAsync(cancellationToken);

        paged.TotalItems = await query.CountAsync(cancellationToken);
        paged.TotalPages = (int)Math.Ceiling(paged.TotalItems / (double)limit);

        return paged;
    }

    public static PagedModel<TModel> PaginateAsync<TModel>(
        this IEnumerable<TModel> query,
        int page,
        int limit)
        where TModel : class
    {

        var paged = new PagedModel<TModel>();

        page = (page < 0) ? 1 : page;

        paged.CurrentPage = page;
        paged.PageSize = limit;

        var startRow = (page - 1) * limit;
        paged.Items = query
            .Skip(startRow)
            .Take(limit)
            .ToList();

        paged.TotalItems = query.Count();
        paged.TotalPages = (int)Math.Ceiling(paged.TotalItems / (double)limit);

        return paged;
    }
}