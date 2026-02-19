using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tenant.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Tnssuys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "Tenant",
                table: "planProperties",
                keyColumn: "Id",
                keyValue: new Guid("018da123-4567-7000-8000-000000000001"),
                column: "IsInternalReportingEnabled",
                value: true);

            migrationBuilder.UpdateData(
                schema: "Tenant",
                table: "planProperties",
                keyColumn: "Id",
                keyValue: new Guid("018da123-4567-7000-8000-000000000002"),
                column: "IsInternalReportingEnabled",
                value: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "Tenant",
                table: "planProperties",
                keyColumn: "Id",
                keyValue: new Guid("018da123-4567-7000-8000-000000000001"),
                column: "IsInternalReportingEnabled",
                value: false);

            migrationBuilder.UpdateData(
                schema: "Tenant",
                table: "planProperties",
                keyColumn: "Id",
                keyValue: new Guid("018da123-4567-7000-8000-000000000002"),
                column: "IsInternalReportingEnabled",
                value: false);
        }
    }
}
