using System.ComponentModel.DataAnnotations;
using ProductCatalog.Domain;

namespace ProductCatalog.Api.Models;

/// <summary>
/// Request body for updating a product's description.
/// </summary>
public sealed class UpdateProductDescriptionRequest
{
    /// <summary>
    /// The new description. Null or whitespace clears it.
    /// </summary>
    [MaxLength(ProductConstraints.DescriptionMaxLength)]
    public string? Description { get; init; }
}
