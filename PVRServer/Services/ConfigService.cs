using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using PVRProtos;
using PVRServer.Database;
using PVRServer.Database.Tables;

namespace PVRServer.Services;

public class ConfigService : Cfg.CfgBase
{
  /// <summary>Logger</summary>
  private readonly ILogger<ConfigService> Logger;

  /// <summary>Service provider</summary>
  private readonly IServiceProvider ServiceProvider;


  /// <summary>
  /// Constructor 
  /// </summary>
  /// <param name="nLogger">Logger (serilog)</param>
  /// <param name="nServiceProvider">Service provider</param>
  public ConfigService(ILogger<ConfigService> nLogger, IServiceProvider nServiceProvider)
  { 
    Logger = nLogger;
    ServiceProvider = nServiceProvider;
  }


  /// <summary>
  /// Retrieves all of the config values and sends them to the client
  /// </summary>
  /// <param name="nRequest">Request (empty)</param>
  /// <param name="nContext">Request context</param>
  /// <returns>Config response with all config options</returns>
  /// <exception cref="RpcException"></exception>
  public override async Task<ConfigResponse> GetAllConfig(Empty nRequest, ServerCallContext nContext)
  {
    try
    { 
      ConfigResponse response = new ConfigResponse();
      List<Config> confLst = await GetAllConfigAsync().ConfigureAwait(false);
      response.Entries.AddRange(confLst.Select(x => new ConfigEntry() // Converts database entries to gRPC objects
      { 
        ConfigName = x.ConfigName,
        ConfigValue = x.ConfigValue,
        ConfigDescription = x.ConfigDescription
      }));

      return response;
    }
    catch(Exception ex)
    { 
      this.Logger.LogError(ex, @"Services\ConfigService.cs\GetAllConfig");
      throw new RpcException(new Status(StatusCode.Internal, "An internal error has occurred when retrieving the config values"));
    }
  }


  /// <summary>
  /// Retrieves a single config option from the database
  /// </summary>
  /// <param name="nRequest">Config option to search for</param>
  /// <param name="nContext">Client context</param>
  /// <returns>Config entry if found</returns>
  /// <exception cref="RpcException">Thrown if not found or invalid request (empty config name)</exception>
  public override async Task<ConfigEntry> GetSingleConfig(ConfigData nRequest, ServerCallContext nContext)
  {
    try
    {
      if(nRequest == null || string.IsNullOrEmpty(nRequest.ConfigName))
      { 
        Logger.LogWarning($"Attempt to get single config with '{nRequest?.ConfigName ?? ""}' as the config name. (HOST: {nContext.Host})");
        throw new RpcException(new Status(StatusCode.InvalidArgument, "Must provide a non-empty config name!"));
      }

      Config foundEntry = await GetSingleConfigAsync(nRequest.ConfigName).ConfigureAwait(false);
      if(foundEntry == null)
      { 
        Logger.LogWarning($"Unable to find config option '{nRequest.ConfigName}'. Aborting request.");
        throw new RpcException(new Status(StatusCode.NotFound, $"Unable to find a config option with the name '{nRequest.ConfigName}'"));
      }

      return new ConfigEntry() { ConfigName = foundEntry.ConfigName, 
                                 ConfigValue = foundEntry.ConfigValue, 
                                 ConfigDescription = foundEntry.ConfigDescription 
                               };
    }
    catch (Exception ex)
    {
      this.Logger.LogError(ex, @"Services\ConfigService.cs\GetSingleConfig");
      throw new RpcException(new Status(StatusCode.Internal, "An internal error has occurred when retrieving the config"));
    }
  }


  /// <summary>
  /// Updates config entries
  /// </summary>
  /// <param name="nRequest">Config entries to update</param>
  /// <param name="nContext">Client context</param>
  /// <returns>Bool indicating if any entries have been updated</returns>
  /// <exception cref="RpcException">If an error is to occur, RpcException is thrown</exception>
  public override async Task<UpdateConfigResponse> UpdateConfig(ConfigUpdateRequest nRequest, ServerCallContext nContext)
  {
    bool dataChanged = false;
    try
    {
      using(IServiceScope scope = this.ServiceProvider.CreateScope())
      { 
        PVRDbContext context = scope.ServiceProvider.GetRequiredService<PVRDbContext>();

        for(int x = 0; x < nRequest.Entries.Count; x++)
        { 
          Config cfg = (await context.ConfigTbl.FindAsync(nRequest.Entries[x].ConfigName).ConfigureAwait(false))!;
          if(cfg == null)
            continue;

          cfg.ConfigValue = nRequest.Entries[x].ConfigValue;
          cfg.IsClientConfig = nRequest.Entries[x].IsClientConfig;

          context.Entry(cfg).State = EntityState.Modified;
          dataChanged = true;
        }

        if(dataChanged) await context.SaveChangesAsync().ConfigureAwait(false);
        return new UpdateConfigResponse() { Result = dataChanged };
      }
    }
    catch (Exception ex)
    {
      this.Logger.LogError(ex, @"Services\ConfigService.cs\UpdateConfig");
      throw new RpcException(new Status(StatusCode.Internal, $"An internal error has occurred when updating the config option{(nRequest.Entries?.Count > 1 ? "s" : "")}"));
    }
  }


  /// <summary>
  /// Deletes a single database entry
  /// </summary>
  /// <param name="nRequest">Request containing config entry info</param>
  /// <param name="nContext">Client context</param>
  /// <returns>Result of data deletion</returns>
  /// <exception cref="RpcException">Thrown when an internal exception is thrown OR an empty config name is provided</exception>
  public override async Task<DeleteConfigResult> DeleteConfig(ConfigEntry nRequest, ServerCallContext nContext)
  {
    try
    {
      using(IServiceScope scope = this.ServiceProvider.CreateScope())
      { 
        PVRDbContext context = scope.ServiceProvider.GetRequiredService<PVRDbContext>();

        if (nRequest == null || string.IsNullOrEmpty(nRequest.ConfigName))
        {
          Logger.LogWarning($"Attempt to delete config with '{nRequest?.ConfigName ?? ""}' as the config name. (HOST: {nContext.Host})");
          throw new RpcException(new Status(StatusCode.InvalidArgument, "Must provide a non-empty config name to delete!"));
        }

        Config entry = await GetSingleConfigAsync(nRequest.ConfigName);
        if(entry == null)
          return new DeleteConfigResult() { Success = false };     

        context.Remove(entry);
        await context.SaveChangesAsync().ConfigureAwait(false);
        return new DeleteConfigResult() { Success = true };
      }
    }
    catch(RpcException rpcEx)
    { 
      throw;
    }
    catch (Exception ex)
    {
      this.Logger.LogError(ex, @"Services\ConfigService.cs\DeleteConfig");
      throw new RpcException(new Status(StatusCode.Internal, $"Internal error: {ex.Message}"));
    }
  }


  /// <summary>
  /// Retrieves a single config option from it's name
  /// </summary>
  /// <param name="nConfigName">Config name</param>
  /// <returns>Found entry - null if not found</returns>
  private async Task<Config> GetSingleConfigAsync(string nConfigName)
  { 
    using(IServiceScope scope = this.ServiceProvider.CreateScope())
    { 
      PVRDbContext context = scope.ServiceProvider.GetRequiredService<PVRDbContext>();
      return (await context.ConfigTbl.FindAsync(nConfigName).ConfigureAwait(false))!;
    }
  }


  /// <summary>
  /// Retrieves all config options from database
  /// </summary>
  /// <returns>List of config entries</returns>
  private async Task<List<Config>> GetAllConfigAsync()
  { 
    using(IServiceScope scope = this.ServiceProvider.CreateScope())
    { 
      PVRDbContext context = scope.ServiceProvider.GetRequiredService<PVRDbContext>();
      return await context.ConfigTbl.ToListAsync().ConfigureAwait(false);
    }
  }
}