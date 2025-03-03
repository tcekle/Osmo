using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Osmo.ConneX.Migrations.ConneXMetrics
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "programming_statistics",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    related_device_id = table.Column<Guid>(type: "uuid", nullable: false),
                    related_job_id = table.Column<Guid>(type: "uuid", nullable: false),
                    related_programmer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<int>(type: "integer", nullable: false),
                    code_name = table.Column<string>(type: "text", nullable: true),
                    program_duration = table.Column<int>(type: "integer", nullable: false),
                    verify_duration = table.Column<int>(type: "integer", nullable: false),
                    blank_check_duration = table.Column<int>(type: "integer", nullable: false),
                    erase_duration = table.Column<int>(type: "integer", nullable: false),
                    bytes_programmed = table.Column<long>(type: "bigint", nullable: false),
                    overhead = table.Column<int>(type: "integer", nullable: false),
                    overall = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_programming_statistics", x => new { x.id, x.timestamp });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "programming_statistics");
        }
    }
}
