using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Identity.Persistence.Migrations
{
    public partial class AddGroupEvents : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GroupEvents",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Subject = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    EventType = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    StartsAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndsAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    MeetingLink = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    GroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupEvents_AspNetUsers_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalSchema: "Identity",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GroupEvents_Groups_GroupId",
                        column: x => x.GroupId,
                        principalSchema: "Identity",
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GroupEvents_CreatedByUserId",
                schema: "Identity",
                table: "GroupEvents",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupEvents_GroupId_StartsAt",
                schema: "Identity",
                table: "GroupEvents",
                columns: new[] { "GroupId", "StartsAt" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GroupEvents",
                schema: "Identity");
        }
    }
}
