using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using MssqlAlwaysEncryptedPoc.ExampleDb;
using OpenTelemetry.Trace;
using System.Diagnostics;

namespace MssqlAlwaysEncryptedPoc.ExampleDb.Migrations;

public class MigrationService(
    IServiceProvider serviceProvider,
    IHostApplicationLifetime hostApplicationLifetime,
    ILogger<MigrationService> logger) : BackgroundService
{
    public const string ActivitySourceName = nameof(MigrationService);
    private static readonly ActivitySource _activitySource = new(ActivitySourceName);

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var activity = _activitySource.StartActivity("Initializing database", ActivityKind.Client);

        var sw = Stopwatch.StartNew();

        try
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ExampleDbContext>();

            await EnsureDatabaseIsCreated(dbContext, cancellationToken);
            await RunMigrations(dbContext, cancellationToken);
            await SeedExamples(dbContext, cancellationToken);
        }
        catch (Exception ex)
        {
            activity?.RecordException(ex);
            throw;
        }

        logger.LogInformation("Database initialization completed after {ElapsedMilliseconds}ms", sw.ElapsedMilliseconds);

        hostApplicationLifetime.StopApplication();
    }

    private async Task EnsureDatabaseIsCreated(ExampleDbContext dbContext, CancellationToken cancellationToken)
    {
        var dbCreator = dbContext.GetService<IRelationalDatabaseCreator>();

        var strategy = dbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            if (!await dbCreator.ExistsAsync(cancellationToken))
            {
                logger.LogInformation("Creating database");

                await dbCreator.CreateAsync(cancellationToken);
            }
        });
    }

    private async Task RunMigrations(ExampleDbContext dbContext, CancellationToken cancellationToken)
    {
        logger.LogInformation("Migrating database");

        var strategy = dbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(dbContext.Database.MigrateAsync, cancellationToken);
    }

    private async Task SeedExamples(ExampleDbContext dbContext, CancellationToken cancellationToken)
    {
        logger.LogInformation("Seeding database");

        if (!dbContext.Examples.Any())
        {
            List<Example> examples = [
                new() { Id = Guid.NewGuid(), Description = "Description 1" },
                new() { Id = Guid.NewGuid(), Description = "Description 2" },
                new() { Id = Guid.NewGuid(), Description = "Description 3" },
            ];

            await dbContext.Examples.AddRangeAsync(examples, cancellationToken);

            logger.LogInformation("Seeding {count} examples", examples.Count);

            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
