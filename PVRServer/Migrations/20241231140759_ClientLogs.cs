using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PVRServer.Migrations
{
    /// <inheritdoc />
    public partial class ClientLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClientLogs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Source = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false),
                    StackTrace = table.Column<string>(type: "nvarchar(MAX)", nullable: true),
                    InsertedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    RaisedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ClientVersion = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    ServerVersion = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientLogs", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientLogs");
        }
    }
}
