using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PVRServer.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Config",
                columns: table => new
                {
                    ConfigName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    ConfigDescription = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    ConfigValue = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false),
                    IsClientConfig = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Config", x => x.ConfigName);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Config");
        }
    }
}
