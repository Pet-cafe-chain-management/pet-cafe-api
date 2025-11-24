using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetCafe.Infrastructures.Migrations
{
    /// <inheritdoc />
    public partial class Update_VaccineSchedule_2025_11_24_21_28 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "f_k_daily_tasks_vaccination_schedules_vaccination_schedule_id",
                table: "daily_tasks");

            migrationBuilder.DropIndex(
                name: "i_x_daily_tasks_vaccination_schedule_id",
                table: "daily_tasks");

            migrationBuilder.AddColumn<Guid>(
                name: "daily_task_id",
                table: "vaccination_schedules",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "i_x_vaccination_schedules_daily_task_id",
                table: "vaccination_schedules",
                column: "daily_task_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "f_k_vaccination_schedules_daily_tasks_daily_task_id",
                table: "vaccination_schedules",
                column: "daily_task_id",
                principalTable: "daily_tasks",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "f_k_vaccination_schedules_daily_tasks_daily_task_id",
                table: "vaccination_schedules");

            migrationBuilder.DropIndex(
                name: "i_x_vaccination_schedules_daily_task_id",
                table: "vaccination_schedules");

            migrationBuilder.DropColumn(
                name: "daily_task_id",
                table: "vaccination_schedules");

            migrationBuilder.CreateIndex(
                name: "i_x_daily_tasks_vaccination_schedule_id",
                table: "daily_tasks",
                column: "vaccination_schedule_id");

            migrationBuilder.AddForeignKey(
                name: "f_k_daily_tasks_vaccination_schedules_vaccination_schedule_id",
                table: "daily_tasks",
                column: "vaccination_schedule_id",
                principalTable: "vaccination_schedules",
                principalColumn: "id");
        }
    }
}
