# How to run

1. Install the latest .NET9 SDK (at least SDK 9.0.100-rc.2) from https://dotnet.microsoft.com/en-us/download/dotnet/9.0

2. Install the Aspire workload:
```
dotnet workload update
dotnet workload install aspire
```

3. Make sure you have docker installed and running

4. Add your database password to the user secrets file of the `MssqlAlwaysEncryptedPoc.Aspire.AppHost` project

```json
{
  "Parameters": {
    "SqlPassword": "your password",
  }
}
```

5. Run `MssqlAlwaysEncryptedPoc.Aspire.AppHost`

The AppHost will automatically initialize & seed the database, and launch the web API.

# How to add DB migrations

```
cd .\MssqlAlwaysEncryptedPoc.Data.DatabaseMigrator\
dotnet ef migrations add <name>
```

# How to encrypt a database column (manually for now)

1. Connect to your database from SQL Server Management Studio 20 (https://learn.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms?view=sql-server-ver16)

2. Find the Examples table, right click on it, select Encrypt Columns, and click through the wizard.

3. Encrypt the Description column with a Deterministic type.

4. Store the master key in a Key Vault of your choice. Finish the wizard.

5. Create a service principal, and give it read access over the keys inside your Key Vault

6. Store the credentials of this service principal in the user secrets file of the `MssqlAlwaysEncryptedPoc.Aspire.AppHost` project:

```json
{
  "Parameters": {
    "SqlPassword": "your password",
    "SqlColumnEncryptionKeyVaultTenantId": "tenant id",
    "SqlColumnEncryptionKeyVaultClientId": "client id",
    "SqlColumnEncryptionKeyVaultClientSecret": "client secret"
  }
}
```

The Web.Api should now be able to decrypt your column.