using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using PVRProtos;

namespace PVRAssets.Networking.Services;
public class ConfigClientService
{
  /// <summary>gRPC generated client object</summary>
  private readonly Cfg.CfgClient Client;


  /// <summary>
  /// Constructor
  /// </summary>
  /// <param name="nServerAddress">Server IP address</param>
  public ConfigClientService(string nServerAddress) => Client = new Cfg.CfgClient(GrpcChannel.ForAddress(nServerAddress));


  /// <summary>
  /// Retrieves a single config result from the server
  /// </summary>
  /// <param name="nName">Name of the config option</param>
  /// <returns>Config entry that it matches</returns>
  public async Task<ConfigEntry> GetStringConfigAsync(ConfigData nName) => await Client.GetSingleConfigAsync(nName).ConfigureAwait(false);


  /// <summary>
  /// Retrieves all config options 
  /// </summary>
  /// <returns>Config response</returns>
  public async Task<ConfigResponse> GetAllConfigAsync() => await Client.GetAllConfigAsync(new Empty()).ConfigureAwait(false);


  /// <summary>
  /// Updates a collection of config options
  /// </summary>
  /// <param name="nRequest">Configs to update</param>
  /// <returns>Boolean indicating success of update</returns>
  public async Task<bool> UpdateConfigAsync(ConfigUpdateRequest nRequest) => (await Client.UpdateConfigAsync(nRequest).ConfigureAwait(false)).Result;


  /// <summary>
  /// Deletes config from the database
  /// </summary>
  /// <param name="nEntry">Entry to delete</param>
  /// <returns>Boolean indicating success</returns>
  public async Task<bool> DeleteConfigAsync(ConfigEntry nEntry) => (await Client.DeleteConfigAsync(nEntry).ConfigureAwait(false)).Success;
}
