using Microsoft.EntityFrameworkCore;
using PVRProtos;
using PVRServer.Database;
using PVRServer.Services;
using Serilog;
using Serilog.Sinks.MSSqlServer;

namespace PVRServer;
public class Program
{
  public static void Main(string[] args)
  {
    var builder = WebApplication.CreateBuilder(args);

    // Load appsettings.development.json first
    builder.Configuration.AddJsonFile("appsettings.development.json", optional: true, reloadOnChange: true);

#if RELEASE
    // Load appsettings.json should only be used in release
    builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
#endif

    SetupSerilog(builder);

    // Add services to the container.
    builder.Services.AddGrpc();
    builder.Services.AddDbContext<PVRDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("PVRConnStr")));

    // builder.Services.AddScoped<TService>(); -- Must add to scoped, especially when expecting to handle EF for database related work
    builder.Services.AddScoped<ConfigService>();
    builder.Services.AddScoped<LoggerService>();

    var app = builder.Build();

    // app.MapGrpcService<TService>(); -- Adds service to the application
    app.MapGrpcService<ConfigService>();
    app.MapGrpcService<LoggerService>();

    app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

    app.Run();
  }


  /// <summary>
  /// Sets up Serilog
  /// </summary>
  /// <param name="nBuilder">Web application builder</param>
  private static void SetupSerilog(WebApplicationBuilder nBuilder)
  {
    string environmentName = nBuilder.Environment.EnvironmentName;

    var loggerConfig = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()                                                     // Logs to console
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day);  // Logs to file in Logs/log-<date_?>.txt

    if (string.Compare(environmentName, "Test", true) != 0)
    {
      loggerConfig
        .WriteTo.MSSqlServer(                                                 // Logs to SQL server ([Logs])
          connectionString: nBuilder.Configuration.GetConnectionString("PVRConnStr"),
          sinkOptions: new MSSqlServerSinkOptions { AutoCreateSqlTable = true, TableName = "Logs" });
    }

    Log.Logger = loggerConfig.CreateLogger();
    nBuilder.Logging.ClearProviders();                                    // Removes all current loggers
    nBuilder.Logging.AddSerilog(Log.Logger);                              // Adds Serilog as project logger
  }
}