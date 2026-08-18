using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRManagement.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AutoGenerateEmployeeCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "employee_code_sequence");

            migrationBuilder.AlterColumn<string>(
                name: "EmployeeCode",
                table: "Employees",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValueSql: "'EMP' || LPAD(nextval('employee_code_sequence')::text, 3, '0')",
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropSequence(
                name: "employee_code_sequence");

            migrationBuilder.AlterColumn<string>(
                name: "EmployeeCode",
                table: "Employees",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldDefaultValueSql: "'EMP' || LPAD(nextval('employee_code_sequence')::text, 3, '0')");
        }
    }
}
