using Grpc.Core;
using PVRProtos;
using PVRServer.Database;
using PVRServer.Database.Tables;

namespace PVRServer.Services;

public class LoggerService : PVRProtos.Logger.LoggerBase
{
  /// <summary>Logger (Serilog)</summary>
  private readonly ILogger<LoggerService> Logger;

  /// <summary>Service Provider (DI)</summary>
  private readonly IServiceProvider ServiceProvider;


  /// <summary>
  /// Constructor
  /// </summary>
  /// <param name="nLogger">Logger (Serilog)</param>
  /// <param name="nServiceProvider">Service Provider (DI)</param>
  public LoggerService(ILogger<LoggerService> nLogger, IServiceProvider nServiceProvider)
  {
    Logger = nLogger;
    ServiceProvider = nServiceProvider;
  }

  /// <summary>
  /// Receives log(s) and saves them to the database 
  /// </summary>
  /// <param name="nRequest">LogRoot with a list of log entries</param>
  /// <param name="nContext">Client context</param>
  /// <returns>Success status</returns>
  /// <exception cref="RpcException">Thrown when an exception is raise</exception>
  public override async Task<LogResponse> SendLog(LogRoot nRequest, ServerCallContext nContext)
  {
    try
    {
      List<ClientLogs> logLst = new List<ClientLogs>();
      foreach (LogDetails details in nRequest.Details)
      {
        ClientLogs curr = new ClientLogs
        {
          StackTrace = details.StackTrace,
          Source = details.Source,
          ServerVersion = "1.0.0.0",
          ClientVersion = details.ClientVersion,
          Message = details.Message,
          RaisedAt = details.RaisedAt.ToDateTime(),
          LogLevel = details.LogLevel
        };
        logLst.Add(curr);
      }

      // 2) Insert them into the database
      using (IServiceScope scope = this.ServiceProvider.CreateScope())
      {
        PVRDbContext context = scope.ServiceProvider.GetRequiredService<PVRDbContext>();

        try
        {
          await context.AddRangeAsync(logLst).ConfigureAwait(false);
          await context.SaveChangesAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
          // Log and return
          this.Logger.LogError(ex, "Error when attempting to insert entries into the database");
          return new LogResponse() { Accepted = false };
        }
      }

      // 3) If all succeeded, return successful response
      return new LogResponse() { Accepted = true };
    }
    catch (Exception ex)
    {
      this.Logger.LogError(ex, @"Services\LoggerService\SendLog");
      throw new RpcException(
          new Status(StatusCode.Internal, "An internal error has occurred while processing client logs!")
      );
    }
  }
}