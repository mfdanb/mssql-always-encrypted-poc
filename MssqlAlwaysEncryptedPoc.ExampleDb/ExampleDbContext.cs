using Microsoft.EntityFrameworkCore;

namespace MssqlAlwaysEncryptedPoc.ExampleDb;

public class ExampleDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Example> Examples { get; set; }
}
