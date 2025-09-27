using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetCafe.Infrastructures.Migrations
{
    /// <inheritdoc />
    public partial class Update_VaccinationRecord_2025_09_28 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_completed",
                table: "vaccination_schedules");

            migrationBuilder.AddColumn<int>(
                name: "required_doses",
                table: "vaccine_types",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "record_id",
                table: "vaccination_schedules",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "status",
                table: "vaccination_schedules",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "schedule_id",
                table: "vaccination_records",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "i_x_vaccination_schedules_record_id",
                table: "vaccination_schedules",
                column: "record_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "f_k_vaccination_schedules_vaccination_records_record_id",
                table: "vaccination_schedules",
                column: "record_id",
                principalTable: "vaccination_records",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "f_k_vaccination_schedules_vaccination_records_record_id",
                table: "vaccination_schedules");

            migrationBuilder.DropIndex(
                name: "i_x_vaccination_schedules_record_id",
                table: "vaccination_schedules");

            migrationBuilder.DropColumn(
                name: "required_doses",
                table: "vaccine_types");

            migrationBuilder.DropColumn(
                name: "record_id",
                table: "vaccination_schedules");

            migrationBuilder.DropColumn(
                name: "status",
                table: "vaccination_schedules");

            migrationBuilder.DropColumn(
                name: "schedule_id",
                table: "vaccination_records");

            migrationBuilder.AddColumn<bool>(
                name: "is_completed",
                table: "vaccination_schedules",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
