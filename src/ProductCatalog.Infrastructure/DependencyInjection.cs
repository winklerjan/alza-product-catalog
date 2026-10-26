using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ProductCatalog.Application;
using ProductCatalog.Infrastructure.Mock;

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

        services.AddSingleton<IProductRepository>(provider switch
        {
            PersistenceProvider.Mock => new InMemoryProductRepository(),
            PersistenceProvider.SqlServer => throw new NotSupportedException(
                "SQL Server persistence is not implemented yet."),
            _ => throw new ArgumentOutOfRangeException(nameof(configuration), provider, "Unsupported persistence provider.")
        });

        return services;
    }
}
