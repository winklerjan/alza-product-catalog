namespace ProductCatalog.Api.Models;

/// <summary>
/// Product representation returned by the API.
/// </summary>
public sealed record ProductDto(int Id, string Name, string ImgUri, decimal Price, string? Description, bool IsAvailable);
