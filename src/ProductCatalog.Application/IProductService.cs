namespace ProductCatalog.Application;

public interface IProductService
{
    Task<IReadOnlyList<ProductDto>> GetAllAsync(CancellationToken ct);

    Task<PagedResult<ProductDto>> GetPageAsync(int page, int pageSize, CancellationToken ct);

    Task<ProductDto?> GetByIdAsync(int id, CancellationToken ct);

    Task<ProductDto?> UpdateDescriptionAsync(int id, string? description, CancellationToken ct);
}
