using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetCafe.Infrastructures.Migrations
{
    /// <inheritdoc />
    public partial class Update_EmployeeSchedule_2025_10_19_03_45 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "i_x_employee_schedules_employee_id_work_date",
                table: "employee_schedules");

            migrationBuilder.DropIndex(
                name: "i_x_employee_schedules_work_date",
                table: "employee_schedules");

            migrationBuilder.DropColumn(
                name: "check_in_time",
                table: "employee_schedules");

            migrationBuilder.DropColumn(
                name: "check_out_time",
                table: "employee_schedules");

            migrationBuilder.DropColumn(
                name: "notes",
                table: "employee_schedules");

            migrationBuilder.DropColumn(
                name: "status",
                table: "employee_schedules");

            migrationBuilder.DropColumn(
                name: "work_date",
                table: "employee_schedules");

            migrationBuilder.CreateIndex(
                name: "i_x_employee_schedules_employee_id",
                table: "employee_schedules",
                column: "employee_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "i_x_employee_schedules_employee_id",
                table: "employee_schedules");

            migrationBuilder.AddColumn<DateTime>(
                name: "check_in_time",
                table: "employee_schedules",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "check_out_time",
                table: "employee_schedules",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "notes",
                table: "employee_schedules",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "status",
                table: "employee_schedules",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Scheduled");

            migrationBuilder.AddColumn<DateTime>(
                name: "work_date",
                table: "employee_schedules",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "i_x_employee_schedules_employee_id_work_date",
                table: "employee_schedules",
                columns: new[] { "employee_id", "work_date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_employee_schedules_work_date",
                table: "employee_schedules",
                column: "work_date");
        }
    }
}
