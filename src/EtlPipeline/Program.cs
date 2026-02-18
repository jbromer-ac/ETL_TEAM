using EtlPipeline.Connections;
using EtlPipeline.Infrastructure;
using EtlPipeline.Sources.SageIntacct;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);

// Register services
builder.Services.AddSingleton<SqlConnectionVerifier>();
builder.Services.AddSingleton<SchemaInitializer>();
builder.Services.AddSingleton<SqlScriptRunner>();
builder.Services.Configure<SageIntacctSettings>(builder.Configuration.GetSection("SageIntacct"));
builder.Services.AddSingleton<SageIntacctClient>();

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

// Step 5: Obtain Sage Intacct session
var intacct = host.Services.GetRequiredService<SageIntacctClient>();
await intacct.GetSessionAsync();

logger.LogInformation("Initialization complete.");
