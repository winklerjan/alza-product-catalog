using ProductCatalog.Infrastructure.Mock;
using ProductCatalog.Infrastructure.Seed;
using Shouldly;

namespace ProductCatalog.UnitTests.Infrastructure;

public class InMemoryProductRepositoryTests
{
    private readonly InMemoryProductRepository _sut = new();

    private static readonly int[] AvailableIds =
        ProductSeedData.All.Where(p => p.IsAvailable).Select(p => p.Id).OrderBy(id => id).ToArray();

    [Fact]
    public async Task GetAllAsync_ReturnsOnlyAvailableSeedProductsOrderedById()
    {
        var result = await _sut.GetAllAsync(CancellationToken.None);

        result.Select(p => p.Id).ShouldBe(AvailableIds);
        result.ShouldAllBe(p => p.IsAvailable);
    }

    [Fact]
    public async Task GetPageAsync_FirstPage_ReturnsRequestedPageSize()
    {
        var result = await _sut.GetPageAsync(1, 10, CancellationToken.None);

        result.Items.Count.ShouldBe(10);
        result.Items.Select(p => p.Id).ShouldBe(AvailableIds.Take(10));
        result.TotalCount.ShouldBe(AvailableIds.Length);
    }

    [Fact]
    public async Task GetPageAsync_LastPartialPage_ReturnsRemainingItems()
    {
        var lastPage = (int)Math.Ceiling(AvailableIds.Length / 10.0);

        var result = await _sut.GetPageAsync(lastPage, 10, CancellationToken.None);

        result.Items.Select(p => p.Id).ShouldBe(AvailableIds.Skip((lastPage - 1) * 10));
    }

    [Fact]
    public async Task GetPageAsync_PastTheEnd_ReturnsEmptyItems()
    {
        var result = await _sut.GetPageAsync(100, 10, CancellationToken.None);

        result.Items.ShouldBeEmpty();
        result.TotalCount.ShouldBe(AvailableIds.Length);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsProduct()
    {
        var result = await _sut.GetByIdAsync(1, CancellationToken.None);

        result.ShouldNotBeNull();
        result.Id.ShouldBe(1);
    }

    [Fact]
    public async Task GetByIdAsync_UnavailableId_StillReturnsProduct()
    {
        var unavailableId = ProductSeedData.All.First(p => !p.IsAvailable).Id;

        var result = await _sut.GetByIdAsync(unavailableId, CancellationToken.None);

        result.ShouldNotBeNull();
        result.IsAvailable.ShouldBeFalse();
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
