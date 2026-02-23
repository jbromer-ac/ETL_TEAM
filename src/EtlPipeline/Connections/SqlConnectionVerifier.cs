using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EtlPipeline.Connections;

public class SqlConnectionVerifier
{
    private readonly IConfiguration _config;
    private readonly ILogger<SqlConnectionVerifier> _logger;

    public SqlConnectionVerifier(IConfiguration config, ILogger<SqlConnectionVerifier> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task VerifyAllAsync(CancellationToken ct = default)
    {
        await VerifyAsync("SourceSqlServer", "Source SQL Server (GoLive-Silverlining)", ct);
        await VerifyAsync("DestinationSqlServer", "Destination SQL Server", ct);
        await VerifyAsync("AzureSqlPaas", "Azure SQL PaaS", ct);
    }

    private async Task VerifyAsync(string connectionStringName, string label, CancellationToken ct)
    {
        var connStr = _config.GetConnectionString(connectionStringName);

        if (string.IsNullOrWhiteSpace(connStr) || connStr.Contains('<'))
        {
            _logger.LogWarning("[{Label}] Connection string not configured — skipping.", label);
            return;
        }

        try
        {
            await using var conn = new SqlConnection(connStr);
            await conn.OpenAsync(ct);
            _logger.LogInformation(
                "[{Label}] Connected successfully. Server: {Server} | Version: {Version}",
                label, conn.DataSource, conn.ServerVersion);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[{Label}] Connection FAILED: {Message}", label, ex.Message);
        }
    }
}
