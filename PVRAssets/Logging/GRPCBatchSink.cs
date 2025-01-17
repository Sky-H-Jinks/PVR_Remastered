using Google.Protobuf.WellKnownTypes;
using PVRAssets.Networking.Services;
using PVRProtos;
using Serilog.Core;
using Serilog.Events;
using System.Collections.Concurrent;

namespace PVRAssets.Logging;
public class GRPCBatchSink : ILogEventSink, IDisposable
{
  /// <summary>Format provider</summary>
  private readonly IFormatProvider? FormatProvider;

  /// <summary>Queue flush interval</summary>
  private readonly TimeSpan FlushInterval;

  /// <summary>Timer</summary>
  private readonly Timer Timer;

  /// <summary>Concurrent queue of LogEvents (Thread Safe)</summary>
  private readonly ConcurrentQueue<LogEvent> LogQueue = new ConcurrentQueue<LogEvent>();

  /// <summary>Indicates if sink has been disposed</summary>
  private bool Disposed = false;

  /// <summary>gRPC logging service</summary>
  private readonly LoggingClientService Service;


  /// <summary>
  /// Constructor
  /// </summary>
  /// <param name="nService">gRPC logging service</param>
  /// <param name="nFlushInterval">Flush interval</param>
  /// <param name="nFormatProvider">Format provider</param>
  /// <exception cref="ArgumentNullException">Service MUST NOT be null</exception>
  public GRPCBatchSink(LoggingClientService nService, TimeSpan nFlushInterval, IFormatProvider? nFormatProvider = null)
  { 
    if(nService == null)
      throw new ArgumentNullException(nameof(nService));

    Service = nService;
    FlushInterval = nFlushInterval;
    FormatProvider = nFormatProvider;
    Timer = new Timer(FlushQueueAsync, null, FlushInterval, FlushInterval);
  }


  /// <summary>
  /// Called when Serilog receives a log
  /// </summary>
  /// <param name="nLogEvent">Log event</param>
  public void Emit(LogEvent nLogEvent)
  {
    LogQueue.Enqueue(nLogEvent);
  }


  /// <summary>
  /// Called when disposed 
  /// </summary>
  public void Dispose()
  {
    if(!Disposed)
    { 
      Timer?.Dispose();
      Disposed = true;
    }
  }


  /// <summary>
  /// Flushes the LogEvent queue
  /// </summary>
  /// <param name="nState">Not used</param>
  private async void FlushQueueAsync(object? nState)
  {
    try
    {
      LogRoot root = new LogRoot();
      
      /* Dequeues and formats LogEvents */
      while (LogQueue.TryDequeue(out LogEvent? _event))
      { 
        if(_event == null)
          continue;

        LogDetails curr = new LogDetails();

        if (_event.Exception == null)
        {
          curr.Source = "N/A";
          curr.Message = _event.RenderMessage(FormatProvider);
          curr.StackTrace = null;
        }
        else
        {
          curr.Source = _event.Exception.Source ?? "N/A";
          curr.Message = _event.Exception.Message ?? "N/A";
          curr.StackTrace = _event.Exception.StackTrace ?? "N/A";
        }

        curr.RaisedAt = Timestamp.FromDateTimeOffset(_event.Timestamp);
        curr.ClientVersion = "1.0.0.0"; // Fix this to retrieve currnet build/version2
        curr.LogLevel = _event.Level.ToString();

        root.Details.Add(curr);
      }

      // Sends requests
      var response = await this.Service.SendLogsAsync(root).ConfigureAwait(false);
      if (!response.Accepted)
      {
        Serilog.Log.Logger.Error("Server failed to process logs!");
      }
    }
    catch (Exception ex)
    {
      Serilog.Log.Logger.Error(ex, "Error when sending logs to server");
    }
  }
}
