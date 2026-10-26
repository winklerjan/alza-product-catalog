using Microsoft.Extensions.Logging;
using NSubstitute;
using ProductCatalog.Application;
using ProductCatalog.Domain;
using Shouldly;

namespace ProductCatalog.UnitTests.Application;

public class ProductServiceTests
{
    private readonly IProductRepository _repository = Substitute.For<IProductRepository>();
    private readonly ILogger<ProductService> _logger = Substitute.For<ILogger<ProductService>>();
    private readonly ProductService _sut;

    public ProductServiceTests()
    {
        _sut = new ProductService(_repository, _logger);
    }

    private static Product CreateProduct(int id = 1, string? description = "Original.", bool isAvailable = true) =>
        new(id, "Widget", "https://placehold.co/600x400?text=Widget", 9.99m, description, isAvailable);

    [Fact]
    public async Task GetAllAsync_ReturnsMappedDtos()
    {
        var products = new List<Product> { CreateProduct(1), CreateProduct(2) };
        _repository.GetAllAsync(Arg.Any<CancellationToken>()).Returns(products);

        var result = await _sut.GetAllAsync(CancellationToken.None);

        result.Select(d => d.Id).ShouldBe([1, 2]);
    }

    [Fact]
    public async Task GetAllAsync_PassesCancellationToken()
    {
        using var cts = new CancellationTokenSource();
        _repository.GetAllAsync(cts.Token).Returns([]);

        await _sut.GetAllAsync(cts.Token);

        await _repository.Received(1).GetAllAsync(cts.Token);
    }

    [Fact]
    public async Task GetPageAsync_ReturnsMappedPage()
    {
        var products = new List<Product> { CreateProduct(1) };
        var repoResult = new PagedResult<Product>(products, Page: 1, PageSize: 10, TotalCount: 1);
        _repository.GetPageAsync(1, 10, Arg.Any<CancellationToken>()).Returns(repoResult);

        var result = await _sut.GetPageAsync(1, 10, CancellationToken.None);

        result.Items.Single().Id.ShouldBe(1);
        result.Page.ShouldBe(1);
        result.PageSize.ShouldBe(10);
        result.TotalCount.ShouldBe(1);
    }

    [Fact]
    public async Task GetByIdAsync_ProductExists_ReturnsMappedDto()
    {
        _repository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(CreateProduct(1));

        var result = await _sut.GetByIdAsync(1, CancellationToken.None);

        result.ShouldNotBeNull();
        result.Id.ShouldBe(1);
    }

    [Fact]
    public async Task GetByIdAsync_UnavailableProduct_ReturnsDtoWithIsAvailableFalse()
    {
        _repository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(CreateProduct(1, isAvailable: false));

        var result = await _sut.GetByIdAsync(1, CancellationToken.None);

        result.ShouldNotBeNull();
        result.IsAvailable.ShouldBeFalse();
    }

    [Fact]
    public async Task GetByIdAsync_ProductMissing_ReturnsNull()
    {
        _repository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns((Product?)null);

        var result = await _sut.GetByIdAsync(1, CancellationToken.None);

        result.ShouldBeNull();
    }

    [Fact]
    public async Task UpdateDescriptionAsync_ProductExists_UpdatesAndReturnsDto()
    {
        var product = CreateProduct(1);
        _repository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(product);

        var result = await _sut.UpdateDescriptionAsync(1, "Updated.", CancellationToken.None);

        result.ShouldNotBeNull();
        result.Description.ShouldBe("Updated.");
        await _repository.Received(1).UpdateAsync(product, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateDescriptionAsync_ProductMissing_ReturnsNullAndDoesNotUpdate()
    {
        _repository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns((Product?)null);

        var result = await _sut.UpdateDescriptionAsync(1, "Updated.", CancellationToken.None);

        result.ShouldBeNull();
        await _repository.DidNotReceive().UpdateAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateDescriptionAsync_PassesCancellationToken()
    {
        using var cts = new CancellationTokenSource();
        var product = CreateProduct(1);
        _repository.GetByIdAsync(1, cts.Token).Returns(product);

        await _sut.UpdateDescriptionAsync(1, "Updated.", cts.Token);

        await _repository.Received(1).UpdateAsync(product, cts.Token);
    }
}
