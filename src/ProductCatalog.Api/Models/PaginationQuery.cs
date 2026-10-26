using System.ComponentModel.DataAnnotations;

namespace ProductCatalog.Api.Models;

/// <summary>
/// Query parameters for a paginated product list.
/// </summary>
public sealed class PaginationQuery
{
    /// <summary>
    /// The 1-based page number.
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "Page must be at least 1.")]
    public int Page { get; init; } = 1;

    /// <summary>
    /// The number of items per page.
    /// </summary>
    [Range(1, 100, ErrorMessage = "PageSize must be between 1 and 100.")]
    public int PageSize { get; init; } = 10;
}
