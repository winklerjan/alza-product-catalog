using ProductCatalog.Domain;

namespace ProductCatalog.Application;

public interface IProductRepository
{
    Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken ct);

    Task<PagedResult<Product>> GetPageAsync(int page, int pageSize, CancellationToken ct);

    Task<Product?> GetByIdAsync(int id, CancellationToken ct);

    Task UpdateAsync(Product product, CancellationToken ct);
}
