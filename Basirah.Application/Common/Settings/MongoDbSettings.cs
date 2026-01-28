namespace Basirah.Application.Common.Settings;

/// <summary>
/// MongoDB database configuration.
/// Bound to "MongoDbSettings" section in appsettings.json.
/// </summary>
public class MongoDbSettings
{
    public const string SectionName = "MongoDbSettings";

    public string ConnectionString { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
}
