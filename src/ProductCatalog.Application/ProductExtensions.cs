using ProductCatalog.Domain;

namespace ProductCatalog.Application;

public static class ProductExtensions
{
    public static ProductDto ToDto(this Product product) =>
        new(product.Id, product.Name, product.ImgUri, product.Price, product.Description);
}
