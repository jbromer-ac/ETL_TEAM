using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EtlPipeline.Infrastructure;

public class SchemaInitializer
{
    private readonly IConfiguration _config;
    private readonly ILogger<SchemaInitializer> _logger;

    private const string EnsureMapSchema = """
        IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'MAP')
        BEGIN
            EXEC('CREATE SCHEMA [MAP]')
            PRINT 'MAP schema created.'
        END
        ELSE
        BEGIN
            PRINT 'MAP schema already exists.'
        END
        """;

    public SchemaInitializer(IConfiguration config, ILogger<SchemaInitializer> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task InitializeAsync(CancellationToken ct = default)
    {
        var connStr = _config.GetConnectionString("DestinationSqlServer");

        if (string.IsNullOrWhiteSpace(connStr) || connStr.Contains('<'))
        {
            _logger.LogWarning("[SchemaInitializer] Destination connection string not configured — skipping schema check.");
            return;
        }

        try
        {
            await using var conn = new SqlConnection(connStr);
            await conn.OpenAsync(ct);

            await using var cmd = new SqlCommand(EnsureMapSchema, conn);
            await cmd.ExecuteNonQueryAsync(ct);

            _logger.LogInformation("[SchemaInitializer] MAP schema verified on destination database.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[SchemaInitializer] Failed to verify/create MAP schema: {Message}", ex.Message);
            throw;
        }
    }
}
