using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PVRServer.Database.Tables;

public class ClientLogs
{
  public long Id { get; set; }               // Error ID
  public string? Source { get; set; }        // Source (Machine Name/IP)
  public string? Message { get; set; }       // Error Message (Exception.Message)
  public string? StackTrace { get; set; }    // Error Stacktrace (Exception.StackTrace)
  public DateTime InsertedAt { get; set; }   // GETDATE()
  public DateTime RaisedAt { get; set; }     // Date time error was raised/sent
  public string? ClientVersion { get; set; } // Client app version
  public string? ServerVersion { get; set; } // gRPC server version
  public string? LogLevel { get; set; }      // Log Level
}


public class ClientLogsConfiguration : IEntityTypeConfiguration<ClientLogs>
{
  /// <summary>
  /// Configures EntityType during migrations
  /// </summary>
  /// <param name="nBuilder">Entity Type Builder for ClientLogs</param>
  public void Configure(EntityTypeBuilder<ClientLogs> nBuilder)
  {
    nBuilder.ToTable("ClientLogs");
    nBuilder.HasKey(x => x.Id);
    nBuilder.Property(x => x.Id)
            .IsRequired()
            .ValueGeneratedOnAdd();

    nBuilder.Property(x => x.Source)
            .IsRequired()
            .HasMaxLength(256);

    nBuilder.Property(x => x.Message)
            .IsRequired()
            .HasMaxLength(1024);

    nBuilder.Property(x => x.StackTrace)
            .IsRequired(false)
            .HasColumnType("nvarchar(MAX)");

    nBuilder.Property(x => x.InsertedAt)
            .IsRequired()
            .HasDefaultValueSql("GETDATE()")
            .ValueGeneratedOnAdd()
            .Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Ignore);
            
    nBuilder.Property(x => x.InsertedAt)
            .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

    nBuilder.Property(x => x.RaisedAt)
            .IsRequired();

    nBuilder.Property(x => x.ClientVersion)
            .IsRequired()
            .HasMaxLength(32);

    nBuilder.Property(x => x.ServerVersion)
            .IsRequired()
            .HasMaxLength(32);

    nBuilder.Property(x => x.LogLevel)
            .IsRequired()
            .HasMaxLength(16);
  }
}