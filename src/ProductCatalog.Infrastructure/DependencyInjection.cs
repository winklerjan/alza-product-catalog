using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using ProductCatalog.Application;
using ProductCatalog.Infrastructure.Mock;
using ProductCatalog.Infrastructure.Persistence;

namespace ProductCatalog.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOptions<PersistenceOptions>()
            .Bind(configuration.GetSection(PersistenceOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        var provider = configuration.GetSection(PersistenceOptions.SectionName).Get<PersistenceOptions>()?.Provider
            ?? throw new OptionsValidationException(
                PersistenceOptions.SectionName, typeof(PersistenceOptions),
                [$"Missing configuration section '{PersistenceOptions.SectionName}'."]);

        switch (provider)
        {
            case PersistenceProvider.Mock:
                services.AddSingleton<IProductRepository, InMemoryProductRepository>();
                break;
            case PersistenceProvider.SqlServer:
                var connectionString = configuration.GetConnectionString("ProductCatalog")
                    ?? throw new InvalidOperationException("Missing connection string 'ProductCatalog'.");
                services.AddDbContext<ProductCatalogDbContext>(options => options.UseSqlServer(connectionString));
                services.AddScoped<IProductRepository, SqlProductRepository>();
                services.AddHostedService<MigrateOnStartupHostedService>();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(configuration), provider, "Unsupported persistence provider.");
        }

        return services;
    }
}
