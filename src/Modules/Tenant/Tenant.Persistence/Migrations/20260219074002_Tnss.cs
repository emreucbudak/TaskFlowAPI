using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tenant.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Tnss : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "Tenant",
                table: "companyPlans",
                keyColumn: "Id",
                keyValue: new Guid("018da123-abcd-7000-9000-000000000001"),
                column: "PlanPrice",
                value: 500);

            migrationBuilder.UpdateData(
                schema: "Tenant",
                table: "companyPlans",
                keyColumn: "Id",
                keyValue: new Guid("018da123-abcd-7000-9000-000000000002"),
                column: "PlanPrice",
                value: 1000);

            migrationBuilder.UpdateData(
                schema: "Tenant",
                table: "companyPlans",
                keyColumn: "Id",
                keyValue: new Guid("018da123-abcd-7000-9000-000000000003"),
                column: "PlanPrice",
                value: 1500);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "Tenant",
                table: "companyPlans",
                keyColumn: "Id",
                keyValue: new Guid("018da123-abcd-7000-9000-000000000001"),
                column: "PlanPrice",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "Tenant",
                table: "companyPlans",
                keyColumn: "Id",
                keyValue: new Guid("018da123-abcd-7000-9000-000000000002"),
                column: "PlanPrice",
                value: 499);

            migrationBuilder.UpdateData(
                schema: "Tenant",
                table: "companyPlans",
                keyColumn: "Id",
                keyValue: new Guid("018da123-abcd-7000-9000-000000000003"),
                column: "PlanPrice",
                value: 1499);
        }
    }
}
