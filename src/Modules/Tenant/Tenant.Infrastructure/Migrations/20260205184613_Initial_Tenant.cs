using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tenant.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial_Tenant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Tenant");

            migrationBuilder.CreateTable(
                name: "planProperties",
                schema: "Tenant",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PeopleAddedLimit = table.Column<int>(type: "integer", nullable: false),
                    TeamLimit = table.Column<int>(type: "integer", nullable: false),
                    IsDailyPlannerEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    IsIncludeTaskPriorityCategory = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeadlineNotificationEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    IsIncludeAddTaskNotifications = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_planProperties", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "companyPlans",
                schema: "Tenant",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PlanName = table.Column<string>(type: "text", nullable: false),
                    PlanPropertiesId = table.Column<Guid>(type: "uuid", nullable: false),
                    PlanPrice = table.Column<int>(type: "integer", nullable: false),
                    isActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_companyPlans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_companyPlans_planProperties_PlanPropertiesId",
                        column: x => x.PlanPropertiesId,
                        principalSchema: "Tenant",
                        principalTable: "planProperties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_companyPlans_PlanPropertiesId",
                schema: "Tenant",
                table: "companyPlans",
                column: "PlanPropertiesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "companyPlans",
                schema: "Tenant");

            migrationBuilder.DropTable(
                name: "planProperties",
                schema: "Tenant");
        }
    }
}
