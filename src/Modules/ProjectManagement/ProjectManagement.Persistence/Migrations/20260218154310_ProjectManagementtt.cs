using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectManagement.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ProjectManagementtt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TaskAnswers",
                schema: "ProjectManagement");

            migrationBuilder.CreateTable(
                name: "IndividualTasks",
                schema: "ProjectManagement",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignedUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    TaskTitle = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: false),
                    Deadline = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IndividualTasks", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IndividualTasks",
                schema: "ProjectManagement");

            migrationBuilder.CreateTable(
                name: "TaskAnswers",
                schema: "ProjectManagement",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AnswerText = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SenderId = table.Column<Guid>(type: "uuid", nullable: false),
                    TaskId = table.Column<Guid>(type: "uuid", nullable: false)
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
        }
    }
}
