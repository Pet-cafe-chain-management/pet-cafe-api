using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetCafe.Infrastructures.Migrations
{
    /// <inheritdoc />
    public partial class Update_Team_Member_2025_12_10_22_47 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "i_x_team_members_team_id_employee_id",
                table: "team_members");

            migrationBuilder.CreateIndex(
                name: "i_x_team_members_team_id",
                table: "team_members",
                column: "team_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "i_x_team_members_team_id",
                table: "team_members");

            migrationBuilder.CreateIndex(
                name: "i_x_team_members_team_id_employee_id",
                table: "team_members",
                columns: new[] { "team_id", "employee_id" },
                unique: true);
        }
    }
}
