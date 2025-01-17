using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PVRServer.Migrations
{
    /// <inheritdoc />
    public partial class ClientLogs_Add_LogLevel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LogLevel",
                table: "ClientLogs",
                type: "nvarchar(16)",
                maxLength: 16,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LogLevel",
                table: "ClientLogs");
        }
    }
}
