using System.Collections.Concurrent;
using ProductCatalog.Application;
using ProductCatalog.Domain;
using ProductCatalog.Infrastructure.Seed;

namespace ProductCatalog.Infrastructure.Mock;

public sealed class InMemoryProductRepository : IProductRepository
{
    private readonly ConcurrentDictionary<int, Product> _products =
        new(ProductSeedData.All.ToDictionary(p => p.Id, Clone));

    public Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken ct)
    {
        IReadOnlyList<Product> result = AvailableOrderedById().Select(Clone).ToList();
        return Task.FromResult(result);
    }

    public Task<PagedResult<Product>> GetPageAsync(int page, int pageSize, CancellationToken ct)
    {
        var available = AvailableOrderedById().ToList();
        IReadOnlyList<Product> items = available.Skip((page - 1) * pageSize).Take(pageSize).Select(Clone).ToList();
        return Task.FromResult(new PagedResult<Product>(items, page, pageSize, available.Count));
    }

    public Task<Product?> GetByIdAsync(int id, CancellationToken ct)
    {
        var product = _products.TryGetValue(id, out var found) ? Clone(found) : null;
        return Task.FromResult(product);
    }

    public Task UpdateAsync(Product product, CancellationToken ct)
    {
        _products[product.Id] = Clone(product);
        return Task.CompletedTask;
    }

    private IEnumerable<Product> AvailableOrderedById() =>
        _products.Values.Where(p => p.IsAvailable).OrderBy(p => p.Id);

    private static Product Clone(Product product) =>
        new(product.Id, product.Name, product.ImgUri, product.Price, product.Description, product.IsAvailable);
}
