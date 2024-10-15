using Microsoft.EntityFrameworkCore;

namespace MssqlAlwaysEncryptedPoc.ExampleDb;

public class EncryptedExampleDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<EncryptedExample> EncryptedExamples { get; set; }
}
