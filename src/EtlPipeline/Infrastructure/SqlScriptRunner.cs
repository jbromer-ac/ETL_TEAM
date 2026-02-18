using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EtlPipeline.Infrastructure;

public class SqlScriptRunner
{
    private readonly IConfiguration _config;
    private readonly ILogger<SqlScriptRunner> _logger;

    public SqlScriptRunner(IConfiguration config, ILogger<SqlScriptRunner> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task RunFolderAsync(string folderPath, CancellationToken ct = default)
    {
        var connStr = _config.GetConnectionString("DestinationSqlServer");

        if (string.IsNullOrWhiteSpace(connStr) || connStr.Contains('<'))
        {
            _logger.LogWarning("[SqlScriptRunner] Destination connection string not configured — skipping.");
            return;
        }

        var files = Directory.GetFiles(folderPath, "*.sql", SearchOption.TopDirectoryOnly)
                             .OrderBy(f => f)
                             .ToArray();

        if (files.Length == 0)
        {
            _logger.LogWarning("[SqlScriptRunner] No .sql files found in {Folder}", folderPath);
            return;
        }

        _logger.LogInformation("[SqlScriptRunner] Running {Count} script(s) from {Folder}", files.Length, folderPath);

        await using var conn = new SqlConnection(connStr);
        await conn.OpenAsync(ct);

        foreach (var file in files)
        {
            var fileName = Path.GetFileName(file);
            try
            {
                var sql = await File.ReadAllTextAsync(file, ct);

                // Split on GO (SQL batch separator), ignoring blank batches
                var batches = sql
                    .Split(["\r\nGO", "\nGO", "\r\ngo", "\ngo"], StringSplitOptions.None)
                    .Select(b => b.Trim())
                    .Where(b => !string.IsNullOrWhiteSpace(b))
                    .ToArray();

                foreach (var batch in batches)
                {
                    await using var cmd = new SqlCommand(batch, conn);
                    await cmd.ExecuteNonQueryAsync(ct);
                }

                _logger.LogInformation("[SqlScriptRunner] OK  {File}", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[SqlScriptRunner] FAIL {File}: {Message}", fileName, ex.Message);
                throw;
            }
        }

        _logger.LogInformation("[SqlScriptRunner] All scripts completed.");
    }
}
