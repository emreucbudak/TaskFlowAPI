using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stats.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkerStatsTotalPoints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TotalPoints",
                schema: "Stats",
                table: "UserStats",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalPoints",
                schema: "Stats",
                table: "UserStats");
        }
    }
}
