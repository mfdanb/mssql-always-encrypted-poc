namespace MssqlAlwaysEncryptedPoc.Aspire.AppHost;

class Program
{
    private static void Main(string[] args)
    {
        var builder = DistributedApplication.CreateBuilder(args);

        var sqlPassword = builder.AddParameter("SqlPassword", secret: true);
        var sqlColumnEncryptionKeyVaultTenantId = builder.AddParameter("SqlColumnEncryptionKeyVaultTenantId", secret: true);
        var sqlColumnEncryptionKeyVaultClientId = builder.AddParameter("SqlColumnEncryptionKeyVaultClientId", secret: true);
        var sqlColumnEncryptionKeyVaultClientSecret = builder.AddParameter("SqlColumnEncryptionKeyVaultClientSecret", secret: true);

        var sqlServer = builder.AddSqlServer("SqlServer", sqlPassword)
            .WithDataVolume();

        var exampleDb = sqlServer.AddDatabase("ExampleDb");

        var migrations = builder.AddProject<Projects.MssqlAlwaysEncryptedPoc_ExampleDb_Migrations>("ExampleDb-Migrations")
            .WithReference(exampleDb, "ExampleDb")
            .WaitFor(sqlServer);

        var webapi = builder.AddProject<Projects.MssqlAlwaysEncryptedPoc_Web_Api>("WebApi")
            .WaitForCompletion(migrations)
            .WithEnvironment($"ConnectionStrings__{exampleDb.Resource.Name}", $"{exampleDb.Resource};Column Encryption Setting=Enabled")
            .WithEnvironment($"SqlColumnEncryption__KeyVaultTenantId", sqlColumnEncryptionKeyVaultTenantId)
            .WithEnvironment($"SqlColumnEncryption__KeyVaultClientId", sqlColumnEncryptionKeyVaultClientId)
            .WithEnvironment($"SqlColumnEncryption__KeyVaultClientSecret", sqlColumnEncryptionKeyVaultClientSecret);

        builder.Build().Run();
    }
}