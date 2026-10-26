using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using ProductCatalog.Api.Controllers;
using ProductCatalog.Api.Models;
using ProductCatalog.Application;
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

    private static ProductDto CreateDto(int id = 1) =>
        new(id, "Widget", "https://placehold.co/600x400?text=Widget", 9.99m, "A description.", true);

    [Fact]
    public async Task GetAll_ReturnsOkWithProducts()
    {
        var products = new List<ProductDto> { CreateDto(1), CreateDto(2) };
        _productService.GetAllAsync(Arg.Any<CancellationToken>()).Returns(products);

        var result = await _sut.GetAll(CancellationToken.None);

        var okResult = result.Result.ShouldBeOfType<OkObjectResult>();
        okResult.StatusCode.ShouldBe(StatusCodes.Status200OK);
        okResult.Value.ShouldBe(products);
    }

    [Fact]
    public async Task GetById_ProductExists_ReturnsOkWithProduct()
    {
        var product = CreateDto(1);
        _productService.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(product);

        var result = await _sut.GetById(1, CancellationToken.None);

        var okResult = result.Result.ShouldBeOfType<OkObjectResult>();
        okResult.StatusCode.ShouldBe(StatusCodes.Status200OK);
        okResult.Value.ShouldBe(product);
    }

    [Fact]
    public async Task GetById_ProductMissing_ReturnsNotFoundProblemDetails()
    {
        _productService.GetByIdAsync(999, Arg.Any<CancellationToken>()).Returns((ProductDto?)null);

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
        var page = new PagedResult<ProductDto>([CreateDto(1)], Page: 1, PageSize: 10, TotalCount: 1);
        _productService.GetPageAsync(1, 10, Arg.Any<CancellationToken>()).Returns(page);

        var result = await _sut.GetPage(new PaginationQuery(), CancellationToken.None);

        var okResult = result.Result.ShouldBeOfType<OkObjectResult>();
        okResult.StatusCode.ShouldBe(StatusCodes.Status200OK);
        okResult.Value.ShouldBe(page);
    }

    [Fact]
    public async Task GetPage_CustomQuery_ForwardsValuesToService()
    {
        var page = new PagedResult<ProductDto>([CreateDto(3)], Page: 3, PageSize: 5, TotalCount: 11);
        _productService.GetPageAsync(3, 5, Arg.Any<CancellationToken>()).Returns(page);

        var result = await _sut.GetPage(new PaginationQuery { Page = 3, PageSize = 5 }, CancellationToken.None);

        var okResult = result.Result.ShouldBeOfType<OkObjectResult>();
        okResult.Value.ShouldBe(page);
        await _productService.Received(1).GetPageAsync(3, 5, Arg.Any<CancellationToken>());
    }
}
