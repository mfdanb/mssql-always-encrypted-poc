using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MssqlAlwaysEncryptedPoc.ExampleDb.Configurations;

class EncryptedExampleConfiguration : IEntityTypeConfiguration<EncryptedExample>
{
    public void Configure(EntityTypeBuilder<EncryptedExample> builder)
    {
        builder.ToTable("encrypted_examples");
    }
}
