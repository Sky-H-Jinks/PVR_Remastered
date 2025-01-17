using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PVRServer.Database.Tables;


/// <summary>
/// Entity Framework template
/// </summary>
public class Config
{
  /// <summary>Config name/key</summary>
  public string? ConfigName;

  /// <summary>Config value</summary>
  public string? ConfigValue;

  /// <summary>Config description </summary>
  public string? ConfigDescription;

  /// <summary>Indicates if client or server config</summary>
  public bool IsClientConfig;
}


/// <summary>
/// Table configuration for Config
/// </summary>
public class ConfigConfiguration : IEntityTypeConfiguration<Config>
{
  /// <summary>
  /// Configures EntityType during migrations
  /// </summary>
  /// <param name="nBuilder">Entity Type Builder for Config</param>
  public void Configure(EntityTypeBuilder<Config> nBuilder)
  {
    nBuilder.ToTable("Config");

    nBuilder.HasKey(x => x.ConfigName);
    nBuilder.Property(x => x.ConfigName)
           .IsRequired()
           .HasMaxLength(256);

    nBuilder.Property(x => x.ConfigValue)
           .IsRequired()
           .HasMaxLength(1024);

    nBuilder.Property(x => x.ConfigDescription)
           .IsRequired(false)
           .HasMaxLength(1024);

    nBuilder.Property(x => x.IsClientConfig)
           .IsRequired();
  }
}