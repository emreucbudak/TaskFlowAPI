using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Pgvector;

#nullable disable

namespace Assistant.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialAssistantKnowledge : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("CREATE EXTENSION IF NOT EXISTS vector;");

            migrationBuilder.CreateTable(
                name: "rag_documents",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    source_key = table.Column<string>(type: "text", nullable: false),
                    title = table.Column<string>(type: "text", nullable: false),
                    checksum = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rag_documents", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "rag_chunks",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    document_id = table.Column<Guid>(type: "uuid", nullable: false),
                    chunk_index = table.Column<int>(type: "integer", nullable: false),
                    chunk_text = table.Column<string>(type: "text", nullable: false),
                    embedding = table.Column<Vector>(type: "vector(1536)", nullable: false),
                    metadata = table.Column<string>(type: "jsonb", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rag_chunks", x => x.id);
                    table.ForeignKey(
                        name: "FK_rag_chunks_rag_documents_document_id",
                        column: x => x.document_id,
                        principalTable: "rag_documents",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_rag_chunks_document_id",
                table: "rag_chunks",
                column: "document_id");

            migrationBuilder.CreateIndex(
                name: "uq_rag_chunks_document_chunk",
                table: "rag_chunks",
                columns: new[] { "document_id", "chunk_index" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uq_rag_documents_source_key",
                table: "rag_documents",
                column: "source_key",
                unique: true);

            migrationBuilder.Sql("CREATE INDEX IF NOT EXISTS ix_rag_chunks_embedding ON rag_chunks USING hnsw (embedding vector_cosine_ops);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP INDEX IF EXISTS ix_rag_chunks_embedding;");

            migrationBuilder.DropTable(
                name: "rag_chunks");

            migrationBuilder.DropTable(
                name: "rag_documents");
        }
    }
}
