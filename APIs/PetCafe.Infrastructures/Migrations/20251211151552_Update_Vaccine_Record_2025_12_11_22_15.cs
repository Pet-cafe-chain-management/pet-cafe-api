using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetCafe.Infrastructures.Migrations
{
    /// <inheritdoc />
    public partial class Update_Vaccine_Record_2025_12_11_22_15 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "batch_number",
                table: "vaccination_records");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "batch_number",
                table: "vaccination_records",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);
        }
    }
}
