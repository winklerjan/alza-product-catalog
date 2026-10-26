using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductCatalog.Domain;
using ProductCatalog.Infrastructure.Seed;

namespace ProductCatalog.Infrastructure.Persistence.Configurations;

public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id).ValueGeneratedNever();

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(ProductConstraints.NameMaxLength);

        builder.Property(p => p.ImgUri)
            .IsRequired()
            .HasMaxLength(ProductConstraints.ImgUriMaxLength);

        builder.Property(p => p.Price)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(p => p.Description)
            .HasMaxLength(ProductConstraints.DescriptionMaxLength);

        builder.Property(p => p.IsAvailable).IsRequired();

        builder.HasData(ProductSeedData.All);
    }
}
