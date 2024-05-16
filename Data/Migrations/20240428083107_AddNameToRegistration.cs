using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trion.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddNameToRegistration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RegistrationName",
                table: "Registration",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RegistrationName",
                table: "Registration");
        }
    }
}
