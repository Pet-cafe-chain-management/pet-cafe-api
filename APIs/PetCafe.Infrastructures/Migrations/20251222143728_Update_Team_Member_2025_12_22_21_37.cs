using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetCafe.Infrastructures.Migrations
{
    /// <inheritdoc />
    public partial class Update_Team_Member_2025_12_22_21_37 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_out_team",
                table: "daily_schedules");

            migrationBuilder.AddColumn<bool>(
                name: "is_out_team",
                table: "team_members",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_out_team",
                table: "team_members");

            migrationBuilder.AddColumn<bool>(
                name: "is_out_team",
                table: "daily_schedules",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
