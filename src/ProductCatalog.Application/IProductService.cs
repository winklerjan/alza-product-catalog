using ProductCatalog.Domain;

namespace ProductCatalog.Application;

public interface IProductService
{
    Task<IReadOnlyList<IReadOnlyProduct>> GetAllAsync(CancellationToken ct);

    Task<PagedResult<IReadOnlyProduct>> GetPageAsync(int page, int pageSize, CancellationToken ct);

    Task<IReadOnlyProduct?> GetByIdAsync(int id, CancellationToken ct);

    Task<IReadOnlyProduct?> UpdateDescriptionAsync(int id, string? description, CancellationToken ct);
}
