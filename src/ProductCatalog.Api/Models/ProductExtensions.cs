using ProductCatalog.Domain;

namespace ProductCatalog.Api.Models;

/// <summary>
/// Maps domain products to their API representation.
/// </summary>
public static class ProductExtensions
{
    /// <summary>
    /// Maps a product to its API representation.
    /// </summary>
    public static ProductDto ToDto(this IReadOnlyProduct product) =>
        new(product.Id, product.Name, product.ImgUri, product.Price, product.Description, product.IsAvailable);
}
