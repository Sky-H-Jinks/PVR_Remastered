using Microsoft.EntityFrameworkCore;
using PVRServer.Database.Tables;
using System.Reflection;

namespace PVRServer.Database;

public class PVRDbContext : DbContext
{
  #region Database tables

  /// <summary>Config DbSet</summary>
  public DbSet<Config> ConfigTbl { get; set; }

  /// <summary>Errors DbSet</summary>
  public DbSet<ClientLogs> ClientLogTbl { get; set; }

  #endregion Database tables


  /// <summary>
  /// Constructor
  /// </summary>
  /// <param name="nOptions">Database options</param>
  public PVRDbContext(DbContextOptions<PVRDbContext> nOptions) : base(nOptions)
  { 
  }


  /// <summary>
  /// Overrides model creating method
  /// Sets configurations to be retrieved from `IEntityTypeConfiguration<T>` child classes
  /// For an example, See Database\Tables\Config.cs
  /// </summary>
  /// <param name="nModelBuilder">Model builder</param>
  protected override void OnModelCreating(ModelBuilder nModelBuilder)
  {
    nModelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
  }
}
