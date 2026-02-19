using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Tenant.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Tntlss : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                schema: "Tenant",
                table: "tenantSubscriptions",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.InsertData(
                schema: "Tenant",
                table: "planProperties",
                columns: new[] { "Id", "IndividualTaskLimit", "IsInternalReportingEnabled", "PeopleAddedLimit", "TeamLimit" },
                values: new object[,]
                {
                    { new Guid("018da123-4567-7000-8000-000000000001"), 100, false, 5, 1 },
                    { new Guid("018da123-4567-7000-8000-000000000002"), 1000, false, 25, 5 },
                    { new Guid("018da123-4567-7000-8000-000000000003"), 10000, true, 1000, 50 }
                });

            migrationBuilder.InsertData(
                schema: "Tenant",
                table: "companyPlans",
                columns: new[] { "Id", "PlanName", "PlanPrice", "PlanPropertiesId", "isActive" },
                values: new object[,]
                {
                    { new Guid("018da123-abcd-7000-9000-000000000001"), "Start-up", 0, new Guid("018da123-4567-7000-8000-000000000001"), true },
                    { new Guid("018da123-abcd-7000-9000-000000000002"), "Business", 499, new Guid("018da123-4567-7000-8000-000000000002"), true },
                    { new Guid("018da123-abcd-7000-9000-000000000003"), "Enterprise", 1499, new Guid("018da123-4567-7000-8000-000000000003"), true }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "Tenant",
                table: "companyPlans",
                keyColumn: "Id",
                keyValue: new Guid("018da123-abcd-7000-9000-000000000001"));

            migrationBuilder.DeleteData(
                schema: "Tenant",
                table: "companyPlans",
                keyColumn: "Id",
                keyValue: new Guid("018da123-abcd-7000-9000-000000000002"));

            migrationBuilder.DeleteData(
                schema: "Tenant",
                table: "companyPlans",
                keyColumn: "Id",
                keyValue: new Guid("018da123-abcd-7000-9000-000000000003"));

            migrationBuilder.DeleteData(
                schema: "Tenant",
                table: "planProperties",
                keyColumn: "Id",
                keyValue: new Guid("018da123-4567-7000-8000-000000000001"));

            migrationBuilder.DeleteData(
                schema: "Tenant",
                table: "planProperties",
                keyColumn: "Id",
                keyValue: new Guid("018da123-4567-7000-8000-000000000002"));

            migrationBuilder.DeleteData(
                schema: "Tenant",
                table: "planProperties",
                keyColumn: "Id",
                keyValue: new Guid("018da123-4567-7000-8000-000000000003"));

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                schema: "Tenant",
                table: "tenantSubscriptions",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);
        }
    }
}
