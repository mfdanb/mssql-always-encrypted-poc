using Microsoft.Data.SqlClient.AlwaysEncrypted.AzureKeyVaultProvider;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MssqlAlwaysEncryptedPoc.Aspire.ServiceDefaults;
using MssqlAlwaysEncryptedPoc.ExampleDb;
using Azure.Identity;

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
            optionsBuilder.UseSqlServer(builder.Configuration.GetConnectionString("ExampleDb") 
                ?? throw new InvalidOperationException("ConnectionStrings__ExampleDb is missing"));

            var configSection = builder.Configuration.GetSection("SqlColumnEncryption");

            var sqlColumnEncryptionKeyReaderServicePrincipal = new ClientSecretCredential(
                configSection.GetValue<string>("KeyVaultTenantId"),
                configSection.GetValue<string>("KeyVaultClientId"),
                configSection.GetValue<string>("KeyVaultClientSecret")
            );

            var encryptionKeyStores = new Dictionary<string, SqlColumnEncryptionKeyStoreProvider>
            {
                { SqlColumnEncryptionAzureKeyVaultProvider.ProviderName, new SqlColumnEncryptionAzureKeyVaultProvider(sqlColumnEncryptionKeyReaderServicePrincipal) }
            };

            SqlConnection.RegisterColumnEncryptionKeyStoreProviders(encryptionKeyStores);
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
