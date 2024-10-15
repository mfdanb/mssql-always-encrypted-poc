using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
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
        using var activity = _activitySource.StartActivity("Initializing databases", ActivityKind.Client);

        var sw = Stopwatch.StartNew();

        try
        {
            using var scope = serviceProvider.CreateScope();
            var exampleDbContext = scope.ServiceProvider.GetRequiredService<ExampleDbContext>();
            var encryptedExampleDbContext = scope.ServiceProvider.GetRequiredService<EncryptedExampleDbContext>();

            await EnsureDatabaseIsCreated(exampleDbContext, encryptedExampleDbContext, cancellationToken);
            await RunMigrations(exampleDbContext, encryptedExampleDbContext, cancellationToken);
            await SeedExamples(exampleDbContext, encryptedExampleDbContext, cancellationToken);
        }
        catch (Exception ex)
        {
            activity?.RecordException(ex);
            throw;
        }

        logger.LogInformation("Database initialization completed after {ElapsedMilliseconds}ms",
            sw.ElapsedMilliseconds);

        hostApplicationLifetime.StopApplication();
    }

    private async Task EnsureDatabaseIsCreated(ExampleDbContext exampleDbContext,
        EncryptedExampleDbContext encryptedExampleDbContext,
        CancellationToken cancellationToken)
    {
        var exampleDbCreator = exampleDbContext.GetService<IRelationalDatabaseCreator>();
        var encryptedExampleDbCreator = encryptedExampleDbContext.GetService<IRelationalDatabaseCreator>();

        var exampleStrategy = exampleDbContext.Database.CreateExecutionStrategy();
        var encryptedExampleStrategy = encryptedExampleDbContext.Database.CreateExecutionStrategy();

        await exampleStrategy.ExecuteAsync(async () =>
        {
            if (!await exampleDbCreator.ExistsAsync(cancellationToken))
            {
                logger.LogInformation("Creating database");

                await exampleDbCreator.CreateAsync(cancellationToken);
            }
        });

        await encryptedExampleStrategy.ExecuteAsync(async () =>
        {
            if (!await encryptedExampleDbCreator.ExistsAsync(cancellationToken))
            {
                logger.LogInformation("Creating encrypted database");

                await encryptedExampleDbCreator.CreateAsync(cancellationToken);
            }
        });
    }

    private async Task RunMigrations(ExampleDbContext exampleDbContext,
        EncryptedExampleDbContext encryptedExampleDbContext,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Migrating databases");

        var exampleStrategy = exampleDbContext.Database.CreateExecutionStrategy();
        var encryptedExampleStrategy = encryptedExampleDbContext.Database.CreateExecutionStrategy();

        await exampleStrategy.ExecuteAsync(exampleDbContext.Database.MigrateAsync, cancellationToken);
        await encryptedExampleStrategy.ExecuteAsync(encryptedExampleDbContext.Database.MigrateAsync, cancellationToken);
    }

    private async Task SeedExamples(ExampleDbContext exampleDbContext,
        EncryptedExampleDbContext encryptedExampleDbContext,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Seeding database");

        if (!exampleDbContext.Examples.Any() && !encryptedExampleDbContext.EncryptedExamples.Any())
        {
            List<Example> examples = [];
            List<EncryptedExample> encryptedExamples = [];

            for (int i = 1; i <= 10000; i++)
            {
                examples.Add(new Example { Id = Guid.NewGuid(), Description = $"Description {i}" });
                encryptedExamples.Add(new EncryptedExample { Id = Guid.NewGuid(), Description = $"Description {i}" });
            }

            await exampleDbContext.Examples.AddRangeAsync(examples, cancellationToken);
            await encryptedExampleDbContext.EncryptedExamples.AddRangeAsync(encryptedExamples, cancellationToken);

            logger.LogInformation("Seeding {Count} examples", examples.Count);
            logger.LogInformation("Seeding {Count} encrypted examples", encryptedExamples.Count);

            await exampleDbContext.SaveChangesAsync(cancellationToken);
            await encryptedExampleDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}