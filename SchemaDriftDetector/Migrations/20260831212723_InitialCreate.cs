using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchemaDriftDetector.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Deploys",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CommitHash = table.Column<string>(type: "TEXT", nullable: false),
                    Environment = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Deploys", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Endpoints",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    RouteTemplate = table.Column<string>(type: "TEXT", nullable: false),
                    HttpMethod = table.Column<string>(type: "TEXT", nullable: false),
                    Environment = table.Column<string>(type: "TEXT", nullable: false),
                    Role = table.Column<string>(type: "TEXT", nullable: false),
                    ApiVersion = table.Column<string>(type: "TEXT", nullable: false),
                    FirstSeenAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastSeenAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsDeprecated = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Endpoints", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DriftAlerts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    EndpointId = table.Column<Guid>(type: "TEXT", nullable: false),
                    DeployId = table.Column<Guid>(type: "TEXT", nullable: true),
                    FieldPath = table.Column<string>(type: "TEXT", nullable: false),
                    Severity = table.Column<string>(type: "TEXT", nullable: false),
                    DeliveryStatus = table.Column<string>(type: "TEXT", nullable: false),
                    RetryCount = table.Column<int>(type: "INTEGER", nullable: false),
                    DetectedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DriftAlerts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DriftAlerts_Deploys_DeployId",
                        column: x => x.DeployId,
                        principalTable: "Deploys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_DriftAlerts_Endpoints_EndpointId",
                        column: x => x.EndpointId,
                        principalTable: "Endpoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PendingDrifts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    EndpointId = table.Column<Guid>(type: "TEXT", nullable: false),
                    FieldPath = table.Column<string>(type: "TEXT", nullable: false),
                    ProposedSchemaJson = table.Column<string>(type: "TEXT", nullable: false),
                    ChangeType = table.Column<string>(type: "TEXT", nullable: false),
                    ConsecutiveCount = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    LastDetectedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PendingDrifts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PendingDrifts_Endpoints_EndpointId",
                        column: x => x.EndpointId,
                        principalTable: "Endpoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SchemaBaselines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    EndpointId = table.Column<Guid>(type: "TEXT", nullable: false),
                    SchemaJson = table.Column<string>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchemaBaselines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SchemaBaselines_Endpoints_EndpointId",
                        column: x => x.EndpointId,
                        principalTable: "Endpoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SchemaVersions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    EndpointId = table.Column<Guid>(type: "TEXT", nullable: false),
                    DeployId = table.Column<Guid>(type: "TEXT", nullable: true),
                    SchemaJson = table.Column<string>(type: "TEXT", nullable: false),
                    ChangeReason = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchemaVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SchemaVersions_Deploys_DeployId",
                        column: x => x.DeployId,
                        principalTable: "Deploys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_SchemaVersions_Endpoints_EndpointId",
                        column: x => x.EndpointId,
                        principalTable: "Endpoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DriftAlerts_DeployId",
                table: "DriftAlerts",
                column: "DeployId");

            migrationBuilder.CreateIndex(
                name: "IX_DriftAlerts_EndpointId",
                table: "DriftAlerts",
                column: "EndpointId");

            migrationBuilder.CreateIndex(
                name: "IX_Endpoints_RouteTemplate_Environment",
                table: "Endpoints",
                columns: new[] { "RouteTemplate", "Environment" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PendingDrifts_EndpointId",
                table: "PendingDrifts",
                column: "EndpointId");

            migrationBuilder.CreateIndex(
                name: "IX_SchemaBaselines_EndpointId",
                table: "SchemaBaselines",
                column: "EndpointId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SchemaVersions_DeployId",
                table: "SchemaVersions",
                column: "DeployId");

            migrationBuilder.CreateIndex(
                name: "IX_SchemaVersions_EndpointId",
                table: "SchemaVersions",
                column: "EndpointId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DriftAlerts");

            migrationBuilder.DropTable(
                name: "PendingDrifts");

            migrationBuilder.DropTable(
                name: "SchemaBaselines");

            migrationBuilder.DropTable(
                name: "SchemaVersions");

            migrationBuilder.DropTable(
                name: "Deploys");

            migrationBuilder.DropTable(
                name: "Endpoints");
        }
    }
}
