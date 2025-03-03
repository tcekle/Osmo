using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Osmo.Database.Migrations.Osmo
{
    /// <inheritdoc />
    public partial class AddAlgorithmsAndJobs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Algorithms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    RelatedDeviceId = table.Column<Guid>(type: "uuid", nullable: false),
                    AlgorithmId = table.Column<string>(type: "text", nullable: true),
                    AlgorithmVersion = table.Column<string>(type: "text", nullable: true),
                    name = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Algorithms", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Jobs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    RelatedAlgorithmId = table.Column<Guid>(type: "uuid", nullable: false),
                    GivenJobId = table.Column<Guid>(type: "uuid", nullable: false),
                    JobName = table.Column<string>(type: "text", nullable: true),
                    JobDescription = table.Column<string>(type: "text", nullable: true),
                    JobChecksum = table.Column<string>(type: "text", nullable: true),
                    SettingChecksum = table.Column<string>(type: "text", nullable: true),
                    name = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jobs", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Algorithms");

            migrationBuilder.DropTable(
                name: "Jobs");
        }
    }
}
