using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Report.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Reporttt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserId",
                schema: "Report",
                table: "Reports",
                newName: "ReportingUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Reports_UserId",
                schema: "Report",
                table: "Reports",
                newName: "IX_Reports_ReportingUserId");

            migrationBuilder.AddColumn<Guid>(
                name: "NotifiedDepartmantId",
                schema: "Report",
                table: "Reports",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "ReportStatusId",
                schema: "Report",
                table: "Reports",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                schema: "Report",
                table: "Reports",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "ReportStatuses",
                schema: "Report",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportStatuses", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reports_ReportStatusId",
                schema: "Report",
                table: "Reports",
                column: "ReportStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_ReportStatuses_ReportStatusId",
                schema: "Report",
                table: "Reports",
                column: "ReportStatusId",
                principalSchema: "Report",
                principalTable: "ReportStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reports_ReportStatuses_ReportStatusId",
                schema: "Report",
                table: "Reports");

            migrationBuilder.DropTable(
                name: "ReportStatuses",
                schema: "Report");

            migrationBuilder.DropIndex(
                name: "IX_Reports_ReportStatusId",
                schema: "Report",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "NotifiedDepartmantId",
                schema: "Report",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "ReportStatusId",
                schema: "Report",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "Title",
                schema: "Report",
                table: "Reports");

            migrationBuilder.RenameColumn(
                name: "ReportingUserId",
                schema: "Report",
                table: "Reports",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Reports_ReportingUserId",
                schema: "Report",
                table: "Reports",
                newName: "IX_Reports_UserId");
        }
    }
}
