using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchemaDriftDetector.Migrations
{
    /// <inheritdoc />
    public partial class AlignEndpointIndexAndNullableProposedSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Endpoints_RouteTemplate_Environment",
                table: "Endpoints");

            migrationBuilder.AlterColumn<string>(
                name: "ProposedSchemaJson",
                table: "PendingDrifts",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.CreateIndex(
                name: "IX_Endpoints_RouteTemplate_HttpMethod_Environment_Role_ApiVersion",
                table: "Endpoints",
                columns: new[] { "RouteTemplate", "HttpMethod", "Environment", "Role", "ApiVersion" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Endpoints_RouteTemplate_HttpMethod_Environment_Role_ApiVersion",
                table: "Endpoints");

            migrationBuilder.AlterColumn<string>(
                name: "ProposedSchemaJson",
                table: "PendingDrifts",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Endpoints_RouteTemplate_Environment",
                table: "Endpoints",
                columns: new[] { "RouteTemplate", "Environment" },
                unique: true);
        }
    }
}
