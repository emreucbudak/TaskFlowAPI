using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tenant.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Tenantttts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Tenant");

            migrationBuilder.CreateTable(
                name: "paymentTransactions",
                schema: "Tenant",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantSubscriptionId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExternalTransactionId = table.Column<string>(type: "text", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Currency = table.Column<string>(type: "text", nullable: false),
                    IsSuccess = table.Column<bool>(type: "boolean", nullable: false),
                    FailureMessage = table.Column<string>(type: "text", nullable: true),
                    ProcessedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_paymentTransactions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "planProperties",
                schema: "Tenant",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PeopleAddedLimit = table.Column<int>(type: "integer", nullable: false),
                    TeamLimit = table.Column<int>(type: "integer", nullable: false),
                    IndividualTaskLimit = table.Column<int>(type: "integer", nullable: false),
                    IsInternalReportingEnabled = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_planProperties", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tenantUsages",
                schema: "Tenant",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrentUserCount = table.Column<int>(type: "integer", nullable: false),
                    CurrentTaskCount = table.Column<int>(type: "integer", nullable: false),
                    CurrentGroupCount = table.Column<int>(type: "integer", nullable: false),
                    CurrentIndividualTaskCount = table.Column<int>(type: "integer", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenantUsages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "companyPlans",
                schema: "Tenant",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PlanName = table.Column<string>(type: "text", nullable: false),
                    PlanPropertiesId = table.Column<Guid>(type: "uuid", nullable: false),
                    PlanPrice = table.Column<int>(type: "integer", nullable: false),
                    isActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_companyPlans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_companyPlans_planProperties_PlanPropertiesId",
                        column: x => x.PlanPropertiesId,
                        principalSchema: "Tenant",
                        principalTable: "planProperties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tenantSubscriptions",
                schema: "Tenant",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    CompanyPlanId = table.Column<Guid>(type: "uuid", nullable: false),
                    PaymentProviderSubscriptionId = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    TenantUsageId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    NextBillingDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CanceledAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenantSubscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tenantSubscriptions_companyPlans_CompanyPlanId",
                        column: x => x.CompanyPlanId,
                        principalSchema: "Tenant",
                        principalTable: "companyPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tenantSubscriptions_tenantUsages_TenantUsageId",
                        column: x => x.TenantUsageId,
                        principalSchema: "Tenant",
                        principalTable: "tenantUsages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_companyPlans_PlanPropertiesId",
                schema: "Tenant",
                table: "companyPlans",
                column: "PlanPropertiesId");

            migrationBuilder.CreateIndex(
                name: "IX_tenantSubscriptions_CompanyPlanId",
                schema: "Tenant",
                table: "tenantSubscriptions",
                column: "CompanyPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_tenantSubscriptions_TenantUsageId",
                schema: "Tenant",
                table: "tenantSubscriptions",
                column: "TenantUsageId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tenantUsages_TenantId",
                schema: "Tenant",
                table: "tenantUsages",
                column: "TenantId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "paymentTransactions",
                schema: "Tenant");

            migrationBuilder.DropTable(
                name: "tenantSubscriptions",
                schema: "Tenant");

            migrationBuilder.DropTable(
                name: "companyPlans",
                schema: "Tenant");

            migrationBuilder.DropTable(
                name: "tenantUsages",
                schema: "Tenant");

            migrationBuilder.DropTable(
                name: "planProperties",
                schema: "Tenant");
        }
    }
}
