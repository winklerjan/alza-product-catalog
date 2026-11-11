namespace ProductCatalog.Domain;

/// <summary>
/// Read-only view of a <see cref="Product"/>, exposing no mutation members.
/// </summary>
public interface IReadOnlyProduct
{
    int Id { get; }
    string Name { get; }
    string ImgUri { get; }
    decimal Price { get; }
    string? Description { get; }
    bool IsAvailable { get; }
}
