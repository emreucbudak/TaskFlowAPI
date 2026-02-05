using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stats.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Initial_Stats : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Stats");

            migrationBuilder.CreateTable(
                name: "UserStats",
                schema: "Stats",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Period = table.Column<DateOnly>(type: "date", nullable: false),
                    TotalTasksAssigned = table.Column<int>(type: "integer", nullable: false),
                    TotalTasksCompleted = table.Column<int>(type: "integer", nullable: false),
                    TasksCompletedBeforeDeadline = table.Column<int>(type: "integer", nullable: false),
                    OverdueIncompleteTasksCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserStats", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserStats_UserId_Period",
                schema: "Stats",
                table: "UserStats",
                columns: new[] { "UserId", "Period" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserStats",
                schema: "Stats");
        }
    }
}
