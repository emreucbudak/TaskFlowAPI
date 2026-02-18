using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Identity.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Identityyyies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "Identity",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-e5f6-4789-9012-34567890abcd"),
                column: "ConcurrencyStamp",
                value: "c8f1c3b2-e4a5-4b6c-8d7e-9f0a1b2c3d4e");

            migrationBuilder.UpdateData(
                schema: "Identity",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("b2c3d4e5-f678-4901-2345-67890abcdeff"),
                column: "ConcurrencyStamp",
                value: "d9g2d4c3-f5b6-5c7d-9e8f-0g1b2c3d4e5f");

            migrationBuilder.UpdateData(
                schema: "Identity",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("c3d4e5f6-7890-1234-5678-90abcdef1234"),
                column: "ConcurrencyStamp",
                value: "e0h3e5d4-g6c7-6d8e-0f9g-1h2c3d4e5f6g");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "Identity",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-e5f6-4789-9012-34567890abcd"),
                column: "ConcurrencyStamp",
                value: "4d564a41-b433-41da-9133-01791a0dae7a");

            migrationBuilder.UpdateData(
                schema: "Identity",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("b2c3d4e5-f678-4901-2345-67890abcdeff"),
                column: "ConcurrencyStamp",
                value: "49c447e9-7cb8-431e-9157-1a3c6488176b");

            migrationBuilder.UpdateData(
                schema: "Identity",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("c3d4e5f6-7890-1234-5678-90abcdef1234"),
                column: "ConcurrencyStamp",
                value: "c89fa4dd-6f20-443e-b849-2284ba00c17d");
        }
    }
}
