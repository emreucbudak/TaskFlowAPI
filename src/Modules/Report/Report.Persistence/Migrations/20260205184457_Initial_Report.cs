using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Report.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Initial_Report : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Report");

            migrationBuilder.CreateTable(
                name: "ReportTopics",
                schema: "Report",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TopicName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportTopics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Reports",
                schema: "Report",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ReportTopicId = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reports_ReportTopics_ReportTopicId",
                        column: x => x.ReportTopicId,
                        principalSchema: "Report",
                        principalTable: "ReportTopics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "Report",
                table: "ReportTopics",
                columns: new[] { "Id", "TopicName" },
                values: new object[,]
                {
                    { 1, "Hata Bildirimi" },
                    { 2, "Geri Bildirim" },
                    { 3, "Diğer" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reports_CreatedAt",
                schema: "Report",
                table: "Reports",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_ReportTopicId",
                schema: "Report",
                table: "Reports",
                column: "ReportTopicId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_UserId",
                schema: "Report",
                table: "Reports",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Reports",
                schema: "Report");

            migrationBuilder.DropTable(
                name: "ReportTopics",
                schema: "Report");
        }
    }
}
