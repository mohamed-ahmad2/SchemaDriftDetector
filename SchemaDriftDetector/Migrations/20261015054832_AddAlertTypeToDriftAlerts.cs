using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchemaDriftDetector.Migrations
{
    /// <inheritdoc />
    public partial class AddAlertTypeToDriftAlerts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AlertType",
                table: "DriftAlerts",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AlertType",
                table: "DriftAlerts");
        }
    }
}
