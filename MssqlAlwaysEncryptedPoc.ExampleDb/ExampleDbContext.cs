using Microsoft.EntityFrameworkCore;

namespace MssqlAlwaysEncryptedPoc.ExampleDb;

public class ExampleDbContext : DbContext
{
    public ExampleDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Example> Examples { get; set; }
}
