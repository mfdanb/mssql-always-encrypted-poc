namespace MssqlAlwaysEncryptedPoc.ExampleDb;

public class Example
{
    public required Guid Id { get; set; } = Guid.NewGuid();
    public required string Description { get; set; }
    //public string? SensitiveText { get; set; }
    //public uint? SensitiveNumber { get; set; }
}