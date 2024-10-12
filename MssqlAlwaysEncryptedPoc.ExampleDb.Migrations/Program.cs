using Microsoft.EntityFrameworkCore;
using MssqlAlwaysEncryptedPoc.Aspire.ServiceDefaults;

namespace MssqlAlwaysEncryptedPoc.ExampleDb.Migrations;

// Documentation: https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/projects
public class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        builder.AddServiceDefaults();

        builder.Services.AddHostedService<MigrationService>();

        builder.Services.AddOpenTelemetry()
            .WithTracing(tracing => tracing.AddSource(MigrationService.ActivitySourceName));

        builder.Services.AddDbContextPool<ExampleDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("sql-database"), sqlOptions =>
            {
                sqlOptions.MigrationsAssembly(typeof(Program).Assembly.GetName().Name);
            }));

        //builder.EnrichSqlServerDbContext<ExampleDbContext>(); // need the aspire version of efcore to use this

        var host = builder.Build();
        host.Run();
    }
}