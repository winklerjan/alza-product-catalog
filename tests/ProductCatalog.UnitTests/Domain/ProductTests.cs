using ProductCatalog.Domain;
using Shouldly;

namespace ProductCatalog.UnitTests.Domain;

public class ProductTests
{
    private const string ValidName = "Wireless Mouse";
    private const string ValidImgUri = "https://placehold.co/600x400?text=Mouse";
    private const decimal ValidPrice = 19.99m;

    [Fact]
    public void Constructor_ValidArguments_SetsProperties()
    {
        var product = new Product(1, ValidName, ValidImgUri, ValidPrice, "A description.");

        product.Id.ShouldBe(1);
        product.Name.ShouldBe(ValidName);
        product.ImgUri.ShouldBe(ValidImgUri);
        product.Price.ShouldBe(ValidPrice);
        product.Description.ShouldBe("A description.");
        product.IsAvailable.ShouldBeTrue();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Constructor_IsAvailableExplicit_SetsProperty(bool isAvailable)
    {
        var product = new Product(1, ValidName, ValidImgUri, ValidPrice, null, isAvailable);

        product.IsAvailable.ShouldBe(isAvailable);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_NameNullOrWhitespace_Throws(string? name)
    {
        Should.Throw<ArgumentException>(() => new Product(1, name!, ValidImgUri, ValidPrice, null));
    }

    [Fact]
    public void Constructor_NameWithSurroundingWhitespace_Trims()
    {
        var product = new Product(1, "  Padded Name  ", ValidImgUri, ValidPrice, null);

        product.Name.ShouldBe("Padded Name");
    }

    [Fact]
    public void Constructor_NameExceedsMaxLength_Throws()
    {
        var tooLong = new string('a', ProductConstraints.NameMaxLength + 1);

        Should.Throw<ArgumentException>(() => new Product(1, tooLong, ValidImgUri, ValidPrice, null));
    }

    [Fact]
    public void Constructor_NameAtMaxLength_Succeeds()
    {
        var atLimit = new string('a', ProductConstraints.NameMaxLength);

        var product = new Product(1, atLimit, ValidImgUri, ValidPrice, null);

        product.Name.ShouldBe(atLimit);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("not-a-uri")]
    [InlineData("ftp://example.com/image.png")]
    [InlineData("/relative/path.png")]
    public void Constructor_ImgUriInvalid_Throws(string? imgUri)
    {
        Should.Throw<ArgumentException>(() => new Product(1, ValidName, imgUri!, ValidPrice, null));
    }

    [Theory]
    [InlineData("http://example.com/image.png")]
    [InlineData("https://example.com/image.png")]
    public void Constructor_ImgUriValidScheme_Succeeds(string imgUri)
    {
        var product = new Product(1, ValidName, imgUri, ValidPrice, null);

        product.ImgUri.ShouldBe(imgUri);
    }

    [Fact]
    public void Constructor_ImgUriExceedsMaxLength_Throws()
    {
        var tooLong = "https://example.com/" + new string('a', ProductConstraints.ImgUriMaxLength);

        Should.Throw<ArgumentException>(() => new Product(1, ValidName, tooLong, ValidPrice, null));
    }

    [Fact]
    public void Constructor_PriceNegative_Throws()
    {
        Should.Throw<ArgumentException>(() => new Product(1, ValidName, ValidImgUri, -0.01m, null));
    }

    [Fact]
    public void Constructor_PriceWithMoreThanTwoDecimalPlaces_Throws()
    {
        Should.Throw<ArgumentException>(() => new Product(1, ValidName, ValidImgUri, 1.999m, null));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(19.99)]
    public void Constructor_PriceValid_Succeeds(double price)
    {
        var product = new Product(1, ValidName, ValidImgUri, (decimal)price, null);

        product.Price.ShouldBe((decimal)price);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_DescriptionNullOrWhitespace_BecomesNull(string? description)
    {
        var product = new Product(1, ValidName, ValidImgUri, ValidPrice, description);

        product.Description.ShouldBeNull();
    }

    [Fact]
    public void Constructor_DescriptionWithSurroundingWhitespace_Trims()
    {
        var product = new Product(1, ValidName, ValidImgUri, ValidPrice, "  Padded  ");

        product.Description.ShouldBe("Padded");
    }

    [Fact]
    public void Constructor_DescriptionExceedsMaxLength_Throws()
    {
        var tooLong = new string('a', ProductConstraints.DescriptionMaxLength + 1);

        Should.Throw<ArgumentException>(() => new Product(1, ValidName, ValidImgUri, ValidPrice, tooLong));
    }

    [Fact]
    public void UpdateDescription_ValidValue_UpdatesDescription()
    {
        var product = new Product(1, ValidName, ValidImgUri, ValidPrice, "Original.");

        product.UpdateDescription("Updated.");

        product.Description.ShouldBe("Updated.");
    }

    [Fact]
    public void UpdateDescription_Whitespace_ClearsDescription()
    {
        var product = new Product(1, ValidName, ValidImgUri, ValidPrice, "Original.");

        product.UpdateDescription("   ");

        product.Description.ShouldBeNull();
    }

    [Fact]
    public void UpdateDescription_ExceedsMaxLength_Throws()
    {
        var product = new Product(1, ValidName, ValidImgUri, ValidPrice, "Original.");
        var tooLong = new string('a', ProductConstraints.DescriptionMaxLength + 1);

        Should.Throw<ArgumentException>(() => product.UpdateDescription(tooLong));
    }
}
