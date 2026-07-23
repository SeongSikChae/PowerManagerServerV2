using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PowerManager.Server.Context.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Device",
                columns: table => new
                {
                    ID = table.Column<string>(type: "TEXT", maxLength: 12, nullable: false),
                    DeviceName = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Model = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Topic = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Password = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    PowerType = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    VoltCalibration = table.Column<double>(type: "REAL", nullable: true, defaultValue: 0.0),
                    ForwardConnector = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Device", x => x.ID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Device");
        }
    }
}
