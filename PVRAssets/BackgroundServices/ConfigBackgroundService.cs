using PVRAssets.Networking.Services;
using PVRProtos;
using Serilog;
using System.Collections.ObjectModel;

namespace PVRAssets.BackgroundServices;

public class ConfigBackgroundService
{
  /// <summary>Collection of config entries from the server</summary>
  public ObservableCollection<ConfigEntry> ConfigEntries { get; private set; }
  
  /// <summary>Cancellation token</summary>
  private CancellationTokenSource? CancellationTokenSource;
  
  /// <summary>Config client gRPC service</summary>
  private readonly ConfigClientService ClientService;
  
  /// <summary>Despatcher invoke for PVR related actions</summary>
  private readonly Action<Action> DispatcherInvoke;

  /// <summary>Number of seconds between each update interval</summary>
  private int UpdateIntervalInSec { get; init; }


  /// <summary>
  /// Constructor
  /// </summary>
  /// <param name="nClientService">Client service object</param>
  /// <param name="nDespatcherAction">Despatcher action</param>
  /// <param name="nStart">If true then the background task will start immediately</param>
  /// <param name="nUpdateTimeoutIntervalInSec">Seconds between each call to the server for updates</param>
  public ConfigBackgroundService(ConfigClientService nClientService, Action<Action> nDespatcherAction, bool nStart = true, int nUpdateTimeoutIntervalInSec = 30)
  {
    ConfigEntries = new ObservableCollection<ConfigEntry>();
    ClientService = nClientService;
    DispatcherInvoke = nDespatcherAction;
    UpdateIntervalInSec = nUpdateTimeoutIntervalInSec;
    if(nStart) 
      StartBackgroundTask();
  }


  /// <summary>
  /// Starts the background task
  /// </summary>
  public void StartBackgroundTask()
  {
    if(CancellationTokenSource?.IsCancellationRequested ?? false) // If not null and isn't cancelled (still running) ignore
      return;

    CancellationTokenSource = new CancellationTokenSource();

    Task.Run(async () =>
    {
      try
      {
        while (!CancellationTokenSource.Token.IsCancellationRequested)
        {
          try
          {
            await RetrieveConfigEntriesAsync();
            await Task.Delay(TimeSpan.FromSeconds(UpdateIntervalInSec), CancellationTokenSource.Token);
          }
          catch(Exception ex)
          { 
            Log.Logger.Error(ex, "Error in main config loop");
          }
        }
      }
      catch (TaskCanceledException tEx)
      {
        Log.Logger.Error(tEx, "Config process cancelled");
      }
      catch (Exception ex)
      {
        Log.Logger.Error(ex, "Error outside of config loop");
      }
    });
  }


  /// <summary>
  /// Stops the background task
  /// </summary>
  public void StopBackgroundTask()
  {
    CancellationTokenSource?.Cancel();
  }


  /// <summary>
  /// Retrieves config options from server and updates them for PVR
  /// </summary>
  /// <returns></returns>
  private async Task RetrieveConfigEntriesAsync()
  {
    try
    {
      var response = await ClientService.GetAllConfigAsync().ConfigureAwait(false);

      if (response?.Entries != null)
      {
        DispatcherInvoke(() =>
        {
          ConfigEntries.Clear();
          foreach (var entry in response.Entries)
          {
            ConfigEntries.Add(entry);
          }
        });
      }
    }
    catch (Exception ex)
    {
      Log.Logger.Error(ex, "Error when attempting to retrieve config options from server!");
    }
  }
}