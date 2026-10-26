namespace ProductCatalog.Domain;

public sealed class Product
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public string ImgUri { get; private set; }
    public decimal Price { get; private set; }
    public string? Description { get; private set; }

    public Product(int id, string name, string imgUri, decimal price, string? description)
    {
        Id = id;
        Name = ValidateName(name);
        ImgUri = ValidateImgUri(imgUri);
        Price = ValidatePrice(price);
        Description = NormalizeDescription(description);
    }

    private Product()
    {
        Name = string.Empty;
        ImgUri = string.Empty;
    }

    public void UpdateDescription(string? description)
    {
        Description = NormalizeDescription(description);
    }

    private static string ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name is required.", nameof(name));
        }

        var trimmed = name.Trim();
        if (trimmed.Length > ProductConstraints.NameMaxLength)
        {
            throw new ArgumentException(
                $"Name must be at most {ProductConstraints.NameMaxLength} characters.", nameof(name));
        }

        return trimmed;
    }

    private static string ValidateImgUri(string imgUri)
    {
        if (string.IsNullOrWhiteSpace(imgUri))
        {
            throw new ArgumentException("ImgUri is required.", nameof(imgUri));
        }

        if (imgUri.Length > ProductConstraints.ImgUriMaxLength)
        {
            throw new ArgumentException(
                $"ImgUri must be at most {ProductConstraints.ImgUriMaxLength} characters.", nameof(imgUri));
        }

        if (!Uri.TryCreate(imgUri, UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            throw new ArgumentException("ImgUri must be an absolute http or https URI.", nameof(imgUri));
        }

        return imgUri;
    }

    private static decimal ValidatePrice(decimal price)
    {
        if (price < 0)
        {
            throw new ArgumentException("Price must not be negative.", nameof(price));
        }

        if (decimal.Round(price, 2) != price)
        {
            throw new ArgumentException("Price must have at most 2 decimal places.", nameof(price));
        }

        return price;
    }

    private static string? NormalizeDescription(string? description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            return null;
        }

        var trimmed = description.Trim();
        if (trimmed.Length > ProductConstraints.DescriptionMaxLength)
        {
            throw new ArgumentException(
                $"Description must be at most {ProductConstraints.DescriptionMaxLength} characters.",
                nameof(description));
        }

        return trimmed;
    }
}
