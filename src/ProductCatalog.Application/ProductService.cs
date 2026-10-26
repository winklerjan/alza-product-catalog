using Microsoft.Extensions.Logging;

namespace ProductCatalog.Application;

public sealed partial class ProductService(IProductRepository repository, ILogger<ProductService> logger) : IProductService
{
    public async Task<IReadOnlyList<ProductDto>> GetAllAsync(CancellationToken ct)
    {
        var products = await repository.GetAllAsync(ct);
        return products.Select(p => p.ToDto()).ToList();
    }

    public async Task<PagedResult<ProductDto>> GetPageAsync(int page, int pageSize, CancellationToken ct)
    {
        var result = await repository.GetPageAsync(page, pageSize, ct);
        var items = result.Items.Select(p => p.ToDto()).ToList();
        return new PagedResult<ProductDto>(items, result.Page, result.PageSize, result.TotalCount);
    }

    public async Task<ProductDto?> GetByIdAsync(int id, CancellationToken ct)
    {
        var product = await repository.GetByIdAsync(id, ct);
        return product?.ToDto();
    }

    public async Task<ProductDto?> UpdateDescriptionAsync(int id, string? description, CancellationToken ct)
    {
        var product = await repository.GetByIdAsync(id, ct);
        if (product is null)
        {
            return null;
        }

        product.UpdateDescription(description);
        await repository.UpdateAsync(product, ct);

        LogDescriptionUpdated(logger, id);

        return product.ToDto();
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Updated description for product {ProductId}")]
    private static partial void LogDescriptionUpdated(ILogger logger, int productId);
}
