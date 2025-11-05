using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using PetCafe.Application;

namespace PetCafe.Infrastructures;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;
    private readonly IMongoClient _client;

    // Static constructor to ensure Guid serializer is registered once
    static MongoDbContext()
    {
        // Set Guid representation to Standard (recommended for new projects)
        // This fixes the "GuidSerializer cannot serialize a Guid when GuidRepresentation is Unspecified" error
        // RegisterSerializer will replace any existing serializer, so it's safe to call multiple times
        var guidSerializer = new GuidSerializer(GuidRepresentation.Standard);
        BsonSerializer.RegisterSerializer(guidSerializer);
    }

    public MongoDbContext(AppSettings appSettings)
    {
        var connectionString = appSettings.ConnectionStrings.MongoDbConnection
            ?? throw new InvalidOperationException("MongoDB connection string not found.");

        // Configure MongoDB client settings
        var settings = MongoClientSettings.FromConnectionString(connectionString);

        _client = new MongoClient(settings);

        // Extract database name from connection string or use default
        var databaseName = ExtractDatabaseName(connectionString) ?? "pet_cafe_db";
        _database = _client.GetDatabase(databaseName);
    }

    public IMongoDatabase Database => _database;
    public IMongoClient Client => _client;

    public IMongoCollection<T> GetCollection<T>(string? collectionName = null)
    {
        collectionName ??= GetCollectionName<T>();
        return _database.GetCollection<T>(collectionName);
    }

    private static string? ExtractDatabaseName(string connectionString)
    {
        try
        {
            var uri = new Uri(connectionString);
            var databaseName = uri.LocalPath.TrimStart('/');
            return string.IsNullOrEmpty(databaseName) ? null : databaseName;
        }
        catch
        {
            // If connection string is not a URI format, return null to use default
            return null;
        }
    }

    private static string GetCollectionName<T>()
    {
        // Convert PascalCase to snake_case for collection names
        var typeName = typeof(T).Name;
        var collectionName = ConvertToSnakeCase(typeName);
        return collectionName;
    }

    private static string ConvertToSnakeCase(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;

        var result = new System.Text.StringBuilder();
        result.Append(char.ToLowerInvariant(input[0]));

        for (int i = 1; i < input.Length; i++)
        {
            if (char.IsUpper(input[i]))
            {
                result.Append('_');
                result.Append(char.ToLowerInvariant(input[i]));
            }
            else
            {
                result.Append(input[i]);
            }
        }

        return result.ToString();
    }
}

