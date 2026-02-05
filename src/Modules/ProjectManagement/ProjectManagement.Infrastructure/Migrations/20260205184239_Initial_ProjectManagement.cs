using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ProjectManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial_ProjectManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "ProjectManagement");

            migrationBuilder.CreateTable(
                name: "TaskPriorityCategories",
                schema: "ProjectManagement",
                columns: table => new
                {
                    TaskPriorityCategoryId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CategoryName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskPriorityCategories", x => x.TaskPriorityCategoryId);
                });

            migrationBuilder.CreateTable(
                name: "TaskStatuses",
                schema: "ProjectManagement",
                columns: table => new
                {
                    TaskStatusId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StatusName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskStatuses", x => x.TaskStatusId);
                });

            migrationBuilder.CreateTable(
                name: "Tasks",
                schema: "ProjectManagement",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TaskName = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    TaskStatusId = table.Column<int>(type: "integer", nullable: false),
                    DeadlineTime = table.Column<DateOnly>(type: "date", nullable: false),
                    TaskPriorityCategoryId = table.Column<int>(type: "integer", nullable: true),
                    CreatedDate = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tasks_TaskPriorityCategories_TaskPriorityCategoryId",
                        column: x => x.TaskPriorityCategoryId,
                        principalSchema: "ProjectManagement",
                        principalTable: "TaskPriorityCategories",
                        principalColumn: "TaskPriorityCategoryId");
                    table.ForeignKey(
                        name: "FK_Tasks_TaskStatuses_TaskStatusId",
                        column: x => x.TaskStatusId,
                        principalSchema: "ProjectManagement",
                        principalTable: "TaskStatuses",
                        principalColumn: "TaskStatusId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Subtasks",
                schema: "ProjectManagement",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TaskTitle = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    AssignedUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    TaskStatusId = table.Column<int>(type: "integer", nullable: false),
                    TaskId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subtasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subtasks_TaskStatuses_TaskStatusId",
                        column: x => x.TaskStatusId,
                        principalSchema: "ProjectManagement",
                        principalTable: "TaskStatuses",
                        principalColumn: "TaskStatusId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Subtasks_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalSchema: "ProjectManagement",
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaskAnswers",
                schema: "ProjectManagement",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AnswerText = table.Column<string>(type: "text", nullable: false),
                    SenderId = table.Column<Guid>(type: "uuid", nullable: false),
                    TaskId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskAnswers_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalSchema: "ProjectManagement",
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubTaskAnswers",
                schema: "ProjectManagement",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AnswerText = table.Column<string>(type: "text", nullable: false),
                    SenderId = table.Column<Guid>(type: "uuid", nullable: false),
                    SubtaskId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubTaskAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubTaskAnswers_Subtasks_SubtaskId",
                        column: x => x.SubtaskId,
                        principalSchema: "ProjectManagement",
                        principalTable: "Subtasks",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubTaskAnswers_SubtaskId",
                schema: "ProjectManagement",
                table: "SubTaskAnswers",
                column: "SubtaskId");

            migrationBuilder.CreateIndex(
                name: "IX_Subtasks_AssignedUserId",
                schema: "ProjectManagement",
                table: "Subtasks",
                column: "AssignedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Subtasks_TaskId",
                schema: "ProjectManagement",
                table: "Subtasks",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_Subtasks_TaskStatusId",
                schema: "ProjectManagement",
                table: "Subtasks",
                column: "TaskStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskAnswers_CreatedDate",
                schema: "ProjectManagement",
                table: "TaskAnswers",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_TaskAnswers_SenderId",
                schema: "ProjectManagement",
                table: "TaskAnswers",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskAnswers_TaskId",
                schema: "ProjectManagement",
                table: "TaskAnswers",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_CreatedDate",
                schema: "ProjectManagement",
                table: "Tasks",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_DeadlineTime",
                schema: "ProjectManagement",
                table: "Tasks",
                column: "DeadlineTime");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_TaskPriorityCategoryId",
                schema: "ProjectManagement",
                table: "Tasks",
                column: "TaskPriorityCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_TaskStatusId",
                schema: "ProjectManagement",
                table: "Tasks",
                column: "TaskStatusId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubTaskAnswers",
                schema: "ProjectManagement");

            migrationBuilder.DropTable(
                name: "TaskAnswers",
                schema: "ProjectManagement");

            migrationBuilder.DropTable(
                name: "Subtasks",
                schema: "ProjectManagement");

            migrationBuilder.DropTable(
                name: "Tasks",
                schema: "ProjectManagement");

            migrationBuilder.DropTable(
                name: "TaskPriorityCategories",
                schema: "ProjectManagement");

            migrationBuilder.DropTable(
                name: "TaskStatuses",
                schema: "ProjectManagement");
        }
    }
}
