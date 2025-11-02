using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetCafe.Infrastructures.Migrations
{
    /// <inheritdoc />
    public partial class Update_Pet_2025_11_02_23_42 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "health_status",
                table: "pets",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "HEALTHY");

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "pets",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "health_status",
                table: "pets");

            migrationBuilder.DropColumn(
                name: "is_active",
                table: "pets");
        }
    }
}
