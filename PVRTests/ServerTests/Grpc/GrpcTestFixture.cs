using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc.Testing;

namespace PVRTests.ServerTests.Grpc;
public class GrpcTestFixture : WebApplicationFactory<PVRServer.Program>, IDisposable
{
  public GrpcChannel Channel { get; private set; } = default!;

  /// <summary>
  /// Creates a GrpcChannel from the underlying test server
  /// so we can call the gRPC endpoints just like a real client.
  /// </summary>
  public GrpcChannel CreateGrpcChannel()
  {
    // The base class has a CreateDefaultClient() that uses the in-memory server.
    var httpClient = this.CreateDefaultClient(new Uri("http://localhost:5000"));
    return GrpcChannel.ForAddress("http://localhost:5000", new GrpcChannelOptions
    {
      HttpClient = httpClient
    });
  }

  protected override void Dispose(bool disposing)
  {
    base.Dispose(disposing);
  }
}
