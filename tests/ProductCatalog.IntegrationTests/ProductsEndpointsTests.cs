using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using ProductCatalog.Application;
using Shouldly;

namespace ProductCatalog.IntegrationTests;

public sealed class ProductsEndpointsTests : IDisposable
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public ProductsEndpointsTests()
    {
        _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Development");
            builder.UseSetting("Persistence:Provider", "Mock");
        });
        _client = _factory.CreateClient();
    }

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    [Fact]
    public async Task GetAllV1_ReturnsAllAvailableSeedProducts()
    {
        var ct = TestContext.Current.CancellationToken;
        var products = await _client.GetFromJsonAsync<List<ProductDto>>("/api/v1/products", ct);

        products.ShouldNotBeNull();
        products.Count.ShouldBe(22);
        products.ShouldAllBe(p => p.IsAvailable);
    }

    [Fact]
    public async Task GetPageV2_DefaultQuery_ReturnsTenItemsWithCorrectTotals()
    {
        var ct = TestContext.Current.CancellationToken;
        var page = await _client.GetFromJsonAsync<PagedResult<ProductDto>>("/api/v2/products", ct);

        page.ShouldNotBeNull();
        page.Items.Count.ShouldBe(10);
        page.TotalCount.ShouldBe(22);
        page.TotalPages.ShouldBe(3);
    }

    [Fact]
    public async Task GetPageV2_CustomPageAndSize_ReturnsRequestedSlice()
    {
        var ct = TestContext.Current.CancellationToken;
        var page = await _client.GetFromJsonAsync<PagedResult<ProductDto>>("/api/v2/products?page=2&pageSize=5", ct);

        page.ShouldNotBeNull();
        page.Items.Count.ShouldBe(5);
        page.Page.ShouldBe(2);
        page.PageSize.ShouldBe(5);
    }

    [Fact]
    public async Task GetPageV2_PastTheEnd_ReturnsEmptyItems()
    {
        var ct = TestContext.Current.CancellationToken;
        var page = await _client.GetFromJsonAsync<PagedResult<ProductDto>>("/api/v2/products?page=100&pageSize=10", ct);

        page.ShouldNotBeNull();
        page.Items.ShouldBeEmpty();
    }

    [Theory]
    [InlineData("/api/v2/products?page=0")]
    [InlineData("/api/v2/products?pageSize=101")]
    public async Task GetPageV2_InvalidPaging_ReturnsBadRequestProblemDetails(string url)
    {
        var response = await _client.GetAsync(url, TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        response.Content.Headers.ContentType?.MediaType.ShouldBe("application/problem+json");
    }

    [Fact]
    public async Task GetById_ExistingId_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/v1/products/1", TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetById_UnknownId_ReturnsNotFoundProblemDetails()
    {
        var response = await _client.GetAsync("/api/v1/products/999", TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        response.Content.Headers.ContentType?.MediaType.ShouldBe("application/problem+json");
    }

    [Fact]
    public async Task PatchDescription_ExistingId_UpdatesAndIsVisibleOnFollowingGet()
    {
        var ct = TestContext.Current.CancellationToken;
        var patchResponse = await _client.PatchAsJsonAsync(
            "/api/v1/products/1", new { description = "Updated via integration test." }, ct);
        patchResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

        var product = await _client.GetFromJsonAsync<ProductDto>("/api/v1/products/1", ct);

        product.ShouldNotBeNull();
        product.Description.ShouldBe("Updated via integration test.");
    }

    [Fact]
    public async Task PatchDescription_TooLong_ReturnsBadRequest()
    {
        var response = await _client.PatchAsJsonAsync(
            "/api/v1/products/1", new { description = new string('a', 4001) }, TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task PatchDescription_UnknownId_ReturnsNotFound()
    {
        var response = await _client.PatchAsJsonAsync(
            "/api/v1/products/999", new { description = "x" }, TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UnsupportedApiVersion_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/v3/products", TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Theory]
    [InlineData("/swagger/v1/swagger.json")]
    [InlineData("/swagger/v2/swagger.json")]
    public async Task SwaggerDocument_ReturnsOk(string url)
    {
        var response = await _client.GetAsync(url, TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task SwaggerV2Document_MentionsPagingParameters()
    {
        var json = await _client.GetStringAsync("/swagger/v2/swagger.json", TestContext.Current.CancellationToken);

        json.ShouldContain("\"page\"");
        json.ShouldContain("\"pageSize\"");
    }

    [Fact]
    public async Task SwaggerV1Document_DoesNotContainV2List()
    {
        var json = await _client.GetStringAsync("/swagger/v1/swagger.json", TestContext.Current.CancellationToken);

        json.ShouldNotContain("\"pageSize\"");
    }
}
