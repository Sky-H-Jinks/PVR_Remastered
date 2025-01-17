using Grpc.Net.Client;
using PVRProtos;

namespace PVRAssets.Networking.Services;
public class LoggingClientService : Logger.LoggerBase
{
  /// <summary>GRPC Client</summary>
  private readonly Logger.LoggerClient Client;
  
  /// <summary>
  /// Constructor
  /// </summary>
  /// <param name="nServerAddress">Server address</param>
  public LoggingClientService(string nServerAddress) => Client = new Logger.LoggerClient(GrpcChannel.ForAddress(nServerAddress));


  /// <summary>
  /// Sends logs to server
  /// </summary>
  /// <param name="nRoot"></param>
  /// <returns></returns>
  public async Task<LogResponse> SendLogsAsync(LogRoot nRoot) => await Client.SendLogAsync(nRoot).ConfigureAwait(false);
}
