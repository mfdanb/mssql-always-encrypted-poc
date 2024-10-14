using Microsoft.EntityFrameworkCore;
using MssqlAlwaysEncryptedPoc.Aspire.ServiceDefaults;
using MssqlAlwaysEncryptedPoc.ExampleDb;

namespace MssqlAlwaysEncryptedPoc.Web.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.AddServiceDefaults();

        builder.Services.AddControllers();

        builder.Services.AddDbContext<ExampleDbContext>(optionsBuilder =>
        {
            optionsBuilder.UseSqlServer(
                builder.Configuration.GetConnectionString("sql-database")
                ?? throw new InvalidOperationException("ConnectionStrings__sql-database is missing")
            );
        });

        // Swagger/OpenAPI -- https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        app.MapDefaultEndpoints();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
