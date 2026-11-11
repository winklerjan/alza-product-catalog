using Microsoft.Extensions.Logging;
using ProductCatalog.Domain;

namespace ProductCatalog.Application;

public sealed partial class ProductService(IProductRepository repository, ILogger<ProductService> logger) : IProductService
{
    public async Task<IReadOnlyList<IReadOnlyProduct>> GetAllAsync(CancellationToken ct)
    {
        var products = await repository.GetAllAsync(ct);
        return products;
    }

    public async Task<PagedResult<IReadOnlyProduct>> GetPageAsync(int page, int pageSize, CancellationToken ct)
    {
        var result = await repository.GetPageAsync(page, pageSize, ct);
        return new PagedResult<IReadOnlyProduct>(result.Items, result.Page, result.PageSize, result.TotalCount);
    }

    public async Task<IReadOnlyProduct?> GetByIdAsync(int id, CancellationToken ct)
    {
        var product = await repository.GetByIdAsync(id, ct);
        return product;
    }

    public async Task<IReadOnlyProduct?> UpdateDescriptionAsync(int id, string? description, CancellationToken ct)
    {
        var product = await repository.GetByIdAsync(id, ct);
        if (product is null)
        {
            return null;
        }

        product.UpdateDescription(description);
        await repository.UpdateAsync(product, ct);

        LogDescriptionUpdated(logger, id);

        return product;
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Updated description for product {ProductId}")]
    private static partial void LogDescriptionUpdated(ILogger logger, int productId);
}
