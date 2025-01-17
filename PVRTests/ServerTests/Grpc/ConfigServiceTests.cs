using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using PVRProtos;

namespace PVRTests.ServerTests.Grpc;

[TestClass]
public class ConfigServiceTests
{
  private static GrpcTestFixture? Fixture;
  private static GrpcChannel? Channel;
  private static Cfg.CfgClient? Client;

  [ClassInitialize]
  public static void Init(TestContext nContext)
  { 
    Fixture = new GrpcTestFixture();
    Channel = Fixture.CreateGrpcChannel();
    Client = new Cfg.CfgClient(Channel);
  }


  [ClassCleanup]
  public static void ClassCleanup()
  {
    if(Channel != null)
    { 
      Channel.ShutdownAsync().Wait();
      Channel = null;
    }

    Fixture?.Dispose();
  }


  [TestMethod]
  public async Task GetAllConfigTestAsync()
  { 
    var response = await Client!.GetAllConfigAsync(new Empty());
    Assert.IsNotNull(response, "Response should not be null");
    Assert.IsNotNull(response.Entries, "Response entries should not be null");
    Assert.IsTrue(response.Entries.Count >= 0, "Zero or more entries should have been returned");
  }


  [TestMethod]
  public async Task UpdateConfigTestAsync_UpdateExpected()
  { 
    var data = await Client!.GetAllConfigAsync(new Empty()).ConfigureAwait(false);
  
    Assert.IsNotNull(data);
    Assert.IsTrue(data.Entries.Count > 0);

    if(data.Entries.Count <= 0)
      return;

    var request = new ConfigUpdateRequest();
    request.Entries.Add(new UpdateConfigEntry
    { 
      ConfigName = data.Entries[0].ConfigName,
      ConfigValue = $"New_Value_{DateTime.Now.ToString()}"
    });

    var response = await Client!.UpdateConfigAsync(request);
    Assert.IsNotNull(response);
    Assert.IsTrue(response.Result);
  }


  [TestMethod]
  public async Task UpdateConfigTestAsync_ExpectFail()
  { 
    try
    { 
      var request = new ConfigUpdateRequest();
      request.Entries.Add(new UpdateConfigEntry()
      { 
        ConfigName = "",
        ConfigValue = "123"
      });

      var data =  await Client!.UpdateConfigAsync(request);
      Assert.IsFalse(data.Result); // Expecting false as data isn't present 
    }
    catch(RpcException _ex)
    { 
      Assert.AreEqual(StatusCode.Internal, _ex.StatusCode, message: "RPC exception thrown as expected!");
    }
    catch(Exception ex)
    { 
      Assert.Fail(ex.Message);
    }
  }


  [TestMethod]
  public async Task DeleteConfig_NameIsEmpty_ExceptionExpected()
  { 
    try
    {
      var result = await Client!.DeleteConfigAsync(new ConfigEntry { ConfigName = "" });
      Assert.IsNull(result);
    }
    catch(RpcException ex)
    { 
      Assert.AreEqual(StatusCode.InvalidArgument, ex.StatusCode);
    }
  }
}
