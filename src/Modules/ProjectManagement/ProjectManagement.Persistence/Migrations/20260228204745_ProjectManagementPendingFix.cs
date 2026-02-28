using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectManagement.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ProjectManagementPendingFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                DO $$
                BEGIN
                    IF NOT EXISTS (
                        SELECT 1
                        FROM information_schema.columns
                        WHERE table_schema = 'ProjectManagement'
                          AND table_name = 'IndividualTasks'
                          AND column_name = 'TaskPriorityCategoryId'
                    ) THEN
                        ALTER TABLE "ProjectManagement"."IndividualTasks"
                        ADD "TaskPriorityCategoryId" integer;
                    END IF;
                END
                $$;
                """);

            migrationBuilder.Sql(
                """
                CREATE INDEX IF NOT EXISTS "IX_IndividualTasks_TaskPriorityCategoryId"
                ON "ProjectManagement"."IndividualTasks" ("TaskPriorityCategoryId");
                """);

            migrationBuilder.Sql(
                """
                DO $$
                BEGIN
                    IF NOT EXISTS (
                        SELECT 1
                        FROM pg_constraint
                        WHERE conname = 'FK_IndividualTasks_TaskPriorityCategories_TaskPriorityCategory~'
                    ) THEN
                        ALTER TABLE "ProjectManagement"."IndividualTasks"
                        ADD CONSTRAINT "FK_IndividualTasks_TaskPriorityCategories_TaskPriorityCategory~"
                        FOREIGN KEY ("TaskPriorityCategoryId")
                        REFERENCES "ProjectManagement"."TaskPriorityCategories" ("TaskPriorityCategoryId")
                        ON DELETE SET NULL;
                    END IF;
                END
                $$;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                ALTER TABLE "ProjectManagement"."IndividualTasks"
                DROP CONSTRAINT IF EXISTS "FK_IndividualTasks_TaskPriorityCategories_TaskPriorityCategory~";
                """);

            migrationBuilder.Sql(
                """
                DROP INDEX IF EXISTS "ProjectManagement"."IX_IndividualTasks_TaskPriorityCategoryId";
                """);

            migrationBuilder.Sql(
                """
                ALTER TABLE "ProjectManagement"."IndividualTasks"
                DROP COLUMN IF EXISTS "TaskPriorityCategoryId";
                """);
        }
    }
}
