using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace ProductCatalog.Infrastructure.Persistence;

/// <summary>
/// Applies pending EF Core migrations on startup when <see cref="PersistenceOptions.MigrateOnStartup"/> is set.
/// </summary>
internal sealed class MigrateOnStartupHostedService(IServiceProvider serviceProvider, IOptions<PersistenceOptions> options)
    : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (!options.Value.MigrateOnStartup)
        {
            return;
        }

        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ProductCatalogDbContext>();
        await dbContext.Database.MigrateAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
