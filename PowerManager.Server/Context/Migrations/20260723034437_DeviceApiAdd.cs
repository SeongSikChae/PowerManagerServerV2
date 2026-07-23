using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PowerManager.Server.Context.Migrations
{
    /// <inheritdoc />
    public partial class DeviceApiAdd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DeviceApi",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    DeviceId = table.Column<string>(type: "TEXT", maxLength: 12, nullable: false),
                    Verify = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    MqttKey = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceApi", x => new { x.UserId, x.DeviceId });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeviceApi");
        }
    }
}
