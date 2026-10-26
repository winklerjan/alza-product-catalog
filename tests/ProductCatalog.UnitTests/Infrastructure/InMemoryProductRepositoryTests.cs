using ProductCatalog.Infrastructure.Mock;
using ProductCatalog.Infrastructure.Seed;
using Shouldly;

namespace ProductCatalog.UnitTests.Infrastructure;

public class InMemoryProductRepositoryTests
{
    private readonly InMemoryProductRepository _sut = new();

    [Fact]
    public async Task GetAllAsync_ReturnsAllSeedProductsOrderedById()
    {
        var result = await _sut.GetAllAsync(CancellationToken.None);

        result.Count.ShouldBe(ProductSeedData.All.Count);
        result.Select(p => p.Id).ShouldBe(result.Select(p => p.Id).OrderBy(id => id));
    }

    [Fact]
    public async Task GetPageAsync_FirstPage_ReturnsRequestedPageSize()
    {
        var result = await _sut.GetPageAsync(1, 10, CancellationToken.None);

        result.Items.Count.ShouldBe(10);
        result.Items.Select(p => p.Id).ShouldBe(Enumerable.Range(1, 10));
        result.TotalCount.ShouldBe(ProductSeedData.All.Count);
        result.TotalPages.ShouldBe(3);
    }

    [Fact]
    public async Task GetPageAsync_LastPartialPage_ReturnsRemainingItems()
    {
        var result = await _sut.GetPageAsync(3, 10, CancellationToken.None);

        result.Items.Count.ShouldBe(5);
        result.Items.Select(p => p.Id).ShouldBe(Enumerable.Range(21, 5));
    }

    [Fact]
    public async Task GetPageAsync_PastTheEnd_ReturnsEmptyItems()
    {
        var result = await _sut.GetPageAsync(100, 10, CancellationToken.None);

        result.Items.ShouldBeEmpty();
        result.TotalCount.ShouldBe(ProductSeedData.All.Count);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsProduct()
    {
        var result = await _sut.GetByIdAsync(1, CancellationToken.None);

        result.ShouldNotBeNull();
        result.Id.ShouldBe(1);
    }

    [Fact]
    public async Task GetByIdAsync_UnknownId_ReturnsNull()
    {
        var result = await _sut.GetByIdAsync(-1, CancellationToken.None);

        result.ShouldBeNull();
    }

    [Fact]
    public async Task UpdateAsync_ExistingProduct_PersistsChange()
    {
        var product = await _sut.GetByIdAsync(1, CancellationToken.None);
        product!.UpdateDescription("Updated via test.");

        await _sut.UpdateAsync(product, CancellationToken.None);
        var reloaded = await _sut.GetByIdAsync(1, CancellationToken.None);

        reloaded!.Description.ShouldBe("Updated via test.");
    }

    [Fact]
    public async Task GetByIdAsync_MutatingReturnedInstance_DoesNotAffectStore()
    {
        var first = await _sut.GetByIdAsync(1, CancellationToken.None);
        first!.UpdateDescription("Mutated locally, never saved.");

        var second = await _sut.GetByIdAsync(1, CancellationToken.None);

        second!.Description.ShouldNotBe("Mutated locally, never saved.");
    }
}
