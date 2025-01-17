using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace PVRServer.Database;

/// <summary>
/// Required for EF command line tools such as 
/// dotnet ef database update InitialCreation
/// 
/// NOTE: Should NOT be used within the project
/// </summary>
public class PVRDbContextFactory : IDesignTimeDbContextFactory<PVRDbContext>
{
  /// <summary>
  /// Creates DbContext object for command line related tools
  /// </summary>
  /// <param name="args">Arguments</param>
  /// <returns>DbContext for EF</returns>
  public PVRDbContext CreateDbContext(string[] args)
  {
    var configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.Development.json")
        .Build();

    var optionsBuilder = new DbContextOptionsBuilder<PVRDbContext>();
    optionsBuilder.UseSqlServer(configuration.GetConnectionString("PVRConnStr"));

    return new PVRDbContext(optionsBuilder.Options);
  }
}
