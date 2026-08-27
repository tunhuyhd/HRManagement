using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRManagement.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailOfEmployee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "email",
                table: "employees",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "email",
                table: "employees");
        }
    }
}
