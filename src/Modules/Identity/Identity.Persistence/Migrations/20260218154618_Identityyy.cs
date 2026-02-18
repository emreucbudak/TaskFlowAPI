using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Identity.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Identityyy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DepartmentMemberId",
                schema: "Identity",
                table: "DepartmentMembers",
                newName: "Id");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "Identity",
                table: "DepartmentMembers",
                newName: "DepartmentMemberId");

            migrationBuilder.UpdateData(
                schema: "Identity",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-e5f6-4789-9012-34567890abcd"),
                column: "ConcurrencyStamp",
                value: "6e6750fe-a61d-4ea8-890c-87a9daa11fdf");

            migrationBuilder.UpdateData(
                schema: "Identity",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("b2c3d4e5-f678-4901-2345-67890abcdeff"),
                column: "ConcurrencyStamp",
                value: "8c28643f-5791-4291-ac81-239e8cc73f75");

            migrationBuilder.UpdateData(
                schema: "Identity",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("c3d4e5f6-7890-1234-5678-90abcdef1234"),
                column: "ConcurrencyStamp",
                value: "aa6ce9e3-c5f9-4ab6-b051-7f8128f6cce7");
        }
    }
}
