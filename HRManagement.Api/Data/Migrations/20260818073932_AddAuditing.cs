using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRManagement.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "updated_at_utc",
                table: "employees",
                newName: "last_modified_at_utc");

            migrationBuilder.AddColumn<Guid>(
                name: "last_modified_by",
                table: "employees",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "audit_logs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    table_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    record_id = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    action = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    changed_columns = table.Column<string>(type: "jsonb", nullable: false),
                    old_values = table.Column<string>(type: "jsonb", nullable: true),
                    new_values = table.Column<string>(type: "jsonb", nullable: true),
                    changed_by = table.Column<Guid>(type: "uuid", nullable: true),
                    changed_by_email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    changed_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_audit_logs", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_audit_logs_changed_at_utc",
                table: "audit_logs",
                column: "changed_at_utc");

            migrationBuilder.CreateIndex(
                name: "ix_audit_logs_table_name_record_id",
                table: "audit_logs",
                columns: new[] { "table_name", "record_id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "audit_logs");

            migrationBuilder.DropColumn(
                name: "last_modified_by",
                table: "employees");

            migrationBuilder.RenameColumn(
                name: "last_modified_at_utc",
                table: "employees",
                newName: "updated_at_utc");
        }
    }
}
