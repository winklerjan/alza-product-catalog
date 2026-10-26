using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using ProductCatalog.Application;

namespace ProductCatalog.Api.Controllers;

/// <summary>
/// Operations for querying and updating products in the catalog.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/products")]
public sealed class ProductsController(IProductService productService) : ControllerBase
{
    /// <summary>
    /// Lists all available products.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <response code="200">The available products.</response>
    [HttpGet]
    [MapToApiVersion("1.0")]
    [ProducesResponseType<IReadOnlyList<ProductDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ProductDto>>> GetAll(CancellationToken ct)
    {
        var products = await productService.GetAllAsync(ct);
        return Ok(products);
    }

    /// <summary>
    /// Retrieves a single product by id.
    /// </summary>
    /// <param name="id">The product id.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <response code="200">The requested product.</response>
    /// <response code="404">No product exists with the given id.</response>
    [HttpGet("{id:int}")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType<ProductDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDto>> GetById(int id, CancellationToken ct)
    {
        var product = await productService.GetByIdAsync(id, ct);
        if (product is null)
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Product not found",
                detail: $"No product exists with id {id}.");
        }

        return Ok(product);
    }
}
