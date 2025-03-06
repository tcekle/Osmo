using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Osmo.ConneX.Migrations.ConneXMetrics
{
    /// <inheritdoc />
    public partial class AddMqttMessages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "mqtt_messages",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    topic = table.Column<string>(type: "text", nullable: true),
                    content_type = table.Column<string>(type: "text", nullable: true),
                    payload = table.Column<byte[]>(type: "bytea", nullable: true),
                    payload_as_string = table.Column<string>(type: "text", nullable: true),
                    timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mqtt_messages", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "mqtt_messages");
        }
    }
}
