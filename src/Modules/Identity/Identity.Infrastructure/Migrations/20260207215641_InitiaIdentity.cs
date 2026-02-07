using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Identity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitiaIdentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Departments_DepartmentId",
                schema: "Identity",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_DepartmentId",
                schema: "Identity",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                schema: "Identity",
                table: "AspNetUsers");

            migrationBuilder.CreateTable(
                name: "DepartmentRoles",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepartmentRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DepartmentMembers",
                schema: "Identity",
                columns: table => new
                {
                    DepartmentMemberId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DepartmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    DepartmentRoleId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepartmentMembers", x => x.DepartmentMemberId);
                    table.ForeignKey(
                        name: "FK_DepartmentMembers_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "Identity",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DepartmentMembers_DepartmentRoles_DepartmentRoleId",
                        column: x => x.DepartmentRoleId,
                        principalSchema: "Identity",
                        principalTable: "DepartmentRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DepartmentMembers_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalSchema: "Identity",
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "Identity",
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("a1b2c3d4-e5f6-4789-9012-34567890abcd"), "6e6750fe-a61d-4ea8-890c-87a9daa11fdf", "Admin", "ADMIN" },
                    { new Guid("b2c3d4e5-f678-4901-2345-67890abcdeff"), "8c28643f-5791-4291-ac81-239e8cc73f75", "Company", "COMPANY" },
                    { new Guid("c3d4e5f6-7890-1234-5678-90abcdef1234"), "aa6ce9e3-c5f9-4ab6-b051-7f8128f6cce7", "Worker", "WORKER" }
                });

            migrationBuilder.InsertData(
                schema: "Identity",
                table: "DepartmentRoles",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Lider" },
                    { 2, "Yardımcı Lider" },
                    { 3, "Çalışan" }
                });

            migrationBuilder.InsertData(
                schema: "Identity",
                table: "GroupRoles",
                columns: new[] { "GroupRolesId", "RoleName" },
                values: new object[,]
                {
                    { 1, "Leader" },
                    { 2, "User" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentMembers_DepartmentId",
                schema: "Identity",
                table: "DepartmentMembers",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentMembers_DepartmentRoleId",
                schema: "Identity",
                table: "DepartmentMembers",
                column: "DepartmentRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentMembers_UserId",
                schema: "Identity",
                table: "DepartmentMembers",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DepartmentMembers",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "DepartmentRoles",
                schema: "Identity");

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-e5f6-4789-9012-34567890abcd"));

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("b2c3d4e5-f678-4901-2345-67890abcdeff"));

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("c3d4e5f6-7890-1234-5678-90abcdef1234"));

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "GroupRoles",
                keyColumn: "GroupRolesId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "GroupRoles",
                keyColumn: "GroupRolesId",
                keyValue: 2);

            migrationBuilder.AddColumn<Guid>(
                name: "DepartmentId",
                schema: "Identity",
                table: "AspNetUsers",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_DepartmentId",
                schema: "Identity",
                table: "AspNetUsers",
                column: "DepartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Departments_DepartmentId",
                schema: "Identity",
                table: "AspNetUsers",
                column: "DepartmentId",
                principalSchema: "Identity",
                principalTable: "Departments",
                principalColumn: "Id");
        }
    }
}
