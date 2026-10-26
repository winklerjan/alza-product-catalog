using Microsoft.EntityFrameworkCore;
using ProductCatalog.Application;
using ProductCatalog.Domain;

namespace ProductCatalog.Infrastructure.Persistence;

public sealed class SqlProductRepository(ProductCatalogDbContext dbContext) : IProductRepository
{
    public async Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken ct) =>
        await AvailableOrderedById().ToListAsync(ct);

    public async Task<PagedResult<Product>> GetPageAsync(int page, int pageSize, CancellationToken ct)
    {
        var query = AvailableOrderedById();
        var totalCount = await query.CountAsync(ct);
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return new PagedResult<Product>(items, page, pageSize, totalCount);
    }

    public Task<Product?> GetByIdAsync(int id, CancellationToken ct) =>
        dbContext.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task UpdateAsync(Product product, CancellationToken ct)
    {
        dbContext.Products.Update(product);
        await dbContext.SaveChangesAsync(ct);
    }

    private IQueryable<Product> AvailableOrderedById() =>
        dbContext.Products.AsNoTracking().Where(p => p.IsAvailable).OrderBy(p => p.Id);
}
