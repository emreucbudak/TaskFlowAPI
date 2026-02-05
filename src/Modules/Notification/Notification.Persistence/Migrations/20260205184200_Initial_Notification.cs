using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Notification.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Initial_Notification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Notification");

            migrationBuilder.CreateTable(
                name: "notificationMessages",
                schema: "Notification",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    SendTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsRead = table.Column<bool>(type: "boolean", nullable: false),
                    ReceiverUserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notificationMessages", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_notificationMessages_IsRead",
                schema: "Notification",
                table: "notificationMessages",
                column: "IsRead");

            migrationBuilder.CreateIndex(
                name: "IX_notificationMessages_ReceiverUserId",
                schema: "Notification",
                table: "notificationMessages",
                column: "ReceiverUserId");

            migrationBuilder.CreateIndex(
                name: "IX_notificationMessages_ReceiverUserId_SendTime",
                schema: "Notification",
                table: "notificationMessages",
                columns: new[] { "ReceiverUserId", "SendTime" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "IX_notificationMessages_SendTime",
                schema: "Notification",
                table: "notificationMessages",
                column: "SendTime");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "notificationMessages",
                schema: "Notification");
        }
    }
}
