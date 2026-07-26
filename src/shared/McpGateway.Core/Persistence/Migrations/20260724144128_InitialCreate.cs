using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace McpGateway.Core.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "audit_entries",
                columns: table => new
                {
                    id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    timestamp = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    upstream_name = table.Column<string>(type: "TEXT", nullable: true),
                    tool_name = table.Column<string>(type: "TEXT", nullable: false),
                    client_name = table.Column<string>(type: "TEXT", nullable: true),
                    status = table.Column<string>(type: "TEXT", nullable: false),
                    latency_ms = table.Column<long>(type: "INTEGER", nullable: false),
                    input = table.Column<string>(type: "TEXT", nullable: true),
                    output = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_audit_entries", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "gateway_clients",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "TEXT", nullable: false),
                    name = table.Column<string>(type: "TEXT", nullable: false),
                    auth_kind = table.Column<string>(type: "TEXT", nullable: false),
                    secret = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_gateway_clients", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "upstreams",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "TEXT", nullable: false),
                    name = table.Column<string>(type: "TEXT", nullable: false),
                    enabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    transport = table.Column<string>(type: "TEXT", nullable: false),
                    command = table.Column<string>(type: "TEXT", nullable: true),
                    args = table.Column<string>(type: "TEXT", nullable: false),
                    environment = table.Column<string>(type: "TEXT", nullable: false),
                    endpoint = table.Column<string>(type: "TEXT", nullable: true),
                    headers = table.Column<string>(type: "TEXT", nullable: false),
                    auth_kind = table.Column<string>(type: "TEXT", nullable: false),
                    auth_config_json = table.Column<string>(type: "TEXT", nullable: true),
                    secret = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_upstreams", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tool_overrides",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "TEXT", nullable: false),
                    upstream_id = table.Column<Guid>(type: "TEXT", nullable: false),
                    tool_name = table.Column<string>(type: "TEXT", nullable: false),
                    enabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    override_name = table.Column<string>(type: "TEXT", nullable: true),
                    override_description = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tool_overrides", x => x.id);
                    table.ForeignKey(
                        name: "fk_tool_overrides_upstreams_upstream_id",
                        column: x => x.upstream_id,
                        principalTable: "upstreams",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_audit_entries_timestamp_client_name_tool_name",
                table: "audit_entries",
                columns: new[] { "timestamp", "client_name", "tool_name" });

            migrationBuilder.CreateIndex(
                name: "ix_gateway_clients_name",
                table: "gateway_clients",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_tool_overrides_upstream_id_tool_name",
                table: "tool_overrides",
                columns: new[] { "upstream_id", "tool_name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_upstreams_name",
                table: "upstreams",
                column: "name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "audit_entries");

            migrationBuilder.DropTable(
                name: "gateway_clients");

            migrationBuilder.DropTable(
                name: "tool_overrides");

            migrationBuilder.DropTable(
                name: "upstreams");
        }
    }
}
