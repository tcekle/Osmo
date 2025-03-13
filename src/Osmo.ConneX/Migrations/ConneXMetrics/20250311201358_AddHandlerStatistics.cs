using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Osmo.ConneX.Migrations.ConneXMetrics
{
    /// <inheritdoc />
    public partial class AddHandlerStatistics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "handler_statistics",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    related_message_id = table.Column<Guid>(type: "uuid", nullable: false),
                    related_handler_id = table.Column<string>(type: "text", nullable: true),
                    session_id = table.Column<string>(type: "text", nullable: true),
                    total_pass = table.Column<int>(type: "integer", nullable: false),
                    total_fail = table.Column<int>(type: "integer", nullable: false),
                    uph = table.Column<int>(type: "integer", nullable: false),
                    system_yield = table.Column<double>(type: "double precision", nullable: false),
                    handler_yield = table.Column<double>(type: "double precision", nullable: false),
                    programmer_yield = table.Column<double>(type: "double precision", nullable: false),
                    devices_failed_on_programmer = table.Column<int>(type: "integer", nullable: false),
                    devices_picked_input = table.Column<int>(type: "integer", nullable: false),
                    devices_failed_on_laser = table.Column<int>(type: "integer", nullable: false),
                    devices_failed_on_3d_system = table.Column<int>(type: "integer", nullable: false),
                    devices_failed_vision = table.Column<int>(type: "integer", nullable: false),
                    devices_failed_rest = table.Column<int>(type: "integer", nullable: false),
                    job_processing_time = table.Column<int>(type: "integer", nullable: false),
                    job_assistance_time = table.Column<int>(type: "integer", nullable: false),
                    job_completion_estimate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_handler_statistics", x => new { x.id, x.timestamp });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "handler_statistics");
        }
    }
}
