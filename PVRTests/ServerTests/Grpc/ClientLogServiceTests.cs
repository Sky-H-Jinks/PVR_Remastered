using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using PVRProtos;

namespace PVRTests.ServerTests.Grpc;

[TestClass]
public class LoggerServiceTests
{
  private static GrpcTestFixture? Fixture;
  private static GrpcChannel? Channel;
  private static Logger.LoggerClient? Client;

  [ClassInitialize]
  public static void ClassInitialize(TestContext context)
  {
    Fixture = new GrpcTestFixture();
    Channel = Fixture.CreateGrpcChannel();
    Client = new Logger.LoggerClient(Channel);
  }

  [ClassCleanup]
  public static void ClassCleanup()
  {
    if (Channel != null)
    {
      Channel.ShutdownAsync().Wait();
      Channel = null;
    }

    Fixture?.Dispose();
  }

  [TestMethod]
  public async Task SendLog_WithValidLogDetails_ShouldReturnAcceptedTrue()
  {
    // Arrange
    var logRoot = new LogRoot();
    logRoot.Details.Add(new LogDetails
    {
      Source = "TestMachine",
      Message = "Test message",
      StackTrace = "Test stacktrace",
      RaisedAt = Timestamp.FromDateTime(DateTime.UtcNow),
      ClientVersion = "1.2.3",
      LogLevel = "ERROR"
    });

    // Act
    var response = await Client!.SendLogAsync(logRoot);

    // Assert
    Assert.IsNotNull(response);
    Assert.IsTrue(response.Accepted, "Expected to accept the log entries");
  }

  [TestMethod]
  public async Task SendLog_WhenInvalidData_ShouldThrowRpcException()
  {
    // For demonstration, we pretend to pass something that triggers an exception
    var logRoot = new LogRoot();
    logRoot.Details.Add(new LogDetails
    {
      LogLevel = ""
    });

    try
    {
      await Client!.SendLogAsync(logRoot);
      Assert.Fail("Expected an RpcException with StatusCode.Internal");
    }
    catch (RpcException ex)
    {
      Assert.AreEqual(StatusCode.Internal, ex.StatusCode);
    }
  }
}