using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using ProductCatalog.Api.Controllers;
using ProductCatalog.Api.Models;
using ProductCatalog.Application;
using ProductCatalog.Domain;
using Shouldly;

namespace ProductCatalog.UnitTests.Api;

public class ProductsControllerTests
{
    private readonly IProductService _productService = Substitute.For<IProductService>();
    private readonly ProductsController _sut;

    public ProductsControllerTests()
    {
        _sut = new ProductsController(_productService)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
    }

    private static Product CreateProduct(int id = 1, string? description = "A description.") =>
        new(id, "Widget", "https://placehold.co/600x400?text=Widget", 9.99m, description);

    [Fact]
    public async Task GetAll_ReturnsOkWithProducts()
    {
        var products = new List<Product> { CreateProduct(1), CreateProduct(2) };
        _productService.GetAllAsync(Arg.Any<CancellationToken>()).Returns(products);

        var result = await _sut.GetAll(CancellationToken.None);

        var okResult = result.Result.ShouldBeOfType<OkObjectResult>();
        okResult.StatusCode.ShouldBe(StatusCodes.Status200OK);
        var dtos = okResult.Value.ShouldBeOfType<List<ProductDto>>();
        dtos.Select(d => d.Id).ShouldBe([1, 2]);
    }

    [Fact]
    public async Task GetById_ProductExists_ReturnsOkWithProduct()
    {
        var product = CreateProduct(1);
        _productService.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(product);

        var result = await _sut.GetById(1, CancellationToken.None);

        var okResult = result.Result.ShouldBeOfType<OkObjectResult>();
        okResult.StatusCode.ShouldBe(StatusCodes.Status200OK);
        okResult.Value.ShouldBe(product.ToDto());
    }

    [Fact]
    public async Task GetById_ProductMissing_ReturnsNotFoundProblemDetails()
    {
        _productService.GetByIdAsync(999, Arg.Any<CancellationToken>()).Returns((IReadOnlyProduct?)null);

        var result = await _sut.GetById(999, CancellationToken.None);

        var objectResult = result.Result.ShouldBeOfType<ObjectResult>();
        objectResult.StatusCode.ShouldBe(StatusCodes.Status404NotFound);
        var problemDetails = objectResult.Value.ShouldBeOfType<ProblemDetails>();
        problemDetails.Title.ShouldBe("Product not found");
        problemDetails.Detail.ShouldBe("No product exists with id 999.");
    }

    [Fact]
    public async Task GetPage_DefaultQuery_ForwardsDefaultsToService()
    {
        var page = new PagedResult<IReadOnlyProduct>([CreateProduct(1)], Page: 1, PageSize: 10, TotalCount: 1);
        _productService.GetPageAsync(1, 10, Arg.Any<CancellationToken>()).Returns(page);

        var result = await _sut.GetPage(new PaginationQuery(), CancellationToken.None);

        var okResult = result.Result.ShouldBeOfType<OkObjectResult>();
        okResult.StatusCode.ShouldBe(StatusCodes.Status200OK);
        var dto = okResult.Value.ShouldBeOfType<PagedResult<ProductDto>>();
        dto.Items.Single().Id.ShouldBe(1);
    }

    [Fact]
    public async Task GetPage_CustomQuery_ForwardsValuesToService()
    {
        var page = new PagedResult<IReadOnlyProduct>([CreateProduct(3)], Page: 3, PageSize: 5, TotalCount: 11);
        _productService.GetPageAsync(3, 5, Arg.Any<CancellationToken>()).Returns(page);

        var result = await _sut.GetPage(new PaginationQuery { Page = 3, PageSize = 5 }, CancellationToken.None);

        var okResult = result.Result.ShouldBeOfType<OkObjectResult>();
        var dto = okResult.Value.ShouldBeOfType<PagedResult<ProductDto>>();
        dto.Items.Single().Id.ShouldBe(3);
        await _productService.Received(1).GetPageAsync(3, 5, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task PatchDescription_ProductExists_ReturnsOkWithUpdatedProduct()
    {
        var updated = CreateProduct(1, description: "Updated.");
        _productService.UpdateDescriptionAsync(1, "Updated.", Arg.Any<CancellationToken>()).Returns(updated);

        var result = await _sut.PatchDescription(
            1, new UpdateProductDescriptionRequest { Description = "Updated." }, CancellationToken.None);

        var okResult = result.Result.ShouldBeOfType<OkObjectResult>();
        okResult.StatusCode.ShouldBe(StatusCodes.Status200OK);
        okResult.Value.ShouldBe(updated.ToDto());
    }

    [Fact]
    public async Task PatchDescription_ProductMissing_ReturnsNotFoundProblemDetails()
    {
        _productService.UpdateDescriptionAsync(999, "x", Arg.Any<CancellationToken>()).Returns((IReadOnlyProduct?)null);

        var result = await _sut.PatchDescription(
            999, new UpdateProductDescriptionRequest { Description = "x" }, CancellationToken.None);

        var objectResult = result.Result.ShouldBeOfType<ObjectResult>();
        objectResult.StatusCode.ShouldBe(StatusCodes.Status404NotFound);
        var problemDetails = objectResult.Value.ShouldBeOfType<ProblemDetails>();
        problemDetails.Title.ShouldBe("Product not found");
        problemDetails.Detail.ShouldBe("No product exists with id 999.");
    }
}
