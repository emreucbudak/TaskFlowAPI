using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskFlow.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "companiesPlan",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PlanName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_companiesPlan", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "messages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MessageText = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_messages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "taskPriorityCategories",
                columns: table => new
                {
                    TaskPriorityCategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    CategoryName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_taskPriorityCategories", x => x.TaskPriorityCategoryId);
                });

            migrationBuilder.CreateTable(
                name: "taskStatuses",
                columns: table => new
                {
                    TaskStatusId = table.Column<Guid>(type: "uuid", nullable: false),
                    StatusName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_taskStatuses", x => x.TaskStatusId);
                });

            migrationBuilder.CreateTable(
                name: "companies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CompanyName = table.Column<string>(type: "text", nullable: false),
                    CompanyPlanId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_companies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_companies_companiesPlan_CompanyPlanId",
                        column: x => x.CompanyPlanId,
                        principalTable: "companiesPlan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "planProperties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PeopleAddedLimit = table.Column<int>(type: "integer", nullable: false),
                    IsIncludeGroupChat = table.Column<bool>(type: "boolean", nullable: false),
                    IsIncludeVideoCall = table.Column<bool>(type: "boolean", nullable: false),
                    TeamLimit = table.Column<int>(type: "integer", nullable: false),
                    IsDailyPlannerEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    IsIncludeTaskPriorityCategory = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeadlineNotificationEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    IsIncludeAddTaskNotifications = table.Column<bool>(type: "boolean", nullable: false),
                    CompanyPlanId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_planProperties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_planProperties_companiesPlan_CompanyPlanId",
                        column: x => x.CompanyPlanId,
                        principalTable: "companiesPlan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TaskName = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    TaskStatusId = table.Column<Guid>(type: "uuid", nullable: false),
                    DeadlineTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TaskPriorityCategoryId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tasks_taskPriorityCategories_TaskPriorityCategoryId",
                        column: x => x.TaskPriorityCategoryId,
                        principalTable: "taskPriorityCategories",
                        principalColumn: "TaskPriorityCategoryId");
                    table.ForeignKey(
                        name: "FK_tasks_taskStatuses_TaskStatusId",
                        column: x => x.TaskStatusId,
                        principalTable: "taskStatuses",
                        principalColumn: "TaskStatusId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "taskAnswers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AnswerText = table.Column<string>(type: "text", nullable: false),
                    TaskId = table.Column<int>(type: "integer", nullable: false),
                    TaskId1 = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_taskAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_taskAnswers_tasks_TaskId1",
                        column: x => x.TaskId1,
                        principalTable: "tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_companies_CompanyPlanId",
                table: "companies",
                column: "CompanyPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_planProperties_CompanyPlanId",
                table: "planProperties",
                column: "CompanyPlanId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_taskAnswers_TaskId1",
                table: "taskAnswers",
                column: "TaskId1");

            migrationBuilder.CreateIndex(
                name: "IX_tasks_TaskPriorityCategoryId",
                table: "tasks",
                column: "TaskPriorityCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_tasks_TaskStatusId",
                table: "tasks",
                column: "TaskStatusId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "companies");

            migrationBuilder.DropTable(
                name: "messages");

            migrationBuilder.DropTable(
                name: "planProperties");

            migrationBuilder.DropTable(
                name: "taskAnswers");

            migrationBuilder.DropTable(
                name: "companiesPlan");

            migrationBuilder.DropTable(
                name: "tasks");

            migrationBuilder.DropTable(
                name: "taskPriorityCategories");

            migrationBuilder.DropTable(
                name: "taskStatuses");
        }
    }
}
