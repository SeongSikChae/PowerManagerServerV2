using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PowerManager.Server.Context.Migrations
{
    /// <inheritdoc />
    public partial class ElectricPowerAdd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ElectricPower",
                columns: table => new
                {
                    PowerType = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    PowerTypeName = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElectricPower", x => x.PowerType);
                });

            migrationBuilder.InsertData(
                table: "ElectricPower",
                columns: new[] { "PowerType", "PowerTypeName" },
                values: new object[,]
                {
                    { "HouseHigh", "주택용 고압" },
                    { "HouseLow", "주택용 저압" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ElectricPower");
        }
    }
}
