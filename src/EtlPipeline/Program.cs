using EtlPipeline.Connections;
using EtlPipeline.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);

// Register services
builder.Services.AddSingleton<SqlConnectionVerifier>();
builder.Services.AddSingleton<SchemaInitializer>();
builder.Services.AddSingleton<SqlScriptRunner>();

var host = builder.Build();

var logger = host.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("=== ETL Pipeline ===");

// Step 1: Verify all configured connections
var verifier = host.Services.GetRequiredService<SqlConnectionVerifier>();
await verifier.VerifyAllAsync();

// Step 2: Ensure destination schema prerequisites exist
var schemaInit = host.Services.GetRequiredService<SchemaInitializer>();
await schemaInit.InitializeAsync();

// Step 3: Run select & translate scripts
var scriptRunner = host.Services.GetRequiredService<SqlScriptRunner>();
await scriptRunner.RunFolderAsync(Path.Combine(AppContext.BaseDirectory, "select_and_translate"));

// Step 4: Run transform scripts
await scriptRunner.RunFolderAsync(Path.Combine(AppContext.BaseDirectory, "transform"));

logger.LogInformation("Initialization complete.");
