using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetCafe.Infrastructures.Migrations
{
    /// <inheritdoc />
    public partial class Update_Team_2025_11_02_02_33 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_active",
                table: "work_shifts");

            migrationBuilder.DropColumn(
                name: "is_active",
                table: "team_members");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "work_shifts",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "team_members",
                type: "boolean",
                nullable: false,
                defaultValue: true);
        }
    }
}
