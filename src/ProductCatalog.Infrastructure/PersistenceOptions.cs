using System.ComponentModel.DataAnnotations;

namespace ProductCatalog.Infrastructure;

public enum PersistenceProvider
{
    Mock,
    SqlServer
}

public sealed class PersistenceOptions
{
    public const string SectionName = "Persistence";

    [Required]
    public PersistenceProvider Provider { get; set; }

    public bool MigrateOnStartup { get; set; }
}
