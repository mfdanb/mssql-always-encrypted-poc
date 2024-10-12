namespace MssqlAlwaysEncryptedPoc.Aspire.AppHost;

class Program
{
    private static void Main(string[] args)
    {
        var builder = DistributedApplication.CreateBuilder(args);

        var sqlPassword = builder.AddParameter("sql-password", secret: true);

        var sqlServer = builder.AddSqlServer("sql-server", sqlPassword)
            .WithDataVolume();

        var sqlDatabase = sqlServer.AddDatabase("sql-database");

        var migrations = builder.AddProject<Projects.MssqlAlwaysEncryptedPoc_ExampleDb_Migrations>("database-migrations")
            .WithReference(sqlDatabase)
            .WaitFor(sqlServer);

        var webapi = builder.AddProject<Projects.MssqlAlwaysEncryptedPoc_Web_Api>("web-api")
            .WithReference(sqlDatabase)
            .WaitForCompletion(migrations);

        builder.AddProject<Projects.MssqlAlwaysEncryptedPoc_Web_Blazor>("web-frontend")
            .WithExternalHttpEndpoints()
            .WithReference(webapi);


        builder.Build().Run();
    }
}