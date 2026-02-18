using EtlPipeline.Connections;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);

// Register services
builder.Services.AddSingleton<SqlConnectionVerifier>();

var host = builder.Build();

var logger = host.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("=== ETL Pipeline ===");

// Step 1: Verify all configured connections
var verifier = host.Services.GetRequiredService<SqlConnectionVerifier>();
await verifier.VerifyAllAsync();

logger.LogInformation("Connection verification complete.");
